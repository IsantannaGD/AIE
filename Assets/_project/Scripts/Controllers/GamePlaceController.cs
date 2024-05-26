using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamePlaceController : MonoBehaviour
{
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private FoodBehavior _foodPrefab;

    [SerializeField] private WallAssemblyFunc _topWall;
    [SerializeField] private WallAssemblyFunc _rightWall;
    [SerializeField] private WallAssemblyFunc _botWall;
    [SerializeField] private WallAssemblyFunc _leftWall;
    [SerializeField] private WallAssemblyFunc _divisorAll;

    [SerializeField] private FoodBehavior _foodOne;
    [SerializeField] private FoodBehavior _foodTwo;

    [SerializeField] private EnemyController _enemyOneController;
    [SerializeField] private EnemyController _enemyTwoController;

    [SerializeField] private float _cellSize = 0.3f;

    private void Start()
    {
        GameManager.OnFoodEaten += SpawnFood;

        GameStartCallback();
    }

    private void GameStartCallback()
    {
        GameSpaceType whereSpawn = GameSpaceType.SinglePlayerSpace;

        SpawnPlayer(GameSpaceType.SinglePlayerSpace);

        FoodBehavior food = Instantiate(_foodPrefab, GetRandomPosition(whereSpawn), Quaternion.identity);
        food.OnSpawn(whereSpawn);

        if (whereSpawn == GameSpaceType.PlayerTwoSpace)
        { _foodTwo = food; }
        else
        { _foodOne = food; }

        EnemyController enemy = Instantiate(_enemyPrefab, GetRandomPosition(whereSpawn), Quaternion.identity);
        enemy.OnSpawnEnemy(whereSpawn, food.CurrentLocation);

        if (whereSpawn == GameSpaceType.PlayerTwoSpace)
        { _enemyTwoController = enemy;}
        else
        { _enemyOneController = enemy; }
    }

    private void SpawnPlayer(GameSpaceType whereSpawn)
    {
        Player player = Instantiate(_playerPrefab, GetRandomPosition(whereSpawn), Quaternion.identity);
    }

    private void SpawnEnemy(GameSpaceType whereSpawn)
    {
        FoodBehavior food = whereSpawn == GameSpaceType.PlayerTwoSpace ? _foodTwo : _foodOne;
        EnemyController enemy = Instantiate(_enemyPrefab, GetRandomPosition(whereSpawn), Quaternion.identity);
        enemy.OnSpawnEnemy(whereSpawn, food.CurrentLocation);

        if (whereSpawn == GameSpaceType.PlayerTwoSpace)
        { _enemyTwoController = enemy;}
        else
        { _enemyOneController = enemy; }
    }

    private void SpawnFood(GameSpaceType localSpace)
    {
        FoodBehavior food = Instantiate(_foodPrefab, GetRandomPosition(localSpace), Quaternion.identity);
        food.OnSpawn(localSpace);

        if (localSpace == GameSpaceType.PlayerTwoSpace)
        { _foodTwo = food; }
        else
        { _foodOne = food; }

        EnemyController enemy = localSpace == GameSpaceType.PlayerTwoSpace ? _enemyTwoController : _enemyOneController;
        enemy.SetTargetPosition(food.CurrentLocation);
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
                maxRange = 30;

                break;
        }

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

    private void OnDestroy()
    {
        GameManager.OnFoodEaten -= SpawnFood;
    }
}
