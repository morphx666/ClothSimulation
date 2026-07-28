using SFML.System;
using System;

namespace ClothSimulation.Classes.Engine.Common {
    public static class MathHelper {
        public static float SigM(float x) {
            return (float)(1.0 / (1.0 + Math.Exp(-x)));
        }

        public static float SigM0(float x) {
            return SigM(x) - 0.5f;
        }
    }

    public static class MathVec2 {
        public static float Length(Vector2f v) {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        public static float Angle(Vector2f v1, Vector2f v2) {
            float dot = (float)(v1.X * v2.X + v1.Y * v2.Y);
            float det = (float)(v1.X * v2.Y - v1.Y * v2.X);
            return (float)Math.Atan2(det, dot);
        }

        public static float Dot(Vector2f v1, Vector2f v2) {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static float Cross(Vector2f v1, Vector2f v2) {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public static Vector2f Normal(Vector2f v) {
            return new Vector2f(-v.Y, v.X);
        }

        public static Vector2f Rotate(Vector2f v, float angle) {
            float ca = (float)Math.Cos(angle);
            float sa = (float)Math.Sin(angle);
            return new Vector2f(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        public static Vector2f Normalize(Vector2f v) {
            return v / Length(v);
        }
    };
}