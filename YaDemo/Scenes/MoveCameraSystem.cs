using System.Numerics;
using Silk.NET.Input;
using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Bootstrap;
using YaEngine.Core;
using YaEngine.Input;
using YaEngine.Model;
using YaEngine.Render;

namespace YaDemo
{
    public class MoveCameraSystem : IModelSystem
    {
        public UpdateStep UpdateStep => ModelSteps.Update;

        private static readonly Key[] MoveBindings = { Key.D, Key.A, Key.W, Key.S };
        private static readonly Key[] LookBindings = { Key.Right, Key.Left, Key.Down, Key.Up };
        
        public void Execute(IWorld world)
        {
            if (!world.TryGetSingleton(out InputContext input)) return;
            if (!world.TryGetSingleton(out Time time)) return;
            
            var rotate = input.MouseDelta * 0.01f;
            var speed = 2.5f * time.DeltaTime;
            if (input.IsKeyPressed(Key.ShiftLeft))
            {
                speed *= 2;
            }
            
            var move = Vector2.Zero;
            AppendMovement(input, LookBindings, ref rotate, speed);
            AppendMovement(input, MoveBindings, ref move, speed);

            world.ForEach((Entity _, Camera _, Transform transform) =>
            {
                if (move.LengthSquared() > MathUtils.Epsilon)
                {
                    var forward = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, transform.Rotation));
                    var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));
                    transform.Position += forward * move.Y + right * move.X;
                }

                if (rotate.LengthSquared() > MathUtils.Epsilon)
                {
                    transform.Rotation = Rotate(rotate, transform.Rotation);
                }
            });
        }

        private void AppendMovement(InputContext input, Key[] bindings, ref Vector2 value, float delta)
        {
            if (input.IsKeyPressed(bindings[0]))
            {
                value.X += delta;
            }
            if (input.IsKeyPressed(bindings[1]))
            {
                value.X -= delta;
            }
            if (input.IsKeyPressed(bindings[2]))
            {
                value.Y += delta;
            }
            if (input.IsKeyPressed(bindings[3]))
            {
                value.Y -= delta;
            }
        }

        private Quaternion Rotate(Vector2 offset, Quaternion current)
        {
            var rotationEulers = current.ToEulerDegrees();
            rotationEulers.Y -= offset.X;
            rotationEulers.X += offset.Y * 2;

            var (pitch, yaw, roll) = rotationEulers.ToRadians().NormalizeAngles().Deconstruct();
            return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
        }
    }
}