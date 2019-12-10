using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDArmor : BasicRoleProperty
{
    public int RecommendedGrade;//推荐等级
    //public int Durability;//装备耐久
    public SDConstants.ArmorType armorType = SDConstants.ArmorType.light;
    public int grade;//装备当前强化等级
    private void Start()
    {
        
    }
    public void initArmor(int level,RoleAttributeList dataRA
        ,RoleAttributeList rateRA,int cri,int criDmg,int dmgReduction,int dmgReflection
        ,int RewardRate, RoleBarChart bcRegendPerTurn, string id, string name
        , int wakeNum, int grade)
    {
        initData(level
            , dataRA, rateRA, cri, criDmg, dmgReduction, dmgReflection, RewardRate
            , bcRegendPerTurn, id, name, wakeNum);
        AddMultiplier(grade);

    }
    public override void initData(int level
        , RoleAttributeList dataRA, RoleAttributeList rateRA
        , int cri, int criDmg, int dmgReduction, int dmgReflection, int RewardRate
        , RoleBarChart bcRegendPerTurn, string id, string name
        , int wakeNum)
    {
        base.initData(level
            , dataRA, rateRA, cri, criDmg, dmgReduction, dmgReflection, RewardRate
            , bcRegendPerTurn, id, name, wakeNum);
    }
    public void initGradeShow(int grade)
    {
        this.grade = grade;
        AddMultiplier(this.grade);
    }
}
public class Helmet : SDArmor
{
    protected EquipPosition equipPos = EquipPosition.Head;
}
public class Breastplate : SDArmor
{
    protected EquipPosition equipPos = EquipPosition.Breast;
}
public class Gardebras : SDArmor
{
    protected EquipPosition equipPos = EquipPosition.Arm;
}
public class Legging : SDArmor
{
    protected EquipPosition equipPos = EquipPosition.Leg;
}




public class Jewelry : BasicRoleProperty
{
    public int RecommendedGrade;//推荐等级
    public bool ShowState;
    protected EquipPosition equipPos = EquipPosition.Finger;
    public SDConstants.JewelryType _jewelryType;
    public int grade;
    private void Start()
    {
        
    }
    public void initGradeShow(int grade)
    {
        this.grade = grade;
        AddMultiplier(this.grade);
    }
}