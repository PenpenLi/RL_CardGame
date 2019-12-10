using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equip", menuName = "Wun/道具/新装备")]
[System.Serializable]
public class EquipItem : ItemBase, ItemBase.IUsable
{
    [SerializeField]
    [EnumMemberNames("头盔","胸甲","臂甲","腿甲","饰品","武器","END")]
    private EquipPosition equipPos;
    public EquipPosition EquipPos
    {
        get { return equipPos; }
    }
    public EquipItem()
    {
        ItemType = SDConstants.ItemType.Equip;
    }
    public void OnUse()
    {
        Debug.Log("UseEquip");
    }
}
