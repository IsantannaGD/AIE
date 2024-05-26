using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action OnGameStart;
    public static Action<EntityBase> OnRegisterEntity;
    public static Action<EntityBase> OnRemoveEntity;
    public static Action<Vector2> OnRegisterLimits;
    public static Action<GameSpaceType> OnFoodEaten;
    public static Action OnSaveState;
    public static Action OnGameOver;

    public static GameManager Instance;

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private List<EntityBase> _allEntitiesInGame = new List<EntityBase>();
    [SerializeField] private List<Vector2> _allWallPositions = new List<Vector2>();

    [SerializeField] private GameMode _gameMode;

    public List<Vector2> AllWallPositions => _allWallPositions;

    public GameMode CurrentGameMode => _gameMode;

    public bool CheckPositionViability(Vector2 pos)
    {
        foreach (EntityBase entityBase in _allEntitiesInGame)
        {
            if (entityBase.CurrentLocation == pos)
            { return false; }
        }

        return true;
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
        OnGameOver += Test;
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

    private void Test()
    {
        _gameOverPanel.SetActive(true);
    }

}
