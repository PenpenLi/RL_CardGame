using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
public class SkillSlot : MonoBehaviour
{
    public enum skillSlotType
    {
        skill0,skill1,omegaSkill,
    }
    //
    [Space(10)]
    public Transform emptyPanel;
    public bool isEmpty
    {
        get 
        {
            if (emptyPanel && emptyPanel.gameObject.activeSelf) return true;
            return false;
        }
    }
    [Space(10)]
    public Image skillIcon;
    public Image skillBgIcon;
    public Image skillItemImg;
    public Text slotAboveText;
    [Space(10)]
    public skillSlotType slotType;
    public HeroDetailPanel HDP;
    [Space(25)]
    public int lv;
    public string id;
    public bool isOmega;
    public void initSkillSlot(int HeroHashcode)
    {
        bool flag = false;
        if (!SDDataManager.Instance.checkHeroEnableSkill1ByHashcode(HeroHashcode))
        {
            if (slotType == skillSlotType.skill1) flag = true;
        }
        if (!flag)
        {
            gameObject.SetActive(true);
            GDEHeroData hero = SDDataManager.Instance.getHeroByHashcode(HeroHashcode);
            string skillId = string.Empty;
            if (slotType == skillSlotType.skill0)
            {
                skillId = hero.skill0Id;
            }
            else if (slotType == skillSlotType.skill1)
            {
                skillId = hero.skill1Id;
            }
            else if (slotType == skillSlotType.omegaSkill)
            {
                skillId = hero.skillOmegaId;
            }
            OneSkill skill = SDDataManager.Instance.getOwnedSkillById(skillId, HeroHashcode);
            initOneSkillSlot(skill);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    public void initOneSkillSlot(OneSkill skill)
    {
        if(skill == null)
        {
            emptyPanel.gameObject.SetActive(true);
            return;
        }
        if(skill.lv < 0 ||string.IsNullOrEmpty(skill.skillId))
        {
            emptyPanel.gameObject.SetActive(true);
        }
        else
        {
            emptyPanel.gameObject.SetActive(false);
            lv = skill.lv;
            slotAboveText.text = SDGameManager.T("Lv.") + lv;
            Transform skillBtn;;
            if (skill.UseAppointedPrefab)
            {
                skillBtn = skill.SkillPrefab;
            }
            else
            {
                skillBtn = HDP.skillDetailList.AllSkillList[skill.SkillFunctionID];
            } 
            skillItemImg.color = skillBtn.GetComponent<Image>().color;
        }
        id = skill.skillId;
        isOmega = skill.isOmegaSkill;
        if (isOmega)
        {
            skillBgIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(3);
        }
        else
        {
            skillBgIcon.sprite = SDDataManager.Instance.baseFrameSpriteByRarity(1);
        }
    }
}
