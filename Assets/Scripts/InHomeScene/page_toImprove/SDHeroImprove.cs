using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using DG.Tweening;
using System.Linq;

public class SDHeroImprove : BasicImprovePage
{
    public enum ImproveKind
    {
        exp,
        star,
        skill,
        likability,
        end,
    }
    ImproveKind _currentImproveKind = ImproveKind.exp;
    public ImproveKind currentImproveKind
    {
        get { return _currentImproveKind; }
        set
        {
            if (_currentImproveKind != value)
            {
                refreshPart(_currentImproveKind,value);
                _currentImproveKind = value;
                currentImproveKindIntger = (int)_currentImproveKind;
                ClearAllSlots();
            }
        }
    }
    [Space(50)]
    public SDHeroDetail heroDetail;
    //
    [Header("General")]
    public StockPageController StockPage;
    public Button BtnToConfirmCurrentImprove;

    public SingleSlot[] AllSlots;
    public int currentAddingFigure;
    //anim
    private float abAnchorX_on = 0;
    private float abAnchorX_off = -30;
    private float abAnimInterval = 0.125f;

    [Header("exp_part")]
    public Transform improvePanel_exp;
    public Button btn_exp;
    //
    public Text expPart_lv_text;
    public Text expPart_exp_text;
    public Transform expPart_slider_old;
    public Transform expPart_slider_new;
    //
    [Header("star_part")] 
    public Transform improvePanel_star;
    public Button btn_star;
    //
    public ItemStarVision starVision;


    //[Header("likability_part")]
    //public Transform LikabilityTrans;
    [Header("skill_part")]
    public Transform improvePanel_skill;
    public Button btn_skill;

    public SDSkillSelect skillSelect;


    public override void InitImprovePanel()
    {
        base.InitImprovePanel();
        //
        _currentImproveKind = ImproveKind.exp;
        clearAllpart();
        refreshPart(ImproveKind.end, ImproveKind.exp);
        //
        if(heroDetail.heroHeadImg
            && heroDetail.heroHeadImg.CurrentCharacterModel
            && heroDetail.heroHeadImg.CurrentCharacterModel.GetComponent<MeshRenderer>())
        {
            heroDetail.heroHeadImg.CurrentCharacterModel
                .GetComponent<MeshRenderer>().sortingOrder = 1;
        }
    }

    void refreshPart(ImproveKind oldKind,ImproveKind newKind)
    {
        if (oldKind == newKind) return;
        if (oldKind == ImproveKind.exp) concealExpPart();
        else if (oldKind == ImproveKind.star) concealstarPart();
        else if (oldKind == ImproveKind.skill) concealskillPart();

        //
        if (newKind == ImproveKind.exp) showExpPart();
        else if (newKind == ImproveKind.star) showstarPart();
        else if (newKind == ImproveKind.skill) showskillPart();
    }
    void clearAllpart()
    {
        concealExpPart();
        concealstarPart();
        concealskillPart();
    }
    public override void RefreshImprovePanel()
    {
        //base.RefreshImprovePanel();
        if(currentImproveKind == ImproveKind.exp)
        {
            refreshCurrentNewAddingExp();
        }
        else if(currentImproveKind == ImproveKind.star)
        {
            refreshCurrentNewAddingStar();
        }
        else if(currentImproveKind == ImproveKind.skill)
        {
            refreshCurrentNewAddingSkill();
        }
    }
    public void ClearAllSlots()
    {
        foreach(SingleSlot slot in AllSlots)
        {
            slot.ClearSlot();
        }
    }

