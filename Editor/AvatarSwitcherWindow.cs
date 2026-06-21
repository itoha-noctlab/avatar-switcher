#if AKATSUKIYA_VRCSDK3_AVATARS

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using AKATSUKIYA.AvatarSwitcher.Localization;
using VRC.SDK3.Avatars.Components;

namespace AKATSUKIYA.AvatarSwitcher.Editor
{
    public sealed class AvatarSwitcherWindow : EditorWindow
    {
        private const string MenuPath = "Tools/AKATSUKIYA/Avatar Switcher";
        private const string MenuPathWithHotkey = MenuPath + " #a";
        private const string WindowIconFileName = "icon.png";
        private static string WindowTitle => Localized.UI.WindowTitle;
        private static string SettingsMenuLabel => Localized.UI.SettingsMenuLabel;
        private static string EmptyStateMessage => Localized.UI.EmptyStateMessage;
        private static string SettingsWindowTitle => Localized.UI.SettingsWindowTitle;
        private static string HierarchyHighlightLabel => Localized.UI.HierarchyHighlightLabel;
        private static string HierarchyHighlightAltNote => Localized.UI.HierarchyHighlightAltNote;
        private static string SceneNameFallback => Localized.UI.SceneNameFallback;
        private static string UndoSelectAvatarRange => Localized.Message.UndoSelectAvatarRange;
        private static string UndoSwitchAvatar => Localized.Message.UndoSwitchAvatar;
        private static string UndoDeselectAvatar => Localized.Message.UndoDeselectAvatar;
        private const float InitialWindowPaddingX = 32f;
        private const float InitialWindowPaddingY = 24f;
        private const float MinimumInitialWindowWidth = 320f;
        private const float MinimumInitialWindowHeight = 150f;
        private const double RefreshDelaySeconds = 0.12d;
        private const float PathLabelMaxWidthRatio = 0.55f;

        private static readonly Color SelectedRowColorForDarkSkin = new(0.18f, 0.36f, 0.58f, 0.55f);
        private static readonly Color SelectedRowColorForLightSkin = new(0.55f, 0.78f, 1f, 0.70f);
        private static readonly Color HoverBorderColorForDarkSkin = new(0.65f, 0.82f, 1f, 0.95f);
        private static readonly Color HoverBorderColorForLightSkin = new(0.12f, 0.38f, 0.70f, 0.95f);

        private readonly List<AvatarEntry> avatars = new();

        private Vector2 scrollPosition;
        private bool refreshRequested = true;
        private double nextRefreshTime;
        private GameObject lastRangeSelectionAnchor;
        private GUIStyle pathLabelStyle;
        private GUIStyle objectNameLabelStyle;
        private GUIStyle rowLayoutStyle;

        [MenuItem(MenuPathWithHotkey)]
        private static void Open()
        {
            var existingWindows = Resources.FindObjectsOfTypeAll<AvatarSwitcherWindow>();
            if (existingWindows.Length > 0)
            {
                foreach (var existingWindow in existingWindows)
                {
                    existingWindow.Close();
                }
                return;
            }

            var window = GetWindow<AvatarSwitcherWindow>();
            var initialSize = window.GetInitialWindowSize();
            window.minSize = initialSize;
            window.position = window.GetInitialWindowRect(initialSize);
            window.UpdateTitleContent();
            window.Show();
        }

        private void OnEnable()
        {
            maxSize = new Vector2(600, 600);
            UpdateTitleContent();
            EditorApplication.hierarchyChanged += RequestRefresh;
            EditorApplication.update += RefreshIfReady;
            Undo.undoRedoPerformed += RequestRefresh;
            RequestRefresh();
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= RequestRefresh;
            EditorApplication.update -= RefreshIfReady;
            Undo.undoRedoPerformed -= RequestRefresh;
        }

        private void OnFocus()
        {
            UpdateTitleContent();
            RequestRefresh();
        }

