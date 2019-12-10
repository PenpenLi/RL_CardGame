using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 关卡管理器
/// </summary>
public class BattleManager : MonoBehaviour
{
    public BattleRoleData _currentBattleUnit;//当前行动单元
    public BattleRoleData _currentTargetUnit;//当前行动单元目标
    public SDConstants.CharacterType _currentBUType;//当前行动单元标签
    public SDConstants.CharacterType _currentTUType;//当前目标单元标签

    public ActionPanelController _currentActionPanel;
    public SkillFunction _currentSkill;

    public SkillFunction normalAttackSkill;
    #region 战斗中全角色列表
    public List<BattleRoleData> All_Array;
    public List<BattleRoleData> AllSRL_Array;
    public List<BattleRoleData> AllORL_Array;
    public List<BattleRoleData> Remaining_SRL;
    public List<BattleRoleData> Remaining_ORL;
    #endregion
    [HideInInspector]
    public ActionBarManager ABM;
    [HideInInspector]
    public GameController GC;
    [HideInInspector]
    public SkillDetailsList SDL;
    public Transform[] HeroStatusGroup;
    [Header("战斗阶段控制")]
    public bool IsWaitingPlayerAction = false;
    public Transform ActionPanelPos;
    public Transform ActionPanelHidePos;
    [HideInInspector]
    public float CountDownTime = 0f;
    public bool ActionConfirm;//行动确认(所有内容选定完成)
    int confirmSkillStep = 2;
    public float WaitForNextUnitTime;
    #region 道具
    [Header("使用道具时设定")]
    //public SkillAim PropTarget;
    public string _PropTarget;
    public string _PropRange;
    public string PropFunictionName;
    public int param0;
    public int param1;
    public BattleRoleData PropTargetUnit;
    public Button UsingPropBtn;
    public Button CancelBtn;
    public Transform PropPanel;
    public BagController PropBag;
    public PropFunction PF;
    #endregion
    #region 动画设置
    protected float showAndHideTime = 0.1f;
    private float WAIT_FOR_NEXT_UNIT_TIME = 0.1f;
    #endregion
    #region 选择目标交互设置
    [Header("目标选择器"), Space(25)]
    public QuicklySelectSkillTarget SelectController;
    #endregion
    //public Transform skillDetailPanel;
    private void Awake()
    {
        ABM = GetComponentInChildren<ActionBarManager>();
        GC = GetComponentInParent<GameController>();
        SDL = GetComponentInChildren<SkillDetailsList>();
    }
    #region 显示或隐藏技能信息面板
    public void BtnToCloseSDP()
    {
        _currentActionPanel.CloseSDP();
    }
    //开启面板（成功选择技能）
    public void BtnToOpenSDP(SkillFunction skill)
    {
        if (skill.skillIndex >= 0)
        {
            _currentActionPanel.OpenSDP(skill);

        }
        else
        {
            BtnToCloseSDP();
        }
    }
    #endregion
    public void BattleInit()
    {
        SDGameManager.Instance.isGaming = true;
        IsWaitingPlayerAction = false;
        All_Array = new List<BattleRoleData>();
        AllSRL_Array = new List<BattleRoleData>();
        AllORL_Array = new List<BattleRoleData>();
        Remaining_SRL = new List<BattleRoleData>();
        Remaining_ORL = new List<BattleRoleData>();
        GameObject[] heros = GameObject.FindGameObjectsWithTag(SDConstants.HERO_TAG);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(SDConstants.ENEMY_TAG);
        foreach(GameObject obj in heros)
        {
            BattleRoleData unit = obj.GetComponentInParent<BattleRoleData>();
            unit.BM = this;
            AllSRL_Array.Add(unit);
            if (!unit.IsDead) Remaining_SRL.Add(unit);
            All_Array.Add(unit);
        }
        foreach(GameObject obj in enemies)
        {
            BattleRoleData unit = obj.GetComponentInParent<BattleRoleData>();
            unit.BM = this;
            AllORL_Array.Add(unit);
            if (!unit.IsDead) Remaining_ORL.Add(unit);
            All_Array.Add(unit);
        }
        ListUnitActionOrder();
        ABM.initAction(All_Array);
        GC.checkHeroesStatus();
    }
    public void ListUnitActionOrder()
    {
        for(int i = 0; i < All_Array.Count; i++)
        {
            for(int j = i + 1; j < All_Array.Count; j++)
            {
                int speed_i = All_Array[i].ThisBasicRoleProperty()._role.speed;
                int speed_j = All_Array[j].ThisBasicRoleProperty()._role.speed;
                if (speed_j > speed_i) swapTwoUnit(i, j);
            }
        }
    }
    /// <summary>
    /// 调换两个单位的编号
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public void swapTwoUnit(int i, int j)
    {
        BattleRoleData tmp = All_Array[i];
        All_Array[i] = All_Array[j];
        All_Array[j] = tmp;
    }

