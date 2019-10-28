using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttritubeChange : SkillFunction
{
    [Space(100)]
    public int changeDataInPc;
    public AttributeData TargetTag;
    public StateTag TargetState;
    public int changeLastTime;

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

        StartCoroutine(IEStartSkill(source, target));
    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_CAST, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        target.ControlStateVisual(TargetState, true, changeLastTime);
        target.ReadThisStateEffect(TargetState).UniqueEffectInRA.AddPerc(valCaused(source,target), TargetTag);

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForEnd(source));
    }
    public int valCaused(BattleRoleData source, BattleRoleData target)
    {
        return (int)(changeDataInPc * (1 + 0.25f * SkillGrade));
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }
}
