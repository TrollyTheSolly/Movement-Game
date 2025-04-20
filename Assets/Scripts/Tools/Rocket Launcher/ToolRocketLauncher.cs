using UnityEngine;

public class ToolRocketLauncher : ToolBase
{
    private GameObject _rocketInstance;
    private RocketLauncherConfig _config;
    private GameObject _instantiatedRocketLauncher;

    public ToolRocketLauncher()
    {
        _config = Resources.Load<RocketLauncherConfig>("Configs/RocketLauncherConfig");
        if (_config == null)
        {
            Debug.LogError("RocketLauncherConfig not found! Make sure it is in Resources/Configs/");
        }
    }
    public override void Activate(ToolContext context)
    {
        _rocketInstance = GameObject.Instantiate(_config.rocketPrefab, context.CameraTransform.position + _config.rocketSpawnOffset, context.CameraTransform.rotation);
        _rocketInstance.GetComponent<RocketBehaviour>().Configure(_config, context.PlayerVelocity);
        //_rocketInstance.transform.rotation = context.CameraTransform.rotation;
    }

    public override void Clear(ToolContext context)
    {
        // Optional clear logic
    }

    public override void Held(ToolContext context)
    {
        Debug.Log("Spawning rocket launcher");
        _instantiatedRocketLauncher = GameObject.Instantiate(_config.rocketLauncherPrefab, context.CameraTransform);
    }

    public override void Unheld(ToolContext context)
    {
        GameObject.Destroy(_instantiatedRocketLauncher);
    }
}