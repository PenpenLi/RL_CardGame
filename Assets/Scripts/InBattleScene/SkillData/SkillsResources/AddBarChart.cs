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
        CalculateBeforeFunction(source, target);

        List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], new NDBarChart(valCaused(source, list[i]))));
        }
    }

    public void PropStartSkill(BattleRoleData source, BattleRoleData target, NDBarChart val
        , SDConstants.AOEType aoeType)
    {
        base.StartSkill(source, target);
        if (IsProcessing) return;
        IsProcessing = true;
        IsUsed = true;

        List<BattleRoleData> list = DealWithAOEAction(source, target,aoeType);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], val));
        }
    }

    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target,NDBarChart _val)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_cast, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);
        if (_val.HP != 0)
            SLEffectManager.Instance.playCommonEffectLocalBarChartAdd
                (target.transform.position);
        else if(_val.MP!=0)
            SLEffectManager.Instance.playCommonEffectLocalBarChartAdd
                (target.transform.position, SDConstants.BCType.mp);
        else if(_val.TP!=0)
            SLEffectManager.Instance.playCommonEffectLocalBarChartAdd
                (target.transform.position, SDConstants.BCType.tp);
        yield return new WaitForSeconds(effectLastTime);
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_idle, true);

        RoleBarChart val = RoleBarChart.zero;
        if (_val.HP.dataTag == NumberData.DataType.integer) val.HP = _val.HP.DATA;
        else if(_val.HP.dataTag == NumberData.DataType.percent)
        {
            val.HP = (int)(target.HpController.MaxHp * _val.HP.DECIMAL); 
        }
        if (_val.MP.dataTag == NumberData.DataType.integer) val.MP = _val.MP.DATA;
        else if(_val.MP.dataTag == NumberData.DataType.percent)
        {
            val.MP = (int)(target.MpController.maxMp * _val.MP.DECIMAL);
        }
        if (_val.TP.dataTag == NumberData.DataType.integer) val.TP = _val.TP.DATA;
        else if(_val.TP.dataTag == NumberData.DataType.percent)
        {
            val.TP = (int)(target.TpController.maxTp * _val.TP.DECIMAL);
        }

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

