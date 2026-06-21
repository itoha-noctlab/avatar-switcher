using AKATSUKIYA.AvatarSwitcher.Localization;

namespace AKATSUKIYA.AvatarSwitcher.Editor
{
    internal static class Localized
    {
        public static class UI
        {
            public static readonly LocalizedString WindowTitle = new(
                en: "Avatar Switcher",
                ja: "Avatar Switcher",
                ko: "Avatar Switcher",
                zh: "Avatar Switcher"
            );

            public static readonly LocalizedString SettingsMenuLabel = new(
                en: "Settings",
                ja: "設定",
                ko: "설정",
                zh: "设置"
            );

            public static readonly LocalizedString ManualMenuLabel = new(
                en: "Manual",
                ja: "マニュアル",
                ko: "매뉴얼",
                zh: "手册"
            );

            public static readonly LocalizedString ContactMenuLabel = new(
                en: "Bug Report / Contact",
                ja: "不具合報告・お問い合わせ",
                ko: "버그 신고 / 문의",
                zh: "错误报告 / 联系"
            );

            public static readonly LocalizedString EmptyStateMessage = new(
                en: "There are no GameObjects with VRC Avatar Descriptor in the Hierarchy.",
                ja: "ヒエラルキー上に VRC Avatar Descriptor が設定された GameObject はありません。",
                ko: "Hierarchy에 VRC Avatar Descriptor가 설정된 GameObject가 없습니다.",
                zh: "Hierarchy 中没有设置 VRC Avatar Descriptor 的 GameObject。"
            );

            public static readonly LocalizedString SettingsWindowTitle = new(
                en: "Avatar Switcher Settings",
                ja: "Avatar Switcher 設定",
                ko: "Avatar Switcher 설정",
                zh: "Avatar Switcher 设置"
            );

            public static readonly LocalizedString HierarchyHighlightLabel = new(
                en: "Highlight GameObject in the Hierarchy when selected",
                ja: "選択時にヒエラルキー上でゲームオブジェクトを強調表示",
                ko: "선택 시 Hierarchy에서 GameObject를 강조 표시",
                zh: "选中时在 Hierarchy 中高亮显示 GameObject"
            );

            public static readonly LocalizedString HierarchyHighlightAltNote = new(
                en: "(Hold Alt while clicking to invert this setting.)",
                ja: "（Alt キーを押しながらクリックすると設定とは逆に動作します。）",
                ko: "（Alt 키를 누른 채 클릭하면 설정과 반대로 동작합니다。）",
                zh: "（按住 Alt 键单击时会以与此设置相反的方式工作。）"
            );

            public static readonly LocalizedString SceneNameFallback = new(
                en: "Scene",
                ja: "シーン",
                ko: "씬",
                zh: "场景"
            );
        }

        public static class Message
        {
            public static readonly LocalizedString UndoSelectAvatarRange = new(
                en: "Select Avatar Range",
                ja: "アバター範囲選択",
                ko: "아바타 범위 선택",
                zh: "选择头像范围"
            );

            public static readonly LocalizedString UndoSwitchAvatar = new(
                en: "Switch Avatar",
                ja: "アバター切り替え",
                ko: "아바타 전환",
                zh: "切换头像"
            );

            public static readonly LocalizedString UndoDeselectAvatar = new(
                en: "Deselect Avatar",
                ja: "アバター選択解除",
                ko: "아바타 선택 해제",
                zh: "取消选择头像"
            );

            public static readonly LocalizedString UndoToggleGameObjectActive = new(
                en: "Toggle GameObject Active",
                ja: "GameObject の有効状態を切り替え",
                ko: "GameObject 활성 상태 전환",
                zh: "切换 GameObject 活跃状态"
            );
        }
    }
}