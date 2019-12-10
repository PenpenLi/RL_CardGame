using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy_Rank",menuName ="Wun/角色/敌人阶级")]
public class EnemyRank:ScriptableObject
{
    [SerializeField]
    [DisplayName("识别码")]
    private string _ID;
    public string ID
    {
        get { return _ID; }
    }

    [SerializeField]
    [DisplayName("阶级名称")]
    private string _name;
    public string NAME
    {
        get { return _name; }
    }

    [SerializeField]
    private int _index;
    public int Index
    {
        get { return _index; }
    }

    [SerializeField]
    [EnumMemberNames("炮灰", "一般敌人", "精英","首领","邪神")]
    private SDConstants.EnemyType enemyType = SDConstants.EnemyType.fodder;
    public SDConstants.EnemyType EnemyType
    {
        get { return enemyType; }
    }


}