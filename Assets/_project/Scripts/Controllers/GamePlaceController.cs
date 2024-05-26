using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamePlaceController : MonoBehaviour
{
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private FoodBehavior _foodPrefab;

    [SerializeField] private List<EntityBase> _allTypesPowerUp = new List<EntityBase>();

    [SerializeField] private WallAssemblyFunc _divisorWall;

    [SerializeField] private FoodBehavior _foodOne;
    [SerializeField] private FoodBehavior _foodTwo;

    [SerializeField] private EnemyController _enemyOneController;
    [SerializeField] private EnemyController _enemyTwoController;

    [SerializeField] private float _cellSize = 0.3f;
    [SerializeField] private float _powerUpRespawnCooldown = 60f;
    [SerializeField] private WaitForSeconds _powerUpWaitFor;

    private void Start()
    {
        GameManager.Instance.OnFoodEaten += SpawnFood;
        GameManager.Instance.OnEnemyDie += SpawnEnemy;
        GameManager.OnGameStart += GameStartCallback;

        GameStartCallback();
    }

    private void GameStartCallback()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.SinglePlayer:
                SpawnPlayer(GameSpaceType.SinglePlayerSpace);

                FoodBehavior foodSingle = Instantiate(_foodPrefab, GetRandomPosition(GameSpaceType.SinglePlayerSpace), Quaternion.identity);
                foodSingle.OnEntitySpawn(GameSpaceType.SinglePlayerSpace);
                _foodOne = foodSingle;

                SpawnEnemy(GameSpaceType.SinglePlayerSpace);
                break;
            case GameMode.LocalMultiPlayer:

                _divisorWall.gameObject.SetActive(true);

                SpawnPlayer(GameSpaceType.PlayerOneSpace);

                FoodBehavior foodOne = Instantiate(_foodPrefab, GetRandomPosition(GameSpaceType.PlayerOneSpace), Quaternion.identity);
                foodOne.OnEntitySpawn(GameSpaceType.PlayerOneSpace);
                _foodOne = foodOne;

                SpawnEnemy(GameSpaceType.PlayerOneSpace);

                SpawnPlayer(GameSpaceType.PlayerTwoSpace);

                FoodBehavior foodTwo = Instantiate(_foodPrefab, GetRandomPosition(GameSpaceType.PlayerTwoSpace), Quaternion.identity);
                foodTwo.OnEntitySpawn(GameSpaceType.PlayerTwoSpace);
                _foodTwo = foodTwo;

                SpawnEnemy(GameSpaceType.PlayerTwoSpace);
                break;
        }

        StartCoroutine(PowerUpRespawnRoutine());
    }

    private void SpawnPlayer(GameSpaceType whereSpawn)
    {
        Player player = Instantiate(_playerPrefab, GetRandomPosition(whereSpawn), Quaternion.identity);
        player.OnEntitySpawn(whereSpawn);
    }

    private void SpawnEnemy(GameSpaceType whereSpawn)
    {
        FoodBehavior food = whereSpawn == GameSpaceType.PlayerTwoSpace ? _foodTwo : _foodOne;
        EnemyController enemy = Instantiate(_enemyPrefab, GetRandomPosition(whereSpawn), Quaternion.identity);
        enemy.OnEntitySpawn(whereSpawn);
        enemy.SetTargetPosition(food.CurrentLocation);

        if (whereSpawn == GameSpaceType.PlayerTwoSpace)
        { _enemyTwoController = enemy;}
        else
        { _enemyOneController = enemy; }
    }

    private void SpawnFood(GameSpaceType localSpace)
    {
        FoodBehavior food = Instantiate(_foodPrefab, GetRandomPosition(localSpace), Quaternion.identity);
        food.OnEntitySpawn(localSpace);

        if (localSpace == GameSpaceType.PlayerTwoSpace)
        { _foodTwo = food; }
        else
        { _foodOne = food; }

        EnemyController enemy = localSpace == GameSpaceType.PlayerTwoSpace ? _enemyTwoController : _enemyOneController;
        enemy.SetTargetPosition(food.CurrentLocation);
    }

    private IEnumerator PowerUpRespawnRoutine()
    {
        _powerUpWaitFor = new WaitForSeconds(_powerUpRespawnCooldown);

        while (!GameManager.Instance.GameOver)
        {
            yield return _powerUpWaitFor;

            if (!GameManager.Instance.GamePaused)
            {
                int random = 0;

                switch (GameManager.Instance.CurrentGameMode)
                {
                    case GameMode.SinglePlayer:
                        random = Random.Range(0, _allTypesPowerUp.Count);
                        var power = Instantiate(_allTypesPowerUp[random], GetRandomPosition(GameSpaceType.SinglePlayerSpace), quaternion.identity) ;
                        power.OnEntitySpawn(GameSpaceType.SinglePlayerSpace);
                        break;
                    case GameMode.LocalMultiPlayer:
                        random = Random.Range(0, _allTypesPowerUp.Count);
                        var power1 = Instantiate(_allTypesPowerUp[random], GetRandomPosition(GameSpaceType.PlayerOneSpace), quaternion.identity) ;
                        power1.OnEntitySpawn(GameSpaceType.PlayerOneSpace);

                        random = Random.Range(0, _allTypesPowerUp.Count);
                        var power2 = Instantiate(_allTypesPowerUp[random], GetRandomPosition(GameSpaceType.PlayerTwoSpace), quaternion.identity) ;
                        power2.OnEntitySpawn(GameSpaceType.PlayerTwoSpace);
                        break;
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }

    private Vector2 GetRandomPosition(GameSpaceType whereSpawn)
    {
        int minRange = 0;
        int maxRange = 0;

        switch(whereSpawn)
        {
            case GameSpaceType.SinglePlayerSpace:
                minRange = -30;
                maxRange = 31;

                break;
            case GameSpaceType.PlayerOneSpace:
                minRange = -30;
                maxRange = 0;

                break;
            case GameSpaceType.PlayerTwoSpace:
                minRange = 1;
                maxRange = 31;

                break;
        }

        float x = Random.Range(minRange, maxRange) * _cellSize;
        float y = Random.Range(-11, 11) * _cellSize;

        Vector2 randomPos = new Vector2(x, y);

        while (!GameManager.Instance.CheckPositionViability(randomPos, whereSpawn))
        {
            x = Random.Range(minRange, maxRange) * _cellSize;
            y = Random.Range(-11, 11) * _cellSize;

            randomPos = new Vector2(x, y);
        }

        return randomPos;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnFoodEaten -= SpawnFood;
        GameManager.Instance.OnEnemyDie -= SpawnEnemy;
        GameManager.OnGameStart -= GameStartCallback;
    }
}
