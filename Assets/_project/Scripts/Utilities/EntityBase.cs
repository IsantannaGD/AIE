using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EntityBase : MonoBehaviour
{
    [SerializeField] protected EntityType _entityType;
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
        TimeTravelBeacon beacon = new TimeTravelBeacon();
        beacon.Position = transform.position;
        beacon.Type = _entityType;
        beacon.SetID = _gameSetID;

        GameManager.OnAddBeaconInRecorder?.Invoke(beacon);
    }

    private void BaseInitializations()
    {
        GameManager.OnCreateBeaconsRequest += OnSaveStateCallback;
    }

    private void OnDisable()
    {
        GameManager.OnRemoveEntity?.Invoke(this);
    }

    private void OnDestroy()
    {
        GameManager.OnCreateBeaconsRequest -= OnSaveStateCallback;
    }
}
