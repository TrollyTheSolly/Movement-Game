using UnityEngine;
using UnityEngine.Serialization;

// Make it easy to create new Trick definitions in the Project window
[CreateAssetMenu(fileName = "New Trick", menuName = "Scoring/Trick Definition")]
public class TrickDefinition : ScriptableObject
{
    public string trickName = "Unnamed Trick";
    // Define the sequence of states required for this trick
    public State[] requiredSequence;
    public int baseScore = 100;
    // How long the combo window stays open for this trick after performing it
    public float comboTimeout = 3.0f;
    public bool hasEndSequence = false;
    public State[] endSequence;
    [FormerlySerializedAs("startDelay")] public int requiredStartMultiplier = 0;
    public GameObject trickTextPrefab;
}