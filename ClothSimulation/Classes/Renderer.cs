using ClothSimulation.Classes.Engine.Physics;
using SFML.Graphics;
using System;

namespace ClothSimulation.Classes {
    public class Renderer {
        private readonly PhysicSolver solver;
        private readonly VertexArray va;

        public Renderer(PhysicSolver s) {
            solver = s;
            va = new(PrimitiveType.Lines);
        }

        public void UpdateVertexArrayFromConstraints() {
            int linksCount = solver.Constraints.Count;
            va.Resize((UInt32)(2 * linksCount));
            for(UInt32 i = 0; i < linksCount; i++) {
                LinkConstraint currentLink = solver.Constraints[(int)i];
                va[2 * i] = new(currentLink.Particle1.Position);
                va[2 * i + 1] = new(currentLink.Particle2.Position);
            }
        }

        public void Render(RenderWindow w) {
            UpdateVertexArrayFromConstraints();
            w.Draw(va);
        }
    }
}
