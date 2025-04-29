using UnityEngine;

public class PlayerVisualInterpolator : MonoBehaviour
{
    [SerializeField] private PlayerMovementConfig movementConfig;
    [SerializeField] private Transform visualTransform;

    // Interpolation variables
    private Vector3 _lastFixedPosition;
    private Vector3 _currentFixedPosition;
    private float _fixedDeltaTimeTimer;
    
    public void Tick()
    {
        InterpolatePosition();
    }
    
    
    private void InterpolatePosition()
    {
        if (movementConfig.interpolate == false) return;

        _fixedDeltaTimeTimer += Time.deltaTime;

        float interpolationFactor = _fixedDeltaTimeTimer / Time.fixedDeltaTime;
        interpolationFactor = Mathf.Clamp01(interpolationFactor);

        Vector3 interpolatedPosition = Vector3.Lerp(_lastFixedPosition, _currentFixedPosition, interpolationFactor);
        visualTransform.position = interpolatedPosition;
    }

    public void RegisterFixedTick(Vector3 newFixedPosition)
    {
        _lastFixedPosition = _currentFixedPosition;
        _currentFixedPosition = newFixedPosition;
        _fixedDeltaTimeTimer = 0f;
    }
}
