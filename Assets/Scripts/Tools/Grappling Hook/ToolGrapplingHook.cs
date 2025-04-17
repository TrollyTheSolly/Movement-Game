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
            Ray ray = new Ray(context.CameraTransform.position, context.CameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, config.grappleDistance, config.layerMask))
            {
                grapplePoint = hit.point;
                Debug.Log(hit.point);
                grappleActive = true;

                ropeInstance = GameObject.Instantiate(config.ropePrefab);
                var ropeScript = ropeInstance.GetComponent<GrappleRope>();
                ropeScript.Initialize(context.PlayerVisualTransform, grapplePoint);
            }
        }
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