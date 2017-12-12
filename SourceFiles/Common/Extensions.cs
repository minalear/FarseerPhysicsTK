using OpenTK;

namespace FarseerPhysics.Common
{
    public static class Extensions
    {
        public static float Distance(this Vector2 v1, Vector2 v2)
        {
            return (v2 - v1).Length;
        }
        public static float DistanceSquared(this Vector2 v1, Vector2 v2)
        {
            return (v2 - v1).LengthSquared;
        }
    }
}
