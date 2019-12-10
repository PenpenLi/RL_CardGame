using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBuffPanel : BasicSubMenuPanel
{
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
    }


}
