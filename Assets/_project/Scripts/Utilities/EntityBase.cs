using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EntityBase : MonoBehaviour
{
    [SerializeField] protected EntityType _entityType;
    [SerializeField] protected Vector2 _savedLocation;

    public EntityType EntityType => _entityType;
    public Vector2 CurrentLocation => transform.position;

    protected void Start()
    {
        BaseInitializations();
        Initializations();
    }

    protected virtual void Initializations()
    {

    }

    protected virtual void OnSaveStateCallback()
    {
        _savedLocation = transform.position;
    }

    private void BaseInitializations()
    {
        GameManager.OnSaveState += OnSaveStateCallback;
    }

    private void OnEnable()
    {
        GameManager.OnRegisterEntity?.Invoke(this);
    }

    private void OnDisable()
    {
        GameManager.OnRemoveEntity?.Invoke(this);
    }

    private void OnDestroy()
    {
        GameManager.OnSaveState -= OnSaveStateCallback;
    }
}
