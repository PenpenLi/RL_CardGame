using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class HeroDetailPanel : BasicSubMenuPanel
{
    [HideInInspector]
    public int currentHeroHashcode;
    public SDHeroDetail detail;
    public SDEquipSelect equip;
    public SDHeroDeploySkills skill;
    public SDHeroImprove improve;
    public Transform aboveMenu;
    public enum RoleDetailSubType
    {
        heroDetail,
        heroEquip,
        heroImprove,
        heroSkill,
        heroInfor,
        end,
    }
    public RoleDetailSubType CurrentRDSubType = RoleDetailSubType.heroDetail;
    List<RoleDetailSubType> history = new List<RoleDetailSubType>();
    public Transform equipedSkillsPanel;
    public SkillDetailsList skillDetailList;
    public SkillSlot[] skillSlots;
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
        CurrentRDSubType = RoleDetailSubType.end;
        history.Clear();
        closeAllSubPanelTrans();
        BtnToHeroDetail();
    }


    #region roleDetailPanelSubMenu
    public Transform RDSubPanel(RoleDetailSubType RDType)
    {
        switch (RDType)
        {
            case RoleDetailSubType.heroDetail:
                return detail.transform;
            case RoleDetailSubType.heroEquip:
                return equip.transform;
            case RoleDetailSubType.heroSkill:
                return skill.transform;
            case RoleDetailSubType.heroImprove:
                return improve.transform;
            case RoleDetailSubType.end:
                return null;
            default: return detail.transform;
        }
    }
    public void closeAllSubPanelTrans()
    {
        detail.transform.gameObject.SetActive(false);
        equip.transform.gameObject.SetActive(false);
        skill.transform.gameObject.SetActive(false);
        improve.transform.gameObject.SetActive(false);
    }


    #region AboveMenu_LinkTo
    public void BtnToHeroDetail()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroDetail)
        {
            resetAllRDSubPanel();

            if(CurrentRDSubType != RoleDetailSubType.end)
            {
                UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
            }
            bool flag = false;
            if(CurrentRDSubType == RoleDetailSubType.heroSkill
                || CurrentRDSubType == RoleDetailSubType.heroEquip)
            {
                flag = true;
            }
            CurrentRDSubType = RoleDetailSubType.heroDetail;
            if (!flag)
            {
                UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));
            }
            historyAdd(CurrentRDSubType);

        }
    }
    public void BtnToHeroImprove()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroImprove)
        {
            resetAllRDSubPanel();

            if (CurrentRDSubType != RoleDetailSubType.end)
            {
                if (CurrentRDSubType == RoleDetailSubType.heroDetail)
                {
                    RDSubPanel(RoleDetailSubType.heroDetail).gameObject.SetActive(false);
                }
                else
                {
                    UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
                }
                if (CurrentRDSubType == RoleDetailSubType.heroSkill 
                    || CurrentRDSubType == RoleDetailSubType.heroEquip)
                {
                    //UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(RoleDetailSubType.heroDetail));
                    RDSubPanel(RoleDetailSubType.heroDetail).gameObject.SetActive(false);
                }


            }
            CurrentRDSubType = RoleDetailSubType.heroImprove;
            UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));
            historyAdd(CurrentRDSubType);

            //
            improve.InitImprovePanel();
        }
    }
    public void BtnToHeroEquip()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroEquip)
        {
            if (CurrentRDSubType != RoleDetailSubType.end
                && CurrentRDSubType != RoleDetailSubType.heroDetail)
            {
                UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
                UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(RoleDetailSubType.heroDetail));
            }
            CurrentRDSubType = RoleDetailSubType.heroEquip;
            UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));
            historyAdd(CurrentRDSubType);

            //
            equip.initPosEquipSelectPanel(equip.equipPos, equip.isSecondJewelryPos);
        }
    }
    public void BtnToHeroSkill()
    {
        if (CurrentRDSubType != RoleDetailSubType.heroSkill)
        {
            //resetAllRDSubPanel();
            if(CurrentRDSubType != RoleDetailSubType.end 
                && CurrentRDSubType != RoleDetailSubType.heroDetail)
            {
                UIEffectManager.Instance.hideAnimFadeOut(RDSubPanel(CurrentRDSubType));
                UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(RoleDetailSubType.heroDetail));
            }
            CurrentRDSubType = RoleDetailSubType.heroSkill;
            UIEffectManager.Instance.showAnimFadeIn(RDSubPanel(CurrentRDSubType));
            historyAdd(CurrentRDSubType);

            //
            skill.initHeroSkillListPanel();
        }
    }
    #endregion


    public void resetAllRDSubPanel()
    {
        improve.CloseThisPanel();
        //heroDetail.ModelAndEquipsPanel.gameObject.SetActive(false);
    }
    public void readHeroEquipedSkills(int heroHashcode)
    {
        for(int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].initSkillSlot(heroHashcode);
        }
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(heroHashcode);
        detail.skillid0 =hero.skill0Id;
        detail.skillid1 = hero.skill1Id;
        detail.skillidOmega = hero.skillOmegaId;

    }
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
            Debug.Log("HDP_Close_End");
            //homeScene.UseSMTToSubMenu(panelFrom);
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
