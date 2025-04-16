using System.Runtime.CompilerServices;
using UnityEngine;

public class TeleportOrbBehaviour : MonoBehaviour
{
    private float orbSpeed;
    private Vector3 orbDirection;
    private GameObject player;
    private float gravity;
    private Vector3 velocity = Vector3.zero;
    public void Configure(Transform cameraTransform, TeleportOrbConfig config, GameObject newPlayer)
    {
        orbSpeed = config.orbSpeed;
        orbDirection = cameraTransform.forward;
        player = newPlayer;
        gravity = config.gravity;
        velocity = orbSpeed * orbDirection;
    }

    public void FixedUpdate()
    {
        velocity.y -= gravity;
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            controller.enabled = false;
            player.transform.position = transform.position;
            controller.enabled = true;

            GameObject.Destroy(this);
        }
    }
}
