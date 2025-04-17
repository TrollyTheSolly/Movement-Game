using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Grapple Hook Config")]
public class GrappleHookConfig : ScriptableObject
{
    public float ropeLength;
    public float grappleDistance;
    public float ropeStiffness;
    public GameObject ropePrefab;
    public LayerMask layerMask;
    public float dampingFactor;
}
