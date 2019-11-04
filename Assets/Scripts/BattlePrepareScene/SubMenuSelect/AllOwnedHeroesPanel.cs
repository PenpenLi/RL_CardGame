using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AllOwnedHeroesPanel : MonoBehaviour
{
    HomeScene _hs;
    public HomeScene homeScene
    {
        get
        {
            if (_hs == null) _hs = FindObjectOfType<HomeScene>();
            return _hs;
        }

    }
    public BasicHeroSelect BHS;
    public Transform RoleDetailPanel;
    public Transform RoleStagePanel;
    [Header("RoleDetailPanel__Content")]
    public HeroDetailPanel detailpanel;
    public enum panelContent
    {
        main, end
    }
    public panelContent currentPanelContent = panelContent.end;
    public void ShowAllOwnedHeroes()
    {
        currentPanelContent = panelContent.main;
        SDGameManager.Instance.isSelectHero = true;
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.All;
        UIEffectManager.Instance.showAnimFadeIn(RoleStagePanel);
        BHS.gameObject.SetActive(true);
        BHS.heroesInit();
    }

    public void commonBackAction()
    {
        if(currentPanelContent == panelContent.main)
        {
            homeScene.SubMenuClose();
        }
        else if(currentPanelContent == panelContent.end)
        {

        }
    }
}
