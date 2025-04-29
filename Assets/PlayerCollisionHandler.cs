using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private CharacterController _controller;
    private Transform _transform;
    [SerializeField] float checkHeadDistance;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
    }
    
    public void CheckHeadCollision(ref Vector3 velocity)
    {
        Vector3 top = _transform.position + Vector3.up * (_controller.height / 2f);
        if (Physics.Raycast(top, Vector3.up, checkHeadDistance))
        {
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }
        }
    }
    
    public void CheckSideCollision(ref Vector3 velocity)
    {
        Vector3[] directions = {
            _transform.right,
            -_transform.right,
            _transform.forward,
            -_transform.forward
        };

        Vector3 center = _transform.position + Vector3.up * (_controller.height / 2f);
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        foreach (var dir in directions)
        {
            if (Physics.Raycast(center, dir, out RaycastHit hit, _controller.radius + 0.1f))
            {
                // Only affect velocity if moving toward the obstacle
                if (Vector3.Dot(horizontalVelocity, dir) > 0)
                {
                    // Project velocity on the plane perpendicular to the wall normal, allowing sliding
                    Vector3 normal = hit.normal;
                    Vector3 adjustedVelocity = Vector3.ProjectOnPlane(horizontalVelocity, normal);
                    velocity.x = adjustedVelocity.x;
                    velocity.z = adjustedVelocity.z;
                }
            }
        }
    }

    public void PreviousCheckSideCollision(ref Vector3 velocity)
    {
        Vector3[] directions = {
            _transform.right,
            -_transform.right,
            _transform.forward,
            -_transform.forward
        };

        Vector3 center = _transform.position + Vector3.up * (_controller.height / 2f);

        foreach (var dir in directions)
        {
            if (Physics.Raycast(center, dir, out RaycastHit hit, _controller.radius + 0.1f))
            {
                Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
                if (Vector3.Dot(horizontalVelocity, dir) > 0)
                {
                    velocity.x = 0;
                    velocity.z = 0;
                    break;
                }
            }
        }
    }
}
