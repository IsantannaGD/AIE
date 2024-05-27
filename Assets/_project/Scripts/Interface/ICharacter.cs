using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public void BodyGrow(BodyPartType type = BodyPartType.Regular);
    public void ReceiveDamage();
    public void PickPowerUp(EntityType type);

}
