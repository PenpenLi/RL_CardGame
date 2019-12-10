using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoddessPanel : BasicSubMenuPanel
{
    public HEWPageController pageController;

    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        //pageController.ResetPage();
        pageController.showGoddessOwned();
    }
    public void WhenCloseThisPanel()
    {
        pageController.ResetPage();
    }


    public override void commonBackAction()
    {
        base.commonBackAction();
        WhenCloseThisPanel();
        homeScene.SubMenuClose();
    }
}
