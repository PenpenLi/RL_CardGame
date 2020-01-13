using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StandardState:BasicState
{
    [DisplayName("是-使用施法者属性/否-使用百分比伤害")]
    public bool UseSourceAToDmg = false;
    [ConditionalHide("UseSourceAToDmg",true,false)]
    public NumberData ExtraDmg;
    [ConditionalHide("UseSourceAToDmg",true,true)]
    public int UsePercent;

    [SerializeField]
    private SDConstants.AOEType stateAOE = SDConstants.AOEType.None;
    public SDConstants.AOEType StateAOE
    {
        get { return stateAOE; }
        set { stateAOE = value; }
    }
    public  void StartState(SkillFunction HS,BattleRoleData source, BattleRoleData target)
    {
        SkillGrade = HS.SkillGrade;
        NAME = HS.name;
        if (StatePossibility(HS, source, target))
            SimpleStartState(source, target);
    }
    public bool StatePossibility(SkillFunction HS,BattleRoleData source,BattleRoleData target)
    {
        if (AllRandomSetClass.PercentIdentify(StatePossibilityBuff))
        {
            if (HS.ThisSkillAim == SkillAim.Self)
            {
                Success = false; return Success;
            }
            if (HS.ThisSkillAim == SkillAim.Friend)
            {
                Success = false; return Success; 
            }
        }
        float rate = AllRandomSetClass.SimplePercentToDecimal
            (StatePossibilityBuff
            - target.ThisBasicRoleProperty().ReadRA(stateTag));
        rate = Mathf.Clamp(rate, 0.05f, 0.95f);
        if (UnityEngine.Random.Range(0, 1f) < rate) 
        {
            Success = true;
            return Success; 
        }
        Success = false;
        return Success;
    }
    #region Function


    /// <summary>
    /// 状态造成的伤害计算
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public override int dmgCaused(BattleRoleData source,BattleRoleData target)
    {
        int basicVal;
        if (!UseSourceAToDmg)
        {
            basicVal = ExtraDmg.DATA;
            if (ExtraDmg.dataTag == NumberData.DataType.percent)
            {
                basicVal = (int)(target.HpController.MaxHp * ExtraDmg.DECIMAL);
            }
        }
        else
        {
            float _decimal = AllRandomSetClass.SimplePercentToDecimal(UsePercent);
            float val = 0;
            if (stateKind == SkillKind.Elemental)
            {
                val = source.ThisBasicRoleProperty()._role.at * _decimal;
            }
            else if (stateKind == SkillKind.Physical)
            {
                val = source.ThisBasicRoleProperty()._role.mt * _decimal;
            }
            else if (stateKind == SkillKind.Arcane)
            {
                val = (source.ThisBasicRoleProperty()._role.mt
                    + source.ThisBasicRoleProperty()._role.at)
                    * _decimal;
            }
            basicVal = (int)val;
        }
        return basicVal;
    }

    /// <summary>
    /// 状态造成的属性影响计算
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public override RoleAttributeList ralCaused(BattleRoleData source,BattleRoleData target)
    {
        RoleAttributeList ral = RoleAttributeList.zero;
        for(int i = 0; i < AllChangesInRAL.Count; i++)
        {
            ChangeInRAL c = AllChangesInRAL[i];
            int d = c.ChangeData.DATA;
            if (c.IsForAD)
            {
                if (c.UseDataType == ChangeInRAL.DataOrigin.normal)
                {
                    if (c.ChangeData.dataTag == NumberData.DataType.percent)
                    {
                        d = (int)(target.ThisBasicRoleProperty().ReadRA(c.ADTag)
                            * c.ChangeData.DECIMAL);
                    }
                }
                else if (c.UseDataType == ChangeInRAL.DataOrigin.otherAtb)
                {
                    int ori = target.ThisBasicRoleProperty().ReadRA(c.OtherAtb);
                    d = (int)(ori * AllRandomSetClass.SimplePercentToDecimal(c.UsePercent));
                }
                else if (c.UseDataType == ChangeInRAL.DataOrigin.armor)
                {
                    RoleAttributeList _ral = getRALFromEquip(c.UsePos, source, target);
                    d = _ral.read(c.ADTag);
                }

                ral.Add(d, c.ADTag);
            }
            else
            {
                if (c.UseDataType == ChangeInRAL.DataOrigin.normal)
                {
                    if (c.ChangeData.dataTag == NumberData.DataType.percent)
                    {
                        d = (int)(target.ThisBasicRoleProperty().ReadRA(c.STag)
                            * c.ChangeData.DECIMAL);
                    }
                }
                else if (c.UseDataType == ChangeInRAL.DataOrigin.otherAtb)
                {
                    int ori = target.ThisBasicRoleProperty().ReadRA(c.OtherAtb);
                    d = (int)(ori * AllRandomSetClass.SimplePercentToDecimal(c.UsePercent));
                }
                else if (c.UseDataType == ChangeInRAL.DataOrigin.armor)
                {
                    RoleAttributeList _ral = getRALFromEquip(c.UsePos, source, target);
                    d = _ral.read(c.STag);
                }

                ral.Add(d, c.STag);
            }
        }
        return ral;
    }
    RoleAttributeList getRALFromEquip(EquipPosition UsePos,BattleRoleData source,BattleRoleData target)
    {
        RoleAttributeList _ral = RoleAttributeList.zero;
        SDArmor SDA = null;
        if (UsePos == EquipPosition.Head)
        {
            SDA = target.HeroProperty._helmet;
        }
        else if (UsePos == EquipPosition.Breast)
        {
            SDA = target.HeroProperty._breastplate;
        }
        else if (UsePos == EquipPosition.Arm)
        {
            SDA = target.HeroProperty._gardebras;
        }
        else if (UsePos == EquipPosition.Leg)
        {
            SDA = target.HeroProperty._legging;
        }
        else if (UsePos == EquipPosition.Hand)
        {
            SDA = target.HeroProperty._weapon;
        }
        if (SDA != null)
            _ral = SDA.RoleBasicRA.Clone;
        if (UsePos == EquipPosition.Finger)
        {
            RoleAttributeList r0 = RoleAttributeList.zero;
            if (target.HeroProperty._jewelry0 != null)
                r0 = target.HeroProperty._jewelry0.RoleBasicRA.Clone;
            RoleAttributeList r1 = RoleAttributeList.zero;
            if (target.HeroProperty._jewelry1 != null)
                r1 = target.HeroProperty._jewelry1.RoleBasicRA.Clone;
            _ral = r0 + r1;
        }
        return _ral;
    }
    
    public override RoleBarChart bcCaused(BattleRoleData source, BattleRoleData target)
    {
        return base.bcCaused(source, target);
    }
    #endregion

    public StandardState(string _name = "STANDARDSTATE",int grade = 0)
    {
        SkillGrade = grade;
        NAME = _name;
    }
}
