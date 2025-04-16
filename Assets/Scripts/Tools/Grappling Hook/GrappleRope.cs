using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform startTransform;
    private Vector3 endPoint;

    public void Initialize(Transform start, Vector3 end)
    {
        lineRenderer = GetComponent<LineRenderer>();
        startTransform = start;
        endPoint = end;

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        if (lineRenderer != null && startTransform != null)
        {
            lineRenderer.SetPosition(0, startTransform.position);
            lineRenderer.SetPosition(1, endPoint);
        }
    }
}
