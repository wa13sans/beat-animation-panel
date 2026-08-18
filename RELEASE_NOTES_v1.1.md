# Beat Animation Panel v1.1

v1.1 adds fixed-BPM playback sessions, Unity AnimationClip generation, multilingual usage documentation, and editor-side performance improvements.

| Change | Description |
|---|---|
| Fixed BPM session | BPM input is disabled after Play and remains locked until Reset. |
| Animation integration | Create an AnimationClip with `BeatAnimationTarget.beatPulse` keys for each marker. |
| Animation workflow | Generated clips are saved under `Assets/BeatAnimationPanel/Generated`. |
| Optimization | Marker lists are regenerated only after relevant settings change; playback does not rebuild markers every frame. |
| Documentation | README.md now includes Korean, English, and Japanese setup and workflow instructions. |

The generated clip can be connected to a GameObject with `BeatAnimationTarget` and `Animator`, then inspected in Unity's Animation window.
