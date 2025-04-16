using TMPro;
using UnityEngine;

public class ToolMomentumFreezer : ToolBase
{
    private Vector3 storedVelocity = Vector3.zero;
    private bool velocityStored = false;
    private MomentumFreezerConfig config;
    private GameObject instantiatedHeldFreezer;

    public ToolMomentumFreezer()
    {
        config = Resources.Load<MomentumFreezerConfig>("Configs/MomentumFreezerConfig");
        if (config == null)
        {
            Debug.LogError("MomentumFreezerConfig not found! Make sure it is in Resources/Configs/");
        }
    }
    public override void Activate(ToolContext context)
    {
        if (velocityStored)
        {
            Debug.Log("Velocity applied");
            Vector3 newLookDirection = context.CameraTransform.forward;

            float speed = storedVelocity.magnitude;

            Vector3 redirectedVelocity = newLookDirection * speed;

            context.ModifierSystem.AddModifier(redirectedVelocity);

            velocityStored = false;
            UpdateText("");
            return;
        } else
        {
            Debug.Log("Velocity saved");
            storedVelocity = context.PlayerVelocity;
            context.ModifierSystem.AddModifier(-storedVelocity);
            velocityStored = true;
            float velocityToStore = storedVelocity.magnitude * 10;
            UpdateText(FormatVelocity(storedVelocity));
            return;
        }
    }

    public override void Clear(ToolContext context)
    {
        
    }

    public override void Held(ToolContext context)
    {
        instantiatedHeldFreezer = GameObject.Instantiate(config.HeldMomentumFreezerPrefab, context.CameraTransform);
        if (velocityStored)
        {
            UpdateText(FormatVelocity(storedVelocity));
        }
    }

    public override void Unheld(ToolContext context)
    {
        Debug.Log("Destroying tool...");
        GameObject.Destroy(instantiatedHeldFreezer);
    }

    private string FormatVelocity(Vector3 inVector)
    {
        float velocity = inVector.magnitude;
        velocity *= 10;
        return (velocity.ToString("F1"));
    }

    private void UpdateText(string newText)
    {
        instantiatedHeldFreezer.GetComponentInChildren<TextMeshPro>().text = newText;
    }
}