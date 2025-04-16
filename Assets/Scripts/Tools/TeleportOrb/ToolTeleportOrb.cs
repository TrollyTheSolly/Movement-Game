using UnityEngine;

public class ToolTeleportOrb : ToolBase
{

    [SerializeField] private TeleportOrbConfig config;
    private GameObject player;
    private GameObject teleportOrbInstance;

    public ToolTeleportOrb()
    {
        config = Resources.Load<TeleportOrbConfig>("Configs/TeleportOrbConfig");
        if (config == null)
        {
            Debug.LogError("TeleportOrbConfig not found! Make sure it is in Resources/Configs/");
        }
    }

    public override void Activate(ToolContext context)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (teleportOrbInstance != null) GameObject.Destroy(teleportOrbInstance);
        teleportOrbInstance = GameObject.Instantiate(config.teleportOrbPrefab, context.PlayerLocation + context.CameraTransform.forward * 2, Quaternion.identity);
        teleportOrbInstance.GetComponent<TeleportOrbBehaviour>().Configure(context.CameraTransform, config, player);
    }

    public override void Clear(ToolContext context)
    {
        GameObject.Destroy(teleportOrbInstance);
    }

    public override void Held(ToolContext context)
    {

    }

    public override void Unheld(ToolContext context)
    {

    }
}