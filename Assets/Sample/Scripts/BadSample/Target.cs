using System.Collections;
using UnityEngine;

namespace XqLua.Sample.BadSample {
    public class Target : MonoBehaviour {
        [SerializeField] private ParticleSystem _hitParticle = default;
        [SerializeField] private UIManager _uiManager = default;
        [SerializeField] private int _maxHp = 100;
        [SerializeField] private float _dangerZoneRatio = 0.325f;
        [SerializeField] private float _recoverDelaySecond = 1.0f;
        [SerializeField] private float _secondToRecover = 1.0f;
        [SerializeField] private float _uiAnimationSecond = 0.5f;

        private Coroutine _uiAnimation = default;
        private int _hp = default;
        private bool _isRecovering = false;

        public void Start() {
            _hp = _maxHp;
            _uiManager.UpdateTargetHp(_hp);
        }
        public void Hit(int damage) {
            if (!_isRecovering) {
                _hitParticle.Play();
                int previous = _hp;
                _hp -= damage;

                if(_hp < (_maxHp * _dangerZoneRatio)) {
                    _uiManager.ChangeTargetHpColor(Color.yellow);
                }

                if (_uiAnimation != null) {
                    StopCoroutine(_uiAnimation);
                }
                _uiAnimation = StartCoroutine(HitCore(previous));

                if (_hp <= 0) {
                    _isRecovering = true;
                    _uiManager.ChangeTargetHpColor(Color.red);
                    StartCoroutine(Recover());
                }
            }
        }

        private IEnumerator HitCore(int previous) {
            int differrence = _hp - previous;
            float interval = _uiAnimationSecond / Mathf.Abs(differrence);
            int addend = (int)Mathf.Sign(differrence);
            while (previous != _hp) {
                previous += addend;
                _uiManager.UpdateTargetHp(previous);
                yield return new WaitForSeconds(interval);
            }
            _uiAnimation = null;
        }

        private IEnumerator Recover() {
            yield return new WaitForSeconds(_recoverDelaySecond);

            float difference = _maxHp - _hp;
            float interval = _secondToRecover / difference;
            while (true) {
                _hp++;
                _uiManager.UpdateTargetHp(_hp);
                if (_hp >= _maxHp) {
                    break;
                }
                yield return new WaitForSeconds(interval);
            }
            _uiManager.ChangeTargetHpColor(Color.white);
            _isRecovering = false;
        }
    }
}
