using System.Collections;
using UnityEngine;

public enum BodyPartType
{
    Regular = 0,
    BatteringRam = 1,
    EnginePower = 2
}

public class BodyPartBehavior : EntityBase, IDangerousEncounter
{
    [SerializeField] private SpriteRenderer _partDisplay;
    [SerializeField] private Color _regularColor;

    [SerializeField] private float _intangibleTime;
    [SerializeField] private BodyPartType _partType;
    public BodyPartType PartType => _partType;

    public override void OnEntitySpawn(int gameSet)
    {
        _gameSetID = gameSet;
        GameManager.OnRegisterEntity?.Invoke(this);
        StartCoroutine(MakeIntangible());
    }

    public void DangerousInteractionCallback(ICharacter playerTouched)
    {
       playerTouched.ReceiveDamage();
    }

    public void ChangeTypeCallback(BodyPartType newType)
    {
        _partType = newType;

        switch (newType)
        {
            case BodyPartType.Regular:
                _partDisplay.color = _regularColor;
                break;
            case BodyPartType.BatteringRam:
                _partDisplay.color = Color.magenta;
                break;
            case BodyPartType.EnginePower:
                _partDisplay.color = Color.white;
                break;
        }
    }

    public void MakeIntangibleCallback()
    {
        StartCoroutine(MakeIntangible());
    }

    private IEnumerator MakeIntangible()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;

        yield return new WaitForSeconds(_intangibleTime);

        col.enabled = true;
    }
}
