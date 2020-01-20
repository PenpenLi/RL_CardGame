using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System.Linq;

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
    public HEWPageController PAGE;
    [Header("简易角色信息板")]
    public SimpleHeroDetailVision simpleHDV;
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
        MainPanel.RPC.initRoleModelToRolePosPlace();

        heroBtnFunction(MainPanel.currentHeroIndexInTeam);
        PAGE.ItemsInit(SDConstants.ItemType.Hero);
        foreach (SingleItem item in PAGE.items)
        {
            if (heroesInTeam.Contains(item.itemHashcode))
            {
                item.isSelected = true;
            }
            else
            {
                item.isSelected = false;
            }
        }
    }

    public void heroBtnTapped(int index)
    {
        //SDGameManager.Instance.isSelectHero = true;
        SDGameManager.Instance.heroSelectType = SDConstants.HeroSelectType.Battle;
        MainPanel.currentHeroIndexInTeam = index;
        heroBtnFunction(index);

        //
        MainPanel.currentEditType = SelectTeamUnitPanel.editType.hero;
        PAGE.ItemsInit(SDConstants.ItemType.Hero);
        foreach(SingleItem item in PAGE.items)
        {
            if (heroesInTeam.Contains(item.itemHashcode))
            {
                item.isSelected = true;
            }
            else
            {
                item.isSelected = false;
            }
        }
    }
    public void heroBtnFunction(int index)
    {
        if (heroesInTeam.Length <= index)
        {
            simpleHDV.EmptyVision();return;
        }
        if (heroesInTeam[index] <= 0)
        {
            simpleHDV.EmptyVision();
        }
        else
        {
            int _hashcode = heroesInTeam[index];
            simpleHDV.ShowHeroMessage(_hashcode);
        }
    }

}
