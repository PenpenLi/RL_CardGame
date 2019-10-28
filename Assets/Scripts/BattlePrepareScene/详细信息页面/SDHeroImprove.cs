using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDHeroImprove : MonoBehaviour
{
    public SDHeroDetail heroDetail;
    public enum ImproveKind
    {
        exp,
        star,
        skill,
        likability,
        end,
    }
    public ImproveKind currentImproveKind = ImproveKind.exp;
    public Transform emptyStockPanel;
    //public ScrollRect rect;
    [Header("exp_part")]
    public Transform ExpTrans;
    public Text lvText;
    public Text expText;
    public Transform expSlider;
    public Transform expSlider_listorder;
    [Header("star_part")]
    public Transform StarTrans;
    public ItemStarVision starVision;
    [Header("likability_part")]
    public Transform LikabilityTrans;
    [Header("skill_part")]
    public Transform SkillTrans;
    public SDSkillSelect skillSelect;
    [Header("UseItemsPanel")]
    public Transform animPlace;
    public StockPageController stockPage;
    public int maxSelectedNum;

    public void initImprovePanel()
    {
        int hashcode = heroDetail.Hashcode;
        GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(hashcode);
        int exp = hero.exp;
        int lv = SDDataManager.Instance.getLevelByExp(exp);
        lvText.text = SDGameManager.T("Lv.") + lv;
        int e0 = exp - SDDataManager.Instance.getExpByLevel(lv);
        int e1 = (lv + 1) * SDConstants.MinExpPerLevel;
        expText.text = e0 + "/" + e1;
        expSlider.localScale = Vector3.up + Vector3.forward + Vector3.right * (e0 * 1f / e1);
        stocksInit(currentImproveKind);
    }
    public void closeThisPanel()
    {
        stockPage.ResetPage();

    }
    public void refreshImprovePanel()
    {
        ImproveKind kind = currentImproveKind;
        if(kind == ImproveKind.star)
        {
            ExpTrans?.gameObject.SetActive(false);
            StarTrans?.gameObject.SetActive(true);
            LikabilityTrans?.gameObject.SetActive(false);
            SkillTrans?.gameObject.SetActive(false);

            starVision.StarNum = heroDetail.LEVEL;
        }
        else if(kind == ImproveKind.likability)
        {
            ExpTrans?.gameObject.SetActive(false);
            StarTrans?.gameObject.SetActive(false);
            LikabilityTrans?.gameObject.SetActive(true);
            SkillTrans?.gameObject.SetActive(false);
        }
        else if(kind == ImproveKind.exp)
        {
            ExpTrans?.gameObject.SetActive(true);
            StarTrans?.gameObject.SetActive(false);
            LikabilityTrans?.gameObject.SetActive(false);
            SkillTrans?.gameObject.SetActive(false);

            lvText.text = heroDetail.LvText.text;
            expText.text = heroDetail.e0 + "/" + heroDetail.e1;
            expSlider.localScale = heroDetail.ExpSlider.localScale;
        }
        else if(kind == ImproveKind.skill)
        {
            ExpTrans?.gameObject.SetActive(false);
            StarTrans?.gameObject.SetActive(false);
            LikabilityTrans?.gameObject.SetActive(false);
            SkillTrans?.gameObject.SetActive(true);
        }

        //skill
        if (SkillTrans.gameObject.activeSelf)
        {
            SkillTrans.GetComponent<SDSkillSelect>().initHeroAllSkills();
        }
        else
        {
            SkillTrans.GetComponent<SDSkillSelect>().resetSkillList();
        }
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

        }
        else if(improveKind == ImproveKind.skill)
        {
            MType = SDConstants.MaterialType.skill; 

        }
        currentImproveKind = improveKind;
        stockPage.ResetPage();
        stockPage.maxSelectedNum = maxSelectedNum;
        stockPage.ItemInit(MType);
        stockPage.SelectEmpty();
        refreshImprovePanel();
        if (stockPage.items.Count == 0) emptyStockPanel.gameObject.SetActive(true);
        else emptyStockPanel.gameObject.SetActive(false);
    }


    public void BtnToCancelImprove()
    {
        stockPage.SelectEmpty();
    }
    public void BtnToConfirmImprove()
    {
        List<RTSingleStockItem> list = new List<RTSingleStockItem>();
        for (int i = 0; i < stockPage.items.Count; i++)
        {
            if (stockPage.items[i].isSelected) list.Add(stockPage.items[i]);
        }
        if (list.Count > 0)
        {
            consumeToImprove(list, currentImproveKind);
            //
            initImprovePanel();
        }
    }
    public void consumeToImprove(List<RTSingleStockItem> list, ImproveKind kind = ImproveKind.exp)
    {
        int hashcode = heroDetail.Hashcode;

        #region 换算系统
        for (int i = 0; i < list.Count; i++)
        {
            RTSingleStockItem stock = list[i];
            if (stock.stockType == SDConstants.StockType.material)
            {
                Debug.Log("升级材料");
                if (kind == ImproveKind.exp && stock.materialType == SDConstants.MaterialType.exp)
                {
                    int useNum = stock.UsedNum;
                    SDDataManager.Instance.addExpToHeroByHashcode
                        (hashcode, SDDataManager.Instance.getMaterialFigureById(stock.itemId) * useNum);
                }
                else if(kind == ImproveKind.star && stock.materialType == SDConstants.MaterialType.star)
                {
                    //SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).starNumUpgradeTimes++;
                }
                else if(kind == ImproveKind.skill && stock.materialType == SDConstants.MaterialType.skill)
                {
                    if (list[i].itemId == 3010011)//随机开启或+1
                    {
                        skillImprove_material0(list[i].UsedNum);
                    }
                    else if(list[i].itemId == 3010012)//全部开启或+1
                    {
                        skillImprove_material1(list[i].UsedNum);
                    }
                }
                else if(kind == ImproveKind.likability && stock.materialType == SDConstants.MaterialType.likability)
                {

                }
                SDDataManager.Instance.consumeMaterial(stock.itemId, stock.UsedNum);
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
        List<OneSkill> all = new List<OneSkill>();
        for(int i = 0; i < SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode).Count; i++)
        {
            if (SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode)[i].lv < SDConstants.SkillMaxGrade)
            {
                all.Add(all[i]);
            }
        }
        for (int j = 0; j < useNum; j++)
        {
            int aim = UnityEngine.Random.Range(0, all.Count);
            OneSkill targetSkill = all[aim];
            bool useInOwnedSkill = false;
            foreach (GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
            {
                if (skill.Id == targetSkill.skillId)
                {
                    //选择该技能+1
                    useInOwnedSkill = true;
                    skill.Lv++;
                    SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                    break;
                }
            }
            if (!useInOwnedSkill)
            {
                //选择该技能加入角色技能列表
                GDEASkillData skill = new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
                {
                    Id = targetSkill.skillId
                    ,
                    Lv = 0
                };
                SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned.Add(skill);
                SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
            }
        }

    }
    public void skillImprove_material1(int useNum)
    {
        List<OneSkill> all = new List<OneSkill>();
        for (int i = 0; i < SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode).Count; i++)
        {
            if (SDDataManager.Instance.getAllSkillsByHashcode(heroDetail.Hashcode)[i].lv < SDConstants.SkillMaxGrade)
            {
                all.Add(all[i]);
            }
        }
        for (int j = 0; j < useNum; j++)
        {
            for (int i = 0; i < all.Count; i++)
            {
                bool useInOwnedSkill = false;
                foreach (GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
                {
                    if (skill.Id == all[i].skillId)
                    {
                        useInOwnedSkill = true;
                        skill.Lv++;
                        SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                        break;
                    }
                }
                if (!useInOwnedSkill)
                {
                    GDEASkillData skill = new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
                    {
                        Id = all[i].skillId
                        ,
                        Lv = 0
                    };
                    SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned.Add(skill);
                    SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                }
            }
        }
    }
    public void skillImprove_hero(GDEHeroData heroMaterial)
    {
        List<int> ups = new List<int>();
        for(int i = 0; i < heroMaterial.skillsOwned.Count; i++)
        {
            ups.Add(heroMaterial.skillsOwned[i].Id);
        }
        for(int i = 0; i < ups.Count; i++)
        {
            bool useInOwnedSkill = false;
            foreach(GDEASkillData skill in SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned)
            {
                if(skill.Id == ups[i])
                {
                    useInOwnedSkill = true;
                    skill.Lv = Mathf.Min(skill.Lv++ , SDConstants.SkillMaxGrade);
                    SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
                    break;
                }
            }
            if (!useInOwnedSkill)
            {
                GDEASkillData skill = new GDEASkillData(GDEItemKeys.ASkill_normalAttack)
                {
                    Id = ups[i]
                    ,
                    Lv = 0
                };
                SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).skillsOwned.Add(skill);
                SDDataManager.Instance.getHeroByHashcode(heroDetail.Hashcode).Set_skillsOwned();
            }
        }
    }
    #endregion


    public void BtnToChangeImproveKind()
    {
        int c = (int)currentImproveKind;
        ImproveKind k = (ImproveKind)((c + 1) % (int)ImproveKind.end);
        stocksInit(k);
    }
}
