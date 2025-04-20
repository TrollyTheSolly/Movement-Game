using UnityEngine;

public class ToolTeleportOrb : ToolBase
{

    [SerializeField] private TeleportOrbConfig _config;
    private GameObject _player;
    private GameObject _teleportOrbInstance;

    public ToolTeleportOrb()
    {
        _config = Resources.Load<TeleportOrbConfig>("Configs/TeleportOrbConfig");
        if (_config == null)
        {
            Debug.LogError("TeleportOrbConfig not found! Make sure it is in Resources/Configs/");
        }
    }

    public override void Activate(ToolContext context)
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_teleportOrbInstance != null) GameObject.Destroy(_teleportOrbInstance);
        _teleportOrbInstance = GameObject.Instantiate(_config.teleportOrbPrefab, context.PlayerLocation + context.CameraTransform.forward * 2, Quaternion.identity);
        _teleportOrbInstance.GetComponent<TeleportOrbBehaviour>().Configure(context.CameraTransform, _config, _player);
    }

    public override void Clear(ToolContext context)
    {
        GameObject.Destroy(_teleportOrbInstance);
    }

    public override void Held(ToolContext context)
    {

    }

    public override void Unheld(ToolContext context)
    {

    }
}