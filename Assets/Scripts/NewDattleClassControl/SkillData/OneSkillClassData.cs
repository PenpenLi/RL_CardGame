using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillKind
{
    Elemental,
    Physical,
    Arcane,
}
[System.Serializable]
public class OneState
{
    public OneStateShow ThisStateShow;
    //状态种类
    SkillKind _thisstatekind;
    public SkillKind ThisStateKind
        { get { return _thisstatekind; } private set { _thisstatekind = value; } }
    public string StateKindIcon;

    //此次状态目标
    SkillAim _ThisSkillTarget;
    public SkillAim ThisSkillTarget { get { return _ThisSkillTarget; } set { _ThisSkillTarget = value; } }

    //状态触发效果
    public OneEffect[] AllStateResults;


    //
    public OneState()
    {

    }
    public OneState Copy()
    {
        return new OneState();
    }
}
[System.Serializable]
public class OneStateShow
{
    public int StateId;
    public int StateTime;
    public string StateName;
    //
    public string StateIcon;
    public string StateBgIcon;
}



public enum SkillAim
{
    Self,
    OneOther,
    AllOthers,
    OneFriend,
    AllFriends,
}
public enum DataType
{
    Normal,
    HP_SP,
    HPOnly,
    SPOnly,
    MP,
    TP,
}
[System.Serializable]
public class OneEffect
{
    //施加数值影响设置
    public float DataRate = 1;
    int _data;
    public int Data { get { return (int)(_data * DataRate); } private set{ _data = value; }}
    DataType _thisdatatype;
    public DataType ThisDataType { get { return _thisdatatype; } set { _thisdatatype = value; } }
    public void SetDataRate(float Multi)
    {
        DataRate = Multi;
    }


    //此次影响目标
    SkillAim _ThisSkillTarget;
    public SkillAim ThisSkillTarget { get { return _ThisSkillTarget; }set { _ThisSkillTarget = value; } }

    //
    static public OneEffect CreateStandardSkillData()
    {
        OneEffect s = new OneEffect();
        return s;
    }
}




[System.Serializable]
public class OneSkill
{
    public int SkillId;
    public string SkillName;
    public string Icon;
    public string BgIcon;
    public string Desc;

    //技能种类
    SkillKind _thisskillkind;
    public SkillKind ThisSkillKind
    {
        get { return _thisskillkind; }
        private set { _thisskillkind = value; }
    }
    public string SkillKindIcon;

    //技能具体效果
    public OneEffect[] AllSkillResults;
    public OneState SkillState;

}