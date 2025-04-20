using UnityEngine;
using static UnityEngine.Rendering.STP;

public class ToolJumpPad : ToolBase
{
    public JumpPadConfig Config;
    public GameObject JumpPadPrefab;
    private GameObject _jumpPadInstance;

    public ToolJumpPad()
    {
        Config = Resources.Load<JumpPadConfig>("Configs/JumpPadConfig");
        if (Config == null)
        {
            Debug.LogError("JumpPadConfig not found! Make sure it is in Resources/Configs/");
        }
    }

    public override void Activate(ToolContext context)
    {
        _jumpPadInstance = GameObject.Instantiate(Config.jumpPadPrefab, context.PlayerLocation + context.CameraTransform.forward * 2, Quaternion.identity);
        _jumpPadInstance.GetComponent<Rigidbody>().AddForce(context.CameraTransform.forward * Config.throwForce);
        _jumpPadInstance.GetComponent<JumpPadBehaviour>().Configure(Config.jumpForce);
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