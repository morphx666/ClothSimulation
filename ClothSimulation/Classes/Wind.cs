using ClothSimulation.Classes.Engine.Physics;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothSimulation.Classes {
    public class Wind {
        public FloatRect rect;
        public Vector2f force;

        public Wind(Vector2f s, Vector2f p, Vector2f f) {
            rect = new(p, s);
            force = f;
        }

        public void Update(float dt) {
            rect.Left += 1.0f * force.X * dt;
            //rect.top += force.y * dt;
        }
    }

    public class WindManager {
        public List<Wind> winds = new();
        public float world_width = 0.0f;

        public WindManager(float width) {
            world_width = width;
        }

        public void Update(PhysicSolver solver, float dt) {
            foreach(Wind w in winds) {
                w.Update(dt);
                foreach(Particle p in solver.Objects) {
                    if(w.rect.Contains(p.Position.X, p.Position.Y)) {
                        p.Forces += 1.0f * w.force / dt;
                    }
                }

                if(w.rect.Left > world_width) {
                    w.rect.Left = -w.rect.Width;
                }
            }
        }
    }
}
