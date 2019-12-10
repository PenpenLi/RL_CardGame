using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 通用英雄选择类
/// </summary>
public class BasicHeroSelect : MonoBehaviour
{
    public HEWPageController pageController;
    public int index;
    public int currentPageIndex;
    #region Panel列表
    [Header("需要使用的Panel")]
    public SDHeroSelect heroSelectPanel;
    public RTBagPanel bagPanel;
    public AllOwnedHeroesPanel allRolePanel;
    #endregion
    [Space(25)]
    public ScrollRect scrollRect;
    public Transform heroDetails;
    public Transform emptyHeroPanel;

    public void heroesInit()
    {
        pageController.pageIndex = 0;
        pageController.ItemsInit(SDConstants.ItemType.Hero);
        showEmptyHeroDetail();
        if (pageController.itemCount == 0) emptyHeroPanel.gameObject.SetActive(true);
        else emptyHeroPanel.gameObject.SetActive(false);
    }
    public void closeBtnTapped()
    {
        SDGameManager.Instance.isSelectHero = false;

    }
    public void showEmptyHeroDetail()
    {
        heroDetails?.gameObject.SetActive(false);
    }
}
