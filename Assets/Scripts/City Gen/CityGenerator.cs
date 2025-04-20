using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Serialization;

public class ProceduralCityGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public GameObject buildingPrefab;
    public int chunkSize = 100; // Size of each city chunk
    public int generationRadius = 200; // How far ahead to generate
    public float buildingDensity = 0.3f; // 0-1, how packed buildings are
    [FormerlySerializedAs("BuildingsPerChunk")] public int buildingsPerChunk = 1;

    [Header("Building Settings")]
    public Vector2 buildingWidthRange = new Vector2(5, 20);
    public Vector2 buildingDepthRange = new Vector2(5, 20);
    public Vector2 buildingHeightRange = new Vector2(10, 50);
    public Material[] buildingMaterials;

    private Transform _player;
    private Dictionary<Vector2Int, bool> _generatedChunks = new Dictionary<Vector2Int, bool>();
    private List<GameObject> _spawnedBuildings = new List<GameObject>();

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (_player == null)
        {
            Debug.LogError("No player found in scene! Add a GameObject with 'Player' tag.");
            return;
        }

        GenerateInitialCity();
    }

    private void Update()
    {
        if (_player == null) return;

        Vector2Int currentChunk = GetCurrentChunkCoord();

        // Check all chunks within generation radius
        int chunksInRadius = Mathf.CeilToInt(generationRadius / (float)chunkSize);

        for (int x = -chunksInRadius; x <= chunksInRadius; x++)
        {
            for (int z = -chunksInRadius; z <= chunksInRadius; z++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunk.x + x, currentChunk.y + z);

                if (!_generatedChunks.ContainsKey(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                    _generatedChunks[chunkCoord] = true;
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
                _generatedChunks[new Vector2Int(x, z)] = true;
            }
        }
    }

    private void GenerateChunk(Vector2Int chunkCoord)
    {
        int buildingsPerChunk = this.buildingsPerChunk;

        for (int i = 0; i < buildingsPerChunk; i++)
        {
            // Calculate position within chunk
            float xPos = chunkCoord.x * chunkSize + Random.Range(0, chunkSize);
            float zPos = chunkCoord.y * chunkSize + Random.Range(0, chunkSize);

            // Skip if position is too close to player (to avoid spawning inside player)
            if (Vector3.Distance(new Vector3(xPos, 0, zPos), _player.position) < 20f)
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
        MatTiler textureTiling = building.AddComponent<MatTiler>();
        textureTiling.tilingBase = 0.15f;

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

        _spawnedBuildings.Add(building);
    }

    private Vector2Int GetCurrentChunkCoord()
    {
        int x = Mathf.FloorToInt(_player.position.x / chunkSize);
        int z = Mathf.FloorToInt(_player.position.z / chunkSize);
        return new Vector2Int(x, z);
    }

    private void CleanupDistantBuildings()
    {
        float cleanupDistance = generationRadius * 1.5f;
        Vector3 playerPos = _player.position;

        for (int i = _spawnedBuildings.Count - 1; i >= 0; i--)
        {
            if (_spawnedBuildings[i] == null)
            {
                _spawnedBuildings.RemoveAt(i);
                continue;
            }

            // Calculate horizontal distance only (ignoring Y axis)
            Vector2 playerPosHorizontal = new Vector2(playerPos.x, playerPos.z);
            Vector2 buildingPosHorizontal = new Vector2(_spawnedBuildings[i].transform.position.x, _spawnedBuildings[i].transform.position.z);
            float horizontalDistance = Vector2.Distance(playerPosHorizontal, buildingPosHorizontal);

            if (horizontalDistance > cleanupDistance)
            {
                Destroy(_spawnedBuildings[i]);
                _spawnedBuildings.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_player == null) return;

        // Draw generation radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_player.position, generationRadius);

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