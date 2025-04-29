using UnityEngine;

public class PlayerGroundSlam : PlayerSimpleAirMovement
{
    public override void Enter(PlayerContext context)
    {

    }

    public override void Exit(PlayerContext context)
    {

    }

    public override Vector3 Update(PlayerContext context)
    {
        Vector3 newVelocity = context.CurrentVelocity;
        
        if (context.MovementConfig.groundSlamCancelVerticalVelocity)
        {
            if (newVelocity.y > 0)
            {
                newVelocity.y = 0;
            }
        }
        
        
        float accel = context.MovementConfig.groundSlamAcceleration;
        float speed = context.MovementConfig.groundSlamSpeed;
        
        
        newVelocity.y = newVelocity.y - (accel * Time.fixedDeltaTime);
        newVelocity.y = Mathf.Clamp(newVelocity.y, -speed, float.PositiveInfinity);
        newVelocity = ApplyAirControl(context, newVelocity, 1f);
        return newVelocity;
    }
}
