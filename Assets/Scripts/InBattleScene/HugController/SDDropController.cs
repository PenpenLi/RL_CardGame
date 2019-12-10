using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;

public class SDDropController : MonoBehaviour
{
    public List<string> allProbablyDropIds_normal = new List<string>();
    public List<string> allProbablyDropIds_rare = new List<string>();
    public List<string> allProbablyDropIds_legend = new List<string>();
    #region 概率
    public float normalRate;
    public float _normalRate
    {
        get 
        { 
            if (allProbablyDropIds_normal.Count > 0) return normalRate;
            else return 0;
        }
    }
    public float rareRate;
    public float _rareRate
    {
        get
        {
            if (allProbablyDropIds_rare.Count > 0) return rareRate;
            else return 0;
        }
    }
    public float legendRate;
    public float _legendRate
    {
        get
        {
            if (allProbablyDropIds_legend.Count > 0) return legendRate;
            else return 0;
        }
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
        normalRate = rareRate = legendRate = 0;
    }
    public void addDrop_normal(List<string> list, float rate)
    {
        for(int i = 0; i < list.Count; i++)
        {
            allProbablyDropIds_normal.Add(list[i]);
        }
        normalRate = rate;
    }
    public void addDrop_rare(List<string> list, float rate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            allProbablyDropIds_rare.Add(list[i]);
        }
        rareRate = rate;
    }
    public void addDrop_legend(List<string> list, float rate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            allProbablyDropIds_legend.Add(list[i]);
        }
        legendRate = rate;
    }

    public void initDropController(bool isBoss = false)
    {
        clearAlldropsRate();
        int currLv = SDGameManager.Instance.currentLevel;
        List<Dictionary<string, string>> allMs = SDDataManager.Instance.ReadFromCSV("material");
        List<Dictionary<string, string>> allPs = SDDataManager.Instance.ReadFromCSV("prop");
        List<Dictionary<string, string>> allEs = SDDataManager.Instance.ReadFromCSV("equip");
        List<Dictionary<string, string>> allWs = SDDataManager.Instance.ReadFromCSV("weapon");
        if(SDGameManager.Instance.gameType == SDConstants.GameType.Normal)
        {
            for(int i = 0; i < allMs.Count; i++)
            {
                int rarity = SDDataManager.Instance.getInteger(allMs[i]["rarity"]);
                string id = (allMs[i]["id"]);
                if (rarity == 0)
                {
                    allProbablyDropIds_normal.Add(id);
                }
                else if(rarity == 1)
                {
                    allProbablyDropIds_rare.Add(id);
                }
                else if(rarity == 2)
                {
                    allProbablyDropIds_legend.Add(id);
                }
            }
            for (int i = 0; i < allPs.Count; i++)
            {
                int rarity = SDDataManager.Instance.getInteger(allPs[i]["rarity"]);
                string id = (allPs[i]["id"]);
                if (rarity == 0)
                {
                    allProbablyDropIds_normal.Add(id);
                }
                else if (rarity == 1)
                {
                    allProbablyDropIds_rare.Add(id);
                }
                else if (rarity == 2)
                {
                    allProbablyDropIds_legend.Add(id);
                }
            }
            for (int i = 0; i < allEs.Count; i++)
            {
                int rarity = SDDataManager.Instance.getInteger(allEs[i]["rarity"]);
                string id = (allEs[i]["id"]);
                if (rarity == 0)
                {
                    allProbablyDropIds_normal.Add(id);
                }
                else if (rarity == 1)
                {
                    allProbablyDropIds_rare.Add(id);
                }
                else if (rarity == 2)
                {
                    allProbablyDropIds_legend.Add(id);
                }
            }
            for (int i = 0; i < allWs.Count; i++)
            {
                int rarity = SDDataManager.Instance.getInteger(allWs[i]["rarity"]);
                string id = (allWs[i]["id"]);
                if (rarity == 0)
                {
                    allProbablyDropIds_normal.Add(id);
                }
                else if (rarity == 1)
                {
                    allProbablyDropIds_rare.Add(id);
                }
                else if (rarity == 2)
                {
                    allProbablyDropIds_legend.Add(id);
                }
            }
            normalRate = 1f;
            rareRate = 0.25f;
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
        float[] all = new float[] { _normalRate, _rareRate, _legendRate };
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
        else if (final == 2)//legend
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
