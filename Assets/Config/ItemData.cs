using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataEntry
{
    public ItemData[] ItemDatas;
}

[System.Serializable]
public class ItemData
{
    /// <summary>
    /// 道具ID
    /// </summary>
    public int item_ID;
    /// <summary>
    /// 道具分类
    /// </summary>
    public enum ItemType
    {
        unKown = -1,
        Equip,//装备
        Chip,//碎片
        Spec,//特殊道具
    }
    public ItemType item_type = ItemType.unKown;
    /// <summary>
    /// 道具细分分类
    /// </summary>
    public int item_kind;
    /// <summary>
    /// 道具名称
    /// </summary>
    public string item_name;
    /// <summary>
    /// 道具描述
    /// </summary>
    public string item_description;
    /// <summary>
    /// 道具Icon
    /// </summary>
    public string item_icon;
    public string item_bgicon;
    public int item_count;
    public int item_quality;
    /// <summary>
    /// 道具的操作：分解/合成/出售/装备
    /// </summary>
    public int item_operation;

   /// <summary>
   /// 道具构造
   /// </summary>
   /// <param name="itemID"></param>
   /// <param name="name"></param>
   /// <param name="desc">描述</param>
   /// <param name="type">-1:all;0:equip;1:chips;2:special;</param>
   /// <param name="kind">SubEquipType道具子类型</param>
   /// <param name="icon">道具Icon</param>
   /// <param name="bgIcon">道具稀有度Icon</param>
   /// <param name="count">数量</param>
   /// <param name="qual">品质0:white-Poor;1:cyan-Common;2:green-Uncommon;3:red-Rare;4:purple-Epic;5:orange-Legendary;6:yellow-Supereme</param>
   /// <param name="opera">可操作类型：分解、合成、出售、装备</param>
    public ItemData(int itemID, string name, string desc, int type, int kind, string icon, string bgIcon, int count, int qual, int opera)
    {
        this.item_ID = itemID;
        this.item_name = name;
        this.item_description = desc;
        this.item_type = (ItemType)type;
        this.item_kind = kind;
        this.item_icon = icon;
        this.item_bgicon = bgIcon;
        this.item_count = count;
        this.item_quality = qual;
        this.item_operation = opera;
    }
}
