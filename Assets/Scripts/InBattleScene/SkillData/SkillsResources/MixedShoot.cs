using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedShoot : SkillFunction
{
    [Header("DataDetail"), Space(25)]
    public NumberData physicalND = new NumberData(100, NumberData.DataType.percent);
    public NumberData elementalND = new NumberData(100, NumberData.DataType.percent);
    public NumberData arcaneND = new NumberData(50, NumberData.DataType.percent);

    public override void StartSkill(BattleRoleData source, BattleRoleData target)
    {
        base.StartSkill(source, target);
        IsProcessing = true;
        IsUsed = true;
        CalculateBeforeFunction(source, target);


        List<BattleRoleData> list = DealWithAOEAction(source, target,AOEType);
        for (int i = 0; i < list.Count; i++)
        {
            BattleRoleData T = list[i];
            StartCoroutine(IEStartSkill(source, T
                , valCaused(source, T, SkillKind.Physical)
                , valCaused(source, T, SkillKind.Elemental)
                , valCaused(source, T, SkillKind.Arcane))
                );
            stateWork(source, T);
        }


    }
    public IEnumerator IEStartSkill(BattleRoleData source, BattleRoleData target
        , int physicalVal, int elementalVal, int arcaneVal)
    {
        source.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (source.unit_character_model.CurrentCharacterModel.anim_cast, false);
        SLEffectManager.Instance.playCommonEffectCast(source.transform.position);
        yield return new WaitForSeconds(castLastTime);

        source.playBulletCastAnimation(bullet, source.unit_model.position, target.unit_model.position);
        yield return new WaitForSeconds(bulletLastTime);
        int d0 = physicalVal;
        int d1 = elementalVal;
        int d2 = arcaneVal;
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
            d0 = (int)(d0 * criD);
            d1 = (int)(d1 * criD);
            d2 = (int)(d2 * criD);
        }
        #endregion
        #region Expect
        int NowExpect = (int)(UnityEngine.Random.Range(-1f, 1f) * source.ExpectResult);
        d0 += NowExpect;
        NowExpect = (int)(UnityEngine.Random.Range(-1f, 1f) * source.ExpectResult);
        d1 += NowExpect;
        NowExpect = (int)(UnityEngine.Random.Range(-1f, 1f) * source.ExpectResult);
        d2 += NowExpect;
        #endregion
        #region 传输技能状态
        target.HpController.isCriDmg = IsCausedCritDmg;
        target.HpController.isEvoHappen = IsCausedMiss;
        target.HpController.isFault = IsCausedFault;
        #endregion
        float DECIMAL = AllRandomSetClass.SimplePercentToDecimal(100 + target.PhysModify);
        d0 = (int)(d0 * DECIMAL);
        d1 = (int)(d1 * DECIMAL);
        d2 = (int)(d2 * DECIMAL);

        Debug.Log(source.name + " slash cause "
            + d0 + " physical damage "
            + d1 + " elemental damage "
            + d2 + " arcane damage"
            + " to " + target.name);
        target.HpController.getDamage(d0, SkillKind.Physical);
        target.HpController.getDamage(d1, SkillKind.Elemental);
        target.HpController.getDamage(d2, SkillKind.Arcane);

        yield return new WaitForSeconds(skillLastTime);
        StartCoroutine(IEWaitForEnd(source));
    }

    public int valCaused(BattleRoleData source, BattleRoleData target, SkillKind kind)
    {
        int B = 0;
        NumberData d = physicalND;
        AttributeData atb = AttributeData.AT;
        if (kind == SkillKind.Elemental)
        {
            atb = AttributeData.MT;
            d = elementalND;
        }
        else if (kind == SkillKind.Physical)
        {
            atb = AttributeData.AT;
            d = physicalND;
        }
        else if (kind == SkillKind.Arcane)
        {
            d = arcaneND;
        }
        if (physicalND.dataTag == NumberData.DataType.percent)
        {
            for (int i = 0; i < SkillGrade; i++)
            {
                d.DATA += 10;
            }

            HSkilInfo hsi = GetComponent<HSkilInfo>();
            if (hsi && hsi.UseAppointedAtb)
            {
                atb = hsi.Atb;
            }
            else
            {
                if (kind == SkillKind.Arcane)
                {
                    B = (int)((source.ThisBasicRoleProperty().ReadRA(AttributeData.MT)
                        + source.ThisBasicRoleProperty().ReadRA(AttributeData.AT)) / 2
                        * d.DECIMAL);
                }
            }
            B = (int)(source.ThisBasicRoleProperty().ReadRA(atb) * d.DECIMAL);
        }
        else
        {
            B = physicalND.DATA;
        }
        if (HSModifity)
        {
            B = HSModifity.AllResult(source, target, kind, SkillGrade);
        }
        B = ValChangedByCharacterType(source, target, B);
        return B;
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
