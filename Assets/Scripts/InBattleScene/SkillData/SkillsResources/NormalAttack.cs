using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : SkillFunction
{
    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        if (IsProcessing) return;
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);

        StartCoroutine(IEStartSkill(source,target));
    }
    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target)
    {
        source.playMoveTowardAnimation(target.transform.position);
        yield return new WaitForSeconds(moveTowardAndBackTime);

        if(source.unit_character_model.CurrentCharacterModel!=null)
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_attack, false);
        SLEffectManager.Instance.playCommonEffectNormalAttack(target.transform.position);
        yield return new WaitForSeconds(SDConstants.AnimTime_ATTACK - hitTime);
        int damage = dmgCaused(source, target);
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
        damage = (int)(damage * criD);
        damage += NowExpect;
        #region 传输技能状态
        target.HpController.isCriDmg = IsCausedCritDmg;
        target.HpController.isEvoHappen = IsCausedMiss;
        target.HpController.isFault = IsCausedFault;
        #endregion
        damage = (int)(damage * AllRandomSetClass.SimplePercentToDecimal(100 + target.PhysModify));
        Debug.Log(source.name + " attack cause " + damage + " damage to " + target.name);
        target.HpController.getDamage(damage, SkillKind.Physical);

        yield return new WaitForSeconds(hitTime);
        yield return new WaitForSeconds(skillLastTime);

        if (source.unit_character_model.CurrentCharacterModel != null)
            source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_idle, true);

        source.playMoveBackAnimation();
        yield return new WaitForSeconds(moveTowardAndBackTime);

        StartCoroutine(IEWaitForEnd(source));
    }

    public override void EndSkill()
    {
        base.EndSkill();
    }


    public override int dmgCaused(BattleRoleData source,BattleRoleData target)
    {
        return base.dmgCaused(source, target);
    }

    public override bool isSkillMeetConditionToAutoRelease()
    {
        return true;
    }
}
