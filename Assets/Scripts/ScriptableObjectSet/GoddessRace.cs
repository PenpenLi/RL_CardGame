using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="goddess_race",menuName ="Wun/角色/女神种族")]
public class GoddessRace : CharacterRace
{
    public GoddessRace()
    {
        CharacterType = SDConstants.CharacterType.Goddess;
    }
}
