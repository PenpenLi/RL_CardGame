using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDHeroDeploySkills : MonoBehaviour
{
    public SDHeroDetail heroDetail;
    public Transform ModelAndSkilllistPanel;
    public void showRoleModelPanel()
    {
        heroDetail.ModelAndEquipsPanel.SetParent(transform);
        heroDetail.ModelAndEquipsPanel.localScale = Vector3.one;
        heroDetail.ModelAndEquipsPanel.gameObject.SetActive(true);
    }
}
