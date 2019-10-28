using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ActionPanelController : MonoBehaviour
{
    public Transform activeSign;
    public SkillFunction CurrentSkill;
    public SkillFunction DefaultSkill;
    public SkillFunction[] SkillGroup;
    [HideInInspector]
    public BattleManager BM;
    [HideInInspector]
    public SkillDetailsList SDL;
    //public Transform[] allSkillsPrefabs;
    #region 视觉化设置
    public RectTransform ActionPanel;
    public Transform[] skillPos;
    #region 显示技能详细信息
    [Header("选中技能的详细信息展示")]
    public RectTransform SkillPanelContent;
    private Transform SkillDetailsPanel;
    public Text SkillLimit;
    public Text SkillName;
    public Text SkillTag;
    public Text SkillBaseData;
    public Text SkillStateList;
    public Text SkillDesc;
    private float showAndHideTime = 0.02f;
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        SkillDetailsPanel = SkillPanelContent.GetChild(0);
        SkillDetailsPanel.localScale = Vector3.zero;
        GetComponent<CanvasGroup>().alpha = 0;
        //
        if (BM == null) BM = FindObjectOfType<BattleManager>();
        DefaultSkill = BM.normalAttackSkill;
    }

    public void initActionPanel(List<OneSkill> skills, int heroId)
    {
        BM = GetComponentInParent<BattleManager>();
        SDL = GetComponentInParent<SkillDetailsList>();
        for (int i = 0; i < skillPos.Length; i++)
        {
            if (i < skills.Count)
            {
                skillPos[i].gameObject.SetActive(true);
                addSkill(skills[i], skillPos[i], heroId);
            }
            else
            {
                skillPos[i].gameObject.SetActive(false);
            }
        }
        SkillGroup = ActionPanel.GetComponentsInChildren<SkillFunction>();
    }
    #region 技能列表显示与隐藏
    public void showActionPanel()
    {
        this.transform.position = BM.ActionPanelPos.position;
        this.transform.SetParent(BM.ActionPanelPos);
        this.transform.localScale = Vector3.one;
        this.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        float dis = -BM.ActionPanelPos.GetComponent<RectTransform>().sizeDelta.y/2;
        this.GetComponent<RectTransform>().offsetMax = Vector2.up * dis;
        //
        ActionPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ActionPanel.GetComponent<CanvasGroup>().alpha = 0;
        ActionPanel.GetComponent<CanvasGroup>().interactable = false;
        refreshSkillsStatus();
        if(BM._currentBUType == SDConstants.CharacterType.Hero)
        {
            GetComponent<CanvasGroup>().alpha = 1;
            //ActionPanel.localScale = Vector3.zero;
            ActionPanel.GetComponent<CanvasGroup>().DOFade(1, showAndHideTime);
            //ActionPanel.DOScale(Vector3.one, showAndHideTime);
            ActionPanel.GetComponent<CanvasGroup>().interactable = true;

            if(!DefaultSkill.GetComponent<NormalAttack>())
                DefaultSkill.BtnTapped();
        }
        this.transform.SetAsLastSibling();
    }
    public void hideActionPanel()
    {
        if (ActionPanel == null) return;
        //bm.hideskilldetails
        //Debug.Log("收起技能栏:" + GetComponentInParent<BattleRoleData>().name);
        if (SDDataManager.Instance.SettingData.isAutoBattle)
        {
            ActionPanel.GetComponent<CanvasGroup>().interactable = false;
            ActionPanel.GetComponent<CanvasGroup>().DOFade(0, showAndHideTime);
            //ActionPanel.DOScale(Vector3.zero, showAndHideTime);
            StartCoroutine(IEHideActionPanel());
        }
        else
        {
            ActionPanel.GetComponent<CanvasGroup>().alpha = 1;
            ActionPanel.GetComponent<CanvasGroup>().interactable = false;
            ActionPanel.GetComponent<CanvasGroup>().DOFade(0, showAndHideTime);
            //ActionPanel.DOScale(Vector3.zero, showAndHideTime);
            StartCoroutine(IEHideActionPanel());
        }
    }
    public void hideEnemyActionPanel()
    {
        //Debug.Log("收起技能栏---敌方:" + GetComponentInParent<BattleRoleData>().name);
        ActionPanel.GetComponent<CanvasGroup>().alpha = 0;
        ActionPanel.GetComponent<CanvasGroup>().interactable = false;
        ActionPanel.GetComponent<RectTransform>().anchoredPosition
            = BM.ActionPanelHidePos.GetComponent<RectTransform>().anchoredPosition;
        GetComponent<CanvasGroup>().alpha = 0;
    }
    public IEnumerator IEHideActionPanel()
    {
        yield return new WaitForSeconds(showAndHideTime);
        ActionPanel.GetComponent<RectTransform>().anchoredPosition
            = BM.ActionPanelHidePos.GetComponent<RectTransform>().anchoredPosition;
        GetComponent<CanvasGroup>().alpha = 0;
    }
    #endregion
    #region 技能详情页面
    public void OpenSDP(SkillFunction skill)
    {
        HSkilInfo info = skill.GetComponent<HSkilInfo>();
        OneSkill detail = info.HSDetail;
        SkillPanelContent.GetComponent<Image>().color = Color.clear;
        SkillPanelContent.GetChild(0).localScale = Vector3.zero;
        SkillPanelContent.localScale = Vector3.one;
        SkillDetailsPanel.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        //显示当前技能详情
        ShowThisSkillDetails( skill, detail);
    }
    public void CloseSDP()
    {

        SkillDetailsPanel.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack);
        SkillPanelContent.GetComponent<Image>().DOColor(Color.clear, 0.2f);
        StartCoroutine(IECloseSDP());

    }
    public IEnumerator IECloseSDP()
    {
        yield return new WaitForSeconds(0.2f);
        SkillPanelContent.localScale = Vector3.zero;
    }
    public void ShowThisSkillDetails(SkillFunction skill, OneSkill detail)
    {
        SkillName.text = detail.SkillName + "·Lv " + skill.SkillGrade;
        SkillBaseData.text 
            = (skill.CritR != 0 ? string.Format("基础暴击修正 {0:D}", skill.CritR) : "")
            + (skill.AccuracyR!=0?string.Format("·基础精度修正 {0:D}",skill.AccuracyR):"")
            + (skill.ExpectR!=0?string.Format("·基础期望修正 {0:D}",skill.ExpectR):"");
        SkillDesc.text = detail.Desc;

        /*
        SkillTag.text = CurrentSkill.ThisSkillBreed.ToString()
            + "·" + CurrentSkill.ThisSkillKind.ToString() + "·" + CurrentSkill.ThisSkillTarget.ToString();
        SkillBaseData.text = (CurrentSkill.AccuracyR != 0 ? "基础精度修正(" + SignedNumber(CurrentSkill.AccuracyR) + "%)·" : "")
            + CurrentSkill.AllTargetResults != null ? DamageFixText(CurrentSkill.AllTargetResults.All_Use_Array[0]) : ""
            + (CurrentSkill.CritR != 0 ? "基础暴击修正(" + SignedNumber(CurrentSkill.CritR) + "%)·" : "");
        */
    }
    string SignedNumber(int number)
    {
        if (number >= 0) return "+" + number.ToString();
        else return "-" + number.ToString();
    }
    #endregion

    public void refreshSkillsStatus()
    {
        activeSign.gameObject.SetActive(false);
        if(BM._currentBUType == SDConstants.CharacterType.Enemy)
        {
            if (SkillGroup != null && SkillGroup.Length > 0)
            {
                chooseASkillFromSkillGroup();
                DefaultSkill = CurrentSkill;
            }
        }
        CurrentSkill = DefaultSkill;
        BM.showCurrentUnitSkillTarget();
        BroadcastMessage("refreshBtnState");
    }
    public void DeactiveAllBtns()
    {
        BroadcastMessage("DeactiveBtn");
    }
    public void chooseASkillFromSkillGroup()
    {
        CurrentSkill = SkillGroup[0];
        for(int i = 0; i < SkillGroup.Length; i++)
        {
            SkillFunction skill = SkillGroup[i];
            if (skill.isSkillAvailable())
            {
                if (skill.isSkillMeetConditionToAutoRelease())
                {
                    CurrentSkill = skill;
                    return;
                }
            }
        }
    }
    public void chooseRandomSkillFromGroup()
    {
        //CurrentSkill = SkillGroup[0];
        List<SkillFunction> list = new List<SkillFunction>();
        for(int i = 0; i < SkillGroup.Length; i++)
        {
            SkillFunction skill = SkillGroup[i];
            if(skill.isSkillAvailable() && skill.isSkillMeetConditionToAutoRelease())
            {
                list.Add(skill);
            }
        }
        if (list.Count > 0)
            CurrentSkill = list[UnityEngine.Random.Range(0, list.Count)];
        else
        {
            if (BM == null) BM = FindObjectOfType<BattleManager>();
            CurrentSkill = BM.normalAttackSkill;
        }
    }
    public void addSkill(OneSkill skill,Transform trans, int heroId)
    {
        int index = skill.SkillFunctionID;
        Transform s = Instantiate(SDL.AllSkillList[index]) as Transform;
        s.GetComponentInChildren<HSkilInfo>().HSDetail = skill;
        s.SetParent(trans);
        s.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        s.localScale = Vector3.one;
        HSkilInfo info = s.GetComponent<HSkilInfo>();
        if (skill.Breed != SkillBreed.End) info.breed = skill.Breed;
        if (skill.Kind != SkillKind.End) info.kind = skill.Kind;
        if (skill.SkillAoe != SDConstants.AOEType.End)
            info.AOEType = skill.SkillAoe;
        if (skill.MpTpAddType != SDConstants.AddMpTpType.End)
            info.AfterwardsAddType = skill.MpTpAddType;
        SkillFunction basicSkillController = s.GetComponent<SkillFunction>();

        if (skill.Aim!= SkillAim.End) basicSkillController.ThisSkillAim = skill.Aim;

        basicSkillController.SkillGrade = skill.lv;
        basicSkillController.IsRare = skill.isOmegaSkill;


        if(skill.BulletImg!=null && skill.BulletImg != "")
        {
            basicSkillController.bullet.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("Sprites/" + skill.BulletImg);
        }
        //判断是否为稀有角色



        //
        basicSkillController.initIcon();
    }
}
