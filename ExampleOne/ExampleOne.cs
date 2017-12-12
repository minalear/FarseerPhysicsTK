using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using ExampleShared;

namespace ExampleOne
{
    public class ExampleOne : GameWindow
    {
        private ShapeRenderer shapeRenderer;

        private World world;
        private Body body01, body02;

        //Conversion between Farseer units and screen pixels
        const float unitToPixel = 100f;
        const float pixelToUnit = 1 / unitToPixel;

        public ExampleOne()
            : base(800, 450, GraphicsMode.Default, "Farseer Physics - Example One")
        {
            GL.ClearColor(Color4.White);

            shapeRenderer = new ShapeRenderer(800, 450);

            world = new World(new Vector2(0f, 9.8f));

            //Create two rectangles, one that doesn't move and acts like a platform, and another that's a dynamic box

            Vector2 size = new Vector2(50f, 50f);
            body01 = BodyFactory.CreateRectangle(world, size.X * pixelToUnit, size.Y * pixelToUnit, 1f);
            body01.BodyType = BodyType.Dynamic;
            body01.Position = new Vector2(400f, 0f) * pixelToUnit;

            size = new Vector2(800f, 50f);
            body02 = BodyFactory.CreateRectangle(world, size.X * pixelToUnit, size.Y * pixelToUnit, 1f);
            body02.BodyType = BodyType.Kinematic;
            body02.Position = new Vector2(400f, 225f) * pixelToUnit;
            body02.Rotation = MathHelper.PiOver6;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shapeRenderer.Begin();

            //Create matrix transforms based on the body's rotation and position
            Vector2 body01Position = (body01.Position * unitToPixel) - new Vector2(25f, 25f);
            shapeRenderer.SetTransform(
                Matrix4.CreateTranslation(-body01Position.X - 25f, -body01Position.Y - 25f, 0f) *
                Matrix4.CreateRotationZ(body01.Rotation) *
                Matrix4.CreateTranslation(body01Position.X + 25f, body01Position.Y + 25f, 0f));
            shapeRenderer.DrawRect(body01Position, new Vector2(50f), Color4.Black);

            Vector2 body02Position = (body02.Position * unitToPixel) - new Vector2(400f, 25f);
            shapeRenderer.SetTransform(
                Matrix4.CreateTranslation(-body02Position.X - 400f, -body02Position.Y - 25f, 0f) *
                Matrix4.CreateRotationZ(body02.Rotation) *
                Matrix4.CreateTranslation(body02Position.X + 400f, body02Position.Y + 25f, 0f));
            shapeRenderer.DrawRect(body02Position, new Vector2(800f, 50f), Color4.Black);
            shapeRenderer.End();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //Constant timestep
            world.Step(0.01f);
        }
    }
}
