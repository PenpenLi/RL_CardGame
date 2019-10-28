using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveOne : SkillFunction
{
    [Space(100)]
    public RoleBarChart BasicVal;
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        source.HpController.consumeHp(BCCostPerTime.HP);
        source.MpController.consumeMp(BCCostPerTime.MP);
        source.TpController.consumeTp(BCCostPerTime.TP);
        //
        UseSkillAddMpTp(source, GetComponent<HSkilInfo>().AfterwardsAddType);
        List<BattleRoleData> list = DealWithAOEAction(source, target);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], valCaused(source, list[i])));
        }
    }
    public void PropStartSkill(BattleRoleData source, BattleRoleData target, RoleBarChart val
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
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target, RoleBarChart val)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_CAST, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        SLEffectManager.Instance.playCommonEffectRevive(target.transform.position);
        yield return new WaitForSeconds(effectLastTime);
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_IDLE, true);
        Debug.Log(source.name + "revive BCAdd "
            + " hp+" + val.HP + " mp+" + val.MP + " tp+" + val.TP + "  to " + target.name);
        target.revive(val);

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForShortEnd(source));
    }
    public RoleBarChart valCaused(BattleRoleData source, BattleRoleData target)
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
        foreach(BattleRoleData unit in BM.Remaining_SRL)
        {
            if (unit.IsDead)
            {
                _autoBattleTarget = unit;
                flag = true;
            }
        }
        return flag;
    }
}
