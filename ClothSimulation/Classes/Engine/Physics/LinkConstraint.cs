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

        public LinkConstraint(Particle p_1, Particle p_2) {
            Particle1 = p_1;
            Particle2 = p_2;

            Distance = MathVec2.Length(p_1.Position - p_2.Position);
        }

        public bool IsValid() {
            //return (Particle2 != null) && (Particle1 != null) && !Broken;
            return !Broken;
        }

        public void Solve() {
            if(!IsValid()) { return; }
            Particle p_1 = Particle1;
            Particle p_2 = Particle2;
            Vector2f v = p_1.Position - p_2.Position;
            float dist = MathVec2.Length(v);
            if(dist > Distance) {
                Broken = dist > Distance * MaxElongationRatio;
                Vector2f n = v / dist;
                float c = Distance - dist;
                Vector2f p = -(c * Strength) / (p_1.Mass + p_2.Mass) * n;
                // Apply position correction
                p_1.Move(-p / p_1.Mass);
                p_2.Move(p / p_2.Mass);
            }
        }
    }
}
