using UnityEngine;
using UnityEngine.UI;

namespace XqLua.Sample.BadSample {
    public class UIManager : MonoBehaviour {

        public bool IsFireButtonPressed { get { return _fireButton.IsPressed(); } }
        public bool IsReloadButtonPressed { get { return _reloadButton.IsPressed(); } }

        [SerializeField] private Text _ammoCounter = default;
        [SerializeField] private Text _reloadWarning = default;

        [SerializeField] private Button _fireButton = default;
        [SerializeField] private Button _reloadButton = default;

        public void UpdateAmmoCounter(int ammo) {
            _ammoCounter.text = ammo.ToString();
        }

        public void SetReloadWarning(bool state) {
            _reloadWarning.enabled = state;
        }
    }
}