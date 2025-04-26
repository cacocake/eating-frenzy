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
    [SerializeField] private float _prefabSpacing = 5.0f;
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
    private ConsumablePrefabData _smallConsumablePrefabData;
    private ConsumablePrefabData _mediumConsumablePrefabData;
    private ConsumablePrefabData _largeConsumablePrefabData;
    private ConsumablePrefabData _animalConsumablePrefabData;
    private const float k_edgeSafeDistance = 5.0f;
    private GameObject _floorObject;
    private ConsumablePrefabData[] _consumables;
    private float[] _weights;

    private float _mapWidth;
    private float _mapHeight;
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
        _consumables = new ConsumablePrefabData[] {_smallConsumablePrefabData, _mediumConsumablePrefabData, _largeConsumablePrefabData, _animalConsumablePrefabData };
        _weights = new float[] { _smallConsumableSpawnWeight, _mediumConsumableSpawnWeight, _largeConsumableSpawnWeight, _animalConsumableSpawnWeight };
        PopulateEdges();
        PopulateConsumables();
        PopulateDecorations();
    }

    private void PopulateEdges() {
        for(float x = -_mapWidth / 2; x < _mapWidth / 2; x += _prefabSpacing) {
            Instantiate(_edgePrefab, new Vector3(x, 0.1f, (- _mapHeight / 2) + _prefabSpacing), Quaternion.identity).transform.parent = _floorObject.transform;
            Instantiate(_edgePrefab, new Vector3(x, 0.1f, (_mapHeight / 2) - _prefabSpacing), Quaternion.identity).transform.parent = _floorObject.transform;
        }
        for(float z = -_mapHeight / 2; z < _mapHeight / 2; z += _prefabSpacing) {
            Instantiate(_edgePrefab, new Vector3((- _mapWidth / 2) + _prefabSpacing, 0.1f, z), Quaternion.identity).transform.parent = _floorObject.transform;
            Instantiate(_edgePrefab, new Vector3((_mapWidth / 2) - _prefabSpacing, 0.1f, z), Quaternion.identity).transform.parent = _floorObject.transform;
        }
    }

    private void PopulateConsumables() {
        List<Vector3> spawnPositions = new List<Vector3>();
        int maxAttempts = 10000;
        int attempts = 0;
        while(_consumablePrefabSpawnCount > 0 && attempts < maxAttempts) {
            ConsumablePrefabData consumablePrefabData = GetWeightedRandomConsumablePrefab(_weights);
            
            float x = Random.Range(-(_mapWidth / 2) + k_edgeSafeDistance, (_mapWidth / 2) - k_edgeSafeDistance);
            float z = Random.Range(-(_mapHeight / 2) + k_edgeSafeDistance, (_mapHeight / 2) - k_edgeSafeDistance);
            bool isNewPositonTooCloseTooSpawnedPositions = spawnPositions.Any(existingSpawnedPosition => Vector3.Distance(existingSpawnedPosition, new Vector3(x, 0.5f, z)) < consumablePrefabData.CollisionCheckRadius);
            
            if(isNewPositonTooCloseTooSpawnedPositions) {
                attempts++;
                continue;
            }

            ushort randomIndex = (ushort)Random.Range(0, consumablePrefabData.PrefabContainer.Length);
            spawnPositions.Add(new Vector3(x, 0.5f, z));
            GameObject prefab = Instantiate(consumablePrefabData.PrefabContainer[randomIndex], new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            prefab.transform.parent = _floorObject.transform;
            _consumablePrefabSpawnCount--;
            attempts = 0;
        }
        Debug.Log($"{_consumablePrefabSpawnCount} were left to spawn.");
    }

    private void PopulateDecorations() {
        for(int propCount = 0; propCount <= _decorationPropPrefabSpawnCount; propCount++) {
            float x = Random.Range(-(_mapWidth / 2) + k_edgeSafeDistance, (_mapWidth / 2) - k_edgeSafeDistance);
            float z = Random.Range(-(_mapHeight / 2) + k_edgeSafeDistance, (_mapHeight / 2) - k_edgeSafeDistance);

            ushort randomIndex = (ushort)Random.Range(0, _decorationPropPrefabs.Length);
            GameObject prefab = Instantiate(_decorationPropPrefabs[randomIndex], new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            prefab.transform.parent = _floorObject.transform;;
        }
    }

    private ConsumablePrefabData GetWeightedRandomConsumablePrefab(float[] weights) {
        if (weights.Length == 0) {
            Debug.LogError("Weighted array is empty.");
            return _smallConsumablePrefabData;
        }

        float totalWeight = weights.Sum();
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0.0f;
        for(int index = 0; index < weights.Length; index++) {
            cumulativeWeight += weights[index];
            if (randomValue < cumulativeWeight) {
                return _consumables[index];
            }
        }

        Debug.LogError("Should not reach here, returning a random value...");
        return _consumables[Random.Range(0, _consumables.Length)];
    }
}
