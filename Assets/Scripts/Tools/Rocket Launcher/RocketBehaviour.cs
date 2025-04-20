using TMPro;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private Vector3 _startVelocity = Vector3.zero;
    private VelocityModifierSystem _modifierSystem;
    private RocketLauncherConfig _config;
    private Rigidbody _rb;


    public void Awake()
    {
        _modifierSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<VelocityModifierSystem>();
        _rb = GetComponent<Rigidbody>();
    }

    public void Configure(RocketLauncherConfig rocketConfig, Vector3 playerVelocity)
    {
        this._config = rocketConfig;
        this._startVelocity = playerVelocity;
    }

    private void Update()
    {
        Vector3 totalVelocity = _config.rocketSpeed * transform.forward;

        if (_config.relativeRocketspeed)
        {
            totalVelocity += _startVelocity;
        }
        _rb.MovePosition(_rb.position + totalVelocity * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _config.explosionRadius);
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
                if (_config.explosionRadiusFalloff > 0)
                {
                    // Normalized distance (0 at center, 1 at edge of radius)
                    float normalizedDistance = explosionDistance / _config.explosionRadius;
                    // Apply falloff curve (inverse power)
                    falloffFactor = Mathf.Pow(1 - normalizedDistance, _config.explosionRadiusFalloff);
                    // Clamp to ensure it doesn't go negative
                    falloffFactor = Mathf.Clamp01(falloffFactor);
                }

                // Calculate final force
                float finalForce = _config.explosionForce * falloffFactor;

                // Apply force to player
                _modifierSystem.AddModifier(explosionDir * finalForce);
            }
        }

        // Destroy the rocket after explosion
        GameObject explosionInstance = GameObject.Instantiate(_config.explosionEffect, position:transform.position, Quaternion.identity);
        Destroy(explosionInstance, 1f);
        Destroy(gameObject);
    }

}
