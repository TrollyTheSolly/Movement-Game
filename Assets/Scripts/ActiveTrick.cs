using TMPro;
using UnityEngine;

// Make sure TextMeshPro is imported (Window -> TextMeshPro -> Import TMP Essential Resources)


public class ActiveTrick

{
    public TrickDefinition Definition { get; }

    public int Multiplier { get; set; }

    public float LastPerformedTime { get; set; }

    public TextMeshProUGUI UiTextElement { get; set; } // Reference to the UI Text

    public int StartMultiplier { get; set; }

    private readonly ComboFeelPlayer _player;


    public ActiveTrick(TrickDefinition definition, TextMeshProUGUI uiElement)
    {
        Definition = definition;
        Multiplier = 1; // Starts at 1x
        LastPerformedTime = Time.time;
        UiTextElement = uiElement;
        UpdateUIText(); // Set initial text
        _player = uiElement.gameObject.GetComponent<ComboFeelPlayer>();
    }


    public void IncrementMultiplier()
    {
        Multiplier++;
        LastPerformedTime = Time.time; // Reset timeout timer
        UpdateUIText();
        _player.PlayUpdate();
    }


    public void IncrementStartMultiplier()
    {
        StartMultiplier++;
        LastPerformedTime = Time.time;
    }


    public void UpdateUIText()
    {
        if (UiTextElement != null)
            // Format: Trick Name x Multiplier = Current Score
            UiTextElement.text = $"{Definition.trickName} x {Multiplier} = {Definition.baseScore * Multiplier}";
    }


    public int GetFinalScore()
    {
        // Check if the start multiplier threshold was ever met
        if (StartMultiplier >= Definition.requiredStartMultiplier)
            // Threshold met, calculate score normally
            return Definition.baseScore * Multiplier;

        // Threshold NOT met, award NO points for this trick instance
        Debug.Log(
            $"Trick '{Definition.trickName}' ended before meeting start multiplier requirement ({StartMultiplier}/{Definition.requiredStartMultiplier}). No score awarded.");
        return 0;
    }


// Call this when the trick expires or is banked

    public void CleanUp()
    {
        if (UiTextElement != null)
        {
            if (_player != null)
            {
                _player.PlayDisable(); // Trigger the disable animation
                float destroyDelay = _player.GetDisableAnimationDuration();
                if (UiTextElement.transform.parent)
                {
                    Object.Destroy(UiTextElement.transform.parent.gameObject, destroyDelay);
                }
                else
                {
                    Object.Destroy(UiTextElement.gameObject);
                }
                
            }
            else
            {
                if (UiTextElement.transform.parent)
                {
                    Object.Destroy(UiTextElement.transform.parent.gameObject);
                }
                else
                {
                    Object.Destroy(UiTextElement.gameObject);
                }
            }
        }
    }
}