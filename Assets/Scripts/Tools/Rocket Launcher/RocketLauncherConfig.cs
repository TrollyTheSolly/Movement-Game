using UnityEngine;
[CreateAssetMenu(menuName = "Tools/Rocket Launcher Config")]
public class RocketLauncherConfig : ScriptableObject
{
    public GameObject rocketPrefab;
    public float rocketSpeed = 1f;
    public float explosionRadius = 1f;
    public float explosionForce = 1f;
    public float explosionRadiusFalloff = 1f;
    public GameObject explosionEffect;
}
