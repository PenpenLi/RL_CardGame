using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    //public Transform ActionBarPlace;
    private int[] standardSpeedArray;
    private int[] speedLevels;
    //private float[] startPosArray;
    //
    public Transform startHeroPos;
    public Transform startEnemyPos;
    public Transform endPos;
    public Transform actionUnitPrefab;
    public List<ActionRoleData> allActionUnits;
    public Transform actionUnitParent;
    //
    public ActionRoleData CurrentActionUnit;
    public bool IsWaitingAction = false;
    [HideInInspector]
    public BattleManager BM;
    //
    //public float ABMaxTime = 0.75f;
    //[HideInInspector]
    //public float actionBarMoveSpeed = 0.5f;
    public float ActionBarSpeedFix = 1f;
    public int CurrentRoundNum = 0;

    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        BM = GetComponentInParent<BattleManager>();
        standardSpeedArray = new int[SDConstants.MaxSelfNum + SDConstants.MaxOtherNum];
        speedLevels = new int[SDConstants.MaxSelfNum + SDConstants.MaxOtherNum];
        for (int i = 0; i < speedLevels.Length; i++)
        {
            speedLevels[i] = 100 - i * 10;
        }
        CheckDaynight();
        //BM.ABM = this;
        BM.whenOneRoundStart();
    }

    //关卡开始时初始化所有行动单元，赋予速度
    public void initAction(List<BattleRoleData> allUnits)
    {
        stopAction();
        foreach(Transform t in actionUnitParent) Destroy(t.gameObject);
        //
        standardSpeedArray = new int[allUnits.Count];
        allActionUnits = new List<ActionRoleData>();
        for(int i = 0; i < allUnits.Count; i++)
        {
            BattleRoleData BUnit = allUnits[i];
            Transform actionUnit = Instantiate(actionUnitPrefab) as Transform;
            actionUnit.SetParent(actionUnitParent);
            actionUnit.GetComponent<ActionRoleData>().nameText.text = BUnit.name;
            ActionRoleData unit = actionUnit.GetComponent<ActionRoleData>();
            //
            int speed = BUnit.ThisBasicRoleProperty()._role.ReadCurrentRoleRA(AttributeData.Speed);
            standardSpeedArray[i] = speed;
            unit.speed = speed;
            //
            //actionUnit.GetComponent<Canvas>().sortingOrder = i;
            if(BUnit._Tag == SDConstants.CharacterType.Enemy)
            {
                actionUnit.position = startEnemyPos.position;
                unit.isEnemy = true;
                if (BUnit.IsBoss)
                {
                    unit.headImage.initCharacterModelById
                        (BUnit.UnitId, SDConstants.CharacterAnimType.Enemy, 0.15f, true);
                }
                else
                {
                    unit.headImage.initCharacterModelById
                        (BUnit.UnitId, SDConstants.CharacterAnimType.Enemy, 0.3f, true);
                }
            }
            else
            {
                actionUnit.position = startHeroPos.position;
                unit.isEnemy = false;
                int hashcode = BUnit.unitHashcode;
                string heroId = SDDataManager.Instance.getHeroIdByHashcode(hashcode);
                SDConstants.CharacterAnimType type
                    = (SDConstants.CharacterAnimType)
                    (SDDataManager.Instance.getHeroCareerById(heroId));
                unit.headImage.initCharacterModel(hashcode, type, 3f);
            }
            actionUnit.localScale = Vector3.one;
            if(BUnit.ReadThisStateEnable(StateTag.Dizzy))
            {
                unit.stateBgImage.sprite = unit.stateSprites[1];
            }
            else
            {
                unit.stateBgImage.sprite = unit.stateSprites[0];
            }
            unit.battleUnit = BUnit;
            unit.isActed = false;
            allActionUnits.Add(unit);
            if (BUnit.IsDead) { actionUnit.gameObject.SetActive(false); }
        }

        BuildABMoveSpeed(allActionUnits);
        //BM.NextActionUnit();
        startAction();
    }
    public void RefreshUnitSpeeds(List<BattleRoleData> allUnits)
    {
        for(int i = 0; i < allUnits.Count; i++)
        {
            BattleRoleData BUnit = allUnits[i];
            //if(!BUnit.IsDead)
            int speed = BUnit.ThisBasicRoleProperty()._role.speed;
            standardSpeedArray[i] = speed;
        }
        BuildABMoveSpeed(allActionUnits);
    }
    public void BuildABMoveSpeed(List<ActionRoleData> AList)
    {
        AList.Sort((x,y) => { return -x.battleUnit.ThisBasicRoleProperty()._role.speed
            .CompareTo(y.battleUnit.ThisBasicRoleProperty()._role.speed); });//AList降序排序
        for(int i = 0; i < AList.Count; i++)
        {
            AList[i].speed = (speedLevels[i] + UnityEngine.Random.Range
                (-3, 3)) * 1f / 100;
        }
    }
    private int CompareByTotalTime(int x, int y)
    {
        return y.CompareTo(x);
    }
    /// <summary>
    /// 开始行动前如果存在，则将上一个到达终点的行动单元置于初始位置
    /// </summary>
    public void startAction()
    {
        resetLastActionUnit();
        IsWaitingAction = false;
    }
    public void stopAction() { IsWaitingAction = true; }
    /// <summary>
    /// 将所有行动单元设置为没有行动过
    /// </summary>
    public void resetAction()
    {
        for(int i = 0; i < allActionUnits.Count; i++)
        {
            allActionUnits[i].isActed = false;
        }
        RefreshUnitSpeeds(BM.All_Array);
    }
    public void refreshAllUnitStatus()
    {
        for(int i = 0; i < allActionUnits.Count; i++)
        {
            if (allActionUnits[i].battleUnit.IsDead)
                allActionUnits[i].gameObject.SetActive(false);
            else
            {
                if (allActionUnits[i].battleUnit.ReadThisStateEnable(StateTag.Dizzy))
                    allActionUnits[i].stateBgImage.sprite = allActionUnits[i].stateSprites[1];
                else
                    allActionUnits[i].stateBgImage.sprite = allActionUnits[i].stateSprites[0];
            }
        }
    }

    #region 重置行动过的行动单元
    public void resetLastActionUnit()
    {
        if (CurrentActionUnit != null && CurrentActionUnit.transform.position.x >= endPos.position.x)
            resetActionUnitPos(CurrentActionUnit);
    }
    public void resetActionUnitPos(ActionRoleData unit)
    {
        if (unit.isEnemy)
        {
            unit.transform.position = startEnemyPos.position;
            if (SDGameManager.Instance.isFastModeEnabled) unit.isActed = false;
        }
        else
        {
            unit.transform.position = startHeroPos.position;
            if (SDGameManager.Instance.isFastModeEnabled) unit.isActed = false;
        }
    }
    #endregion
    #region 回合记录
    public void WhenOneRoundFinish()
    {
        CurrentRoundNum++;
        CheckDaynight();
        //
        BM.whenOneRoundEnd();
    }
    public void CheckDaynight()
    {
        if (SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            int a = CurrentRoundNum / SDConstants.RoundToChangeDayNight;
            int dn = SDDataManager.Instance.ResidentMovementData.CurrentDayNightId;
            if ((dn + a) % 2 != dn)
            {
                SDDataManager.Instance.ResidentMovementData.CurrentDayNightId = (dn + a) % 2;
            }
        }
        else
        {
            SDDataManager.Instance.ResidentMovementData.CurrentDayNightId = 0;
        }
    }
    #endregion


    void FixedUpdate()
    {
        if (IsWaitingAction || allActionUnits == null || SDGameManager.Instance.isGamePaused) { return; }
        for (int i = 0; i < allActionUnits.Count; i++)
        {
            if (!allActionUnits[i].isActed)
            {
                if (allActionUnits[i].transform.position.x < endPos.position.x)
                {
                    allActionUnits[i].transform.position
                        += Vector3.right * ActionBarSpeedFix
                        * allActionUnits[i].speed * Time.deltaTime;
                }
                if (allActionUnits[i].transform.position.x >= endPos.position.x)
                {
                    if (allActionUnits[i].battleUnit.IsDead)
                    {
                        allActionUnits[i].isActed = true;
                        if(allActionUnits[i].battleUnit != null)
                            Debug.Log("该角色" + allActionUnits[i].battleUnit.name + " 状态为死亡");
                        resetActionUnitPos(allActionUnits[i]);
                    }
                    else
                    {
                        if (allActionUnits[i].battleUnit.ReadThisStateEnable(StateTag.Dizzy))
                        {
                            allActionUnits[i].isActed = true;
                            resetActionUnitPos(allActionUnits[i]);
                            BM.ShowNextActionUnit();
                        }
                        else
                        {
                            stopAction();
                            CurrentActionUnit = allActionUnits[i];
                            allActionUnits[i].isActed = true;
                            Debug.Log("当前行动单位 name: " + CurrentActionUnit.battleUnit.name);
                            BM.ShowNextActionUnit();

                            if (!SDGameManager.Instance.isFastModeEnabled)
                            {
                                if (i == allActionUnits.Count - 1)
                                {
                                    resetAction();
                                }
                            }
                            break;
                        }
                    }                   
                    //一回合结束
                    if(i == allActionUnits.Count - 1)
                    {
                        resetAction();
                        WhenOneRoundFinish();
                        //
                        BM.whenOneRoundStart();
                    }
                }
            }
        }
    }



    
    
}