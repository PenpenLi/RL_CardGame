using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单背包项
/// </summary>
public class RTSingleBagItem : MonoBehaviour
{
    public Image bgImg;
    public Image itemImg;
    public RTEquipUpTimes itemUpTimes;
    public Text levelText;
    public Text itemName;
    public Text numText;
    public SDConstants.BagItemType type;
    public int itemId;
    public int itemNum;
    public int itemLevel;
    public int itemUpLv;
    //protected 


    public void InitDropMaterial(int id , int num)
    {
        type = SDConstants.BagItemType.Material;
        numText.transform.parent.gameObject.SetActive(true);
        if(bgImg != null)
        {
            bgImg.sprite = SLEffectManager.Instance.bagItemBgSps[0];
        }


    }

    public void InitProps(int id)
    {
        type = SDConstants.BagItemType.Prop;
        numText.transform.parent.gameObject.SetActive(true);
        GetComponent<Button>().enabled = true;
        if(bgImg != null)
        {
            bgImg.sprite = SLEffectManager.Instance.bagItemBgSps[0];
        }


    }
}
