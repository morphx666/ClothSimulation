using ClothSimulation.Classes.Engine.Physics;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace ClothSimulation.Classes {
    public class Wind {
        public FloatRect rect;
        public Vector2f force;

        public Wind(Vector2f position, Vector2f size, Vector2f force) {
            rect = new(position, size);
            this.force = force;
        }

        public void Update(float dt) {
            rect.Position += new Vector2f(force.X * dt, 0 * force.Y * dt);
        }
    }

    public class WindManager {
        public List<Wind> winds = [];
        public float worldWidth = 0.0f;

        public WindManager(float width) {
            worldWidth = width;
        }

        public void Update(PhysicSolver solver, float dt) {
            foreach(Wind w in winds) {
                w.Update(dt);
                foreach(Particle p in solver.Objects) {
                    if(w.rect.Contains(new Vector2f(p.Position.X, p.Position.Y))) {
                        p.Forces += 1.0f * w.force / dt;
                    }
                }

                if(w.rect.Left > worldWidth) {
                    w.rect.Position = new Vector2f(-w.rect.Width, w.rect.Top);
                }
            }
        }
    }
}
