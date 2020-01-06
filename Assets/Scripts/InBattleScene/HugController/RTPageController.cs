using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包页面控制器类，管理所有背包单项
/// </summary>
public class RTPageController : MonoBehaviour
{
    public SDConstants.BagItemType type;
    public RTSingleBagItem[] items;
    public RTBagItemDetail itemDetailPanel;
    //public 
    public ScrollRect scrollrect;
    public int singleEquipHeight = 180;

    #region 物品按SDContants.BagItemType构建
    public void ItemsInit()
    {
        if(type == SDConstants.BagItemType.Helmet)
        {

        }
        else if(type == SDConstants.BagItemType.Breastplate)
        {

        }
        else if(type == SDConstants.BagItemType.Gardebras)
        {

        }
        else if(type == SDConstants.BagItemType.Legging)
        {

        }
        else if(type == SDConstants.BagItemType.Weapon)
        {

        }
        else if(type == SDConstants.BagItemType.Jewelry)
        {

        }
        else if(type == SDConstants.BagItemType.Prop)
        {

        }
        else if(type == SDConstants.BagItemType.Material)
        {

        }
        else if(type == SDConstants.BagItemType.Keys)
        {

        }
    }
    #endregion
    public bool showDropMaterials()
    {
        bool flag = false;
        for(int i = 0; i < (int)SDConstants.MaterialType.end; i++)
        {
            int tmp = i;
            items[i].gameObject.SetActive(true);
            if(tmp < (int)SDConstants.MaterialType.end)
            {
                int num = SDGameManager.Instance.DropMaterials[i];
                if (num > 0)
                {
                    flag = true;
                    //items[i].InitDropMaterial(i + 10001, num);
                }
                else
                {
                    items[i].gameObject.SetActive(false);
                }
            }
            else
            {
                items[i].gameObject.SetActive(false);
            }
        }
        if (!flag)
        {
            transform.parent.gameObject.SetActive(false);
        }
        return flag;
    }
    public void showDropProps()
    {
        int lv = SDGameManager.Instance.currentLevel;
        int num = lv / SDConstants.LevelNumPerSection;
        for(int i = SDConstants.BagMaxNumPerPage - num; i< SDConstants.BagMaxNumPerPage; i++)
        {
            items[i].gameObject.SetActive(true);
            //int dropId = 
            //SDDataManager.Instance.addProp();
            items[i].itemNum = 1;
            //items[i].InitProps(dropId);
        }
    }
}
