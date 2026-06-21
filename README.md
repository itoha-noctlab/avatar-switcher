# Avatar Switcher

## 【Overview】

- A Unity Editor extension that detects VRChat avatars in the Hierarchy, lists them, and switches `IsActive`.
- Adds a way to toggle a GameObject's `IsActive` by clicking the left-edge icon area of each Hierarchy row.
- Distributed as a VPM package. In VCC, add `https://vpm.noctlab.com/vpm.json` to install it.

## 【Supported Languages】

- [ja (日本語)](https://noctlab.com/docs/avatar-switcher/ja)
- [en (English)](https://noctlab.com/docs/avatar-switcher/en)
- [ko (한국어)](https://noctlab.com/docs/avatar-switcher/ko)
- [zh (简体中文)](https://noctlab.com/docs/avatar-switcher/zh)

## 【Required Dependency】

- `com.vrchat.avatars` `>=3.5.0`

## 【Usage】

### Avatar Switcher Window

1. Open or close the window from Unity's menu at `Tools/AKATSUKIYA/Avatar Switcher`, or press `Shift+A`.
2. Click an avatar row in the list to toggle the active state of its GameObject.
3. Hold `Ctrl` or `Shift` while clicking to select multiple avatars at once.

![m1](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m1.webp)

### Hierarchy Active Toggle

1. Click the GameObject icon area on the left edge of a Hierarchy row to toggle that GameObject's active state.

![m2](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m2.webp)

## 【Behavior】

- Clicking an avatar in the list activates that avatar.
  - `Ctrl`+click allows multiple avatars to remain selected.
  - `Shift`+click enables range selection.
  - Depending on the highlight setting, the selected avatar is highlighted in the Hierarchy.
  - `Alt`+click inverts the highlight behavior.
  - When an avatar is activated, it is shown again if it was hidden by Scene Visibility.

- Right-click anywhere in the window to open the settings menu.
  - Enabling "Highlight GameObject in the Hierarchy when selected" makes label clicks in the avatar list highlight the target in the Hierarchy.
  - `Alt`+click inverts that setting.

- Right-click anywhere in the window to open the context menu.
  - The context menu contains "Settings", "Manual", and "Bug Report / Contact".
  - "Manual" opens https://noctlab.com/docs/avatar-switcher.
  - "Bug Report / Contact" opens https://noctlab.com/contact?t=tool&tool=Avatar+Switcher.

- Click the left-edge icon area of a GameObject row in the Hierarchy to toggle that GameObject's active state.
  - If multiple objects are selected in the Hierarchy, clicking one of them toggles the active state of all selected GameObjects.

## 【License】

- This package is provided under the MIT License. See [LICENSE](https://github.com/itoha-noctlab/avatar-switcher/blob/master/LICENSE.txt) for details.

## 【Contact】

- If you find a problem, please contact us via BOOTH message or https://noctlab.com/contact?t=tool&tool=Avatar+Switcher.
