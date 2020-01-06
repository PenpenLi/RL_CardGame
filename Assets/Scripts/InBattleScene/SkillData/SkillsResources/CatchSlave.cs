using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
public class CatchSlave : SkillFunction
{
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);

        List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        foreach(BattleRoleData t in list)
        {
            StartCoroutine(IEStartSkill(source, t, NumberData.Build(valCaused(source, t))));
        }
    }
    public void PropStartSkill(BattleRoleData source, BattleRoleData target, NumberData val
        , SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;

        AOEType = aoeType;
        List<BattleRoleData> list = DealWithAOEAction(source, target,aoeType);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], val));
        }
    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target,NumberData _val)
    {
        source.playMoveTowardAnimation(target.transform.position);
        yield return new WaitForSeconds(moveTowardAndBackTime);
        //
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_attack, false);
        SLEffectManager.Instance.playCommonEffectSlash(target.transform.position);
        yield return new WaitForSeconds(SDConstants.AnimTime_ATTACK - hitTime);
        //
        float r = 0;
        if (_val.dataTag == NumberData.DataType.integer)
            r = AllRandomSetClass.SimplePercentToDecimal(_val.DATA);
        else
        {
            r = _val.DECIMAL;
        }

        //目标敌人级别越高成功率越低
        int level = target.ThisBasicRoleProperty().LEVEL;
        for(int i = 0; i < level; i++)
        {
            r *= 0.8f;
        }

        //
        if (UnityEngine.Random.Range(0, 1) < r)//捕获成功
        {
            SDDataManager.Instance.AddSlave(target.ThisBasicRoleProperty().ID);
            target.HpController.FadeAndDisappear();
        }
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_idle, true);
        source.playMoveBackAnimation();
        yield return new WaitForSeconds(moveTowardAndBackTime);

        StartCoroutine(IEWaitForEnd(source));
    }
    public int valCaused(BattleRoleData source,BattleRoleData target)
    {
        int sBF = SDDataManager.Instance.HeroBattleForce(source.unitHashcode);
        int tBF = SDDataManager.Instance.HeroBattleForce(target.unitHashcode);
        float rate = tBF * 1f / sBF;
        if (rate < 0.25f) return 75;
        else return 0;
    }
    public override void EndSkill()
    {
        base.EndSkill();

    }
    public override bool isSkillMeetConditionToAutoRelease()
    {
        return true;
    }
}
