using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSDamage : GoddessSkill
{
    [Space]
    [SerializeField]
    private NumberData _DamageData;
    public int UpBySkillGrade;
    public NumberData DamageData
    {
        get 
        {
            NumberData ND = _DamageData;
            ND.DATA += UpBySkillGrade * skillGrade;
            return ND;
        }
        set { _DamageData = value; }
    }
    public override void StartSkill()
    {
        base.StartSkill();
        List<BattleRoleData> list = DealWithAOEAction(TargetIsHero, AOE);
        for(int i = 0; i < list.Count; i++)
        {
            IEStartSkill(list[i], DamageData);
            stateWork(list[i]);
        }
    }

    public void IEStartSkill(BattleRoleData target, NumberData data)
    {
        int val = data.DATA;
        if(data.dataTag == NumberData.DataType.percent)
        {
            val = (int)(target.HpController.MaxHp * data.DECIMAL);
        }
        target.HpController.getDamage(val,Kind);
        //
        EndSkill(0.2f);
    }
}

[System.Serializable]
public class GSState:BasicState
{
    public NumberData DmgData;
    public void StartState(GoddessSkill GS, BattleRoleData target)
    {
        SkillGrade = GS.skillGrade;
        NAME = nameof(GS)+ "#" + GS.AOE.ToString();
        if (StatePossibility(target))
        {
            SimpleStartState(null, target);
        }
    }
    public bool StatePossibility(BattleRoleData target)
    {
        if (AimAtSelf)
        {
            Success = AllRandomSetClass.PercentIdentify(StatePossibilityBuff);
            return Success;
        }
        float rate = AllRandomSetClass.SimplePercentToDecimal
            (StatePossibilityBuff - target.ThisBasicRoleProperty().ReadRA(stateTag));
        rate = Mathf.Clamp(rate, 0.05f, 0.95f);
        if (UnityEngine.Random.Range(0, 1f) < rate)
        {
            Success = true;
        }
        return Success;
    }
    public override int dmgCaused(BattleRoleData source, BattleRoleData target)
    {
        //return base.dmgCaused(source, target);
        int val = DmgData.DATA;
        if (DmgData.dataTag == NumberData.DataType.percent)
            val = (int)(val * DmgData.DECIMAL);
        return val;
    }
}