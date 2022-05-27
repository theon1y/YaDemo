using System;
using BulletSharp.Math;
using Silk.NET.Input;
using YaEcs;
using YaEngine.Input;
using YaEngine.Physics;

namespace YaDemo
{
    public class RestartPhysicsSystem : IPhysicsSystem
    {
        public UpdateStep UpdateStep => PhysicsSteps.UpdatePhysics;
        
        public void Execute(IWorld world)
        {
            if (!world.TryGetSingleton(out InputContext input)) return;
            if (!input.IsKeyDown(Key.R)) return;

            var random = new Random();
            world.ForEach((Entity _, RigidBody rb) =>
            {
                if (rb is not BulletRigidBody rigidBody) return;
                if (rb.Mass == 0) return;
                
                rigidBody.Value.Translate(new Vector3(0, random.Next(2, 10), 0));
                rigidBody.Value.Activate();
            });
        }
    }
}