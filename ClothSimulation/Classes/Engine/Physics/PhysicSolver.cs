using SFML.System;
using System;
using System.Collections.Generic;

namespace ClothSimulation.Classes.Engine.Physics {
    public class PhysicSolver {
        public readonly List<Particle> Objects = [];
        public readonly List<LinkConstraint> Constraints = [];

        // Simulator iterations count
        private readonly int solverIterations;
        private readonly int subSteps;
        private const float frictionCoefficient = 0.5f;
        private readonly List<LinkConstraint> toBeRemoved = [];

        public PhysicSolver(int iterations = 1, int subSteps = 16) {
            solverIterations = iterations;
            this.subSteps = subSteps;
        }

        public void Update(float dt) {
            float subStepDt = dt / subSteps;
            RemoveBrokenLinks();
            for(int i = subSteps; i-- > 0;) {
                ApplyGravity();
                ApplyAirFriction();
                UpdatePositions(subStepDt);
                SolveConstraints();
                UpdateDerivatives(subStepDt);
            }
        }

        public void ApplyGravity() {
            Vector2f gravity = new(0.0f, 1500.0f);
            foreach(Particle p in Objects) {
                p.Forces += gravity * p.Mass;
            }
        }

        public void ApplyAirFriction() {
            foreach(Particle p in Objects) {
                p.Forces -= p.Velocity * frictionCoefficient;
            }
        }

        public void UpdatePositions(float dt) {
            foreach(Particle p in Objects) {
                p.Update(dt);
            }
        }

        public void UpdateDerivatives(float dt) {
            foreach(Particle p in Objects) {
                p.UpdateDerivatives(dt);
            }
        }

        public void SolveConstraints() {
            for(int i = solverIterations; i-- > 0;) {
                Constraints.ForEach(c => c.Solve());
            }
        }

        public void RemoveBrokenLinks() {
            toBeRemoved.Clear();
            foreach(LinkConstraint l in Constraints) {
                if(!l.IsValid()) toBeRemoved.Add(l);
            }
            toBeRemoved.ForEach(l => Constraints.Remove(l));
        }

        public UInt32 AddParticle(Vector2f position) {
            Objects.Add(new Particle(position));
            return (UInt32)(Objects.Count - 1);
        }

        public void AddLink(UInt32 particle1Idx, UInt32 particle2Idx, float maxElongationRatio = 1.5f) {
            LinkConstraint lc = new(Objects[(int)particle1Idx], Objects[(int)particle2Idx]);
            Constraints.Add(lc);
            lc.MaxElongationRatio = maxElongationRatio;
        }
    }
}