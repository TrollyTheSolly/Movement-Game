using UnityEngine;

public class PlayerOrchestrator : MonoBehaviour
{
    [SerializeField] private PlayerStateManager playerStateManager;
    [SerializeField] private PlayerVisualInterpolator playerVisualInterpolator;
    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
    }

    private void Update()
    {
        playerStateManager.Tick();
        playerVisualInterpolator.Tick();
        
    }

    private void FixedUpdate()
    {
        playerStateManager.FixedTick();
        _scoreManager.FixedTick();
    }
}
