using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MomentumFreezerConfig", menuName = "Tools/MomentumFreezerConfig")]
public class MomentumFreezerConfig : ScriptableObject
{
    [FormerlySerializedAs("HeldMomentumFreezerPrefab")] public GameObject heldMomentumFreezerPrefab;
}
