using System;
using System.Collections.Generic;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using OpenTK;

namespace ExampleShared
{
    public static class ShapeConstructor
    {
        public static float[] ConstructShapeFromBody(Body body)
        {
            List<float> vertices = new List<float>();
            const float unitToPixels = 100f;

            foreach (Fixture fixture in body.FixtureList)
            {
                Type shapeType = fixture.Shape.GetType();

                if (shapeType == typeof(PolygonShape))
                {
                    PolygonShape polygon = (PolygonShape)fixture.Shape;
                    for (int i = 0; i < polygon.Vertices.Count; i++)
                    {
                        vertices.Add(polygon.Vertices[i].X * unitToPixels);
                        vertices.Add(polygon.Vertices[i].Y * unitToPixels);
                    }
                }
                else if (shapeType == typeof(CircleShape))
                {
                    CircleShape circle = (CircleShape)fixture.Shape;

                    const int sides = 32;
                    float div = MathHelper.TwoPi / sides;

                    for (int i = 0; i < sides; i++)
                    {
                        vertices.Add((float)Math.Cos(div * i) * circle.Radius * unitToPixels);
                        vertices.Add((float)Math.Sin(div * i) * circle.Radius * unitToPixels);
                    }
                }
            }

            return vertices.ToArray();
        }
    }
}
