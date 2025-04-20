using System.Runtime.CompilerServices;
using UnityEngine;

public class TeleportOrbBehaviour : MonoBehaviour
{
    private float _orbSpeed;
    private Vector3 _orbDirection;
    private GameObject _player;
    private float _gravity;
    private Vector3 _velocity = Vector3.zero;
    public void Configure(Transform cameraTransform, TeleportOrbConfig config, GameObject newPlayer)
    {
        _orbSpeed = config.orbSpeed;
        _orbDirection = cameraTransform.forward;
        _player = newPlayer;
        _gravity = config.gravity;
        _velocity = _orbSpeed * _orbDirection;
    }

    public void FixedUpdate()
    {
        _velocity.y -= _gravity;
        transform.position += _velocity * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            CharacterController controller = _player.GetComponent<CharacterController>();
            controller.enabled = false;
            _player.transform.position = transform.position;
            controller.enabled = true;

            GameObject.Destroy(this);
        }
    }
}
