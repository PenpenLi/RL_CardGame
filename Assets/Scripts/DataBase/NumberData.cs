using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NumberData
{
    public enum DataType
    {
        integer, percent,
    }
#if UNITY_EDITOR
    [EnumMemberNames("整数", "百分数整数部分")]
#endif
    public DataType dataTag;
    public int DATA;
    public NumberData(int data, DataType tag = DataType.integer)
    {
        dataTag = tag; DATA = data;
    }

    public static bool operator ==(NumberData x, int y)
    {
        return x.DATA == y;
    }
    public static bool operator !=(NumberData x, int y)
    {
        return x.DATA != y;
    }
    public static bool operator >(NumberData x, int y)
    {
        return x.DATA > y;
    }
    public static bool operator <(NumberData x, int y)
    {
        return x.DATA < y;
    }
    public static NumberData zero
    {
        get { return new NumberData(0); }
    }
    public override bool Equals(object obj)
    {
        return obj.ToString() == ToString();
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        if (dataTag == DataType.integer) return "" + DATA;
        else if (dataTag == DataType.percent) return DATA + "%";
        return base.ToString();
    }
    public int this[int d]
    {
        get { return DATA; }
    }
    public float DECIMAL
    {
        get
        {
            if (dataTag == DataType.integer) return DATA;
            else if (dataTag == DataType.percent) return DATA * 1f / 100;
            else return DATA;
        }
    }
    public static NumberData Build(int data, DataType tag = DataType.integer)
    {
        return new NumberData(data, tag);
    }
}

[System.Serializable]
public class NDBarChart
{
    public NumberData[] Datas
    {
        get { return new NumberData[] { HP,MP,TP}; }
    }
    public NDBarChart(RoleBarChart bc)
    {
        HP = new NumberData(bc.HP);
        MP = new NumberData(bc.MP);
        TP = new NumberData(bc.TP);
    }
    public static NDBarChart zero
    {
        get { return new NDBarChart(RoleBarChart.zero); }
    }
    public NumberData HP;
    public NumberData MP;
    public NumberData TP;

    public static NDBarChart Build(NumberData hp, NumberData mp, NumberData tp)
    {
        return new NDBarChart(RoleBarChart.zero)
        {
            HP = hp,
            MP = mp,
            TP = tp,
        };
    }
}