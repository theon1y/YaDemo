using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Audio;

namespace YaDemo
{
    public class EnableMusicSystem : IUpdateSystem
    {
        public UpdateStep UpdateStep => UpdateSteps.Update;
        
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