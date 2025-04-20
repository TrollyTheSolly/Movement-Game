public class PlayerWalking : PlayerSimpleMovement
{
    protected override float GetTargetSpeed(PlayerContext context)
    {
        return context.MovementConfig.walkSpeed;
    }
}