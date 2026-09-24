using UnityEngine;

namespace XqLua.Sample.GameSample {
    [CreateAssetMenu(fileName = "AudioAlbum", menuName = "XqLua Sample/AudioAlbum")]
    public class AudioAlbum : ScriptableObject {
        [SerializeField] private AudioClip _shotSound = default;

        public AudioClip ShotSound => _shotSound;
    }
}
