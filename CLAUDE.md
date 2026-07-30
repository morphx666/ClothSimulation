# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A C# port of johnBuffer's [ClothSimulation](https://github.com/johnBuffer/ClothSimulation) (originally C++/SFML). Single .NET 10 console executable using SFML.Net 3.0 for windowing and rendering. No test project, no linter config.

## Commands

```bash
dotnet build                       # builds to Release/net10.0/ (both Debug and Release configs)
dotnet build -c Release
dotnet run --project ClothSimulation   # opens the simulation window (requires a display)
```

`OutputPath` is overridden to `..\Release\` for both configurations, so `Debug` and `Release` builds overwrite the same output directory. `Release/` is gitignored.

## Architecture

Verlet-style particle simulation with distance constraints. Everything is driven from a single loop in [Program.cs](ClothSimulation/Program.cs) — there is no game/engine framework layer.

**Simulation pipeline** — `PhysicSolver.Update(dt)` ([PhysicSolver.cs](ClothSimulation/Classes/Engine/Physics/PhysicSolver.cs)) splits the frame into `subSteps` (default 16) and per substep runs: apply gravity → apply air friction → integrate positions → solve constraints (`solverIterations` passes, default 1) → recompute velocities from position delta and zero out forces. Broken links are culled once at the top of `Update`, before any substep.

**Two-phase integration** — `Particle.Update` does semi-implicit Euler from accumulated `Forces`; `Particle.UpdateDerivatives` then *recomputes* `Velocity` from `(Position - positionOld) / dt`. This is what makes constraint solving behave like Verlet: `LinkConstraint.Solve` moves particles directly via `Particle.Move` (position projection, no forces), and the derivative pass folds that displacement back into velocity. Any new constraint type must follow the same rule — mutate `Position`, never `Velocity`.

**Force accumulation contract** — external systems (wind, mouse drag) add into `Particle.Forces` *between* solver updates. `Forces` is reset every substep by `UpdateDerivatives`, so anything writing forces must do so each frame before `solver.Update`. Because forces survive only into the first substep, `WindManager.Update`'s division by `dt` cancels the `dt` in that substep's integration — a gust's impulse works out to `force / subSteps` regardless of frame length. Wind and mouse drag are therefore unaffected by frame duration, but *are* scaled by `subSteps` and applied once per frame (a higher frame rate means more total wind).

**Pinning** — `Particle.IsMoving = false` makes a particle immovable; both `Update` and `Move` early-return. The top row of the cloth is pinned in `Program.Main`.

**Link breaking** — `LinkConstraint` sets `Broken = true` inside `Solve` when elongation exceeds `MaxElongationRatio`; the solver removes it on the next `Update`. Erasing with the right mouse button removes particles from `solver.Objects` and marks their links `Broken` — links hold direct `Particle` references, so failing to mark them would leave the renderer drawing lines to detached particles.

**Rendering** — [Renderer.cs](ClothSimulation/Classes/Renderer.cs) rebuilds a single `VertexArray` of `PrimitiveType.Lines` (two vertices per constraint) every frame. Particles are never drawn; the cloth is entirely the link mesh.

## Gotchas

- The main loop targets 60 FPS via `SetFramerateLimit` and measures the real frame time with an SFML `Clock`, so `dt` is **variable**. It is clamped to `[1/1000, 1/20]` — the lower bound keeps `UpdateDerivatives` and `WindManager.Update` from dividing by zero, the upper bound stops a stalled frame (window drag, breakpoint) from blowing up the solver. Anything reading `dt` must tolerate it changing frame to frame.
- Cloth topology is index-arithmetic based (`idx - 1` for the left neighbor, `idx - clothWidth` for the top). It assumes particles are added in row-major order and that `Objects` indices are stable; particle removal during erasing invalidates those assumptions, which is why links keep object references rather than indices.
- SFML native libraries ship with the `SFML.Graphics` NuGet package under `Release/net10.0/runtimes/` — do not add or manage them manually.
