using System;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.GameSample {
    public class ReloadWarningView : MonoBehaviour, IDisposable {

        [SerializeField] private Text _warningText = default;

        public ReloadWarningView Initialize() {
            _warningText.enabled = false;
            return this;
        }

        public void Dispose() {

        }

        public void OnReloadRequired() {
            _warningText.enabled = true;
        }
        public void OnReloaded() {
            _warningText.enabled = false;
        }
    }
}
