using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharaPropertyEntry
{
    public CharaProperty[] CharaProperty;
}

[System.Serializable]
public class CharaProperty 
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int chara_ID;
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum CharaType
    {
        unKown=-1,
        NPC,
        ENEMY,
        HERO,
    }
    public CharaType chara_type = CharaType.unKown;
    /// <summary>
    /// 角色详细类型
    /// </summary>
    public int chara_kind;
    /// <summary>
    /// 角色名称
    /// </summary>
    public string chara_name;
    /// <summary>
    /// 角色描述
    /// </summary>
    public string chara_description;
    /// <summary>
    /// 角色Icon
    /// </summary>
    public string chara_icon;
    /// <summary>
    /// 角色品质显示icon
    /// </summary>
    public string chara_bgicon;
    /// <summary>
    /// 角色数量
    /// </summary>
    public int chara_count;
    /// <summary>
    /// 角色品质
    /// </summary>
    public int chara_quality;
    /// <summary>
    /// 角色的操作：升级/解散/部署/死亡/招募
    /// </summary>
    public int chara_operation;
    [SerializeField]
    public Role BattleData;

    public CharaProperty(int ID)
    {
        chara_ID = ID;

    }
}
