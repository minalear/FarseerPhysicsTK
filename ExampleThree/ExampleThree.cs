using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using FarseerPhysics;
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
        private List<Body> spheres;

        public ExampleThree()
            : base(800, 450, GraphicsMode.Default, "Farseer Physics - Example Three")
        {
            GL.ClearColor(Color4.White);

            shapeRenderer = new ShapeRenderer(800, 450);
            lastState = GamePad.GetState(0);

            world = new World(new Vector2(0f, 9.8f));

            geometry = MapLoader.LoadMap(world, "Maps/physics_map.json");

            spheres = new List<Body>();
            for (int i = 0; i < 250; i++)
            {
                Vector2 position = ConvertUnits.ToSimUnits(new Vector2((float)rng.NextDouble() * 650f, -(float)rng.NextDouble() * 100f - 10f));
                spheres.Add(BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(6f), 1f, position, BodyType.Dynamic));
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shapeRenderer.Begin();
            shapeRenderer.SetCamera(
                //Matrix4.CreateTranslation(1200f, 250f, 0f) * 
                Matrix4.CreateScale(0.45f));
            foreach (Shape shape in geometry)
                shapeRenderer.DrawShape(shape.Data, Color4.Black);
            foreach (Body body in spheres)
                shapeRenderer.DrawCircle(ConvertUnits.ToDisplayUnits(body.Position), 6f, 24, Color4.Red);
            shapeRenderer.End();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GamePadState gpadState = GamePad.GetState(0);

            //Constant timestep
            world.Step(0.01f);
            lastState = gpadState;
        }
    }
}
