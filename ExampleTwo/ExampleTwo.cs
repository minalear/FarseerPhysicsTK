using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using ExampleShared;

namespace ExampleTwo
{
    public class ExampleTwo : GameWindow
    {
        private ShapeRenderer shapeRenderer;

        private World world;
        private Body rocket;
        private Body platform;
        private List<Body> boxes;

        private float[] rocketShape;
        private GamePadState lastState;
        private Random rng;

        //Conversion between Farseer units and screen pixels
        const float unitToPixel = 100f;
        const float pixelToUnit = 1 / unitToPixel;

        public ExampleTwo()
            : base(800, 450, GraphicsMode.Default, "Farseer Physics - Example Two")
        {
            GL.ClearColor(Color4.White);

            shapeRenderer = new ShapeRenderer(800, 450);
            lastState = GamePad.GetState(0);
            rng = new Random();

            world = new World(new Vector2(0f, 9.8f));
            boxes = new List<Body>();

            //rocket = BodyFactory.CreateCapsule(world, 48f * pixelToUnit, 12f * pixelToUnit, 1f, new Vector2(400f, 225f));
            rocket = BodyFactory.CreateCircle(world, 24f * pixelToUnit, 1f);
            rocket.BodyType = BodyType.Dynamic;
            rocket.Position = new Vector2(400f, 225f) * pixelToUnit;
            //rocket.FixedRotation = true;

            platform = BodyFactory.CreateRectangle(world, 800f * pixelToUnit, 800f * pixelToUnit, 1f);
            platform.BodyType = BodyType.Static;
            platform.Position = new Vector2(400f, 825f) * pixelToUnit;

            rocketShape = ShapeConstructor.ConstructShapeFromBody(rocket);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shapeRenderer.Begin();

            //Create matrix transforms based on the body's rotation and position
            Vector2 rocketPosition = rocket.Position * unitToPixel;
            shapeRenderer.SetTransform(
                Matrix4.CreateRotationZ(rocket.Rotation) *
                Matrix4.CreateTranslation(rocketPosition.X, rocketPosition.Y, 0f));
            shapeRenderer.DrawShape(rocketShape, Color4.Black);

            shapeRenderer.ClearTransform();
            shapeRenderer.DrawRect(new Vector2(0f, 425f), new Vector2(800f, 25f), Color4.Red);

            foreach (Body box in boxes)
            {
                Vector2 boxPos = (box.Position * unitToPixel) - new Vector2(25f);
                shapeRenderer.SetTransform(
                    Matrix4.CreateTranslation(-boxPos.X - 25f, -boxPos.Y - 25f, 0f) * 
                    Matrix4.CreateRotationZ(box.Rotation) * 
                    Matrix4.CreateTranslation(boxPos.X + 25f, boxPos.Y + 25f, 0f));
                shapeRenderer.DrawRect(boxPos, new Vector2(50f), Color4.Brown);
            }

            shapeRenderer.End();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GamePadState gpadState = GamePad.GetState(0);

            //Apply thrust and movement
            if (gpadState.Buttons.B == ButtonState.Pressed)
                rocket.ApplyForce(new Vector2(0f, -2.5f));
            if (gpadState.ThumbSticks.Left.LengthSquared > 0.01f)
                rocket.ApplyForce(gpadState.ThumbSticks.Left * 1.5f);

            //Spawn box
            if (gpadState.Buttons.X == ButtonState.Released && lastState.Buttons.X == ButtonState.Pressed)
                spawnBox();

            //Constant timestep
            world.Step(0.01f);
            lastState = gpadState;
        }

        private void spawnBox()
        {
            Vector2 position = new Vector2((float)rng.NextDouble() * 800f, -100f) * pixelToUnit;
            Vector2 size = new Vector2(50f) * pixelToUnit;

            Body box = BodyFactory.CreateRectangle(world, size.X, size.Y, 1f, position);
            box.BodyType = BodyType.Dynamic;

            boxes.Add(box);
        }
    }
}
