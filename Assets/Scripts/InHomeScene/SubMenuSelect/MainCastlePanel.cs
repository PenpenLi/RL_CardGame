using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class MainCastlePanel : BasicSubMenuPanel
{
    [Space(25)]
    public int LvUpEnableBuildingCount;
    public List<BasicSubMenuPanel> AllBuildings = new List<BasicSubMenuPanel>();
    public Transform BuildingLinkCard;
    public ScrollRect CardScrollrect;
    public List<BuiildingSimpleDetailVesion> AllCards = new List<BuiildingSimpleDetailVesion>();
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        refreshAllBuildingsInfor();
        AllBuildingLinkCardsInit();
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        ClearAllCards();
        homeScene.SubMenuClose();
    }

    public void refreshAllBuildingsInfor()
    {
        AllBuildings.Clear();
        for (int i = 0; i < homeScene.AllSubMenus.Length; i++)
        {
            if (homeScene.AllSubMenus[i].GetComponent<BasicSubMenuPanel>()
                && homeScene.AllSubMenus[i].GetComponent<BasicSubMenuPanel>().LvUpEnable)
            {
                AllBuildings.Add(homeScene.AllSubMenus[i].GetComponent<BasicSubMenuPanel>());
            }
        }
        LvUpEnableBuildingCount = AllBuildings.Count;
        //建筑升级存档数量修正
        if (SDDataManager.Instance.PlayerData.buildingsOwned.Count < AllBuildings.Count)
        {
            for (int i = 0; i < AllBuildings.Count; i++)
            {
                BasicSubMenuPanel Menu = AllBuildings[i];
                bool flag = true;
                foreach (GDEtownBuildingData B in SDDataManager.Instance.PlayerData.buildingsOwned)
                {
                    if (B.id == Menu.buildingId)
                    {
                        flag = false; break;
                    }
                }
                if (flag)
                {
                    SDDataManager.Instance.PlayerData.buildingsOwned.Add
                        (new GDEtownBuildingData(GDEItemKeys.townBuilding_newTownBuilding)
                        {
                            id = Menu.buildingId,
                            exp = 0,
                            NPC = null,
                        });
                    SDDataManager.Instance.PlayerData.Set_buildingsOwned();
                }
            }
        }
        for (int i = 0; i < AllBuildings.Count; i++)
        {
            foreach(GDEtownBuildingData b in SDDataManager.Instance.PlayerData.buildingsOwned)
            {
                if(b.id == AllBuildings[i].buildingId)
                {
                    AllBuildings[i].exp = b.exp;
                    if (AllBuildings[i].ShowNPC)
                    {
                        if (b.NPC == null)
                        {
                            b.NPC = new GDENPCData(GDEItemKeys.NPC_noone)
                            {
                                id = AllBuildings[i].RepresentNPCId,
                            };
                            SDDataManager.Instance.PlayerData.Set_buildingsOwned();
                        }
                        else if(b.NPC.id != AllBuildings[i].RepresentNPCId)
                        {
                            b.NPC.id = AllBuildings[i].RepresentNPCId;
                            SDDataManager.Instance.PlayerData.Set_buildingsOwned();
                        }
                    }
                    else
                    {
                        if (b.NPC != null)
                        {
                            b.NPC = null;
                            SDDataManager.Instance.PlayerData.Set_buildingsOwned();
                        }
                    }
                    break;
                }
            }
        }
    }
    public void AllBuildingLinkCardsInit()
    {
        for (int i = 0; i < AllBuildings.Count; i++)
        {
            if (AllBuildings[i].buildingId == buildingId) continue;

            Transform card = Instantiate(BuildingLinkCard) as Transform;
            card.SetParent(CardScrollrect.content);
            card.localScale = Vector3.one;
            card.gameObject.SetActive(true);
            BuiildingSimpleDetailVesion C 
                = card.GetComponent<BuiildingSimpleDetailVesion>();
            C.MCP = this;
            C.initBuildingLinkCard(AllBuildings[i].buildingId);
            AllCards.Add(C);
        }
        CardScrollrect.verticalNormalizedPosition = 0;
    }
    public void ClearAllCards()
    {
        for (int i = 0; i < AllCards.Count; i++)
        {
            Destroy(AllCards[i].gameObject);
        }
        AllCards.Clear();
    }
}
