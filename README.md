# Beat Animation Panel Beta 1

> This is the Beta 1 release. The v1.1 fixed-BPM and AnimationClip integration remains included.

Beat Animation Panel is a Unity Editor tool that generates beat markers from a fixed BPM and creates an `AnimationClip` whose keyframes can be used in the Unity Animation window.

## 한국어 사용 설명서

### 주요 변경 사항

Beta 1에서는 재생 세션 중 **BPM 변속을 잠급니다**. 재생을 시작한 뒤에는 BPM 필드가 비활성화되며, **처음으로** 버튼을 눌러야 새 BPM을 입력할 수 있습니다. 따라서 작업 중 187.5 BPM에서 250 BPM으로 갑자기 바뀌는 것처럼 타이밍이 변하는 문제를 방지할 수 있습니다.

또한 **AnimationClip 생성** 버튼이 추가되었습니다. 이 버튼은 마커의 시간에 `beatPulse` 키를 만들고, 각 마커의 절반 박자 뒤에 값을 0으로 되돌리는 애니메이션 클립을 생성합니다. 생성된 클립은 `Assets/BeatAnimationPanel/Generated` 폴더에 저장됩니다.

### 설치

저장소를 내려받은 뒤 `Assets/BeatAnimationPanel` 폴더를 Unity 프로젝트의 `Assets` 폴더 안에 복사합니다. Unity 2022.3 LTS 이상을 권장합니다. 상단 메뉴에서 **Window > Beat Animation Panel**을 선택해 창을 엽니다.

### BPM 마커 사용

`BPM`에 고정할 BPM을 입력합니다. `박자/마디`는 한 마디의 박자 수이고, `마디 수`는 생성할 마디 수입니다. `세분화`를 `1/1`, `1/2`, `1/4` 중에서 선택하면 한 박자를 더 촘촘한 마커로 나눌 수 있습니다. 설정을 변경하면 마커 목록과 시간 값이 자동으로 다시 생성됩니다.

**재생**을 누르면 현재 BPM으로 재생 세션이 시작되고 BPM 입력이 잠깁니다. **정지**는 재생만 멈추며 현재 세션의 BPM 잠금은 유지합니다. BPM을 바꾸려면 반드시 **처음으로**를 누른 다음 새 BPM을 입력해야 합니다.

### Animation 창과 연동

먼저 Scene의 GameObject에 **BeatAnimationTarget** 컴포넌트를 추가합니다. 이 컴포넌트는 애니메이션이 조절할 `beatPulse` 값과 현재 `beatIndex`를 제공합니다. 다음으로 Beat Animation Panel에서 BPM과 마커 설정을 정하고 **AnimationClip 생성**을 누릅니다.

생성된 `.anim` 파일을 선택한 뒤, 해당 GameObject에 Animator가 있는지 확인합니다. Animation 창에서 새 Animation Clip으로 생성된 클립을 선택하거나 Animator Controller의 상태에 클립을 연결하면 타임라인에 비트 키프레임이 표시됩니다. `BeatAnimationTarget.beatPulse` 커브는 각 마커에서 1로 올라갔다가 다음 절반 박자에 0으로 내려가므로, 이 값을 이용해 Scale, Opacity, Emission 등의 시각 효과를 연결할 수 있습니다.

> 이 도구는 Unity Animation 창의 타임라인에 사용할 수 있는 AnimationClip을 생성합니다. 단, 생성된 클립을 GameObject에서 재생하려면 해당 GameObject에 BeatAnimationTarget과 Animator를 연결해야 합니다.

### 테스트와 최적화

Unity의 **Window > General > Test Runner**를 열고 **Editor** 탭에서 `BeatMarkerCalculatorTests`를 실행합니다. 테스트는 마커 개수, BPM 간격, 세분화, 마디 전환, 다운비트, 잘못된 입력 보정을 확인합니다.

마커는 BPM이나 타이밍 설정이 실제로 변경될 때만 다시 생성됩니다. 재생 중에는 매 프레임 전체 마커 목록을 만들지 않고 현재 인덱스만 계산하며, GUI 재생성은 필요한 경우에만 요청합니다. AnimationClip 생성도 버튼을 눌렀을 때만 실행됩니다.

## English User Guide

### What changed in Beta 1

Beta 1 **locks the BPM during a playback session**. After playback starts, the BPM field is disabled, so timing cannot unexpectedly change from 187.5 BPM to 250 BPM. Press **Reset** before entering a new BPM.

