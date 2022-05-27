using System.Numerics;
using YaEcs;
using YaEngine.Animation;
using YaEngine.Core;
using YaEngine.ImGui;
using YaEngine.Render;
using static ImGuiNET.ImGui;

namespace YaDemo
{
    public class ShowTransformsGuiSystem : IImGuiSystem
    {
        public void Execute(IWorld world)
        {
            Begin("Transforms");
            SetWindowSize(new Vector2(300, 500));
            
            world.ForEach((Entity entity, Transform transform) =>
            {
                var label = world.TryGetComponent(entity, out Camera _) ? "Camera: " : "Entity: ";
                LabelText(label, "");
                SameLine();
                Text(entity.Id.ToString());
                ShowTransform(transform);
                if (world.TryGetComponent(entity, out Animator animator))
                {
                    ShowAnimator(animator);
                }
            });
            
            End();
        }

        private void ShowTransform(Transform transform)
        {
            LabelText("Transform:", "");
            Text("Position");
            SameLine();
            Text(transform.Position.ToString());
            Text("Rotation");
            SameLine();
            Text(transform.Rotation.ToEulerDegrees().ToString());
            Text("Scale");
            SameLine();
            Text(transform.Scale.ToString());
        }

        private void ShowAnimator(Animator animator)
        {
            var animationName = animator.Animation?.Name ?? "none";
            LabelText("Animator:", "");
            Text("Animation: ");
            SameLine();
            Text(animationName);
            Text("Time: ");
            SameLine();
            Text(animator.Time.ToString());
        }
    }
}