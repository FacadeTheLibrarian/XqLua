using System;
using UnityEngine;
using UnityEngine.UI;
using XqLua;
using XqLua.Unity.Extension;

namespace XqLua.Sample.GameSample {
    public class CannonTriggerView : MonoBehaviour, IDisposable {

        public IPublisher<Empty> OnButtonClicked => _button.OnClickAsPublisher();
        [SerializeField] private Button _button = default;

        public CannonTriggerView Initialize() {
            return this;
        }
        public void Dispose() {

        }

        public void Interactive(bool isInteractive) {
            _button.interactable = isInteractive;
        }
    }
}