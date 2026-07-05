using System;
using UnityEngine;
using UnityEngine.UI;
using XqLua.Unity.Extension;

namespace XqLua.Sample.GameSample {
    public class CannonReloadView : MonoBehaviour, IDisposable {

        public IPublisher<Empty> OnButtonClicked => _button.OnClickAsPublisher();
        [SerializeField] private Button _button = default;

        public CannonReloadView Initialize() {
            return this;
        }
        public void Dispose() {

        }

        public void Interactive(bool isInteractive) {
            _button.interactable = isInteractive;
        }
    }
}