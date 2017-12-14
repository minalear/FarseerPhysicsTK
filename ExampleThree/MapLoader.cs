using System;
using System.IO;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Newtonsoft.Json;
using OpenTK;

namespace ExampleThree
{
    public static class MapLoader
    {
        public static Shape[] LoadMap(World world, string path)
        {
            MapFile map = JsonConvert.DeserializeObject<MapFile>(File.ReadAllText(path));
            var geometry = new List<Shape>();

            int layerIndex = -1;
            for (int i = 0; i < map.Layers.Length; i++)
            {
                if (map.Layers[i].Name == "Collision")
                {
                    layerIndex = i;
                    break;
                }
            }

            //Cannot find collision layer
            if (layerIndex == -1)
                throw new InvalidOperationException();

            for (int i = 0; i < map.Layers[layerIndex].Objects.Length; i++)
            {
                ObjectInfo obj = map.Layers[layerIndex].Objects[i];
                if (obj.Polygon == null) continue;

                Vertices polygon = new Vertices(obj.Polygon.Length);

                List<float> data = new List<float>();
                for (int k = 0; k < obj.Polygon.Length; k++)
                {
                    //Adjust origin
                    Vector2 vertex = ConvertUnits.ToSimUnits(obj.Polygon[k]);
                    polygon.Add(vertex);

                    data.Add(obj.Polygon[k].X + obj.X);
                    data.Add(obj.Polygon[k].Y + obj.Y);
                }

                Body body = BodyFactory.CreatePolygon(world, polygon, 1f, ConvertUnits.ToSimUnits(new Vector2(obj.X, obj.Y)), 0f, BodyType.Static);
                geometry.Add(new Shape() { Data = data.ToArray() });
            }

            return geometry.ToArray();
        }
    }

    public struct Shape
    {
        public float[] Data;
    }

    public class MapFile
    {
        public string BackgroundColor;
        public int Width, Height;
        public int NextObjectID;
        public LayerInfo[] Layers;
    }
    public class LayerInfo
    {
        public string Name;
        public ObjectInfo[] Objects;
    }
    public class ObjectInfo
    {
        public string Name;
        public int ID;
        public float X, Y;
        public float Width, Height;
        public Vector2[] Polygon;
    }
}
