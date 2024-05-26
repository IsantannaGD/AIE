using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : EntityBase, ICharacter
{
    private Vector2 _direction = Vector2.up;

    [SerializeField] private Vector2 _snakeIndex;
    [SerializeField] private GameSpaceType _placedSpace;

    [SerializeField] private Vector2 _targetPosition;
    [SerializeField] private BodyPartBehavior _bodyPrefab;
    [SerializeField] private List<BodyPartBehavior> _fullBody = new List<BodyPartBehavior>();
    
    [SerializeField] private float _cellSize = 0.3f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _cargoLoadMultiplier;
    [SerializeField] private float _externalMultiplier;

    [SerializeField] private DirectionType _directionIndex = DirectionType.Up;
    [SerializeField] private float _moveTime = 0f;
    [SerializeField] private bool _isRotating = false;
    [SerializeField] private bool _redefiningRotation;
    [SerializeField] private bool _rightPathBlocked, _leftPathBlocked, _upperPathBlocked, _bottomPathBlocked = false;

    public void OnSpawnEnemy(GameSpaceType spacePlaced, Vector2 initialTarget)
    {
        _placedSpace = spacePlaced;
        _targetPosition = initialTarget;
    }
    public void SetTargetPosition(Vector2 target)
    {
        _targetPosition = target;
    }

    public void BodyGrow()
    {
        Vector2 pos = transform.position;
        if (_fullBody.Count != 0)
        { pos = _fullBody[^1].transform.position; }

        BodyPartBehavior bodyPart = Instantiate(_bodyPrefab, pos, Quaternion.identity);
        bodyPart.OnSpawnBodyPart();
        _fullBody.Add(bodyPart);
    }

    public void ReceiveDamage()
    {
        throw new System.NotImplementedException();
    }

    public void PickPowerUp()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IInteractableObject obj))
        {
            obj.InteractionCallback(this);
            return;
        }

        if (other.gameObject.TryGetComponent(out IDangerousEncounter enemyTouch))
        {
            enemyTouch.DangerousInteractionCallback(this);
        }
    }

    private void SelectRotation(DirectionType directionTarget)
    {
        if (!_redefiningRotation)
        {
            if (_directionIndex == directionTarget || _isRotating)
            { return; }
        }

        switch (directionTarget)
        {
            case DirectionType.Up:
                if (_directionIndex == DirectionType.Left)
                { RotateToRightCallback(); }
                else if (_directionIndex == DirectionType.Right)
                { RotateToLeftCallback(); }
                else
                {
                    if (!_rightPathBlocked)
                    { RotateToRightCallback(); }
                    else
                    { RotateToLeftCallback(); }
                }
                break;
            case DirectionType.Right:
            case DirectionType.Down:
                if ((int)_directionIndex + 1 == (int)directionTarget)
                {RotateToRightCallback();}
                else if ((int)_directionIndex - 1 == (int)directionTarget)
                { RotateToLeftCallback(); }
                else
                {
                    if (!_rightPathBlocked)
                    { RotateToRightCallback(); }
                    else
                    { RotateToLeftCallback(); }
                }

                break;
            case DirectionType.Left:
                if(_directionIndex == DirectionType.Up)
                {RotateToLeftCallback();}
                else if(_directionIndex == DirectionType.Down)
                {RotateToRightCallback();}
                else
                {
                    if (!_rightPathBlocked)
                    { RotateToRightCallback(); }
                    else
                    { RotateToLeftCallback(); }
                }
                break;
        }
    }

    private void RotateToRightCallback()
    {
        switch (_directionIndex)
        {
            case DirectionType.Up:
                _directionIndex++;
                _direction = Vector2.right;
                break;
            case DirectionType.Right:
                _directionIndex++;
                _direction = Vector2.down;
                break;
            case DirectionType.Down:
                _directionIndex++;
                _direction = Vector2.left;
                break;
            case DirectionType.Left:
                _directionIndex = DirectionType.Up;
                _direction = Vector2.up;
                break;
        }

        _isRotating = true;
    }

    private void RotateToLeftCallback()
    {
        switch (_directionIndex)
        {
            case DirectionType.Up:
                _directionIndex = DirectionType.Left;
                _direction = Vector2.left;
                break;
            case DirectionType.Right:
                _directionIndex--;
                _direction = Vector2.up;
                break;
            case DirectionType.Down:
                _directionIndex--;
                _direction = Vector2.right;
                break;
            case DirectionType.Left:
                _directionIndex--;
                _direction = Vector2.down;
                break;
        }
        
        _isRotating = true;
    }

    private void Move()
    {
        if (Time.time > _moveTime)
        {
            Vector2 lastDirection = _direction;
            DirectionType lastDirectionIndex = _directionIndex;

            DirectionCheck();

            while (!MovePossibility(transform.position + (Vector3)_direction * _cellSize))
            {
                if (MovePossibility(transform.position + (Vector3)lastDirection * _cellSize))
                {
                    _direction = lastDirection;
                    _directionIndex = lastDirectionIndex;
                }
                else
                {
                    switch (_directionIndex)
                    {
                        case DirectionType.Up:
                            _upperPathBlocked = true;
                            break;
                        case DirectionType.Right:
                            _rightPathBlocked = true;
                            break;
                        case DirectionType.Down:
                            _bottomPathBlocked = true;
                            break;
                        case DirectionType.Left:
                            _leftPathBlocked = true;
                            break;
                    }

                    _redefiningRotation = true;

                    SelectRotation(_directionIndex);
                }
            }

            for (int i = _fullBody.Count - 1; i > 0; i--)
            { _fullBody[i].transform.position = _fullBody[i - 1].transform.position; }

            if (_fullBody.Count > 0)
            { _fullBody[0].transform.position = transform.position;}

            float realVelocity = _speed - (_cargoLoadMultiplier * _fullBody.Count) + _externalMultiplier;
            realVelocity = realVelocity < 2 ? 2f : realVelocity;

            transform.position += (Vector3)_direction * _cellSize;
            _moveTime = Time.time + 1 / realVelocity;
            _snakeIndex = transform.position / _cellSize;
            _isRotating = false;

            PathRedefined();
        }
    }

    private void DirectionCheck()
    {
        Vector3 pos = transform.position;

        if (Math.Round(_targetPosition.x, 3) > Math.Round(pos.x, 3))
        {
            if (_direction != Vector2.right)
            {
                SelectRotation(DirectionType.Right);
            }
        }
        else if (Math.Round(_targetPosition.x, 3) < Math.Round(pos.x, 3))
        {
            if (_direction != Vector2.left)
            {
                SelectRotation(DirectionType.Left);
            }
        }
        else if (Math.Round(_targetPosition.y, 3) > Math.Round(pos.y, 3))
        {
            if (_direction != Vector2.up)
            {
                SelectRotation(DirectionType.Up);
            }
        }
        else if (Math.Round(_targetPosition.y, 3) < Math.Round(pos.y, 3))
        {
            if (_direction != Vector2.down)
            {
                SelectRotation(DirectionType.Down);
            }
        }
    }

    private bool MovePossibility(Vector2 futurePos)
    {
        foreach (Vector2 wallPosition in GameManager.Instance.AllWallPositions)
        {
            if (wallPosition.EqualsRounded(futurePos))
            { return false; }
        }

        foreach (BodyPartBehavior bodyPartBehavior in _fullBody)
        {
            if (bodyPartBehavior.CurrentLocation.EqualsRounded(futurePos))
            { return false; }
        }

        return true;
    }

    private void PathRedefined()
    {
        _upperPathBlocked = false;
        _rightPathBlocked = false;
        _bottomPathBlocked = false;
        _leftPathBlocked = false;
        _redefiningRotation = false;
    }
}
