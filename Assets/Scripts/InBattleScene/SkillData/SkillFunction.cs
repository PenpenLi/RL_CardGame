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
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public bool SkillDisable;
    protected Button _btn;
    #region 消耗设置(全部列出)
    [Header("消耗量数据")]
    public int BCCostPerTime;//技能消耗(常量)N
    public int BCCostPerLevel;//技能消耗(随角色品阶增长)N
    public int BCCostPerSkillGrade;//技能消耗(随技能等级增长)N
    public int BCCostUsingPercent;//技能消耗(固定占比)%
    #endregion
    #region 技能细节设置
    //[Header("技能细节信息"),Space(25)]
    int skillgrade;
    public int SkillGrade
    {
        get 
        {
            if (GetComponent<HSkilInfo>()) { skillgrade = GetComponent<HSkilInfo>().HSDetail.lv; }
            return skillgrade; 
        }
        set
        { 
            skillgrade = value;
            if (GetComponent<HSkilInfo>()) { GetComponent<HSkilInfo>().HSDetail.lv = skillgrade; }
        }
    }
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

    private SDConstants.AOEType _aoetype;
    public SDConstants.AOEType AOEType
    {
        get
        {
            if (GetComponent<HSkilInfo>()) 
            { _aoetype = GetComponent<HSkilInfo>().AOEType; }
            return _aoetype;
        }
        set
        {
            _aoetype = value;
            if (GetComponent<HSkilInfo>())
            { GetComponent<HSkilInfo>().AOEType = value; }
        }
    }
    public bool IsProcessing = false;
    public bool RandomTarget = false;
    //
    public HSExportDmgModifity HSModifity
    {
        get { return GetComponent<HSExportDmgModifity>(); }
    }
    #endregion

    protected HeroController _heroController;
    protected BattleManager BM;
    protected ActionPanelController APC;
    #region 动画设置
    protected float moveTowardAndBackTime = 0.1f;
    protected float castLastTime = 0.5f;
    protected float effectLastTime = 1f;
    protected float skillLastTime = 0.8f;
    protected float bulletLastTime = 0.4f;
    protected float endWaitTime = 0.5f;
    protected float shortEndWaitTime = 0.25f;
    protected float hitTime = 0.2f;
    #endregion
    [Space(25)]
    public int currentActionLevel;
    public BattleRoleData _autoBattleTarget;
    public int skillIndex = 0;
    public Transform bullet;
    //
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public bool IsCausedCritDmg;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public bool IsCausedMiss;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public bool IsCausedFault;

    public bool IsOmega = false;


    [Header("ExtraStateAdd"), Space(25)]
    public bool UseState;
#if UNITY_EDITOR
    [ConditionalHide("UseState", true)]
