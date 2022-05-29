using System.Numerics;
using Silk.NET.Input;
using YaEcs;
using YaEngine.ImGui;
using YaEngine.Input;
using YaEngine.SceneManagement;
using static ImGuiNET.ImGui;

namespace YaDemo
{
    public class SceneManagerGuiSystem : IImGuiSystem
    {
        private readonly ISceneManager sceneManager;

        public SceneManagerGuiSystem(ISceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        public void Execute(IWorld world)
        {
            if (!world.TryGetSingleton(out InputContext inputContext)) return;

            var screenSize = GetMainViewport().Size;
            Begin("Scenes");
            var scenes = sceneManager.GetScenes();
            for (var i = 0; i < scenes.Count; ++i)
            {
                var key = Key.F1 + i;
                var scene = scenes[i];
                Text($"{key} - {scene}");
                if (inputContext.IsKeyDown(key))
                {
                    sceneManager.LoadScene(scene);
                }
            }
            var windowSize = GetWindowSize();
            SetWindowPos(screenSize - windowSize - Vector2.One * 20);
            End();
        }
    }
}