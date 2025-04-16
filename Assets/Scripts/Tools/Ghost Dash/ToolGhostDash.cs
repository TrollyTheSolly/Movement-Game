using UnityEngine;

public class ToolGhostDash : ToolBase
{

    private GhostDashConfig config;

    public ToolGhostDash()
    {
        config = Resources.Load<GhostDashConfig>("Configs/GhostDashConfig");
        if (config == null)
        {
            Debug.LogError("GhostDashConfig not found! Make sure it is in Resources/Configs/");
        }
    }


    public override void Activate(ToolContext context)
    {
        Debug.Log("Ghost Dash Used.");
        Debug.Log(config.ghostDashTime);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 newPosition = player.GetComponent<Transform>().position + (context.PlayerVelocity * config.ghostDashTime);
        Vector3 dashDelta = context.PlayerVelocity * config.ghostDashTime;
        player.GetComponent<CharacterController>().Move(dashDelta);
    }

    public override void Clear(ToolContext context)
    {
        // Optional clear logic
    }

    public override void Held(ToolContext context)
    {

    }

    public override void Unheld(ToolContext context)
    {

    }
}