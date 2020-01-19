using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using DG.Tweening;
using System.Linq;
public class GoddessDetailPanel : BasicSubMenuPanel
{
    [Space(50)]
    public PageView GoddessPV;
    //public GoddessInfo[] AllGoddesses;
    //public string CurrentGoddessId;
    public GoddessInfo CurrentGoddess;
    [Space]
    public SDGoddesDetail SDGD;
    //public Transform emptyPanel;
    public Transform runePanel;
    public int currentGoddessRunePos;
    [Space]
    public Transform GD_normal_place;
    public Transform GD_improve_place;
    public Transform GD_rune_place;
    public float GD_animTime;
    [Space]
    //public SDGoddessImprove SDGI;
    public RuneDetailPanel RDP;
    public enum subPanelType
    {
        none,
        rune,
        //improve,
        detail
    }
    //[Space]
    [HideInInspector]
    public subPanelType _currentSubPanelType = subPanelType.none;
    public subPanelType currentSubPanelType
    {
        get { return _currentSubPanelType; }
        set
        {
            _currentSubPanelType = value;
        }
    }
    public override void whenOpenThisPanel()
    {
        base.whenOpenThisPanel();
        //AllGoddesses = Resources.LoadAll<GoddessInfo>("ScriptableObjects/生物列表");
        GoddessPV.maxIndex = SDDataManager.Instance.PlayerData
            .goddessOwned.Count - 1;
        refreshGoddessList();
        currentSubPanelType = subPanelType.none;
        runePanel.gameObject.SetActive(false);
        //SDGI.gameObject.SetActive(false);
        SDGD.transform.position = GD_normal_place.position;
    }
    public override void commonBackAction()
    {
        if(currentSubPanelType == subPanelType.none)
        {
            base.commonBackAction();
            SDGD.transform.position = GD_normal_place.position;
            homeScene.SubMenuClose();
            if (panelFrom != HomeScene.HomeSceneSubMenu.End)
            {
                homeScene.CurrentSubMenuType = panelFrom;
            }
        }
        else if(currentSubPanelType == subPanelType.rune)
        {
            closeRuneSetPanel();
        }
    }

    public void refreshGoddessList()
    {
        foreach (Transform s in GoddessPV.rect.content)
        {
            Image _image = s.GetComponentInChildren<Image>();
            if (s.GetSiblingIndex() == 2)
            {
                _image.transform.localScale = Vector3.one * 1.25f;
                _image.color = Color.white;
            }
            else
            {
                _image.transform.localScale = Vector3.one;
                _image.color = Color.grey;
            }
        }

        int currentIndex = GoddessPV.currentIndex;

        GDEgoddessData GD = SDDataManager.Instance.PlayerData.goddessOwned.Find
            (s => s.index == currentIndex);

        if (GD != null)
        {
            SDGD.initgoddessDetailVision(GD);
            SDGD.isEmpty = false;
        }
        else SDGD.isEmpty = true;
    }
    public void startMoveGoddessPV()
    {
        foreach (Transform s in GoddessPV.rect.content)
        {
            Image _image = s.GetComponentInChildren<Image>();
            _image.transform.localScale = Vector3.one * 1f;
            _image.color = Color.white;
        }
    }



    public void initRuneEquipAndSetPanel(int index)
    {
        currentGoddessRunePos = index;
        SDGameManager.Instance.stockUseType = SDConstants.StockUseType.work;
        ShowRuneSetPanel();
    }
    public void ShowRuneSetPanel()
    {
        if (currentSubPanelType == subPanelType.rune) return;
        currentSubPanelType = subPanelType.rune;
        //
        UIEffectManager.Instance.showAnimFadeIn(runePanel);
        HEWPageController page = runePanel.GetComponentInChildren<HEWPageController>();
        page.ItemsInit(SDConstants.ItemType.Rune);
        GoddessPV.GetComponent<CanvasGroup>().DOFade(0.5f, 0.1f);
        //
        SDGD.transform.DOMove(GD_rune_place.position, GD_animTime);
    }
    public void closeRuneSetPanel()
    {
        currentSubPanelType = subPanelType.none;
        //
        UIEffectManager.Instance.hideAnimFadeOut(runePanel);
        HEWPageController page = runePanel.GetComponentInChildren<HEWPageController>();
        page.ResetPage();
        GoddessPV.GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
        //
        SDGD.transform.DOMove(GD_normal_place.position, GD_animTime);
    }
    public void refreshAllGoddessesCondition()
    {
        GoddessInfo[] All = Resources.LoadAll<GoddessInfo>("ScriptableObjects/goddess");
        for(int i = 0; i < All.Length; i++)
        {
            SDDataManager.Instance.addGoddess(All[i]);
        }

        GoddessInfo info = All.ToList().Find(x => x.Name.Contains("莱希亚"));
        SDDataManager.Instance.IncreaseGoddessVolume(info.ID, 50);
    }



    public void Btn_to_RunePanel()
    {
        homeScene._runePanel.GetComponent<RunePanel>().ChangeCurrentRuneHashcode
            (RDP.hashcode);
        homeScene.runeBtnTapped(true);
    }
}
