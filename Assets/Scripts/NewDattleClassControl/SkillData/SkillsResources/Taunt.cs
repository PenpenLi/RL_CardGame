using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : SkillFunction
{
    [Space(100)]
    public int TauntBuff;//+-()%
    public StateTag TauntStateTag;
    public int TauntLastTime;
    public int debuffMaxPc = 15;
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

        StartCoroutine(IEStartSkill(source, target, tauntCaused(source, target), true));

    }

    public IEnumerator IEStartSkill(BattleRoleData source,BattleRoleData target,int val,bool haveDebuff = false)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim(SDConstants.AnimName_CAST, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        Debug.Log(source.name + " add " + val + " taunt to " + target.name);
        target.ControlStateVisual(TauntStateTag, true, TauntLastTime);
        target.ReadThisStateEffect(TauntStateTag).UniqueEffectInRA.Taunt += val;
        if (haveDebuff)
        {
            target.ReadThisStateEffect(TauntStateTag).UniqueEffectInRA += debuffList(target);
        }

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForEnd(source));
    }
    public int tauntCaused(BattleRoleData source,BattleRoleData target)
    {
        HSExportDmgModifity S = GetComponent<HSExportDmgModifity>();
        if (S)
        {
            return S.AllResult(source, target, AttributeData.MT, SkillGrade);
        }
        else
        {
            int t = target.ThisBasicRoleProperty().ReadRA(AttributeData.Taunt);
            int tbuff = TauntBuff + (int)(TauntBuff*(1f / SDConstants.SkillMaxGrade * SkillGrade / 2))
                +(int)(TauntBuff*(1f/SDConstants.UnitMAxStarNum * source.ThisBasicRoleProperty().LEVEL / 2));
            t = (int)(t * AllRandomSetClass.SimplePercentToDecimal(tbuff));
            return t;
        }
    }
    public RoleAttributeList debuffList(BattleRoleData target)
    {
        RoleAttributeList list = new RoleAttributeList();
        list.AD = -debuffCaused(target, AttributeData.AD);
        list.MD = -debuffCaused(target, AttributeData.MD);
        return list;
    }
    public int debuffCaused(BattleRoleData target , AttributeData tag)
    {
        int debuff = debuffMaxPc - SkillGrade * debuffMaxPc / SDConstants.SkillMaxGrade;
        int aim = target.ThisBasicRoleProperty()._role.ReadCurrentRoleRA(tag);
        int d = (int)(aim * AllRandomSetClass.SimplePercentToDecimal(debuff));
        return d;
    }
    public int debuffCaused(BattleRoleData target , StateTag tag)
    {
        int debuff = debuffMaxPc - SkillGrade * debuffMaxPc / SDConstants.SkillMaxGrade;
        int aim = target.ThisBasicRoleProperty()._role.ReadCurrentRoleRA(tag);
        int d = (int)(aim * AllRandomSetClass.SimplePercentToDecimal(debuff));
        return d;
    }
    public override void EndSkill()
    {
        base.EndSkill();
    }
}
