using System;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class AudioController : IDisposable {
        private Disposables _disposables = default;

        public AudioController(Cannon cannon, AudioSourceService audioSourceService, AudioAlbum audioAlbum) {
            _disposables = new Disposables();
            cannon.OnFire.Subscribe(_ => audioSourceService.PlayOneShot(audioAlbum.ShotSound)).AddTo(_disposables);
        }

        public void Dispose() {
            _disposables.Dispose();
        }
    }
}
