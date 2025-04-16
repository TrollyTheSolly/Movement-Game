using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Jump Pad Tool")]
public class JumpPadConfig : ScriptableObject
{
    public float ThrowForce;
    public float JumpForce;
    public GameObject JumpPadPrefab;
}
