using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using System.Linq;
using DG.Tweening;

public class OuterMenuController : MonoBehaviour
{
    public HomeScene homeScene;
    public Transform basicOuterMenuPlace;
    [Header("实时变化类")]
    public Text coin_num_text;
    public Text damond_num_text;
    public Text jiancai_num_text;
    public Image dayNightChangeImg;
    public Sprite[] ImgForChanging = new Sprite[2];
    [Header("修改后变化类")]
    public Image goddessIcon;
    public Text player_name_text;
    [Header("设置页面")]
    public Transform SettingPanel;
    public Transform SettingMenu;

    public void ReadAllDataFromGDE()
    {
        SDDataManager dm = SDDataManager.Instance;
        //
        refreshNumbers();
        //
        bool teamHaveGoddess = false;
        if (dm.PlayerData.heroesTeam != null
            && dm.PlayerData.heroesTeam.Count > 0)
        {
            GDEunitTeamData team = dm.PlayerData.heroesTeam[0];
            if (!string.IsNullOrEmpty(team.goddess))
            {
                teamHaveGoddess = true;
                GoddessInfo info = dm.getGoddessInfoById(team.goddess);
                //goddessIcon.sprite = 
                player_name_text.text = info.Name;
            }
        }
        if (!teamHaveGoddess)
        {
            List<GDEgoddessData> gs = dm.getAllGoddessesUnLocked();
            if (gs.Count > 0)
            {
                GoddessInfo info = dm.getGoddessInfoById(gs[0].id);
                //goddessIcon.sprite = 
                player_name_text.text = info.Name;
            }
        }
        //
        //player_name_text = dm.PlayerData
        //
    }
    public void refreshNumbers()
    {
        SDDataManager dm = SDDataManager.Instance;
        coin_num_text.text = "" + dm.GetCoin();
        damond_num_text.text = "" + dm.GetDamond();
        jiancai_num_text.text = "" + dm.getJiancai();
        //
        int currentDayNightId = dm.ResidentMovementData.CurrentDayNightId;
        dayNightChangeImg.sprite = ImgForChanging[currentDayNightId];
    }
    public void btnTapped_coin()
    {
        if(homeScene.CurrentSubMenuType != HomeScene.HomeSceneSubMenu.Store)
        {
            bool flag = homeScene.CurrentSubMenuType != HomeScene.HomeSceneSubMenu.End;
            homeScene.storeBtnTapped(flag);
        }
    }
    public void btnTapped_damond()
    {
        if (homeScene.CurrentSubMenuType != HomeScene.HomeSceneSubMenu.Store)
        {
            bool flag = homeScene.CurrentSubMenuType != HomeScene.HomeSceneSubMenu.End;
            homeScene.storeBtnTapped(flag);
        }
    }
    #region SettingPanel
    public void OpenSettingPanel()
    {
        UIEffectManager.Instance.showAnimFadeIn_withoutScale(SettingPanel);
        //UIEffectManager.Instance.showAnimFadeIn(SettingMenu);
        SettingMenu.localScale = Vector3.zero;
        SettingMenu.DOScale(Vector3.one, 0.1f);
        UIEffectManager.Instance.hideAnimFadeOut_withoutScale(basicOuterMenuPlace);
        //
    }
    public void CloseSettingPanel()
    {
        UIEffectManager.Instance.hideAnimFadeOut_withoutScale(SettingPanel);
        UIEffectManager.Instance.showAnimFadeIn_withoutScale(basicOuterMenuPlace);
        //
    }
    #endregion
}
