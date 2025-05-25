using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Random = System.Random;

namespace Managers
{
    public class SpawnPointsManager : MonoBehaviour
    {
        private readonly Random _random = new();
        
        [SerializeField] private float enemyRespawnDelay = 3.0f;
        [SerializeField] private float playerRespawnDelay = 3.0f;
        [SerializeField] private int maxEnemyCount = 6;
        
        private int _currentEnemyCount = 0;

        private List<GameObject> _enemySpawnPoints;
        private List<GameObject> _playerSpawnPoints;

        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject playerPrefab;

        private bool _initialized = false;
        private bool _needFirstPlayerSpawn = false;

        private float _blockPositionFor = 3.0f;
        private Dictionary<GameObject, float> _blockedPositions = new ();

        public void Initialize(List<GameObject> enemySpawnPoints, List<GameObject> playerSpawnPoints)
        {
            _enemySpawnPoints = enemySpawnPoints;
            _playerSpawnPoints = playerSpawnPoints;
            
            _initialized = true;
            _needFirstPlayerSpawn = true;
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            if (_needFirstPlayerSpawn)
            {
                SpawnPlayer();
                _needFirstPlayerSpawn = false;
            }
            
            while (_currentEnemyCount < maxEnemyCount)
            {
                _currentEnemyCount++;
                SpawnEnemy();
            }
            
            UpdateBlockedPositions();
        }

        private void UpdateBlockedPositions()
        {
            foreach (var pos in _blockedPositions.Keys.ToList())
            {
                _blockedPositions[pos] -= Time.deltaTime;
    
                if (_blockedPositions[pos] <= 0.0f)
                {
                    _blockedPositions.Remove(pos);
                }
            }
        }

        private void SpawnEnemy()
        {
            StartCoroutine(TimeUtils.WaitAndDo(enemyRespawnDelay, () =>
            {
                int spawnPointIdx = _random.Next(_enemySpawnPoints.Count);
                if (_blockedPositions.ContainsKey(_enemySpawnPoints[spawnPointIdx]))
                {
                    _currentEnemyCount--;  // Revert counter increment on failure
                    return;
                }
                Vector3 position = _enemySpawnPoints[spawnPointIdx].transform.position;
                GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                _blockedPositions.Add(_enemySpawnPoints[spawnPointIdx], _blockPositionFor);
                newEnemy.GetComponent<EnemyTankController>().OnEnemyDeath += () => _currentEnemyCount--;
            }));
        }
        
        private void SpawnPlayer()
        {
            int spawnPointIdx = _random.Next(_playerSpawnPoints.Count);
            Vector3 spawnPosition = _playerSpawnPoints[spawnPointIdx].transform.position;
            GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }

        public void RespawnPlayer(GameObject player)
        {
            StartCoroutine(TimeUtils.WaitAndDo(playerRespawnDelay, () =>
            {
                int spawnPointIdx = _random.Next(_playerSpawnPoints.Count);
                Vector3 spawnPosition = _playerSpawnPoints[spawnPointIdx].transform.position;
                player.transform.position = spawnPosition;
                player.SetActive(true);
            }));
        }
    }
}