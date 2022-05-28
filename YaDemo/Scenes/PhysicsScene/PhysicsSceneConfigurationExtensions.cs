using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YaEcs;
using YaEngine.ImGui;
using YaEngine.Physics;

namespace YaDemo
{
    public static class PhysicsSceneConfigurationExtensions
    {
        public static IServiceCollection AddPhysicsScene(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection
                //.AddScoped<IPhysicsSystem, DebugDrawSystem>()
                //.AddScoped<IUpdateSystem, MoveLightSystem>()
                .AddScoped<IInitializeSystem, BuildPhysicsSceneSystem>()
                .AddScoped<IPhysicsSystem, RestartPhysicsSystem>()
                .AddScoped<IPhysicsSystem, ThrowCubeSystem>()
                .AddScoped<IImGuiSystem, PhysicsControlsGuiSystem>()
                .AddScoped<IUpdateSystem, MoveCameraSystem>()
                .AddScoped<IUpdateSystem, QuitSystem>();
        }
    }
}