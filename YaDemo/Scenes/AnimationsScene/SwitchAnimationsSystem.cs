using System.Linq;
using Silk.NET.Input;
using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Animation;
using YaEngine.Input;

namespace YaDemo
{
    public class SwitchAnimationsSystem : IUpdateSystem
    {
        public UpdateStep UpdateStep => UpdateSteps.Update;
        
        public void Execute(IWorld world)
        {
            if (!world.TryGetSingleton(out InputContext input)) return;

            var direction = 0;
            if (input.IsKeyDown(Key.E))
            {
                direction = 1;
            }

            if (input.IsKeyDown(Key.Q))
            {
                direction = -1;
            }

            if (direction != 0)
            {
                world.ForEach((Entity _, Animator animator) =>
                {
                    var animations = animator.Animations.Keys.OrderBy(x => x).ToList();
                    var currentIndex = animations.IndexOf(animator.Animation?.Name);
                    var nextIndex = (currentIndex + direction + animations.Count) % animations.Count;
                    animator.Play(animations[nextIndex]);
                });
            }
        }
    }
}