using ClothSimulation.Classes.Engine.Physics;
using SFML.Graphics;
using System;

namespace ClothSimulation.Classes {
    public class Renderer {
        private PhysicSolver solver;
        private VertexArray va;

        public Renderer(PhysicSolver s) {
            solver = s;
            va = new(PrimitiveType.Lines);
        }

        public void UpdateVA() {
            int links_count = solver.Constraints.Count;
            va.Resize((UInt32)(2 * links_count));
            for(UInt32 i = 0; i < links_count; i++) {
                LinkConstraint current_link = solver.Constraints[(int)i];
                va[2 * i] = new(current_link.Particle1.Position);
                va[2 * i + 1] = new(current_link.Particle2.Position);
                //va[2 * i].Position = current_link.particle_1.position;
                //va[2 * i + 1].Position = current_link.particle_2.position;
            }
        }

        public void Render(RenderWindow w) {
            UpdateVA();
            w.Draw(va);
        }
    }
}
