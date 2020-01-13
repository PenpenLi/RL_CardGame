using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : BasicRoleProperty
{
    public SDHero _hero;
    #region 装备
    public Helmet _helmet;
    public Breastplate _breastplate;
    public Gardebras _gardebras;//已舍弃
    public Legging _legging;
    public Jewelry _jewelry0;
    public Jewelry _jewelry1;

    public SDWeapon _weapon;
    #endregion
    public List<string> _equipedSkills;

    //将英雄本身数据加上装备数据
    public void InitHeroBasicProperties()
    {
        if (_hero)
        {
            this.RoleBasicRA = _hero.RoleBasicRA.Clone;
            this.CRIDmg = _hero.CRIDmg;
            this.DmgReduction = _hero.DmgReduction;
            this.DmgReflection = _hero.DmgReflection;
            this.RewardRate = _hero.RewardRate;
            this.BarChartRegendPerTurn = _hero.BarChartRegendPerTurn;

            this.ID = _hero.ID;
            this.Name = _hero.Name;
            this.LEVEL = _hero.LEVEL;
        }
        if (_helmet)
        {
            this.RoleBasicRA += _helmet.RoleBasicRA.Clone;
            this.CRIDmg += _helmet.CRIDmg;
            this.DmgReduction += _helmet.DmgReduction;
            this.DmgReflection += _helmet.DmgReflection;
            this.RewardRate += _helmet.RewardRate;
            this.BarChartRegendPerTurn += _helmet.BarChartRegendPerTurn;
        }
        if (_breastplate)
        {
            this.RoleBasicRA += _breastplate.RoleBasicRA.Clone;
            this.CRIDmg += _breastplate.CRIDmg;
            this.DmgReduction += _breastplate.DmgReduction;
            this.DmgReflection += _breastplate.DmgReflection;
            this.RewardRate += _breastplate.RewardRate;
            this.BarChartRegendPerTurn += _breastplate.BarChartRegendPerTurn;
        }
        if (_gardebras)
        {
            this.RoleBasicRA += _gardebras.RoleBasicRA.Clone;
            this.CRIDmg += _gardebras.CRIDmg;
            this.DmgReduction += _gardebras.DmgReduction;
            this.DmgReflection += _gardebras.DmgReflection;
            this.RewardRate += _gardebras.RewardRate;
            this.BarChartRegendPerTurn += _gardebras.BarChartRegendPerTurn;
        }
        if (_legging)
        {
            this.RoleBasicRA += _legging.RoleBasicRA.Clone;
            this.CRIDmg += _legging.CRIDmg;
            this.DmgReduction += _legging.DmgReduction;
            this.DmgReflection += _legging.DmgReflection;
            this.RewardRate += _legging.RewardRate;
            this.BarChartRegendPerTurn += _legging.BarChartRegendPerTurn;
        }
        if (_jewelry0)
        {
            this.RoleBasicRA += _jewelry0.RoleBasicRA.Clone;
            this.CRIDmg += _jewelry0.CRIDmg;
            this.DmgReduction += _jewelry0.DmgReduction;
            this.DmgReflection += _jewelry0.DmgReflection;
            this.RewardRate += _jewelry0.RewardRate;
            this.BarChartRegendPerTurn += _jewelry0.BarChartRegendPerTurn;
        }
        if (_jewelry1)
        {
            this.RoleBasicRA += _jewelry1.RoleBasicRA.Clone;
            this.CRIDmg += _jewelry1.CRIDmg;
            this.DmgReduction += _jewelry1.DmgReduction;
            this.DmgReflection += _jewelry1.DmgReflection;
            this.RewardRate += _jewelry1.RewardRate;
            this.BarChartRegendPerTurn += _jewelry1.BarChartRegendPerTurn;
        }
        if (_weapon)
        {
            this.RoleBasicRA += _weapon.RoleBasicRA.Clone;
            this.CRIDmg += _weapon.CRIDmg;
            this.DmgReduction += _weapon.DmgReduction;
            this.DmgReflection += _weapon.DmgReflection;
            this.RewardRate += _weapon.RewardRate;
            this.BarChartRegendPerTurn += _weapon.BarChartRegendPerTurn;
        }
        initRoleClassData();
    }

}
