using UnityEngine;

public class MatTiler : MonoBehaviour
{
    [Tooltip("Base tiling size when scale is 1")]
    public float tilingBase = 1.0f;

    private void Start()
    {
        UpdateTiling();
    }

    public void UpdateTiling()
    {
        // Get the renderer component
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.sharedMaterial == null)
            return;

        // Get the object's scale
        Vector3 objectScale = transform.localScale;

        // Calculate tiling based on the object's scale
        Vector2 tiling = new Vector2(
            tilingBase * objectScale.x,
            tilingBase * objectScale.y
        );

        // Create a new material instance to avoid affecting other objects with the same material
        Material material = new Material(renderer.sharedMaterial);
        material.mainTextureScale = tiling;

        // Apply the material
        renderer.material = material;
    }

    // Call this if the object's scale changes after instantiation
    private void OnTransformParentChanged()
    {
        UpdateTiling();
    }

    private void OnValidate()
    {
        // Update in editor when values change
        if (Application.isEditor && !Application.isPlaying)
            UpdateTiling();
    }
}
