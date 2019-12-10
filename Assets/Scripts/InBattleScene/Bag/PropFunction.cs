using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具方法
/// </summary>
public class PropFunction : MonoBehaviour
{
    public void addHp(BattleRoleData currentActionUnit,BattleRoleData propTargetUnit, int param
        ,SDConstants.AOEType aoe = SDConstants.AOEType.None)
    {
        AddHeal heal;
        heal = gameObject.GetComponent<AddHeal>();
        if (heal == null)
        {
            heal = gameObject.AddComponent<AddHeal>();
        }
        heal.PropStartSkill(currentActionUnit, propTargetUnit, param,aoe);
    }
    public void addMp(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit, int param
        ,SDConstants.AOEType aoe = SDConstants.AOEType.None)
    {
        AddBarChart barchart;
        barchart = gameObject.GetComponent<AddBarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<AddBarChart>();
        }
        RoleBarChart nc = new RoleBarChart() { DATA = new Vector3(0, param, 0) };
        barchart.PropStartSkill(currentActionUnit, propTargetUnit, nc,aoe);
    }
    public void addTp(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit, int param
        ,SDConstants.AOEType AOEType = SDConstants.AOEType.None)
    {
        AddBarChart barchart;
        barchart = gameObject.GetComponent<AddBarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<AddBarChart>();
        }
        RoleBarChart nc = new RoleBarChart() { DATA = new Vector3(0, 0, param) };
        barchart.PropStartSkill(currentActionUnit, propTargetUnit, nc, AOEType);
    }
    public void addBarchart(BattleRoleData currentActionUnit,BattleRoleData propTargetUnit
        , List<int> param, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        AddBarChart barchart;
        barchart = gameObject.GetComponent<AddBarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<AddBarChart>();
        }

        RoleBarChart nc = new RoleBarChart() { DATA = new Vector3(param[0], param[1], param[2]) };
        barchart.PropStartSkill(currentActionUnit, propTargetUnit, nc, aoeType);
    }
    public void hpRegendPerTurn(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
        , int param0, int param1, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        HpRegend regen;
        regen = gameObject.GetComponent<HpRegend>();
        if (regen == null) regen = gameObject.AddComponent<HpRegend>();
        regen.PropStartSkill(currentActionUnit, propTargetUnit, param0, param1
            , aoeType);
    }
    public void mpRegendPerTurn(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , int param0, int param1, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        MpRegend regen;
        regen = gameObject.GetComponent<MpRegend>();
        if (regen == null) regen = gameObject.AddComponent<MpRegend>();
        regen.PropStartSkill(currentActionUnit, propTargetUnit, param0, param1
            , aoeType);
    }
    public void tpRegendPerTurn(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , int param0, int param1, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        TpRegend regen;
        regen = gameObject.GetComponent<TpRegend>();
        if (regen == null) regen = gameObject.AddComponent<TpRegend>();
        regen.PropStartSkill(currentActionUnit, propTargetUnit, param0, param1, aoeType);
    }
    public void revive(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , int param0, int param1, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        ReviveOne r;
        r = gameObject.GetComponent<ReviveOne>();
        if (r == null) r = gameObject.AddComponent<ReviveOne>();
        RoleBarChart bc = new RoleBarChart() { DATA = new Vector3(param0, param1, 0) };
        r.PropStartSkill(currentActionUnit, propTargetUnit, bc, aoeType);
    }

    public void addState(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
        , StateTag state_tag
        , int param0, int param1, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {        
        StandardState s;
        s = gameObject.GetComponent<StandardState>();
        if (s == null) s = gameObject.AddComponent<StandardState>();
        s.stateTag = state_tag;
        s.stateKind = SkillKind.Elemental;
        s.stateBreed = SkillBreed.Absence;
        List<BattleRoleData> list 
            = DealWithAOEAction(currentActionUnit, propTargetUnit, aoeType);
        for(int i = 0; i < list.Count; i++)
        {
            s.StateFunctionWork(currentActionUnit, list[i], param0, param1);
        }
    }

    public void catchSlave(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , int param)
    {
        CatchSlave c;
        c = gameObject.GetComponent<CatchSlave>();
        if (c == null) c = gameObject.AddComponent<CatchSlave>();
        c.PropStartSkill(currentActionUnit, propTargetUnit, param);
    }


    public List<BattleRoleData> DealWithAOEAction
        (BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
        , SDConstants.AOEType AOE)
    {
        BattleManager BM = FindObjectOfType<BattleManager>();
        List<BattleRoleData> results = new List<BattleRoleData>();
        if (AOE == SDConstants.AOEType.None)
        {
            results.Add(propTargetUnit);
        }
        else if (AOE == SDConstants.AOEType.Horizontal)
        {
            int childId = propTargetUnit.transform.GetSiblingIndex();
            int nextId = (childId + 2) % SDConstants.MaxSelfNum;

            results.Add(propTargetUnit);

            BattleRoleData next
                = propTargetUnit.transform.parent.GetChild(nextId)
                .GetComponent<BattleRoleData>();

            bool flag = false;
            if (next.IsDead && this.name.Contains("Revive"))
            {
                flag = true;
            }
            else if (!next.IsDead)
            {
                flag = true;
            }
            if (flag) results.Add(next);
        }
        else if (AOE == SDConstants.AOEType.Horizontal1)
        {
            BattleRoleData unit0
                = propTargetUnit.transform.parent.GetChild(0).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(2).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.Horizontal2)
        {
            BattleRoleData unit0
                = propTargetUnit.transform.parent.GetChild(1).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(3).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.Vertical)
        {
            int childId = propTargetUnit.transform.GetSiblingIndex();
            int nextId;
            if (childId > 1) { nextId = 5 - childId; }
            else { nextId = 1 - childId; }

            //this.StartSkill(source, target);
            results.Add(propTargetUnit);


            BattleRoleData next
                = propTargetUnit.transform.parent.GetChild(nextId)
                .GetComponent<BattleRoleData>();

            bool flag = false;
            if (next.IsDead && this.name.Contains("Revive"))
            {
                flag = true;
            }
            else if (!next.IsDead)
            {
                flag = true;
            }
            if (flag) results.Add(next);
            //this.StartSkill(source, next);

        }
        else if (AOE == SDConstants.AOEType.Vertical1)
        {
            BattleRoleData unit0
                = propTargetUnit.transform.parent.GetChild(2).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(3).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.Vertical2)
        {
            BattleRoleData unit0
                = propTargetUnit.transform.parent.GetChild(0).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(1).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.All)
        {
            BattleRoleData[] list
                = propTargetUnit.transform.parent
                .GetComponentsInChildren<BattleRoleData>();
            for (int i = 0; i < list.Length; i++)
            {
                bool flag = false;
                if (list[i].IsDead && this.name.Contains("Revive"))
                {
                    flag = true;
                }
                else if (!list[i].IsDead)
                {
                    flag = true;
                }
                if (flag)
                {
                    //this.StartSkill(source, list[i]);
                    results.Add(list[i]);
                }
            }
        }
        else if (AOE == SDConstants.AOEType.Random1)//随机选择一个
        {
            List<int> list = new List<int>();
            for (int i = 0; i < BM.All_Array.Count; i++)
            {
                if (BM.All_Array[i].IsDead && this.name.Contains("Revive"))
                {
                    list.Add(i);
                }
                else if (!BM.All_Array[i].IsDead) { list.Add(i); }
            }
            List<int> _results = RandomIntger.NumListReturn(1, list.Count);
            for (int i = 0; i < _results.Count; i++)
            {
                //this.StartSkill(source, BM.All_Array[list[_results[i]]]);
                results.Add(BM.All_Array[list[_results[i]]]);
            }
        }
        else if (AOE == SDConstants.AOEType.Random2)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < BM.All_Array.Count; i++)
            {
                if (BM.All_Array[i].IsDead && this.name.Contains("Revive"))
                {
                    list.Add(i);
                }
                else if (!BM.All_Array[i].IsDead) { list.Add(i); }
            }
            List<int> _results = RandomIntger.NumListReturn(2, list.Count);
            for (int i = 0; i < _results.Count; i++)
            {
                results.Add(BM.All_Array[list[_results[i]]]);
                //this.StartSkill(source, BM.All_Array[list[_results[i]]]);
            }
        }
        else if (AOE == SDConstants.AOEType.Random3)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < BM.All_Array.Count; i++)
            {
                if (BM.All_Array[i].IsDead && this.name.Contains("Revive"))
                {
                    list.Add(i);
                }
                else if (!BM.All_Array[i].IsDead) { list.Add(i); }
            }
            List<int> _results = RandomIntger.NumListReturn(3, list.Count);
            for (int i = 0; i < _results.Count; i++)
            {
                results.Add(BM.All_Array[list[_results[i]]]);
                //this.StartSkill(source, BM.All_Array[list[_results[i]]]);
            }
        }
        else if (AOE == SDConstants.AOEType.Continuous2)
        {
            results.Add(propTargetUnit);
            List<int> list = new List<int>();
            for (int i = 0; i < BM.All_Array.Count; i++)
            {
                if (BM.All_Array[i].IsDead && this.name.Contains("Revive"))
                {
                    list.Add(i);
                }
                else if (!BM.All_Array[i].IsDead) { list.Add(i); }
            }
            int index = UnityEngine.Random.Range(0, list.Count);
            results.Add(BM.All_Array[list[index]]);
        }
        else if (AOE == SDConstants.AOEType.Continuous3)
        {
            results.Add(propTargetUnit);
            List<int> list = new List<int>();
            for (int i = 0; i < BM.All_Array.Count; i++)
            {
                if (BM.All_Array[i].IsDead && this.name.Contains("Revive"))
                {
                    list.Add(i);
                }
                else if (!BM.All_Array[i].IsDead) { list.Add(i); }
            }
            int index = UnityEngine.Random.Range(0, list.Count);
            results.Add(BM.All_Array[list[index]]);

            index = UnityEngine.Random.Range(0, list.Count);
            results.Add(BM.All_Array[list[index]]);
        }
        else
        {
            results.Add(propTargetUnit);
        }
        return results;
    }
}
