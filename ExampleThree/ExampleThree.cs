using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using ExampleShared;

namespace ExampleThree
{
    public class ExampleThree : GameWindow
    {
        private ShapeRenderer shapeRenderer;
        private World world;
        private Random rng = new Random();
        
        private GamePadState lastState;
        private Shape[] geometry;
        
        private Body rocket;
        private Shape rocketShape;

        public ExampleThree()
            : base(800, 450, GraphicsMode.Default, "Farseer Physics - Example Three")
        {
            GL.ClearColor(Color4.White);

            shapeRenderer = new ShapeRenderer(800, 450);
            lastState = GamePad.GetState(0);

            world = new World(new Vector2(0f, 9.8f));

            geometry = MapLoader.LoadMap(world, "Maps/physics_map.json");
            initRocket();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shapeRenderer.Begin();

            float cameraScale = 2f;
            Vector2 rocketPosition = ConvertUnits.ToDisplayUnits(rocket.Position);
            Vector2 cameraCenter = -rocketPosition + new Vector2(400f, 225f) / cameraScale;

            //Set camera
            shapeRenderer.SetCamera(
                Matrix4.CreateTranslation(cameraCenter.X, cameraCenter.Y, 0f) * 
                Matrix4.CreateScale(cameraScale));

            //Draw world geometry
            shapeRenderer.SetTransform(Matrix4.Identity);
            foreach (Shape shape in geometry)
                shapeRenderer.DrawShape(shape.Data, Color4.Black);

            //Draw player rocket
            shapeRenderer.SetTransform(
                Matrix4.CreateRotationZ(rocket.Rotation) * 
                Matrix4.CreateTranslation(rocketPosition.X, rocketPosition.Y, 0f));
            shapeRenderer.DrawShape(rocketShape.Data, Color4.Red);

            shapeRenderer.End();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GamePadState gpadState = GamePad.GetState(0);
            
            /*if (gpadState.ThumbSticks.Left.LengthSquared > 0.01f)
            {
                Vector2 leftStick = gpadState.ThumbSticks.Left / 4f;
                leftStick.Y = -leftStick.Y;
                rocket.ApplyForce(leftStick);
            }*/
            if (gpadState.ThumbSticks.Left.LengthSquared > 0.01f)
            {
                Vector2 leftStick = gpadState.ThumbSticks.Left;
                rocket.Rotation = (float)Math.Atan2(leftStick.X, leftStick.Y);

                //Only apply horizontal thrust
                leftStick.Y = 0f;
                rocket.ApplyForce(leftStick / 6f);
            }
            if (gpadState.Buttons.A == ButtonState.Pressed)
            {
                float x = (float)Math.Sin(rocket.Rotation);
                float y = -(float)Math.Cos(rocket.Rotation);

                rocket.ApplyForce(new Vector2(0f, y) / 5f);
            }

            //Constant timestep
            world.Step(0.01f);
            lastState = gpadState;
        }

        private void initRocket()
        {
            const float height = 8f / 2f;
            const float width = 5f / 2f;

            Vertices vertices = new Vertices(3);
            vertices.Add(ConvertUnits.ToSimUnits(new Vector2(  0f, -height)));
            vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-width, height)));
            vertices.Add(ConvertUnits.ToSimUnits(new Vector2(width, height)));

            rocket = BodyFactory.CreatePolygon(world, vertices, 5f);
            rocket.BodyType = BodyType.Dynamic;
            rocket.Position = ConvertUnits.ToSimUnits(new Vector2(100f, 50f));
            rocket.AngularDamping = 300f;
            rocket.LinearDamping = 8f;

            rocketShape = new Shape();
            rocketShape.Data = new float[6];

            rocketShape.Data[0] = 0f;
            rocketShape.Data[1] = -height;

            rocketShape.Data[2] = -width;
            rocketShape.Data[3] = height;

            rocketShape.Data[4] = width;
            rocketShape.Data[5] = height;
        }
    }
}
