using YaEcs;
using YaEngine.ImGui;
using static ImGuiNET.ImGui;

namespace YaDemo
{
    public class PhysicsControlsGuiSystem : IImGuiSystem
    {
        public void Execute(IWorld world)
        {
            Begin("Controls");
            Text("WASD to move the camera");
            Text("SPACE to throw a cube");
            Text("ALT to toggle cursor");
            Text("ESC to quit");
            End();
        }
    }
}