    #region 行动目标设置
    public void showCurrentUnitSkillTarget(bool showDieTarget = false)
    {
        SkillAim aim = SkillAim.Other;
        if (_currentActionPanel!=null && _currentActionPanel.CurrentSkill != null)
        {
            aim = _currentActionPanel.CurrentSkill.ThisSkillAim;
        }
        int t = 0;
        if(aim == SkillAim.Friend )
        {
            t = _currentBattleUnit.IsEnemy ? 1 : -1;
        }
        else if(aim == SkillAim.Other)
        {
            t = _currentBattleUnit.IsEnemy ? -1 : 1;
        }
        if (aim == SkillAim.Self)
        {
            for (int i = 0; i < AllSRL_Array.Count; i++)
            {
                AllSRL_Array[i].IsOptionTarget = false;
                AllSRL_Array[i].SetOptionSignState();
            }
            for (int i = 0; i < AllORL_Array.Count; i++)
            {
                AllORL_Array[i].IsOptionTarget = false;
                AllORL_Array[i].SetOptionSignState();
            }
            _currentBattleUnit.IsOptionTarget = true;
            _currentBattleUnit.SetOptionSignState();
        }
        else
        {
            if (t == -1)
            {
                for (int i = 0; i < AllSRL_Array.Count; i++)
                {
                    if (AllSRL_Array[i].IsDead && !showDieTarget)
                    {
                        AllSRL_Array[i].IsOptionTarget = false;
                    }
                    else
                    {
                        AllSRL_Array[i].IsOptionTarget = true;
                    }
                    AllSRL_Array[i].SetOptionSignState();
                }
                for (int i = 0; i < AllORL_Array.Count; i++)
                {
                    AllORL_Array[i].IsOptionTarget = false;
                    AllORL_Array[i].SetOptionSignState();
                }
            }
            else if (t == 1)
            {
                for (int i = 0; i < AllORL_Array.Count; i++)
                {
                    if (AllORL_Array[i].IsDead && !showDieTarget)
                    {
                        AllORL_Array[i].IsOptionTarget = false;
                    }
                    else
                    {
                        AllORL_Array[i].IsOptionTarget = true;
                    }
                    AllORL_Array[i].SetOptionSignState();
                }
                for (int i = 0; i < AllSRL_Array.Count; i++)
                {
                    AllSRL_Array[i].IsOptionTarget = false;
                    AllSRL_Array[i].SetOptionSignState();
                }
            }
            else //skillAim.all 触发
            {
                for (int i = 0; i < AllSRL_Array.Count; i++)
                {
                    if (AllSRL_Array[i].IsDead && !showDieTarget)
                    {
                        AllSRL_Array[i].IsOptionTarget = false;
                    }
                    else
                    {
                        AllSRL_Array[i].IsOptionTarget = true;
                    }
                    AllSRL_Array[i].SetOptionSignState();
                }
                for (int i = 0; i < AllORL_Array.Count; i++)
                {
                    if (AllORL_Array[i].IsDead && !showDieTarget)
                    {
                        AllORL_Array[i].IsOptionTarget = false;
                    }
                    else
                    {
                        AllORL_Array[i].IsOptionTarget = true;
                    }
                    AllORL_Array[i].SetOptionSignState();
                }
            }
        }
       
    }
    public void showPropTarget(bool showDieTarget = false)
    {

    }
    public void hideOptionTarget()
    {
        for(int i = 0; i < All_Array.Count; i++)
        {
            All_Array[i].IsOptionTarget = false;
            All_Array[i].SetOptionSignState();
        }
    }
    #endregion
    public void WhenCurrentSkillBeingUsed()
    {
        //角色释放技能动画

        //
        ActionConfirm = true;
    }
    void FixedUpdate()
    {
        //IsWaitingPlayerAction = ABM.IsWaitingAction;
        if (CountDownTime > 0)
        {
            CountDownTime -= Time.deltaTime;
            return;
        }
        if (IsWaitingPlayerAction)
        {
            if (SDDataManager.Instance.SettingData.isAutoBattle 
                && _currentActionPanel != null)
            {
                IsWaitingPlayerAction = false;

                _currentActionPanel.hideActionPanel();

                UsingPropBtn.interactable = false;
                CancelBtn.interactable = false;

                _currentActionPanel.chooseASkillFromSkillGroup();

                //chooseAutoBattleTarget();
                if (_currentBattleUnit.IsEnemy) chooseRandomHeroToAttack();
                else chooseAutoBattleTarget();

                CountDownTime = 1.5f;
                handleAction();
            }
            else
            {
                if (SDGameManager.Instance.isUsingProp)//正在使用道具
                {
                    if (_PropTarget == SDConstants.AUTO_TARGET_TAG)
                    {
                        if (Input.GetMouseButtonDown(0)) ActionConfirm = true;
                    }
                    else
                    {
                        //SelectController.GetComponent<Image>().raycastTarget = true;
                        //使用QuicklySelectSkillTarget来选择和判断角色
                        if (Input.GetMouseButtonDown(0))
                        {
                            int touchId = SelectController.WhenPointDownIndex();
                            if (All_Array[touchId].IsOptionTarget)
                            {
                                ActionConfirm = true;
                                PropTargetUnit = All_Array[touchId];
                                SelectController.ConfirmTarget(touchId);
                            }
                            else
                            {
                                RolePlayJumpAnim(All_Array[touchId]);
                                Debug.Log("无效目标");
                            }
                        }
                    }
                    if (ActionConfirm)
                    {
                        ActionConfirm = false;
                        CountDownTime = 0.5f;
                        startUseProp(PropFunictionName);
                        CancelBtn.interactable = false;
                        hideOptionTarget();
                        SelectController.ResetTarget();
                        //handleAction();
                    }
                }
                else
                {
                    if (!SDGameManager.Instance.isGamePaused && confirmSkillStep != 2)
                    {
                        if (ActionConfirm && confirmSkillStep == 1)
                        {
                            ActionConfirm = false;
                            confirmSkillStep = 2;
                            CountDownTime = 0.75f;
                            //
                            Debug.Log("目标为：" + _currentTUType + " name:" + _currentTargetUnit.name);
                            RolePlayJumpAnim(_currentTargetUnit);
                            handleAction();
                        }
                    }
                }
            }
        }
    }
    int currentTouchId = -1;
    public void ConfirmSkillAndTarget()
    {
        ActionConfirm = false;
        _currentSkill = _currentActionPanel.CurrentSkill;
        int touchId = SelectController.WhenPointDownIndex();

        if (_currentSkill.ThisSkillAim == SkillAim.Self || _currentSkill.RandomTarget)
        {
            if (confirmSkillStep == 0) confirmSkillStep = 1;
            else if(confirmSkillStep == 1)
            {
                ActionConfirm = true;
                _currentTargetUnit = All_Array[touchId];
                _currentTUType = _currentTargetUnit._Tag;
                //
                if (_currentSkill.ThisSkillAim != SkillAim.Self)
                    SelectController.ConfirmTarget(touchId);
                else currentTouchId = 0;
            }

        }
        else
        {
            if (All_Array[touchId].IsOptionTarget)
            {
                if (currentTouchId < 0) currentTouchId = touchId;
                if (touchId != currentTouchId)
                {
                    currentTouchId = touchId;
                    confirmSkillStep = 0;
                }
                else
                {
                    if(confirmSkillStep == 1)
                    {
                        ActionConfirm = true;
                    }
                    else if(confirmSkillStep == 0)
                    {
                        confirmSkillStep = 1;
                        _currentTargetUnit = All_Array[currentTouchId];
                        _currentTUType = _currentTargetUnit._Tag;

                        SelectController.ConfirmTarget(touchId);
                    }
                }
            }
            else
            {
                RolePlayJumpAnim(All_Array[touchId]);
                Debug.Log("无效目标");
            }
        }

    }

