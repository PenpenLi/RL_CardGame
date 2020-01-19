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

        StartCoroutine(IEStartSkill(source,target,new NumberData(dmgCaused(source,target))));
    }
    public void PropStartSkill(BattleRoleData source, BattleRoleData target,NumberData val,SDConstants.AOEType aoe)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;

        AOEType = aoe;
        List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        for (int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], val, false));
            stateWork(source,list[i]);
        }
    }

    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target,NumberData val,bool AsSkill = true)
    {
        source.playMoveTowardAnimation(target.unit_model.position);
        yield return new WaitForSeconds(moveTowardAndBackTime);

        if(source.unit_character_model.CurrentCharacterModel!=null)
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_attack, false);
        SLEffectManager.Instance.playCommonEffectNormalAttack(target.transform.position);
        yield return new WaitForSeconds(SDConstants.AnimTime_ATTACK - hitTime);
        int damage = val.DATA;
        if(val.dataTag == NumberData.DataType.percent)
        {
            damage = (int)(target.HpController.MaxHp * val.DECIMAL);
        }
        if (AsSkill)
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
        }
        else
        {
            Debug.Log(source.name + " casue " + damage + " damage to " + target.name);
            target.HpController.getDamage(damage, SkillKind.End);
        }

        yield return new WaitForSeconds(hitTime);
        //yield return new WaitForSeconds(skillLastTime);

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
