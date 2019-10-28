using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRandomSetClass
{
    /// <summary>
    /// 整数转百分比生成概率的结果
    /// </summary>
    /// <param name="data">输入整数</param>
    /// <returns></returns>
    public static bool PercentIdentify(int data)
    {
        int a = UnityEngine.Random.Range(0,100);
        if (a < data)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 百分数转小数
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static float SimplePercentToDecimal(int percent)
    {
        return percent * 1f / 100;
    }
}
public class RandomIntger
{
    #region 标准随机
    public static int Choose(float[] probs)
    {

        float total = 0;

        for (int i = 0; i < probs.Length; i++)
        {
            total += probs[i];
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }
    public int Result;
    public static int SimpleReturn(int all)
    {
        float[] a = new float[all];
        for (int i = 0; i < all; i++) { a[i] = 1; }
        return Choose(a);
    }
    public static int StandardReturn(float[] p)
    {
        return Choose(p);
    }
    #endregion
    # region 随机选择多个
    static public int[] NumArrayReturn(int SN, float[] APs)
    {
        int[] allImport = new int[SN];
        if (SN > APs.Length)
        {
            for (int i = 0; i < SN; i++) { allImport[i] = i; }
            return allImport;
        }
        //
        int temp;
        List<int> all = new List<int>();
        for (int i = 1; i <= SN; i++)
        {
            temp = StandardReturn(APs);
            if (!all.Contains(temp))
            {
                allImport[i - 1] = temp;
                all.Add(temp);
            }
            else
            {
                i--;
            }
        }
        return allImport;
    }
    public static int[] NumArrayReturn(int SN,int AP)
    {
        int[] allImport = new int[SN];
        if (SN > AP)
        {
            for (int i = 0; i < SN; i++) { allImport[i] = i; }
            return allImport;
        }
        //
        int temp = 0;
        List<int> all = new List<int>();
        for (int i = 1; i <= SN; i++)
        {
            temp = SimpleReturn(AP);
            if (!all.Contains(temp))
            {
                allImport[i - 1] = temp;
                all.Add(temp);
            }
            else
            {
                i--;
            }
        }
        return allImport;
    }
    public static int[] MultipleReturn(float[] probs, int count)
    {
        if (probs.Length < count)
        {
            int[] sequence = new int[probs.Length];
            for (int i = 0; i < sequence.Length; i++)
            {
                sequence[i] = i;
            }
            return sequence;
        }
        //
        int[] output = new int[count];
        List<int> list = new List<int>();
        int temp = 0;
        for (int i = 1; i <= count; i++)
        {
            temp = StandardReturn(probs);
            if (!list.Contains(temp))
            {
                output[i - 1] = temp;
                list.Add(temp);
            }
            else
            {
                i--;
            }
        }
        return output;
    }

    static public List<int> NumListReturn(int SN, float[] APs)
    {
        int[] allImport = new int[SN];
        //
        int temp;
        List<int> all = new List<int>();
        for (int i = 1; i <= SN; i++)
        {
            temp = StandardReturn(APs);
            if (!all.Contains(temp))
            {
                allImport[i - 1] = temp;
                all.Add(temp);
            }
            else
            {
                i--;
            }
        }
        return all;
    }
    static public List<int> NumListReturn(int SN,int AP)
    {
        int[] allImport = new int[SN];
        //
        int temp;
        List<int> all = new List<int>();
        for (int i = 1; i <= SN; i++)
        {
            temp = SimpleReturn(AP);
            if (!all.Contains(temp))
            {
                allImport[i - 1] = temp;
                all.Add(temp);
            }
            else
            {
                i--;
            }
        }
        return all;
    }
    #endregion
    #region 费雪耶兹(高纳德)随机置乱算法
    public static int[] FisherYatesReturn(int all)
    {
        if (all > 0)
        {
            int[] t = new int[all];
            for (int i = 0; i < all; i++) { t[i] = i; }
            //
            int[] _t = new int[all];
            for (int i = all - 1; i > 0; i--)
            {
                int rand = SimpleReturn(i + 1);
                _t[i] = rand;
                t[rand] = i;
            }
            return _t;
        }
        else
        {
            Debug.Log("FY乱置错误：输入值超出范围");
            return new int[] { all };
        }
    }
    #endregion
}
