using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using YaEcs;
using YaEngine.Bootstrap;
using YaEngine.Core;
using YaEngine.Model;
using YaEngine.Physics;
using YaEngine.Render;

namespace YaDemo
{
    public class BuildPhysicsSceneSystem : IInitializeModelSystem
    {
        public int Priority => InitializePriorities.Third;

        public Task ExecuteAsync(IWorld world)
        {
            CreateCamera(world);
            CreateLight(world, Color.White.ToVector3());
            CreateGround(world);
            CreatePyramid(world, 10);
            return Task.CompletedTask;
        }

        private void CreatePyramid(IWorld world, int floors)
        {
            var random = new Random();
            for (var i = 0; i < floors; ++i)
            {
                var width = floors - i;
                var height = i + 1;
                for (var x = 0; x < width; ++x)
                for (var z = 0; z < width; ++z)
                {
                    var position = new Vector3(x, height, z);
                    var color = Color.FromArgb(255, random.Next(255), random.Next(255), random.Next(255));
                    CreateCube(world, position, color, 1);
                }
            }
        }

        private void CreateRandomCubes(IWorld world, int count, int range)
        {
            var random = new Random();
            for (var i = 0; i < count; ++i)
            {
                var position = new Vector3(random.Next(-range, range), random.Next(2, 20), random.Next(-range, range));
                var color = Color.FromArgb(255, random.Next(255), random.Next(255), random.Next(255));
                CreateCube(world, position, color, 1);   
            }
        }

        private void CreateGround(IWorld world)
        {
            world.Create(new Transform
                {
                    Position = new Vector3(0, 0, 0),
                    Scale = new Vector3(50, 1, 50)
                },
                new RendererInitializer
                {
                    Material = new MaterialInitializer
                    {
                        ShaderInitializer = DiffuseShader.Value,
                        Vector4Uniforms = new Dictionary<string, Vector4>
                        {
                            ["uTexture0Uv"] = new(8, 8, 0, 0)
                        }
                    },
                    Mesh = Cube.Mesh,
                },
                new ColliderInitializer
                {
                });
        }

        private static void CreateCube(IWorld world, Vector3 position, Color color, float mass)
        {
            var lightParentTransform = new Transform
            {
                Position = position,
            };
            world.Create(
                new Transform
                {
                    Position = position,
                    //Parent = lightParentTransform
                },
                new RendererInitializer
                {
                    Material = new MaterialInitializer
                    {
                        ShaderInitializer = DiffuseColorShader.Value,
                        Vector4Uniforms =
                        {
                            ["uColor"] = color.ToVector4()
                        }
                    },
                    Mesh = Cube.Mesh
                },
                new ColliderInitializer
                {
                    Mass = mass
                });
        }

        private static void CreateLight(IWorld world, Vector3 color)
        {
            var lightParentTransform = new Transform
            {
                Position = new Vector3(-4, 11, 15),
            };
            world.Create(
                new Transform
                {
                    Parent = lightParentTransform
                },
                new AmbientLight { Color = color },
                new RendererInitializer
                {
                    Material = new MaterialInitializer
                    {
                        ShaderInitializer = ColorShader.Value,
                        Vector4Uniforms = new Dictionary<string, Vector4>
                        {
                            ["uColor"] = new(color, 1f)
                        }
                    },
                    Mesh = Quad.Mesh,
                    CullFace = false
                });
        }

        private static void CreateCamera(IWorld world)
        {
             world.Create(
                new Camera { Fov = 45 },
                new Transform
                {
                    Position = new Vector3(-4, 11, 15),
                    Rotation = MathUtils.FromEulerDegrees(34, 160, 0)
                });
        }
    }
}