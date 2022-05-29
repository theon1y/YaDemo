using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YaEcs;
using YaEngine.Animation;
using YaEngine.Audio;
using YaEngine.Bootstrap;
using YaEngine.Core;
using YaEngine.Import;
using YaEngine.Model;
using YaEngine.Render;
using YaEngine.VFX.ParticleSystem;
using YaEngine.VFX.ParticleSystem.Modules;
using YaEngine.VFX.ParticleSystem.Modules.Shapes;
using YaEngine.VFX.ParticleSystem.Modules.Value;
using YaEngine.VFX.ParticleSystem.Shaders;

namespace YaDemo
{
    public class BuildAnimationsSceneSystem : IInitializeModelSystem
    {
        public int Priority => InitializePriorities.Third;
        
        private readonly ModelImporter modelImporter;
        private readonly ILogger<BuildAnimationsSceneSystem> logger;

        public BuildAnimationsSceneSystem(ModelImporter modelImporter, ILogger<BuildAnimationsSceneSystem> logger)
        {
            this.modelImporter = modelImporter;
            this.logger = logger;
        }
        
        public async Task ExecuteAsync(IWorld world)
        {
            CreateCamera(world);
            CreateLight(world, Color.IndianRed.ToVector3());
            
            await CreateCharacter(world);
            
            CreateMusic(world);
            CreateParticleSystem(world);
        }

        private static void CreateParticleSystem(IWorld world)
        {
            var particleTexturePath = "Assets/Textures/particle.png";
            var particleTexture = GetTexture(particleTexturePath);
            var light = world.Entities
                .FirstOrDefault(x => world.TryGetComponent<AmbientLight>(x, out _));
            world.TryGetComponent(light, out Transform lightTransform);
            world.Create(new Transform { Parent = lightTransform, Position = lightTransform.Position }, 
                new ParticleEffect
            {
                Material = new MaterialInitializer
                {
                    ShaderInitializer = BillboardParticleShader.Value,
                    TextureInitializer = particleTexture,
                    Blending = Blending.Additive
                },
                Mesh = Quad.Mesh,
                MaxParticles = 20,
                Modules = new List<IModule>
                {
                    new EmissionModule
                    {
                        Rate = 10f,
                        Duration = 5,
                        IsLooping = true,
                        ParticleLifetime = new Vector2(1, 2),
                        ParticleSpeed = new Vector2(1, 3)
                    },
                    new ShapeModule { Shape = new ConeShape(45) },
                    new LifetimeModule(),
                    new ColorModule { Provider = new InterpolateVector4(Color.Red.ToVector4(), Color.Transparent.ToVector4()) },
                    new ScaleModule { Provider = new InterpolateVector3(Vector3.One, Vector3.One * 0.5f)  },
                    new RotateModule { Provider = new Constant<Quaternion>(Quaternion.Identity) },
                    new MoveModule(),
                }
            });
        }

        private static TextureInitializer GetTexture(string texturePath)
        {
            var textureName = Path.GetFileNameWithoutExtension(texturePath);
            var charTexture = new TextureInitializer(textureName, new FileTextureProvider(texturePath));
            return charTexture;
        }

        private static void CreateMusic(IWorld world)
        {
            world.Create(new Music(),
                new AudioInitializer
                {
                    AudioProvider =
                        new Mp3AudioProvider("Assets/Audio/price-of-freedom-33106.mp3")
                });
        }

        private async Task CreateCharacter(IWorld world)
        {
            var options = new ImportOptions(Scale: 0.1f);
            
            var modelPath = "Assets/Models/BH-2 Free.fbx";
            var modelImport = modelImporter.Import(modelPath, options);
            var meshes = modelImport.Meshes;
            var avatar = modelImport.Avatar;
            logger.LogInformation("Imported meshes: {0}",
                string.Join(", ", meshes.Select(x => x.Name)));
            
            var animationsPath = "Assets/Animations";
            var animationImports = await ImportAnimations(animationsPath, options,
                (filePath, _) => Path.GetFileNameWithoutExtension(filePath));
            var animations = animationImports
                .SelectMany(x => x.Animations)
                .Concat(modelImport.Animations)
                .ToList();
            logger.LogInformation("Imported animations: {0}",
                string.Join(", ", animations.Select(x => x.Name)));
            
            var albedoPath = "Assets/Textures/BH-2_AlbedoTransparency.png";
            var albedoTexture = GetTexture(albedoPath);
            var specularPath = "Assets/Textures/BH-2_SpecularSmoothness.png";
            var specularTexture = GetTexture(specularPath);
            var normalPath = "Assets/Textures/BH-2_Normal.png";
            var normalTexture = GetTexture(normalPath);
            var material = new MaterialInitializer
            {
                ShaderInitializer = DiffuseAnimationShader.Value,
                TextureInitializer = albedoTexture,
                TextureUniforms =
                {
                    ["uSpecularTexture0"] = specularTexture,
                    ["uNormalTexture0"] = normalTexture
                },
                FloatUniforms =
                {
                    ["uAmbientStrength"] = 1f,
                    ["uDiffuseStrength"] = 1f,
                    ["uSpecularStrength"] = 1f,
                    ["uShiness"] = 64
                }
            };

            var animator = new Animator(animations, avatar);
            var characterTransform = new Transform
            {
                Position = new Vector3(0f, 0f, 5f),
                Scale = Vector3.One * 0.5f,
            };
            var character = world.Create(characterTransform);
            var transforms = new Transform[avatar.Hierarchy.Length - 1];
            world.AddComponent(character, animator);
            for (var i = 0; i < transforms.Length; ++i)
            {
                var avatarNode = avatar.Hierarchy[i];
                var parentTransform = avatarNode.ParentIndex < 0 ? characterTransform : transforms[avatarNode.ParentIndex];
                var transform = new Transform { Parent = parentTransform };
                transform.SetLocalTransform(avatarNode.LocalTransform);
                transforms[i] = transform;
                
                foreach (var meshIndex in avatarNode.MeshIndexes)
                {
                    var mesh = modelImport.Meshes[meshIndex];
                    var renderer = world.Create(transform,
                        new RendererInitializer
                        {
                            Material = material,
                            Mesh = mesh,
                            BoneMatrices = animator.BoneMatrices
                        });
                }
            }
        }

        private static void CreateLight(IWorld world, Vector3 color)
        {
            var lightParentTransform = new Transform
            {
                Position = new Vector3(0f, 5f, 5f),
            };
            world.Create(
                new Transform
                {
                    Parent = lightParentTransform
                },
                new AmbientLight { Color = color });
        }

        private static void CreateCamera(IWorld world)
        {
             world.Create(
                new Camera { Fov = 45 },
                new Transform
                {
                    Position = new Vector3(-4, 11, 15),
                    Rotation = MathUtils.FromEulerDegrees(30, 165, 4)
                });
        }

        private async Task<ModelImporterResult[]> ImportAnimations(string path, ImportOptions options,
            Func<string, string, string> nameGenerator)
        {
            if (string.IsNullOrEmpty(path)) return Array.Empty<ModelImporterResult>();
            
            var animationPaths = new DirectoryInfo(path)
                .GetFiles()
                .Select(x => x.FullName);
            var tasks = animationPaths
                .Select(async path =>
                {
                    var import = await Task.Run(() => modelImporter.Import(path, options));
                    foreach (var animation in import.Animations)
                    {
                        animation.Name = nameGenerator(path, animation.Name);
                    }
                    return import;
                })
                .ToArray();
            await Task.WhenAll(tasks);
            return tasks
                .Select(x => x.Result)
                .ToArray();
        }
    }
}