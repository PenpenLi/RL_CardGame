using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OneItemData
{
    //装备数据
    /// <summary>
    /// 物品Id
    /// </summary>
    public int Id;

    public enum ItemType
    {
        UnKnown,
        Equip,
        Chip,
        Spec,
    }
    public ItemType ThisItemType = ItemType.UnKnown;
    /// <summary>
    /// 物品名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 物品介绍
    /// </summary>
    public string Desc;
    /// <summary>
    /// 物品图标
    /// </summary>
    public string Icon;
    /// <summary>
    /// 物品品质
    /// </summary>
    public int Quality;
    /// <summary>
    /// 稀有度图标
    /// </summary>
    public string BgIcon;

    //
    public int Count;//数量
    public int Operation;//装备可操作性模式

}


//----------------------装备数据-----------------------//
public enum EquipPosition
{
    Head,
    Breast,
    Leg,
    Hand,
    Neck,
    Finger,
    Weapon,
}
public class OneEquipageData : OneItemData
{
    EquipPosition _thisequipposition;
    public EquipPosition ThisEquipagePosition
    {
        get { return _thisequipposition; }
        private set { _thisequipposition = value; }
    }


    public int SameEquipKind;//同装备槽内装备细分名称

    //各项属性数值修正（直接修正）
    public RoleSelfAbility RoleAttributeList;

    //装备在装备上的限制与特征
    public int Weight;//用于控制最大装备质量上限
    public int Durability;//装备耐久
    /// <summary>
                          /// 装备槽限制（双手武器限制副手）
                          /// </summary>
    public List<EquipPosition> ThisEquipLimit = new List<EquipPosition>();
}