        private Rect GetInitialWindowRect(Vector2 initialSize)
        {
            var cursorPosition = GetMouseCursorScreenPosition();
            return new Rect(cursorPosition.x, cursorPosition.y, initialSize.x, initialSize.y);
        }

        private Vector2 GetInitialWindowSize()
        {
            EnsureStyles();

            var avatarEntries = new List<AvatarEntry>();
            foreach (var descriptor in EnumerateAvatarDescriptorsInHierarchyOrder())
            {
                avatarEntries.Add(new AvatarEntry(descriptor.gameObject, GetParentPathSegments(descriptor.gameObject)));
            }

            if (avatarEntries.Count == 0)
            {
                var emptyContent = new GUIContent(EmptyStateMessage);
                var emptyWidth = EditorStyles.helpBox.CalcSize(emptyContent).x + InitialWindowPaddingX;
                var emptyHeight = EditorStyles.helpBox.CalcHeight(emptyContent, Mathf.Max(emptyWidth - InitialWindowPaddingX, MinimumInitialWindowWidth)) + InitialWindowPaddingY;
                return new Vector2(Mathf.Max(emptyWidth, MinimumInitialWindowWidth), Mathf.Max(emptyHeight, MinimumInitialWindowHeight));
            }

            var commonParentSegmentCount = GetCommonParentSegmentCount(avatarEntries);
            var maxPathWidth = 0f;
            var maxObjectNameWidth = 0f;
            foreach (var avatarEntry in avatarEntries)
            {
                avatarEntry.SetCommonParentSegmentCount(commonParentSegmentCount);
                maxPathWidth = Mathf.Max(maxPathWidth, pathLabelStyle.CalcSize(new GUIContent(avatarEntry.ParentPath)).x);
                maxObjectNameWidth = Mathf.Max(maxObjectNameWidth, objectNameLabelStyle.CalcSize(new GUIContent(avatarEntry.ObjectName)).x);
            }

            var contentWidth = Mathf.Max(
                maxPathWidth + maxObjectNameWidth + InitialWindowPaddingX,
                Mathf.Ceil(maxPathWidth / PathLabelMaxWidthRatio) + InitialWindowPaddingX,
                MinimumInitialWindowWidth);
            var contentHeight = Mathf.Max(
                (EditorGUIUtility.singleLineHeight * avatarEntries.Count) + InitialWindowPaddingY,
                MinimumInitialWindowHeight);
            return new Vector2(contentWidth, contentHeight);
        }

        private static Vector2 GetMouseCursorScreenPosition()
        {
#if UNITY_EDITOR_WIN
            if (TryGetMouseCursorScreenPosition(out var cursorPosition))
            {
                return cursorPosition;
            }
#else
            return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
#endif

            return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

#if UNITY_EDITOR_WIN
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetCursorPos(out NativePoint lpPoint);

        private static bool TryGetMouseCursorScreenPosition(out Vector2 screenPosition)
        {
            if (GetCursorPos(out var nativePoint))
            {
                screenPosition = new Vector2(nativePoint.X, nativePoint.Y);
                return true;
            }

            screenPosition = default;
            return false;
        }
#endif

        private void UpdateTitleContent()
        {
            titleContent = new GUIContent(WindowTitle, LoadWindowIcon());
        }

        private Texture2D LoadWindowIcon()
        {
            var script = MonoScript.FromScriptableObject(this);
            var scriptPath = AssetDatabase.GetAssetPath(script);
            if (string.IsNullOrEmpty(scriptPath))
            {
                return null;
            }

            var scriptFolder = System.IO.Path.GetDirectoryName(scriptPath)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(scriptFolder))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>($"{scriptFolder}/{WindowIconFileName}");
        }