The **Create AnimationClip** button creates an animation asset in `Assets/BeatAnimationPanel/Generated`. It adds a `beatPulse` key with value 1 at every marker and returns the value to 0 halfway through the marker interval.

### Installation

Copy `Assets/BeatAnimationPanel` into your Unity project's `Assets` folder. Unity 2022.3 LTS or newer is recommended. Open the tool from **Window > Beat Animation Panel**.

### Generate markers

Enter the fixed value in `BPM`. Set `Beats / bar`, `Bars`, and `Subdivision`. The subdivision options are 1, 2, and 4 timing points per beat. Changing a timing setting regenerates the marker list and timestamps.

Press **Play** to start a playback session and lock the BPM. **Stop** pauses playback but keeps the session lock. Press **Reset** to unlock the BPM and enter a different value.

### Connect to the Unity Animation window

Add the **BeatAnimationTarget** component to a GameObject in your Scene. Set the BPM and marker settings, then press **Create AnimationClip**. The generated `.anim` asset appears in `Assets/BeatAnimationPanel/Generated`.

Make sure the GameObject has an Animator. Select the generated clip in the Animation window or connect it to a state in the Animator Controller. The `BeatAnimationTarget.beatPulse` curve rises to 1 at each marker and falls to 0 halfway through the interval. You can use this property to drive scale, opacity, emission, or other visual effects.

> The tool creates an AnimationClip that can be used by the Unity Animation timeline. To play it on a GameObject, connect that GameObject to a BeatAnimationTarget and an Animator.

### Tests and performance

Open **Window > General > Test Runner**, select the **Editor** tab, and run `BeatMarkerCalculatorTests`. The tests cover marker count, BPM timing, subdivisions, bar changes, downbeats, and invalid input handling.

Markers are regenerated only when BPM or timing settings change. During playback, the editor computes only the current marker index instead of rebuilding the complete list every frame. AnimationClip generation runs only when the button is pressed.

## 日本語ユーザーガイド

### Beta 1 の変更点

Beta 1 では、再生セッション中の **BPM 変更をロック**します。再生を開始すると BPM フィールドが無効になり、187.5 BPM から 250 BPM へ意図せずテンポが変わる問題を防ぎます。新しい BPM を入力する場合は、先に **先頭へ** を押してください。

**AnimationClipを生成** ボタンを押すと、`Assets/BeatAnimationPanel/Generated` に AnimationClip が作成されます。各マーカーの時刻で `beatPulse` を 1 にし、マーカー間隔の半分の時刻で 0 に戻します。

### インストール

`Assets/BeatAnimationPanel` フォルダーを Unity プロジェクトの `Assets` フォルダーへコピーします。Unity 2022.3 LTS 以降を推奨します。上部メニューの **Window > Beat Animation Panel** からウィンドウを開きます。

### マーカーの生成

`BPM` に固定する BPM を入力します。`拍子/小節`、`小節数`、`分割`を設定します。分割は 1 拍あたり 1、2、4 個のタイミングに対応します。設定を変更すると、マーカーと時刻が自動的に更新されます。

**再生**を押すと再生セッションが始まり、BPM がロックされます。**停止**は再生を止めますがロックを維持します。**先頭へ**を押すとロックが解除され、新しい BPM を入力できます。

### Unity Animation ウィンドウとの連携

Scene の GameObject に **BeatAnimationTarget** コンポーネントを追加します。BPM とマーカー設定を決めて、**AnimationClipを生成**を押します。生成された `.anim` は `Assets/BeatAnimationPanel/Generated` に保存されます。

GameObject に Animator があることを確認し、Animation ウィンドウで生成されたクリップを選択するか、Animator Controller のステートへ接続します。`BeatAnimationTarget.beatPulse` カーブは各マーカーで 1 になり、間隔の半分で 0 に戻ります。この値を Scale、Opacity、Emission などの視覚効果に利用できます。

### テストと最適化

**Window > General > Test Runner** を開き、**Editor** タブで `BeatMarkerCalculatorTests` を実行します。マーカー数、BPM 間隔、分割、小節、ダウンビート、不正な入力の補正を検証します。

マーカーは設定が変更された場合だけ再生成されます。再生中は毎フレーム全リストを作り直さず、現在のマーカーインデックスだけを計算します。AnimationClip の生成もボタンを押したときだけ実行されます。

## License

This project is released under the MIT License. See [LICENSE](LICENSE).
