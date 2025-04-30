using UnityEngine;
using UnityEngine.UI; // For Layout Group (optional but recommended)
using TMPro;          // For TextMeshPro Text
using System.Collections.Generic;
using System.Linq;      // For LINQ operations like .ToArray()

public class ScoreManager : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Player References")]
    [SerializeField] private string playerTag = "Player"; // Tag to find the player GameObject
    private GameObject _player;
    private PlayerStateManager _playerStateManager; // Assuming this script exists on the player

    [Header("Trick Definitions")]
    [Tooltip("Assign all your Trick Definition ScriptableObjects here.")]
    [SerializeField] private List<TrickDefinition> availableTricks = new List<TrickDefinition>();

    [Header("Scoring & UI")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] private TextMeshProUGUI totalScoreText; // Assign a TextMeshProUGUI element to show the total score
    //[SerializeField] private GameObject trickTextPrefab; // Assign a Prefab with a TextMeshProUGUI component
    [SerializeField] private Transform trickTextContainer; // Assign a Panel/GameObject (ideally with a VerticalLayoutGroup) where trick texts will appear

    [Header("History Settings")]
    [SerializeField] private int historySize = 5; // Increased size might be needed for longer trick sequences

    // --- Runtime Data ---
    private List<ActiveTrick> _activeTricks = new List<ActiveTrick>();
    private Queue<State> _stateHistory = new Queue<State>(); // Stores recent player states

    // --- Initialization ---
    private void Start()
    {
        // Find Player and StateManager
        _player = GameObject.FindGameObjectWithTag(playerTag);
        if (_player == null)
        {
            Debug.LogError($"ScoreManager: Could not find GameObject with tag '{playerTag}'.");
            enabled = false; // Disable this script if player not found
            return;
        }

        _playerStateManager = _player.GetComponent<PlayerStateManager>();
        if (_playerStateManager == null)
        {
            Debug.LogError($"ScoreManager: Player GameObject '{_player.name}' does not have a PlayerStateManager component.");
            enabled = false; // Disable if component missing
            return;
        }

        if (trickTextContainer == null)
        {
            Debug.LogError("ScoreManager: Trick Text Container is not assigned.");
            enabled = false;
            return;
        }

        UpdateTotalScoreUI(); // Initialize total score display
    }

    // --- Main Loop ---
    public void FixedTick()
    {
        if (_playerStateManager == null) return; // Don't run if setup failed

        StoreState();        // Record the player's current state
        CheckForNewTricks(); // See if the recent history matches any defined trick
        UpdateActiveTricks(); // Handle timeouts and score banking for active tricks
    }

    // --- State Handling ---
    private void StoreState()
    {
        // Optional: Only add state if it's different from the last one to avoid spamming the queue
        State currentState = _playerStateManager.GetCurrentState();
        if (_stateHistory.Count == 0 || _stateHistory.Last() != currentState)
        {
             _stateHistory.Enqueue(currentState);
        }

        // Maintain history size limit
        while (_stateHistory.Count > historySize)
        {
            _stateHistory.Dequeue();
        }
    }

    // --- Trick Detection ---
    private void CheckForNewTricks()
    {
        if (_stateHistory.Count == 0) return; // Need at least one state

        // Check each defined trick against the end of the history
        foreach (TrickDefinition trickDef in availableTricks)
        {
            if (MatchesSequence(trickDef.requiredSequence))
            {
                // We found a match!
                PerformTrick(trickDef);
            }
        }
    }

    // Checks if the *end* of the state history matches the required sequence
    private bool MatchesSequence(params State[] sequence)
    {
        if (sequence == null || sequence.Length == 0) return false; // Invalid sequence
        if (_stateHistory.Count < sequence.Length) return false;  // Not enough history

        // Get the relevant part of the history queue as an array
        State[] historyEndArray = _stateHistory.ToArray(); // Gets all items
        int historyStartIndex = historyEndArray.Length - sequence.Length;

        // Compare element by element
        for (int i = 0; i < sequence.Length; i++)
        {
            if (historyEndArray[historyStartIndex + i] != sequence[i])
            {
                return false; // Mismatch found
            }
        }

        // If we got here, the sequence matches
        Debug.Log($"Matched sequence for trick!"); // Add name later if needed
        return true;
    }

    // --- Active Trick Management ---

    // Called when a trick sequence is successfully matched
    private void PerformTrick(TrickDefinition definition)
    {
        // Check if this trick is already active (for combo)
        ActiveTrick existingTrick = FindActiveTrick(definition);

        if (existingTrick != null)
        {
            // It's a combo! Increment the multiplier and reset its timer
            if (existingTrick.Definition.requiredStartMultiplier == 0)
            {
                existingTrick.IncrementMultiplier();
            }
            else
            {
                if (existingTrick.StartMultiplier >= definition.requiredStartMultiplier)
                {
                    existingTrick.IncrementMultiplier();
                }
                else
                {
                    existingTrick.IncrementStartMultiplier();
                    Debug.Log("Incrementing start multiplier");
                }
            }
            
            Debug.Log($"{definition.trickName} combo! Now x{existingTrick.Multiplier}");
        }
        else
        {
            // It's a new trick (or the start of a combo chain)
            Debug.Log($"Performed new trick: {definition.trickName}");

            // Instantiate the UI text element
            GameObject newTextGO = Instantiate(definition.trickTextPrefab, trickTextContainer);
            newTextGO.SetActive(false);
            TextMeshProUGUI newTMP = newTextGO.GetComponentInChildren<TextMeshProUGUI>();

            if (newTMP != null)
            {
                // Create a new ActiveTrick instance to track it
                ActiveTrick newActiveTrick = new ActiveTrick(definition, newTMP);
                _activeTricks.Add(newActiveTrick);
            }
            else
            {
                Debug.LogError("Failed to find TextMeshProUGUI component on instantiated prefab!");
                Destroy(newTextGO); // Clean up the wrongly instantiated object
            }
        }
    }

    // Finds an active trick matching the given definition
    private ActiveTrick FindActiveTrick(TrickDefinition definition)
    {
        foreach (ActiveTrick active in _activeTricks)
        {
            // Compare using the ScriptableObject reference equality
            if (active.Definition == definition)
            {
                return active;
            }
        }
        return null; // Not found
    }


    // Handles timeouts for tricks currently in the combo list
    private void UpdateActiveTricks()
    {
        // Iterate backwards because we might remove items from the list
        for (int i = _activeTricks.Count - 1; i >= 0; i--)
        {
            ActiveTrick trick = _activeTricks[i];

            // Check if the time since the last performance exceeds the trick's specific timeout
            if (Time.time > trick.LastPerformedTime + trick.Definition.comboTimeout)
            {
                // Timeout! Add score, clean up UI, and remove from active list
                Debug.Log($"{trick.Definition.trickName} timed out. Adding {trick.GetFinalScore()} points.");
                EndTrick(trick, i);
            }

            if (trick.Definition.hasEndSequence)
            {
                if (MatchesSequence(trick.Definition.endSequence))
                {
                    EndTrick(trick, i);
                }
            }

            if (trick.Definition.requiredStartMultiplier > 0)
            {
                if (trick.Multiplier < trick.Definition.requiredStartMultiplier)
                {
                    trick.UiTextElement.enabled = false;
                }
                else
                {
                    trick.UiTextElement.enabled = true;
                    trick.UiTextElement.gameObject.SetActive(true);
                }
            }
            else
            {
                trick.UiTextElement.gameObject.SetActive(true);
            }
        }
    }

    private void EndTrick(ActiveTrick trick, int index)
    {
        AddScore(trick.GetFinalScore()); // Add score to total
        trick.CleanUp();                 // Destroy UI element
        _activeTricks.RemoveAt(index);       // Remove from the list
    }

    // --- Scoring ---
    private void AddScore(int amount)
    {
        totalScore += amount;
        UpdateTotalScoreUI();
    }

    private void UpdateTotalScoreUI()
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Score: {totalScore}";
        }
    }

    // --- Public Accessors (Optional) ---
    public int GetTotalScore()
    {
        return totalScore;
    }
}