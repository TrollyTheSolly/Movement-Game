using UnityEngine;

public abstract class ToolBase
{
    public string ToolName;

    public abstract void Activate(ToolContext context);
    public abstract void Clear(ToolContext context);

    public abstract void Held(ToolContext context);

    public abstract void Unheld(ToolContext context);
}