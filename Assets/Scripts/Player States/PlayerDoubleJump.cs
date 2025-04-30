using UnityEngine;

public class PlayerDoubleJump : IPlayerState
{
    public void Enter(PlayerContext context)
    {

    }

    public void Exit(PlayerContext context)
    {

    }

    public Vector3 Update(PlayerContext context)
    {
        Vector3 newVelocity = context.CurrentVelocity;
        newVelocity.y = context.MovementConfig.jumpForce;
        return newVelocity;
    }
}
