using YaEcs;
using YaEngine.ImGui;
using static ImGuiNET.ImGui;

namespace YaDemo
{
    public class AnimationControlsGuiSystem : IImGuiSystem
    {
        public void Execute(IWorld world)
        {
            Begin("Controls");
            Text("WASD to move the camera");
            Text("QE to switch animations");
            Text("ALT to toggle cursor");
            Text("ESC to quit");
            End();
        }
    }
}