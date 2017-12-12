using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ExampleShared
{
    public class ShapeRenderer : IDisposable
    {
        private ShaderProgram shader;
        private VertexArray vertexArray;

        public ShapeRenderer(int width, int height)
        {
            string vertexSource = System.IO.File.ReadAllText("Shaders/basic_vertex.glsl");
            string fragmentSource = System.IO.File.ReadAllText("Shaders/basic_fragment.glsl");

            shader = new ShaderProgram(vertexSource, fragmentSource);

            vertexArray = new VertexArray();
            vertexArray.Bind();
            vertexArray.SetBufferData(new float[] { 0f, 0f }, BufferUsageHint.DynamicDraw);
            vertexArray.EnableAttribute(0, 2, VertexAttribPointerType.Float, 2, 0);
            vertexArray.Unbind();

            shader.Use();
            shader.SetMatrix4("proj", false, Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f));
            shader.SetColor4("drawColor", Color4.Black);
            ClearTransform(); //Set model transform
            ClearCamera();
        }

        public void Resize(int width, int height)
        {
            shader.SetMatrix4("proj", false, Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f));
        }

        public void Begin()
        {
            shader.Use();
            vertexArray.Bind();
        }
        public void End()
        {
            vertexArray.Unbind();
            shader.Clear();
        }

        public void SetTransform(Matrix4 matrix)
        {
            shader.SetMatrix4("model", false, matrix);
        }
        public void ClearTransform()
        {
            shader.SetMatrix4("model", false, Matrix4.Identity);
        }
        public void SetCamera(Matrix4 matrix)
        {
            shader.SetMatrix4("view", false, matrix);
        }
        public void ClearCamera()
        {
            shader.SetMatrix4("view", false, Matrix4.Identity);
        }

        public void DrawCircle(Vector2 position, float radius, int sides, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createCircleVertexInfo(position, radius, sides);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, sides);
        }
        public void DrawRect(Vector2 position, Vector2 size, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createRectVertexInfo(position, size);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);
        }
        public void DrawLine(Vector2 pos1, Vector2 pos2, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createLineVertexInfo(pos1, pos2);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
        }
        public void DrawArc(Vector2 center, float radius, int sides, float startAngle, float radians, Color4 color)
        {
            //Adjust the sides to match a full circle sides value
            sides = (int)(sides / (MathHelper.TwoPi / radians));
            shader.SetColor4("drawColor", color);

            createArcVertexInfo(center, radius, sides, startAngle, radians);
            GL.DrawArrays(PrimitiveType.Lines, 0, sides * 2);
        }
        public void DrawShape(float[] vertices, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, vertices.Length / 2);
        }
        public void DrawShape(float[] vertices, int count, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, count);
        }
        public void DrawLineStrip(float[] vertices, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, vertices.Length / 2);
        }
        public void DrawPixel(Vector2 position, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createShapeVertexInfo(new float[] { position.X, position.Y });
            GL.DrawArrays(PrimitiveType.Points, 0, 2);
        }
        public void DrawPixels(float[] vertices, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length / 2);
        }

        public void FillCircle(Vector2 position, float radius, int sides, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createCircleVertexInfo(position, radius, sides);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, sides);
        }
        public void FillRect(Vector2 position, Vector2 size, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createRectVertexInfo(position, size);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
        }
        public void FillShape(float[] vertices, Color4 color)
        {
            shader.SetColor4("drawColor", color);

            createShapeVertexInfo(vertices);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, vertices.Length / 2);
        }

        public void Dispose()
        {
            shader.Dispose();
            vertexArray.Dispose();
        }

        private void createCircleVertexInfo(Vector2 center, float radius, int sides)
        {
            float[] verts = new float[sides * 2];

            double div = MathHelper.TwoPi / sides;
            for (int i = 0; i < sides; i++)
            {
                verts[0 + i * 2] = (float)(Math.Cos(div * i) * radius) + center.X;
                verts[1 + i * 2] = (float)(Math.Sin(div * i) * radius) + center.Y;
            }

            createShapeVertexInfo(verts);
        }
        private void createArcVertexInfo(Vector2 center, float radius, int sides, float startAngle, float radians)
        {
            float[] verts = new float[sides * 4];

            double div = radians / sides;
            for (int i = 0; i < sides; i++)
            {
                verts[0 + i * 4] = (float)(Math.Cos(div * i + startAngle) * radius) + center.X;
                verts[1 + i * 4] = (float)(Math.Sin(div * i + startAngle) * radius) + center.Y;

                verts[2 + i * 4] = (float)(Math.Cos(div * (i + 1) + startAngle) * radius) + center.X;
                verts[3 + i * 4] = (float)(Math.Sin(div * (i + 1) + startAngle) * radius) + center.Y;
            }

            createShapeVertexInfo(verts);
        }
        private void createRectVertexInfo(Vector2 position, Vector2 size)
        {
            float[] verts = new float[8];

            //0.5 offset to fix rendering issues
            verts[0] = position.X - 0.5f;
            verts[1] = position.Y - 0.5f;

            verts[2] = position.X - 0.5f;
            verts[3] = position.Y + size.Y + 0.5f;

            verts[4] = position.X + size.X + 0.5f;
            verts[5] = position.Y + size.Y + 0.5f;

            verts[6] = position.X + size.X + 0.5f;
            verts[7] = position.Y - 0.5f;

            createShapeVertexInfo(verts);
        }
        private void createLineVertexInfo(Vector2 pos1, Vector2 pos2)
        {
            float[] verts = new float[4];

            verts[0] = pos1.X;
            verts[1] = pos1.Y;

            verts[2] = pos2.X;
            verts[3] = pos2.Y;

            createShapeVertexInfo(verts);
        }
        private void createShapeVertexInfo(float[] verts)
        {
            vertexArray.SetBufferData(verts, BufferUsageHint.DynamicDraw);
        }
    }
}
