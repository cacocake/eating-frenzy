using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPopulator : MonoBehaviour {
    
    private struct ConsumablePrefabData {
        public GameObject[] PrefabContainer;
        public float CollisionCheckRadius;
    }

    [SerializeField] private GameObject _edgePrefab;
    [SerializeField] private GameObject[] _decorationPropPrefabs;
    [SerializeField] private GameObject[] _smallConsumablePrefabs;
    [SerializeField] private GameObject[] _mediumConsumablePrefabs;
    [SerializeField] private GameObject[] _largeConsumablePrefabs;
    [SerializeField] private GameObject[] _animalConsumablePrefabs;
    [SerializeField] private float _edgePrefabSpacing = 5.0f;
    [SerializeField] private ushort _consumablePrefabSpawnCount = 1000;
    [SerializeField] private ushort _decorationPropPrefabSpawnCount = 1000;
    [SerializeField] private float _smallConsumableSpawnWeight = 0.5f;
    [SerializeField] private float _mediumConsumableSpawnWeight = 0.5f;
    [SerializeField] private float _largeConsumableSpawnWeight = 0.25f;
    [SerializeField] private float _animalConsumableSpawnWeight = 0.25f;
    [SerializeField] private float _smallConsumableSpawnCollisionCheckRadius = 2.0f;
    [SerializeField] private float _mediumConsumableSpawnCollisionCheckRadius = 4.0f;
    [SerializeField] private float _largeConsumableSpawnCollisionCheckRadius = 6.0f;
    [SerializeField] private float _animalConsumableSpawnCollisionCheckRadius = 10.0f;
    [SerializeField] private float k_consumableSafeDistanceFromEdge = 5.0f;
    [SerializeField] private float k_decorationSafeDistanceFromEdge = 2.0f;

    private ConsumablePrefabData _smallConsumablePrefabData;
    private ConsumablePrefabData _mediumConsumablePrefabData;
    private ConsumablePrefabData _largeConsumablePrefabData;
    private ConsumablePrefabData _animalConsumablePrefabData;
    private GameObject _floorObject;
    private ConsumablePrefabData[] _consumables;
    private float[] _weights;
    private float _mapWidth;
    private float _mapHeight;
    
    private const float k_consumableHeightSpawn = 0.75f;

    private void Start() {
        _floorObject = GameObject.FindGameObjectWithTag("Floor");
        _mapWidth = _floorObject.GetComponent<Collider>().bounds.size.x;
        _mapHeight = _floorObject.GetComponent<Collider>().bounds.size.z;
        _smallConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _smallConsumablePrefabs,
            CollisionCheckRadius = _smallConsumableSpawnCollisionCheckRadius
        };
        _mediumConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _mediumConsumablePrefabs,
            CollisionCheckRadius = _mediumConsumableSpawnCollisionCheckRadius
        };
        _largeConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _largeConsumablePrefabs,
            CollisionCheckRadius = _largeConsumableSpawnCollisionCheckRadius
        };
        _animalConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _animalConsumablePrefabs,
            CollisionCheckRadius = _animalConsumableSpawnCollisionCheckRadius
        };
        _consumables = new ConsumablePrefabData[] { _smallConsumablePrefabData, _mediumConsumablePrefabData, _largeConsumablePrefabData, _animalConsumablePrefabData };
        _weights = new float[] { _smallConsumableSpawnWeight, _mediumConsumableSpawnWeight, _largeConsumableSpawnWeight, _animalConsumableSpawnWeight };
        PopulateConsumables();
    }

    private void PopulateConsumables() {
        List<Vector3> spawnPositions = new List<Vector3>();
        int maxAttempts = 10000;
        int attempts = 0;
        while(_consumablePrefabSpawnCount > 0 && attempts < maxAttempts) {
            ConsumablePrefabData consumablePrefabData = GetWeightedRandomConsumablePrefab(_weights);
            
            float x = Random.Range(-(_mapWidth / 2) + k_consumableSafeDistanceFromEdge, (_mapWidth / 2) - k_consumableSafeDistanceFromEdge);
            float z = Random.Range(-(_mapHeight / 2) + k_consumableSafeDistanceFromEdge, (_mapHeight / 2) - k_consumableSafeDistanceFromEdge);
            bool isNewPositonTooCloseTooSpawnedPositions = spawnPositions.Any(existingSpawnedPosition => (existingSpawnedPosition - new Vector3(x, k_consumableHeightSpawn, z)).sqrMagnitude < Mathf.Pow(consumablePrefabData.CollisionCheckRadius, 2));
            
            if(isNewPositonTooCloseTooSpawnedPositions) {
                attempts++;
                continue;
            }

            ushort randomIndex = (ushort)Random.Range(0, consumablePrefabData.PrefabContainer.Length);
            spawnPositions.Add(new Vector3(x, 0.5f, z));
            GameObject prefab = Instantiate(consumablePrefabData.PrefabContainer[randomIndex], new Vector3(x, k_consumableHeightSpawn, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            
            if(prefab == null) {
                Debug.LogError("Consumable prefab is null!");
                attempts++;
                continue;
            }

            prefab.transform.parent = _floorObject.transform;
            _consumablePrefabSpawnCount--;
            attempts = 0;
        }
    }

    private void PopulateEdges() { // No longer necessary. Used to populate the edges of the map to save in scene and for quick iteration. Left in for evaluation purposes and future randomization.
        for(float x = -_mapWidth / 2; x < _mapWidth / 2; x += _edgePrefabSpacing) {
            Instantiate(_edgePrefab, new Vector3(x, 0.1f, (- _mapHeight / 2) + _edgePrefabSpacing), Quaternion.Euler(0, Random.Range(0f, 360f), 0)).transform.parent = _floorObject.transform;
            Instantiate(_edgePrefab, new Vector3(x, 0.1f, (_mapHeight / 2) - _edgePrefabSpacing), Quaternion.Euler(0, Random.Range(0f, 360f), 0)).transform.parent = _floorObject.transform;
        }
        
        for(float z = -_mapHeight / 2; z < _mapHeight / 2; z += _edgePrefabSpacing) {
            Instantiate(_edgePrefab, new Vector3((- _mapWidth / 2) + _edgePrefabSpacing, 0.1f, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0)).transform.parent = _floorObject.transform;
            Instantiate(_edgePrefab, new Vector3((_mapWidth / 2) - _edgePrefabSpacing, 0.1f, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0)).transform.parent = _floorObject.transform;
        }
    }

    private void PopulateDecorations() { // No longer necessary. Used to populate the map with decorations to save in scene and for quick iteration. Left in for evaluation purposes and future randomization.
        for(int propCount = 0; propCount <= _decorationPropPrefabSpawnCount; propCount++) {
            float x = Random.Range(-(_mapWidth / 2) + k_decorationSafeDistanceFromEdge, (_mapWidth / 2) - k_decorationSafeDistanceFromEdge);
            float z = Random.Range(-(_mapHeight / 2) + k_decorationSafeDistanceFromEdge, (_mapHeight / 2) - k_decorationSafeDistanceFromEdge);

            ushort randomIndex = (ushort)Random.Range(0, _decorationPropPrefabs.Length);
            GameObject prefab = Instantiate(_decorationPropPrefabs[randomIndex], new Vector3(x, 0.0f, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            if(prefab == null) {
                Debug.LogError("Decoration prefab is null!");
                continue;
            }
            prefab.transform.parent = _floorObject.transform;;
        }
    }

    private ConsumablePrefabData GetWeightedRandomConsumablePrefab(float[] weights) {
        if (weights.Length == 0) {
            Debug.LogError("Weighted array is empty! Returning small consumable data.");
            return _smallConsumablePrefabData;
        }

        float totalWeight = weights.Sum();
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0.0f;
        for(int index = 0; index < weights.Length; index++) {
            cumulativeWeight += weights[index];
            if (randomValue < cumulativeWeight) {
                if (index >= _consumables.Length) {
                    Debug.LogError("Index out of bounds for consumables array! Returning small consumable data.");
                    return _smallConsumablePrefabData;
                }
                return _consumables[index];
            }
        }

        Debug.LogError("Should not reach here! Returning small consumable data.");
        return _smallConsumablePrefabData;
    }

}
