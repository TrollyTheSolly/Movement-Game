using UnityEngine;

public class SpinnerBehaviour : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 90f;

    private void FixedUpdate()
    {
        // Rotate around the specified axis at the set speed
        transform.Rotate(rotationAxis, rotationSpeed * Time.fixedDeltaTime);
    }
}