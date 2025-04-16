public class DefaultPlayerStateFactory : IPlayerStateFactory
{
    public IPlayerState CreateState(State state)
    {
        switch (state)
        {
            case State.Walking:
                return new PlayerWalking();
            case State.Running:
                return new PlayerRunning();
            case State.Jumping:
                return new PlayerJumping();
            case State.Airborne:
                return new PlayerAirborne();
            default:
                throw new System.ArgumentException($"Unknown state: {state}");
        }
    }
}