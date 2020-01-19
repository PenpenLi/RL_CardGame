using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDHeroBattleStatus : MonoBehaviour
{
    #region 角色细节
    [Header("角色细节信息")]
    public Text heroNameText;
    public Text lvText;
    public Transform expSliderImg;
    public Image heroJob;
    public Image heroRace;
    [Space(10)]
    public Image heroFaceImg;
    public Image heroFaceBgImg;
    [HideInInspector] public BattleRoleData heroUnit;
    [HideInInspector] public HP_Controller HpC;
    #endregion
    [Header("装备简易列表显示")]
    public RectTransform EquipArrayContent;
    public void BuildThisStatusVision(BattleRoleData unit,HP_Controller hpC)
    {
        heroUnit = unit;
        HpC = hpC;

        heroNameText.text = heroUnit.ThisBasicRoleProperty().Name;


        RefreshHeroStatus();
    }
    public void RefreshHeroStatus()
    {
        ExpVision();
    }
    public void ExpVision()
    {
        //int exp = heroUnit.ThisBasicRoleProperty().
        if(!heroUnit.IsEnemy)
            lvText.text = "Lv."+heroUnit.HeroProperty._hero.grade;
    }
    
    public void SelectThisHero()
    {

    }


}
