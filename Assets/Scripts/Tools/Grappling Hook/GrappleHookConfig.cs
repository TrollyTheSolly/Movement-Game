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
    public float grappleControlFactor;
    public bool useForgivingAim = true;      // Enable/disable the forgiving aim system
    public int spreadRayCount = 8;           // Number of rays to fire in the spread
    public float spreadAngle = 10f;          // The total angle of the horizontal spread in degrees
    public bool useVerticalSpread = true;    // Whether to also spread rays vertically
    public int verticalRayCount = 3;         // Number of rays to use for vertical spread
    public float verticalSpreadAngle = 8f;   // The total angle of the vertical spread in degrees  
    public bool visualizeRays = false;       // Debug option to visualize rays
}
