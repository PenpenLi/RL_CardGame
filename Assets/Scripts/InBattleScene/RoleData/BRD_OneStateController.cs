using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateTag
{
    Bleed = 0,
    Mind = 1,
    Fire = 2,
    Frost = 3,
    Corrosion = 4,
    Hush = 5,
    Dizzy = 6,
    Confuse = 7,
    End = 8,
}

[System.Serializable]
public class BRD_OneStateController 
{
#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private StateTag _statetag;
    public StateTag stateTag
    {
        get { return _statetag; }
        set { _statetag = value; }
    }


    public int stateCondition = 0;

    //public int LastTime = 0;
    public RoleAttributeList UniqueEffectInRA = new RoleAttributeList();

    /// <summary>
    /// 伤害值
    /// </summary>
    public int ExtraDmg;
    public void Clear(bool ResetCondition = false)
    {
        UniqueEffectInRA = RoleAttributeList.zero;
        //BCArray.ResetSelf();
        ExtraDmg = 0;
        if (ResetCondition) stateCondition = 0;
    }
}

[System.Serializable]
public class OneStateController
{
    [SerializeField]
    private string _ID;
    public string ID { get { return _ID; } set { _ID = value; } }

    [SerializeField]
    private string _NAME;
    public string NAME { get { return _NAME; } set { _NAME = value; } }

    [SerializeField]
    private string _icon;
    public string Icon { get { return _icon; } set { _icon = value; } }

    [SerializeField]
    private RoleAttributeList _RAL = RoleAttributeList.zero;
    public RoleAttributeList RAL { get { return _RAL; } set { _RAL = value; } }

    [SerializeField]
    private RoleBarChart barchart = RoleBarChart.zero;
    public RoleBarChart BarChart { get { return barchart; } set { barchart = value; } }

    [SerializeField]
    private int extraDmg = 0;
    public int ExtraDmg { get { return extraDmg; } set { extraDmg = value; } }

    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("按时间结束","使用技能后结束/行动后结束","被攻击后结束")]
#endif
    private StandardState.StateEndType stateEndType;
    public StandardState.StateEndType StateEndType
    {
        get { return stateEndType; }
        set { stateEndType = value; }
    }
    [SerializeField]
#if UNITY_EDITOR
    [ConditionalHide("stateEndType",(int)StandardState.StateEndType.time,true,false)]
#endif
    private int lastTime;
    public int LastTime { get { return lastTime; } set { lastTime = value; } }

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    private StateTag _statetag = StateTag.End;
    public StateTag StateTag
    {
        get { return _statetag; }
        set { _statetag = value; }
    }

    [SerializeField, Header(" 能否被新状态挤走")]
    private bool canSqueeze = true;
    public bool CanSqueeze
    {
        get { return canSqueeze; }
        set { canSqueeze = value; }
    }

    public delegate void StateExtraEvent(BattleRoleData stateTarget);
    public event StateExtraEvent OnCheckState;
    public void ExtraFunction(BattleRoleData target)
    {
        OnCheckState?.Invoke(target);
    }
}