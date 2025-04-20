using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Transform _startTransform;
    private Vector3 _endPoint;

    public void Initialize(Transform start, Vector3 end)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _startTransform = start;
        _endPoint = end;

        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        if (_lineRenderer != null && _startTransform != null)
        {
            _lineRenderer.SetPosition(0, _startTransform.position);
            _lineRenderer.SetPosition(1, _endPoint);
        }
    }
}
