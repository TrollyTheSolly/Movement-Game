using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerContext context);
    void Exit(PlayerContext context);
    Vector3 Update(PlayerContext context);
}