    public void ShowNextActionUnit()
    {
        StartCoroutine(IEShowNextActionUnit());
    }
    public IEnumerator IEShowNextActionUnit()
    {
        yield return new WaitForSeconds(WaitForNextUnitTime * 2);
        NextActionUnit();
    }
    public void NextActionUnit()
    {
        if (Remaining_ORL.Count == 0) return;
        if (Remaining_SRL.Count == 0) return;
        if (_currentBattleUnit != null) _currentBattleUnit.actionSign.gameObject.SetActive(false);
        _currentBattleUnit = All_Array[0];
        All_Array.Remove(_currentBattleUnit);
        All_Array.Add(_currentBattleUnit);
        if (_currentBattleUnit.IsDead)
        {
            NextActionUnit();
            return;
        }
        if (_currentBattleUnit.ReadThisStateEnable(StateTag.Dizzy))
        {
            _currentBattleUnit.CheckStates();
            if (SDGameManager.Instance.isFastModeEnabled)
            {
                NextActionUnit();
            }
            return;
        }
        _currentBattleUnit.actionSign.gameObject.SetActive(true);
        _currentActionPanel = _currentBattleUnit.APC;
        checkCurrentUnitTag();
        //
        _currentBattleUnit.CheckStates();//计算角色状态对其影响
        if (_currentBattleUnit.IsDead)
        {
            NextActionUnit();
            return;//重新进行死亡判断
        }
        if(_currentBUType != SDConstants.CharacterType.Enemy)
        {
            if (SDDataManager.Instance.SettingData.isAutoBattle)
            {
                _currentActionPanel.chooseRandomSkillFromGroup();
                chooseAutoBattleTarget();
                handleAction();
            }
            else
            {
                IsWaitingPlayerAction = true;
                //显示全部可用交互
                _currentActionPanel.showActionPanel();
                UsingPropBtn.interactable = true;
                CancelBtn.interactable = true;
            }
        }
        else
        {
            _currentActionPanel.showActionPanel();
            _currentActionPanel.chooseASkillFromSkillGroup();
            chooseRandomHeroToAttack();
            handleAction();
        }
        currentTouchId = -1;
        confirmSkillStep = 0;
    }
    /// <summary>
    /// 敌人释放技能时判定目标
    /// </summary>
    public void chooseRandomHeroToAttack()
    {
        //Debug.Log("敌方：" + _currentBattleUnit.name + " 自动选择目标");
        _currentSkill = _currentActionPanel.CurrentSkill;
        if(_currentSkill.ThisSkillAim == SkillAim.Other)
        {
            _currentTargetUnit = Remaining_SRL[TauntFunction()];
            _currentTUType = SDConstants.CharacterType.Hero;
        }
        else
        {
            if (_currentSkill.ThisSkillAim == SkillAim.Self || _currentSkill.ThisSkillAim == SkillAim.All)
            {
                _currentTargetUnit = _currentBattleUnit;
            }
            else
            {
                int _rand = UnityEngine.Random.Range(0, Remaining_ORL.Count);
                _currentTargetUnit = Remaining_ORL[_rand];
                _currentTUType = SDConstants.CharacterType.Enemy;
            }
        }

    }
    /// <summary>
    /// 受英雄嘲讽值影响
    /// </summary>
    /// <param name="heros">当前的英雄们</param>
    /// <returns></returns>
    public int TauntFunction()
    {
        float[] allTs = new float[Remaining_SRL.Count];
        float maxT = 0;
        for(int i = 0; i < allTs.Length; i++)
        {
            allTs[i] = Remaining_SRL[i].HeroProperty._role.taunt;
            allTs[i] = Mathf.Max(0.5f, allTs[i]);
            float f = allTs[i];
            if (Remaining_SRL[i].HeroProperty._hero._heroJob 
                == _currentBattleUnit.EnemyProperty.AttackPreferenceInJob)
            {
                allTs[i] += f / 2;
            }
            if(Remaining_SRL[i].HeroProperty._hero._heroRace 
                == _currentBattleUnit.EnemyProperty.AttackPreferenceInRace)
            {
                allTs[i] += f / 2;
            }
            maxT = Mathf.Max(maxT, allTs[i]);
        }
        if(_currentBattleUnit.EnemyProperty.CunningAttack)
            for(int i = 0; i < allTs.Length; i++)
            {
                allTs[i] = Mathf.Max(maxT - allTs[i] , 0.5f);
            }
        int _rand = RandomIntger.StandardReturn(allTs);
        return _rand;
    }
    /// <summary>
    /// 英雄标准技能自动化后的目标选择
    /// </summary>
    public void chooseAutoBattleTarget()
    {
        _currentSkill = _currentActionPanel.CurrentSkill;
        bool ToHero=true;
        if(_currentSkill.ThisSkillAim == SkillAim.Self || _currentSkill.ThisSkillAim == SkillAim.All)
        {
            _currentTargetUnit = _currentBattleUnit;
            _currentTUType = _currentBattleUnit._Tag;
            return;
        }
        else if(_currentSkill.ThisSkillAim == SkillAim.Friend && _currentBattleUnit.IsEnemy)
        { ToHero = false; }
        else if (_currentSkill.ThisSkillAim == SkillAim.Other && !_currentBattleUnit.IsEnemy)
        { ToHero = false; }
        if (ToHero)
        {
            if (Remaining_SRL.Count <= 0) Debug.Log("GameOver");
            else
            {
                int _rand = UnityEngine.Random.Range(0, Remaining_SRL.Count);
                if (_currentSkill.name.Contains("Heal")) _rand = AutoHealTarget(false);
                _currentTargetUnit = Remaining_SRL[_rand];
                _currentTUType = SDConstants.CharacterType.Hero;

            }
        }
        else
        {
            //if (Remaining_ORL.Count <= 0) BattleSuccess();
            //else
            if(Remaining_ORL.Count>0)
            {
                int _rand = UnityEngine.Random.Range(0, Remaining_ORL.Count);
                if (_currentSkill.name.Contains("Heal")) _rand = AutoHealTarget(true);
                _currentTargetUnit = Remaining_ORL[_rand];
                _currentTUType = SDConstants.CharacterType.Enemy;
            }
        }
    }
    /// <summary>
    /// 治疗技能选择当前生命值比率最低的角色
    /// </summary>
    /// <param name="isEnemy"></param>
    /// <returns></returns>
    public int AutoHealTarget(bool isEnemy)
    {
        if (!isEnemy)
        {
            int target = 0;
            float hpR = Remaining_SRL[0].HpController.CurrentHp * 1f
                / Remaining_SRL[0].HpController.MaxHp;
            for(int i = 0; i < Remaining_SRL.Count; i++)
            {
                if(Remaining_SRL[i].HpController.CurrentHp * 1f
                    / Remaining_SRL[0].HpController.MaxHp < hpR)
                {
                    target = i;
                }
            }
            return target;
        }
        else
        {
            int target = 0;
            float hpR = Remaining_ORL[0].HpController.CurrentHp * 1f
                / Remaining_ORL[0].HpController.MaxHp;
            for (int i = 0; i < Remaining_ORL.Count; i++)
            {
                if (Remaining_ORL[i].HpController.CurrentHp * 1f
                    / Remaining_ORL[i].HpController.MaxHp < hpR)
                {
                    target = i;
                }
            }
            return target;
        }
    }
    public void checkCurrentUnitTag()
    {
        if(_currentBattleUnit._Tag == SDConstants.CharacterType.Hero)
        {
            _currentBUType = SDConstants.CharacterType.Hero;
        }
        else
        {
            _currentBUType = SDConstants.CharacterType.Enemy;
        }
    }


