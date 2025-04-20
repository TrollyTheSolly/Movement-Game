using Unity.VisualScripting;
using UnityEngine;
public class ToolGrapplingHook : ToolBase
{
    public bool GrappleActive = false;
    public Vector3 GrapplePoint { get; private set; }
    public GrappleHookConfig Config;
    GameObject _ropeInstance;

    public ToolGrapplingHook()
    {
        Config = Resources.Load<GrappleHookConfig>("Configs/GrappleHookConfig");
        if (Config == null)
        {
            Debug.LogError("GrappleHookConfig not found! Make sure it is in Resources/Configs/");
        }
    }

    public override void Activate(ToolContext context)
    {
        if (!GrappleActive)
        {
            // Try the primary straight raycast first
            Ray ray = new Ray(context.CameraTransform.position, context.CameraTransform.forward);
            if (TryGrapple(ray, context))
                return;

            // If the straight raycast failed, try multiple rays in a spread pattern
            if (Config.useForgivingAim && Config.spreadRayCount > 0)
            {
                // Try each ray in the spread until one hits
                for (int i = 0; i < Config.spreadRayCount; i++)
                {
                    // Calculate a spread direction based on the current index
                    float angle = ((float)i / (Config.spreadRayCount - 1) - 0.5f) * Config.spreadAngle;
                    Vector3 spreadDirection = Quaternion.AngleAxis(angle, context.CameraTransform.up) * context.CameraTransform.forward;

                    // Apply vertical spread if configured
                    if (Config.useVerticalSpread)
                    {
                        float vertAngle = ((float)(i % Config.verticalRayCount) / (Config.verticalRayCount - 1) - 0.5f) * Config.verticalSpreadAngle;
                        spreadDirection = Quaternion.AngleAxis(vertAngle, context.CameraTransform.right) * spreadDirection;
                    }

                    Ray spreadRay = new Ray(context.CameraTransform.position, spreadDirection.normalized);

                    // For debugging
                    if (Config.visualizeRays)
                    {
                        Debug.DrawRay(context.CameraTransform.position, spreadDirection.normalized * Config.grappleDistance, Color.yellow, 0.5f);
                    }

                    if (TryGrapple(spreadRay, context))
                        return;
                }
            }
        }
    }

    private bool TryGrapple(Ray ray, ToolContext context)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Config.grappleDistance, Config.layerMask))
        {
            GrapplePoint = hit.point;
            Debug.Log("Grapple connected at: " + hit.point);
            GrappleActive = true;
            Config.ropeLength = Vector3.Distance(context.PlayerVisualTransform.position, GrapplePoint);
            _ropeInstance = GameObject.Instantiate(Config.ropePrefab);
            var ropeScript = _ropeInstance.GetComponent<GrappleRope>();
            ropeScript.Initialize(context.PlayerVisualTransform, GrapplePoint);
            return true;
        }
        return false;
    }

    public override void Clear(ToolContext context)
    {
        GrappleActive = false;
        GameObject.Destroy(_ropeInstance);
        _ropeInstance = null;
    }

    public bool IsActive()
    {
        return GrappleActive;
    }

    public override void Held(ToolContext context)
    {
    }

    public override void Unheld(ToolContext context)
    {
    }
}