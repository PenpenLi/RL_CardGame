using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具方法
/// </summary>
public class PropFunction : MonoBehaviour
{
    bool UseState;
    StandardState _state = new StandardState();
    List<string> functionsWithoutState = new List<string>();
    public void checkSpecialStr(string specialStr,string propid)
    {
        UseState = false;
        _state = new StandardState(propid);
        functionsWithoutState = new List<string>();
        //state
        int lastTime = 0;
        List<StandardState.ChangeInRAL> ralList = new List<StandardState.ChangeInRAL>();
        //
        string[] strings = specialStr.Split('|');
        for(int i = 0; i < strings.Length; i++)
        {
            string[] tmp = strings[i].Split(':');
            string fn = tmp[0];
            string[] paramStr = tmp[1].Split(',');
            if(fn.Contains("add") && paramStr.Length > 1)
            {
                UseState = true;
                int time = SDDataManager.Instance.getInteger(paramStr[1]);
                lastTime = Mathf.Max(lastTime, time);
                int d = SDDataManager.Instance.getInteger(paramStr[0]);
                NumberData D = new NumberData(d);
                if (fn.Contains("_pc")) D.dataTag = NumberData.DataType.percent;
                //
                if (fn.Contains("HP")) _state.ChangeInBarChart.HP = D;
                else if (fn.Contains("MP")) _state.ChangeInBarChart.MP = D;
                else if (fn.Contains("TP")) _state.ChangeInBarChart.TP = D;
            }
            else if (fn.Contains("up_"))
            {
                UseState = true;
                int time = SDDataManager.Instance.getInteger(paramStr[1]);
                lastTime = Mathf.Max(lastTime, time);
                int d = SDDataManager.Instance.getInteger(paramStr[0]);
                NumberData D = new NumberData(d);
                if (fn.Contains("_pc")) D.dataTag = NumberData.DataType.percent;
                StandardState.ChangeInRAL cral;
                bool isad = ROHelp.CheckStringIsADElseST(fn.Split('_')[1]
                    , out AttributeData ad, out StateTag st);
                if (isad)
                {
                    cral = new StandardState.ChangeInRAL(D, ad);
                }
                else
                {
                    cral = new StandardState.ChangeInRAL(D, st);
                }
                ralList.Add(cral);
            }
            else if (fn.Contains("debuff_"))
            {
                StateTag tag = ROHelp.STATE_TAG(fn.Split('_')[1]);
                if(fn.Split('_')[1].ToLower() == "random")
                {
                    tag = (StateTag)(UnityEngine.Random.Range(0, (int)StateTag.End));
                }
                _state.stateTag = tag;
            }
            else if (fn.Contains("stateDmg"))
            {
                UseState = true;
                int time = SDDataManager.Instance.getInteger(paramStr[1]);
                lastTime = Mathf.Max(lastTime, time);
                int d = SDDataManager.Instance.getInteger(paramStr[0]);
                NumberData D = new NumberData(d);
                if (fn.Contains("_pc")) D.dataTag = NumberData.DataType.percent;
                _state.ExtraDmg = D;
            }
            //其余转为直接影响列表
            else
            {
                functionsWithoutState.Add(strings[i]);
            }
        }
    }
    void UsePropWithoutState(string FN,SDConstants.AOEType AOE
        , BattleRoleData currentActionUnit, BattleRoleData propTargetUnit)
    {
        string[] fns = FN.Split(':');
        string[] fnElements = fns[0].Split('_');
        string[] paramstrs = fns[1].Split(',');
        int p0 = 0; int p1 = 0;
        p0 = SDDataManager.Instance.getInteger(paramstrs[0]);
        if (paramstrs.Length > 1) p1 = SDDataManager.Instance.getInteger(paramstrs[1]);

        NumberData param0 = new NumberData(p0);NumberData param1 = new NumberData(p1);
        string functionName = fns[0];
        if (functionName.Contains("addHP"))
        {
            addHp(currentActionUnit, propTargetUnit, param0
                , AOE);
        }
        else if (functionName.Contains("addMP"))
        {
            addMp(currentActionUnit, propTargetUnit, param0
                , AOE);
        }
        else if (functionName.Contains("addTP"))
        {
            addTp(currentActionUnit, propTargetUnit, param0
                , AOE);
        }
        else if (functionName.Contains( "revive"))
        {
            bool flag = false;
            if (propTargetUnit.IsDead)
            {
                flag = true;
            }
            else
            {
                List<BattleRoleData> lit = DealWithAOEAction(currentActionUnit
                    , propTargetUnit, AOE);
                foreach (BattleRoleData unit in lit)
                {
                    if (unit.IsDead)
                    {
                        flag = true;
                        propTargetUnit = unit;
                        break;
                    }
                }
            }
            if (flag)
            {
                revive(currentActionUnit, propTargetUnit, param0, param1
                    , AOE);
            }
        }
        else if (functionName.Contains("catch"))
        {
            if (!propTargetUnit.IsDead
                && propTargetUnit.HpController.CurrentHp * 1f / propTargetUnit.HpController.MaxHp
                <= 0.2f)
            {
                catchSlave(currentActionUnit, propTargetUnit, param0);
            }
        }
        else if (functionName.Contains("remove"))
        {
            List<BattleRoleData> list = DealWithAOEAction
                    (currentActionUnit, propTargetUnit, AOE);
            if (fnElements[1].ToLower() != "all")
            {
                StateTag tag = ROHelp.STATE_TAG(fnElements[1]);
                if (fnElements[1].ToLower() == "random")
                {
                    tag = (StateTag)(UnityEngine.Random.Range(0, (int)StateTag.End));
                }               
                for(int i = 0; i < list.Count; i++)
                {
                    list[i].clearPerTagStates(tag);
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].clearAllStates();
                }
            }

        }
        else if (functionName.Contains("damage"))
        {
            damage(currentActionUnit, propTargetUnit, param0, AOE);
        }
    }

    public void UseProp(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
        ,SDConstants.AOEType AOE)
    {
        //立即触发之效果
        for(int i = 0; i < functionsWithoutState.Count; i++)
        {
            UsePropWithoutState(functionsWithoutState[i], AOE, currentActionUnit, propTargetUnit);
        }
        //建立道具添加的状态
        if (UseState)
        {
            addState(currentActionUnit, propTargetUnit, _state, AOE);
        }
        //
        
    }

    #region AllFunctions
    public void addHp(BattleRoleData currentActionUnit,BattleRoleData propTargetUnit, NumberData param
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
    public void addMp(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit, NumberData param
        ,SDConstants.AOEType aoe = SDConstants.AOEType.None)
    {
        AddBarChart barchart;
        barchart = gameObject.GetComponent<AddBarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<AddBarChart>();
        }
        NDBarChart nc = NDBarChart.zero;
        nc.MP = param;
        barchart.PropStartSkill(currentActionUnit, propTargetUnit, nc,aoe);
    }
    public void addTp(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit, NumberData param
        ,SDConstants.AOEType AOEType = SDConstants.AOEType.None)
    {
        AddBarChart barchart;
        barchart = gameObject.GetComponent<AddBarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<AddBarChart>();
        }
        NDBarChart nc = NDBarChart.zero;
        nc.TP = param;
        barchart.PropStartSkill(currentActionUnit, propTargetUnit, nc, AOEType);
    }
    public void addBarchart(BattleRoleData currentActionUnit,BattleRoleData propTargetUnit
        , List<NumberData> param, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        AddBarChart barchart;
        barchart = gameObject.GetComponent<AddBarChart>();
        if (barchart == null)
        {
            barchart = gameObject.AddComponent<AddBarChart>();
        }

        NDBarChart nc = NDBarChart.zero;
        nc.HP = param[0];nc.MP = param[1];nc.TP = param[2];
        barchart.PropStartSkill(currentActionUnit, propTargetUnit, nc, aoeType);
    }
    public void revive(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , NumberData param0, NumberData param1, SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        ReviveOne r;
        r = gameObject.GetComponent<ReviveOne>();
        if (r == null) r = gameObject.AddComponent<ReviveOne>();
        NDBarChart bc = NDBarChart.Build(param0, param1, NumberData.zero);
        r.PropStartSkill(currentActionUnit, propTargetUnit, bc, aoeType);
    }
    public void addState(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
        , StandardState state
        , SDConstants.AOEType aoeType = SDConstants.AOEType.None)
    {
        if (state.AimAtSelf)
        {
            propTargetUnit = currentActionUnit;
        }
        List<BattleRoleData> list 
            = DealWithAOEAction(currentActionUnit, propTargetUnit, aoeType);
        for(int i = 0; i < list.Count; i++)
        {
            state.SimpleStartState(currentActionUnit, list[i]);
        }
    }
    public void catchSlave(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , NumberData param)
    {
        CatchSlave c;
        c = gameObject.GetComponent<CatchSlave>();
        if (c == null) c = gameObject.AddComponent<CatchSlave>();
        c.PropStartSkill(currentActionUnit, propTargetUnit, param);
    }
    public void damage(BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
    , NumberData param,SDConstants.AOEType aoe = SDConstants.AOEType.None)
    {
        NormalAttack n;
        n = gameObject.GetComponent<NormalAttack>();
        if (n == null) n = gameObject.AddComponent<NormalAttack>();
        n.PropStartSkill(currentActionUnit, propTargetUnit, param, aoe);
    }
    #endregion

    public List<BattleRoleData> DealWithAOEAction
        (BattleRoleData currentActionUnit, BattleRoleData propTargetUnit
        , SDConstants.AOEType AOE, bool isRevive = false)
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
            if (next.IsDead && isRevive)
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
            if (unit0.IsDead && isRevive) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(2).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && isRevive) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.Horizontal2)
        {
            BattleRoleData unit0
                = propTargetUnit.transform.parent.GetChild(1).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && isRevive) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(3).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && isRevive) flag1 = true;
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
            if (next.IsDead && isRevive)
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
            if (unit0.IsDead && isRevive) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(3).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && isRevive) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.Vertical2)
        {
            BattleRoleData unit0
                = propTargetUnit.transform.parent.GetChild(0).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && isRevive) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = propTargetUnit.transform.parent.GetChild(1).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && isRevive) flag1 = true;
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
                if (list[i].IsDead && isRevive)
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
                if (BM.All_Array[i].IsDead && isRevive)
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
                if (BM.All_Array[i].IsDead && isRevive)
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
                if (BM.All_Array[i].IsDead && isRevive)
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
                if (BM.All_Array[i].IsDead && isRevive)
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
                if (BM.All_Array[i].IsDead && isRevive)
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
