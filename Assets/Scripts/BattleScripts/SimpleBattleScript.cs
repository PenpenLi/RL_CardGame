using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleBattleScript : MonoBehaviour
{
    public bool BattleEnable;
    public float BehaveBar_MaxValue;
    [Header("玩家操纵菜单")]
    public Transform SkillBtnContent;
    public GameObject SkillBtn;
    [Header("战斗界面配置")]
    public RectTransform SelfSidePlace;
    public RectTransform OthesidePlace;
    List<GameObject> SkillBtnList;
    GameObject RoleSlider;
    public Transform ProgressBarContent;
    [HideInInspector]
    public List<RoleDisplay> AllSelves = new List<RoleDisplay>();
    [HideInInspector]
    public List<RoleDisplay> AllOthers = new List<RoleDisplay>();
    int BattleSituation;
    [HideInInspector]
    public bool StartBattleMenu;
    [Header("交互动画设置")]
    public float ActiveRoleSize;
    //
    [HideInInspector]
    public bool BattlePause;
    private float ST;
    [Header("战斗动画时间")]
    public float BattlePauseIntervial = 0.75f;
    [HideInInspector]
    public float TimeSpeed = 1;//控制进度条是否前进

    // Start is called before the first frame update
    void Start()
    {
        RoleSlider = Resources.Load<GameObject>("Prefabs/ProgressBarSlider");
        //例子
        //SimpleSetRoles();
    }

    //逐帧运动
    void FixedUpdate()
    {
        if (StartBattleMenu)
        {
            if (BattlePause)
            {
                TimeSpeed = 0;
                ST += Time.deltaTime;
                if (ST >= BattlePauseIntervial)
                {
                    ST = 0;
                    BattlePause = false;
                    TimeSpeed = 1;
                    BattleSituation = 0;
                    //角色恢复原大小
                    if (RDInRound!=null)
                    {
                        RDInRound.PerRolePlace.transform.GetChild(0).localScale = Vector3.one;
                    }
                }
            }
            else
            {
                WhenBattleing();
            }
            //显示所有单位信息
            ShowRoleSliders(AllSelves);
            ShowRoleSliders(AllOthers);
        }
    }

    #region 构建战场
    //构建战斗双方
    public void SimpleSetRoles()
    {
        BattleRoleManager brm = GetComponent<BattleRoleManager>();
        SetSideRoles(brm.PlayersList, true);
        SetSideRoles(brm.MonstersList, false);
        SetRolesSkills();
    }
    void SetRolesSkills()
    {
        #region 玩家技能
        AllSelves[0].role.skills.Add(skill.CreateDamageSkill("斩击", SkillTarget.ToFoe, AllSelves[0].role.AP));
        AllSelves[0].role.skills.Add(skill.CreateDamageSkill("盾击", SkillTarget.ToFoe, AllSelves[0].role.AP / 2));
        //
        AllSelves[1].role.skills.Add(skill.CreateDamageSkill("突刺", SkillTarget.ToFoe, AllSelves[1].role.AP));
        State p1s0 = new State(StateType.DamageOverTime, 5, 5);
        AllSelves[1].role.skills.Add(skill.CreateAddStateSkill("淬毒武器", SkillTarget.ToFoe, p1s0));
        //
        AllSelves[2].role.skills.Add(skill.CreateDamageSkill("射击", SkillTarget.ToFoe, AllSelves[2].role.AP));
        AllSelves[2].role.skills.Add(skill.CreateHealSkill("自我治疗", SkillTarget.ToSelf, 50));
        #endregion
        #region 敌人技能
        AllOthers[0].role.skills.Add(skill.CreateDamageSkill("攻击", SkillTarget.ToFoe, AllOthers[0].role.AP));
        AllOthers[0].role.skills.Add(skill.CreateDamageSkill("下盘攻击", SkillTarget.ToFoe, AllOthers[0].role.AP + 10));
        //
        AllOthers[1].role.skills.Add(skill.CreateDamageSkill("斩击", SkillTarget.ToFoe, AllOthers[1].role.AP));
        State m1s0 = new State(StateType.DamageOverTime, 5, 5);
        AllOthers[1].role.skills.Add(skill.CreateAddStateSkill("撕裂猛击", SkillTarget.ToFoe, p1s0));
        //
        AllOthers[2].role.skills.Add(skill.CreateDamageSkill("撕咬", SkillTarget.ToFoe, AllOthers[2].role.AP));
        AllOthers[2].role.skills.Add(skill.CreateHealSkill("伤口舔舐", SkillTarget.ToSelf, 50));
        #endregion
    }
    public List<RoleDisplay> SideRolesList(bool isleft)
    {
        if (isleft) { return AllSelves; } else { return AllOthers; }
    }
    public Transform SidePlace(bool isleft)
    {
        if (isleft) { return SelfSidePlace; } else { return OthesidePlace; }
    }
    public void SetSideRoles(List<Role> SideRoles,bool IsLeft)
    {
        SideRolesList(IsLeft).Clear();
        for (int i = 0; i < SideRoles.Count; i++)
        {
            RoleDisplay a = new RoleDisplay(SideRoles[i], SidePlace(IsLeft).GetChild(i).gameObject);
            SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().TargetNum = i;
            SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().IsLeft = IsLeft;
            SideRolesList(IsLeft).Add(a);
            SideRolesList(IsLeft)[i].role.skills = new List<skill>();
            SideRolesList(IsLeft)[i].role.skills.Add(skill.CreateDamageSkill("普通攻击", 
                SkillTarget.ToFoe, SideRolesList(IsLeft)[i].role.AP));
            SideRolesList(IsLeft)[i].role.states = new List<State>();
        }
        //
        for (int i = 0; i < SidePlace(IsLeft).childCount; i++)
        {
            if (i < SideRolesList(IsLeft).Count)
            {
                SidePlace(IsLeft).GetChild(i).gameObject.SetActive(true);
                //名称
                SidePlace(IsLeft).GetChild(i).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = 
                    SideRolesList(IsLeft)[i].role.name;
                //生命值
                SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().ThisCardHp.GetComponent<Slider>().maxValue =
                    SideRolesList(IsLeft)[i].role.MaxHp;
                SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().ThisCardHp.GetComponent<Slider>().value = 
                    SideRolesList(IsLeft)[i].role.hp;
                //魔力值
                SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().ThisCardSp.GetComponent<Slider>().maxValue =
                    SideRolesList(IsLeft)[i].role.MaxSp;
                SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().ThisCardSp.GetComponent<Slider>().value =
                    SideRolesList(IsLeft)[i].role.sp;
                //怒气值
                SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().ThisCardTp.GetComponent<Slider>().maxValue =
                    SideRolesList(IsLeft)[i].role.MaxTp;
                SidePlace(IsLeft).GetChild(i).GetComponent<BattlePerRoleCard>().ThisCardTp.GetComponent<Slider>().value =
                    SideRolesList(IsLeft)[i].role.tp;
                //设置顶部进度条
                GameObject pb = Instantiate(RoleSlider, ProgressBarContent) as GameObject;
                pb.GetComponentInChildren<Text>().text = SideRolesList(IsLeft)[i].role.name;
                pb.GetComponentInChildren<Image>().color = IsLeft?Color.green:Color.red;
                SideRolesList(IsLeft)[i].RoleProgressSliderObj = pb;
            }
            else
            {
                SidePlace(IsLeft).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    #endregion
    #region 构建交互菜单
    //构建技能按钮
    void ShowSelfSideBtns(Role role)
    {
        Debug.Log("进入 " + role.name + " 回合");
        int L = role.skills.Count;
        if (L < SkillBtnContent.childCount)
        {
            for(int i = L; i < SkillBtnContent.childCount; i++) { SkillBtnContent.GetChild(i).gameObject.SetActive(false); }
        }
        for (int i = 0; i < L; i++)
        {
            if (i >= SkillBtnContent.childCount)
            {
                Instantiate(SkillBtn, SkillBtnContent);
            }
            SkillBtnContent.GetChild(i).gameObject.SetActive(true);
            SkillBtnContent.GetChild(i).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = role.skills[i].skillName;
            SkillBtnContent.GetChild(i).GetChild(0).GetComponent<Image>().color = SkillBtnColor(role.skills[i]);
            SkillBtnContent.GetChild(i).GetComponent<SkillBtnScript>().OpSkillNum = i;
        }
        //技能按钮可用
        ControlAllSelfSkillBtns(true);
        //当前活跃角色动画
        if (RDInRound != null)
        {
            RDInRound.PerRolePlace.transform.GetChild(0).localScale = Vector3.one * ActiveRoleSize;
        }
    }
    Color SkillBtnColor(skill s)
    {
        switch (s.Type)
        {
            case SkillType.Damage:
                return new Color(255, 225, 225, 255);
            case SkillType.Heal:
                return new Color(225, 255, 225, 255);
            case SkillType.AddState:
                return new Color(225, 225, 255, 255);
        }
        return Color.white;
    }
    void ControlAllSelfSkillBtns(bool a)
    {
        for (int i = 0; i < SkillBtnContent.childCount; i++)
        {
            SkillBtnContent.GetChild(i).GetComponentInChildren<Button>().interactable = a;
        }
    }
    #endregion

    /// <summary>
    /// 战斗回合
    /// </summary>
    public void JudgeWhichRoleRound(bool isleft)
    {
        for(int i = 0; i < SideRolesList(isleft).Count; i++)
        {
            if (SideRolesList(isleft)[i].role.hp > 0)
            {
                SideRolesList(isleft)[i].RoleProgressFs += (float)SideRolesList(isleft)[i].role.Speed / 
                    100 * Time.deltaTime * TimeSpeed;
                SideRolesList(isleft)[i].RoleProgressSliderObj.GetComponentInChildren<Slider>().value =
                    SideRolesList(isleft)[i].RoleProgressFs / BehaveBar_MaxValue;

                if (SideRolesList(isleft)[i].RoleProgressSliderObj.GetComponentInChildren<Slider>().value >= 1)
                {
                    SideRolesList(isleft)[i].RoleProgressFs = 
                        SideRolesList(isleft)[i].RoleProgressSliderObj.GetComponentInChildren<Slider>().value = 0;
                    TimeSpeed = 0;
                    RDInRound = SideRolesList(isleft)[i];
                    BattleSituation = isleft ? -1 : 1;
                    Debug.Log(BattleSituation.ToString());
                }
            }
        }
    }
    RoleDisplay RDInRound;

    void WhenBattleing()
    {
        if (BattleEnable)
        {
            BattleSituation = 0;
            JudgeWhichRoleRound(true);//玩家进度条
            JudgeWhichRoleRound(false);//敌人进度条
            //
            if (BattleSituation == -1)//玩家回合
            {
                ShowSelfSideBtns(RDInRound.role);
            }
            else if (BattleSituation == 1)//敌方回合
            {
                MonsterBehave(RDInRound);
            }
            else
            {

            }
        }
    }
    /// <summary>
    /// 显示单位实时信息
    /// </summary>
    void ShowRoleSliders(List<RoleDisplay> roles)
    {
        for(int i = 0; i < roles.Count; i++)
        {
            roles[i].PerRolePlace.GetComponent<BattlePerRoleCard>().ThisCardHp.value = roles[i].role.hp;
            roles[i].PerRolePlace.GetComponent<BattlePerRoleCard>().ThisCardSp.value = roles[i].role.sp;
            roles[i].PerRolePlace.GetComponent<BattlePerRoleCard>().ThisCardTp.value = roles[i].role.tp;
        }
    }


    //敌人行为
    int MonsterTarget_one()
    {
        float[] allPs_player = new float[AllSelves.Count];
        for (int i = 0; i < allPs_player.Length; i++) { allPs_player[i] = AllSelves[i].role.TauntRate; }
        return RandomIntger.StandardReturn(allPs_player);
    }
    void MonsterBehave(RoleDisplay monster)
    {
        Debug.Log(monster.role.name + " 行动");
        skill s = monster.role.SelectSkill(Random.Range(0, monster.role.skills.Count));
        List<RoleDisplay> a = new List<RoleDisplay>();
        a.Add(AllSelves[MonsterTarget_one()]);
        if (RoleAct(monster, a, s))
        {
            RoundEnd();
        }
        //
        if (RDInRound != null)
        {
            RDInRound.PerRolePlace.transform.GetChild(0).localScale = Vector3.one * ActiveRoleSize;
        }
        //
        BattlePause = true;
    }

    public List<RoleDisplay> GetSkillTarget(skill skill,int targetnum)
    {
        List<RoleDisplay> a = new List<RoleDisplay>();
        if (skill.Target == SkillTarget.ToFoe)
        {
            a.Add(AllOthers[targetnum]);
        }
        else if (skill.Target == SkillTarget.ToAllFoes)
        {
            for (int i = 0; i < AllOthers.Count; i++) { a.Add(AllOthers[i]); }
        }
        else if (skill.Target == SkillTarget.ToFriend)
        {
            a.Add(AllSelves[targetnum]);
        }
        else if(skill.Target == SkillTarget.ToAllFriends)
        {
            for (int i = 0; i < AllSelves.Count; i++) { a.Add(AllSelves[i]); }
        }
        else if (skill.Target == SkillTarget.ToSelf)
        {
            a.Add(RDInRound);
        }
        else if (skill.Target == SkillTarget.NoAim)
        {
            Debug.Log("技能为无目标技能(选择全场角色)");
            for(int i = 0; i < AllSelves.Count; i++) { a.Add(AllSelves[i]); }
            for(int i = 0; i < AllOthers.Count; i++) { a.Add(AllOthers[i]); }
        }
        return a;
    }
    static bool RoleAct(RoleDisplay self, List<RoleDisplay> target,skill s)
    {
        //状态生效
        self.role.StateAffect();
        Transform sl = self.PerRolePlace.GetComponent<BattlePerRoleCard>().ThisCardAllStates;
        for (int i = 0; i < sl.childCount; i++)
        {
            sl.GetChild(i).gameObject.SetActive(false);
            if (i < self.role.states.Count)
            {
                sl.GetChild(i).gameObject.SetActive(true);
                //显示具体状态造型
            }
        }

        for(int i = 0; i < target.Count; i++)
        {
            target[i].role.BeHit_AP(s);
            if (target[i].role.hp <= 0 || self.role.hp <= 0)
            {
                return false;
            }
        }      
        return true;
    }

    //玩家行为
    int SkillNum;
    /// <summary>
    /// 选择技能
    /// </summary>
    /// <param name="n">技能编号</param>
    public void PlayerAct(int n)
    {
        SkillNum = n;
        skill PlayerUsedSkill = RDInRound.role.skills[n];
        if(PlayerUsedSkill.Target == SkillTarget.NoAim || PlayerUsedSkill.Target == SkillTarget.ToSelf)
        {
            ControlAllSelfSkillBtns(false);
            List<RoleDisplay> a = new List<RoleDisplay>();
            a.Add(RDInRound);
            if (!RoleAct(RDInRound, a, PlayerUsedSkill)) { RoundEnd(); }
            BattlePause = true;
        }
        else
        {
            PlayerSelectSkill = true;
        }
        Debug.Log("成功选择技能 " + n);
        
    }
    bool PlayerSelectSkill;
    public void PlayerSelectSkillTarget(int targetnum,bool IsLeft)
    {
        skill PlayerUsedSkill = RDInRound.role.skills[SkillNum];
        //判断是否为正确对象
        if (IsLeft)
        {
            if(PlayerUsedSkill.Target == SkillTarget.ToFoe || PlayerUsedSkill.Target == SkillTarget.ToAllFoes)
            {
                PlayerSelectSkill = false;
            }
        }
        else
        {
            if (PlayerUsedSkill.Target == SkillTarget.ToFriend || PlayerUsedSkill.Target == SkillTarget.ToAllFriends)
            {
                PlayerSelectSkill = false;
            }
        }
        //指向性技能成功发动
        if (PlayerSelectSkill)
        {
            PlayerSelectSkill = false;
            Debug.Log(PlayerUsedSkill.skillName);
            List<RoleDisplay> targetlist = GetSkillTarget(PlayerUsedSkill, targetnum);

            if (!RoleAct(RDInRound, targetlist, PlayerUsedSkill)) { RoundEnd(); }

            BattlePause = true;
            ControlAllSelfSkillBtns(false);
        }
        else
        {
            Debug.Log("显示 " + SidePlace(IsLeft).GetChild(targetnum).
                GetComponentInChildren<TextMeshProUGUI>().text + " 详细信息");
        }
        
    }
    //回合结束结算
    void RoundEnd()
    {
        if (!CheckSideRolesAllHps(AllSelves))
        {
            Debug.Log("玩家失败");
            BattleEnd();
        }
        else if (!CheckSideRolesAllHps(AllOthers))
        {
            Debug.Log("玩家获胜");
            BattleEnd();
        }
    }
    public bool CheckSideRolesAllHps(List<RoleDisplay> rdl)
    {
        bool a = false;
        for(int i = 0; i < rdl.Count; i++)
        {
            if (!rdl[i].HaveDead)
            {
                if (rdl[i].role.hp > 0)
                {
                    a = true;
                }
                else
                {
                    Debug.Log( rdl[i].role.name + " 阵亡");
                    rdl[i].RoleProgressSliderObj.GetComponentInChildren<Slider>().value = 0;
                    rdl[i].RoleProgressSliderObj.GetComponentInChildren<Slider>().handleRect.
                        GetComponentInChildren<Image>().color =
                        rdl[i].RoleProgressSliderObj.GetComponentInChildren<Slider>().handleRect.
                        GetComponentInChildren<Image>().color / 2;

                    rdl[i].HaveDead = true;
                }
            }
        }
        return a;
    }
    void BattleEnd()
    {
        BattleEnable = false;
    }
}



public class RoleDisplay
{
    public Role role;
    public GameObject PerRolePlace;
    public GameObject RoleProgressSliderObj;
    public float RoleProgressFs;
    public bool HaveDead;
    public RoleDisplay(Role r,GameObject p)
    {
        role = r;PerRolePlace = p;
    }
}