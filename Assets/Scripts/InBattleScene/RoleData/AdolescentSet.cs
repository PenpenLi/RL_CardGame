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
            float a = Evo * 0.5f / Accur;
            a = Mathf.Max(1 - a, SDConstants.AccurEvo_Min);
            return a;
        }
        else
        {
            return SDConstants.AccurEvo_Min;
        }
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

