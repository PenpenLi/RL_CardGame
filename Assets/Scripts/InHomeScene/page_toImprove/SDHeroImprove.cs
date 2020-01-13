using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

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
                _currentImproveKind = value;
                currentImproveKindIntger = (int)_currentImproveKind;
            }
        }
    }
    [Space(50)]
    public SDHeroDetail heroDetail;
    public Transform emptyStockPanel;
    [Header("exp_part")]
    public Text lvText;
    public Text expText;
    public Transform expSlider;
    public Transform expSlider_listorder;
    [Header("star_part")]
    public ItemStarVision starVision;
    [Header("likability_part")]
    public Transform LikabilityTrans;
    [Header("skill_part")]
    public SDSkillSelect skillSelect;

    public override void InitImprovePanel()
    {
        base.InitImprovePanel();

        stockPage.heroImproveController = this;

        stocksInit(currentImproveKind);
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
        lvText.text = SDGameManager.T("Lv.") + lv;
        int e0 = exp - SDDataManager.Instance.getMinExpReachLevel(lv);
        int e1 = SDDataManager.Instance.ExpBulkPerLevel(lv + 1);
        expText.text = e0 + "/" + e1;
        expSlider.localScale = Vector3.up + Vector3.forward + Vector3.right * SDDataManager.Instance.getExpRateByExp(exp);
    }

    public void stocksInit(ImproveKind improveKind)
    {
        stockPage.pageIndex = 0;

        SDConstants.MaterialType MType = SDConstants.MaterialType.exp;
        if (improveKind == ImproveKind.exp) 
        {
            maxSelectedNum = 10;
        }
        else if (improveKind == ImproveKind.star) 
        {
            MType = SDConstants.MaterialType.star;
            maxSelectedNum = heroDetail.StarNumVision.StarNum < 3 ? 2 : 3;
        }
        else if(improveKind == ImproveKind.likability)
        {
            MType = SDConstants.MaterialType.likability;
            maxSelectedNum = 10;
        }
        else if(improveKind == ImproveKind.skill)
        {
            MType = SDConstants.MaterialType.skill;
            maxSelectedNum = 10;
        }
        currentImproveKind = improveKind;
        stockPage.ResetPage();
        stockPage.maxSelectedNum = maxSelectedNum;
        stockPage.ItemInitForHero(MType);
        stockPage.SelectEmpty();
        RefreshImprovePanel();
        if (stockPage.items.Count == 0) emptyStockPanel.gameObject.SetActive(true);
        else emptyStockPanel.gameObject.SetActive(false);
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

    public void BtnToChangeImproveKind()
    {
        int c = (int)currentImproveKind;
        ImproveKind k = (ImproveKind)((c + 1) % (int)ImproveKind.end);
        stocksInit(k);
    }
}
