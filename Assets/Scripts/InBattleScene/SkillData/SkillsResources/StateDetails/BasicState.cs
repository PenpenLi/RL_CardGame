using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicState 
{
    public int SkillGrade;
    public string NAME;
    public bool Success;
    #region 状态类基本数据
    public enum StateEndType
    {
        time, skill, beAtked,
    }
    [Header("状态类基本数据"), Space(25)]
    public StateEndType stateEndType = StateEndType.time;
    /// <summary>
    /// 设置状态持续时间
    /// </summary>
#if UNITY_EDITOR
    [ConditionalHide("stateEndType", (int)StateEndType.time, true, false)]
#endif
    public int StateLastTime;
    /// <summary>
    /// 该状态所属类型
    /// </summary>
    public StateTag stateTag;
    /// <summary>
    /// 该状态造成伤害类型
    /// </summary>
#if UNITY_EDITOR
    [EnumMemberNames("元素", "物理", "奥秘", "无")]
#endif
    public SkillKind stateKind;
    /// <summary>
    /// 触发状态概率
    /// </summary>
    public int StatePossibilityBuff = 100;//%
    #endregion
    [System.Serializable]
    public class ChangeInRAL
    {
        public string Title;
#if UNITY_EDITOR
        [DisplayName("是否为标准抗性(否则为标准属性)")]
#endif
        public bool IsForRD = true;
#if UNITY_EDITOR
        [ConditionalHide("IsForRD", true, true)]
#endif
        public AttributeData ADTag;
#if UNITY_EDITOR
        [ConditionalHide("IsForRD", true, false)]
#endif
        public StateTag STag;
        //
        public enum DataOrigin
        {
            normal,
            otherAtb,
            armor,
        }
        public DataOrigin UseDataType = DataOrigin.normal;
#if UNITY_EDITOR
        [ConditionalHide("UseDataType", (int)DataOrigin.normal, true, false)]
#endif
        public NumberData ChangeData;
#if UNITY_EDITOR
        [ConditionalHide("UseDataType", (int)DataOrigin.otherAtb, true, false)]
#endif
        public AttributeData OtherAtb;
#if UNITY_EDITOR
        [ConditionalHide("UseDataType", (int)DataOrigin.otherAtb, true, false)]
#endif
        public int UsePercent;
#if UNITY_EDITOR
        [ConditionalHide("UseDataType", (int)DataOrigin.armor, true, false)]
#endif
        public EquipPosition UsePos;

        public ChangeInRAL(NumberData data, AttributeData tag)
        {
            UseDataType = DataOrigin.normal;
            ChangeData = data; IsForRD = true; ADTag = tag;
        }
        public ChangeInRAL(NumberData data, StateTag tag)
        {
            UseDataType = DataOrigin.normal;
            ChangeData = data; IsForRD = false; STag = tag;
        }
    }
    public List<ChangeInRAL> AllChangesInRAL = new List<ChangeInRAL>();
    public NDBarChart ChangeInBarChart = NDBarChart.zero;

    [SerializeField]
    private bool aimAtSelf = false;
    public bool AimAtSelf
    {
        get { return aimAtSelf; }
        set { aimAtSelf = value; }
    }
    public void SimpleStartState(BattleRoleData source, BattleRoleData target)
    {
        StateFunctionWork(source, target, ralCaused(source, target)
                , bcCaused(source, target)
                , dmgCaused(source, target), StateLastTime);
    }
    public void StateFunctionWork(BattleRoleData source, BattleRoleData target
    , RoleAttributeList ral, RoleBarChart bc
    , int dmg, int lastTime)
    {

        OneStateController state = new OneStateController()
        {
            ID = NAME + "#" + stateTag.ToString().ToUpper(),
            NAME = NAME,
            LastTime = stateEndType == StateEndType.time ? lastTime : 1,
            RAL = ral,
            BarChart = bc,
            ExtraDmg = dmg,
            StateTag = stateTag,
        };
        if (target.AddStandardState(state))
        {
            Debug.Log("对 " + target.name + "添加 状态(sstate)" + state.NAME + " 成功");
        }
    }
    public virtual int dmgCaused(BattleRoleData source,BattleRoleData target)
    {
        return 0;
    }
    public virtual RoleAttributeList ralCaused(BattleRoleData source,BattleRoleData target)
    {
        return RoleAttributeList.zero;
    }
    public virtual RoleBarChart bcCaused(BattleRoleData source, BattleRoleData target)
    {
        RoleBarChart bc = RoleBarChart.zero;
        int hp = ChangeInBarChart.HP.DATA;
        if (ChangeInBarChart.HP.dataTag == NumberData.DataType.percent)
        {
            hp = (int)(target.HpController.MaxHp * ChangeInBarChart.HP.DECIMAL);
        }
        bc.HP = hp;
        int mp = ChangeInBarChart.MP.DATA;
        if (ChangeInBarChart.MP.dataTag == NumberData.DataType.percent)
        {
            mp = (int)(target.MpController.maxMp * ChangeInBarChart.MP.DECIMAL);
        }
        bc.MP = mp;
        int tp = ChangeInBarChart.TP.DATA;
        if (ChangeInBarChart.TP.dataTag == NumberData.DataType.percent)
        {
            tp = (int)(target.TpController.maxTp * ChangeInBarChart.TP.DECIMAL);
        }
        bc.TP = tp;
        return bc;
    }
}
