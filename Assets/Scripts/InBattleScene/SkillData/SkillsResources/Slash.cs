using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : SkillFunction
{
    [Header("ExtraStateAdd"), Space(100)]
    public StandardState _standardState;
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);


        List<BattleRoleData> list = DealWithAOEAction(source, target);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i]));
            if (_standardState) _standardState.StartState(this, source, list[i] 
                , dmgCaused(source,list[i]) / 10 );
        }


    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target)
    {
        source.playMoveTowardAnimation(target.transform.position);
        yield return new WaitForSeconds(moveTowardAndBackTime);

        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_attack, false);
        SLEffectManager.Instance.playCommonEffectSlash(target.transform.position);
        yield return new WaitForSeconds(SDConstants.AnimTime_ATTACK - hitTime);
        int damage = dmgCaused(source, target);
        #region 计算技能状态
        source.SkillCheck(this, target);
        IsCausedCritDmg = source.CritHappen;
        IsCausedMiss = !source.AccurHappen;
        IsCausedFault = source.FaultHappen;
        if (IsCausedCritDmg)
        {
            float criD
                = AllRandomSetClass.SimplePercentToDecimal
                (source.ThisBasicRoleProperty().CRIDmg + 100);
            damage = (int)(damage * criD);
        }
        int NowExpect = (int)(UnityEngine.Random.Range(-1f, 1f) * source.ExpectResult);
        #endregion
        damage += NowExpect;
        #region 传输技能状态
        target.HpController.isCriDmg = IsCausedCritDmg;
        target.HpController.isEvoHappen = IsCausedMiss;
        target.HpController.isFault = IsCausedFault;
        #endregion

        damage = (int)(damage * AllRandomSetClass.SimplePercentToDecimal(100 + target.PhysModify));

        Debug.Log(source.name + " slash cause " + damage + " damage to " + target.name);
        target.HpController.getDamage(damage, SkillKind.Physical);

        yield return new WaitForSeconds(hitTime);
        //yield return new WaitForSeconds(skillLastTime);
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_idle, true);
        source.playMoveBackAnimation();
        yield return new WaitForSeconds(moveTowardAndBackTime);

        StartCoroutine(IEWaitForEnd(source));
    }
    public override int dmgCaused(BattleRoleData source, BattleRoleData target)
    {
        return base.dmgCaused(source, target);
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
