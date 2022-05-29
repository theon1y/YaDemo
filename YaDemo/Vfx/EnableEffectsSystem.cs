using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Model;
using YaEngine.VFX.ParticleSystem;

namespace YaDemo
{
    public class EnableEffectsSystem : IModelSystem
    {
        public UpdateStep UpdateStep => ModelSteps.Update;
        
        public void Execute(IWorld world)
        {
            world.ForEach((Entity _, ParticleRenderer particleStorage) =>
            {
                particleStorage.Particles.IsPlaying = true;
            });
        }
    }
}