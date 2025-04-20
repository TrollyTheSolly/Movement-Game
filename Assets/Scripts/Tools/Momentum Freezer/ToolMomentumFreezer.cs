using TMPro;
using UnityEngine;

public class ToolMomentumFreezer : ToolBase
{
    private Vector3 _storedVelocity = Vector3.zero;
    private bool _velocityStored = false;
    private MomentumFreezerConfig _config;
    private GameObject _instantiatedHeldFreezer;

    public ToolMomentumFreezer()
    {
        _config = Resources.Load<MomentumFreezerConfig>("Configs/MomentumFreezerConfig");
        if (_config == null)
        {
            Debug.LogError("MomentumFreezerConfig not found! Make sure it is in Resources/Configs/");
        }
    }
    public override void Activate(ToolContext context)
    {
        if (_velocityStored)
        {
            Debug.Log("Velocity applied");
            Vector3 newLookDirection = context.CameraTransform.forward;

            float speed = _storedVelocity.magnitude;

            Vector3 redirectedVelocity = newLookDirection * speed;

            context.ModifierSystem.AddModifier(redirectedVelocity);

            _velocityStored = false;
            UpdateText("");
            return;
        } else
        {
            Debug.Log("Velocity saved");
            _storedVelocity = context.PlayerVelocity;
            context.ModifierSystem.AddModifier(-_storedVelocity);
            _velocityStored = true;
            float velocityToStore = _storedVelocity.magnitude * 10;
            UpdateText(FormatVelocity(_storedVelocity));
            return;
        }
    }

    public override void Clear(ToolContext context)
    {
        
    }

    public override void Held(ToolContext context)
    {
        _instantiatedHeldFreezer = GameObject.Instantiate(_config.heldMomentumFreezerPrefab, context.CameraTransform);
        if (_velocityStored)
        {
            UpdateText(FormatVelocity(_storedVelocity));
        }
    }

    public override void Unheld(ToolContext context)
    {
        Debug.Log("Destroying tool...");
        GameObject.Destroy(_instantiatedHeldFreezer);
    }

    private string FormatVelocity(Vector3 inVector)
    {
        float velocity = inVector.magnitude;
        velocity *= 10;
        return (velocity.ToString("F1"));
    }

    private void UpdateText(string newText)
    {
        _instantiatedHeldFreezer.GetComponentInChildren<TextMeshPro>().text = newText;
    }
}