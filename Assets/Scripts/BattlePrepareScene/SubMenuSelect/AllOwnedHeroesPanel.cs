using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AllOwnedHeroesPanel : MonoBehaviour
{
    public BasicHeroSelect BHS;
    public Transform RoleDetailPanel;
    public Transform RoleStagePanel;
    [Header("RoleDetailPanel__Content")]
    public SDHeroDetail heroDetail;
    public SDEquipSelect heroEquip;
    public SDHeroImprove heroImprove;
    public enum OwnedHeroPanelSubType
    {
        Stage,
        HeroDetail,

    }
    [Header("子页面索引器")]
    public OwnedHeroPanelSubType CurrentSubType = OwnedHeroPanelSubType.Stage;
    public enum RoleDetailSubType
    {
        heroDetail,
        heroEquip,
        heroImprove,
        heroSkill,
    }
    [Header("角色子页面内的子页面索引器")]
    public RoleDetailSubType CurrentRDSubType = RoleDetailSubType.heroDetail;
    void Start()
    {
        //ShowAllOwnedHeroes();
    }

    public void ShowAllOwnedHeroes()
    {
        SDGameManager.Instance.isSelectHero = true;
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.All;
        BHS.heroesInit();
        showStagePanel();
    }

    public void showStagePanel()
    {
        CurrentSubType = OwnedHeroPanelSubType.Stage;
        RoleStagePanel.gameObject.SetActive(true);
        RoleStagePanel.localScale = Vector3.one;
        if (RoleStagePanel.GetComponent<CanvasGroup>())
            RoleStagePanel.GetComponent<CanvasGroup>().alpha = 1;
        //RoleStagePanel.GetComponent<CanvasGroup>().alpha = 1;
        RoleDetailPanel.localScale = Vector3.zero;
        //RoleDetailPanel.GetComponent<CanvasGroup>().alpha = 0;
        BHS.gameObject.SetActive(true);

    }
    public void showRoleDetailPanel()
    {
        CurrentSubType = OwnedHeroPanelSubType.HeroDetail;
        RoleStagePanel.gameObject.SetActive(false);
        //RoleStagePanel.GetComponent<CanvasGroup>().alpha = 1;
        //RoleDetailPanel.localScale = Vector3.one;
        UIEffectManager.Instance.showAnimFadeIn(RoleDetailPanel);
        //RoleDetailPanel.GetComponent<CanvasGroup>().alpha = 0;
        BHS.gameObject.SetActive(false);

        //BtnToHeroDetail();
    }

    #region roleDetailPanelSubMenu
    public Transform RDSubPanel(RoleDetailSubType RDType)
    {
        switch (RDType)
        {
            case RoleDetailSubType.heroDetail:
                return heroDetail.transform;
            case RoleDetailSubType.heroEquip:
                return heroEquip.transform;
            case RoleDetailSubType.heroImprove:
                return heroImprove.transform;
            //case RoleDetailSubType.heroSkill:
                //return 
            default:return heroDetail.transform;
        }
    }
    public void BtnToHeroDetail()
    {
        if(CurrentRDSubType != RoleDetailSubType.heroDetail)
        {
            resetAllRDSubPanel();

            //RDSubPanel(CurrentRDSubType).DOScale(Vector3.zero, 0.2f);
            UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
            CurrentRDSubType = RoleDetailSubType.heroDetail;
            //RDSubPanel(CurrentRDSubType).DOScale(Vector3.one, 0.2f);
            UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));

            //heroDetail.ModelAndEquipsPanel.gameObject.SetActive(true);
        }
    }
    public void BtnToHeroImprove()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroImprove)
        {
            resetAllRDSubPanel();

            //RDSubPanel(CurrentRDSubType).DOScale(Vector3.zero, 0.2f);
            UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
            CurrentRDSubType = RoleDetailSubType.heroImprove;
            //RDSubPanel(CurrentRDSubType).DOScale(Vector3.one, 0.2f);
            UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));

            //
            heroImprove.initImprovePanel();
        }
    }
    public void BtnToHeroSkill()
    {
        if(CurrentRDSubType != RoleDetailSubType.heroSkill)
        {
            resetAllRDSubPanel();

            UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
            CurrentRDSubType = RoleDetailSubType.heroImprove;
            UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));


        }
    }
    public void resetAllRDSubPanel()
    {

        heroImprove.closeThisPanel();
        //heroDetail.ModelAndEquipsPanel.gameObject.SetActive(false);
    }
    #endregion
}
