using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Equip_weapon_race",menuName ="Wun/道具/武器类型")]
public class WeaponRace : BaseRank
{
    [SerializeField]
    [EnumMemberNames("大剑(克莱莫尔)，双手剑","长柄武器"
        ,"锐器，单手剑，匕首，双刀","弓"
        ,"钝器，战锤","护身符，十字架"
        ,"辅助器，魔法书","增强器，法杖")]
    private SDConstants.WeaponClass weaponClass;
    public SDConstants.WeaponClass WeaponClass
    {
        get { return weaponClass; }
        set { weaponClass = value; }
    }
}
