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
    //public Transform SelectUnitTeamPanel;
    //public Transform OneTeam;
    public ScrollRect TeamsScrolrect;
    //public OneUnitTeam CurrentTeamVision;
    public Transform TeamListParent;
    //public List<OneUnitTeam> Teams;
    [Space(10)]
    public Transform EditUnitTeamPanel;
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        WhenOpenThisMenu();
    }
    public void WhenOpenThisMenu()
    {
        BasicHeroSelect bhs = HS.heroesSelectPanel.GetComponent<BasicHeroSelect>();
        bhs.pageController.scrollRectReset();
        if (string.IsNullOrEmpty(SDGameManager.Instance.currentHeroTeamId))
        {
            SDGameManager.Instance.currentHeroTeamId = "TEAM#" + 1;
        }
        SetupSelectUnitTeamPanel();
        //EditUnitTeamPanel.localScale = Vector3.zero;
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
        //GDEunitTeamData Team = SDDataManager.Instance.getHeroTeamByTeamId(teamId);
        //CurrentTeamVision.initThisUnitTeam(Team);
        //showTHisUnitTeamOtherData(Team);
    }
    public void openEditUnitTeamPanel(string teamId)
    {
        SelectTeamUnitPanel STUP = EditUnitTeamPanel.GetComponent<SelectTeamUnitPanel>();
        STUP.CurrentTeamId = teamId;
        STUP.whenOpenThisPanel();
        //UIEffectManager.Instance.showAnimFadeIn(EditUnitTeamPanel);
        HS.heroPanelInit();
        HS.heroItemsInTeam[0].heroBtnTapped();
    }
    public void refreshCurrentUnitTeam()
    {
        PageView pv = TeamsScrolrect.GetComponent<PageView>();
        int _pvIndex = pv.currentIndex + 1;
        string pvIndex = "TEAM#" + _pvIndex;
        //GDEunitTeamData Team = SDDataManager.Instance.getHeroTeamByTeamId(pvIndex);
        //CurrentTeamVision.initThisUnitTeam(Team);
        //showTHisUnitTeamOtherData(Team);
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
