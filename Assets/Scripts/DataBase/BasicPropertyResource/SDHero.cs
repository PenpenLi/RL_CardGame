using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDHero : BasicRoleProperty
{
    public Job _heroJob;
    public Race _heroRace;
    public int grade;
    public CharacterSex gender = CharacterSex.Unknown;
    //
    public int nameBeforeId;
    //
    private void Start()
    {
        
    }
    public override void initData_Hero(Job heroJob, Race heroRace, int grade, int quality, int Level
    , RoleAttributeList ra, int criDmg
    , int dmgReduction, int dmgReflection, int RewardRate
    , RoleBarChart bcRegendPerTurn, string id, string name, int wakeNum)
    {
        base.initData_Hero(heroJob, heroRace, grade, quality, Level
            , ra, criDmg, dmgReduction, dmgReflection, RewardRate
            , bcRegendPerTurn, id, name, wakeNum);
        Debug.Log(name + " " + heroJob + " & " + heroRace);
        this._heroJob = heroJob;
        this._heroRace = heroRace;
    }
}
