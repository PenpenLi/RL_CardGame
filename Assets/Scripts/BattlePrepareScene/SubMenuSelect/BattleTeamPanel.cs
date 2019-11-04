using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using UnityEngine.EventSystems;

public class BattleTeamPanel : MonoBehaviour
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
    public SDHeroSelect HS;
    [Space(25)]
    public Transform SelectUnitTeamPanel;
    //public Transform OneTeam;
    public ScrollRect TeamsScrolrect;
    public OneUnitTeam CurrentTeamVision;
    public Transform TeamListParent;
    //public List<OneUnitTeam> Teams;
    [Space(10)]
    public Transform EditUnitTeamPanel;
    public enum panelContent
    {
        main,edit,end
    }
    public panelContent currentPanelContent = panelContent.end;
    //public bool isFromLevelEnterPanel;
    public HomeScene.HomeSceneSubMenu panelFrom = HomeScene.HomeSceneSubMenu.End;

    public void WhenOpenThisMenu()
    {
        currentPanelContent = panelContent.main;
        BasicHeroSelect bhs = HS.heroesSelectPanel.GetComponent<BasicHeroSelect>();
        bhs.pageController.scrollRectReset();
        if (SDGameManager.Instance.currentHeroTeamIndex == 0)
        {
            SDGameManager.Instance.currentHeroTeamIndex = 1;
        }
        OpenSelectUnitTeamPanel(SDGameManager.Instance.currentHeroTeamIndex);
        EditUnitTeamPanel.localScale = Vector3.zero;
        showTeamId(SDGameManager.Instance.currentHeroTeamIndex);
    }
    public void OpenSelectUnitTeamPanel(int teamId)
    {
        if (SDDataManager.Instance.getHeroGroup().Count != SDConstants.MaxBattleTeamNum)
        {
            SDDataManager.Instance.getHeroGroup().Clear();
            for (int i = 0; i < SDConstants.MaxBattleTeamNum; i++)
            {
                GDEunitTeamData T = new GDEunitTeamData(GDEItemKeys.unitTeam_emptyHeroTeam)
                {
                    id = i + 1,
                    heroes = new List<int>() { 0, 0, 0, 0 }
                };
                SDDataManager.Instance.PlayerData.heroesTeam.Add(T);
                SDDataManager.Instance.PlayerData.Set_heroesTeam();
            }
        }
        //Debug.Log("HeroesTeamCount: " + SDDataManager.Instance.PlayerData.heroesTeam.Count);
        GDEunitTeamData Team = SDDataManager.Instance.getHeroTeamByTeamId(teamId);
        CurrentTeamVision.initThisUnitTeam(Team);
        showTHisUnitTeamOtherData(Team);
    }
    public void showTHisUnitTeamOtherData(GDEunitTeamData team)
    {

    }
    public void openEditUnitTeamPanel(int teamId)
    {
        currentPanelContent = panelContent.edit;
        SelectTeamUnitPanel STUP = EditUnitTeamPanel.GetComponent<SelectTeamUnitPanel>();
        STUP.CurrentTeamId = teamId;
        STUP.whenOpenThisPanel();
        UIEffectManager.Instance.showAnimFadeIn(EditUnitTeamPanel);
        HS.heroPanelInit();
        HS.heroItemsInTeam[0].heroBtnTapped();
    }
    public void refreshCurrentUnitTeam()
    {
        PageView pv = TeamsScrolrect.GetComponent<PageView>();
        int pvIndex = pv.currentIndex + 1;
        GDEunitTeamData Team = SDDataManager.Instance.getHeroTeamByTeamId(pvIndex);
        CurrentTeamVision.initThisUnitTeam(Team);
        showTHisUnitTeamOtherData(Team);
        showTeamId(pvIndex);
    }
    public void showTeamId(int teamId)
    {
        int index = Mathf.Clamp(teamId - 1,0,TeamListParent.childCount-1);
        for (int i = 0; i < TeamListParent.childCount; i++)
        {
            if(i == index)
            {
                TeamListParent.GetChild(i).GetComponentInChildren<Image>().color
                    = new Color32(85, 210, 163, 255);
            }
            else
            {
                TeamListParent.GetChild(i).GetComponentInChildren<Image>().color
                    = new Color32(17, 17, 17, 255);
            }
        }
    }


    public void ConfirmBattleTeam(int TeamId)
    {
        SDGameManager.Instance.currentHeroTeamIndex = TeamId;
        //
        ChapterLevelController CLC = homeScene.LevelEnterPanel.GetComponent<LevelEnterPanel>().CLC;
        CLC.GoToBattleScene();
    }
    public void changeSpriterenderersStatus(bool needHide)
    {
        Transform modelGroup
            = EditUnitTeamPanel.GetComponent<SelectTeamUnitPanel>().RPC.RoleGroup;
        if (needHide)
        {
            for(int i = 0; i < modelGroup.childCount; i++)
            {
                modelGroup.GetChild(i).gameObject.SetActive(false);
            }


        }
        else
        {
            for (int i = 0; i < modelGroup.childCount; i++)
            {
                modelGroup.GetChild(i).gameObject.SetActive(true);
            }


        }
    }



    public void commonBackAction()
    {
        if(currentPanelContent == panelContent.main)
        {
            homeScene.SubMenuClose();
            if (panelFrom != HomeScene.HomeSceneSubMenu.End)
            {
                homeScene.CurrentSubMenuType = panelFrom;
            }
        }
        else if(currentPanelContent == panelContent.edit)
        {
            currentPanelContent = panelContent.main;
            EditUnitTeamPanel.localScale = Vector3.zero;
            showTeamId(SDGameManager.Instance.currentHeroTeamIndex);
        }
        else if(currentPanelContent == panelContent.end)
        {

        }
    }

}
