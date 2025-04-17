using Unity.VisualScripting;
using UnityEngine;
public class ToolGrapplingHook : ToolBase
{
    public bool grappleActive = false;
    public Vector3 grapplePoint { get; private set; }
    public GrappleHookConfig config;
    GameObject ropeInstance;

    public ToolGrapplingHook()
    {
        config = Resources.Load<GrappleHookConfig>("Configs/GrappleHookConfig");
        if (config == null)
        {
            Debug.LogError("GrappleHookConfig not found! Make sure it is in Resources/Configs/");
        }
    }

    public override void Activate(ToolContext context)
    {
        if (!grappleActive)
        {
            // Try the primary straight raycast first
            Ray ray = new Ray(context.CameraTransform.position, context.CameraTransform.forward);
            if (TryGrapple(ray, context))
                return;

            // If the straight raycast failed, try multiple rays in a spread pattern
            if (config.useForgivingAim && config.spreadRayCount > 0)
            {
                // Try each ray in the spread until one hits
                for (int i = 0; i < config.spreadRayCount; i++)
                {
                    // Calculate a spread direction based on the current index
                    float angle = ((float)i / (config.spreadRayCount - 1) - 0.5f) * config.spreadAngle;
                    Vector3 spreadDirection = Quaternion.AngleAxis(angle, context.CameraTransform.up) * context.CameraTransform.forward;

                    // Apply vertical spread if configured
                    if (config.useVerticalSpread)
                    {
                        float vertAngle = ((float)(i % config.verticalRayCount) / (config.verticalRayCount - 1) - 0.5f) * config.verticalSpreadAngle;
                        spreadDirection = Quaternion.AngleAxis(vertAngle, context.CameraTransform.right) * spreadDirection;
                    }

                    Ray spreadRay = new Ray(context.CameraTransform.position, spreadDirection.normalized);

                    // For debugging
                    if (config.visualizeRays)
                    {
                        Debug.DrawRay(context.CameraTransform.position, spreadDirection.normalized * config.grappleDistance, Color.yellow, 0.5f);
                    }

                    if (TryGrapple(spreadRay, context))
                        return;
                }
            }
        }
    }

    private bool TryGrapple(Ray ray, ToolContext context)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, config.grappleDistance, config.layerMask))
        {
            grapplePoint = hit.point;
            Debug.Log("Grapple connected at: " + hit.point);
            grappleActive = true;
            config.ropeLength = Vector3.Distance(context.PlayerVisualTransform.position, grapplePoint);
            ropeInstance = GameObject.Instantiate(config.ropePrefab);
            var ropeScript = ropeInstance.GetComponent<GrappleRope>();
            ropeScript.Initialize(context.PlayerVisualTransform, grapplePoint);
            return true;
        }
        return false;
    }

    public override void Clear(ToolContext context)
    {
        grappleActive = false;
        GameObject.Destroy(ropeInstance);
        ropeInstance = null;
    }

    public bool IsActive()
    {
        return grappleActive;
    }

    public override void Held(ToolContext context)
    {
    }

    public override void Unheld(ToolContext context)
    {
    }
}