using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EntityBase : MonoBehaviour
{
    [SerializeField] protected GameSpaceType _spaceSpawned;
    [SerializeField] protected EntityType _entityType;
    [SerializeField] protected Vector2 _savedLocation;

    public EntityType EntityType => _entityType;
    public Vector2 CurrentLocation => transform.position;

    public abstract void OnEntitySpawn(GameSpaceType spaceSpawn);

    protected void Start()
    {
        BaseInitializations();
        Initializations();
    }

    protected virtual void Initializations()
    { }

    protected virtual void OnSaveStateCallback(GameSpaceType spaceTarget)
    {
        if (spaceTarget != _spaceSpawned)
        { return;}

        _savedLocation = transform.position;
    }

    private void BaseInitializations()
    {
        GameManager.Instance.OnTimeTravelPick += OnSaveStateCallback;
    }

    private void OnDisable()
    {
        GameManager.OnRemoveEntity?.Invoke(this, _spaceSpawned);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTimeTravelPick -= OnSaveStateCallback;
    }
}
