#if UNITY_INCLUDE_TESTS
using NUnit.Framework;

namespace BeatAnimationPanel.Tests
{
    public class BeatMarkerCalculatorTests
    {
        [Test]
        public void Generate_120Bpm_4Beats_2Bars_CreatesEightMarkers()
        {
            var markers = BeatMarkerCalculator.Generate(120f, 4, 2, 1);

            Assert.That(markers, Has.Count.EqualTo(8));
            Assert.That(markers[0].time, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(markers[1].time, Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(markers[4].bar, Is.EqualTo(2));
            Assert.That(markers[4].beat, Is.EqualTo(1));
        }

        [Test]
        public void Generate_SubdivisionFour_CreatesFourMarkersPerBeat()
        {
            var markers = BeatMarkerCalculator.Generate(60f, 4, 1, 4);

            Assert.That(markers, Has.Count.EqualTo(16));
            Assert.That(markers[1].time, Is.EqualTo(0.25f).Within(0.0001f));
            Assert.That(markers[4].beat, Is.EqualTo(2));
            Assert.That(markers[4].isDownbeat, Is.False);
        }

        [Test]
        public void Generate_MarksOnlyFirstSubdivisionOfEachBarAsDownbeat()
        {
            var markers = BeatMarkerCalculator.Generate(120f, 4, 2, 2);

            Assert.That(markers[0].isDownbeat, Is.True);
            Assert.That(markers[1].isDownbeat, Is.False);
            Assert.That(markers[8].isDownbeat, Is.True);
            Assert.That(markers[8].bar, Is.EqualTo(2));
        }

        [Test]
        public void Generate_ClampsInvalidBpmAndCountsToSafeValues()
        {
            var markers = BeatMarkerCalculator.Generate(0f, 0, 0, 0);

            Assert.That(markers, Has.Count.EqualTo(1));
            Assert.That(BeatMarkerCalculator.SecondsPerBeat(0f, 0), Is.EqualTo(60f).Within(0.0001f));
        }
    }
}
#endif
