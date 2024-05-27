using System.Collections.Generic;
using UnityEngine;

public class Player : EntityBase, ICharacter
{
    private Vector2 _direction = Vector2.up;

    [SerializeField] private Color _regularColor;

    [SerializeField] private SpriteRenderer _headDisplay;

    [SerializeField] private BodyPartBehavior _bodyPrefab;
    [SerializeField] private List<BodyPartBehavior> _playerBody = new List<BodyPartBehavior>();
    [SerializeField] private List<BodyPartType> _bodyPartTypeList = new List<BodyPartType>();

    [SerializeField] private float _cellSize = 0.3f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _cargoLoadMultiplier;
    [SerializeField] private float _externalMultiplier;

    [SerializeField] private DirectionType _directionIndex = DirectionType.Up;
    [SerializeField] private float _moveTime = 0f;
    [SerializeField] private bool isRotating = false;

    [SerializeField] private string _leftKey;
    [SerializeField] private string _rightKey;

    [SerializeField] private bool _haveBatteringRam = false;
    [SerializeField] private bool _haveTimeTravel = false;

    [SerializeField] private int _batteringRamCount = 0;

    public override void OnEntitySpawn(int gameSet)
    {
        _gameSetID = gameSet;
        GameManager.OnRegisterEntity?.Invoke(this);

        _bodyPartTypeList.Add(BodyPartType.Regular);
    }

    public void SetInput(char l, char r)
    {
        _leftKey = $"{l}";
        _rightKey =$"{r}";
    }

    public void BodyGrow(BodyPartType type = BodyPartType.Regular)
    {
        Vector2 pos = transform.position;
        if (_playerBody.Count != 0)
        { pos = _playerBody[^1].transform.position; }

        BodyPartBehavior bodyPart = Instantiate(_bodyPrefab, pos, Quaternion.identity);
        bodyPart.OnEntitySpawn(_gameSetID);
        _playerBody.Add(bodyPart);
        _bodyPartTypeList.Insert(0, type);

        ReOrderBodePartCallback();
    }

    public void ReceiveDamage()
    {
        if (!_haveTimeTravel)
        {
            GameManager.Instance.OnGameOver?.Invoke(_gameSetID);
            Debug.LogWarning("Morreu Otário");
            return;
        }

        GameManager.OnTimeTravelUse?.Invoke();
        _haveTimeTravel = false;
    }

    public void PickPowerUp(EntityType type)
    {
        switch (type)
        {
            case EntityType.EnginePowerPowerUp:
                PickUpEnginePowerCallback();
                break;
            case EntityType.BatteryRamPowerUp:
                PickUpBatteringRamCallback();
                break;
            case EntityType.TimeTravelPowerUp:
                PickUpTimeTravelCallback();
                break;
        }
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
            if (_haveBatteringRam && !other.gameObject.TryGetComponent(out WallBehavior wall))
            {
                UseBatteringRamCallback();
            }
            else
            {
                enemyTouch.DangerousInteractionCallback(this);
            }
        }
    }

    private void PickUpEnginePowerCallback()
    {
        _externalMultiplier += _playerBody.Count * (_cargoLoadMultiplier * 1.5f);
        BodyGrow(BodyPartType.EnginePower);
    }

    private void PickUpBatteringRamCallback()
    {
        BodyGrow(BodyPartType.BatteringRam);

        _batteringRamCount++;
        _haveBatteringRam = _batteringRamCount > 0;
    }

    private void PickUpTimeTravelCallback()
    {
        GameManager.OnTimeTravelPick?.Invoke();
        _haveTimeTravel = true;
    }

    private void UseBatteringRamCallback()
    {
        BodyPartBehavior lostPart = default;

        foreach (BodyPartBehavior bodyPartBehavior in _playerBody)
        {
            bodyPartBehavior.MakeIntangibleCallback();

            if (bodyPartBehavior.PartType == BodyPartType.BatteringRam)
            {
                lostPart = bodyPartBehavior;
            }
        }

        int remove = _bodyPartTypeList.FindIndex((x) => x == BodyPartType.BatteringRam);
        _bodyPartTypeList.RemoveAt(remove);
        _playerBody.Remove(lostPart);
        Destroy(lostPart.gameObject);

        _batteringRamCount--;
        _haveBatteringRam = _batteringRamCount > 0;

        ReOrderBodePartCallback();
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
            isRotating = false;
        }
    }

    private void ReOrderBodePartCallback()
    {
        switch (_bodyPartTypeList[0])
        {
            case BodyPartType.Regular:
                _headDisplay.color = _regularColor;
                break;
            case BodyPartType.BatteringRam:
                _headDisplay.color = Color.magenta;
                break;
            case BodyPartType.EnginePower:
                _headDisplay.color = Color.white;
                break;
        }

        for (int i = 0; i < _playerBody.Count; i++)
        {
            _playerBody[i].ChangeTypeCallback(_bodyPartTypeList[i + 1]);
        }
    }
}
