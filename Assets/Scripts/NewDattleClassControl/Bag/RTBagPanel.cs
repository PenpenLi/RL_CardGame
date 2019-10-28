using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

/// <summary>
/// 背包类
/// </summary>
public class RTBagPanel : MonoBehaviour
{
    public SDConstants.BagItemType itemType;
    public Text numText;
    public RTPageController pageController;
    public ScrollRect scrollRect;
    public Transform quickSellSign;
    public Transform emptyEquipPanel;
    //public RTForgePanel forgePanel;
    public Image[] typeBtnImages;
    public Button[] typeBtns;

    public void initBagPanel()
    {
        if(itemType == SDConstants.BagItemType.Helmet)
        {

        }
        else if(itemType == SDConstants.BagItemType.Breastplate)
        {

        }
        else if(itemType == SDConstants.BagItemType.Gardebras)
        {

        }
        else if(itemType == SDConstants.BagItemType.Legging)
        {

        }
        else if(itemType == SDConstants.BagItemType.Weapon)
        {

        }
        else if(itemType == SDConstants.BagItemType.Prop)
        {

        }
        else if(itemType == SDConstants.BagItemType.Material)
        {

        }
        else if(itemType == SDConstants.BagItemType.Keys)
        {

        }
        setTypeBtnActived((int)itemType);
    }
    public void setTypeBtnActived(int index)
    {
        for(int i = 0; i < typeBtnImages.Length; i++)
        {
            if(i == index)
            {
                typeBtnImages[i].sprite = SLEffectManager.Instance.typeBtnBgSps[1];
                typeBtns[i].enabled = false;
            }
            else
            {
                typeBtnImages[i].sprite = SLEffectManager.Instance.typeBtnBgSps[0];
                typeBtns[i].enabled = true;
            }
        }
    }



}
