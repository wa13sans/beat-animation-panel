using UnityEngine;

namespace BeatAnimationPanel
{
    public class BeatAnimationTarget : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float beatPulse;

        public int beatIndex;
    }
}
