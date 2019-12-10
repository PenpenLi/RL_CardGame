using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "enemy_info", menuName = "Wun/角色/敌人种族")]
public class EnemyRace : CharacterRace
{
    public EnemyRace()
    {
        CharacterType = SDConstants.CharacterType.Enemy;
    }
}
