using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSAddState : GoddessSkill
{
    public override void StartSkill()
    {
        base.StartSkill();
        List<BattleRoleData> list = DealWithAOEAction(TargetIsHero, AOE);
        for (int i = 0; i < list.Count; i++)
        {
            stateWork(list[i]);
        }
    }
}
