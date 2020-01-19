using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[CreateAssetMenu(fileName ="enemy_info",menuName ="Wun/角色/敌人信息")]
public class EnemyInfo : CharacterInfo
{
    [SerializeField]
    private List<EnemyRace> race;
    public List<EnemyRace> Race 
    {
        get { return race; }
        set { race = value; } 
    }

    [SerializeField]
    private EnemyRank enemyRank;
    public EnemyRank EnemyRank
    {
        get { return enemyRank; }
        set { enemyRank = value; }
    }

    [SerializeField]
    private RoleAttributeList _RAL;
    public RoleAttributeList RAL
    {
        get { return _RAL; }
        set { _RAL = value; }
    }

    [SerializeField]
    private string specialStr;
    public string SpecialStr
    {
        get { return specialStr; }
        set { specialStr = value; }
    }

    public EnemyInfo()
    {
        CharacterType = SDConstants.CharacterType.Enemy;
    }
    public override void initData(string id, string name, string desc,CharacterSex sex, string faceIcon
        , SDConstants.CharacterType ctype = SDConstants.CharacterType.Enemy)
    {
        base.initData(id, name, desc, sex, faceIcon, ctype);
    }

    [Space]
    public int weight;
    [Header("掉落设置")]
    public float dropPercent;
    public List<consumableItem> dropItems;


    [Space]
    [SerializeField]
    private bool _useSpineData;
    public bool UseSpineData
    {
        get { return _useSpineData; }
        private set { _useSpineData = value; }
    }
    [SerializeField, ConditionalHide("_useSpineData", true, false)]
    private RoleSkeletonData _SpineData;
    public RoleSkeletonData SpineData
    {
        get { return _SpineData; }
        private set { _SpineData = value; }
    }
}
