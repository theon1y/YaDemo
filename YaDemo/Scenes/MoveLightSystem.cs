using System;
using System.Linq;
using YaEcs;
using YaEcs.Bootstrap;
using YaEngine.Core;
using YaEngine.Model;
using YaEngine.Render;

namespace YaDemo
{
    public class MoveLightSystem : IModelSystem
    {
        public UpdateStep UpdateStep => ModelSteps.Update;
        
        public void Execute(IWorld world)
        {
            //MoveToCamera(world);
            MoveInCylinder(world);
        }

        private static void MoveToCamera(IWorld world)
        {
            if (!world.TryGetSingleton(out CameraRegistry cameraRegistry)) return;

            var camera = cameraRegistry.Cameras.FirstOrDefault();
            if (!world.TryGetComponent(camera, out Transform cameraTransform)) return;

            world.ForEach((Entity _, AmbientLight _, Transform transform) =>
            {
                transform.Parent.Position = cameraTransform.Position;
            });
        }

        private static void MoveInCylinder(IWorld world)
        {
            if (!world.TryGetSingleton(out Time time)) return;

            const float speed = 0.2f;
            const float size = 10;
            var sin = (float) Math.Sin(time.TimeSinceStartup * speed);
            var sin2 = (float) Math.Sin(time.TimeSinceStartup * speed * 2);
            var cos2 = (float) Math.Cos(time.TimeSinceStartup * speed * 2);
            world.ForEach((Entity _, AmbientLight _, Transform transform) =>
            {
                // transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)time.TimeSinceStartup);
                // return;
                var position = transform.Position;
                position.X = sin2 * size;
                position.Z = -cos2 * size;
                position.Y = sin * size;
                transform.Position = position;
            });
        }
    }
}