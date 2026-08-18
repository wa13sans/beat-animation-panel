using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BeatAnimationPanel
{
    public class BeatAnimationPanelWindow : EditorWindow
    {
        private enum Language { Korean, Japanese, English }
        private const string LanguageKey = "BeatAnimationPanel.Language";

        private Language language;
        private float bpm = 120f;
        private int beatsPerBar = 4;
        private int barCount = 8;
        private int subdivision = 1;
        private bool isPlaying;
        private bool isTimingLocked;
        private float lockedBpm;
        private double playStartTime;
        private Vector2 scroll;
        private GUIStyle markerStyle;
        private GUIStyle activeMarkerStyle;
        private List<BeatMarker> markers = new List<BeatMarker>();

        [MenuItem("Window/Beat Animation Panel")]
        public static void Open()
        {
            var window = GetWindow<BeatAnimationPanelWindow>();
            window.titleContent = new GUIContent("Beat Animation Panel");
            window.minSize = new Vector2(540f, 420f);
            window.Show();
        }

        private void OnEnable()
        {
            language = (Language)EditorPrefs.GetInt(LanguageKey, (int)Language.Korean);
            EditorApplication.update += OnEditorUpdate;
            RegenerateMarkers();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (isPlaying) Repaint();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawHeader();
            DrawControls();
            DrawTimeline();
        }

        private void EnsureStyles()
        {
            if (markerStyle != null) return;
            markerStyle = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fixedHeight = 42f,
                margin = new RectOffset(3, 3, 3, 3)
            };
            activeMarkerStyle = new GUIStyle(markerStyle)
            {
                fontStyle = FontStyle.Bold
            };
            activeMarkerStyle.normal.textColor = Color.white;
            activeMarkerStyle.normal.background = MakeTexture(new Color(0.18f, 0.62f, 1f, 1f));
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(T("Beat Animation Panel", "ビートアニメーションパネル", "Beat Animation Panel"), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            var nextLanguage = (Language)EditorGUILayout.EnumPopup(language, EditorStyles.toolbarPopup, GUILayout.Width(105));
            if (nextLanguage != language)
            {
                language = nextLanguage;
                EditorPrefs.SetInt(LanguageKey, (int)language);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox(T(
                "BPM을 입력하면 박자마다 마커가 생성되고 재생 중 현재 마커가 강조됩니다.",
                "BPMを入力すると拍ごとにマーカーが生成され、再生中のマーカーが強調されます。",
                "Enter a BPM to generate beat markers. The current marker is highlighted during playback."), MessageType.Info);
        }

        private void DrawControls()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(isTimingLocked);
            bpm = Mathf.Clamp(EditorGUILayout.FloatField(T("BPM", "BPM", "BPM"), bpm), 1f, 999f);
            EditorGUI.EndDisabledGroup();
            if (isTimingLocked)
                EditorGUILayout.HelpBox(T("재생 세션 중에는 BPM 변속이 잠깁니다. 처음으로를 눌러 잠금을 해제하세요.", "再生セッション中はBPM変更がロックされます。先頭へで解除できます。", "BPM changes are locked during the playback session. Press Reset to unlock."), MessageType.Warning);
            beatsPerBar = EditorGUILayout.IntSlider(T("박자/마디", "拍子/小節", "Beats / bar"), beatsPerBar, 1, 16);
            barCount = EditorGUILayout.IntSlider(T("마디 수", "小節数", "Bars"), barCount, 1, 64);
            subdivision = EditorGUILayout.IntPopup(T("세분화", "分割", "Subdivision"), subdivision,
                new[] { "1/1", "1/2", "1/4" }, new[] { 1, 2, 4 });
            if (EditorGUI.EndChangeCheck()) RegenerateMarkers();

            var interval = BeatMarkerCalculator.SecondsPerBeat(ActiveBpm(), subdivision);
            var totalLength = markers.Count == 0 ? 0f : markers[markers.Count - 1].time + interval;
            EditorGUILayout.LabelField(T("한 마커 간격", "マーカー間隔", "Marker interval"), interval.ToString("0.000") + " s");
            EditorGUILayout.LabelField(T("총 길이", "合計時間", "Total duration"), totalLength.ToString("0.00") + " s");
            EditorGUILayout.LabelField(T("생성된 마커", "生成されたマーカー", "Generated markers"), markers.Count.ToString());

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(T("마커 새로고침", "マーカーを更新", "Regenerate markers"), GUILayout.Height(28))) RegenerateMarkers();
            if (GUILayout.Button(T("AnimationClip 생성", "AnimationClipを生成", "Create AnimationClip"), GUILayout.Height(28))) CreateAnimationClip();
            var playLabel = isPlaying ? T("정지", "停止", "Stop") : T("재생", "再生", "Play");
            if (GUILayout.Button(playLabel, GUILayout.Height(28)))
            {
                isPlaying = !isPlaying;
                if (isPlaying)
                {
                    lockedBpm = bpm;
                    isTimingLocked = true;
                    playStartTime = EditorApplication.timeSinceStartup;
                }
            }
            if (GUILayout.Button(T("처음으로", "先頭へ", "Reset"), GUILayout.Height(28)))
            {
                isPlaying = false;
                isTimingLocked = false;
                lockedBpm = 0f;
                playStartTime = EditorApplication.timeSinceStartup;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawTimeline()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(T("애니메이션 마커", "アニメーションマーカー", "Animation Markers"), EditorStyles.boldLabel);
            var interval = BeatMarkerCalculator.SecondsPerBeat(ActiveBpm(), subdivision);
            var currentIndex = isPlaying && markers.Count > 0
                ? (int)((EditorApplication.timeSinceStartup - playStartTime) / interval) % markers.Count
                : -1;

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));
            for (var i = 0; i < markers.Count; i++)
            {
                var marker = markers[i];
                if (marker.isDownbeat)
                    EditorGUILayout.LabelField(string.Format(T("마디 {0}", "小節 {0}", "Bar {0}"), marker.bar), EditorStyles.miniBoldLabel);
                var style = i == currentIndex ? activeMarkerStyle : markerStyle;
                var caption = string.Format(T("마커 {0}\n{1:0.000}s", "マーカー {0}\n{1:0.000}s", "Marker {0}\n{1:0.000}s"), marker.index + 1, marker.time);
                if (GUILayout.Button(caption, style, GUILayout.ExpandWidth(true)))
                {
                    isPlaying = false;
                    playStartTime = EditorApplication.timeSinceStartup - marker.time;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void RegenerateMarkers()
        {
            markers = BeatMarkerCalculator.Generate(ActiveBpm(), beatsPerBar, barCount, subdivision);
            Repaint();
        }

        private float ActiveBpm()
        {
            return isTimingLocked ? lockedBpm : bpm;
        }

        private void CreateAnimationClip()
        {
            if (markers.Count == 0) RegenerateMarkers();
            var folder = "Assets/BeatAnimationPanel/Generated";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                if (!AssetDatabase.IsValidFolder("Assets/BeatAnimationPanel"))
                    AssetDatabase.CreateFolder("Assets", "BeatAnimationPanel");
                AssetDatabase.CreateFolder("Assets/BeatAnimationPanel", "Generated");
            }

            var activeBpm = ActiveBpm();
            var safeBpm = Mathf.RoundToInt(activeBpm);
            var path = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/BeatAnimation_{1}BPM.anim", folder, safeBpm));
            var clip = new AnimationClip { name = "BeatAnimation_" + safeBpm + "BPM" };
            clip.frameRate = 60f;
            var keys = new Keyframe[markers.Count * 2];
            var interval = BeatMarkerCalculator.SecondsPerBeat(activeBpm, subdivision);
            for (var i = 0; i < markers.Count; i++)
            {
                var time = markers[i].time;
                keys[i * 2] = new Keyframe(time, 1f);
                keys[i * 2 + 1] = new Keyframe(time + interval * 0.5f, 0f);
            }

            var curve = new AnimationCurve(keys);
            var binding = EditorCurveBinding.FloatCurve(string.Empty, typeof(BeatAnimationTarget), "beatPulse");
            AnimationUtility.SetEditorCurve(clip, binding, curve);
            AssetDatabase.CreateAsset(clip, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = clip;
            EditorGUIUtility.PingObject(clip);
            ShowNotification(new GUIContent(T("AnimationClip이 생성되었습니다.", "AnimationClipを生成しました。", "AnimationClip created.")));
        }

        private string T(string korean, string japanese, string english)
        {
            switch (language)
            {
                case Language.Japanese: return japanese;
                case Language.English: return english;
                default: return korean;
            }
        }
    }
}