        private void OnGUI()
        {
            EnsureStyles();
            RefreshIfReady();
            HandleContextMenu();

            if (Event.current.type == EventType.MouseMove)
            {
                Repaint();
            }

            using (new EditorGUILayout.VerticalScope())
            {
                if (avatars.Count == 0)
                {
                    EditorGUILayout.HelpBox(EmptyStateMessage, MessageType.Info);
                    return;
                }

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
                {
                    scrollPosition = scrollView.scrollPosition;

                    var avatarSnapshot = avatars.ToArray();
                    foreach (var avatar in avatarSnapshot)
                    {
                        DrawAvatarRow(avatar);
                    }
                }
            }
        }

        private static void HandleContextMenu()
        {
            if (Event.current.type != EventType.ContextClick)
            {
                return;
            }

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(SettingsMenuLabel), false, AvatarSwitcherSettingsWindow.Open);
            menu.ShowAsContext();
            Event.current.Use();
        }

        private void DrawAvatarRow(AvatarEntry avatar)
        {
            if (avatar.GameObject == null)
            {
                return;
            }

            var rowHeight = EditorGUIUtility.singleLineHeight;
            var rowRect = EditorGUILayout.GetControlRect(false, rowHeight + 2, rowLayoutStyle, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint && avatar.IsActive)
            {
                var rowColor = EditorGUIUtility.isProSkin ? SelectedRowColorForDarkSkin : SelectedRowColorForLightSkin;
                EditorGUI.DrawRect(rowRect, rowColor);
            }

            if (Event.current.type == EventType.Repaint && rowRect.Contains(Event.current.mousePosition))
            {
                DrawBorder(rowRect, EditorGUIUtility.isProSkin ? HoverBorderColorForDarkSkin : HoverBorderColorForLightSkin);
            }

            var pathWidth = GetPathLabelWidth(avatar.ParentPath);
            var pathRect = new Rect(rowRect.xMin, rowRect.yMin, pathWidth, rowRect.height);
            EditorGUI.LabelField(pathRect, avatar.ParentPath, pathLabelStyle);

            var objectNameRect = new Rect(pathRect.xMax, rowRect.yMin, rowRect.width - pathWidth, rowRect.height);
            EditorGUI.LabelField(objectNameRect, avatar.ObjectName, objectNameLabelStyle);

            if (!TryQueueToggleFromMouseDown(pathRect, avatar.GameObject, true))
            {
                TryQueueToggleFromMouseDown(objectNameRect, avatar.GameObject, true);
            }
        }

