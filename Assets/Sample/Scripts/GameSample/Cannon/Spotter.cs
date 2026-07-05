using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace XqLua.Sample.GameSample {
    public class Spotter : MonoBehaviour, IDisposable {
        private Target[] _targets = default;
        public Spotter Initialize(Target[] targets) {
            _targets = targets;
            return this;
        }

        public void Dispose() {
        }

        public bool TrySpot(out Target possibleTarget) {
            if (_targets.Length <= 0) {
                Debug.Log("SpotterにTargetが登録されていません！");
                possibleTarget = null;
                return false;
            }

            int randomIndex = Random.Range(0, _targets.Length);
            possibleTarget = _targets[randomIndex];
            return true;
        }
    }
}
