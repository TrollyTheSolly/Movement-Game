using UnityEngine;

public class ToolRocketLauncher : ToolBase
{
    private GameObject _rocketInstance;
    private RocketLauncherConfig config;
    private GameObject instantiatedRocketLauncher;

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
        _rocketInstance = GameObject.Instantiate(config.rocketPrefab, context.CameraTransform.position + config.rocketSpawnOffset, context.CameraTransform.rotation);
        _rocketInstance.GetComponent<RocketBehaviour>().Configure(config, context.PlayerVelocity);
        //_rocketInstance.transform.rotation = context.CameraTransform.rotation;
    }

    public override void Clear(ToolContext context)
    {
        // Optional clear logic
    }

    public override void Held(ToolContext context)
    {
        Debug.Log("Spawning rocket launcher");
        instantiatedRocketLauncher = GameObject.Instantiate(config.rocketLauncherPrefab, context.CameraTransform);
    }

    public override void Unheld(ToolContext context)
    {
        GameObject.Destroy(instantiatedRocketLauncher);
    }
}