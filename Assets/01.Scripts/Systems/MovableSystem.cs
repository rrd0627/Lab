using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
public partial class MovableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Movable move, ref PhysicsVelocity phyVel) =>
        {
            var step = move.Direction * move.Speed;
            phyVel.Linear = step;
        }).Schedule();
    }
}