        private bool TryQueueToggleFromMouseDown(Rect targetRect, GameObject target, bool pingWhenActivated)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && targetRect.Contains(Event.current.mousePosition))
            {
                QueueToggleAvatar(target, Event.current.control, Event.current.shift, ResolvePingWhenActivated(pingWhenActivated, Event.current.alt));
                Event.current.Use();
                return true;
            }

            return false;
        }

        private static bool ResolvePingWhenActivated(bool supportsPing, bool invertSetting)
        {
            if (!supportsPing)
            {
                return false;
            }

            var pingWhenActivated = AvatarSwitcherSettings.PingObjectWhenActivated;
            return invertSetting ? !pingWhenActivated : pingWhenActivated;
        }

        private static void DrawBorder(Rect rect, Color color)
        {
            var previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), Texture2D.linearGrayTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - 1f, rect.width, 1f), Texture2D.linearGrayTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), Texture2D.linearGrayTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 1f, rect.yMin, 1f, rect.height), Texture2D.linearGrayTexture);
            GUI.color = previousColor;
        }

        private void QueueToggleAvatar(GameObject target, bool allowMultiple, bool selectRange, bool pingWhenActivated)
        {
            if (target == null)
            {
                return;
            }

            // OnGUI の描画中に avatars を更新すると GUILayout の状態が崩れるため、次の Editor 更新で処理する。
            EditorApplication.delayCall += () =>
            {
                if (target == null)
                {
                    return;
                }

                var activatesTarget = !target.activeSelf;
                if (selectRange && TryActivateAvatarRange(target, pingWhenActivated))
                {
                    if (activatesTarget)
                    {
                        lastRangeSelectionAnchor = target;
                    }
                    return;
                }

                if (target.activeSelf)
                {
                    if (!allowMultiple && CountActiveAvatars() > 1)
                    {
                        ActivateAvatar(target, false);
                    }
                    else
                    {
                        DeactivateAvatar(target);
                    }
                }
                else
                {
                    ActivateAvatar(target, allowMultiple);
                    lastRangeSelectionAnchor = target;
                }

                if (activatesTarget && pingWhenActivated)
                {
                    EditorGUIUtility.PingObject(target);
                }
            };
        }

        private bool TryActivateAvatarRange(GameObject target, bool pingWhenActivated)
        {
            if (lastRangeSelectionAnchor == null || !lastRangeSelectionAnchor.activeSelf || !HasActiveAvatar())
            {
                return false;
            }

            var anchorIndex = IndexOfAvatar(lastRangeSelectionAnchor);
            var targetIndex = IndexOfAvatar(target);
            if (anchorIndex < 0 || targetIndex < 0)
            {
                return false;
            }

            var activatesTarget = !target.activeSelf;
            var firstIndex = Mathf.Min(anchorIndex, targetIndex);
            var lastIndex = Mathf.Max(anchorIndex, targetIndex);
            var changedObjects = new List<GameObject>();
            var rangeObjects = new List<GameObject>();

            for (var index = firstIndex; index <= lastIndex; index++)
            {
                var gameObject = avatars[index].GameObject;
                if (gameObject == null)
                {
                    continue;
                }

                rangeObjects.Add(gameObject);
                if (!gameObject.activeSelf)
                {
                    changedObjects.Add(gameObject);
                }
            }

            if (changedObjects.Count > 0)
            {
                Undo.RecordObjects(changedObjects.ToArray(), UndoSelectAvatarRange);
                foreach (var changedObject in changedObjects)
                {
                    changedObject.SetActive(true);
                }

                MarkDirtyScenes(changedObjects);
            }

            ShowInSceneVisibility(rangeObjects);
            RefreshImmediately();

            if (activatesTarget && pingWhenActivated)
            {
                EditorGUIUtility.PingObject(target);
            }

            return true;
        }

        private bool HasActiveAvatar()
        {
            foreach (var avatar in avatars)
            {
                if (avatar.GameObject != null && avatar.GameObject.activeSelf)
                {
                    return true;
                }
            }

            return false;
        }

        private int CountActiveAvatars()
        {
            var activeCount = 0;
            foreach (var avatar in avatars)
            {
                if (avatar.GameObject != null && avatar.GameObject.activeSelf)
                {
                    activeCount++;
                }
            }

            return activeCount;
        }

        private int IndexOfAvatar(GameObject target)
        {
            for (var index = 0; index < avatars.Count; index++)
            {
                if (avatars[index].GameObject == target)
                {
                    return index;
                }
            }

            return -1;
        }

        private void ActivateAvatar(GameObject target, bool allowMultiple)
        {
            if (target == null)
            {
                return;
            }

            var changedObjects = new List<GameObject>();
            if (!allowMultiple)
            {
                foreach (var avatar in avatars)
                {
                    if (avatar.GameObject == null || avatar.GameObject == target || !avatar.GameObject.activeSelf)
                    {
                        continue;
                    }
                    changedObjects.Add(avatar.GameObject);
                }
            }

            if (!target.activeSelf)
            {
                changedObjects.Add(target);
            }

            if (changedObjects.Count == 0)
            {
                return;
            }

            Undo.RecordObjects(changedObjects.ToArray(), UndoSwitchAvatar);

            if (!allowMultiple)
            {
                foreach (var avatar in avatars)
                {
                    if (avatar.GameObject != null && avatar.GameObject != target && avatar.GameObject.activeSelf)
                    {
                        avatar.GameObject.SetActive(false);
                    }
                }
            }

            target.SetActive(true);
            ShowInSceneVisibility(new[] { target });
            MarkDirtyScenes(changedObjects);
            RefreshImmediately();
        }

        private static void ShowInSceneVisibility(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject != null && SceneVisibilityManager.instance.IsHidden(gameObject))
                {
                    SceneVisibilityManager.instance.Show(gameObject, true);
                }
            }
        }

        private void DeactivateAvatar(GameObject target)
        {
            if (target == null || !target.activeSelf)
            {
                return;
            }

            Undo.RecordObject(target, UndoDeselectAvatar);
            target.SetActive(false);
            MarkDirtyScenes(new[] { target });
            RefreshImmediately();
        }

        private void RequestRefresh()
        {
            // ヒエラルキー変更が連続しても、一覧の再取得は短い間隔でまとめる。
            refreshRequested = true;
            nextRefreshTime = EditorApplication.timeSinceStartup + RefreshDelaySeconds;
            Repaint();
        }

        private void RefreshIfReady()
        {
            if (!refreshRequested || EditorApplication.timeSinceStartup < nextRefreshTime)
            {
                return;
            }

            RefreshImmediately();
        }

        private void RefreshImmediately()
        {
            refreshRequested = false;
            avatars.Clear();

            var collectedAvatars = new List<AvatarEntry>();
            foreach (var descriptor in EnumerateAvatarDescriptorsInHierarchyOrder())
            {
                collectedAvatars.Add(new AvatarEntry(descriptor.gameObject, GetParentPathSegments(descriptor.gameObject)));
            }

            var commonParentSegmentCount = GetCommonParentSegmentCount(collectedAvatars);
            foreach (var avatar in collectedAvatars)
            {
                avatar.SetCommonParentSegmentCount(commonParentSegmentCount);
                avatars.Add(avatar);
            }

            Repaint();
        }

        private static IEnumerable<VRCAvatarDescriptor> EnumerateAvatarDescriptorsInHierarchyOrder()
        {
            for (var sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.isLoaded)
                {
                    continue;
                }

                var rootGameObjects = scene.GetRootGameObjects();
                foreach (var rootGameObject in rootGameObjects)
                {
                    var descriptors = rootGameObject.GetComponentsInChildren<VRCAvatarDescriptor>(true);
                    foreach (var descriptor in descriptors)
                    {
                        if (descriptor != null && descriptor.gameObject != null && !EditorUtility.IsPersistent(descriptor.gameObject))
                        {
                            yield return descriptor;
                        }
                    }
                }
            }
        }

        private void EnsureStyles()
        {
            if (pathLabelStyle != null && objectNameLabelStyle != null && rowLayoutStyle != null)
            {
                return;
            }

            rowLayoutStyle = new GUIStyle(GUIStyle.none)
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(1, 1, 1, 1),
            };

            pathLabelStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                fontSize = 10,
                wordWrap = false,
            };

            objectNameLabelStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                wordWrap = false,
                fontStyle = FontStyle.Bold,
            };
        }

        private float GetPathLabelWidth(string parentPath)
        {
            var measuredWidth = pathLabelStyle.CalcSize(new GUIContent(parentPath)).x;
            return Mathf.Ceil(Mathf.Min(measuredWidth, Mathf.Max(80f, position.width * PathLabelMaxWidthRatio)));
        }

        private static string[] GetParentPathSegments(GameObject gameObject)
        {
            var sceneName = gameObject.scene.IsValid() ? gameObject.scene.name : SceneNameFallback;
            var names = new List<string>();
            names.Add(sceneName);

            var current = gameObject.transform.parent;

            while (current != null)
            {
                names.Add(current.name);
                current = current.parent;
            }

            if (names.Count > 1)
            {
                var parentNames = names.GetRange(1, names.Count - 1);
                parentNames.Reverse();
                names.RemoveRange(1, names.Count - 1);
                names.AddRange(parentNames);
            }

            return names.ToArray();
        }

        private static int GetCommonParentSegmentCount(IReadOnlyList<AvatarEntry> avatarEntries)
        {
            if (avatarEntries.Count == 0)
            {
                return 0;
            }

            var commonSegmentCount = avatarEntries[0].ParentPathSegments.Length;
            for (var avatarIndex = 1; avatarIndex < avatarEntries.Count; avatarIndex++)
            {
                var pathSegments = avatarEntries[avatarIndex].ParentPathSegments;
                commonSegmentCount = Mathf.Min(commonSegmentCount, pathSegments.Length);

                for (var segmentIndex = 0; segmentIndex < commonSegmentCount; segmentIndex++)
                {
                    if (avatarEntries[0].ParentPathSegments[segmentIndex] == pathSegments[segmentIndex])
                    {
                        continue;
                    }

                    commonSegmentCount = segmentIndex;
                    break;
                }
            }

            return commonSegmentCount;
        }

        private static void MarkDirtyScenes(IEnumerable<GameObject> changedObjects)
        {
            var scenes = new HashSet<Scene>();
            foreach (var changedObject in changedObjects)
            {
                if (changedObject == null || !changedObject.scene.IsValid())
                {
                    continue;
                }
                scenes.Add(changedObject.scene);
            }

            foreach (var scene in scenes)
            {
                if (scene.isLoaded)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                }
            }
        }

        private sealed class AvatarEntry
        {
            public AvatarEntry(GameObject gameObject, string[] parentPathSegments)
            {
                GameObject = gameObject;
                ParentPathSegments = parentPathSegments;
                ParentPath = ToDisplayParentPath(0);
            }

            public GameObject GameObject { get; }

            public string[] ParentPathSegments { get; }

            public string ParentPath { get; private set; }

            public string ObjectName => GameObject == null ? string.Empty : GameObject.name;

            public bool IsActive => GameObject != null && GameObject.activeSelf;

            public void SetCommonParentSegmentCount(int commonParentSegmentCount)
            {
                ParentPath = ToDisplayParentPath(commonParentSegmentCount);
            }

            private string ToDisplayParentPath(int commonParentSegmentCount)
            {
                if (ParentPathSegments.Length <= commonParentSegmentCount)
                {
                    return "/";
                }

                var displaySegments = new string[ParentPathSegments.Length - commonParentSegmentCount];
                for (var index = 0; index < displaySegments.Length; index++)
                {
                    displaySegments[index] = ParentPathSegments[commonParentSegmentCount + index];
                }

                return $"/{string.Join("/", displaySegments)}/";
            }
        }

        private static class AvatarSwitcherSettings
        {
            private const string PingObjectWhenActivatedKey = "AKATSUKIYA.AvatarSwitcher.PingObjectWhenActivated";

            public static bool PingObjectWhenActivated
            {
                get => EditorPrefs.GetBool(PingObjectWhenActivatedKey, true);
                set => EditorPrefs.SetBool(PingObjectWhenActivatedKey, value);
            }
        }

        private sealed class AvatarSwitcherSettingsWindow : EditorWindow
        {
            public static void Open()
            {
                var window = GetWindow<AvatarSwitcherSettingsWindow>(true, AvatarSwitcherWindow.SettingsWindowTitle);
                window.minSize = new Vector2(320f, 92f);
                window.maxSize = new Vector2(320f, 92f);
                window.Show();
            }

            private void OnGUI()
            {
                EditorGUIUtility.labelWidth = 260f;

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.Space(6f);

                    var pingObjectWhenActivated = EditorGUILayout.Toggle(HierarchyHighlightLabel, AvatarSwitcherSettings.PingObjectWhenActivated);
                    if (pingObjectWhenActivated != AvatarSwitcherSettings.PingObjectWhenActivated)
                    {
                        AvatarSwitcherSettings.PingObjectWhenActivated = pingObjectWhenActivated;
                    }
                    EditorGUILayout.LabelField($"　{HierarchyHighlightAltNote}", EditorStyles.miniLabel);
                }
            }
        }
    }
}

#endif
