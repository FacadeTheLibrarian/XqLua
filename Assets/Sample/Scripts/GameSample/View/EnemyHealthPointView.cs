using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XqLua.Extension;

namespace XqLua.Sample.GameSample {
    public class EnemyHealthPointView : MonoBehaviour, IDisposable {

        [SerializeField] private Text _hpText = default;
        [SerializeField] private float _animationSecond = 0.5f;

        private Coroutine _runningCoroutine = default;
        private int _current = 0;

        private Disposables _disposables = default;

        public EnemyHealthPointView Initialize(HealthPoint viewModel) {
            _disposables = new Disposables();
            _current = viewModel.Hp.Value;
            _hpText.text = _current.ToString();
            viewModel.Hp
                .Skip(1)
                .Subscribe(OnValueChanged)
                .AddTo(_disposables);

            viewModel.OnDefeated.Subscribe(_ => _hpText.color = Color.red).AddTo(_disposables);
            viewModel.OnRecovered.Subscribe(_ => _hpText.color = Color.white).AddTo(_disposables);
            viewModel.OnDangerZone.Subscribe(_ => _hpText.color = Color.yellow).AddTo(_disposables);
            return this;
        }
        public void Dispose() {
            _disposables.Dispose();
        }
        public void OnValueChanged(int value) {
            if (_runningCoroutine != null) {
                StopCoroutine(_runningCoroutine);
            }

            IEnumerator enumerator = OnValueChangedCore(value);
            _runningCoroutine = StartCoroutine(enumerator);
        }

        private IEnumerator OnValueChangedCore(int target) {
            int differrence = target - _current;
            float interval = _animationSecond / Mathf.Abs(differrence);
            int addend = (int)Mathf.Sign(differrence);
            while (_current != target) {
                _current += addend;
                _hpText.text = _current.ToString();
                yield return new WaitForSeconds(interval);
            }
            _runningCoroutine = null;
        }
    }
}
