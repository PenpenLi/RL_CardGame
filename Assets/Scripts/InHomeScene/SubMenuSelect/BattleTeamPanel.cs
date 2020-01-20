using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using UnityEngine.EventSystems;

public class BattleTeamPanel : BasicSubMenuPanel
{ 
    public SDHeroSelect HS;
    [Space(25)]
    public ScrollRect TeamsScrolrect;
    public Transform TeamListParent;
    [Space(10)]
    public SelectTeamUnitPanel STUP;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        WhenOpenThisMenu();
    }
    public void WhenOpenThisMenu()
    {
        HEWPageController PAGE = HS.PAGE;
        PAGE.scrollRectReset();
        if (string.IsNullOrEmpty(SDGameManager.Instance.currentHeroTeamId))
        {
            SDGameManager.Instance.currentHeroTeamId = "TEAM#" + 1;
        }
        SetupSelectUnitTeamPanel();
        showTeamId(SDGameManager.Instance.currentHeroTeamId);
        openEditUnitTeamPanel(SDGameManager.Instance.currentHeroTeamId);
    }
    void SetupSelectUnitTeamPanel()
    {
        if (SDDataManager.Instance.getHeroGroup().Count != SDConstants.MaxBattleTeamNum)
        {
            SDDataManager.Instance.getHeroGroup().Clear();
            for (int i = 0; i < SDConstants.MaxBattleTeamNum; i++)
            {
                GDEunitTeamData T = new GDEunitTeamData(GDEItemKeys.unitTeam_emptyHeroTeam)
                {
                    id = "TEAM#"+ (i + 1),
                    goddess = string.Empty,
                    badge = 0
                };
                SDDataManager.Instance.PlayerData.heroesTeam.Add(T);
                SDDataManager.Instance.PlayerData.Set_heroesTeam();
            }
        }
    }
    public void openEditUnitTeamPanel(string teamId)
    {
        STUP.CurrentTeamId = teamId;
        STUP.whenOpenThisPanel();
        HS.heroPanelInit();
        HS.heroItemsInTeam[0].heroBtnTapped();
    }
    public void refreshCurrentUnitTeam()
    {
        PageView pv = TeamsScrolrect.GetComponent<PageView>();
        int _pvIndex = pv.currentIndex + 1;
        string pvIndex = "TEAM#" + _pvIndex;
        showTeamId(pvIndex);
        openEditUnitTeamPanel(pvIndex);
    }
    public void showTeamId(string teamId)
    {
        int _index = SDDataManager.Instance.getInteger(teamId.Split('#')[1]);
        int index = Mathf.Clamp(_index - 1,0,TeamListParent.childCount-1);
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


    public void ConfirmBattleTeam(string TeamId)
    {
        SDGameManager.Instance.currentHeroTeamId = TeamId;
        //
        ChapterLevelController CLC = homeScene._LevelEnterPanel.GetComponent<LevelEnterPanel>().CLC;
        CLC.GoToBattleScene();
    }
    public void changeSpriterenderersStatus(bool needHide)
    {
        Transform modelGroup
            = STUP.RPC.RoleGroup;
        if (needHide)
        {
            for(int i = 0; i < modelGroup.childCount; i++)
            {
                modelGroup.GetComponentInChildren<MeshRenderer>().sortingOrder = -1;
            }
        }
        else
        {
            for (int i = 0; i < modelGroup.childCount; i++)
            {
                modelGroup.GetComponentInChildren<MeshRenderer>().sortingOrder = 0;
            }


        }
    }



    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
        if (panelFrom != HomeScene.HomeSceneSubMenu.End)
        {
            homeScene.CurrentSubMenuType = panelFrom;
        }
    }






}
