using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class RTSingleSkillItem : MonoBehaviour
{
    public Image itemImg;
    public Text DownText;
    public Image itemFrameImg;
    public string ItemId;
    public int lv;
    public bool isOmega;
    public Transform SelectedPanel;
    public bool isSelected
    {
        get 
        { 
            if (SelectedPanel && SelectedPanel.gameObject.activeSelf) return true;
            return false;
        }
        set { SelectedPanel.gameObject.SetActive(value); }
    }

    public SDConstants.deployType deploy = SDConstants.deployType.skill;
    public int use_type;
    public void BtnTapped()
    {
        if(deploy == SDConstants.deployType.skill)
        {
            chooseSkillToShowDetail();
        }
    }
    public void chooseSkillToShowDetail()
    {
        SDSkillSelect SSS = GetComponentInParent<SDSkillSelect>();
        if (SSS)
        {
            SSS.refreshAllSkillsCondition(ItemId);
        }
    }
    public void initSkillItem(OneSkill baseskilldata , int heroHashcode = 0 )
    {
        deploy = SDConstants.deployType.skill;
        ItemId = baseskilldata.skillId;
        lv = baseskilldata.lv;
        DownText.text = SDGameManager.T("Lv.") + lv;
        if (heroHashcode > 0)
        {
            OneSkill S = SDDataManager.Instance.getOwnedSkillById
                (ItemId, heroHashcode);
            isOmega = S.isOmegaSkill;
            if (isOmega)
            {
                itemFrameImg.sprite = SDDataManager.Instance
                    .baseFrameSpriteByRarity(3);
            }
            else
            {
                itemFrameImg.sprite = SDDataManager.Instance
                    .baseFrameSpriteByRarity(1);
            }
        }
    }
}
