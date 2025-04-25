using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPopulator : MonoBehaviour {
    private struct ConsumablePrefabData {
        public GameObject[] PrefabContainer;
        public float CollisionCheckRadius;
    }

    [SerializeField] private GameObject _edgePrefab;
    [SerializeField] private GameObject[] _smallConsumablePrefabs;
    [SerializeField] private GameObject[] _mediumConsumablePrefabs;
    [SerializeField] private GameObject[] _largeConsumablePrefabs;
    [SerializeField] private float _prefabSpacing = 5.0f;
    [SerializeField] private ushort _consumablePrefabSpawnCount = 1000;
    [SerializeField] private float k_smallConsumableSpawnWeight = 0.5f;
    [SerializeField] private float k_mediumConsumableSpawnWeight = 0.5f;
    [SerializeField] private float k_largeConsumableSpawnWeight = 0.25f;
    private ConsumablePrefabData _smallConsumablePrefabData;
    private ConsumablePrefabData _mediumConsumablePrefabData;
    private ConsumablePrefabData _largeConsumablePrefabData;
    private const float k_smallConsumableSpawnCollisionCheckRadius = 0.5f;
    private const float k_mediumConsumableSpawnCollisionCheckRadius = 2.0f;
    private const float k_largeConsumableSpawnCollisionCheckRadius = 7.0f;
    private const float k_edgeSafeDistance = 7.0f;
    private GameObject _floorObject;
    private Dictionary<float, ConsumablePrefabData> _weightConsumablePrefabDataDictionary = new Dictionary<float, ConsumablePrefabData>();

    private float _mapWidth;
    private float _mapHeight;
    private void Start() {
        _floorObject = GameObject.FindGameObjectWithTag("Floor");
        _mapWidth = _floorObject.GetComponent<Collider>().bounds.size.x;
        _mapHeight = _floorObject.GetComponent<Collider>().bounds.size.z;
        _smallConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _smallConsumablePrefabs,
            CollisionCheckRadius = k_smallConsumableSpawnCollisionCheckRadius
        };
        _mediumConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _mediumConsumablePrefabs,
            CollisionCheckRadius = k_mediumConsumableSpawnCollisionCheckRadius
        };
        _largeConsumablePrefabData = new ConsumablePrefabData {
            PrefabContainer = _largeConsumablePrefabs,
            CollisionCheckRadius = k_largeConsumableSpawnCollisionCheckRadius
        };
        _weightConsumablePrefabDataDictionary.Add(k_smallConsumableSpawnWeight, _smallConsumablePrefabData);
        _weightConsumablePrefabDataDictionary.Add(k_mediumConsumableSpawnWeight, _mediumConsumablePrefabData);
        _weightConsumablePrefabDataDictionary.Add(k_largeConsumableSpawnWeight, _largeConsumablePrefabData);
        EdgePopulator();
        ConsumableObjectPopulator();
    }

    private void EdgePopulator() {
        for(float x = -_mapWidth / 2; x < _mapWidth / 2; x += _prefabSpacing) {
            Instantiate(_edgePrefab, new Vector3(x, 0.1f, (- _mapHeight / 2) + _prefabSpacing), Quaternion.identity).transform.parent = _floorObject.transform;
            Instantiate(_edgePrefab, new Vector3(x, 0.1f, (_mapHeight / 2) - _prefabSpacing), Quaternion.identity).transform.parent = _floorObject.transform;
        }
        for(float z = -_mapHeight / 2; z < _mapHeight / 2; z += _prefabSpacing) {
            Instantiate(_edgePrefab, new Vector3((- _mapWidth / 2) + _prefabSpacing, 0.1f, z), Quaternion.identity).transform.parent = _floorObject.transform;
            Instantiate(_edgePrefab, new Vector3((_mapWidth / 2) - _prefabSpacing, 0.1f, z), Quaternion.identity).transform.parent = _floorObject.transform;
        }
    }

    private void ConsumableObjectPopulator() {
        List<Vector3> spawnPositions = new List<Vector3>();
        int maxAttempts = 10000;
        int attempts = 0;
        while(_consumablePrefabSpawnCount > 0 && attempts < maxAttempts) {
            ConsumablePrefabData consumablePrefabData = GetRandomPrefabDataFromWeightedDictionary(_weightConsumablePrefabDataDictionary);
            
            float x = Random.Range(-(_mapWidth / 2) + k_edgeSafeDistance, (_mapWidth / 2) - k_edgeSafeDistance);
            float z = Random.Range(-(_mapHeight / 2) + k_edgeSafeDistance, (_mapHeight / 2) - k_edgeSafeDistance);
            bool isNewPositonTooCloseTooSpawnedPositions = spawnPositions.Any(existingSpawnedPosition => Vector3.Distance(existingSpawnedPosition, new Vector3(x, 0.5f, z)) < consumablePrefabData.CollisionCheckRadius);
            
            if(isNewPositonTooCloseTooSpawnedPositions) {
                attempts++;
                continue;
            }

            ushort randomIndex = (ushort)Random.Range(0, consumablePrefabData.PrefabContainer.Length);
            spawnPositions.Add(new Vector3(x, 0.5f, z));
            Instantiate(consumablePrefabData.PrefabContainer[randomIndex], new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(0f, 360f), 0)).transform.parent = _floorObject.transform;
            _consumablePrefabSpawnCount--;
            attempts = 0;
        }
        Debug.Log($"{_consumablePrefabSpawnCount} were left to spawn.");
    }

    private ConsumablePrefabData GetRandomPrefabDataFromWeightedDictionary(Dictionary<float, ConsumablePrefabData> weightedDictionary) {
        if (weightedDictionary == null || weightedDictionary.Count == 0) {
            Debug.LogError("Weighted dictionary is empty or null.");
            return _smallConsumablePrefabData;
        }

        float totalWeight = weightedDictionary.Sum(pair => pair.Key);
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        foreach (var pair in weightedDictionary) {
            cumulativeWeight += pair.Key;
            if (randomValue < cumulativeWeight) {
                return pair.Value;
            }
        }

        Debug.LogError("Should not reach here, returning a random value...");
        return weightedDictionary.ElementAt(Random.Range(0, weightedDictionary.Count)).Value;
    }
}
