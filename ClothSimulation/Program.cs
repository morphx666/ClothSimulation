using ClothSimulation.Classes.Engine.Physics;
using ClothSimulation.Classes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using Color = SFML.Graphics.Color;
using System.Collections.Generic;

// From the code by johnBuffer @ https://github.com/johnBuffer/ClothSimulation
// https://www.youtube.com/watch?v=KpJzoFzDDMw

namespace ClothSimulation {
    public class Program {
        //[DllImport("user32.dll")]
        //private static extern bool GetCursorPos(ref Point lpPoint);

        public static void Main(string[] args) {
            UInt32 windowWidth = 1280;
            UInt32 windowHeight = 1024;
            RenderWindow window = new(new VideoMode(windowWidth, windowHeight), "Cloth Simulation", Styles.Close);
            window.SetActive(true);

            PhysicSolver solver = new();
            Renderer renderer = new(solver);

            UInt32 clothWidth = 100;
            UInt32 clothHeight = 70;
            float linksLength = 10.0f;
            float startX = (windowWidth - (clothWidth - 1) * linksLength) * 0.5f;

            // Initialize the cloth
            for(UInt32 y = 0; y < clothHeight; y++) {
                float max_elongation = 1.2f * (2.0f - y / (float)clothHeight);
                for(UInt32 x = 0; x < clothWidth; x++) {
                    UInt32 idx = solver.AddParticle(new Vector2f(startX + x * linksLength, y * linksLength));
                    
                    // Add left link if there is a particle on the left
                    if(x > 0) {
                        solver.AddLink(idx - 1, idx, max_elongation * 0.9f);
                    }

                    // Add top link if there is a particle on the top
                    if(y > 0) {
                        solver.AddLink(idx - clothWidth, idx, max_elongation);
                    } else {
                        // If not, pin the particle
                        solver.Objects[(int)idx].IsMoving = false;
                    }
                }
            }

            Vector2f mousePosition = new();
            Vector2f lastMousePosition = new();
            bool isDragging = false;
            bool isErasing = false;

            // Add events callback for mouse control
            window.MouseMoved += (object s, MouseMoveEventArgs e) => {
                mousePosition.X = e.X;
                mousePosition.Y = e.Y;
            };
            window.MouseButtonPressed += (object s, MouseButtonEventArgs e) => {
                switch(e.Button) {
                    case Mouse.Button.Left:
                        isDragging = true;
                        lastMousePosition = mousePosition;
                        break;
                    case Mouse.Button.Right:
                        isErasing = true;
                        break;
                }
            };
            window.MouseButtonReleased += (object s, MouseButtonEventArgs e) => {
                switch(e.Button) {
                    case Mouse.Button.Left:
                        isDragging = false;
                        break;
                    case Mouse.Button.Right:
                        isErasing = false;
                        break;
                }
            };
            window.Closed += (_, __) => window.Close();

            // Add 2 wind waves
            WindManager wind = new(windowWidth);
            wind.winds.Add(new(
                new Vector2f(100.0f, windowHeight),
                new Vector2f(0.0f, 0.0f),
                new Vector2f(1000.0f, 0.0f)
            ));
            wind.winds.Add(new(
                new Vector2f(20.0f, windowHeight),
                new Vector2f(0.0f, 0.0f),
                new Vector2f(3000.0f, 0.0f)
            ));

            // Main loop
            const float dt = 1.0f / 60.0f;
            List<Particle> toBeRemoved = new();
            while(window.IsOpen) {
                window.DispatchEvents();

                if(isDragging) {
                    // Apply a force on the particles in the direction of the mouse's movement
                    Vector2f mouseSpeed = mousePosition - lastMousePosition;
                    lastMousePosition = mousePosition;
                    ApplyForceOnCloth(mousePosition, 100.0f, mouseSpeed * 8000.0f, solver);
                }

                if(isErasing) {
                    // Delete all nodes that are in the range of the mouse
                    toBeRemoved.Clear();
                    foreach(Particle p in solver.Objects)
                        if(IsInRadius(p, mousePosition, 5.0f)) toBeRemoved.Add(p);
                    toBeRemoved.ForEach(p => {
                        solver.Objects.Remove(p);
                        foreach(LinkConstraint lc in solver.Constraints) {
                            if(lc.Particle1?.id == p.id || lc.Particle2?.id == p.id) lc.Broken = true;
                        }
                    });
                }

                // Update physics
                wind.Update(solver, dt);
                solver.Update(dt);

                window.Clear(Color.Black);
                renderer.Render(window);
                window.Display();
            }
        }

        private static bool IsInRadius(Particle p, Vector2f center, float radius) {
            Vector2f v = center - p.Position;
            return v.X * v.X + v.Y * v.Y < radius * radius;
        }


        private static void ApplyForceOnCloth(Vector2f position, float radius, Vector2f force, PhysicSolver solver) {
            foreach(Particle p in solver.Objects) {
                if(IsInRadius(p, position, radius)) p.Forces += force;
            }
        }
    }
}