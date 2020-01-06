using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSHeal : GoddessSkill
{
    [Space]
    [SerializeField]
    private NumberData _HealData;
    public int UpBySkillGrade;
    public NumberData HealData
    {
        get
        {
            NumberData ND = _HealData;
            ND.DATA += UpBySkillGrade * skillGrade;
            return ND;
        }
        set { _HealData = value; }
    }
    public override void StartSkill()
    {
        base.StartSkill();
        List<BattleRoleData> list = DealWithAOEAction(TargetIsHero, AOE);
        for (int i = 0; i < list.Count; i++)
        {
            IEStartSkill(list[i], HealData);
            stateWork(list[i]);
        }
    }
    public void IEStartSkill(BattleRoleData target,NumberData data)
    {
        int val = data.DATA;
        if (data.dataTag == NumberData.DataType.percent)
        {
            val = (int)(target.HpController.MaxHp * data.DECIMAL);
        }
        target.HpController.addHp(val);
        //
        EndSkill(0.2f);
    }
}
