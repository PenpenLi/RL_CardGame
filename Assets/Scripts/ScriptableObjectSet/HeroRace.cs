using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="hero_race",menuName ="Wun/角色/英雄种族")]
public class HeroRace : CharacterRace
{
    [SerializeField]
#if UNITY_EDITOR
    [EnumMemberNames("人类","精灵","龙裔","End")]
#endif
    private Race race;
    public Race Race
    {
        get { return race; }
        set { race = value; }
    }
    public HeroRace()
    {
        CharacterType = SDConstants.CharacterType.Hero;
    }
}
