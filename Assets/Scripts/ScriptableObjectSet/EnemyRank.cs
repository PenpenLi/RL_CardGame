using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy_Rank",menuName ="Wun/角色/敌人阶级")]
public class EnemyRank:BaseRank
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("炮灰", "一般敌人", "精英","首领","邪神")]
#endif
    private SDConstants.EnemyType enemyType = SDConstants.EnemyType.fodder;
    public SDConstants.EnemyType EnemyType
    {
        get { return enemyType; }
    }
}