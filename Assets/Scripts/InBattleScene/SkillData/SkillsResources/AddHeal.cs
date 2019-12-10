using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHeal : SkillFunction
{
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);

        List<BattleRoleData> list = DealWithAOEAction(source, target);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], ValCaused(source, list[i]), true));
        }
    }
    public void PropStartSkill(BattleRoleData source, BattleRoleData target,int val
        , SDConstants.AOEType AOEType = SDConstants.AOEType.None)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;

        if (GetComponent<HSkilInfo>()) 
        {
            GetComponent<HSkilInfo>().AOEType = AOEType;
            List<BattleRoleData> list = DealWithAOEAction(source, target);
            for (int i = 0; i < list.Count; i++)
            {
                StartCoroutine(IEStartSkill(source, list[i], val));
            }
        }
        else
        {
            StartCoroutine(IEStartSkill(source, target, val));
        }
    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target,int val,bool useSpecial=false)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_cast, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);
        if (useSpecial)
        {
            #region 计算技能状态
            source.SkillCheck(this, target);
            IsCausedCritDmg = source.CritHappen;
            IsCausedMiss = !source.AccurHappen;
            IsCausedFault = source.FaultHappen;
            float criD = 1;
            if (IsCausedCritDmg)
            {
                criD
                    = AllRandomSetClass.SimplePercentToDecimal
                    (source.ThisBasicRoleProperty().CRIDmg + 100);
            }
            int NowExpect = (int)(UnityEngine.Random.Range(-1f, 1f) * source.ExpectResult);
            #endregion
            val = (int)(val * criD);
            val += NowExpect;
            #region 传输技能状态
            target.HpController.isCriDmg = IsCausedCritDmg;
            target.HpController.isEvoHappen = IsCausedMiss;
            target.HpController.isFault = IsCausedFault;
            #endregion
        }
        SLEffectManager.Instance.playCommonEffectLocalBarChartAdd(target.transform.position);
        yield return new WaitForSeconds(effectLastTime);

        Debug.Log(source.name + " heal cause " + val + " to " + target.name);
        target.HpController.addHp(val);
        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForShortEnd(source));
    }
    public int ValCaused(BattleRoleData source, BattleRoleData target)
    {
        HSExportDmgModifity S = GetComponent<HSExportDmgModifity>();
        if (S)
        {
            //int level = source.ThisBasicRoleProperty().LEVEL;
            return S.AllResult(source,target,AttributeData.MT,SkillGrade);
        }
        return source.ThisBasicRoleProperty().ReadRA(AttributeData.MT);
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }
    public override bool isSkillMeetConditionToAutoRelease()
    {
        //return base.isSkillMeetConditionToAutoRelease();
        BattleRoleData _unit = BM._currentBattleUnit;
        if (_unit.HeroProperty.LEVEL >= SDConstants.LEVELHaveSpecialBuff) return false;
        foreach(BattleRoleData unit in BM.Remaining_SRL)
        {
            if (unit.HpController.CurrentHp <= (unit.HpController.MaxHp * 0.75f))
            {
                _autoBattleTarget = unit;
                return true;
            }
        }
        return false;
    }
}
