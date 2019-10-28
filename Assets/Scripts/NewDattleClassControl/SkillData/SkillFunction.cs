using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SkillFunction : MonoBehaviour
{
    public bool IsActive = false;
    public bool IsUsed = false;
    
    public int SkillFiniteTimes = 0;//次数限制型技能设置
    [HideInInspector]
    public int SFT_CurrentTimes;//不断增长，当>=时触发

    protected Button _btn;
    #region 消耗设置(全部列出)
    [Header("消耗量数据")]
    public RoleBarChart BCCostPerTime;//技能消耗(常量)N
    public RoleBarChart BCCostPerLevel;//技能消耗(随角色品阶增长)N
    public RoleBarChart BCCostPerSkillGrade;//技能消耗(随技能等级增长)N
    public RoleBarChart BCCostUsingPercent;//技能消耗(固定占比)%
    #endregion
    #region 技能细节设置
    [Header("技能细节信息"),Space(25)]
    public int SkillGrade;
    public int CritR
    {
        get { return SkillDetailsR[0].Data + skillDetailsRPerSkillGrade[0].Data*SkillGrade; }
    }//暴击修正
    public int AccuracyR
    {
        get { return SkillDetailsR[1].Data + skillDetailsRPerSkillGrade[1].Data * SkillGrade; }
    }//精度修正
    public int ExpectR
    {
        get { return SkillDetailsR[2].Data + skillDetailsRPerSkillGrade[2].Data * SkillGrade; }
    }//浮动修正
    public SkillDetailR[] SkillDetailsR =
    {
        new SkillDetailR("暴击修正 CritR")
        ,new SkillDetailR("精准修正 AccuracyR")
        ,new SkillDetailR("浮动修正 ExpectR")
    };
    public SkillDetailR[] skillDetailsRPerSkillGrade =
    {
        new SkillDetailR("暴击修正CritR——增量")
        ,new SkillDetailR("精准修正AccuracyR——增量")
        ,new SkillDetailR("浮动修正ExpectR——增量")
    };
    public SkillAim ThisSkillAim;
    public SDConstants.AOEType AOEType
    {
        get
        {
            if (GetComponent<HSkilInfo>()) 
            { return GetComponent<HSkilInfo>().AOEType; }
            return SDConstants.AOEType.None;
        }
    }
    public bool IsProcessing = false;
    public bool RandomTarget = false;
    #endregion

    protected HeroController _heroController;
    protected BattleManager BM;
    protected ActionPanelController APC;
    #region 动画设置
    protected float moveTowardAndBackTime = 0.3f;
    protected float castLastTime = 0.5f;
    protected float effectLastTime = 1f;
    protected float skillLastTime = 0.8f;
    protected float bulletLastTime = 0.4f;
    protected float endWaitTime = 0.5f;
    protected float shortEndWaitTime = 0.25f;
    protected float hitTime = 0.4f;
    #endregion
    [Space(25)]
    public int currentActionLevel;
    public BattleRoleData _autoBattleTarget;
    public int skillIndex = 0;
    public Transform bullet;
    //
    public bool IsCausedCritDmg;
    public bool IsCausedMiss;
    public bool IsCausedFault;


    public bool IsRare = false;
    private void Start()
    {
        _btn = GetComponent<Button>();
        _heroController = GetComponentInParent<HeroController>();
        if(_heroController)
            APC = _heroController.GetComponentInChildren<ActionPanelController>();
        BM = FindObjectOfType<BattleManager>();
        if (_heroController)
        {
            BCCostPerTime += BCCostPerLevel * _heroController.LEVEL
                + BCCostPerSkillGrade * SkillGrade
                + _heroController._role.ReadAllMaxSSD.ExpandByRBCPercent
                (BCCostUsingPercent);
        }
    }

    /// <summary>
    /// 稀有角色特殊立绘显示
    /// </summary>
    public void initIcon()
    {
        if (IsRare)
        {
            Image img = GetComponent<Image>();
            //稀有角色拥有专用技能按钮
            //string spName = img.sprite.name.Substring(0, 10) + "_2";
            //img.sprite = 
        }
    }
    public void setUnused()
    {
        IsUsed = false;
    }
    public void refreshBtnState()
    {
        bool btnEnable = true;
        if (SkillFiniteTimes > 0)//该技能存在使用次数限制
        {
            if (SFT_CurrentTimes >= SkillFiniteTimes)
            {
                btnEnable = false;
            }
            //显示当前剩余次数
        }
        OneRoleClassData R = BM._currentBattleUnit.ThisBasicRoleProperty()._role;
        RoleBarChart CurrentBC = R.ReadAllMaxSSD;
        if (BCCostPerTime.HP
            < CurrentBC.HP
            && BCCostPerTime.MP
            <= CurrentBC.MP
            && BCCostPerTime.TP
            <= CurrentBC.TP) { btnEnable = true; }
        else { btnEnable = false; }
        if (!btnEnable)//显示技能是否可以继续使用
        {

        }
    }
    public virtual void StartSkill(BattleRoleData source,BattleRoleData target)
    {
        if (IsProcessing) return;
        if (!CheckIfCanConsume(source)) return;

        currentActionLevel = SDGameManager.Instance.currentLevel;
        //BM.BtnToCloseSDP();
        source.APC.hideActionPanel();
        if (BM == null) BM = FindObjectOfType<BattleManager>();
        BM.IsWaitingPlayerAction = false;//技能释放结束，继续行动条（virtual会被abm覆盖）
        
    }
    public  bool CheckIfCanConsume(BattleRoleData source)
    {
        if(source.HpController.CurrentHp < BCCostPerTime.HP && BCCostPerTime.HP>0)
        {
            Debug.Log("无法消耗足量生命");return false;
        }
        if(source.MpController.currentMp < BCCostPerTime.MP && BCCostPerTime.MP>0)
        {
            Debug.Log("无法消耗足量法力");return false;
        }
        if(source.TpController.currentTp < BCCostPerTime.TP && BCCostPerTime.TP>0)
        {
            Debug.Log("无法消耗足量怒气");return false;
        }
        return true;
    }
    public virtual void EndSkill()
    {
        if (_heroController)
        {

        }
        else
        {
            if (APC) APC.hideEnemyActionPanel();
        }
    }
    public IEnumerator IEWaitForEnd(BattleRoleData source)
    {
        EndSkill();
        yield return new WaitForSeconds(endWaitTime);
        IsProcessing = false;
        IsUsed = false;
        BM.startActionBar(currentActionLevel);
    }
    public IEnumerator IEWaitForShortEnd(BattleRoleData source)
    {
        EndSkill();
        yield return new WaitForSeconds(shortEndWaitTime);
        IsProcessing = false;
        IsUsed = false;
        BM.startActionBar(currentActionLevel);
    }

    public void DeactiveBtn()
    {
        IsActive = false;
    }
    public void BtnTapped()
    {
        if (SDGameManager.Instance.isUsingProp) return;
        Debug.Log(name + "Being Tapped");
        APC.DeactiveAllBtns();
        if (!IsActive)
        {
            IsActive = true;
            APC.CurrentSkill = this;
            if(this.name.Contains("Revive"))
            {
                BM.showCurrentUnitSkillTarget(true);
            }
            else
            {
                BM.showCurrentUnitSkillTarget();
                if(ThisSkillAim == SkillAim.Self)
                {
                    BM._currentBattleUnit.IsOptionTarget = true;
                    BM._currentBattleUnit.SetOptionSignState();
                }
            }
            BM.BtnToOpenSDP(this);
        }
    }
    public virtual bool isSkillAvailable()
    {
        bool s = false;
        if (SkillFiniteTimes > 0 && SFT_CurrentTimes < SkillFiniteTimes)
        {
            s = true;
        }
        else if(SkillFiniteTimes<=0)
        {
            OneRoleClassData R = BM._currentBattleUnit.ThisBasicRoleProperty()._role;
            RoleBarChart CurrentBC = R.ReadAllMaxSSD;
            if(BCCostPerTime.HP
                < CurrentBC.HP 
                && BCCostPerTime.MP
                <= CurrentBC.MP
                && BCCostPerTime.TP
                <= CurrentBC.TP)
            {
                s = true;
            }
        }
        return s;
        
    }
    public virtual bool isSkillMeetConditionToAutoRelease()
    {
        return true;
    }

    //是否满足自动释放概率
    public bool isMeetProbability(float prob)
    {
        bool s = false;
        if (UnityEngine.Random.Range(0, 1f) < prob)
        {
            s = true;
        }
        return s;
    }

    public List<BattleRoleData> DealWithAOEAction(BattleRoleData source, BattleRoleData target)
    {
        if (BM == null) BM = FindObjectOfType<BattleManager>();
        List<BattleRoleData> results = new List<BattleRoleData>();
        SDConstants.AOEType AOE = this.AOEType;
        if (AOE == SDConstants.AOEType.None)
        {
            //this.StartSkill(source, target);
            results.Add(target);
        }
        else if (AOE == SDConstants.AOEType.Horizontal)
        {
            int childId = target.transform.GetSiblingIndex();
            int nextId = (childId + 2) % SDConstants.MaxSelfNum;

            //this.StartSkill(source, target);
            results.Add(target);

            BattleRoleData next
                = target.transform.parent.GetChild(nextId)
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
        else if(AOE == SDConstants.AOEType.Horizontal1)
        {
            BattleRoleData unit0 
                = target.transform.parent.GetChild(0).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = target.transform.parent.GetChild(2).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if(AOE == SDConstants.AOEType.Horizontal2)
        {
            BattleRoleData unit0
                = target.transform.parent.GetChild(1).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = target.transform.parent.GetChild(3).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.Vertical)
        {
            int childId = target.transform.GetSiblingIndex();
            int nextId;
            if (childId > 1) { nextId = 5 - childId; }
            else { nextId = 1 - childId; }

            //this.StartSkill(source, target);
            results.Add(target);


            BattleRoleData next
                = target.transform.parent.GetChild(nextId)
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
        else if(AOE == SDConstants.AOEType.Vertical1)
        {
            BattleRoleData unit0
                = target.transform.parent.GetChild(2).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = target.transform.parent.GetChild(3).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if(AOE == SDConstants.AOEType.Vertical2)
        {
            BattleRoleData unit0
                = target.transform.parent.GetChild(0).GetComponent<BattleRoleData>();
            bool flag0 = false;
            if (unit0.IsDead && this.name.Contains("Revive")) flag0 = true;
            else if (!unit0.IsDead) flag0 = true;
            if (flag0) results.Add(unit0);
            BattleRoleData unit1
                = target.transform.parent.GetChild(1).GetComponent<BattleRoleData>();
            bool flag1 = false;
            if (unit1.IsDead && this.name.Contains("Revive")) flag1 = true;
            else if (!unit1.IsDead) flag1 = true;
            if (flag1) results.Add(unit1);
        }
        else if (AOE == SDConstants.AOEType.All)
        {
            BattleRoleData[] list
                = target.transform.parent
                .GetComponentsInChildren<BattleRoleData>();
            if (this.ThisSkillAim != SkillAim.All)
            {
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
            else
            {
                for (int i = 0; i < BM.All_Array.Count; i++)
                {
                    if (BM.All_Array[i].IsDead && this.name.Contains("Revive"))
                    {
                        //this.StartSkill(source, BM.AllSRL_Array[i]);
                        results.Add(BM.AllSRL_Array[i]);
                    }
                    else if (!BM.All_Array[i].IsDead)
                    {
                        //this.StartSkill(source, BM.AllSRL_Array[i]);
                        results.Add(BM.AllSRL_Array[i]);
                    }

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
        else if(AOE == SDConstants.AOEType.Continuous2)
        {
            results.Add(target);
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
        else if(AOE == SDConstants.AOEType.Continuous3)
        {
            results.Add(target);
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
            results.Add(target);
            //this.StartSkill(source, target);
        }
        return results;
    }

    #region UseSkillToAddMpTp
    /// <summary>
    /// 使用技能后能够获得的技力
    /// </summary>
    /// <param name="addType">回能比率，越高回能数量越低</param>
    public void UseSkillToAddMp(BattleRoleData source,int addF = 15)
    {
        source.MpController.addMp(SDDataManager.Instance.FigureAByPc
            (source.MpController.maxMp / addF, source.ThisBasicRoleProperty().MpAddRate));
    }
    /// <summary>
    /// 使用技能增加的怒气值
    /// </summary>
    /// <param name="source"></param>
    /// <param name="addF">回怒比率，越高回怒数量越低</param>
    public void UseSkillToAddTp(BattleRoleData source,int addF = 25)
    {
        source.TpController.addTp
            (SDDataManager.Instance.FigureAByPc(source.TpController.maxTp / addF
            , source.ThisBasicRoleProperty().TPAddRate));//技能增加Tp 
    }
    public void UseSkillAddMpTp(BattleRoleData source
        ,SDConstants.AddMpTpType AddType = SDConstants.AddMpTpType.Normal)
    {
        if (AddType == SDConstants.AddMpTpType.Normal)
        {
            UseSkillToAddMp(source, 15);
            UseSkillToAddTp(source, 25);
        }
        else if (AddType == SDConstants.AddMpTpType.PreferMp)
        {
            UseSkillToAddMp(source, 10);
            UseSkillToAddTp(source, 25);
        }
        else if (AddType == SDConstants.AddMpTpType.PreferTp)
        {
            UseSkillToAddMp(source, 15);
            UseSkillToAddTp(source, 15);
        }
        else if (AddType == SDConstants.AddMpTpType.PreferBoth)
        {
            UseSkillToAddMp(source, 10);
            UseSkillToAddTp(source, 15);
        }
        else if (AddType == SDConstants.AddMpTpType.LowMp)
        {
            //UseSkillToAddMp(source, 25);
            UseSkillToAddTp(source, 25);
        }
        else if(AddType == SDConstants.AddMpTpType.LowTp)
        {
            UseSkillToAddMp(source, 15);
            //UseSkillToAddTp(source, 25);
        }
        else if(AddType == SDConstants.AddMpTpType.LowBoth)
        {

        }
        else if(AddType == SDConstants.AddMpTpType.YearnMp)
        {
            UseSkillToAddMp(source, 5);
            UseSkillToAddTp(source, 25);
        }
        else if(AddType == SDConstants.AddMpTpType.YearnTp)
        {
            UseSkillToAddMp(source, 15);
            UseSkillToAddTp(source, 10);
        }
        else if(AddType == SDConstants.AddMpTpType.YearnBoth)
        {
            UseSkillToAddMp(source, 5);
            UseSkillToAddTp(source, 10);
        }
    }
    #endregion


    public virtual bool isAddSpecialBuff(BattleRoleData source, BattleRoleData target)
    {
        bool s = false;
        //if()
        return s;
    }
}


[System.Serializable]
public class SkillDetailR
{
    public string name;
    public int Data;
    public SkillDetailR(string name,int data = 0)
    {
        this.name = name;
        this.Data = data;
    }
}