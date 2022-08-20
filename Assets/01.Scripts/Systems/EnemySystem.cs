using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Physics;
using Unity.Physics.Systems;


[UpdateAfter(typeof(EndFramePhysicsSystem))]
public partial class EnemySystem : SystemBase
{
    private Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1234);
    protected override void OnUpdate()
    {
        MovementRaycast raycaster = new MovementRaycast() { physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld };
        rng.NextInt();
        var rngTemp = rng;

        Entities.ForEach((ref Movable move, ref Enemy enemy, in Translation translation) =>
        {
            if (math.distance(translation.Value, enemy.prevCell) > 0.9f)
            {
                enemy.prevCell = math.round(translation.Value);
                var validDir = new NativeList<float3>(Allocator.Temp);

                if (!raycaster.CheckRay(translation.Value, new float3(0, 0, -1), move.Direction))
                    validDir.Add(new float3(0, 0, -1));
                if (!raycaster.CheckRay(translation.Value, new float3(0, 0, 1), move.Direction))
                    validDir.Add(new float3(0, 0, 1));
                if (!raycaster.CheckRay(translation.Value, new float3(-1, 0, 0), move.Direction))
                    validDir.Add(new float3(-1, 0, 0));
                if (!raycaster.CheckRay(translation.Value, new float3(1, 0, 0), move.Direction))
                    validDir.Add(new float3(1, 0, 0));

                move.Direction = validDir[rngTemp.NextInt(validDir.Length)];

                validDir.Dispose();
            }
        }).Schedule();
    }


    private struct MovementRaycast
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        public bool CheckRay(float3 pos, float3 dir, float3 curDir)
        {
            if(dir.Equals(-curDir))
                return true;

            var ray = new RaycastInput()
            {
                Start = pos,
                End = pos + (dir * 0.9f),
                Filter = new CollisionFilter()
                {
                    GroupIndex = 0,
                    BelongsTo = 1 << 1,
                    CollidesWith = 1 << 2,

                }
            };

            return physicsWorld.CastRay(ray);
        }
    }
}
