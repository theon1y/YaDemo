using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using YaEcs;
using YaEngine.ImGui;
using YaEngine.SceneManagement;

namespace YaDemo
{
    public class AnimationsSceneProvider : ISceneProvider
    {
        public string Name => "Animations";
        
        private readonly IServiceProvider serviceProvider;

        public AnimationsSceneProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public SceneSystems GetSceneSystems()
        {
            return new()
            {
                InitializeSystems = new List<IInitializeSystem>
                {
                    ActivatorUtilities.CreateInstance<BuildAnimationsSceneSystem>(serviceProvider),
                    new RegisterGuiSystem(new List<IImGuiSystem>
                    {
                        ActivatorUtilities.CreateInstance<ShowTransformsGuiSystem>(serviceProvider),
                        ActivatorUtilities.CreateInstance<AnimationControlsGuiSystem>(serviceProvider),
                        ActivatorUtilities.CreateInstance<SceneManagerGuiSystem>(serviceProvider),
                    })
                },
                UpdateSystems = new List<IUpdateSystem>
                {
                    ActivatorUtilities.CreateInstance<EnableEffectsSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<EnableMusicSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<SwitchAnimationsSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<MoveCameraSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<QuitSystem>(serviceProvider),
                    ActivatorUtilities.CreateInstance<MoveLightSystem>(serviceProvider),
                },
                DisposeSystems = new List<IDisposeSystem>()
            };
        }
    }
}