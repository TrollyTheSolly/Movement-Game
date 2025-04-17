using UnityEngine;

public class ToolRocketLauncher : ToolBase
{
    private GameObject _rocketInstance;
    private RocketLauncherConfig config;

    public ToolRocketLauncher()
    {
        config = Resources.Load<RocketLauncherConfig>("Configs/RocketLauncherConfig");
        if (config == null)
        {
            Debug.LogError("RocketLauncherConfig not found! Make sure it is in Resources/Configs/");
        }
    }
    public override void Activate(ToolContext context)
    {
        _rocketInstance = GameObject.Instantiate(config.rocketPrefab, context.PlayerLocation + context.CameraTransform.forward * 2, Quaternion.identity);
        _rocketInstance.GetComponent<RocketBehaviour>().Configure(config.rocketSpeed, config.explosionRadius, config.explosionForce, config.explosionRadiusFalloff);
        _rocketInstance.transform.rotation = context.CameraTransform.rotation;
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