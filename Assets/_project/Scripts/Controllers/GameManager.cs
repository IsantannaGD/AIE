using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static Action<EntityBase> OnRegisterEntity;
    public static Action<EntityBase> OnRemoveEntity;
    public static Action<Vector2> OnRegisterLimits;
    public static Action OnGameStart;
    public static Action OnTimeTravelUse;
    public static Action OnTimeTravelPick;
    public static Action OnGamePause;
    public delegate void GameEvents(int setId);
    public GameEvents OnFoodEaten;
    public GameEvents OnEnemyDie;
    public GameEvents OnGameOver;

    public static GameManager Instance;

    [SerializeField] private List<EntityBase> _allEntitiesInGame = new List<EntityBase>();
    [SerializeField] private List<Vector2> _allWallPositions = new List<Vector2>();

    [FormerlySerializedAs("_gameMode")] [SerializeField] private GameModeType gameModeType;

    [SerializeField] private bool _gameOver;
    [SerializeField] private bool _gamePaused;

    public bool GameOver => _gameOver;
    public bool GamePaused => _gamePaused;
    public List<Vector2> AllWallPositions => _allWallPositions;
    public GameModeType CurrentGameModeType => gameModeType;

    public bool CheckPositionViability(Vector2 pos)
    {
        foreach (EntityBase entityBase in _allEntitiesInGame)
        {
            if (entityBase.CurrentLocation == pos)
            { return false; }
        }

        return true;
    }

    public void SetGameMode(GameModeType modeType)
    {
        gameModeType = modeType;
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

        //SceneManager.LoadScene("MainMenu");
    }

    private void RegisterEntityCallback(EntityBase entity)
    {
        _allEntitiesInGame.Add(entity);
    }

    private void RemoveEntityFromListCallback(EntityBase entity)
    {
        _allEntitiesInGame.Remove(entity);
    }

    private void RegisterWall(Vector2 pos)
    {
        _allWallPositions.Add(pos);
    }
}
