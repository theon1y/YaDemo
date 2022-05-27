using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YaEcs;
using YaEngine.ImGui;

namespace YaDemo
{
    public static class AnimationsSceneConfigurationExtensions
    {
        public static IServiceCollection AddAnimationsScene(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection
                .AddScoped<IInitializeSystem, BuildSceneSystem>()
                .AddScoped<IUpdateSystem, EnableEffectsSystem>()
                .AddScoped<IUpdateSystem, EnableMusicSystem>()
                .AddScoped<IUpdateSystem, SwitchAnimationsSystem>()
                .AddScoped<IUpdateSystem, MoveCameraSystem>()
                .AddScoped<IUpdateSystem, QuitSystem>()
                .AddScoped<IImGuiSystem, ShowTransformsGuiSystem>()
                .AddScoped<IUpdateSystem, MoveLightSystem>();
        }
    }
}