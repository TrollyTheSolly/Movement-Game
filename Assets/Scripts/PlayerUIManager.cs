using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    public UIDocument ui;
    private List<DropdownField> _dropdowns = new List<DropdownField>();
    [SerializeField] private PlayerToolManager toolManager;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ui.enabled = !ui.enabled;
        }
    }

    private void Awake()
    {
        // Grab and register callbacks on each dropdown
        for (int i = 1; i <= 4; i++)
        {
            DropdownField field = ui.rootVisualElement.Q<DropdownField>(i.ToString());
            if (field != null)
            {
                int index = i - 1; // Closure-safe index
                _dropdowns.Add(field);

                // Register callback
                field.RegisterValueChangedCallback(evt =>
                {
                    switch (evt.newValue)
                    {
                        case "Grappling Hook":
                            toolManager.Toolbelt[index] = new ToolGrapplingHook();
                            break;
                        case "Ghost Dash":
                            toolManager.Toolbelt[index] = new ToolGhostDash();
                            break;
                        case "Rocket Launcher":
                            toolManager.Toolbelt[index] = new ToolRocketLauncher();
                            break;
                        case "Momentum Freezer":
                            toolManager.Toolbelt[index] = new ToolMomentumFreezer();
                            break;
                        default:
                            toolManager.Toolbelt[index] = null;
                            break;
                    }
                });
            }
        }
    }
}
