# Avatar Switcher

## 【概要】

- 一个 Unity Editor 扩展，用于检测 Hierarchy 中的 VRChat 头像，列出后切换 `IsActive`。
- 通过点击 Hierarchy 每行左侧的图标区域，可以切换该 GameObject 的 `IsActive`。
- 作为 VPM 包发布。可在 VCC 中添加 `https://vpm.noctlab.com/vpm.json` 来安装。

## 【支持语言 / Supported Languages】

- [ja (日本語)](https://noctlab.com/docs/avatar-switcher/ja)
- [en (English)](https://noctlab.com/docs/avatar-switcher/en)
- [ko (한국어)](https://noctlab.com/docs/avatar-switcher/ko)
- [zh (简体中文)](https://noctlab.com/docs/avatar-switcher/zh)

## 【必需依赖】

- `com.vrchat.avatars` `>=3.5.0`

## 【使用方法】

### Avatar Switcher 窗口操作

1. 在 Unity 菜单中选择 `Tools/AKATSUKIYA/Avatar Switcher`，或按下 `Shift+A` 打开/关闭窗口。
2. 点击列表中的头像行，即可切换对应 GameObject 的激活状态。
3. 按住 `Ctrl` 或 `Shift` 再点击，可以同时选择多个头像。

![m1](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m1.webp)

### Hierarchy 激活状态切换功能

1. 点击 Hierarchy 中 GameObject 行最左侧的图标区域，即可切换该 GameObject 的激活状态。

![m2](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m2.webp)

## 【行为说明】

- 点击头像列表会激活对应头像。
  - `Ctrl`+点击可同时保留多个头像为选中状态。
  - `Shift`+点击可进行范围选择。
  - 根据高亮设置，选中的头像会在 Hierarchy 中高亮显示。
  - `Alt`+点击会反转高亮行为。
  - 激活头像时，如果它被 Scene Visibility 隐藏，会自动恢复显示。

- 在窗口任意位置右键，可以打开设置菜单。
  - 启用“选中时在 Hierarchy 中高亮显示 GameObject”后，点击头像列表标签时会在 Hierarchy 中高亮目标对象。
  - `Alt`+点击会执行与该设置相反的行为。

- 点击 Hierarchy 中 GameObject 行最左侧的图标区域，可切换该 GameObject 的激活状态。
  - 当 Hierarchy 中已多选对象时，点击其中一个会同时切换所有已选 GameObject 的激活状态。

## 【许可证】

- 本软件包采用 MIT License。详情请参阅 [LICENSE](https://github.com/itoha-noctlab/avatar-switcher/blob/main/LICENSE.txt)。

## 【联系方式】

- 如有问题，请通过 BOOTH 消息或 https://noctlab.com/contact?t=tool&tool=Avatar+Switcher 联系我们。
