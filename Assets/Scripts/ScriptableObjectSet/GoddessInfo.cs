using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="goddess_info",menuName ="Wun/角色/女神信息")]
public class GoddessInfo : CharacterInfo
{
    [SerializeField]
    private GoddessRace goddessRace;
    public GoddessRace GoddessRace
    {
        get { return goddessRace; }
    }

    public GoddessInfo()
    {
        CharacterType = SDConstants.CharacterType.Goddess;
    }
}
