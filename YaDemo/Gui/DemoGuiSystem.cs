using YaEcs;
using YaEngine.ImGui;

namespace YaDemo
{
    public class DemoGuiSystem : IImGuiSystem
    {
        public void Execute(IWorld world)
        {
            ImGuiNET.ImGui.ShowDemoWindow();
        }
    }
}