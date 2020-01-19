using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class BuiildingSimpleDetailVesion : MonoBehaviour
{
    public Image BuildingImg;
    public Image NPCImg;
    public Text NameText;
    public Text DetailText;
    public string BuildingId;
    public int IndexInHS;
    public HomeScene.HomeSceneSubMenu thisTag;
    public MainCastlePanel MCP;
    public Button lvUpBtn;

    public void initBuildingLinkCard(string id)
    {
        BuildingId = id;
        BasicSubMenuPanel P = null;
        for (int i = 0; i < MCP.homeScene.AllSubMenus.Length; i++)
        {
            if (MCP.homeScene.AllSubMenus[i] == null)
            {
                continue;
            }
            P = MCP.homeScene.AllSubMenus[i].GetComponent<BasicSubMenuPanel>();
            if(P.buildingId == BuildingId)
            {
                IndexInHS = i;
                thisTag = (HomeScene.HomeSceneSubMenu)i;
                //buildingImg
                //NPCImg
                NameText.text = thisTag.ToString() + "·" + SDGameManager.T("Lv.")
                    + P.Level;
                break;
            }
        }
        if (P == null) return;
        bool flag = P.CheckIfCanLvUp();
        lvUpBtn.interactable = flag;
    }

    public void BtnTapped()
    {
        MCP.commonBackAction();
        MCP.homeScene.UseSMTToSubMenu(thisTag);
    }

    public void ExtraBtnTapped()
    {
        BasicSubMenuPanel P = MCP.homeScene.AllSubMenus[(int)thisTag]
            .GetComponent<BasicSubMenuPanel>();
        bool flag = P.CheckIfCanLvUp();
        if (flag)
        {
            P.BtnToLvUp();
        }
    }
}
