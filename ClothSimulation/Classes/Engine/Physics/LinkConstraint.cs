using ClothSimulation.Classes.Engine.Common;
using SFML.System;

namespace ClothSimulation.Classes.Engine.Physics {
    public class LinkConstraint {
        public Particle Particle1;
        public Particle Particle2;
        public float MaxElongationRatio = 1.5f;
        public float Distance = 1.0f;
        public float Strength = 1.0f;
        public bool Broken = false;

        public LinkConstraint(Particle p1, Particle p2) {
            Particle1 = p1;
            Particle2 = p2;

            Distance = MathVec2.Length(p1.Position - p2.Position);
        }

        public bool IsValid() {
            //return (Particle2 != null) && (Particle1 != null) && !Broken;
            return !Broken;
        }

        public void Solve() {
            if(!IsValid()) { return; }
            Particle p1 = Particle1;
            Particle p2 = Particle2;
            Vector2f v = p1.Position - p2.Position;
            float dist = MathVec2.Length(v);
            if(dist > Distance) {
                Broken = dist > Distance * MaxElongationRatio;
                Vector2f n = v / dist;
                float c = Distance - dist;
                Vector2f p = -(c * Strength) / (p1.Mass + p2.Mass) * n;
                // Apply position correction
                p1.Move(-p / p1.Mass);
                p2.Move(p / p2.Mass);
            }
        }
    }
}
