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

    public void initBuildingLinkCard(string id)
    {
        BuildingId = id;
        for(int i = 0; i < MCP.homeScene.AllSubMenus.Length; i++)
        {
            BasicSubMenuPanel P 
                = MCP.homeScene.AllSubMenus[i].GetComponent<BasicSubMenuPanel>();
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

    }

    public void BtnTapped()
    {
        MCP.commonBackAction();
        MCP.homeScene.UseSMTToSubMenu(thisTag);
    }
}
