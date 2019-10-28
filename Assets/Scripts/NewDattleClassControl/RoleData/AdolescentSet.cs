using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdolescentSet : MonoBehaviour
{
    /// <summary>
    /// 光照适应性
    /// </summary>
    public enum BeamAdaptablity
    {
        None = 0,
        AdaptDayTime,
        AdaptNightTime,
    }
    /// <summary>
    /// 角色成长公式
    /// </summary>
    /// <param name="Level">等级</param>
    /// <param name="hpUp">生命+</param>
    /// <param name="spUp">护盾+</param>
    /// <param name="mpUp">法力+</param>
    /// <param name="tpUp">怒气+</param>
    /// <param name="atUp">物伤+</param>
    /// <param name="adUp">物防+</param>
    /// <param name="mtUp">法伤+</param>
    /// <param name="mdUp">法防+</param>
    /// <returns></returns>
    public static RoleAttributeList AttritubeShowByLevel_basic(int Level
        ,float hpUp,float mpUp, float tpUp
        ,float atUp,float adUp,float mtUp,float mdUp)
    {
        RoleAttributeList A = new RoleAttributeList();
        #region 随等级提升基础属性
        A.Hp += (int)(Level * hpUp);//生命
        A.Mp += (int)(Level * mpUp);//法力
        A.Tp += (int)(Level * tpUp);//怒气

        A.AT += (int)(Level * atUp);//物攻
        A.MT += (int)(Level * mtUp);//魔攻
        A.AD += (int)(Level * adUp);//物防
        A.MD += (int)(Level * mdUp);//魔防
        #endregion
        return A;
    }

    /// <summary>
    /// 整数算法：整除
    /// </summary>
    /// <param name="N">被除数</param>
    /// <param name="DivideN">除数</param>
    /// <returns></returns>
    public static int DividedIntger(int N,int DivideN)
    {
        return (N - N % DivideN) / DivideN;
    }
    /// <summary>
    /// 暴击算法:无限趋近100%
    /// </summary>
    /// <param name="Crit">爆击值</param>
    /// <returns></returns>
    public static float CritFunction(int Crit)
    {
        return Crit * 1f / (SDConstants.CritConstNumber + Crit);
    }
    /// <summary>
    /// 精准算法
    /// </summary>
    /// <param name="Accur">精准值</param>
    /// <param name="Evo">闪避值</param>
    /// <returns></returns>
    public static float AccurFunction(int Accur, int Evo)
    {
        if (Accur > 0)
        {
            float a = Evo * 1f / Accur;
            a = Mathf.Max(1 - a, SDConstants.AccurEvo_Min);
            return a;
        }
        else
        {
            return SDConstants.AccurEvo_Min;
        }
    }
    /// <summary>
    /// 种族加成
    /// </summary>
    /// <param name="RoleRace"></param>
    /// <returns></returns>
    public static RoleSelfAbility AbilityByRace(Race RoleRace)
    {
        RoleSelfAbility A = new RoleSelfAbility(0,0,0,0,0,Vector2.zero);
        switch (RoleRace)
        {
            case Race.Human:
                //人类无特征
                break;
            case Race.Elf:
                A.Dex += 2;
                A.Per += 2;
                A.Str += -4;
                break;
            case Race.Dragonborn:
                A.Dur += 2;
                A.Wis += 2;
                A.Dex += -2;
                A.Per += -2;
                break;
        }
        return A;
    }
    /// <summary>
    /// 角色基础属性换算为战斗属性
    /// </summary>
    /// <param name="Ability"></param>
    /// <returns></returns>
    public static RoleAttributeList AttritubeFromAbility(RoleSelfAbility Ability)
    {
        RoleAttributeList A = new RoleAttributeList();
        A.Hp = Ability.IntgerFromAbility(1, 0.2f, 0.2f, 0.2f, 0.4f) * 3;
        A.Mp = Ability.IntgerFromAbility(0, 0, 0.5f, 0f, 0.5f) * 3;
        A.AT = Ability.IntgerFromAbility(0.2f, 0.8f, 0.2f, 0.8f, 0.2f);
        A.AD = Ability.IntgerFromAbility(0.5f, 0.8f, 0.2f, 0.2f, 0.5f);
        A.MT = Ability.IntgerFromAbility(0.2f, 0.2f, 0.8f, 0.2f, 0.8f);
        A.MD = Ability.IntgerFromAbility(0.5f, 0.2f, 0.8f, 0.2f, 0.5f);
        //
        A.Speed = Ability.IntgerFromAbility(-0.1f, 0.1f, -0.1f, 0.2f, -0.1f);
        A.Taunt = Ability.IntgerFromAbility(0.1f, 0.2f, -0.2f, 0.1f, -0.1f);
        A.Accur = Ability.IntgerFromAbility(0,0,0.3f,0.3f,0.3f);
        A.Evo = Ability.IntgerFromAbility(-0.1f, -0.1f, 0.2f, 0.6f, 0.2f);
        A.Crit = Ability.IntgerFromAbility(0, 0.1f, 0.1f, 0.1f, 0);
        A.Expect = Ability.IntgerFromAbility(0.1f, 0.1f, 0.1f, 0.1f, -0.4f);

        //
        A.Bleed_Def = Ability.IntgerFromAbility(0.1f, 0.05f, 0, 0, 0);
        return A;
    }
    /// <summary>
    /// 全战斗属性百分比增减+-()%
    /// </summary>
    /// <param name="ForwardRA">原属性</param>
    /// <param name="Percent">增加或减少的百分比</param>
    /// <returns></returns>
    public static RoleAttributeList RAChangeInPercent(RoleAttributeList ForwardRA,int Percent)
    {
        float Decimal = AllRandomSetClass.SimplePercentToDecimal(Percent);
        RoleAttributeList RA = ForwardRA;
        for(int i = 0; i < (int)AttributeData.End; i++)
        {
            AttributeData Tag = (AttributeData)i;
            RA.Add( (int)(RA.read(Tag)*Decimal) ,Tag);
        }
        for(int i = 0; i < (int)StateTag.End; i++)
        {
            StateTag Tag = (StateTag)i;
            RA.Add((int)(RA.read(Tag) * Decimal), Tag);
        }
        return RA;
    }

}

