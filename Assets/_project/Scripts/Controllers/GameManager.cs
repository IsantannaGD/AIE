using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static Action OnGameStart;
    public static Action<EntityBase, GameSpaceType> OnRegisterEntity;
    public static Action<EntityBase, GameSpaceType> OnRemoveEntity;
    public static Action<Vector2> OnRegisterLimits;
    public static Action OnGamePause;
    public delegate void GameEvents(GameSpaceType spaceType);
    public GameEvents OnFoodEaten;
    public GameEvents OnEnemyDie;
    public GameEvents OnTimeTravelUse;
    public GameEvents OnTimeTravelPick;
    public GameEvents OnGameOver;

    public static GameManager Instance;

    [SerializeField] private List<EntityBase> _allEntitiesInGameSpaceOne = new List<EntityBase>();
    [SerializeField] private List<EntityBase> _allEntitiesInGameSpaceTwo = new List<EntityBase>();
    [SerializeField] private List<Vector2> _allWallPositions = new List<Vector2>();

    [SerializeField] private GameMode _gameMode;

    [SerializeField] private bool _gameOver;
    [SerializeField] private bool _gamePaused;

    public bool GameOver => _gameOver;
    public bool GamePaused => _gamePaused;
    public List<Vector2> AllWallPositions => _allWallPositions;
    public GameMode CurrentGameMode => _gameMode;

    public bool CheckPositionViability(Vector2 pos, GameSpaceType spaceTarget)
    {
        List<EntityBase> listToCheck = spaceTarget == GameSpaceType.PlayerTwoSpace ? _allEntitiesInGameSpaceTwo : _allEntitiesInGameSpaceOne;

        foreach (EntityBase entityBase in listToCheck)
        {
            if (entityBase.CurrentLocation == pos)
            { return false; }
        }

        return true;
    }

    public void SetGameMode(GameMode mode)
    {
        _gameMode = mode;
    }

    private void Awake()
    {
        Instance = this;
        Initializations();
    }

    private void Initializations()
    {
        OnRegisterEntity += RegisterEntityCallback;
        OnRemoveEntity += RemoveEntityFromListCallback;
        OnRegisterLimits += RegisterWall;

        SceneManager.LoadScene("MainMenu");
    }

    private void RegisterEntityCallback(EntityBase entity, GameSpaceType space)
    {
        switch(space)
        {
            case GameSpaceType.SinglePlayerSpace:
            case GameSpaceType.PlayerOneSpace:
                _allEntitiesInGameSpaceOne.Add(entity);
                break;
            case GameSpaceType.PlayerTwoSpace:
                _allEntitiesInGameSpaceTwo.Add(entity);
                break;
        }
    }

    private void RemoveEntityFromListCallback(EntityBase entity, GameSpaceType space)
    {
        switch(space)
        {
            case GameSpaceType.SinglePlayerSpace:
            case GameSpaceType.PlayerOneSpace:
                _allEntitiesInGameSpaceOne.Remove(entity);
                break;
            case GameSpaceType.PlayerTwoSpace:
                _allEntitiesInGameSpaceTwo.Remove(entity);
                break;
        }
    }

    private void RegisterWall(Vector2 pos)
    {
        _allWallPositions.Add(pos);
    }
}
