using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HP_Controller : MonoBehaviour
{
    /// <summary>
    /// 角色类型(怪物，英雄，女神)
    /// </summary>
    public SDConstants.CharacterType _characterType;
    /// <summary>
    /// 战斗单元引用
    /// </summary>
    public BattleRoleData _unit;
    /// <summary>
    /// 是否显示血量
    /// </summary>
    public bool ShowHpStatus = false;
    #region UI构建
    public Transform BarChartStatus;
    public Text HpText;
    public Text ShieldText;
    public Text DmgText;
    public Text ExtraDmgText;
    public Text HealText;
    [HideInInspector]
    public SDHeroBattleStatus status;
    #endregion
    public int CurrentHp = 0;
    public int MaxHp = 0;
    public int CurrentShieldHp = 0;
    public int MaxShieldHp = 0;
    #region 动画构建
    private float TEXT_FADE_TIME = 0.15f;
    private float TEXT_MOVE_TIME = 0.3f;
    private float TEXT_MOVE_HEIGHT = 50f;
    public Transform foreImg;
    public Transform foreShieldImg;
    #endregion
    public bool isCriDmg = false;
    public bool isEvoHappen = false;
    public bool isFault = false;
    //extra
    public bool isSpShield = false;
    public bool receiveExtraDamage = false;
    public int currentExtraDamage = 0;
    // Start is called before the first frame update
    void Start()
    {
        _unit = GetComponentInParent<BattleRoleData>();
        status = GetComponentInChildren<SDHeroBattleStatus>();
        _characterType = _unit._Tag;
        initHpPanel();
    }

    public void initHpPanel()
    {
        if (_characterType == SDConstants.CharacterType.Hero)
        {
            ShowHpStatus = true;
        }
        else
        {
            ShowHpStatus = false;
            if (ShowHpStatus || SDGameManager.Instance.showEnemyState)
            {
                ShowHpStatus = true;
            }
            if (_unit.ThisBasicRoleProperty().ForceShield > 0)
            {
                foreShieldImg.transform.localScale = Vector3.one;
            }
            else
            {
                foreShieldImg.transform.localScale = Vector3.zero;
            }
        }
        reFreshState();
    }
    //判断是否需要显示血量
    public void checkIfShowHpStatus()
    {
        if (_characterType == SDConstants.CharacterType.Hero) ShowHpStatus = true;
        else
        {
            ShowHpStatus = false;
            if (SDGameManager.Instance.showEnemyState) ShowHpStatus = true;
        }
        if (ShowHpStatus) BarChartStatus.GetComponent<CanvasGroup>().alpha = 1;
        else { BarChartStatus.GetComponent<CanvasGroup>().alpha = 0; }
    }

    //刷新血量数值和颜色
    public void reFreshState() { StartCoroutine(IERefreshState()); }
    public IEnumerator IERefreshState()
    {
        yield return new WaitForSeconds(0.1f);
        CurrentHp = _unit.ThisBasicRoleProperty()._role.ReadCurrentRoleRA((int)AttributeData.Hp);
        MaxHp = CurrentHp;
        HpText.text = "" + CurrentHp;
        if (_unit.ThisBasicRoleProperty().ForceShield > 0)
        {
            CurrentShieldHp = _unit.ThisBasicRoleProperty().ForceShield;
            MaxShieldHp = CurrentShieldHp;
            ShieldText.text = "" + MaxShieldHp;
        }
        BarChartStatus.GetComponent<CanvasGroup>().alpha
            = ShowHpStatus ? 1 : 0;
        checkHpFull();
        if (CurrentShieldHp > 0)
        {
            checkShieldFull();
        }


        //角色当前状态详情可视化
        if (!_unit.IsEnemy && ShowHpStatus)
        {
            BarChartStatus.SetParent(_unit.BM.HeroStatusGroup[_unit.posIndex]);
            BarChartStatus.position = _unit.BM.HeroStatusGroup[_unit.posIndex].position;
            BarChartStatus.localScale = Vector3.one * 0.8f;
            BarChartStatus.GetComponentInChildren<SDHeroBattleStatus>().BuildThisStatusVision
                (_unit, this);
        }
    }

    //数值修改逻辑
    /// <summary>
    /// 伤害计算逻辑
    /// </summary>
    /// <param name="dmg">伤害量(默认正号)</param>
    /// <param name="kind">伤害类型(不同防御力)</param>
    /// <param name="canReduce">能否被减伤</param>
    public void getDamage(int dmg, SkillKind kind , bool canReduce = true)
    {
        if (_unit.IsDead) return;
        if (SDGameManager.Instance.DEBUG_GODLIKE
            && _characterType == SDConstants.CharacterType.Hero)
        {
            dmg = 0;
        }
        if (canReduce)
        {
            float DmgR = AllRandomSetClass.SimplePercentToDecimal
            (Mathf.Abs(100 - _unit.ThisBasicRoleProperty().DmgReduction));
            dmg = (int)(dmg * DmgR);
        }
        //配置额外伤害加成
        dmg = _unit.IsEnemy ? _unit.BM.heroToEnemyDmg(dmg) : _unit.BM.enemyToHeroDmg(dmg);
        //Debug.Log(name + "是否被击中 闪避" + isEvoHappen + "  " + "失误" + isFault);
        if (CurrentShieldHp > 0)
        {
            if (isEvoHappen || isFault) dmg = 0;
            //打破护盾
            if(dmg >= CurrentShieldHp)
            {
                CurrentShieldHp = 0;
                checkShieldFull();
                checkHpFull();
            }
            else
            {
                CurrentShieldHp -= dmg;
                checkShieldFull();
            }

        }
        else
        {
            int def = 0;
            if (kind == SkillKind.Physical)
            {
                def = _unit.ThisBasicRoleProperty()._role.ad;
            }
            else if (kind == SkillKind.Elemental)
            {
                def = _unit.ThisBasicRoleProperty()._role.md;
            }
            else if (kind == SkillKind.Arcane)
            {
                def = _unit.ThisBasicRoleProperty()._role.ad / 2
                    + _unit.ThisBasicRoleProperty()._role.md / 2;
            }
            dmg -= def;
            dmg = Mathf.Max(dmg, SDConstants.MinDamageCount);

            //添加Buff类建筑的影响
            dmg = BattleGroupStateController.setNormalDmgByPlayerData(dmg, kind, _unit);

            if (isEvoHappen || isFault) dmg = 0;
            showDamageAnimation(dmg);
            //检测是否死亡
            if (dmg >= CurrentHp)
            {
                _unit.IsDead = true;
                CurrentHp = 0;
                _unit.playDieAnimation();
            }
            else
            {
                CurrentHp -= dmg;
                if (receiveExtraDamage)
                {
                    getExtraDamage(currentExtraDamage);
                }
                else
                {
                    _unit.playHurtAnimation();
                }

            }
            checkHpFull();
        }
        //
        _unit.CheckStatesWithTag_beAtked();
    }
    public void getExtraDamage(int dmg, bool isCounter = false)
    {
        if (SDGameManager.Instance.DEBUG_GODLIKE && _characterType == SDConstants.CharacterType.Hero)
            dmg = 0;
        if(dmg!=0)
            showExtraDamageAnimation(dmg, isCounter);
        if (dmg >= CurrentHp)
        {
            _unit.IsDead = true;
            CurrentHp = 0;
            HpText.text = "" + CurrentHp;
            _unit.playDieAnimation();
        }
        else
        {
            CurrentHp -= dmg;
            HpText.text = "" + CurrentHp;
            _unit.playHurtAnimation();
        }
        checkHpFull();
        //
        _unit.CheckStatesWithTag_beAtked();
    }

    public void FadeAndDisappear()
    {
        if (_unit.IsDead) return;
        _unit.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (_unit.unit_character_model.CurrentCharacterModel.anim_fade, true);
        setHp(0);
        _unit.IsDead = true;
        _unit.playFadeAnimation();
    }

    public void showDamageAnimation(int dmg)
    {
        if (!_unit.IsDead)
        { ShowDamageAnimation(dmg); }

    }
    public void ShowDamageAnimation(int dmg)
    {
        StartCoroutine(IEShowDamageAnim(dmg));
    }
    public IEnumerator IEShowDamageAnim(int dmg)
    {
        StartCoroutine(IEShowTextAnim(DmgText, dmg, false));
        if (_unit.unit_character_model.CurrentCharacterModel == null) yield break;
        _unit.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (_unit.unit_character_model.CurrentCharacterModel.anim_hurt);
        yield return new WaitForSeconds(0.25f);
        _unit.unit_character_model.CurrentCharacterModel.ChangeModelAnim
            (_unit.unit_character_model.CurrentCharacterModel.anim_idle, true);
    }


    public void showExtraDamageAnimation(int dmg, bool isCounter=false)
    {
        if (!_unit.IsDead) ShowExtraDamageAnimation(dmg, isCounter);
    }
    public void ShowExtraDamageAnimation(int dmg,bool isCounter = false)
    {
        StartCoroutine(IEShowExtraDamageAnimation(dmg, isCounter));
    }
    public IEnumerator IEShowExtraDamageAnimation(int dmg,bool isCounter = false)
    {
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(IEShowTextAnim(ExtraDmgText, dmg, isCounter));
    }


    public void consumeHp(int val)
    {
        if (SDGameManager.Instance.DEBUG_GODLIKE) return;
        CurrentHp -= val;
        CurrentHp = Mathf.Max(CurrentHp, 1);
        HpText.text = "" + CurrentHp;
        checkHpFull();
    }
    public void addHp(int val, bool useSpecial = false)
    {
        if (useSpecial)
        {
            isEvoHappen = false;
            if (isCriDmg)
            {
                float c = AllRandomSetClass.SimplePercentToDecimal(_unit.ThisBasicRoleProperty().CRIDmg + 100);
                val = (int)(val * c);
            }
            if (isFault) val = 0;
            showHealAnimation(val);
            CurrentHp += val;
        }
        else
        {
            isCriDmg = isEvoHappen = isFault = false;
            showHealAnimation(val);
            CurrentHp += val;
        }
        if (CurrentHp >= _unit.ThisBasicRoleProperty()._role.ReadCurrentRoleRA((int)AttributeData.Hp))
        {
            CurrentHp = _unit.ThisBasicRoleProperty()._role.ReadCurrentRoleRA((int)AttributeData.Hp);
        }
        HpText.text = "" + CurrentHp;
        checkHpFull();
    }
    public void setHp(int val)
    {
        CurrentHp = val < MaxHp ? val : MaxHp;
        HpText.text = "" + CurrentHp;
        checkHpFull();
    }
    public void recoverHp()
    {
        CurrentHp = MaxHp;
        HpText.text = "" + CurrentHp;
        checkHpFull();
    }
    public void showHealAnimation(int val)
    {
        if (_unit.IsDead) return;
        StartCoroutine(IEShowTextAnim(HealText, val, true));
    }
    public IEnumerator IEShowTextAnim(Text text, int val, bool isAddVal = false)
    {
        text.transform.localPosition = Vector3.zero;
        text.transform.localScale = Vector3.one;
        text.text = (isAddVal ? "+" : "-") + val;
        text.color = SDConstants.color_red;
        if (isAddVal)
        {
            text.color = SDConstants.color_green;
            if (isCriDmg)
            {
                isCriDmg = false;
                text.text = "暴击" + " +" + val;
            }
        }
        if (isCriDmg&&!isAddVal)
        {
            isCriDmg = false;
            text.color = SDConstants.color_orange;
            text.text = "暴击" + " " + val;
        }
        if (isFault)
        {
            isFault = false;
            text.color = SDConstants.color_red_dark;
            text.text = "失误";//FAULT
        }
        if (isEvoHappen)
        {
            isEvoHappen = false;
            text.color = SDConstants.color_blue_light;
            text.text = "闪避";//MISS
        }

        text.gameObject.SetActive(true);
        text.DOFade(1, TEXT_FADE_TIME);
        yield return new WaitForSeconds(TEXT_FADE_TIME);
        text.transform.DOLocalMove(Vector2.up * TEXT_MOVE_HEIGHT, TEXT_MOVE_TIME);
        text.DOFade(0, TEXT_FADE_TIME);
        yield return new WaitForSeconds(TEXT_FADE_TIME);
        text.gameObject.SetActive(false);
    }

    public void checkHpFull()
    {
        HpText.text = "" + CurrentHp;
        if (CurrentHp == MaxHp) { HpText.color = Color.white; }
        else { HpText.color = Color.white; }
        refreshHpBarChart();
    }
    public void checkShieldFull()
    {
        ShieldText.text = "" + CurrentShieldHp;
        float scale = CurrentShieldHp * 1f / MaxShieldHp;
        foreShieldImg.transform.localScale = new Vector3(scale, 1, 1);
    }

    public void refreshHpBarChart()
    {
        float scale = MaxHp>0?CurrentHp * 1f / MaxHp:0;
        foreImg.transform.localScale = new Vector3(scale, 1, 1);
    }


    
}
