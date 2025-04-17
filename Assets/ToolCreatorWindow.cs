using UnityEngine;
using UnityEditor;
using System.IO;

public class ToolCreatorWindow : EditorWindow
{
    private string toolName = "NewTool";
    private string toolsDirectory = "Assets/Scripts/Tools";

    [MenuItem("Tools/Tool Creator")]
    public static void ShowWindow()
    {
        GetWindow<ToolCreatorWindow>("Tool Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tool Creator", EditorStyles.boldLabel);

        toolName = EditorGUILayout.TextField("Tool Name:", toolName);

        toolsDirectory = EditorGUILayout.TextField("Tools Directory:", toolsDirectory);

        if (GUILayout.Button("Create Tool"))
        {
            CreateToolFiles();
        }
    }

    private void CreateToolFiles()
    {
        if (string.IsNullOrEmpty(toolName))
        {
            EditorUtility.DisplayDialog("Error", "Tool name cannot be empty", "OK");
            return;
        }

        // Ensure directories exist
        string toolSubDirectory = Path.Combine(toolsDirectory, toolName);
        if (!Directory.Exists(toolSubDirectory))
            Directory.CreateDirectory(toolSubDirectory);

        // Create Tool class file
        string toolFilePath = Path.Combine(toolSubDirectory, $"Tool{toolName}.cs");
        File.WriteAllText(toolFilePath, GenerateToolClassContent());

        // Create Config class file
        string configClassFilePath = Path.Combine(toolSubDirectory, $"{toolName}Config.cs");
        File.WriteAllText(configClassFilePath, GenerateConfigClassContent());

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Tool {toolName} created successfully!", "OK");
    }

    private string GenerateToolClassContent()
    {
        return
    $@"using UnityEngine;
using TMPro;

public class Tool{toolName} : ToolBase
{{
    private {toolName}Config config;
    private GameObject instantiatedHeldObject;

    public Tool{toolName}()
    {{
        config = Resources.Load<{toolName}Config>(""Configs/{toolName}Config"");
        if (config == null)
        {{
            Debug.LogError(""{toolName}Config not found! Make sure it is in Resources/Configs/"");
        }}
    }}

    public override void Activate(ToolContext context)
    {{
        // Implement tool-specific activation logic
        Debug.Log(""{toolName} activated"");
    }}

    public override void Clear(ToolContext context)
    {{
        // Implement tool-specific clear logic
    }}

    public override void Held(ToolContext context)
    {{
        instantiatedHeldObject = GameObject.Instantiate(config.HeldPrefab, context.CameraTransform);
    }}

    public override void Unheld(ToolContext context)
    {{
        GameObject.Destroy(instantiatedHeldObject);
    }}
}}";
    }

    private string GenerateConfigClassContent()
    {
        return
    $@"using UnityEngine;

[CreateAssetMenu(fileName = ""{toolName}Config"", menuName = ""Tools/{toolName}Config"")]
public class {toolName}Config : ScriptableObject
{{
    public GameObject HeldPrefab;
}}";
    }
}