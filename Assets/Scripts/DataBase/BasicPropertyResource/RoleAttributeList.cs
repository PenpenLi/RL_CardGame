using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameDataEditor;

/// <summary>
/// 角色主要战斗属性
/// </summary>
public enum AttributeData
{
    Hp = 0,//生命
    Mp = 1,//法力
    Tp = 2,//怒气
    AT = 3, //物理攻
    AD = 4, //物理防
    MT = 5, //法术攻
    MD = 6, //法术防
    Speed = 7, //速度
    Taunt = 8, //嘲讽
    Accur = 9, //精准
    Evo = 10, //闪避
    Crit = 11, //暴击
    Expect = 12, //随机期望
    End,
}
[System.Serializable]
public class RoleAttributeList
{
    #region 所有战斗属性
    /// <summary>
    /// 生命
    /// </summary>
    public int Hp { get { return AllAttributeData[(int)AttributeData.Hp]; } set { AllAttributeData[(int)AttributeData.Hp] = value; } }
    /// <summary>
    /// 法力
    /// </summary>
    public int Mp { get { return AllAttributeData[(int)AttributeData.Mp]; } set { AllAttributeData[(int)AttributeData.Mp] = value; } }
    /// <summary>
    /// 怒气
    /// </summary>
    public int Tp { get { return AllAttributeData[(int)AttributeData.Tp]; } set { AllAttributeData[(int)AttributeData.Tp] = value; } }
    /// <summary>
    /// 物理攻击
    /// </summary>
    public int AT { get { return AllAttributeData[(int)AttributeData.AT]; } set { AllAttributeData[(int)AttributeData.AT] = value; } }
    /// <summary>
    /// 物理防御
    /// </summary>
    public int AD { get { return AllAttributeData[(int)AttributeData.AD]; } set { AllAttributeData[(int)AttributeData.AD] = value; } }
    /// <summary>
    /// 魔法攻击
    /// </summary>
    public int MT { get { return AllAttributeData[(int)AttributeData.MT]; } set { AllAttributeData[(int)AttributeData.MT] = value; } }
    /// <summary>
    /// 魔法防御
    /// </summary>
    public int MD { get { return AllAttributeData[(int)AttributeData.MD]; } set { AllAttributeData[(int)AttributeData.MD] = value; } }
    /// <summary>
    /// 速度
    /// </summary>
    public int Speed { get { return AllAttributeData[(int)AttributeData.Speed]; } set { AllAttributeData[(int)AttributeData.Speed] = value; } }
    /// <summary>
    /// 嘲讽值
    /// </summary>
    public int Taunt { get { return AllAttributeData[(int)AttributeData.Taunt]; } set { AllAttributeData[(int)AttributeData.Taunt] = value; } }
    /// <summary>
    /// 精准值
    /// </summary>
    public int Accur { get { return AllAttributeData[(int)AttributeData.Accur]; } set { AllAttributeData[(int)AttributeData.Accur] = value; } }
    /// <summary>
    /// 闪避值
    /// </summary>
    public int Evo { get { return AllAttributeData[(int)AttributeData.Evo]; } set { AllAttributeData[(int)AttributeData.Evo] = value; } }
    /// <summary>
    /// 爆击值
    /// </summary>
    public int Crit { get { return AllAttributeData[(int)AttributeData.Crit]; } set { AllAttributeData[(int)AttributeData.Crit] = value; } }
    /// <summary>
    /// 结果震荡幅度
    /// </summary>
    public int Expect { get { return AllAttributeData[(int)AttributeData.Expect]; } set { AllAttributeData[(int)AttributeData.Expect] = value; } }
    //[HideInInspector]
    public int[] AllAttributeData = new int[(int)AttributeData.End];
    #endregion
    #region 所有抗性
    /// <summary>
    /// 流血抗性
    /// </summary>
    public int Bleed_Def { get { return AllResistData[(int)StateTag.Bleed]; } set { AllResistData[(int)StateTag.Bleed] = value; } }
    /// <summary>
    /// 心灵抗性
    /// </summary>
    public int Mind_Def { get { return AllResistData[(int)StateTag.Mind]; } set { AllResistData[(int)StateTag.Mind] = value; } }
    /// <summary>
    /// 灼烧抗性
    /// </summary>
    public int Fire_Def { get { return AllResistData[(int)StateTag.Fire]; } set { AllResistData[(int)StateTag.Fire] = value; } }
    /// <summary>
    /// 霜冻抗性
    /// </summary>
    public int Frost_Def { get { return AllResistData[(int)StateTag.Frost]; } set { AllResistData[(int)StateTag.Frost] = value; } }
    /// <summary>
    /// 腐蚀抗性
    /// </summary>
    public int Corrosion_Def { get { return AllResistData[(int)StateTag.Corrosion]; } set { AllResistData[(int)StateTag.Corrosion] = value; } }
    /// <summary>
    /// 禁言抗性
    /// </summary>
    public int Hush_Def { get { return AllResistData[(int)StateTag.Hush]; } set { AllResistData[(int)StateTag.Hush] = value; } }
    /// <summary>
    /// 眩晕抗性
    /// </summary>
    public int Dizzy_Def { get { return AllResistData[(int)StateTag.Dizzy]; } set { AllResistData[(int)StateTag.Dizzy] = value; } }
    /// <summary>
    /// 混乱抗性
    /// </summary>
    public int Confuse_Def { get { return AllResistData[(int)StateTag.Confuse]; } set { AllResistData[(int)StateTag.Confuse] = value; } }
    //[HideInInspector]
    public int[] AllResistData = new int[(int)StateTag.End];
    #endregion
    //
    public int BattleForce
    {
        get
        {
            int a = 0;
            for (int i = 0; i < AllAttributeData.Length; i++)
            {
                a += new OneAttritube(AllAttributeData[i], (AttributeData)i).BattleForce;
            }
            return a;
        }
    }

