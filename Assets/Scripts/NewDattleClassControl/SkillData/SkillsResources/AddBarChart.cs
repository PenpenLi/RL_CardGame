using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBarChart : SkillFunction
{
    [Space(100)]
    public RoleBarChart BasicVal;
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        if (IsProcessing) return;
        IsProcessing = true;
        IsUsed = true;
        source.HpController.consumeHp(BCCostPerTime.HP);
        source.MpController.consumeMp(BCCostPerTime.MP);
        source.TpController.consumeTp(BCCostPerTime.TP);
        //
        source.TpController.addTp(source.TpController.maxTp / 25);//技能增加Tp  
        StartCoroutine(IEStartSkill(source, target, valCaused(source, target)));
    }
    public void PropStartSkill(BattleRoleData source,BattleRoleData target,RoleBarChart val)
    {
        base.StartSkill(source, target);
        if (IsProcessing) return;
        IsProcessing = true;
        IsUsed = true;
        StartCoroutine(IEStartSkill(source, target, val));
    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target,RoleBarChart val)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_CAST, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);
        if (val.HP != 0)
            SLEffectManager.Instance.playCommonEffectLocalBarChartAdd
                (target.transform.position);
        else if(val.MP!=0)
            SLEffectManager.Instance.playCommonEffectLocalBarChartAdd
                (target.transform.position, SDConstants.BCType.mp);
        else if(val.TP!=0)
            SLEffectManager.Instance.playCommonEffectLocalBarChartAdd
                (target.transform.position, SDConstants.BCType.tp);
        yield return new WaitForSeconds(effectLastTime);
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_IDLE, true);
        Debug.Log(source.name + "cause BCAdd " 
            + " hp+" + val.HP + " mp+" + val.MP + " tp+" + val.TP + "  to " + target.name);
        if (val.HP > 0) target.HpController.addHp(val.HP);
        if (val.MP > 0) target.MpController.addMp(val.MP);
        if (val.TP > 0) target.TpController.addTp(val.TP);

        StartCoroutine(IEWaitForShortEnd(source));
    }
    public RoleBarChart valCaused(BattleRoleData source,BattleRoleData target)
    {
        return BasicVal;
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }
    public override bool isSkillMeetConditionToAutoRelease()
    {
        bool flag = false;
        //return base.isSkillMeetConditionToAutoRelease();
        if (BasicVal.HP > 0)
        {
            foreach (BattleRoleData unit in BM.Remaining_SRL)
            {
                if (unit.HpController.CurrentHp <= (unit.HpController.MaxHp) * 0.6f)
                {
                    _autoBattleTarget = unit;
                    flag = true;
                }
            }
        }
        if (BasicVal.MP > 0)
        {
            foreach (BattleRoleData unit in BM.Remaining_SRL)
            {
                if (unit.MpController.currentMp <= (unit.MpController.maxMp) * 0.5f)
                {
                    _autoBattleTarget = unit;
                    flag = true;
                }
            }
        }        
        return flag;
    }
}