    #region exp_part
    public void Btn_exp_part()
    {
        currentImproveKind = ImproveKind.exp;
    }
    void showExpPart()
    {
        improvePanel_exp.gameObject.SetActive(true);
        improvePanel_exp.localScale = Vector3.one;
        RectTransform rect = improvePanel_exp.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.up * rect.sizeDelta.y*1.1f;
        rect.DOAnchorPos(Vector2.zero, animInterval);
        //
        Outline l = btn_exp.GetComponent<Outline>();
        if (l){ l.enabled = true; }
        btn_exp.GetComponent<RectTransform>().DOAnchorPosX(abAnchorX_on, abAnimInterval);
        //
        refreshHeroData_exp();
    }
    void concealExpPart()
    {
        RectTransform rect = improvePanel_exp.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y * 1.1f, animInterval);
        //
        btn_exp.GetComponent<Outline>().enabled = false;
        btn_exp.GetComponent<RectTransform>().DOAnchorPosX(abAnchorX_off, abAnimInterval);
    }
    public void refreshHeroData_exp()
    {
        showAllStocks_exp();
        refreshCurrentNewAddingExp();
    }
    void showAllStocks_exp()
    {
        StockPage.InitStocks_Exp();
    }
    public void refreshCurrentNewAddingExp()
    {
        currentAddingFigure = 0;
        GDEHeroData data = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode);
        //HeroInfo indo = SDDataManager.Instance.getHeroInfoById(data.id);
        for (int i = 0; i < AllSlots.Length; i++)
        {
            bool locked = SDDataManager.Instance.checkHeroExpIfOverflow
                (data.exp, heroDetail.StarNumVision.StarNum);
            SingleSlot slot = AllSlots[i];
            slot.isLocked = locked;
            if (!slot.isEmpty && !slot.isLocked)
            {
                if(slot.ItemType == SDConstants.ItemType.Hero)
                {
                    int _exp = SDDataManager.Instance.getHeroExpPrice(heroDetail.Hashcode);
                    currentAddingFigure += _exp;
                }
                else if(slot.ItemType == SDConstants.ItemType.Consumable)
                {
                    int _exp = SDDataManager.Instance.getFigureFromMaterial(slot._id);
                    currentAddingFigure += _exp;
                }
            }
        }
        //预计等级提升动画
        int oldLv = SDDataManager.Instance.getLevelByExp(data.exp);
        int newLv = SDDataManager.Instance.getLevelByExp(data.exp + currentAddingFigure);
        expPart_lv_text.text = SDGameManager.T("Lv.") + oldLv 
            + " > " 
            + SDGameManager.T("Lv.") + newLv;
        expPart_exp_text.text = (data.exp - SDDataManager.Instance.getMinExpReachLevel(oldLv) 
            + currentAddingFigure) + "/" + SDDataManager.Instance.ExpBulkPerLevel(oldLv);
        expPart_slider_old.localScale = new Vector3
            ((data.exp - SDDataManager.Instance.getMinExpReachLevel(oldLv))
            * 1f/ SDDataManager.Instance.ExpBulkPerLevel(oldLv), 1, 1);
        int overExp = data.exp + currentAddingFigure 
            - SDDataManager.Instance.getMinExpReachLevel(oldLv);
        float rate = overExp * 1f / SDDataManager.Instance.ExpBulkPerLevel(oldLv);
        expPart_slider_new.localScale = new Vector3(rate, 1, 1);

        //升级确认按钮状态
        BtnToConfirmCurrentImprove.interactable = currentAddingFigure > 0
            && !SDDataManager.Instance.checkHeroExpIfOverflow
            (data.exp, heroDetail.StarNumVision.StarNum);

        //exp专有
        TooFlowToAdd = SDDataManager.Instance.checkHeroExpIfOverflow
            (data.exp + currentAddingFigure, heroDetail.StarNumVision.StarNum);
    }
    void confirmImprove_exp()
    {
        if (currentAddingFigure == 0) return;
        bool flag = true;
        for (int i = 0; i < AllSlots.Length; i++)
        {
            SingleSlot slot = AllSlots[i];
            if (slot.isEmpty || slot.isLocked) continue;
            bool _flag = slot.consumeContentInSlot();
            if (!_flag) { flag = false; continue; }
        }
        if (!flag) return;
        SDDataManager.Instance.addExpToHeroByHashcode(heroDetail.Hashcode, currentAddingFigure);
        refreshHeroData_exp();
        //
        heroDetail.initHeroDetailPanel(heroDetail.Hashcode);
    }
    void Btn_Slot_Exp(int index)
    {
        SingleSlot slot = AllSlots[index];
        slot.ClearSlot();
        StockPage.refreshStockConditions_heroImprove();
    }
    public bool addStockToSlot_Exp(RTSingleStockItem stock)
    {
        for(int i = 0; i < AllSlots.Length; i++)
        {
            SingleSlot slot = AllSlots[i];
            if(slot.isEmpty && !slot.isLocked)
            {
                slot.AddFromStock(stock);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region star_part
    public void Btn_star_part()
    {
        currentImproveKind = ImproveKind.star;
    }
    void showstarPart()
    {
        improvePanel_star.gameObject.SetActive(true);
        improvePanel_star.localScale = Vector3.one;
        RectTransform rect = improvePanel_star.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.up * rect.sizeDelta.y * 1.1f;
        rect.DOAnchorPos(Vector2.zero, animInterval);
        //
        Outline l = btn_star.GetComponent<Outline>();
        if (l) { l.enabled = true; }
        //btn_star.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[1];
        btn_star.GetComponent<RectTransform>().DOAnchorPosX( abAnchorX_on, abAnimInterval);
        //
        refreshHeroData_star();
    }
    void concealstarPart()
    {
        RectTransform rect = improvePanel_star.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y * 1.1f, animInterval);
        //
        btn_star.GetComponent<Outline>().enabled = false;
        //btn_star.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[0];
        btn_star.GetComponent<RectTransform>().DOAnchorPosX(abAnchorX_off, abAnimInterval);
    }
    public void refreshHeroData_star()
    {
        refreshCurrentNewAddingStar();
        showAllStocks_star();
    }
    void showAllStocks_star()
    {
        stockPage.InitStocks_Star(heroDetail.StarNumVision.StarNum);
    }
    public void refreshCurrentNewAddingStar()
    {
        currentAddingFigure = 0;
        GDEHeroData data = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode);
        HeroInfo indo = SDDataManager.Instance.getHeroInfoById(data.id);
        starVision.StarNum = data.starNumUpgradeTimes + indo.LEVEL;
        int starNum = starVision.StarNum;
        if (starNum >= SDConstants.UnitMAxStarNum)
        {
            BtnToConfirmCurrentImprove.interactable = false;
            return;
        }
        currentAddingFigure = 1;
        for(int i = 0; i < AllSlots.Length; i++)
        {
            bool unlocked = i < starNum;
            AllSlots[i].isLocked = !unlocked;
            if (unlocked && AllSlots[i].isEmpty)
            {
                currentAddingFigure = 0;
            }
        }
        //预计升星动画

        //升级确认按钮状态
        BtnToConfirmCurrentImprove.interactable = currentAddingFigure > 0
            && SDDataManager.Instance.checkHeroExpIfOverflow(data.exp, starNum);
    }
    void confirmImprove_star()
    {
        if (currentAddingFigure == 0) return;
        bool flag = true;
        int skillUpgrade = 0;
        for (int i = 0; i < AllSlots.Length; i++) 
        {
            SingleSlot slot = AllSlots[i];
            if (slot.isEmpty || slot.isLocked) continue;
            bool _flag = slot.consumeContentInSlot();
            if (!_flag) { flag = false; continue; }
            // 在升星时使用同角色
            if (slot.ItemType == SDConstants.ItemType.Hero && slot._id == heroDetail.ID)
            {
                skillUpgrade++;
            }
        }
        if (!flag) return;
        if (skillUpgrade > 0)
        {
            SDDataManager.Instance.addMainSkillGradeToHeroByHashcode
                (heroDetail.Hashcode, skillUpgrade);
        }
        SDDataManager.Instance.addStarToHeroByHashcode(heroDetail.Hashcode);
        refreshHeroData_star();
        //
        heroDetail.initHeroDetailPanel(heroDetail.Hashcode);
    }
    void Btn_Slot_Star(int index)
    {
        SingleSlot slot = AllSlots[index];
        slot.ClearSlot();
        StockPage.refreshStockConditions_heroImprove();
    }
    public bool addStockToSlot_Star(RTSingleStockItem stock)
    {
        for(int i = 0; i < AllSlots.Length; i++)
        {
            SingleSlot slot = AllSlots[i];
            if(slot.isEmpty && !slot.isLocked)
            {
                slot.AddFromStock(stock);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region skill_part
    public void Btn_skill_part()
    {
        currentImproveKind = ImproveKind.skill;
    }
    void showskillPart()
    {
        improvePanel_skill.gameObject.SetActive(true);
        improvePanel_skill.localScale = Vector3.one;
        RectTransform rect = improvePanel_skill.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.up * rect.sizeDelta.y * 1.1f;
        rect.DOAnchorPos(Vector2.zero, animInterval);
        //
        Outline l = btn_skill.GetComponent<Outline>();
        if (l) { l.enabled = true; }
        //btn_skill.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[1];
        btn_skill.GetComponent<RectTransform>().DOAnchorPosX(abAnchorX_on, abAnimInterval);
        //
        refreshHeroData_skill();
    }
    void concealskillPart()
    {
        RectTransform rect = improvePanel_skill.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y * 1.1f, animInterval);
        //
        btn_skill.GetComponent<Outline>().enabled = false;
        //btn_skill.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[0];
        btn_skill.GetComponent<RectTransform>().DOAnchorPosX(abAnchorX_off, abAnimInterval);
    }
    public void refreshHeroData_skill()
    {
        refreshCurrentNewAddingSkill();
        showAllStocks_skill();
    }
    void showAllStocks_skill()
    {
        StockPage.InitStocks_skill(heroDetail.ID);
    }
    public void refreshCurrentNewAddingSkill()
    {
        currentAddingFigure = 0;
        //GDEHeroData data = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode);
        skillSelect.initHeroAllSkills();
        int MaxG = SDDataManager.Instance.SkillGradeNumWaitingForImprove(heroDetail.Hashcode);
        if (MaxG <= 0)
        {
            BtnToConfirmCurrentImprove.interactable = false;
        }
        Debug.Log("MAG: " + MaxG);
        for (int i = 0; i < AllSlots.Length; i++)
        {
            bool locked = MaxG <= i;
            SingleSlot slot = AllSlots[i];
            slot.isLocked = locked;
            if(!slot.isEmpty && !slot.isLocked)
            {
                currentAddingFigure++;
            }
        }
        //预计技能升级动画



        //升级确认按钮状态
        BtnToConfirmCurrentImprove.interactable = currentAddingFigure > 0;
    }
    void confirmImprove_skill()
    {
        if (currentAddingFigure == 0) return;
        bool flag = true;
        for (int i = 0; i < AllSlots.Length; i++)
        {
            SingleSlot slot = AllSlots[i];
            if (slot.isEmpty || slot.isLocked) continue;
            bool _flag = slot.consumeContentInSlot();
            if (!_flag) { flag = false; continue; }
        }
        if (!flag) return;
        SDDataManager.Instance.addMainSkillGradeToHeroByHashcode
            (heroDetail.Hashcode, currentAddingFigure);
        

        refreshHeroData_skill();
        //
        heroDetail.initHeroDetailPanel(heroDetail.Hashcode);
    }
    void Btn_Slot_Skill(int index)
    {
        SingleSlot slot = AllSlots[index];
        slot.ClearSlot();
        StockPage.refreshStockConditions_heroImprove();
    }
    public bool addStockToSlot_Skill(RTSingleStockItem stock)
    {
        for (int i = 0; i < AllSlots.Length; i++)
        {
            SingleSlot slot = AllSlots[i];
            if (slot.isEmpty && !slot.isLocked)
            {
                slot.AddFromStock(stock);
                return true;
            }
        }
        return false;
    }
    #endregion

    public void BtnTapped_ConfirmImprove()
    {
        if(currentImproveKind == ImproveKind.exp)
        {
            confirmImprove_exp();
        }
        else if(currentImproveKind == ImproveKind.star)
        {
            confirmImprove_star();
        }
        else if(currentImproveKind == ImproveKind.skill)
        {
            confirmImprove_skill();
        }
        //
        heroDetail.heroHeadImg.SetCurrentCMSortingOrder(1);
        ClearAllSlots();
        RefreshImprovePanel();
    }
    public void BtnTapped_ClickSlot(int index)
    {
        if(currentImproveKind == ImproveKind.exp)
        {
            Btn_Slot_Exp(index);
        }
        else if(currentImproveKind == ImproveKind.star)
        {
            Btn_Slot_Star(index);
        }
        else if(currentImproveKind == ImproveKind.skill)
        {
            Btn_Slot_Skill(index);
        }
    }
}
