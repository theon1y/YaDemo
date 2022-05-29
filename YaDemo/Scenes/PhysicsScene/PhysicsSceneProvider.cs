using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using YaEcs;
using YaEngine.ImGui;
using YaEngine.SceneManagement;

namespace YaDemo
{
    public class PhysicsSceneProvider : ISceneProvider
    {
        public string Name => "Physics";
        
        private readonly IServiceProvider serviceProvider;
        
        public PhysicsSceneProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        
        public SceneSystems GetSceneSystems()
        {
            return new()
            {
                InitializeSystems = new List<IInitializeSystem>
                {
                    ActivatorUtilities.CreateInstance<BuildPhysicsSceneSystem>(serviceProvider),
                    new RegisterGuiSystem(new List<IImGuiSystem>
                    {
                        ActivatorUtilities.CreateInstance<PhysicsControlsGuiSystem>(serviceProvider),
                        ActivatorUtilities.CreateInstance<SceneManagerGuiSystem>(serviceProvider),
                    })
                },
                UpdateSystems = new List<IUpdateSystem>
                {
                    ActivatorUtilities.CreateInstance<RestartPhysicsSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<ThrowCubeSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<SwitchAnimationsSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<MoveCameraSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<QuitSystem>(serviceProvider),
                },
                DisposeSystems = new List<IDisposeSystem>()
            };
        }
    }
}