using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;

public class IllustratePanel : BasicSubMenuPanel
{
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        InitEnemiesIllustrateBook();
    }
    public override void commonBackAction()
    {
        base.commonBackAction();
        homeScene.SubMenuClose();
    }

    [Space]
    public HEWPageController PAGE;
    //public Transform 

    public void InitEnemiesIllustrateBook()
    {
        List<GDEItemData> AllEnemies = SDDataManager.Instance.GetAllEnemiesPlayerSaw;
        PAGE.ItemsInit(SDConstants.ItemType.Enemy);
        initEmptyIllustrate();
    }
    public void initCurrentEnemyIllustarte(string id)
    {

    }
    void initEmptyIllustrate()
    {

    }
}
