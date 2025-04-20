using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Tools/Jump Pad Tool")]
public class JumpPadConfig : ScriptableObject
{
    [FormerlySerializedAs("ThrowForce")] public float throwForce;
    [FormerlySerializedAs("JumpForce")] public float jumpForce;
    [FormerlySerializedAs("JumpPadPrefab")] public GameObject jumpPadPrefab;
}
