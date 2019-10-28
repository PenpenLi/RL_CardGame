using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Shoot : SkillFunction
{
    [Space(100)]
    public AttributeData _skillUseRA;
    public SkillKind _skillKind { get { if (GetComponent<HSkilInfo>())
            {
                return GetComponent<HSkilInfo>().kind;
            }
            return SkillKind.Physical;
        } }
    protected SkillBreed _skillBreed = SkillBreed.Remote;
    [Header("ExtraStateAdd"),Space(25)]
    public StandardState _standardState;
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
        UseSkillAddMpTp(source, GetComponent<HSkilInfo>().AfterwardsAddType);

        List<BattleRoleData> list = DealWithAOEAction(source, target);
        for(int i = 0; i < list.Count; i++)
        {
            StartCoroutine(IEStartSkill(source, list[i], damageCaused(source, list[i])));
            if (_standardState)
            {
                _standardState.StartState(this, source, list[i]
                    , damageCaused(source,target) / 5 );
            }
        }

    }
    public IEnumerator IEStartSkill(BattleRoleData source, BattleRoleData target, int dmgVal)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_CAST, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        source.playBulletCastAnimation(bullet, source.transform.position, target.transform.position);
        yield return new WaitForSeconds(bulletLastTime);
        
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
        dmgVal = (int)(dmgVal * criD);
        dmgVal += NowExpect;
        Debug.Log(source.name + "shoot cause" + dmgVal + " damage to " + target.name);
        //source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_IDLE, true);
        #region 传输技能状态
        target.HpController.isCriDmg = IsCausedCritDmg;
        target.HpController.isEvoHappen = IsCausedMiss;
        target.HpController.isFault = IsCausedFault;
        #endregion
        dmgVal = (int)(dmgVal * AllRandomSetClass.SimplePercentToDecimal(100 + target.PhysModify));
        target.HpController.getDamage(dmgVal, _skillKind);

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForEnd(source));
    }
    public int damageCaused(BattleRoleData source,BattleRoleData target)
    {
        int atk = source.ThisBasicRoleProperty().ReadRA(_skillUseRA);
        HSExportDmgModifity _S = GetComponent<HSExportDmgModifity>();
        AttributeData tag = _skillUseRA;
        if (_S)
        {
            atk = _S.AllResult(source, target, tag, SkillGrade);
        }

        return atk;
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }
    public override bool isSkillMeetConditionToAutoRelease()
    {
        //return base.isSkillMeetConditionToAutoRelease();
        return true;
    }


}
