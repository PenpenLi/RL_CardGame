using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using DG.Tweening;

public class HeroDetailPanel : BasicSubMenuPanel
{
    [HideInInspector]
    public int currentHeroHashcode;
    [Space(20)]
    public SDHeroDetail detail;
    public SDEquipSelect equip;
    public SDHeroDeploySkills skill;
    public SDHeroImprove improve;
    public enum RoleDetailSubType
    {
        heroDetail,
        heroEquip,
        heroImprove,
        heroSkill,
        HeroWakeup,
        heroInfor,
        end,
    }
    private RoleDetailSubType _currentRDSubType = RoleDetailSubType.end;
    public RoleDetailSubType CurrentRDSubType
    {
        get { return _currentRDSubType; }
        set
        {
            SubPanelChangeAnim(_currentRDSubType, value);
            _currentRDSubType = value;
        }
    }
    List<RoleDetailSubType> history = new List<RoleDetailSubType>();
    public Transform equipedSkillsPanel;
    public SkillDetailsList skillDetailList;
    [Header("SubPanels")]
    public Transform abovePanelPlace;
    public Transform ap_equipAndIdentitySubPanel;
    public Transform ap_heroImproveConfirmSubPanel;
    public Transform ap_heroWakeupFaceSubPanel;
    [Space(15)]
    public Transform belowPanelPlace;
    public Transform bp_RAlAndImgSubPanel;
    public Transform bp_equipListSubPanel;
    public Transform bp_skillDetailSubPanel;
    public Transform bp_heroImproveMaterialListSubPanel;
    public Transform bp_heroWakeupConfirmSubPanel;
    //
    private float ChangeSubPanelIndetval = 0.15f;
    private float MoveDisRate = 1.15f;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        if(transform.localScale==Vector3.zero|| !gameObject.activeSelf)
            UIEffectManager.Instance.showAnimFadeIn(transform);
        homeScene.CurrentSubMenuType = HomeScene.HomeSceneSubMenu.HeroDetails;
        //
        detail.gameObject.SetActive(true);
        detail.initHeroDetailPanel(currentHeroHashcode);
        //
        _currentRDSubType = RoleDetailSubType.end;
        history.Clear();
        BtnToHeroDetail();
    }


    #region roleDetailPanelSubMenu

    #region UpperBtnList_LinkTo
    public void BtnToHeroDetail()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroDetail)
        {
            resetAllRDSubPanel();
            CurrentRDSubType = RoleDetailSubType.heroDetail;
            historyAdd(CurrentRDSubType);

        }
    }
    public void BtnToHeroImprove()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroImprove)
        {
            resetAllRDSubPanel();
            CurrentRDSubType = RoleDetailSubType.heroImprove;
            historyAdd(CurrentRDSubType);

            //
            improve.InitImprovePanel();
        }
    }
    public void BtnToHeroEquip()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroEquip)
        {
            resetAllRDSubPanel();
            CurrentRDSubType = RoleDetailSubType.heroEquip;
            historyAdd(CurrentRDSubType);

            //
            equip.initPosEquipSelectPanel(equip.equipPos, equip.isSecondJewelryPos);
        }
    }
    public void BtnToHeroSkill()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroSkill)
        {
            resetAllRDSubPanel();
            CurrentRDSubType = RoleDetailSubType.heroSkill;
            historyAdd(CurrentRDSubType);

            //
            skill.initHeroSkillListPanel();
        }
    }
    public void BtnToHeroWakeup()
    {
        if(CurrentRDSubType != RoleDetailSubType.HeroWakeup)
        {
            resetAllRDSubPanel();
            CurrentRDSubType = RoleDetailSubType.HeroWakeup;
            historyAdd(CurrentRDSubType);

            //
            
        }
    }
    #endregion


    public void resetAllRDSubPanel()
    {
        improve.CloseThisPanel();
    }
    #region SubPanelAnim
    public RectTransform SubPanel_above(RoleDetailSubType type)
    {
        if(type == RoleDetailSubType.heroDetail 
            || type == RoleDetailSubType.heroEquip
            || type == RoleDetailSubType.heroSkill)
        {
            return ap_equipAndIdentitySubPanel.GetComponent<RectTransform>();
        }
        else if(type == RoleDetailSubType.heroImprove)
        {
            return ap_heroImproveConfirmSubPanel.GetComponent<RectTransform>();
        }
        else if(type == RoleDetailSubType.HeroWakeup)
        {
            return ap_heroWakeupFaceSubPanel.GetComponent<RectTransform>();
        }
        //
        return null;
    }
    public RectTransform SubPanel_below(RoleDetailSubType type)
    {
        if(type == RoleDetailSubType.heroDetail)
        {
            return bp_RAlAndImgSubPanel.GetComponent<RectTransform>();
        }
        else if(type == RoleDetailSubType.heroEquip)
        {
            return bp_equipListSubPanel.GetComponent<RectTransform>();
        }
        else if(type == RoleDetailSubType.heroSkill)
        {
            return bp_skillDetailSubPanel.GetComponent<RectTransform>();
        }
        else if(type == RoleDetailSubType.heroImprove)
        {
            return bp_heroImproveMaterialListSubPanel.GetComponent<RectTransform>();
        }
        else if(type == RoleDetailSubType.HeroWakeup)
        {
            return bp_heroWakeupConfirmSubPanel.GetComponent<RectTransform>();
        }

        //
        return null;
    }
    void SubPanelChangeAnim(RoleDetailSubType oldType,RoleDetailSubType newType)
    {
        if (oldType == newType) return;
        RectTransform a0 = SubPanel_above(oldType);
        RectTransform a1 = SubPanel_above(newType);
        RectTransform b0 = SubPanel_below(oldType);
        RectTransform b1 = SubPanel_below(newType);
        if(oldType == RoleDetailSubType.end || newType == RoleDetailSubType.end)
        {
            RectTransform baseT_a = SubPanel_above(RoleDetailSubType.heroDetail);
            baseT_a.anchoredPosition = Vector2.zero;
            RectTransform baseT_b = SubPanel_below(RoleDetailSubType.heroDetail);
            baseT_b.anchoredPosition = Vector2.zero;
            for (int i = 0; i < (int)RoleDetailSubType.end; i++)
            {
                RoleDetailSubType T = (RoleDetailSubType)i;
                RectTransform a = SubPanel_above(T);
                RectTransform b = SubPanel_below(T);
                if (a != baseT_a && a!=null)
                {
                    a.anchoredPosition = new Vector2(a.sizeDelta.x * MoveDisRate, 0);
                }
                if(b != baseT_b && b!= null)
                {
                    b.anchoredPosition = new Vector2(-b.sizeDelta.x * MoveDisRate, 0);
                }
            }
            return;
        }
        if(a0 != a1 && a0!=null && a1!=null)
        {
            a0.DOAnchorPosX(a0.sizeDelta.x * MoveDisRate, ChangeSubPanelIndetval);
            a1.DOAnchorPosX(0, ChangeSubPanelIndetval).SetEase(Ease.OutBack).SetUpdate(true);
        }
        if(b0 != b1 && b0!=null && b1!=null)
        {
            b0.DOAnchorPosX(-b0.sizeDelta.x * MoveDisRate, ChangeSubPanelIndetval);
            b1.DOAnchorPosX(0,ChangeSubPanelIndetval).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    #endregion
    #endregion

    public void historyAdd(RoleDetailSubType type)
    {
        if(history == null)
        {
            history = new List<RoleDetailSubType>();
        }
        if (history.Count > 5)
        {
            history.RemoveAt(0);
        }
        history.Add(type);
    }



    public override void commonBackAction()
    {       
        RoleDetailSubType lastOne;
        if (history.Count > 0)
        {
            history.RemoveAt(history.Count - 1);
            if (history.Count > 0)
            {
                lastOne = history[history.Count - 1];
            }
            else lastOne = RoleDetailSubType.end;
        }
        else lastOne = RoleDetailSubType.end;

        if (lastOne == RoleDetailSubType.end)
        {
            base.commonBackAction();
            homeScene.SubMenuClose();
            homeScene.CurrentSubMenuType = panelFrom;
            CurrentRDSubType = RoleDetailSubType.end;
            history.Clear();
        }
        else
        {
            if (lastOne == RoleDetailSubType.heroDetail)
            {
                BtnToHeroDetail();
            }
            else if (lastOne == RoleDetailSubType.heroImprove)
            {
                BtnToHeroImprove();
            }
            else if (lastOne == RoleDetailSubType.heroSkill)
            {
                BtnToHeroSkill();
            }
            else if (lastOne == RoleDetailSubType.heroEquip)
            {

            }
            else if (lastOne == RoleDetailSubType.heroInfor)
            {

            }
            if (history.Count > 0) { history.RemoveAt(history.Count - 1); }
        }
    }
}
