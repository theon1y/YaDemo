using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.VFX.ParticleSystem;

namespace YaDemo
{
    public class EnableEffectsSystem : IUpdateSystem
    {
        public UpdateStep UpdateStep => UpdateSteps.Update;
        
        public void Execute(IWorld world)
        {
            world.ForEach((Entity _, ParticleRenderer particleStorage) =>
            {
                particleStorage.Particles.IsPlaying = true;
            });
        }
    }
}