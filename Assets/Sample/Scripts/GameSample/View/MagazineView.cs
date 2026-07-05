using System;
using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.GameSample {
    public class MagazineView : MonoBehaviour, IDisposable {

        [SerializeField] private Text _counterText = default;

        public MagazineView Initialize() {
            return this;
        }

        public void Dispose() {

        }

        public void OnAmmoCountChanged(int current) {
            _counterText.text = current.ToString();
        }
    }
}