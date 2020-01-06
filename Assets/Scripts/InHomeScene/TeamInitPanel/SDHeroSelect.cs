using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 出战英雄选择类
/// </summary>
public class SDHeroSelect : MonoBehaviour
{
    public SingleHeroTeamItem[] heroItemsInTeam;
    /// <summary>
    /// 队伍中英雄(hashcode)
    /// </summary>
    public int[] heroesInTeam;


    //
    public SelectTeamUnitPanel MainPanel;
    public Transform heroesSelectPanel;
    [Header("简易角色信息板")]
    public SimpleHeroDetailVision simpleHDV;
    // Start is called before the first frame update
    void Start()
    {
        //heroPanelInit();
    }
    public void heroPanelInit()
    {
        string currentTeamId = MainPanel.CurrentTeamId;
        GDEunitTeamData unitTeam = SDDataManager.Instance.getHeroTeamByTeamId(currentTeamId);
        heroesInTeam = new int[SDConstants.MaxSelfNum];
        List<GDEHeroData> all = SDDataManager.Instance.getHerosFromTeam(currentTeamId);
        for(int i = 0; i < SDConstants.MaxSelfNum; i++)
        {
            bool flag = false;
            foreach(GDEHeroData H in all)
            {
                if(i == H.TeamOrder)
                {
                    int HC = H.hashCode;
                    heroesInTeam[i] = HC;
                    heroItemsInTeam[i].initHero(HC);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                heroesInTeam[i] = 0;
                heroItemsInTeam[i].initEmptyHero();
            }
        }
        //initads();
        MainPanel.RPC.initRoleModelToRolePosPlace();

        heroBtnFunction(MainPanel.currentHeroIndexInTeam);
    }

    public void heroBtnTapped(int index)
    {
        SDGameManager.Instance.isSelectHero = true;
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.Battle;
        MainPanel.currentHeroIndexInTeam = index;
        heroBtnFunction(index);

        //
        MainPanel.currentEditType = SelectTeamUnitPanel.editType.hero;
    }
    public void heroBtnFunction(int index)
    {
        showHeroesSelectPanel(index);
        if (heroesInTeam.Length <= index)
        {
            simpleHDV.EmptyVision();return;
        }
        if (heroesInTeam[index] == 0)
        {
            //showHeroesSelectPanel(index);
            simpleHDV.EmptyVision();
        }
        else
        {
            BasicHeroSelect BHS = heroesSelectPanel.GetComponent<BasicHeroSelect>();
            if (BHS)
            {
                //显示英雄详情
                SDHeroDetail HDP = BHS.heroDetails.GetComponent<SDHeroDetail>();
                int _hashcode = heroesInTeam[index];
                HDP.gameObject.SetActive(true);
                HDP.initHeroDetailPanel(_hashcode);

                if(SDGameManager.Instance.heroSelectType != SDConstants.HeroSelectType.Battle)
                {
                    HDP.HeroWholeMessage.whenOpenThisPanel();
                }

                simpleHDV.ReadFromSDHD(HDP);
            }
            else { simpleHDV.EmptyVision(); }
        }
    }
    public void showHeroesSelectPanel(int index)
    {
        heroesSelectPanel.localScale = Vector3.one;

        BasicHeroSelect bhs = heroesSelectPanel.GetComponent<BasicHeroSelect>();
        bhs.index = index;
        //bhs.pageController.scrollRectReset();
        bhs.heroesInit();
        if(!bhs.gameObject.activeSelf)
            UIEffectManager.Instance.showAnimFadeIn(bhs.transform);

    }

}
