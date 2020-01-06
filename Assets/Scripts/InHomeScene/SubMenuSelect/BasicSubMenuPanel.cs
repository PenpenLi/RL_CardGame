using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicSubMenuPanel : MonoBehaviour
{
    HomeScene _hs;
    public HomeScene homeScene
    {
        get
        {
            if (_hs == null) 
                _hs = transform.parent.GetComponentInChildren<HomeScene>();
            return _hs;
        }

    }
    
    /// <summary>
    /// 建筑Id
    /// </summary>
    public string buildingId;
    /// <summary>
    /// 建筑是否存在NPC且可以升级
    /// </summary>
    public bool LvUpEnable;
    [ConditionalHide("LvUpEnable", true)]
    /// <summary>
    /// 建筑经验
    /// </summary>
    //public int exp;
    public int Level;
    [Space, Header("建筑内NPC设置")]
    [SerializeField]
    private bool showNPC;
    public bool ShowNPC { get { return showNPC; } }
    [ConditionalHide("showNPC", true),SerializeField]
    private Talker representNPC;
    public Talker RepresentNPC
    {
        get { return representNPC; }
    }

    public List<Talker> NPC_s = new List<Talker>();
    [Space(10)]
    /// <summary>
    /// 菜单是否开启
    /// </summary>
    public bool thisMenuOpened;
    public HomeScene.HomeSceneSubMenu panelFrom = HomeScene.HomeSceneSubMenu.End;
    public Canvas _canvas
    {
        get { return GetComponent<Canvas>(); }
    }
    public virtual void whenOpenThisPanel()
    {
        thisMenuOpened = true;
        if (LvUpEnable)
        {
            homeScene.SubMenuLvUpBtn.gameObject.SetActive(true);
            if (CheckIfCanLvUp()) homeScene.SubMenuLvUpBtn.interactable = true;
            else homeScene.SubMenuLvUpBtn.interactable = false;
        }
        else
        {
            homeScene.SubMenuLvUpBtn.gameObject.SetActive(false);
        }
        //
        if (ShowNPC)
        {
            if(RepresentNPC)
                RepresentNPC.GetComponent<NPCController>().Init();
        }
    }
    public virtual void commonBackAction()
    {
        thisMenuOpened = false;
    }



    #region NPC&LvUp_Set
    public bool CheckIfCanLvUp()
    {
        if (!LvUpEnable) return false;
        int lv = Level;
        int Exp0 = SDDataManager.Instance.ExpBulkPerLevel(Level+1);
        if (SDDataManager.Instance.PlayerData.JianCai * SDConstants.JianCaiConsumeWithGetOneExp
            >= Exp0)
        {
            return true;
        }
        return false;
    }
    public void BtnToLvUp()
    {
        if (!LvUpEnable) return;
        int lv = Level;
        int Exp0 = SDDataManager.Instance.ExpBulkPerLevel(Level + 1);
        //
        if(SDDataManager.Instance.PlayerData.JianCai*SDConstants.JianCaiConsumeWithGetOneExp
            >= Exp0)
        {
            //可以升级（明确可以上升一级）
            int jiancaiNum = 0;
            while(jiancaiNum * SDConstants.JianCaiConsumeWithGetOneExp
                >= Exp0)
            {
                jiancaiNum++;
            }
            SDDataManager.Instance.ConsumeJiancai(jiancaiNum);
            SDDataManager.Instance.LvUpBuilding(buildingId);
            homeScene.refreshAllBuildingCondition();
        }
        else
        {
            Debug.Log("无法进行升级 材料不足");
        }

    }
    #endregion
}
