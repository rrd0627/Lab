using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public partial class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Player>().        
        ForEach((ref Movable move) =>
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            move.Direction =  new float3(x,0,y);
            

        }).Run();
    }
}
