using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDHero : BasicRoleProperty
{
    public Job _heroJob;
    public Race _heroRace;
    public int grade;
    public int gender = -1;
    //
    public int nameBeforeId;
    //
    private void Start()
    {
        
    }
    public override void initData_Hero(Job heroJob, Race heroRace, int grade, int quality, int Level
    , RoleAttributeList ra, RoleAttributeList raRate, int cri, int criDmg
    , int dmgReduction, int dmgReflection, int RewardRate
    , RoleBarChart bcRegendPerTurn, int id, string name, int wakeNum)
    {
        base.initData_Hero(heroJob, heroRace, grade, quality, Level
            , ra, raRate, cri, criDmg, dmgReduction, dmgReflection, RewardRate
            , bcRegendPerTurn, id, name, wakeNum);
        Debug.Log(name + " " + heroJob + " & " + heroRace);
        this._heroJob = heroJob;
        this._heroRace = heroRace;
    }
}