#endif
    public StandardState _standardState;


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
                + (int)(_heroController._role.CurrentExportRAL.Hp
                * BCCostUsingPercent * 0.01f);
        }
    }

    /// <summary>
    /// 稀有角色特殊立绘显示
    /// </summary>
    public void initIcon()
    {
        if (IsOmega)
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
        Debug.Log("Refresh This SKill:" + this.name + " State");
        bool btnEnable = true;
        #region 该技能存在使用次数限制
        if (SkillFiniteTimes > 0)
        {
            if (SFT_CurrentTimes >= SkillFiniteTimes)
            {
                btnEnable = false;
            }
            //显示当前剩余次数
        }
        #endregion
        BattleRoleData Unit = BM._currentBattleUnit;
        OneRoleClassData R = Unit.ThisBasicRoleProperty()._role;
        RoleBarChart CurrentBC = R.ReadAllMaxSSD;
        #region 该技能消耗超过角色现有量
        bool flag = CheckIfCanConsume(Unit);
        if (!flag) btnEnable = false;
        #endregion
        #region 角色状态影响技能
        if (Unit.checkPerState(StateTag.Hush))
        {
            if (this is NormalAttack)
            {
                btnEnable = true;
            }
            else
            {
                btnEnable = false;
            }
        }
        #endregion
        SkillDisable = !btnEnable;
        if (!btnEnable)//显示技能是否可以继续使用
        {
            _btn.interactable = false;
        }
        else
        {
            _btn.interactable = true;
        }
    }
    public virtual void StartSkill(BattleRoleData source,BattleRoleData target)
    {
        if (IsProcessing) return;
        currentActionLevel = SDGameManager.Instance.currentLevel;
        //BM.BtnToCloseSDP();
        source.APC.hideActionPanel();
        if (BM == null) BM = FindObjectOfType<BattleManager>();
        BM.IsWaitingPlayerAction = false;//技能释放结束，继续行动条（virtual会被abm覆盖）
    }
    /// <summary>
    /// 技能消耗
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public virtual void CalculateBeforeFunction(BattleRoleData source,BattleRoleData target)
    {
        int BC = BCCostPerTime;
        BC += BCCostPerLevel * source.ThisBasicRoleProperty().LEVEL;
        BC += BCCostPerSkillGrade * skillgrade;
        BC += (int)(BCCostUsingPercent * source.MpController.maxMp * 0.01f);
        //
        if (IsOmega)
        {
            BC = source.MpController.maxMp;
        }
        //
        source.MpController.consumeMp(BC);
        //
        if (GetComponent<HSkilInfo>())
            UseSkillAddMpTp(source, GetComponent<HSkilInfo>().AfterwardsAddType);
        else UseSkillAddMpTp(source, SDConstants.AddMpTpType.PreferMp);
    }

    public  bool CheckIfCanConsume(BattleRoleData source)
    {
        if (IsOmega)
        {
            if (source.MpController.currentMp < source.MpController.maxMp)
            {
                Debug.Log("无法消耗足量法力"); return false;
            }
        }
        else
        {
            int bc = BCCostPerTime
                + BCCostPerLevel * source.ThisBasicRoleProperty().LEVEL
                + BCCostPerSkillGrade * skillgrade
                + (int)(BCCostUsingPercent * source.MpController.maxMp * 0.01f);
            if (source.MpController.currentMp < bc && bc > 0)
            {
                Debug.Log("无法消耗足量法力"); return false;
            }
        }

        Debug.Log("技能消耗符合条件");
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
            if(this is ReviveOne)
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
            BattleRoleData R = BM._currentBattleUnit;

            bool flag = CheckIfCanConsume(R);
            s = flag;
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

    public List<BattleRoleData> DealWithAOEAction(BattleRoleData source, BattleRoleData target
        ,SDConstants.AOEType AOE
        , bool isRevive = false)
    {
        if (BM == null) BM = FindObjectOfType<BattleManager>();
        List<BattleRoleData> results = new List<BattleRoleData>();
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
        else if(AOE == SDConstants.AOEType.Vertical2)
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
            if (this.ThisSkillAim != SkillAim.All)
            {
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
            else
            {
                for (int i = 0; i < BM.All_Array.Count; i++)
                {
                    if (BM.All_Array[i].IsDead && isRevive)
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
        else if(AOE == SDConstants.AOEType.Continuous2)
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
        else if(AOE == SDConstants.AOEType.Continuous3)
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

    #region UseSkillToAddMpTp
    public void UseSkillAddMpTp(BattleRoleData source
        ,SDConstants.AddMpTpType AddType = SDConstants.AddMpTpType.Normal)
    {
        int addMp = SkillDetailsList.AddMpAfterSkill(AddType, source);
        source.MpController.addMp(addMp);
        int addTp = SkillDetailsList.AddTpAfterSkill(AddType, source);
        source.TpController.addTp(addTp);
        Debug.Log("AfterSkill AddMp:" + addMp + "||AddTp:" + addTp);
    }
    #endregion
    public virtual bool isAddSpecialBuff(BattleRoleData source, BattleRoleData target)
    {
        bool s = false;
        //if()
        return s;
    }



    public virtual int dmgCaused(BattleRoleData source,BattleRoleData target)
   {
        SkillKind kind = SkillKind.Physical;
        AttributeData Atb = AttributeData.AT;
        HSkilInfo hsi = GetComponent<HSkilInfo>();
        if (hsi)
        {
            kind = hsi.kind;
            if (hsi.UseAppointedAtb)
            {
                Atb = hsi.Atb;
            }
            else
            {
                if (kind == SkillKind.Physical) Atb = AttributeData.AT;
                else if (kind == SkillKind.Elemental) Atb = AttributeData.MT;
                else if (kind == SkillKind.Arcane) Atb = AttributeData.MT;
            }
        }
        int atk = source.ThisBasicRoleProperty().ReadRA(Atb);
        if (atk <= 0) atk = SDConstants.MinDamageCount;
        HSExportDmgModifity S = HSModifity;
        if (S)
        {

            atk = S.AllResult(source, target, kind, SkillGrade);
        }

        atk = ValChangedByCharacterType(source, target, atk);
        
        return atk;
   }
    public int ValChangedByCharacterType(BattleRoleData source,BattleRoleData target,int atk)
    {
        //英雄对敌特性触发
        if (source._Tag == SDConstants.CharacterType.Hero
            && target._Tag == SDConstants.CharacterType.Enemy)
        {
            //当技能释放者为人类目标则为野兽时触发
            if (source.HeroProperty._hero._heroRace == Race.Human
                && target.EnemyProperty._enemy.race == 3)
            {
                return (int)(atk * 1.25f);
            }
        }
        return atk;
    }


    bool stateCantWork;
    public void stateWork(BattleRoleData source,BattleRoleData target)
    {
        if (UseState && !stateCantWork)
        {
            if (_standardState != null)
            {
                SDConstants.AOEType aoe = _standardState.StateAOE;

                if (_standardState.AimAtSelf)
                {
                    target = source;
                    stateCantWork = true;
                    List<BattleRoleData> list = DealWithAOEAction(source, target
                        , _standardState.StateAOE);

                    _standardState.StartState(this, source, target);
                }
                else
                {
                    if (aoe == SDConstants.AOEType.None
                    || aoe == SDConstants.AOEType.Continuous2
                    || aoe == SDConstants.AOEType.Continuous3)
                    {
                        _standardState.StartState(this, source, target);
                    }
                    else
                    {
                        stateCantWork = true;
                        List<BattleRoleData> list = DealWithAOEAction(source, target
                            , _standardState.StateAOE);
                        foreach(BattleRoleData unit in list)
                            _standardState.StartState(this, source, unit);
                    }
                }
            }
        }
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

