using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    UnKnown=-1,
    Equip,
    Ruin,
}

public class EquipItems 
{
    /// <summary>
    /// 唯一ID，区别于其他道具
    /// </summary>
    public int ItemID;

    public ItemType MItemType = ItemType.UnKnown;

    //item name
    public string ItemName;
    /// <summary>
    /// 道具描述信息
    /// </summary>
    public string ItemDesc;

    public string ItemIcon;
    public string ItemBgIcon;
    public int ItemCount;//数量
    public int ItemQua;//品质
    /// <summary>
    /// 道具的可操作模式：可分解 可售卖 可出售 可装备 等
    /// </summary>
    public int ItemOpreation;


    /// <summary>
    /// 构造一个道具
    /// </summary>
    public EquipItems(int item_id)
    {

    }

}
