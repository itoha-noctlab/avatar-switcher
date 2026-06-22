# Avatar Switcher

![avatar-switcher](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/image.png)

## 【개요】

- Hierarchy에 있는 VRChat 아바타를 감지해 목록으로 표시하고, `IsActive`를 전환하는 Unity Editor 확장입니다.
- Hierarchy의 각 GameObject 왼쪽 끝 아이콘 영역을 클릭하여 해당 오브젝트의 `IsActive`를 전환할 수 있습니다.
- VPM 패키지로 배포됩니다. VCC에서 `https://vpm.noctlab.com/vpm.json`을 추가하면 설치할 수 있습니다.

## 【지원 언어 / Supported Languages】

- [ja (日本語)](https://noctlab.com/docs/avatar-switcher/ja)
- [en (English)](https://noctlab.com/docs/avatar-switcher/en)
- [ko (한국어)](https://noctlab.com/docs/avatar-switcher/ko)
- [zh (简体中文)](https://noctlab.com/docs/avatar-switcher/zh)

## 【필수 의존성】

- `com.vrchat.avatars` `>=3.5.0`

## 【사용 방법】

### Avatar Switcher 창 조작

1. Unity 메뉴에서 `Tools/AKATSUKIYA/Avatar Switcher`를 선택하거나 `Shift+A`를 눌러 창을 열고 닫습니다.
2. 목록에 표시된 아바타 행을 클릭하여 GameObject의 활성 상태를 전환합니다.
3. `Ctrl`이나 `Shift`를 누른 채 클릭하면 여러 아바타를 동시에 선택할 수도 있습니다.

![m1](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m1.webp)

### Hierarchy 활성 상태 전환 기능

1. Hierarchy에서 GameObject 아이콘 영역을 클릭하면 해당 GameObject의 활성 상태를 전환할 수 있습니다.

![m2](https://raw.githubusercontent.com/itoha-noctlab/avatar-switcher/refs/heads/master/img/m2.webp)

## 【동작】

- 아바타 목록을 클릭하면 해당 아바타가 활성화됩니다.
  - `Ctrl`+클릭으로 여러 아바타를 동시에 선택 상태로 둘 수 있습니다.
  - `Shift`+클릭으로 범위 선택이 가능합니다.
  - 강조 표시 설정에 따라 선택한 아바타가 Hierarchy에서 강조 표시됩니다.
  - `Alt`+클릭하면 강조 표시 동작이 설정과 반대로 바뀝니다.
  - 아바타의 부모 GameObject가 비활성화되어 있으면 목록에서는 선택되지 않은 상태로 표시됩니다.
  - 아바타를 활성화할 때 루트 계층까지의 비활성화된 부모 GameObject도 함께 활성화됩니다.
  - 아바타를 활성화할 때 Scene Visibility로 숨겨져 있으면 다시 표시됩니다.

- 창에서 마우스 오른쪽 버튼을 클릭하면 설정 메뉴를 열 수 있습니다.
  - "선택 시 Hierarchy에서 GameObject를 강조 표시"를 활성화하면 아바타 목록의 레이블 클릭 시 Hierarchy에서 대상이 강조 표시됩니다.
  - `Alt`+클릭으로 해당 설정의 반대 동작을 수행합니다.

- 창에서 마우스 오른쪽 버튼을 클릭하면 컨텍스트 메뉴를 열 수 있습니다.
  - 컨텍스트 메뉴에는 "설정", "매뉴얼", "버그 신고 / 문의"가 있습니다.
  - "매뉴얼"은 https://noctlab.com/docs/avatar-switcher 를 엽니다.
  - "버그 신고 / 문의"는 https://noctlab.com/contact?t=tool&tool=Avatar+Switcher 를 엽니다.

- Hierarchy의 GameObject 행 왼쪽 끝 아이콘 영역을 클릭하면 해당 GameObject의 활성 상태를 전환합니다.
  - Hierarchy에서 여러 오브젝트를 선택한 상태로 그중 하나를 클릭하면, 선택된 모든 GameObject의 활성 상태를 함께 전환합니다.

## 【라이선스】

- 이 패키지는 MIT License로 제공됩니다. 자세한 내용은 [LICENSE](https://github.com/itoha-noctlab/avatar-switcher/blob/master/LICENSE.txt)를 참조하세요.

## 【문의】

- 문제가 있으면 BOOTH 메시지 또는 https://noctlab.com/contact?t=tool&tool=Avatar+Switcher 로 문의해 주세요.
