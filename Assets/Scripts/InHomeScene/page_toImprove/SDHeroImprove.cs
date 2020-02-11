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
            }
        }
    }
    [Space(50)]
    public SDHeroDetail heroDetail;
    //
    [Header("General")]
    public Sprite[] BtnSprite = new Sprite[2];
    public Transform belowSubPanel;
    [Header("exp_part")]
    public Transform improvePanel_exp;
    public Transform useListPanel_exp;
    public Button btn_exp;
    //
    public Text expPart_lv_text;
    public Text expPart_exp_text;
    public Text expPart_successrate_text;
    //
    public Transform OneStockShow;
    public Transform StockContent_exp;
    public List<oneStockController> AllExpConsumables = new List<oneStockController>();
    public int currentAddingExp;
    [Header("star_part")] 
    public Transform improvePanel_star;
    public Transform useListPanel_star;
    public Button btn_star;
    //
    public ItemStarVision starVision;
    //[Header("likability_part")]
    //public Transform LikabilityTrans;
    [Header("skill_part")]
    public Transform improvePanel_skill;
    public Transform useListPanel_skill;
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
    public override void RefreshImprovePanel()
    {
        base.RefreshImprovePanel();
        //skill
        if (AllImproveTrans[(int)ImproveKind.skill].gameObject.activeSelf)
        {
            skillSelect.initHeroAllSkills();
        }
        else
        {
            skillSelect.resetSkillList();
        }
        //
        starVision.StarNum = heroDetail.LEVEL;

        //
        int hashcode = heroDetail.Hashcode;
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(hashcode);
        int exp = hero.exp;
        int lv = SDDataManager.Instance.getLevelByExp(exp);
    } 
    public override void ConsumeToImprove(List<RTSingleStockItem> list)
    {
        base.ConsumeToImprove(list);

        ImproveKind kind = currentImproveKind;
        int hashcode = heroDetail.Hashcode;

        #region 换算系统
        for (int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if (stock.stockType == SDConstants.StockType.material)
            {

                //Debug.Log("升级材料");
                if (kind == ImproveKind.exp && stock.materialType == SDConstants.MaterialType.exp)
                {
                    int useNum = stock.UsedNum;
                    if (SDDataManager.Instance.consumeConsumable
                        (stock.itemId, out int residue, stock.UsedNum))
                    {
                        SDDataManager.Instance.addExpToHeroByHashcode
                            (hashcode, SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * useNum);
                        Debug.Log("成功消耗材料id" + list[i].itemId 
                            +" " + useNum + "个 剩余" + residue);
                    }
                    else Debug.Log("无消耗不进行强化");
                }
                else if(kind == ImproveKind.star && stock.materialType == SDConstants.MaterialType.star)
                {
                    //SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).starNumUpgradeTimes++;
                }
                else if(kind == ImproveKind.skill && stock.materialType == SDConstants.MaterialType.skill)
                {
                    if (list[i].itemId == "M_M#3010011")//随机开启或+1
                    {
                        skillImprove_material0(list[i].UsedNum);
                    }
                    else if(list[i].itemId == "M_M#3010012")//全部开启或+1
                    {
                        skillImprove_material1(list[i].UsedNum);
                    }
                }
                else if(kind == ImproveKind.likability && stock.materialType == SDConstants.MaterialType.likability)
                {
                    int usenum = stock.UsedNum;
                    SDDataManager.Instance.addLikabilityToHeroByHashcode
                        (hashcode, SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * usenum);
                }
            }
            else if (stock.stockType == SDConstants.StockType.hero)
            {
                if (kind == ImproveKind.exp)
                {
                    SDDataManager.Instance.addExpToHeroByHashcode
                        (hashcode, SDDataManager.Instance.getHeroExpPrice(stock.hashcode));
                }
                else if(kind == ImproveKind.star)
                {

                }
                else if(kind == ImproveKind.skill)
                {
                    skillImprove_hero(SDDataManager.Instance.getHeroByHashcode(list[i].hashcode));
                }
                else if(kind == ImproveKind.likability)
                {

                }
                SDDataManager.Instance.consumeHero(stock.hashcode);
            }
        }
        if(kind == ImproveKind.exp) { }
        else if(kind == ImproveKind.star)
        {
            SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).starNumUpgradeTimes++;
        }
        else if(kind == ImproveKind.skill)
        {
            skillSelect.initHeroAllSkills();
        }
        else if(kind == ImproveKind.likability)
        {

        }
        #endregion

        heroDetail.initHeroDetailPanel(hashcode);
    }
    #region 技能升级材料换算
    public void skillImprove_material0(int useNum)
    {
        List<OneSkill> allSs = SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode);
        for (int j = 0; j < useNum; j++)
        {
            List<OneSkill> all = new List<OneSkill>();
            for (int i = 0; i < allSs.Count; i++)
            {
                if (SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode)[i].lv < SDConstants.SkillMaxGrade)
                {
                    all.Add(allSs[i]);
                }
            }
            int aim = UnityEngine.Random.Range(0, all.Count);
            OneSkill targetSkill = all[aim];
            foreach (GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
            {
                if (skill.Id == targetSkill.skillId)
                {
                    //选择该技能+1
                    skill.Lv++;
                    if (skill.Lv > SDConstants.SkillMaxGrade) skill.Lv = SDConstants.SkillMaxGrade;
                    SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                    break;
                }
            }
        }

    }
    public void skillImprove_material1(int useNum)
    {
        List<OneSkill> allSs = SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode);
        List<OneSkill> all = new List<OneSkill>();
        for (int i = 0; i < allSs.Count; i++)
        {
            if (allSs[i].lv < SDConstants.SkillMaxGrade)
            {
                all.Add(allSs[i]);
            }
        }
        for (int j = 0; j < useNum; j++)
        {
            for (int i = 0; i < all.Count; i++)
            {
                foreach (GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
                {
                    if (skill.Id == all[i].skillId)
                    {
                        skill.Lv++;
                        if (skill.Lv > SDConstants.SkillMaxGrade) skill.Lv = SDConstants.SkillMaxGrade;
                        SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                        break;
                    }
                }
            }
        }
    }
    public void skillImprove_hero(GDEHeroData heroMaterial)
    {
        List<OneSkill> allSs = SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode);
        List<string> ups = new List<string>();
        for(int i = 0; i < heroMaterial.skillsOwned.Count; i++)
        {
            bool flag = false;
            foreach (GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
            {
                if (skill.Id == heroMaterial.skillsOwned[i].Id && skill.Lv >= SDConstants.SkillMaxGrade)
                {
                    flag = true;break;
                }
            }
            if(!flag) ups.Add(heroMaterial.skillsOwned[i].Id);
        }

        for(int i = 0; i < ups.Count; i++)
        {
            foreach(GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
            {
                if(skill.Id == ups[i])
                {
                    skill.Lv++;
                    if (skill.Lv > SDConstants.SkillMaxGrade) skill.Lv = SDConstants.SkillMaxGrade;
                    SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                    Debug.Log("UPSKILL: " + skill.Id + "---" + skill.Lv);
                    break;
                }
            }
            SDDataManager.Instance.PlayerData.Set_herosOwned();
        }
    }
    #endregion
    public bool expectImprove_before(List<RTSingleStockItem> list, ImproveKind kind, RTSingleStockItem newStock)
    {
        bool flag = false;
        int figure = 0;
        for(int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if(stock.stockType == SDConstants.StockType.material)
            {
                if(kind == ImproveKind.exp && stock.materialType == SDConstants.MaterialType.exp)
                {
                    figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * stock.UsedNum;
                }
                else if (kind == ImproveKind.star && stock.materialType == SDConstants.MaterialType.star)
                {
                    figure++;
                }
                else if(kind == ImproveKind.skill && stock.materialType == SDConstants.MaterialType.skill)
                {

                }
                else if(kind == ImproveKind.likability && stock.materialType == SDConstants.MaterialType.likability)
                {
                    figure += SDDataManager.Instance.getFigureFromMaterial(stock.itemId) * stock.UsedNum;
                }
            }
            else if(stock.stockType == SDConstants.StockType.hero)
            {
                if(kind == ImproveKind.exp)
                {
                    figure += SDDataManager.Instance.getHeroExpPrice(stock.hashcode);
                }
                else if(kind == ImproveKind.star && stock.materialType == SDConstants.MaterialType.star)
                {
                    figure++;
                }
                else if(kind == ImproveKind.skill && stock.materialType == SDConstants.MaterialType.skill)
                {

                }
                else if(kind == ImproveKind.likability && stock.materialType == SDConstants.MaterialType.likability)
                {

                }
            }
        }
        if(kind == ImproveKind.exp)
        {
            int exp = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).exp;
            flag = !SDDataManager.Instance.checkHeroExpIfOverflow(exp + figure
                , heroDetail.StarNumVision.StarNum);
        }
        else if(kind == ImproveKind.star)
        {
            if(heroDetail.StarNumVision.StarNum < 3 && figure < 2)
            {
                flag = true;
            }
            else if(heroDetail.StarNumVision.StarNum >=3 && figure < 3)
            {
                flag = true;
            }
        }
        else if(kind == ImproveKind.skill)
        {
            flag = checkifUseThisInSkillImprove(list, newStock);
        }
        else if(kind == ImproveKind.likability)
        {
            if (figure < SDConstants.MinHeartVolume * 8.5f)
            {
                flag = true;
            }
        }
        return flag;
    }

    public bool checkifUseThisInSkillImprove(List<RTSingleStockItem> list , RTSingleStockItem newStock)
    {
        ///现已解锁的所有技能
        List<GDEASkillData> all = SDDataManager.Instance.OwnedSkillsByHero(heroDetail.Hashcode);
        int m0 = 0;
        for (int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if(stock.materialType == SDConstants.MaterialType.skill)
            {
                string specialStr = SDDataManager.Instance.getMaterialSpecialStr(stock.itemId);
                if(specialStr.Contains("random1"))//m0
                { 
                    m0 = stock.UsedNum; 
                }
                else if(specialStr.Contains("all"))//m1
                {
                    foreach(var s in all)
                    {
                        s.Lv += stock.UsedNum;
                    }
                }
            }
            else if(stock.materialType == SDConstants.MaterialType.skill)
            {
                all = listAddHeroSkill(all, SDDataManager.Instance.getHeroByHashcode(stock.hashcode));
            }
        }
        int left = 0;
        for(int i = 0; i < all.Count; i++)
        {
            left += SDConstants.SkillMaxGrade - all[i].Lv;
        }

        if(newStock.stockType == SDConstants.StockType.material)
        {
            if (m0 < left)
            {
                return true;
            }
            return false;
        }
        else if(newStock.stockType == SDConstants.StockType.hero)
        {
            for(int i = 0; i < SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned.Count; i++)
            {
                string id = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned[i].Id;
                foreach(GDEASkillData s in all)
                {
                    if(s.Id == id && s.Lv < SDConstants.SkillMaxGrade)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }
    public List<GDEASkillData> listAddHeroSkill(List<GDEASkillData> oldAll, GDEHeroData hero)
    {
        List<GDEASkillData> all = oldAll;
        for(int i = 0; i < hero.skillsOwned.Count; i++)
        {
            GDEASkillData skill = hero.skillsOwned[i];
            foreach(GDEASkillData s in all)
            {
                if(s.Id == skill.Id)
                {
                    s.Lv++;
                    break;
                }
            }
        }
        return all;
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

        //
        RectTransform rect = belowSubPanel.GetComponent<RectTransform>();
        if (oldKind == ImproveKind.exp)
        {
            rect.anchoredPosition = Vector2.down * 1200;
            rect.DOAnchorPos(Vector2.zero, animInterval);
        }
        if(newKind == ImproveKind.exp)
        {
            rect.DOAnchorPos(Vector2.down * 1200, animInterval);
        }
    }
    void clearAllpart()
    {
        concealExpPart();
        concealstarPart();
        concealskillPart();
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
        useListPanel_exp.gameObject.SetActive(true);
        useListPanel_exp.localScale = Vector2.one;
        RectTransform rect1 = useListPanel_exp.GetComponent<RectTransform>();
        rect1.anchoredPosition = Vector2.left * rect1.sizeDelta.x * 1.1f;
        rect1.DOAnchorPos(Vector2.zero, animInterval);
        //
        Outline l = btn_exp.GetComponent<Outline>();
        if (l){ l.enabled = true; }
        btn_exp.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[1];
        //
        refreshHeroData_exp();
    }
    void concealExpPart()
    {
        RectTransform rect = improvePanel_exp.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y * 1.1f, animInterval);
        //
        RectTransform rect1 = useListPanel_exp.GetComponent<RectTransform>();
        rect1.DOAnchorPos(Vector2.right * rect1.sizeDelta.x * 1.1f, animInterval);
        //
        btn_exp.GetComponent<Outline>().enabled = false;
        btn_exp.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[0];
    }
    public void refreshHeroData_exp()
    {
        GDEHeroData data = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode);
        int currentlv = SDDataManager.Instance.getLevelByExp(data.exp);
        expPart_lv_text.text = SDGameManager.T("Lv.") + currentlv;
        int _exp = data.exp - SDDataManager.Instance.getMinExpReachLevel(currentlv);
        expPart_exp_text.text = _exp + "/" + SDDataManager.Instance.ExpBulkPerLevel(currentlv);
        expPart_successrate_text.text = SDGameManager.T("大成功率:") + 0 + "%";
        //
        showAllStocks_exp();
    }
    void showAllStocks_exp()
    {
        List<string> alls 
            = SDDataManager.Instance.getConsumablesOwned.Select(x => x.id).ToList();
        List<string> all = alls.FindAll(x =>
        {
            consumableItem item = SDDataManager.Instance.getConsumableById(x);
            return item.MaterialType == SDConstants.MaterialType.exp;
        });
        List<oneStockController> have = StockContent_exp
            .GetComponentsInChildren<oneStockController>().ToList();
        List<string> haves = have.Select(x => x.itemId).ToList();
        foreach(oneStockController c in have)
        {
            c.refreshSelf();
        }
        for(int i = 0; i < all.Count; i++)
        {
            if (haves.Contains(all[i]))
            {
                return;
            }
            Transform s = Instantiate(OneStockShow) as Transform;
            s.SetParent(StockContent_exp);
            s.localScale = Vector3.one;
            oneStockController c = s.GetComponent<oneStockController>();
            c.Init(all[i]);
            AllExpConsumables.Add(c);
        }
    }
    public void refreshCurrentNewAddingExp()
    {

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
        useListPanel_star.gameObject.SetActive(true);
        useListPanel_star.localScale = Vector2.one;
        RectTransform rect1 = useListPanel_star.GetComponent<RectTransform>();
        rect1.anchoredPosition = Vector2.left * rect1.sizeDelta.x * 1.1f;
        rect1.DOAnchorPos(Vector2.zero, animInterval);
        //
        Outline l = btn_star.GetComponent<Outline>();
        if (l) { l.enabled = true; }
        btn_star.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[1];
        //
        refreshHeroData_star();
    }
    void concealstarPart()
    {
        RectTransform rect = improvePanel_star.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y * 1.1f, animInterval);
        //
        RectTransform rect1 = useListPanel_star.GetComponent<RectTransform>();
        rect1.DOAnchorPos(Vector2.right * rect1.sizeDelta.x * 1.1f, animInterval);
        //
        btn_star.GetComponent<Outline>().enabled = false;
        btn_star.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[0];
    }
    public void refreshHeroData_star()
    {
        GDEHeroData data = SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode);
        HeroInfo indo = SDDataManager.Instance.getHeroInfoById(data.id);
        starVision.StarNum = data.starNumUpgradeTimes + indo.LEVEL;
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
        useListPanel_skill.gameObject.SetActive(true);
        useListPanel_skill.localScale = Vector2.one;
        RectTransform rect1 = useListPanel_skill.GetComponent<RectTransform>();
        rect1.anchoredPosition = Vector2.left * rect1.sizeDelta.x * 1.1f;
        rect1.DOAnchorPos(Vector2.zero, animInterval);
        //
        Outline l = btn_skill.GetComponent<Outline>();
        if (l) { l.enabled = true; }
        btn_skill.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[1];
        //
        refreshHeroData_skill();
    }
    void concealskillPart()
    {
        RectTransform rect = improvePanel_skill.GetComponent<RectTransform>();
        rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y * 1.1f, animInterval);
        //
        RectTransform rect1 = useListPanel_skill.GetComponent<RectTransform>();
        rect1.DOAnchorPos(Vector2.right * rect1.sizeDelta.x * 1.1f, animInterval);
        //
        btn_skill.GetComponent<Outline>().enabled = false;
        btn_skill.transform.GetChild(0).GetComponent<Image>().sprite = BtnSprite[0];
    }
    public void refreshHeroData_skill()
    {
        skillSelect.initHeroAllSkills();
    }
    #endregion

}
