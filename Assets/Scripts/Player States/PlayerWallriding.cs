using UnityEngine;

public class PlayerWallriding : IPlayerState
{
    private bool _isWallriding = false;
    
    
    public void Enter(PlayerContext context)
    {
        _isWallriding = true;
    }

    public void Exit(PlayerContext context)
    {
        
    }

    public Vector3 Update(PlayerContext context)
    {
        throw new System.NotImplementedException();
    }
}