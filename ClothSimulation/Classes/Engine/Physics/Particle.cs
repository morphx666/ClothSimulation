using SFML.System;
using System;

namespace ClothSimulation.Classes.Engine.Physics {
    public class Particle {
        public float Mass = 1.0f;
        public Vector2f Position;
        private Vector2f position_old;
        public Vector2f Velocity;
        public Vector2f Forces;
        public bool IsMoving = true;
        public Guid id = Guid.NewGuid();

        public Particle(Vector2f pos) {
            Position = pos;
            position_old = pos;
        }

        public void Update(float dt) {
            if(!IsMoving) return;
            position_old = Position;
            Velocity += (Forces / Mass) * dt;
            Position += Velocity * dt;
        }

        public void UpdateDerivatives(float dt) {
            Velocity = (Position - position_old) / dt;
            Forces = new();
        }

        public void Move(Vector2f v) {
            if(!IsMoving) return;
            Position += v; // forces = {};
        }
    }
}