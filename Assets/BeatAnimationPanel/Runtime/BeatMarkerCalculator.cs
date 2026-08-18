using System;
using System.Collections.Generic;

namespace BeatAnimationPanel
{
    [Serializable]
    public struct BeatMarker
    {
        public int index;
        public int bar;
        public int beat;
        public float time;
        public bool isDownbeat;

        public BeatMarker(int index, int bar, int beat, float time, bool isDownbeat)
        {
            this.index = index;
            this.bar = bar;
            this.beat = beat;
            this.time = time;
            this.isDownbeat = isDownbeat;
        }
    }

    public static class BeatMarkerCalculator
    {
        public static float SecondsPerBeat(float bpm, int subdivision = 1)
        {
            var safeBpm = Math.Max(1f, bpm);
            var safeSubdivision = Math.Max(1, subdivision);
            return 60f / safeBpm / safeSubdivision;
        }

        public static List<BeatMarker> Generate(float bpm, int beatsPerBar, int barCount, int subdivision)
        {
            var safeBeatsPerBar = Math.Max(1, beatsPerBar);
            var safeBarCount = Math.Max(1, barCount);
            var safeSubdivision = Math.Max(1, subdivision);
            var markersPerBar = safeBeatsPerBar * safeSubdivision;
            var totalMarkers = markersPerBar * safeBarCount;
            var interval = SecondsPerBeat(bpm, safeSubdivision);
            var markers = new List<BeatMarker>(totalMarkers);

            for (var i = 0; i < totalMarkers; i++)
            {
                var bar = i / markersPerBar + 1;
                var beatInBar = i % markersPerBar;
                var beat = beatInBar / safeSubdivision + 1;
                var isDownbeat = beatInBar == 0;
                markers.Add(new BeatMarker(i, bar, beat, i * interval, isDownbeat));
            }

            return markers;
        }
    }
}
