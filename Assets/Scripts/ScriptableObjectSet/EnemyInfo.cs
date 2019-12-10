using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="enemy_info",menuName ="Wun/角色/敌人信息")]
public class EnemyInfo : CharacterInfo
{
    [SerializeField]
    private EnemyRace race;
    public EnemyRace Race 
    {
        get { return race; } 
    }

    [SerializeField]
    private EnemyRank enemyRank;
    public EnemyRank EnemyRank
    {
        get { return enemyRank; }
    }

    public EnemyInfo()
    {
        CharacterType = SDConstants.CharacterType.Enemy;
    }
}
