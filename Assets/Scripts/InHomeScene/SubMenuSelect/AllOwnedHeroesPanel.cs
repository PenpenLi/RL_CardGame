﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AllOwnedHeroesPanel : BasicSubMenuPanel
{
    public HEWPageController PAGE;
    public Transform RoleDetailPanel;
    public Transform RoleStagePanel;
    [Header("RoleDetailPanel__Content")]
    public HeroDetailPanel detailpanel;
    public enum panelContent
    {
        main, end
    }
    public panelContent currentPanelContent = panelContent.end;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        ShowAllOwnedHeroes();
    }
    public void ShowAllOwnedHeroes()
    {
        currentPanelContent = panelContent.main;
        //SDGameManager.Instance.isSelectHero = true;
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.All;
        UIEffectManager.Instance.showAnimFadeIn(RoleStagePanel);
        PAGE.ItemsInit(SDConstants.ItemType.Hero);
    }

    public override void commonBackAction()
    {   
        if(currentPanelContent == panelContent.main)
        {
            
        }
        else if(currentPanelContent == panelContent.end)
        {

        }
        base.commonBackAction();
        PAGE.ResetPage();
        homeScene.SubMenuClose();
    }
}
