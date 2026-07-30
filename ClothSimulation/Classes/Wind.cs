using ClothSimulation.Classes.Engine.Physics;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace ClothSimulation.Classes {
    public class Wind {
        public FloatRect Bounds;
        public Vector2f Force;

        public Wind(Vector2f position, Vector2f size, Vector2f force) {
            Bounds = new(position, size);
            this.Force = force;
        }

        public void Update(float dt) {
            Bounds.Position += new Vector2f(Force.X * dt, 0 * Force.Y * dt);
        }
    }

    public class WindManager {
        public List<Wind> Winds = [];
        public float WorldWidth = 0.0f;

        public WindManager(float width) {
            WorldWidth = width;
        }

        public void Update(PhysicSolver solver, float dt) {
            foreach(Wind w in Winds) {
                w.Update(dt);
                foreach(Particle p in solver.Objects) {
                    if(w.Bounds.Contains(new Vector2f(p.Position.X, p.Position.Y))) {
                        p.Forces += 1.0f * w.Force / dt;
                    }
                }

                if(w.Bounds.Left > WorldWidth) {
                    w.Bounds.Position = new Vector2f(-w.Bounds.Width, w.Bounds.Top);
                }
            }
        }
    }
}
