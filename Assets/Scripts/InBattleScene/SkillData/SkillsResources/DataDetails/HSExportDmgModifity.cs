using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSExportDmgModifity : MonoBehaviour
{
    #region 输出设置(全部列出)
    [Header("受施术者属性影响"), Space(25)]
    public int BCPerformDataUsingRA;//使用对应属性量()%
    public int PDUsingRA_PerLevel;//%
    public int PDUsingRA_PerSkillGrade;//%
    #endregion
    public int UsingRAShow(BattleRoleData source, AttributeData tag, int LEVEL, int SkillGrade)
    {
        int bc1
            = (int)(AllRandomSetClass.SimplePercentToDecimal(BCPerformDataUsingRA 
            + PDUsingRA_PerSkillGrade * SkillGrade
            + PDUsingRA_PerLevel * LEVEL)
            * source.ThisBasicRoleProperty().ReadRA(tag));//计算受source属性影响值
        return bc1;
    }
    public int AllResult(BattleRoleData source, BattleRoleData target, AttributeData tag, int SkillGrade)
    {
        int LEVEL = source.ThisBasicRoleProperty().LEVEL;
        return UsingRAShow(source, tag, LEVEL, SkillGrade);
    }
    public int AllResult(BattleRoleData source,BattleRoleData target,SkillKind tag,int SkillGrade)
    {
        AttributeData t = tag == SkillKind.Physical ? AttributeData.AT : AttributeData.MT;
        return AllResult(source, target, t, SkillGrade);
    }
}
