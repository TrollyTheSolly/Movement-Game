using UnityEditor.EditorTools;
using UnityEngine;

public interface ITool
{

    public void Activate(ToolContext context);

    public void Clear(ToolContext context);

}
