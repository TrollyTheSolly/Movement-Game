
public class PlayerRunning : PlayerSimpleMovement
{
    protected override float GetTargetSpeed(PlayerContext context)
    {
        return context.MovementConfig.runSpeed;
    }
}