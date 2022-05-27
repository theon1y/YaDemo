using Silk.NET.Input;
using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Bootstrap;
using YaEngine.Input;

namespace YaDemo
{
    public class QuitSystem : IUpdateSystem
    {
        public UpdateStep UpdateStep => UpdateSteps.Update;
        
        public void Execute(IWorld world)
        {
            if (!world.TryGetSingleton(out InputContext input)) return;

            if (input.IsKeyPressed(Key.Escape) && world.TryGetSingleton(out Application application))
            {
                application.Instance.Quit();
            }
        }
    }
}