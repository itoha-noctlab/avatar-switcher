# Avatar Switcher

## 【概要】

- Hierarchy 上の VRChat アバターを検知して一覧表示し、`IsActive` を切り替える Unity Editor 拡張です。
- Hierarchy の各 GameObject 左端アイコン部分をクリックすることでオブジェクトの `IsActive` を切り替える機能を追加します。
- VPMパッケージとして公開しています。VCCで`https://vpm.noctlab.com/vpm.json`を追加して導入できます。

## 【対応言語 / Supported Languages】

- [ja (日本語)](https://noctlab.com/docs/avatar-switcher/ja)
- [en (English)](https://noctlab.com/docs/avatar-switcher/en)
- [ko (한국어)](https://noctlab.com/docs/avatar-switcher/ko)
- [zh (简体中文)](https://noctlab.com/docs/avatar-switcher/zh)

## 【必須依存】

- `com.vrchat.avatars` `>=3.5.0`

## 【使用方法】

### Avatar Switcher ウィンドウ操作

1. Unity のメニューから `Tools/AKATSUKIYA/Avatar Switcher` を選択するか、`Shift+A` を押してウィンドウを開閉します。
2. 一覧に表示されたアバターの行をクリックして GameObject のアクティブ状態を切り替えます。
- `Ctrl` や `Shift` を押しながらクリックすることで、複数のアバターを同時に選択状態にすることもできます。

![m1](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m1.webp)

### Hierarchy のアクティブ状態切り替え機能

1. Hierarchy 上の GameObject アイコンをクリックすることで、その GameObject のアクティブ状態を切り替えができます。

![m2](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m2.webp)

## 【仕様】

- アバター一覧をクリックすることで、対象アバターをアクティブにします。
  - `Ctrl` + クリックで複数のアバターを同時に選択状態にできます。
  - `Shift` + クリックで、範囲選択が可能です。
  - 強調表示の設定に応じて、選択したアバターを Hierarchy 上で強調表示します。
  - `Alt` + クリックで、選択したときに強調表示の設定とは逆の動作をします。
  - アバターをアクティブにしたとき、Scene Visibility で非表示になっている場合は表示状態へ戻します。

- ウインドウ上で右クリックすると、設定メニューを開けます。
  - 「選択時に Hierarchy 上でゲームオブジェクトを強調表示」を有効にすると、アバター一覧のラベルクリック時に Hierarchy 上の対象を強調表示します。
  - `Alt` + クリックで、選択したときに強調表示の設定とは逆の動作をします。

- Hierarchy 上の GameObject 行の左端アイコン部分をクリックして、その GameObject のアクティブ状態を切り替えます。
  - Hierarchy で複数選択中のオブジェクトをクリックした場合、選択中のすべての GameObject のアクティブ状態を切り替えます。

## 【ライセンス】

- このパッケージは MIT License で提供されます。詳細は [LICENSE](https://github.com/itoha-noctlab/avatar-switcher/blob/main/LICENSE.txt) を参照してください。

## 【お問い合わせ】

- 不具合などありましたら、BOOTHメッセージまたは https://noctlab.com/contact?t=tool&tool=Avatar+Switcher にてご連絡ください。
