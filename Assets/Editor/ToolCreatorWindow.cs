using UnityEngine;
using UnityEditor;
using System.IO;

public class ToolCreatorWindow : EditorWindow
{
    private string _toolName = "NewTool";
    private string _toolsDirectory = "Assets/Scripts/Tools";

    [MenuItem("Tools/Tool Creator")]
    public static void ShowWindow()
    {
        GetWindow<ToolCreatorWindow>("Tool Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tool Creator", EditorStyles.boldLabel);

        _toolName = EditorGUILayout.TextField("Tool Name:", _toolName);

        _toolsDirectory = EditorGUILayout.TextField("Tools Directory:", _toolsDirectory);

        if (GUILayout.Button("Create Tool"))
        {
            CreateToolFiles();
        }
    }

    private void CreateToolFiles()
    {
        if (string.IsNullOrEmpty(_toolName))
        {
            EditorUtility.DisplayDialog("Error", "Tool name cannot be empty", "OK");
            return;
        }

        // Ensure directories exist
        string toolSubDirectory = Path.Combine(_toolsDirectory, _toolName);
        if (!Directory.Exists(toolSubDirectory))
            Directory.CreateDirectory(toolSubDirectory);

        // Create Tool class file
        string toolFilePath = Path.Combine(toolSubDirectory, $"Tool{_toolName}.cs");
        File.WriteAllText(toolFilePath, GenerateToolClassContent());

        // Create Config class file
        string configClassFilePath = Path.Combine(toolSubDirectory, $"{_toolName}Config.cs");
        File.WriteAllText(configClassFilePath, GenerateConfigClassContent());

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Tool {_toolName} created successfully!", "OK");
    }

    private string GenerateToolClassContent()
    {
        return
    $@"using UnityEngine;
using TMPro;

public class Tool{_toolName} : ToolBase
{{
    private {_toolName}Config config;
    private GameObject instantiatedHeldObject;

    public Tool{_toolName}()
    {{
        config = Resources.Load<{_toolName}Config>(""Configs/{_toolName}Config"");
        if (config == null)
        {{
            Debug.LogError(""{_toolName}Config not found! Make sure it is in Resources/Configs/"");
        }}
    }}

    public override void Activate(ToolContext context)
    {{
        // Implement tool-specific activation logic
        Debug.Log(""{_toolName} activated"");
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

[CreateAssetMenu(fileName = ""{_toolName}Config"", menuName = ""Tools/{_toolName}Config"")]
public class {_toolName}Config : ScriptableObject
{{
    public GameObject HeldPrefab;
}}";
    }
}