using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GamePlaceController : MonoBehaviour
{
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private List<EntityBase> _allFoodsPrefab = new List<EntityBase>();
    [SerializeField] private TimeTravelPowerUp _timeTravelPrefab;

    [SerializeField] private TimeTravelPowerUp _timeTravelSpawned;

    [SerializeField] private List<GameSetStatus> _gameSetEnables = new List<GameSetStatus>();
    [SerializeField] private int _gameSetCount = 0;

    [SerializeField] private float _cellSize = 0.3f;
    [SerializeField] private float _powerUpRespawnCooldown = 60f;
    [SerializeField] private WaitForSeconds _powerUpWaitFor;

    private void Start()
    {
        GameManager.OnRegisterNewPlayer += RegisterNewPlayers;
        GameManager.OnResetPlayerList += ResetPlayerListCallback;
        GameManager.Instance.OnFoodEaten += SpawnFood;
        GameManager.Instance.OnEnemyDie += SpawnEnemy;
        GameManager.OnGameStart += GameStartCallback;
    }

    private void RegisterNewPlayers(char l, char r)
    {
        if(CheckInputConflict(l, r))
        {return;}

        _gameSetCount++;
        GameSetStatus newSet = new GameSetStatus(_gameSetCount, l, r);
        _gameSetEnables.Add(newSet);
    }

    private void GameStartCallback()
    {
        if (_gameSetCount == 0)
        {
            RegisterNewPlayers('a', 'd');
        }

        for (int i = 0; i < _gameSetCount; i++)
        {
            GameSetStatus newSet = _gameSetEnables[i];
            SpawnPlayer(newSet.GameSetID);

            int random = Random.Range(0, _allFoodsPrefab.Count);
            var food = Instantiate(_allFoodsPrefab[random], GetRandomPosition(), quaternion.identity) ;
            food.OnEntitySpawn(newSet.GameSetID);

            newSet.Food = food;
            SpawnEnemy(newSet.GameSetID);
        }

        StartCoroutine(PowerUpRespawnRoutine());
    }

    private void SpawnPlayer(int setId)
    {
        Player player = Instantiate(_playerPrefab, GetRandomPosition(), Quaternion.identity);
        player.OnEntitySpawn(setId);

        GameSetStatus current = _gameSetEnables.Find((x) => x.GameSetID == setId);
        current.Player = player;
        current.Player.SetInput(current.LeftInput, current.RightInput);
    }

    private void SpawnEnemy(int setId)
    {
        EnemyController enemy = Instantiate(_enemyPrefab, GetRandomPosition(), Quaternion.identity);
        enemy.OnEntitySpawn(setId);

        GameSetStatus current = _gameSetEnables.Find((x) => x.GameSetID == setId);
        current.EnemyController = enemy;
        current.EnemyController.SetTargetPosition(current.Food.CurrentLocation);
    }

    private void SpawnFood(int setId)
    {
        int random = Random.Range(0, _allFoodsPrefab.Count);
        var food = Instantiate(_allFoodsPrefab[random], GetRandomPosition(), quaternion.identity) ;
        food.OnEntitySpawn(setId);

        GameSetStatus current = _gameSetEnables.Find((x) => x.GameSetID == setId);
        current.Food = food;

        current.EnemyController.SetTargetPosition(current.Food.CurrentLocation);
    }

    private IEnumerator PowerUpRespawnRoutine()
    {
        _powerUpWaitFor = new WaitForSeconds(_powerUpRespawnCooldown);

        while (!GameManager.Instance.GameOver)
        {
            yield return _powerUpWaitFor;

            if (!GameManager.Instance.GamePaused)
            {
                if (_timeTravelSpawned != null)
                {
                    Destroy(_timeTravelSpawned.gameObject);
                    _timeTravelSpawned = null;
                }

                TimeTravelPowerUp powerUp = Instantiate(_timeTravelPrefab, GetRandomPosition(), Quaternion.identity);
                powerUp.OnEntitySpawn(0);
                _timeTravelSpawned = powerUp;
            }
        }

        yield return new WaitForEndOfFrame();
    }

    private Vector2 GetRandomPosition()
    {
        int minRange = -30;
        int maxRange = 31;

        float x = Random.Range(minRange, maxRange) * _cellSize;
        float y = Random.Range(-11, 11) * _cellSize;

        Vector2 randomPos = new Vector2(x, y);

        while (!GameManager.Instance.CheckPositionViability(randomPos))
        {
            x = Random.Range(minRange, maxRange) * _cellSize;
            y = Random.Range(-11, 11) * _cellSize;

            randomPos = new Vector2(x, y);
        }

        return randomPos;
    }

    private bool CheckInputConflict(char l, char r)
    {
        foreach (GameSetStatus setEnable in _gameSetEnables)
        {
            if (l.Equals(setEnable.LeftInput) || l.Equals(setEnable.RightInput) || r.Equals(setEnable.LeftInput) || r.Equals(setEnable.RightInput))
            { return true; }
        }

        return false;
    }

    private void ResetPlayerListCallback()
    {
        foreach (GameSetStatus setEnable in _gameSetEnables)
        {
            _gameSetEnables.Remove(setEnable);
        }

        _gameSetCount = 0;
    }

    private void OnDestroy()
    {
        GameManager.OnRegisterNewPlayer -= RegisterNewPlayers;
        GameManager.OnResetPlayerList -= ResetPlayerListCallback;
        GameManager.Instance.OnFoodEaten -= SpawnFood;
        GameManager.Instance.OnEnemyDie -= SpawnEnemy;
        GameManager.OnGameStart -= GameStartCallback;
    }
}
