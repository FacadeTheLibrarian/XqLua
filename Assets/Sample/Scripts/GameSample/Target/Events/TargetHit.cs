using UnityEngine;

namespace XqLua.Sample.GameSample {
    public class TargetHit : ITargetEvents {
        public int Id { get; private set; }
        public Vector3 Position { get; private set; }
        public int Damage { get; private set; }
        public TargetHit(int id, Vector3 position, int damage) {
            Id = id;
            Position = position;
            Damage = damage;
        }
    }
}
