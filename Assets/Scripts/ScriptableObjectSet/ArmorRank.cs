using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Armor_Rank",menuName ="Wun/道具/护甲类型(阶级)")]
public class ArmorRank : BaseRank
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("无甲","轻甲","中甲","重甲","特殊甲","非护甲")]
#endif
    private SDConstants.ArmorType armorType;
    public SDConstants.ArmorType ArmorType
    {
        get { return armorType; }
    }
}
