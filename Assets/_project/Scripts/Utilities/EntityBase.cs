using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EntityBase : MonoBehaviour
{
    [SerializeField] protected EntityType _entityType;
    [SerializeField] protected Vector2 _savedLocation;

    [SerializeField] protected int _gameSetID;

    public int GameSetID => _gameSetID;
    public EntityType EntityType => _entityType;
    public Vector2 CurrentLocation => transform.position;

    public abstract void OnEntitySpawn(int gameSet);

    protected void Start()
    {
        BaseInitializations();
        Initializations();
    }

    protected virtual void Initializations()
    { }

    protected virtual void OnSaveStateCallback()
    {
        _savedLocation = transform.position;
    }

    private void BaseInitializations()
    {
        GameManager.OnTimeTravelPick += OnSaveStateCallback;
    }

    private void OnDisable()
    {
        GameManager.OnRemoveEntity?.Invoke(this);
    }

    private void OnDestroy()
    {
        GameManager.OnTimeTravelPick -= OnSaveStateCallback;
    }
}
