using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "Equip", menuName = "Wun/道具/新装备")]
[System.Serializable]
public class EquipItem : ItemBase, ItemBase.IUsable
{
    [SerializeField]
    private string resistStr;
    public string ResistStr
    { get { return resistStr; } set { resistStr = value; } }

    [SerializeField]
    [EnumMemberNames("头盔","胸甲","臂甲","腿甲","饰品","武器","END")]
    private EquipPosition equipPos;
    public EquipPosition EquipPos
    {
        get 
        {
            return equipPos; 
        }
        set 
        {
            equipPos = value; 
        }
    }

    [SerializeField]
    [ConditionalHide("equipPos",(int)EquipPosition.Hand,true)]
    private WeaponRace weaponRace;
    public WeaponRace WeaponRace
    {
        get { return weaponRace; }
        set { weaponRace = value; }
    }
    public string ArmorRankId
    {
        get
        {
            if (armorRank) return ArmorRank.ID;
            else return string.Empty;
        }
    }
    [SerializeField]
    private ArmorRank armorRank;
    public ArmorRank ArmorRank
    { get { return armorRank; } set { armorRank = value; } }

    [SerializeField]
    private bool _SuitBelong;
    public bool SuitBelong
    { get { return _SuitBelong; } set { _SuitBelong = value; } }

    [SerializeField]
    [ConditionalHide("_SuitBelong",true)]
    private string suitId;
    public string SuitId
    {
        get { return suitId; }
        set { suitId = value; }
    }
    [Space]
    [SerializeField]
    private RoleAttributeList _RAL;
    public RoleAttributeList RAL
    {
        get { return _RAL; }
        set { _RAL = value; }
    }
    [SerializeField]
    private string passiveEffect;
    public string PassiveEffect
    {
        get { return passiveEffect; }
        set { passiveEffect = value; }
    }
    public int BattleForce
    {
        get
        {
            int ralF = RAL.BattleForce;
            int peF = 0;
            return ralF + peF;
        }
    }

    public EquipItem()
    {
        ItemType = SDConstants.ItemType.Equip;
    }
    public override void initData(string id, string name, string desc, int level
        , bool stackable, bool discardable, bool useable, bool sellable, bool inexhaustible
        ,string specialStr
        , SDConstants.ItemType itemtype = SDConstants.ItemType.Equip)
    {
        base.initData(id, name, desc, level, stackable, discardable, useable, sellable
            , inexhaustible,specialStr , itemtype);
    }



    public void OnUse()
    {
        Debug.Log("UseEquip");
    }
}
