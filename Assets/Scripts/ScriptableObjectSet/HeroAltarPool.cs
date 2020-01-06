using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName ="hero_altar_pool",menuName ="Wun/其他/卡池/英雄卡池")]
public class HeroAltarPool : AltarPool
{
    [Space]
    public List<HeroInfo> HeroList;
    public override List<string> IncludeIDsList 
    {
        get
        {
            List<string> list = HeroList.Select(x => x.ID).ToList();
            return list;
        }
    }

    [SerializeField]
    private List<HeroInfo> _HeroesUsingSpecialPossibility;
    public List<HeroInfo> HeroesUsingSpecialPossibility
    {
        get { return _HeroesUsingSpecialPossibility; }
        set { _HeroesUsingSpecialPossibility = value; }
    }


}
