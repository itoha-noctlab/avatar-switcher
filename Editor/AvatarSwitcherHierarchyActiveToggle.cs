#if AKATSUKIYA_VRCSDK3_AVATARS

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using AKATSUKIYA.AvatarSwitcher.Localization;

namespace AKATSUKIYA.AvatarSwitcher.Editor
{
    [InitializeOnLoad]
    internal static class AvatarSwitcherHierarchyActiveToggle
    {
        private const float ToggleWidth = 18f;
        private const float LeftPadding = 0f;
        private static readonly Color HoverBorderColorForDarkSkin = new(0.65f, 0.82f, 1f, 0.95f);
        private static readonly Color HoverBorderColorForLightSkin = new(0.12f, 0.38f, 0.70f, 0.95f);

        static AvatarSwitcherHierarchyActiveToggle()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
            EditorApplication.update += RepaintHierarchyWhileMouseIsOver;
        }

        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject gameObject || EditorUtility.IsPersistent(gameObject))
            {
                return;
            }

            var toggleRect = new Rect(selectionRect.xMin + LeftPadding, selectionRect.y, ToggleWidth, selectionRect.height);
            var mouseIsOverToggle = toggleRect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint && mouseIsOverToggle)
            {
                DrawBorder(toggleRect, EditorGUIUtility.isProSkin ? HoverBorderColorForDarkSkin : HoverBorderColorForLightSkin);
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && mouseIsOverToggle)
            {
                QueueToggleActive(gameObject, Selection.gameObjects);
                Event.current.Use();
            }
        }

        private static void RepaintHierarchyWhileMouseIsOver()
        {
            if (!IsMouseOverHierarchyWindow())
            {
                return;
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        private static bool IsMouseOverHierarchyWindow()
        {
            var mouseOverWindow = EditorWindow.mouseOverWindow;
            return mouseOverWindow != null && mouseOverWindow.GetType().Name == "SceneHierarchyWindow";
        }

        private static void DrawBorder(Rect rect, Color color)
        {
            var previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - 1f, rect.width, 1f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 1f, rect.yMin, 1f, rect.height), Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private static void QueueToggleActive(GameObject gameObject, GameObject[] selectedObjects)
        {
            EditorApplication.delayCall += () =>
            {
                if (gameObject == null)
                {
                    return;
                }

                var nextActive = !gameObject.activeSelf;
                var targetObjects = GetToggleTargets(gameObject, selectedObjects);

                Undo.RecordObjects(targetObjects, Localized.Message.UndoToggleGameObjectActive);
                foreach (var targetObject in targetObjects)
                {
                    if (targetObject != null)
                    {
                        targetObject.SetActive(nextActive);
                    }
                }

                MarkDirtyScenes(targetObjects);
                EditorApplication.RepaintHierarchyWindow();
            };
        }

        private static GameObject[] GetToggleTargets(GameObject clickedObject, GameObject[] selectedObjects)
        {
            if (selectedObjects == null || selectedObjects.Length <= 1)
            {
                return new[] { clickedObject };
            }

            var clickedObjectIsSelected = false;
            foreach (var selectedObject in selectedObjects)
            {
                if (selectedObject == clickedObject)
                {
                    clickedObjectIsSelected = true;
                    break;
                }
            }

            if (!clickedObjectIsSelected)
            {
                return new[] { clickedObject };
            }

            var targets = new System.Collections.Generic.List<GameObject> { clickedObject };
            foreach (var selectedObject in selectedObjects)
            {
                if (selectedObject == null || selectedObject == clickedObject || EditorUtility.IsPersistent(selectedObject))
                {
                    continue;
                }

                targets.Add(selectedObject);
            }

            return targets.ToArray();
        }

        private static void MarkDirtyScenes(GameObject[] changedObjects)
        {
            foreach (var scene in GetDistinctLoadedScenes(changedObjects))
            {
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        private static Scene[] GetDistinctLoadedScenes(GameObject[] gameObjects)
        {
            var scenes = new System.Collections.Generic.List<Scene>();
            foreach (var gameObject in gameObjects)
            {
                if (gameObject == null || !gameObject.scene.IsValid() || !gameObject.scene.isLoaded || scenes.Contains(gameObject.scene))
                {
                    continue;
                }

                scenes.Add(gameObject.scene);
            }

            return scenes.ToArray();
        }
    }
}

#endif
