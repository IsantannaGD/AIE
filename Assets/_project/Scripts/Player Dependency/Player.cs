using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : EntityBase, ICharacter
{
     private Vector2 _direction = Vector2.up;

    [SerializeField] private Vector2 _snakeIndex;

    [SerializeField] private BodyPartBehavior _bodyPrefab;
    [SerializeField] private List<BodyPartBehavior> _playerBody = new List<BodyPartBehavior>();

    [SerializeField] private float _cellSize = 0.3f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _cargoLoadMultiplier;
    [SerializeField] private float _externalMultiplier;

    [SerializeField] private DirectionType _directionIndex = DirectionType.Up;
    [SerializeField] private float _moveTime = 0f;
    [SerializeField] private bool isRotating = false;

    [SerializeField] private KeyCode _leftKey = KeyCode.A;
    [SerializeField] private KeyCode _rightKey = KeyCode.D;

    public void BodyGrow()
    {
        Vector2 pos = transform.position;
        if (_playerBody.Count != 0)
        { pos = _playerBody[^1].transform.position; }

        BodyPartBehavior bodyPart = Instantiate(_bodyPrefab, pos, Quaternion.identity);
        bodyPart.OnSpawnBodyPart();
        _playerBody.Add(bodyPart);
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
        ChangeDirection();
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

    private void ChangeDirection()
    {
        if(isRotating)
        {return;}

        if(Input.GetKeyDown(_rightKey))
        { RotateToRightCallback();}
        
        if(Input.GetKeyDown(_leftKey))
        { RotateToLeftCallback();}
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
    }

    private void Move()
    {
        if (Time.time > _moveTime)
        {
            for (int i = _playerBody.Count - 1; i > 0; i--)
            { _playerBody[i].transform.position = _playerBody[i - 1].transform.position; }

            if (_playerBody.Count > 0)
            { _playerBody[0].transform.position = transform.position;}

            float realVelocity = _speed - (_cargoLoadMultiplier * _playerBody.Count) + _externalMultiplier;
            realVelocity = realVelocity < 2 ? 2f : realVelocity;

            transform.position += (Vector3)_direction * _cellSize;
            _moveTime = Time.time + 1 / realVelocity;
            _snakeIndex = transform.position / _cellSize;
            isRotating = false;
        }
    }
}
