using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoddessSkill:MonoBehaviour
{
    public bool IsProcessing = false;
    public BattleManager BM;
    [Space]
    public bool TargetIsHero;
    public SDConstants.AOEType AOE;
    public int skillGrade;
    public SkillBreed Breed = SkillBreed.Absence;
    public SkillKind Kind = SkillKind.End;

    [Space]
    public bool UseState;
    [ConditionalHide("UseState", true, false)]
    public GSState State;
    public virtual void StartSkill()
    {
        if (!BM) BM = FindObjectOfType<BattleManager>();
        if (IsProcessing) return;
    }
    public List<BattleRoleData> DealWithAOEAction
        (bool targetIsHero, SDConstants.AOEType AOE, bool isRevive = false)
    {
        if (!BM) BM = FindObjectOfType<BattleManager>();

        List<BattleRoleData> results = new List<BattleRoleData>();
        List<BattleRoleData> All;
        BattleRoleData target;
        if (isRevive)
        {
            All = targetIsHero ? BM.AllSRL_Array : BM.AllORL_Array;
            List<BattleRoleData> _all = All.FindAll(x => x.IsDead);
            All = _all;
            target = All[UnityEngine.Random.Range(0, All.Count)];
        }
        else
        {
            All = targetIsHero ? BM.Remaining_SRL : BM.Remaining_ORL;
            target = All[UnityEngine.Random.Range(0, All.Count)];
        }

        if (AOE == SDConstants.AOEType.None)
        {
            //this.StartSkill(source, target);
            results.Add(target);
        }
        else if (AOE == SDConstants.AOEType.Horizontal)
        {
            string _N = target.gameObject.name;
            if (_N.Contains("POS8"))
            {
                results.Add(target);
            }
            else
            {
                Transform _parent = target.transform.parent;
                for (int i = 0; i < _parent.childCount; i++)
                {
                    if (_N.Contains("POS0") || _N.Contains("POS2") || _N.Contains("POS6")
                        || _N.Contains("POS4") || _N.Contains("POS5"))
                    {
                        string N = _parent.GetChild(i).name;
                        if (N.Contains("POS0") || N.Contains("POS2") || N.Contains("POS6")
                            || N.Contains("POS4") || N.Contains("POS5") || N.Contains("POS8"))
                        {
                            BattleRoleData unit0
                        = _parent.GetChild(i).GetComponent<BattleRoleData>();
                            bool flag0 = false;
                            if (unit0.IsDead && isRevive) flag0 = true;
                            else if (!unit0.IsDead) flag0 = true;
                            if (flag0) results.Add(unit0);
                        }
                    }
                    else if (_N.Contains("POS1") || _N.Contains("POS3") || _N.Contains("POS7")
                        || _N.Contains("POS4") || _N.Contains("POS5"))
                    {
                        string N = _parent.GetChild(i).name;
                        if (N.Contains("POS1") || N.Contains("POS3") || N.Contains("POS7")
                            || N.Contains("POS4") || N.Contains("POS5") || N.Contains("POS8"))
                        {
                            BattleRoleData unit0
                                = _parent.GetChild(i).GetComponent<BattleRoleData>();
                            bool flag0 = false;
                            if (unit0.IsDead && isRevive) flag0 = true;
                            else if (!unit0.IsDead) flag0 = true;
                            if (flag0) results.Add(unit0);
                        }
                    }
                }
            }

        }
        else if (AOE == SDConstants.AOEType.Horizontal1)
        {
            Transform _parent = target.transform.parent;

            for (int i = 0; i < _parent.childCount; i++)
            {
                string N = _parent.GetChild(i).name;
                if (N.Contains("POS0") || N.Contains("POS2") || N.Contains("POS6")
                   || N.Contains("POS4") || N.Contains("POS5") || N.Contains("POS8"))
                {
                    BattleRoleData unit0
                = _parent.GetChild(i).GetComponent<BattleRoleData>();
                    bool flag0 = false;
                    if (unit0.IsDead && isRevive) flag0 = true;
                    else if (!unit0.IsDead) flag0 = true;
                    if (flag0) results.Add(unit0);
                }
            }
        }
        else if (AOE == SDConstants.AOEType.Horizontal2)
        {
            Transform _parent = target.transform.parent;

            for (int i = 0; i < _parent.childCount; i++)
            {
                string N = _parent.GetChild(i).name;
                if (N.Contains("POS1") || N.Contains("POS3") || N.Contains("POS7")
                   || N.Contains("POS4") || N.Contains("POS5") || N.Contains("POS8"))
                {
                    BattleRoleData unit0
                = _parent.GetChild(i).GetComponent<BattleRoleData>();
                    bool flag0 = false;
                    if (unit0.IsDead && isRevive) flag0 = true;
                    else if (!unit0.IsDead) flag0 = true;
                    if (flag0) results.Add(unit0);
                }
            }
        }
        else if (AOE == SDConstants.AOEType.Vertical)
        {
            string _N = target.gameObject.name;
            if (_N.Contains("POS8"))
            {
                results.Add(target);
            }
            else
            {
                Transform _parent = target.transform.parent;
                for (int i = 0; i < _parent.childCount; i++)
                {
                    //bool exit = false;
                    if (_N.Contains("POS2") || _N.Contains("POS3") || _N.Contains("POS5")
                        || _N.Contains("POS6") || _N.Contains("POS7"))
                    {
                        string N = _parent.GetChild(i).name;
                        if (N.Contains("POS2") || N.Contains("POS3") || N.Contains("POS5")
                           || N.Contains("POS6") || N.Contains("POS7") || N.Contains("POS8"))
                        {
                            BattleRoleData unit0
                        = _parent.GetChild(i).GetComponent<BattleRoleData>();
                            bool flag0 = false;
                            if (unit0.IsDead && isRevive) flag0 = true;
                            else if (!unit0.IsDead) flag0 = true;
                            if (flag0) results.Add(unit0);
                        }
                    }
                    else if (_N.Contains("POS0") || _N.Contains("POS1") || _N.Contains("POS4")
                        || _N.Contains("POS6") || _N.Contains("POS7"))
                    {
                        string N = _parent.GetChild(i).name;
                        if (N.Contains("POS0") || N.Contains("POS1") || N.Contains("POS4")
                            || N.Contains("POS6") || N.Contains("POS7") || N.Contains("POS8"))
                        {
                            BattleRoleData unit0
                                = _parent.GetChild(i).GetComponent<BattleRoleData>();
                            bool flag0 = false;
                            if (unit0.IsDead && isRevive) flag0 = true;
                            else if (!unit0.IsDead) flag0 = true;
                            if (flag0) results.Add(unit0);
                        }
                    }
                }
            }

        }
        else if (AOE == SDConstants.AOEType.Vertical1)
        {
            Transform _parent = target.transform.parent;

            for (int i = 0; i < _parent.childCount; i++)
            {
                string N = _parent.GetChild(i).name;
                if (N.Contains("POS2") || N.Contains("POS3") || N.Contains("POS5")
                    || N.Contains("POS6") || N.Contains("POS7") || N.Contains("POS8"))
                {
                    BattleRoleData unit0
                    = _parent.GetChild(i).GetComponent<BattleRoleData>();
                    bool flag0 = false;
                    if (unit0.IsDead && isRevive) flag0 = true;
                    else if (!unit0.IsDead) flag0 = true;
                    if (flag0) results.Add(unit0);
                }
            }
        }
        else if (AOE == SDConstants.AOEType.Vertical2)
        {
            Transform _parent = target.transform.parent;
            for (int i = 0; i < _parent.childCount; i++)
            {
                string N = _parent.GetChild(i).name;
                if (N.Contains("POS0") || N.Contains("POS1") || N.Contains("POS4")
                    || N.Contains("POS6") || N.Contains("POS7") || N.Contains("POS8"))
                {
                    BattleRoleData unit0
                        = _parent.GetChild(i).GetComponent<BattleRoleData>();
                    bool flag0 = false;
                    if (unit0.IsDead && isRevive) flag0 = true;
                    else if (!unit0.IsDead) flag0 = true;
                    if (flag0) results.Add(unit0);
                }
            }
        }
        else if (AOE == SDConstants.AOEType.All)
        {
            BattleRoleData[] list
                = target.transform.parent
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
            results.Add(target);
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
            results.Add(target);
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
            results.Add(target);
            //this.StartSkill(source, target);
        }
        return results;
    }

    public virtual void EndSkill(float endtime)
    {

    }

    bool stateCantWork;
    public void stateWork(BattleRoleData target)
    {
        if (UseState && !stateCantWork && State != null)
        {
            if (State.AimAtSelf)
            {
                stateCantWork = true;
                //

            }
            else
            {
                if (AOE == SDConstants.AOEType.None
                    || AOE == SDConstants.AOEType.Continuous2
                    || AOE == SDConstants.AOEType.Continuous3)
                {
                    State.StartState(this, target);
                }
                else
                {
                    stateCantWork = true;
                    List<BattleRoleData> list = DealWithAOEAction(TargetIsHero, AOE);
                    foreach (BattleRoleData unit in list)
                        State.StartState(this, unit);
                }
            }
        }
    }
}
