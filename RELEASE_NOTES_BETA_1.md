# Beat Animation Panel Beta 1

기존 v1.1 기능을 유지한 베타 배포본입니다.

| 항목 | 내용 |
|---|---|
| BPM 고정 | 재생 세션 중 BPM 변속을 막고 Reset 이후에만 새 BPM 입력을 허용합니다. |
| Animation 연동 | BeatAnimationTarget과 AnimationClip 생성 기능을 포함합니다. |
| Unity Animation 창 | 생성된 `beatPulse` 키프레임을 Animation 창에서 확인하고 Animator에 연결할 수 있습니다. |
| 다국어 문서 | README.md에 한국어, English, 日本語 사용 설명서가 포함되어 있습니다. |
| 기존 기능 보존 | v1.1의 마커 생성, 테스트, 최적화, 이슈 템플릿을 그대로 유지합니다. |

이 버전은 베타 테스트용입니다. Unity Test Runner에서 `BeatMarkerCalculatorTests`를 실행한 뒤 실제 프로젝트의 Animator와 Animation 창에서 생성된 클립을 확인해 주세요.
