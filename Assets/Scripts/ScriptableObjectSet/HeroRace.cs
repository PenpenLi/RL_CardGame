using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="hero_race",menuName ="Wun/角色/英雄种族")]
public class HeroRace : CharacterRace
{
    public HeroRace()
    {
        CharacterType = SDConstants.CharacterType.Hero;
    }
}
