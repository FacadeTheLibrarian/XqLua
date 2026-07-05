using System.Collections;
using UnityEngine;

namespace XqLua.Sample.BadSample {
    public class GameManager : MonoBehaviour {
        [SerializeField] private GameObject _cannonMuzzle = default;
        [SerializeField] private GameObject _cannonBall = default;
        [SerializeField] private Renderer _cannonBallRenderer = default;
        [SerializeField] private ParticleSystem _cannonBallTrail = default;
        [SerializeField] private ParticleSystem _muzzleFlash = default;

        [SerializeField] private UIManager _uiManager = default;

        [SerializeField] private int _ammo = 10;
        [SerializeField] private int _shotDamage = 10;
        [SerializeField] private float _secondToTarget = 1.0f;
        [SerializeField] private float _reloadInterval = 0.125f;

        private int _currentAmmo = default;
        private bool _isFiring = false;
        private bool _isReloading = false;

        public void Start() {
            _currentAmmo = _ammo;
            _cannonBallRenderer.enabled = false;
            _uiManager.UpdateAmmoCounter(10);
            _uiManager.SetReloadWarning(false);
        }

        public void Update() {
            if (_uiManager.IsFireButtonPressed && _isFiring == false && _isReloading == false) {
                if (_currentAmmo > 0) {
                    _currentAmmo--;
                    if (_currentAmmo <= 0) {
                        _uiManager.SetReloadWarning(true);
                    }
                    _isFiring = true;
                    _uiManager.UpdateAmmoCounter(_currentAmmo);
                    _muzzleFlash.Play();
                    _cannonBallRenderer.enabled = true;

                    GameObject target = GameObject.Find("Target");
                    Target targetComponent = target.GetComponent<Target>();

                    StartCoroutine(Fire(target, targetComponent));
                }
            }
            else if (_uiManager.IsReloadButtonPressed && _isFiring == false && _isReloading == false) {
                _isReloading = true;
                StartCoroutine(Reload());
            }
        }

        private IEnumerator Fire(GameObject target, Target component) {
            _cannonBall.transform.position = _cannonMuzzle.transform.position;
            Vector3 difference = target.transform.position - _cannonBall.transform.position;
            float elapsedTime = 0.0f;
            _cannonBallTrail.Play();
            while (elapsedTime < _secondToTarget) {
                float unscaledDeltaTime = Time.unscaledDeltaTime;
                elapsedTime += unscaledDeltaTime;
                _cannonBall.transform.position += (difference * unscaledDeltaTime) / _secondToTarget;
                yield return null;
            }
            component.Hit(_shotDamage);
            _cannonBallTrail.Stop();
            _cannonBallRenderer.enabled = false;
            _isFiring = false;
        }

        private IEnumerator Reload() {
            while (true) {
                _currentAmmo++;
                _uiManager.UpdateAmmoCounter(_currentAmmo);
                if (_currentAmmo >= _ammo) {
                    break;
                }
                yield return new WaitForSeconds(_reloadInterval);
            }
            _uiManager.SetReloadWarning(false);
            _isReloading = false;
        }
    }
}