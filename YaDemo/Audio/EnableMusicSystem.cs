using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Audio;
using YaEngine.Model;

namespace YaDemo
{
    public class EnableMusicSystem : IModelSystem
    {
        public UpdateStep UpdateStep => ModelSteps.Update;
        
        public void Execute(IWorld world)
        {
            world.ForEach((Entity _, Music _, AudioSource audioSource) =>
            {
                if (audioSource.IsPlaying) return;
                
                audioSource.Play(true);
            });
        }
    }
}