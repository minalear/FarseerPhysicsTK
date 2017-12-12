using System;
using OpenTK.Graphics.OpenGL4;

namespace ExampleShared
{
    public class VertexArray : IDisposable
    {
        private int vao, vbo;

        public VertexArray()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
        }

        public void SetBufferData(float[] bufferData, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, bufferData.Length * sizeof(float), bufferData, usage);
        }
        public void EnableAttribute(int id, int size, VertexAttribPointerType type, int stride, int offset)
        {
            GL.EnableVertexAttribArray(id);
            GL.VertexAttribPointer(id, size, type, false, stride * sizeof(float), offset * sizeof(float));
        }
        public void DisableAttribute(int id)
        {
            GL.DisableVertexAttribArray(id);
        }

        public void Bind()
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }
        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
        }

        public int ID { get { return vao; } }
    }
}
