#nullable enable

using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AKATSUKIYA.AvatarSwitcher.Localization
{
    public enum LanguageType
    {
        en,
        ja,
        ko,
        zh
    }

    public readonly struct LocalizedString
    {
        public readonly string en;
        public readonly string? ja;
        public readonly string? ko;
        public readonly string? zh;

        public LocalizedString(string en, string? ja = null, string? ko = null, string? zh = null)
        {
            if (string.IsNullOrEmpty(en)) throw new System.ArgumentException("English string must not be null or empty.", nameof(en));
            this.en = en;
            this.ja = ja;
            this.ko = ko;
            this.zh = zh;
        }

        public override string ToString()
        {
            return CurrentLanguage switch
            {
                LanguageType.ja => ja ?? en,
                LanguageType.ko => ko ?? en,
                LanguageType.zh => zh ?? en,
                _ => en,
            };
        }

        public string Format(params object[] args)
        {
            if (args.Length == 0)
            {
                return ToString();
            }

            return string.Format(ToString(), args);
        }

        public static implicit operator string(LocalizedString localizedString) => localizedString.ToString();

        public static implicit operator LocalizedString(string str) => new(str);

        #region static

        private const string CurrentLanguagePrefsKey = "AKATSUKIYA.AvatarSwitcher.CurrentLanguage";
        private const double CurrentLanguageCacheLifetimeSeconds = 10.0;
        private static LanguageType? _currentLanguage;
#if UNITY_EDITOR
        private static double? _invalidateCurrentLanguageAt;
        private static bool _invalidateCurrentLanguageHooked;
#endif

        public static LanguageType CurrentLanguage
        {
            get
            {
                _currentLanguage ??= LoadLanguage();
                return _currentLanguage.Value;
            }
            set
            {
                _currentLanguage = value;
                SaveLanguage(value);
            }
        }

        private static LanguageType LoadLanguage()
        {
            var result = GetSystemLanguage();
#if UNITY_EDITOR
            if (EditorPrefs.HasKey(CurrentLanguagePrefsKey))
            {
                var stored = EditorPrefs.GetString(CurrentLanguagePrefsKey, string.Empty);
                if (System.Enum.TryParse<LanguageType>(stored, true, out var parsed))
                {
                    result = parsed;
                }
            }
#endif
            ScheduleCurrentLanguageInvalidation();
            return result;
        }

        private static void SaveLanguage(LanguageType language)
        {
#if UNITY_EDITOR
            EditorPrefs.SetString(CurrentLanguagePrefsKey, language.ToString());
#endif
            ScheduleCurrentLanguageInvalidation();
        }

        private static void ScheduleCurrentLanguageInvalidation()
        {
#if UNITY_EDITOR
            _invalidateCurrentLanguageAt = EditorApplication.timeSinceStartup + CurrentLanguageCacheLifetimeSeconds;
            if (_invalidateCurrentLanguageHooked)
            {
                return;
            }

            EditorApplication.update += InvalidateCurrentLanguageWhenExpired;
            _invalidateCurrentLanguageHooked = true;
#endif
        }

#if UNITY_EDITOR
        private static void InvalidateCurrentLanguageWhenExpired()
        {
            if (!_invalidateCurrentLanguageAt.HasValue)
            {
                return;
            }

            if (EditorApplication.timeSinceStartup < _invalidateCurrentLanguageAt.Value)
            {
                return;
            }

            _currentLanguage = null;
            _invalidateCurrentLanguageAt = null;

            if (_invalidateCurrentLanguageHooked)
            {
                EditorApplication.update -= InvalidateCurrentLanguageWhenExpired;
                _invalidateCurrentLanguageHooked = false;
            }
        }
#endif

        private static LanguageType GetSystemLanguage()
        {
            var culture = CultureInfo.CurrentCulture;
            if (culture.Name.StartsWith("ja"))
            {
                return LanguageType.ja;
            }
            else if (culture.Name.StartsWith("ko"))
            {
                return LanguageType.ko;
            }
            else if (culture.Name.StartsWith("zh"))
            {
                return LanguageType.zh;
            }
            else
            {
                return LanguageType.en;
            }
        }

        #endregion
    }
}