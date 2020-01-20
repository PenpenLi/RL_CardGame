using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDDropController : MonoBehaviour
{
    public List<string> allProbablyDropIds_normal = new List<string>();
    public List<string> allProbablyDropIds_rare = new List<string>();
    public List<string> allProbablyDropIds_epic = new List<string>();
    public List<string> allProbablyDropIds_legend = new List<string>();
    #region 概率

#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private float _normalRate;
    public float normalRate
    {
        get 
        { 
            if (allProbablyDropIds_normal.Count > 0) return _normalRate;
            else return 0;
        }
        set { _normalRate = value; }
    }

#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private float _rareRate;
    public float rareRate
    {
        get
        {
            if (allProbablyDropIds_rare.Count > 0) return _rareRate;
            else return 0;
        }
        set { _rareRate = value; }
    }

#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private float _epicRate;
    public float epicRate
    {
        get
        {
            if (allProbablyDropIds_epic.Count > 0) return _epicRate;
            else return 0;
        }
        set { _epicRate = value; }
    }

#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private float _legendRate;
    public float legendRate
    {
        get
        {
            if (allProbablyDropIds_legend.Count > 0) return _legendRate;
            else return 0;
        }
        set { _legendRate = value; }
    }

    #endregion
    [HideInInspector]
    public GameController GC;
    #region addDrops
    public void clearAlldropsRate()
    {
        allProbablyDropIds_normal.Clear();
        allProbablyDropIds_rare.Clear();
        allProbablyDropIds_legend.Clear();
        normalRate = rareRate = epicRate = legendRate = 0;
    }

    public void initDropController(bool isBoss = false)
    {
        clearAlldropsRate();
        int currLv = SDGameManager.Instance.currentLevel;
        List<consumableItem> allCs = SDDataManager.Instance.AllConsumableList;
        List<EquipItem> allEs = SDDataManager.Instance.AllEquipList;

        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            foreach(consumableItem c in allCs)
            {
                if (c.LEVEL == 0) allProbablyDropIds_normal.Add(c.ID);
                else if (c.LEVEL == 1) allProbablyDropIds_rare.Add(c.ID);
                else if (c.LEVEL == 2) allProbablyDropIds_epic.Add(c.ID);
                else if (c.LEVEL == 3) allProbablyDropIds_legend.Add(c.ID);
            }
            foreach(EquipItem c in allEs)
            {
                if (c.LEVEL == 0) allProbablyDropIds_normal.Add(c.ID);
                else if (c.LEVEL == 1) allProbablyDropIds_rare.Add(c.ID);
                else if (c.LEVEL == 2) allProbablyDropIds_epic.Add(c.ID);
                else if (c.LEVEL == 3) allProbablyDropIds_legend.Add(c.ID);
            }

            normalRate = 1f;
            rareRate = 0.5f;
            epicRate = 0.25f;
            legendRate = 0.05f;
        }
    }

    #endregion
    private void Start()
    {
        GC = GetComponentInParent<GameController>();
        initDropController();
    }
    public GDEItemData oneDropReward()
    {
        float[] all = new float[] { normalRate, rareRate, epicRate, legendRate };
        int final = RandomIntger.StandardReturn(all);
        GDEItemData drop = new GDEItemData(GDEItemKeys.Item_MaterialEmpty);
        List<string> list = allProbablyDropIds_normal;
        if (final == 0)//normal
        {
            list = allProbablyDropIds_normal;

        }
        else if (final == 1)//rare
        {
            list = allProbablyDropIds_rare;
        }
        else if(final == 2)//epic
        {
            list = allProbablyDropIds_epic;
        }
        else if (final == 3)//legend
        {
            list = allProbablyDropIds_legend;
        }
        int flag = UnityEngine.Random.Range(0, list.Count);
        drop.id = list[flag];
        //int stockKind = drop.id % 1000000;
        int weight = SDDataManager.Instance.getMaterialWeightById(list[flag]);
        if (weight <= 1)
        {
            drop.num = 1;
        }
        else
        {
            drop.num = UnityEngine.Random.Range(1, weight + 1);
        }
        return drop;
    }
}
