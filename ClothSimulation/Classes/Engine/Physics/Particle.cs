using SFML.System;
using System;

namespace ClothSimulation.Classes.Engine.Physics {
    public class Particle {
        public float Mass = 1.0f;
        public Vector2f Position;
        private Vector2f positionOld;
        public Vector2f Velocity;
        public Vector2f Forces;
        public bool IsMoving = true;
        public Guid id = Guid.NewGuid();

        public Particle(Vector2f pos) {
            Position = pos;
            positionOld = pos;
        }

        public void Update(float dt) {
            if(!IsMoving) return;
            positionOld = Position;
            Velocity += (Forces / Mass) * dt;
            Position += Velocity * dt;
        }

        public void UpdateDerivatives(float dt) {
            Velocity = (Position - positionOld) / dt;
            Forces = new();
        }

        public void Move(Vector2f v) {
            if(!IsMoving) return;
            Position += v; // forces = {};
        }
    }
}