using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Teleport Orb Tool")]
public class TeleportOrbConfig : ScriptableObject
{
    public float orbSpeed = 1f;
    public GameObject teleportOrbPrefab;
    public float gravity = 1f;
}
