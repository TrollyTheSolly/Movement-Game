using UnityEngine;

public class PlayerAirborne : PlayerSimpleAirMovement
{
    public override Vector3 Update(PlayerContext context)
    {
        // Using the base air control with full control factor (1.0f)
        return ApplyAirControl(context, context.CurrentVelocity, 1.0f);
    }
}