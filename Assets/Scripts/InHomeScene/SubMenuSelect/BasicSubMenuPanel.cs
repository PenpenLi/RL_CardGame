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
    [ConditionalHide("LvUpEnable",true)]
    /// <summary>
    /// 建筑经验
    /// </summary>
    public int exp;
    public int Level
    {
        get
        {
            return SDDataManager.Instance.getLevelByExp(exp);
        }
    }
    [Space, Header("建筑内NPC设置")]
    [SerializeField]
    private bool showNPC;
    public bool ShowNPC { get { return showNPC; } }
    [ConditionalHide("showNPC", true),SerializeField]
    private string representNPCId;
    public string RepresentNPCId
    {
        get { return representNPCId; }
    }

    public List<Talker> NPC_s = new List<Talker>();
    [Space(10)]
    /// <summary>
    /// 菜单是否开启
    /// </summary>
    public bool thisMenuOpened;
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
        int Exp0 = SDDataManager.Instance.getExpByLevel(lv);
        int _exp = exp - Exp0;
        if (SDDataManager.Instance.PlayerData.JianCai * SDConstants.JianCaiConsumeWithGetOneExp
            >= SDDataManager.Instance.getExpLengthByLevel(lv) - _exp)
        {
            return true;
        }
        return false;
    }
    public void BtnToLvUp()
    {
        if (!LvUpEnable) return;
        int lv = Level;
        int Exp0 = SDDataManager.Instance.getExpByLevel(lv);
        int _exp = exp - Exp0;
        if(SDDataManager.Instance.PlayerData.JianCai*SDConstants.JianCaiConsumeWithGetOneExp
            >= SDDataManager.Instance.getExpLengthByLevel(lv) - _exp)
        {
            //可以升级（明确可以上升一级）
            int jiancaiNum = 0;
            while(jiancaiNum * SDConstants.JianCaiConsumeWithGetOneExp
                >= SDDataManager.Instance.getExpLengthByLevel(lv) - _exp)
            {
                jiancaiNum++;
            }
            SDDataManager.Instance.PlayerData.JianCai -= jiancaiNum;
            SDDataManager.Instance.AddExpToBuilding
                (buildingId, jiancaiNum * SDConstants.JianCaiConsumeWithGetOneExp);
            homeScene.refreshAllBuildingCondition();
        }
        else
        {
            Debug.Log("无法进行升级 材料不足");
        }

    }
    #endregion
}
