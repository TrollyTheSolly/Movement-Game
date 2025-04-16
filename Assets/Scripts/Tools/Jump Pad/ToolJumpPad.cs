using UnityEngine;
using static UnityEngine.Rendering.STP;

public class ToolJumpPad : ToolBase
{
    public JumpPadConfig config;
    public GameObject jumpPadPrefab;
    private GameObject _jumpPadInstance;

    public ToolJumpPad()
    {
        config = Resources.Load<JumpPadConfig>("Configs/JumpPadConfig");
        if (config == null)
        {
            Debug.LogError("JumpPadConfig not found! Make sure it is in Resources/Configs/");
        }
    }

    public override void Activate(ToolContext context)
    {
        _jumpPadInstance = GameObject.Instantiate(config.JumpPadPrefab, context.PlayerLocation + context.CameraTransform.forward * 2, Quaternion.identity);
        _jumpPadInstance.GetComponent<Rigidbody>().AddForce(context.CameraTransform.forward * config.ThrowForce);
        _jumpPadInstance.GetComponent<JumpPadBehaviour>().Configure(config.JumpForce);
    }

    public override void Clear(ToolContext context)
    {
        // Optional clear logic
    }

    public override void Held(ToolContext context)
    {
        throw new System.NotImplementedException();
    }

    public override void Unheld(ToolContext context)
    {
        throw new System.NotImplementedException();
    }
}