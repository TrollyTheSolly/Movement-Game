using UnityEngine;

public class PlayerOrchestrator : MonoBehaviour
{
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] private PlayerVisualInterpolator playerVisualInterpolator;

    private void Update()
    {
        playerStateManager.Tick();
        playerVisualInterpolator.Tick();
    }

    private void FixedUpdate()
    {
        playerStateManager.FixedTick();
    }
}
