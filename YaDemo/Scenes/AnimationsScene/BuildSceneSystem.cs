using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using YaEcs;
using YaEngine.Animation;
using YaEngine.Audio;
using YaEngine.Bootstrap;
using YaEngine.Core;
using YaEngine.Import;
using YaEngine.Render;
using YaEngine.VFX.ParticleSystem;
using YaEngine.VFX.ParticleSystem.Modules;
using YaEngine.VFX.ParticleSystem.Modules.Shapes;
using YaEngine.VFX.ParticleSystem.Modules.Value;
using YaEngine.VFX.ParticleSystem.Shaders;

namespace YaDemo
{
    public class BuildSceneSystem : IInitializeSystem
    {
        public int Priority => InitializePriorities.Third;

        private readonly MeshImporter meshImporter;
        private readonly AnimationImporter animationImporter;
        private readonly AvatarImporter avatarImporter;
        
        public BuildSceneSystem(MeshImporter meshImporter, AnimationImporter animationImporter, AvatarImporter avatarImporter)
        {
            this.meshImporter = meshImporter;
            this.animationImporter = animationImporter;
            this.avatarImporter = avatarImporter;
        }
        
        public Task ExecuteAsync(IWorld world)
        {
            CreateCamera(world);
            CreateLight(world, Color.White.ToVector3());
            
            var charTexturePath =
                "D:/Projects/project-v/Assets/Content/CharacterContent/Parts/Body/Textures/T_body1_base3_D.png";
            var charTexture = GetTexture(charTexturePath);
            CreateHuman(world, charTexture);
            
            CreateMusic(world);
            CreateParticleSystem(world);

            return Task.CompletedTask;
        }

        private static void CreateParticleSystem(IWorld world)
        {
            var particleTexturePath =
                "D:/Projects/project-v/Assets/Locations/Location_Amusement_Part_1/Effects/DustMotesEffect/DustMoteParticle.png";
            var particleTexture = GetTexture(particleTexturePath);
            world.Create(new Transform { Position = new Vector3(-1f, 1f, 6f) }, 
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
                        new RiffWaveAudioProvider("D:/Projects/project-v/fmod-audio/Assets/music/abandoned_village_1_120.wav")
                });
        }

        private void CreateHuman(IWorld world, TextureInitializer? charTexture)
        {
            var meshPath = "D:/Projects/project-v/Assets/Content/CharacterContent/Model/Human_Model.fbx";
            var meshes = meshImporter.Import(meshPath);
            var avatar = avatarImporter.Import(meshPath);
            var animations = ImportAnimations();
            Console.WriteLine($"Imported animations: {string.Join(", ", animations.Select(x => x.Name))}");
            var animator = new Animator(animations, avatar);
            animator.Play(animations[0].Name);
            world.Create(new Transform
                {
                    Position = new Vector3(0f, 0f, 5f),
                    Scale = Vector3.One * 0.05f,
                },
                new RendererInitializer
                {
                    Material = new MaterialInitializer
                    {
                        ShaderInitializer = DiffuseAnimationShader.Value,
                        TextureInitializer = charTexture
                    },
                    Mesh = meshes[0]
                },
                animator);
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

        private Animation[] ImportAnimations()
        {
            var animationsPaths = new[]
            {
                "D:/Projects/project-v/Assets/Content/CharacterContent/Animation/Idle/idle/neutral_idle.fbx",
                "D:/Projects/project-v/Assets/Content/CharacterContent/Animation/Hit/hit_react.fbx",
                "D:/Projects/project-v/Assets/Content/CharacterContent/Animation/Moving/run_slow.fbx",
                "D:/Projects/project-v/Assets/Content/CharacterContent/Animation/Moving/run_slow_weapon.fbx",
                "D:/Projects/project-v/Assets/Content/CharacterContent/Animation/Rifle/rifle_idle_free.fbx",
                "D:/Projects/project-v/Assets/Content/CharacterContent/Animation/Rifle/rifle_shot.fbx",
            };
            return animationsPaths
                .SelectMany(x =>
                {
                    var prefix = Path.GetFileNameWithoutExtension(x);
                    var animations = animationImporter.Import(x);
                    foreach (var animation in animations)
                    {
                        animation.Name = prefix + animation.Name;
                    }
                    return animations;
                })
                .ToArray();
        }
    }
}