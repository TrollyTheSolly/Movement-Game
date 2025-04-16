using UnityEngine;

public class PlayerGrappling : IPlayerState
{
    public void Enter(PlayerContext context)
    {
        
    }

    public void Exit(PlayerContext context)
    {
        
    }

    public Vector3 Update(PlayerContext context)
    {
        ToolGrapplingHook grappleTool = context.playerTool as ToolGrapplingHook;

        Vector3 toBody = context.position - grappleTool.grapplePoint;
        float currentDistance = toBody.magnitude;

        if (currentDistance == 0) return Vector3.zero;

        Vector3 forceDirection = toBody.normalized;
        float stretch = currentDistance - grappleTool.config.ropeLength;

        if (stretch <= 0)
            return context.currentVelocity;

        Vector3 tensionForce = -forceDirection * stretch * grappleTool.config.ropeStiffness * Time.fixedDeltaTime;

        Vector3 velocity = context.currentVelocity;
        Vector3 parallelVelocity = Vector3.Project(velocity, forceDirection);
        Vector3 tangentialVelocity = velocity - parallelVelocity;

        Vector3 correctedVelocity = tangentialVelocity + parallelVelocity * 0.5f;

        return correctedVelocity + tensionForce;
    }
}
