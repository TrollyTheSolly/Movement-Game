using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCubeScaler : MonoBehaviour
{
    public GameObject edgePrefab; // Assign the edge prefab in the inspector
    public Vector3 edgeThickness = new Vector3(0.1f, 0.1f, 0.1f); // Customizable thickness of the edge
    private List<GameObject> edges = new List<GameObject>();

    void Start()
    {
        SpawnEdges();
    }

    void SpawnEdges()
    {
        if (edgePrefab == null)
        {
            Debug.LogError("Edge prefab not assigned!");
            return;
        }

        Vector3 cubeScale = transform.localScale;

        // Get the half extents
        float hx = cubeScale.x / 2f;
        float hy = cubeScale.y / 2f;
        float hz = cubeScale.z / 2f;

        // 12 edge directions: each one connects two corners along one axis.
        Vector3[] edgeCenters = new Vector3[]
{
    // Top edges
    new Vector3(0,  hy,  hz), // Top front (X)
    new Vector3(0,  hy, -hz), // Top back  (X)
    new Vector3(-hx, hy, 0),  // Top left  (Z)
    new Vector3( hx, hy, 0),  // Top right (Z)

    // Bottom edges
    new Vector3(0, -hy,  hz), // Bottom front (X)
    new Vector3(0, -hy, -hz), // Bottom back  (X)
    new Vector3(-hx, -hy, 0), // Bottom left  (Z)
    new Vector3( hx, -hy, 0), // Bottom right (Z)

    // Vertical edges
    new Vector3(-hx, 0,  hz), // Front left  (Y)
    new Vector3( hx, 0,  hz), // Front right (Y)
    new Vector3(-hx, 0, -hz), // Back left   (Y)
    new Vector3( hx, 0, -hz), // Back right  (Y)
};

        Vector3[] edgeScales = new Vector3[]
        {
    // Top front/back (X direction)
    new Vector3(cubeScale.x, edgeThickness.y, edgeThickness.z),
    new Vector3(cubeScale.x, edgeThickness.y, edgeThickness.z),

    // Top left/right (Z direction)
    new Vector3(edgeThickness.x, edgeThickness.y, cubeScale.z),
    new Vector3(edgeThickness.x, edgeThickness.y, cubeScale.z),

    // Bottom front/back (X direction)
    new Vector3(cubeScale.x, edgeThickness.y, edgeThickness.z),
    new Vector3(cubeScale.x, edgeThickness.y, edgeThickness.z),

    // Bottom left/right (Z direction)
    new Vector3(edgeThickness.x, edgeThickness.y, cubeScale.z),
    new Vector3(edgeThickness.x, edgeThickness.y, cubeScale.z),

    // Vertical edges (Y direction)
    new Vector3(edgeThickness.x, cubeScale.y, edgeThickness.z),
    new Vector3(edgeThickness.x, cubeScale.y, edgeThickness.z),
    new Vector3(edgeThickness.x, cubeScale.y, edgeThickness.z),
    new Vector3(edgeThickness.x, cubeScale.y, edgeThickness.z),
        };


        for (int i = 0; i < edgeCenters.Length; i++)
        {
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.position = transform.position + transform.rotation * edgeCenters[i];
            edge.transform.rotation = transform.rotation;
            edge.transform.localScale = edgeScales[i];
            edges.Add(edge);
        }
    }

    private void OnDestroy()
    {
        foreach(var edge in edges)
        {
            Destroy(edge);
        }
    }
}