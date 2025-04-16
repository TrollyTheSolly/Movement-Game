using UnityEngine;

public class JumpPadBehaviour : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] VelocityModifierSystem modifierSystem;


    public void Awake()
    {
        modifierSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<VelocityModifierSystem>();
    }
    public void Configure(float force)
    {
        jumpForce = force;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered jump pad");
            modifierSystem.AddModifier(new Vector3(0, jumpForce, 0));
        }
    }
}