    public void handleAction()
    {
        bool goOn = false;
        if(_currentTUType == _currentBUType && _currentSkill.ThisSkillAim != SkillAim.Other)
        {
            goOn = true;
        }
        else if(_currentTUType != _currentBUType && _currentSkill.ThisSkillAim != SkillAim.Friend
            && _currentSkill.ThisSkillAim != SkillAim.Self)
        {
            goOn = true;
        }
        if (goOn)
        {
            hideOptionTarget();
            _currentSkill.StartSkill(_currentBattleUnit, _currentTargetUnit);
            BtnToCloseSDP();
        }
    }
    #region 使用道具
    public void BtnToOpenPropPanel()
    {
        if (SDGameManager.Instance.isUsingProp) return;

        UIEffectManager.Instance.showAnimFadeIn(PropPanel);
        PropBag.InitBag(BagController.useType.use);
    }
    public void BtnToClosePropPanel()
    {
        UIEffectManager.Instance.hideAnimFadeOut(PropPanel);
    }
    public void WhenClickOnPropSlot()
    {

    }
    public void startUseProp(string functionName)
    {
        if(functionName == "addHP")
        {
            PF.addHp(_currentBattleUnit, PropTargetUnit, param0
                , SDDataManager.Instance.AOE_TYPE(_PropRange));
        }
        else if(functionName == "addMP")
        {
            PF.addMp(_currentBattleUnit, PropTargetUnit, param0
                , SDDataManager.Instance.AOE_TYPE(_PropRange));
        }
        else if(functionName == "addTP")
        {
            PF.addTp(_currentBattleUnit, PropTargetUnit, param0
                , SDDataManager.Instance.AOE_TYPE(_PropRange));
        }
        else if(functionName == "hpRegenPerTurn")
        {
            PF.hpRegendPerTurn(_currentBattleUnit, PropTargetUnit, param0
                , param1, SDDataManager.Instance.AOE_TYPE(_PropRange));
        }
        else if (functionName == "mpRegenPerTurn")
        {
            PF.mpRegendPerTurn(_currentBattleUnit, PropTargetUnit, param0
                , param1, SDDataManager.Instance.AOE_TYPE(_PropRange));
        }
        else if (functionName == "tpRegenPerTurn")
        {
            PF.tpRegendPerTurn(_currentBattleUnit, PropTargetUnit, param0
                , param1, SDDataManager.Instance.AOE_TYPE(_PropRange));
        }
        else if(functionName == "revive")
        {
            bool flag = false;
            if (PropTargetUnit.IsDead)
            {
                flag = true;
            }
            else
            {
                List<BattleRoleData> lit = PF.DealWithAOEAction(_currentBattleUnit
                    , PropTargetUnit, SDDataManager.Instance.AOE_TYPE(_PropRange));
                foreach(BattleRoleData unit in lit)
                {
                    if (unit.IsDead)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                PF.revive(_currentBattleUnit, PropTargetUnit, param0, param1
                    , SDDataManager.Instance.AOE_TYPE(_PropRange));
            }
        }
        else if(functionName == "catch")
        {
            if (!PropTargetUnit.IsDead 
                && PropTargetUnit.HpController.CurrentHp * 1f/ PropTargetUnit.HpController.MaxHp 
                <= AllRandomSetClass.SimplePercentToDecimal(param1))
            {
                PF.catchSlave(_currentBattleUnit, PropTargetUnit,param0);
            }
        }
    }
    #endregion
    #region 全局型状态
    public BattleGroupStateController GroupState
    { get
        {
            if (!GetComponent<BattleGroupStateController>())
            {
                gameObject.AddComponent<BattleGroupStateController>();
            }
            return GetComponent<BattleGroupStateController>();
        } }
    public void checkOverallStates()
    {
        //群体状态效果
        GroupState.checkAllGroupState();
    }
    #endregion
    #region 选中角色时角色动画
    public void RolePlayJumpAnim(BattleRoleData _unit)
    {
        CharacterModelController _cha = _unit.GetComponentInChildren<CharacterModelController>();
        if (IsWaitingPlayerAction && _cha != null)
        {
            CharacterModel cha = _cha.CurrentCharacterModel;
            if (cha && !cha._isDead)
            {
                StartCoroutine(IEJumpAnim(cha));
            }
        }
    }
    IEnumerator IEJumpAnim(CharacterModel Unit)
    {
        Unit.ChangeModelAnim(Unit.anim_jump);
        yield return new WaitForSeconds(0.6f);
        Unit.ChangeModelAnim(Unit.anim_idle, true);
    }
    #endregion
    #region actionBar
    public void startActionBar(int currentActionLevel)
    {
        StartCoroutine(IEStartActionBar(currentActionLevel));
    }
    public IEnumerator IEStartActionBar(int currentActionLevel)
    {
        yield return new WaitForSeconds(WAIT_FOR_NEXT_UNIT_TIME);
        if (currentActionLevel == SDGameManager.Instance.currentLevel)
        {
            //Debug.Log("startActionBar" + currentActionLevel);
            ABM.refreshAllUnitStatus();
            ABM.startAction();
        }
    }
    #endregion
    #region 各项结算
    public bool CheckBattleSuccess()
    {
        if (Remaining_ORL.Count == 0)
        {
            return true;
        }
        return false;
    }
    public void BattleSuccess()
    {
        //在成就中记录当前通过关卡数
        SDDataManager.Instance.addAchievementDataByType("passedNum_level");
        //
        if(SDGameManager.Instance.currentLevel % SDConstants.LevelNumPerSection
            == SDConstants.LevelNumPerSection - 1
            && SDGameManager.Instance.currentLevel != 0)//判断为section结束
        {
            SDGameManager.Instance.isGamePaused = true;
            GC.showRemarkLayer();
            return;
        }
        NextBattle();
    }
    public void NextBattle()
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            Debug.Log("单个level战斗胜利");

            //
            SDDataManager.Instance.SetNewBestLevel(SDGameManager.Instance.currentLevel);
            SDGameManager.Instance.currentLevel++;
            foreach (BattleRoleData unit in Remaining_SRL)
            {
                foreach (SkillFunction skill in unit.APC.SkillGroup)
                {
                    skill.setUnused();
                }
            }
            GC.playNextLevelAnim();
            GC.setCurrentLevel();
        }
        else if (SDGameManager.Instance.gameType == SDConstants.GameType.Dungeon)
        {

        }
        else if (SDGameManager.Instance.gameType == SDConstants.GameType.DimensionBoss)
        {
            GameSuccess();
            return;
        }
    }

    public bool CheckBattleLose()
    {
        bool flag = false;
        if (Remaining_SRL.Count == 0)
        {
            SDGameManager.Instance.isGamePaused = true;
            BattleFail();
            flag = true;
        }
        else
        {
            flag = true;
            foreach (BattleRoleData unit in Remaining_SRL)
            {
                if (!unit.IsDead)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                SDGameManager.Instance.isGamePaused = true;
                BattleFail();
            }
        }
        return flag;
    }
    public void GameSuccess()
    {
        if (!SDGameManager.Instance.isGameFinished) GC.gameSuccess();
    }
    public void BattleFail()
    {
        GC.levelText.text = "战斗失败";
        if (!SDGameManager.Instance.isGameFinished) GC.gameFail();
    }
    #endregion
    #region 回合内监听设置
    public delegate void RoundProcessListener();
    public event RoundProcessListener round_start_event;
    public event RoundProcessListener round_end_event;
    public void whenOneRoundStart()
    {
        round_start_event?.Invoke();
        checkOverallStates();
    }
    public void whenOneRoundEnd()
    {
        round_end_event?.Invoke();
    }
    public event BattleDamageListener heroToEnemyDmg_event;
    public event BattleDamageListener enemyToHeroDmg_event;
    public int heroToEnemyDmg(int val)
    {
        return heroToEnemyDmg_event != null? heroToEnemyDmg_event.Invoke(val) : val;
    }
    public void resetHTED()
    {
        heroToEnemyDmg_event = null;
    }
    public int enemyToHeroDmg(int val)
    {
        return enemyToHeroDmg_event != null ? enemyToHeroDmg_event.Invoke(val) : val;
    }
    public void resetETHD()
    {
        enemyToHeroDmg_event = null;
    }
    #endregion
}

public delegate int BattleDamageListener(int originalDmg);
