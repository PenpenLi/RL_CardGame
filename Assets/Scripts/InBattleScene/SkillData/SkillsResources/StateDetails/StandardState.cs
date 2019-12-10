using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardState : MonoBehaviour
{
    [HideInInspector]
    public int basicVal;
    int SkillGrade;
    #region 状态类专有数据
    [Header("状态类专有数据"), Space(25)]
    /// <summary>
    /// 设置状态持续时间
    /// </summary>
    public int StateLastTime;
    /// <summary>
    /// 该状态所属类型
    /// </summary>
    public StateTag stateTag;
    /// <summary>
    /// 该状态造成伤害类型
    /// </summary>
    public SkillKind stateKind;
    /// <summary>
    /// 状态伤害释放距离
    /// </summary>
    public SkillBreed stateBreed = SkillBreed.Absence;
    /// <summary>
    /// 触发状态概率
    /// </summary>
    public int StatePossibilityBuff = 100;//%
    #endregion
    public  void StartState(SkillFunction HS,BattleRoleData source, BattleRoleData target,int val)
    {
        SkillGrade = HS.SkillGrade;
        if(StatePossibility(HS,source,target))
            StateFunctionWork(source, target, dmgCaused(source,target), StateLastTime);
    }
    public bool StatePossibility(SkillFunction HS,BattleRoleData source,BattleRoleData target)
    {
        if (AllRandomSetClass.PercentIdentify(StatePossibilityBuff))
        {
            if (HS.ThisSkillAim == SkillAim.Self) return true;
            if (HS.ThisSkillAim == SkillAim.Friend) return true;
        }
        float rate = AllRandomSetClass.SimplePercentToDecimal
            (StatePossibilityBuff
            - target.ThisBasicRoleProperty().ReadRA(stateTag));
        rate = Mathf.Clamp(rate, 0.05f, 0.95f);
        if (UnityEngine.Random.Range(0, 1f) < rate) return true;
        return false;
    }

    public void StateFunctionWork(BattleRoleData source, BattleRoleData target
        ,int val, int lastTime)
    {
        target.ControlStateVisual(stateTag, true, lastTime);
        target.OneState(stateTag).ExtraDmg += val;
    }
    /// <summary>
    /// 直接影响(伤害)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public int dmgCaused(BattleRoleData source,BattleRoleData target)
    {
        HSExportDmgModifity _S = GetComponent<HSExportDmgModifity>();
        if (_S)
        {
            return _S.AllResult(source, target, stateKind, SkillGrade);
        }
        if(stateKind == SkillKind.Elemental)
        {
            basicVal = source.ThisBasicRoleProperty()._role.at/10;
        }
        else if(stateKind == SkillKind.Physical)
        {
            basicVal = source.ThisBasicRoleProperty()._role.mt/10;
        }
        else if(stateKind == SkillKind.Arcane)
        {
            basicVal = (source.ThisBasicRoleProperty()._role.mt + source.ThisBasicRoleProperty()._role.at)
                / 20;
        }
        return basicVal;
    }

}
