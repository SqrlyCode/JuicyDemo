using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool _isSpawning;
    [SerializeField] private float _spawnInterval = 2f;
    
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPositions;

    private float _timer;

    private void Update()
    {
        if (_isSpawning)
        {
            _timer += Time.deltaTime;
            if (_timer >= _spawnInterval)
            {
                _timer = 0;
                int rndSpawnId = Random.Range(0, _spawnPositions.Length);
                Vector3 spawnPos = _spawnPositions[rndSpawnId].position;
                Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}