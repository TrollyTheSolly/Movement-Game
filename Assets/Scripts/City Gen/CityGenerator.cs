using UnityEngine;
using System.Collections.Generic;

public class ProceduralCityGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public GameObject buildingPrefab;
    public int chunkSize = 100; // Size of each city chunk
    public int generationRadius = 200; // How far ahead to generate
    public float buildingDensity = 0.3f; // 0-1, how packed buildings are
    public int BuildingsPerChunk = 1;

    [Header("Building Settings")]
    public Vector2 buildingWidthRange = new Vector2(5, 20);
    public Vector2 buildingDepthRange = new Vector2(5, 20);
    public Vector2 buildingHeightRange = new Vector2(10, 50);
    public Material[] buildingMaterials;

    private Transform player;
    private Dictionary<Vector2Int, bool> generatedChunks = new Dictionary<Vector2Int, bool>();
    private List<GameObject> spawnedBuildings = new List<GameObject>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("No player found in scene! Add a GameObject with 'Player' tag.");
            return;
        }

        GenerateInitialCity();
    }

    private void Update()
    {
        if (player == null) return;

        Vector2Int currentChunk = GetCurrentChunkCoord();

        // Check all chunks within generation radius
        int chunksInRadius = Mathf.CeilToInt(generationRadius / (float)chunkSize);

        for (int x = -chunksInRadius; x <= chunksInRadius; x++)
        {
            for (int z = -chunksInRadius; z <= chunksInRadius; z++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunk.x + x, currentChunk.y + z);

                if (!generatedChunks.ContainsKey(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                    generatedChunks[chunkCoord] = true;
                }
            }
        }

        // Optional: Clean up buildings too far away to save memory
        CleanupDistantBuildings();
    }

    private void GenerateInitialCity()
    {
        int chunksInRadius = Mathf.CeilToInt(generationRadius / (float)chunkSize);

        for (int x = -chunksInRadius; x <= chunksInRadius; x++)
        {
            for (int z = -chunksInRadius; z <= chunksInRadius; z++)
            {
                GenerateChunk(new Vector2Int(x, z));
                generatedChunks[new Vector2Int(x, z)] = true;
            }
        }
    }

    private void GenerateChunk(Vector2Int chunkCoord)
    {
        int buildingsPerChunk = BuildingsPerChunk;

        for (int i = 0; i < buildingsPerChunk; i++)
        {
            // Calculate position within chunk
            float xPos = chunkCoord.x * chunkSize + Random.Range(0, chunkSize);
            float zPos = chunkCoord.y * chunkSize + Random.Range(0, chunkSize);

            // Skip if position is too close to player (to avoid spawning inside player)
            if (Vector3.Distance(new Vector3(xPos, 0, zPos), player.position) < 20f)
                continue;

            CreateBuilding(new Vector3(xPos, 0, zPos));
        }
    }

    private void CreateBuilding(Vector3 position)
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("No building prefab assigned!");
            return;
        }

        GameObject building = Instantiate(buildingPrefab);

        // Randomize building size
        float width = Random.Range(buildingWidthRange.x, buildingWidthRange.y);
        float depth = Random.Range(buildingDepthRange.x, buildingDepthRange.y);
        float height = Random.Range(buildingHeightRange.x, buildingHeightRange.y);

        building.transform.localScale = new Vector3(width, height, depth);
        building.transform.position = position;

        // Assign random material if available
        if (buildingMaterials != null && buildingMaterials.Length > 0)
        {
            Renderer renderer = building.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = buildingMaterials[Random.Range(0, buildingMaterials.Length)];
            }
        }

        spawnedBuildings.Add(building);
    }

    private Vector2Int GetCurrentChunkCoord()
    {
        int x = Mathf.FloorToInt(player.position.x / chunkSize);
        int z = Mathf.FloorToInt(player.position.z / chunkSize);
        return new Vector2Int(x, z);
    }

    private void CleanupDistantBuildings()
    {
        float cleanupDistance = generationRadius * 1.5f;
        Vector3 playerPos = player.position;

        for (int i = spawnedBuildings.Count - 1; i >= 0; i--)
        {
            if (spawnedBuildings[i] == null)
            {
                spawnedBuildings.RemoveAt(i);
                continue;
            }

            // Calculate horizontal distance only (ignoring Y axis)
            Vector2 playerPosHorizontal = new Vector2(playerPos.x, playerPos.z);
            Vector2 buildingPosHorizontal = new Vector2(spawnedBuildings[i].transform.position.x, spawnedBuildings[i].transform.position.z);
            float horizontalDistance = Vector2.Distance(playerPosHorizontal, buildingPosHorizontal);

            if (horizontalDistance > cleanupDistance)
            {
                Destroy(spawnedBuildings[i]);
                spawnedBuildings.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Draw generation radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, generationRadius);

        // Draw current chunk
        Vector2Int chunk = GetCurrentChunkCoord();
        Vector3 chunkCenter = new Vector3(
            chunk.x * chunkSize + chunkSize * 0.5f,
            0,
            chunk.y * chunkSize + chunkSize * 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(chunkCenter, new Vector3(chunkSize, 1, chunkSize));
    }
}