    #region 快速索引与运算
    public static RoleAttributeList operator +(RoleAttributeList a, RoleAttributeList b)
    {
        for (int i = 0; i < (int)AttributeData.End; i++)
        {
            a.AllAttributeData[i] += b.AllAttributeData[i];
        }
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            a.AllResistData[i] += b.AllResistData[i];
        }
        return a;
    }
    public static RoleAttributeList operator +(RoleAttributeList a, int b)
    {
        for (int i = 0; i < (int)AttributeData.End; i++)
        {
            a.AllAttributeData[i] += b;
        }
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            a.AllResistData[i] += b;
        }
        return a;
    }
    public static RoleAttributeList operator -(RoleAttributeList a, RoleAttributeList b)
    {
        for (int i = 0; i < (int)AttributeData.End; i++)
        {
            a.AllAttributeData[i] -= b.AllAttributeData[i];
        }
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            a.AllResistData[i] -= b.AllResistData[i];
        }
        return a;
    }
    public static RoleAttributeList operator -(RoleAttributeList a, int b)
    {
        for (int i = 0; i < (int)AttributeData.End; i++)
        {
            a.AllAttributeData[i] -= b;
        }
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            a.AllResistData[i] -= b;
        }
        return a;
    }

    #region ADD
    public void Add(int figure, AttributeData tag)
    {
        AllAttributeData[(int)tag] += figure;
    }
    public void Add(int figure, StateTag tag)
    {
        AllResistData[(int)tag] += figure;
    }
    public void Add(OneAttritube OA)
    {
        if (OA.Type == OneAttritube.FigureType.perc)
        {
            if (OA.isAD)
            {
                AddPerc(OA.figure, OA.ADType);
            }
            else
            {
                AddPerc(OA.figure, OA.STType);
            }
        }
        else if (OA.Type == OneAttritube.FigureType.natural)
        {
            if (OA.isAD)
            {
                Add(Mathf.Abs(OA.figure), OA.ADType);
            }
            else
            {
                Add(Mathf.Abs(OA.figure), OA.STType);
            }
        }
        else
        {
            if (OA.isAD)
            {
                Add(OA.figure, OA.ADType);
            }
            else
            {
                Add(OA.figure, OA.STType);
            }
        }
    }
    public void AddAll(int figure)
    {
        for (int i = 0; i < (int)AttributeData.End; i++) { Add(figure, (AttributeData)i); }
        for (int i = 0; i < (int)StateTag.End; i++) { Add(figure, (StateTag)i); }
    }
    public enum AddType
    {
        common
        , barChart
        , phy
        , mage
        , allA
        , resist
        , end
    }
    public void AddAll(int figure, AddType type)
    {
        switch (type)
        {
            case AddType.common:
                Add(figure, AttributeData.Hp);
                Add(figure, AttributeData.Mp);
                Add(figure, AttributeData.AT);
                Add(figure, AttributeData.AD);
                Add(figure, AttributeData.MT);
                Add(figure, AttributeData.MD);
                break;
            case AddType.barChart:
                Add(figure, AttributeData.Hp);
                Add(figure, AttributeData.Mp);
                break;
            case AddType.phy:
                Add(figure, AttributeData.AT);
                Add(figure, AttributeData.AD);
                break;
            case AddType.mage:
                Add(figure, AttributeData.MT);
                Add(figure, AttributeData.MD);
                break;
            case AddType.allA:
                for (int i = 0; i < (int)AttributeData.End; i++)
                {
                    Add(figure, (AttributeData)i);
                }
                break;
            case AddType.resist:
                for (int i = 0; i < (int)StateTag.End; i++)
                {
                    Add(figure, (StateTag)i);
                }
                break;
        }
    }
    public void AddPerc(int percent, AttributeData tag)
    {
        AllAttributeData[(int)tag] = (int)(AllAttributeData[(int)tag]
            * AllRandomSetClass.SimplePercentToDecimal(100 + percent));
    }
    public void AddPerc(int figure, StateTag tag)
    {
        AllResistData[(int)tag] = (int)(AllResistData[(int)tag]
            * AllRandomSetClass.SimplePercentToDecimal(100 + figure));
    }
    public void AddPercAll(int figure)
    {
        for (int i = 0; i < (int)AttributeData.End; i++) { AddPerc(figure, (AttributeData)i); }
        for (int i = 0; i < (int)StateTag.End; i++) { AddPerc(figure, (StateTag)i); }
    }
    public void AddPercAll(int figure, AddType type)
    {
        switch (type)
        {
            case AddType.common:
                AddPerc(figure, AttributeData.Hp);
                AddPerc(figure, AttributeData.Mp);
                AddPerc(figure, AttributeData.AT);
                AddPerc(figure, AttributeData.AD);
                AddPerc(figure, AttributeData.MT);
                AddPerc(figure, AttributeData.MD);
                break;
            case AddType.barChart:
                AddPerc(figure, AttributeData.Hp);
                AddPerc(figure, AttributeData.Mp);
                break;
            case AddType.phy:
                AddPerc(figure, AttributeData.AT);
                AddPerc(figure, AttributeData.AD);
                break;
            case AddType.mage:
                AddPerc(figure, AttributeData.MT);
                AddPerc(figure, AttributeData.MD);
                break;
            case AddType.allA:
                for (int i = 0; i < (int)AttributeData.End; i++)
                {
                    AddPerc(figure, (AttributeData)i);
                }
                break;
            case AddType.resist:
                for (int i = 0; i < (int)StateTag.End; i++)
                {
                    AddPerc(figure, (StateTag)i);
                }
                break;
        }
    }

    #endregion
    #region SET
    public void Set(int figure, AttributeData tag)
    {
        if ((int)tag >= AllAttributeData.Length) return;
        AllAttributeData[(int)tag] = figure;
    }
    public void Set(int figure, StateTag tag)
    {
        if ((int)tag >= AllResistData.Length) return;
        AllResistData[(int)tag] = figure;
    }
    #endregion
    public int read(AttributeData tag, int rate = 100)
    {
        int t = (int)tag;
        if (t < AllAttributeData.Length)
            return (int)(AllAttributeData[(int)tag]
                * AllRandomSetClass.SimplePercentToDecimal(rate));
        else return 0;
    }
    public int read(StateTag tag, int rate = 100)
    {
        int t = (int)tag;
        if (t < AllResistData.Length)
            return (int)(AllResistData[(int)tag]
                * AllRandomSetClass.SimplePercentToDecimal(rate));
        else return 0;
    }
    public void AddGDEData(GDERoleAttritubeData roledata)
    {
        for (int i = 0; i < (int)AttributeData.End; i++)
        {
            if (i < roledata.AD_List.Count)
            {
                Add(roledata.AD_List[i], (AttributeData)i);
            }
        }
        for (int i = 0; i < (int)StateTag.End; i++)
        {
            if (i < roledata.RD_List.Count)
            {
                Add(roledata.RD_List[i], (StateTag)i);
            }
        }
    }
    public void AffectedByRate(RoleAttributeList rate)
    {
        for (int i = 0; i < AllAttributeData.Length; i++)
        {
            if (rate.AllAttributeData[i] != 0)
            {
                float DECIMAL = (rate.AllAttributeData[i] + 100) * 1f / 100;
                AllAttributeData[i] = (int)(AllAttributeData[i] * DECIMAL);
            }
        }
        for (int i = 0; i < AllResistData.Length; i++)
        {
            if (rate.AllResistData[i] != 0)
            {
                float DECIMAL = (rate.AllResistData[i] + 100) * 1f / 100;
                AllResistData[i] = (int)(AllResistData[i] * DECIMAL);
            }
        }
    }
    public GDERoleAttritubeData TurnIntoGDEData
    {
        get
        {
            GDERoleAttritubeData data = new GDERoleAttritubeData(GDEItemKeys.RoleAttritube_BasicRA);
            for (int i = 0; i < AllAttributeData.Length; i++)
            {
                if (i < data.AD_List.Count) { data.AD_List[i] = AllAttributeData[i]; }
                else data.AD_List.Add(AllAttributeData[i]);
            }
            for (int i = 0; i < AllResistData.Length; i++)
            {
                if (i < data.RD_List.Count) { data.RD_List[i] = AllResistData[i]; }
                else data.RD_List.Add(AllResistData[i]);
            }
            return data;
        }
    }
    public static RoleAttributeList GDEToRAL(GDERoleAttritubeData data)
    {
        RoleAttributeList RAL = RoleAttributeList.zero;
        for(int i = 0; i < data.AD_List.Count; i++)
        {
            RAL.AllAttributeData[i] = data.AD_List[i];
        }
        for(int i = 0; i < data.RD_List.Count; i++)
        {
            RAL.AllResistData[i] = data.RD_List[i];
        }
        return RAL;
    }

    public static RoleAttributeList zero = new RoleAttributeList()
    {
        Hp = 0,
        Mp = 0,
        Tp = 0,
        AT = 0,
        AD = 0,
        MT = 0,
        MD = 0,
        Speed = 0,
        Taunt = 0,
        Accur = 0,
        Evo = 0,
        Crit = 0,
        Expect = 0,
        Bleed_Def = 0,
        Mind_Def = 0,
        Fire_Def = 0,
        Frost_Def = 0,
        Corrosion_Def = 0,
        Hush_Def = 0,
        Dizzy_Def = 0,
        Confuse_Def = 0
    };
    public bool HaveData
    {
        get
        {
            foreach (int d in AllAttributeData) { if (d != 0) return true; }
            foreach (int d in AllResistData) { if (d != 0) return true; }
            return false;
        }
    }
    public static RoleAttributeList RandomSet(ScopeInt barchartAd
        ,ScopeInt atkdefAd,ScopeInt extraAd,ScopeInt Rd)
    {
        RoleAttributeList L = RoleAttributeList.zero;
        L.Hp = UnityEngine.Random.Range(barchartAd.Min, barchartAd.Max);
        L.Mp = UnityEngine.Random.Range(barchartAd.Min, barchartAd.Max);
        L.Tp = UnityEngine.Random.Range(barchartAd.Min, barchartAd.Max);
        //
        L.AT = UnityEngine.Random.Range(atkdefAd.Min, atkdefAd.Max);
        L.AD = UnityEngine.Random.Range(atkdefAd.Min, atkdefAd.Max);
        L.MT = UnityEngine.Random.Range(atkdefAd.Min, atkdefAd.Max);
        L.MD = UnityEngine.Random.Range(atkdefAd.Min, atkdefAd.Max);
        //
        L.Speed = UnityEngine.Random.Range(extraAd.Min, extraAd.Max);
        L.Taunt = UnityEngine.Random.Range(extraAd.Min, extraAd.Max);
        L.Accur = UnityEngine.Random.Range(extraAd.Min, extraAd.Max);
        L.Evo = UnityEngine.Random.Range(extraAd.Min, extraAd.Max);
        L.Crit = UnityEngine.Random.Range(extraAd.Min, extraAd.Max);
        L.Expect = UnityEngine.Random.Range(extraAd.Min, extraAd.Max);
        //
        for(int i = 0; i < L.AllResistData.Length; i++)
        {
            L.AllResistData[i] = UnityEngine.Random.Range(Rd.Min, Rd.Max);
        }
        return L;
    }
    public RoleAttributeList Clone
    {
        get
        {
            RoleAttributeList ral = new RoleAttributeList();
            for (int i = 0; i < ral.AllAttributeData.Length; i++)
            {
                ral.AllAttributeData[i] = this.AllAttributeData[i];
            }
            for (int i = 0; i < ral.AllResistData.Length; i++)
            {
                ral.AllResistData[i] = this.AllResistData[i];
            }
            return ral;
        }
    }
    #endregion
}
[System.Serializable]
public class OneAttritube
{
    public int index;
    /// <summary>
    /// true:AttritubeData;false:ResistData
    /// </summary>
    public bool isAD;
    public AttributeData ADType
    {
        get
        {
            if (isAD) return (AttributeData)index;
            else return AttributeData.End;
        }
    }
    public StateTag STType
    {
        get
        {
            if (!isAD) return (StateTag)index;
            else return StateTag.End;
        }
    }
    public enum FigureType
    {
        basic = 0
            ,perc = 1
            ,natural = 2
    }
    public FigureType Type;
    public int figure;
    #region 构建
    /// <summary>
    /// empty内容
    /// </summary>
    public OneAttritube()
    {
        figure = index = 0;Type = FigureType.basic;isAD = true;
    }
    public OneAttritube(int figure, int index,bool isAD = true
        , FigureType figureType = FigureType.basic)
    {
        this.figure = figure;this.index = index;this.isAD = isAD;
        this.Type = figureType;
    }
    public OneAttritube(int figure, int index, bool isAD = true
        , int figureType = 0)
    {
        this.figure = figure; this.index = index; this.isAD = isAD;
        this.Type = (FigureType)figureType;
    }
    public OneAttritube(int figure, AttributeData ad
        , FigureType figureType = FigureType.basic)
    {
        this.figure = figure; this.index = (int)ad;
        this.isAD = true;
        this.Type = figureType;
    }
    public OneAttritube(int figure, AttributeData ad, int figureType)
    {
        this.figure = figure;this.index = (int)ad;
        this.isAD = true;
        this.Type = (FigureType)figureType;
    }
    public OneAttritube(int figure, StateTag ad
    , FigureType figureType = FigureType.basic)
    {
        this.figure = figure; this.index = (int)ad;
        this.isAD = false;
        this.Type = figureType;
    }
    public OneAttritube(int figure, StateTag ad, int figureType = 0)
    {
        this.figure = figure; this.index = (int)ad;
        this.isAD = false;
        this.Type = (FigureType)figureType;
    }
    #endregion
    public static OneAttritube ReadEffectString(string effect)
    {
        string[] list = effect.Split(':');
        if (list.Length > 1)
        {
            string name = list[0];
            if (name == "" || name == null) name = "hp";

            int f = 0;
            FigureType _type = FigureType.basic;
            if (list[1].Contains("%"))
            {
                string l = list[1].Split('%')[0];
                f = SDDataManager.Instance.getInteger(l);
                _type = FigureType.perc;
            }
            else if (list[1].Contains("u"))
            {
                string l = list[1].Split('u')[0];
                f = SDDataManager.Instance.getInteger(l);
                _type = FigureType.natural;
            }
            else
            {
                f = SDDataManager.Instance.getInteger(list[1]);
            }
            
            if(name == "hp")
            {
                return new OneAttritube(f, AttributeData.Hp, _type);
            }
            else if(name == "mp")
            {
                return new OneAttritube(f, AttributeData.Mp, _type);
            }
            else if(name == "tp")
            {
                return new OneAttritube(f, AttributeData.Tp, _type);
            }
            else if(name == "at")
            {
                return new OneAttritube(f, AttributeData.AT, _type);
            }
            else if (name == "ad")
            {
                return new OneAttritube(f, AttributeData.AD, _type);
            }
            else if (name == "mt")
            {
                return new OneAttritube(f, AttributeData.MT, _type);
            }
            else if (name == "md")
            {
                return new OneAttritube(f, AttributeData.MD, _type);
            }
            else if (name == "speed")
            {
                return new OneAttritube(f, AttributeData.Speed, _type);
            }
            else if (name == "taunt")
            {
                return new OneAttritube(f, AttributeData.Taunt, _type);
            }
            else if (name == "accur")
            {
                return new OneAttritube(f, AttributeData.Accur, _type);
            }
            else if (name == "evo")
            {
                return new OneAttritube(f, AttributeData.Evo, _type);
            }
            else if (name == "crit")
            {
                return new OneAttritube(f, AttributeData.Crit, _type);
            }
            else if (name == "expect")
            {
                return new OneAttritube(f, AttributeData.Expect, _type);
            }
            //抗性
            else if (name == "bleed")
            {
                return new OneAttritube(f, StateTag.Bleed, _type);
            }
            else if (name == "mind")
            {
                return new OneAttritube(f, StateTag.Mind, _type);
            }
            else if (name == "fire")
            {
                return new OneAttritube(f, StateTag.Fire, _type);
            }
            else if (name == "frost")
            {
                return new OneAttritube(f, StateTag.Frost, _type);
            }
            else if (name == "corrosion")
            {
                return new OneAttritube(f, StateTag.Corrosion, _type);
            }
            else if (name == "hush")
            {
                return new OneAttritube(f, StateTag.Hush, _type);
            }
            else if (name == "dizzy")
            {
                return new OneAttritube(f, StateTag.Dizzy, _type);
            }
            else if(name == "confuse")
            {
                return new OneAttritube(f, StateTag.Confuse, _type);
            }
            else { return null; }
        }
        else return null;
    }

    public int BattleForce
    {
        get
        {
            if (isAD)
            {
                if (ADType == AttributeData.Hp || ADType == AttributeData.Mp
                    || ADType == AttributeData.Tp)
                {
                    return figure / 3;
                }
                else if (ADType == AttributeData.AD || ADType == AttributeData.AT
                   || ADType == AttributeData.MD || ADType == AttributeData.MT)
                {
                    return figure;
                }
                else
                {
                    return figure * 2;
                }
            }
            else
            {
                return figure / 10;
            }
        }

    }
}