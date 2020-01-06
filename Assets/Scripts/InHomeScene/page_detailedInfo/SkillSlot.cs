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
    public Image data0Icon;
    public Text data0;
    public Image data1Icon;
    public Text data1;
    public Image data2Icon;
    public Text data2;
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
    public Transform lockedPanel;
    public bool isLocked
    {
        get 
        { 
            if (lockedPanel && lockedPanel.gameObject.activeSelf) return true;
            return false;
        }
    }
    [Space(10)]
    public Image skillIcon;
    public Image skillBgIcon;
    public Image skillItemImg;
    [Space(10)]
    public skillSlotType slotType;
    public HeroDetailPanel HDP;
    [Space(25)]
    public int lv;
    public void initSkillSlot(int HeroHashcode)
    {
        bool flag = false;
        if (!SDDataManager.Instance.checkHeroEnableSkill1ByHashcode(HeroHashcode))
        {
            if (slotType == skillSlotType.skill1) flag = true;
        }
        if (!flag)
        {
            lockedPanel.gameObject.SetActive(false);
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
            lockedPanel.gameObject.SetActive(true);
        }

    }

    public void initOneSkillSlot(OneSkill skill)
    {
        if(skill.skillId == "@SH_NOR#000" || skill.lv < 0 
            || skill== null||string.IsNullOrEmpty(skill.skillId))
        {
            emptyPanel.gameObject.SetActive(true);
        }
        else
        {
            emptyPanel.gameObject.SetActive(false);
            lv = skill.lv;
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

    }
}
