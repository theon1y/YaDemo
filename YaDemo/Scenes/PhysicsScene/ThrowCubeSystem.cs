using System.Drawing;
using System.Linq;
using System.Numerics;
using Silk.NET.Input;
using YaEcs;
using YaEngine.Core;
using YaEngine.Input;
using YaEngine.Physics;
using YaEngine.Render;
using YaEngine.Render.OpenGL;

namespace YaDemo
{
    public class ThrowCubeSystem : IPhysicsSystem
    {
        public UpdateStep UpdateStep => PhysicsSteps.Raycast;
        
        public void Execute(IWorld world)
        {
            if (!world.TryGetSingleton(out InputContext input)) return;
            if (!input.IsKeyDown(Key.Space)) return;
            
            if (!world.TryGetSingleton(out CameraRegistry cameraRegistry)) return;
            
            var camera = cameraRegistry.Cameras.First();
            if (!world.TryGetComponent(camera, out Transform cameraTransform)) return;

            var forward = cameraTransform.GetWorldForward();
            var from = cameraTransform.Position + forward;
            var cube = SpawnCube(world, from + forward);
            ThrowCube(world, cube, forward * 10000);
        }

        private static void ThrowCube(IWorld world, Entity cube, Vector3 force)
        {
            if (!world.TryGetComponent(cube, out RigidBody rigidBody)) return;

            rigidBody.Force = force;
            rigidBody.Dampen = 0.92f;
        }

        private static Entity SpawnCube(IWorld world, Vector3 position)
        {
            if (!world.TryGetSingleton(out RenderApi renderApi) || renderApi is not GlRenderApi glRenderApi) return 0;
            if (!world.TryGetSingleton(out ShaderRegistry shaderRegistry)) return 0;
            if (!world.TryGetSingleton(out TextureRegistry textureRegistry)) return 0;
            if (!world.TryGetSingleton(out IShaderFactory shaderFactory)) return 0;
            if (!world.TryGetSingleton(out ITextureFactory textureFactory)) return 0;
            if (!world.TryGetSingleton(out Physics physics) || physics is not BulletPhysics bulletPhysics) return 0;

            var cubeTransform = new Transform { Position = position };
            var cube = world.Create(cubeTransform);
            
            var rendererInitializer = new RendererInitializer
            {
                Material = new MaterialInitializer
                {
                    ShaderInitializer = DiffuseColorShader.Value,
                    Vector4Uniforms =
                    {
                        ["uColor"] = Color.White.ToVector4()
                    }
                },
                Mesh = Cube.Mesh
            };
            
            var renderer = InitializeGlRenderSystem.InitializeRenderer(shaderFactory, textureFactory,
                rendererInitializer, shaderRegistry, textureRegistry, glRenderApi.Gl);
            world.AddComponent(cube, renderer);
            
            var colliderInitializer = new ColliderInitializer
            {
                Mass = 100.0f
            };
            CreateBulletShapesSystem.CreateRigidBody(world, cube, cubeTransform, colliderInitializer,
                bulletPhysics);
            return cube;
        }
    }
}