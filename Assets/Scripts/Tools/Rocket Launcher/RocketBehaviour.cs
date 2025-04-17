using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private float rocketSpeed = 1f;
    private float explosionRadius = 1f;
    private float explosionForce = 1f;
    private float explosionRadiusFalloff = 1f;
    private VelocityModifierSystem modifierSystem;
    private GameObject explosionEffect;


    public void Awake()
    {
        modifierSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<VelocityModifierSystem>();
    }

    public void Configure(float speed, float explosionRadius, float explosionForce, float explosionRadiusFalloff, GameObject explosionSystem)
    {
        this.rocketSpeed = speed;
        this.explosionRadius = explosionRadius;
        this.explosionForce = explosionForce;
        this.explosionRadiusFalloff = explosionRadiusFalloff;
        explosionEffect = explosionSystem;
    }

    private void Update()
    {
        transform.position += rocketSpeed * transform.forward * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Player"))
            {
                Vector3 explosionDir = hitCollider.transform.position - transform.position;
                float explosionDistance = explosionDir.magnitude;

                // Normalize the direction
                explosionDir = explosionDir.normalized;

                // Calculate falloff based on distance
                float falloffFactor = 1f;
                if (explosionRadiusFalloff > 0)
                {
                    // Normalized distance (0 at center, 1 at edge of radius)
                    float normalizedDistance = explosionDistance / explosionRadius;
                    // Apply falloff curve (inverse power)
                    falloffFactor = Mathf.Pow(1 - normalizedDistance, explosionRadiusFalloff);
                    // Clamp to ensure it doesn't go negative
                    falloffFactor = Mathf.Clamp01(falloffFactor);
                }

                // Calculate final force
                float finalForce = explosionForce * falloffFactor;

                // Apply force to player
                modifierSystem.AddModifier(explosionDir * finalForce);
            }
        }

        // Destroy the rocket after explosion
        GameObject explosionInstance = GameObject.Instantiate(explosionEffect, position:transform.position, Quaternion.identity);
        Destroy(explosionInstance, 1f);
        Destroy(gameObject);
    }

}
