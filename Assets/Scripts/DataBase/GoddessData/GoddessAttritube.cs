using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoddessAttritube
{
    [SerializeField,Header("敏捷")]
    private int agile;
    public int Agile { get { return agile; } }

    [SerializeField,Header("耐力")]
    private int stamina;
    public int Stamina { get { return stamina; } }

    [SerializeField,Header("恢复")]
    private int recovery;
    public int Recovery { get { return recovery; } }

    [SerializeField,Header("统帅")]
    private int leader;
    public int Leader { get { return leader; } }

    public int GoddessA(string nam)
    {
        if (nam == nameof(agile) || nam == nameof(Agile)) { return Agile; }
        else if (nam == nameof(stamina) || nam == nameof(Stamina)) { return Stamina; }
        else if (nam == nameof(recovery)||nam == nameof(Recovery)) { return Recovery; }
        else if(nam == nameof(leader) || nam == nameof(Leader)) { return Leader; }
        Debug.Log("超出GoddessAttritube范围");
        return 0;
    }
    public int GoddessA(GoddessAttiType type)
    {
        switch (type)
        {
            case GoddessAttiType.agile:return Agile;
            case GoddessAttiType.stamina:return Stamina;
            case GoddessAttiType.recovery:return Recovery;
            case GoddessAttiType.leader:return Leader;
            default:return 0;
        }
    }
    public GoddessAttritube(int agile,int stamina,int recovery,int leader)
    {
        this.agile = agile;
        this.stamina = stamina;
        this.recovery = recovery;
        this.leader = leader;
    }
    public static GoddessAttritube zero
    {
        get { return new GoddessAttritube(0, 0, 0, 0); }
    }
    public static GoddessAttritube operator +(GoddessAttritube a,GoddessAttritube b)
    {
        return new GoddessAttritube(a.Agile + b.Agile, a.Stamina + b.Stamina
            , a.Recovery + b.Recovery, a.Leader + b.Leader);
    }
    public static GoddessAttritube operator -(GoddessAttritube a, GoddessAttritube b)
    {
        return new GoddessAttritube(a.Agile - b.Agile, a.Stamina - b.Stamina
            , a.Recovery - b.Recovery, a.Leader - b.Leader);
    }

    public void CheckSelf()
    {
        agile = Mathf.Clamp(Agile, -SDConstants.GoddessMaxAtti, SDConstants.GoddessMaxAtti);
        stamina = Mathf.Clamp(Stamina, -SDConstants.GoddessMaxAtti, SDConstants.GoddessMaxAtti);
        recovery = Mathf.Clamp(Recovery, -SDConstants.GoddessMaxAtti, SDConstants.GoddessMaxAtti);
        leader = Mathf.Clamp(Leader, -SDConstants.GoddessMaxAtti, SDConstants.GoddessMaxAtti);
    }
}

public enum GoddessAttiType
{
    agile=0,
    stamina=1,
    recovery=2,
    leader=3,
    end,
}