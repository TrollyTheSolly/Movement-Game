using TMPro;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private Vector3 startVelocity = Vector3.zero;
    private VelocityModifierSystem modifierSystem;
    private RocketLauncherConfig config;
    private Rigidbody rb;


    public void Awake()
    {
        modifierSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<VelocityModifierSystem>();
        rb = GetComponent<Rigidbody>();
    }

    public void Configure(RocketLauncherConfig rocketConfig, Vector3 playerVelocity)
    {
        this.config = rocketConfig;
        this.startVelocity = playerVelocity;
    }

    private void Update()
    {
        Vector3 totalVelocity = config.rocketSpeed * transform.forward;

        if (config.relativeRocketspeed)
        {
            totalVelocity += startVelocity;
        }
        rb.MovePosition(rb.position + totalVelocity * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, config.explosionRadius);
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
                if (config.explosionRadiusFalloff > 0)
                {
                    // Normalized distance (0 at center, 1 at edge of radius)
                    float normalizedDistance = explosionDistance / config.explosionRadius;
                    // Apply falloff curve (inverse power)
                    falloffFactor = Mathf.Pow(1 - normalizedDistance, config.explosionRadiusFalloff);
                    // Clamp to ensure it doesn't go negative
                    falloffFactor = Mathf.Clamp01(falloffFactor);
                }

                // Calculate final force
                float finalForce = config.explosionForce * falloffFactor;

                // Apply force to player
                modifierSystem.AddModifier(explosionDir * finalForce);
            }
        }

        // Destroy the rocket after explosion
        GameObject explosionInstance = GameObject.Instantiate(config.explosionEffect, position:transform.position, Quaternion.identity);
        Destroy(explosionInstance, 1f);
        Destroy(gameObject);
    }

}
