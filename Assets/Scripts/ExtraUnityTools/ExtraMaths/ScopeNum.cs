using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

public class ScopeNum : MonoBehaviour
{

}
[System.Serializable]
public class ScopeInt
{
    #region MIN
    [SerializeField]
    private int min;
    public int Min
    {
        get { return min; }
        set
        {
            if (max < value) min = max;
            else min = value;
            if (min > current) current = min;
        }
    }
    #endregion

    #region MAX
    [SerializeField]
    private int max;
    public int Max
    {
        get { return max; }
        set
        {
            if (value < 0) max = 0;
            else if (value < min) max = min + 1;
            else max = value;
            if (max < current) current = max;
        }
    }
    #endregion

    #region CURRENT
    private int current;
    public int Current
    {
        get
        {
            return current;
        }

        set
        {
            if (value > Max) current = Max;
            else if (value < Min) current = Min;
            else current = value;
        }
    }
    #endregion

    #region ScopInt_Init
    public ScopeInt() { Min = 0;Max = 1; }
    public ScopeInt(int min,int max)
    {
        this.Min = min;
        this.Max = max;
    }
    public ScopeInt(int max)
    {
        Min = 0;Max = max;
    }
    #endregion
    public bool IsMax
    {
        get { return Current == Max; }
    }
    public bool IsMin
    {
        get { return Current == Min; }
    }
    public void ToMin() { Current = Min; }
    public void ToMax() { Current = Max; }
    public int Rest
    {
        get { return Max - Current; }
    }
    /// <summary>
    /// 四分之一
    /// </summary>
    public int Quarter { get { return Max / 4; } }

    public int Half { get { return Max / 2; } }

    /// <summary>
    /// 四分之三
    /// </summary>
    public int Three_Fourths { get { return (int)(Max * 0.75f); } }

    /// <summary>
    /// 三分之一
    /// </summary>
    public int One_Third { get { return Max / 3; } }
    #region 运算符重载
    #region 加减乘除
    public static ScopeInt operator +(ScopeInt left, int right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = left.Current + right };
    }
    public static ScopeInt operator -(ScopeInt left, int right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = left.Current - right };
    }
    public static ScopeInt operator +(ScopeInt left, float right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = (int)(left.Current + right) };
    }
    public static ScopeInt operator -(ScopeInt left, float right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = (int)(left.Current - right) };
    }
    public static int operator +(int left, ScopeInt right)
    {
        return left + right.Current;
    }
    public static int operator -(int left, ScopeInt right)
    {
        return left - right.Current;
    }
    public static float operator +(float left, ScopeInt right)
    {
        return left + right.Current;
    }
    public static float operator -(float left, ScopeInt right)
    {
        return left - right.Current;
    }


    public static ScopeInt operator *(ScopeInt left, int right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = left.Current * right };
    }
    public static ScopeInt operator /(ScopeInt left, int right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = left.Current * right };
    }
    public static ScopeInt operator *(ScopeInt left, float right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = (int)(left.Current * right) };
    }
    public static ScopeInt operator /(ScopeInt left, float right)
    {
        return new ScopeInt(left.Min, left.Max) { Current = (int)(left.Current / right) };
    }
    public static int operator *(int left, ScopeInt right)
    {
        return left * right.Current;
    }
    public static int operator /(int left, ScopeInt right)
    {
        return left / right.Current;
    }
    public static float operator *(float left, ScopeInt right)
    {
        return left * right.Current;
    }
    public static float operator /(float left, ScopeInt right)
    {
        return left / right.Current;
    }

    public static ScopeInt operator ++(ScopeInt original)
    {
        original.Current++;
        return original;
    }
    public static ScopeInt operator --(ScopeInt original)
    {
        original.Current--;
        return original;
    }
    #endregion

    #region 大于、小于
    public static bool operator >(ScopeInt left, int right)
    {
        return left.Current > right;
    }
    public static bool operator <(ScopeInt left, int right)
    {
        return left.Current < right;
    }
    public static bool operator >(ScopeInt left, float right)
    {
        return left.Current > right;
    }
    public static bool operator <(ScopeInt left, float right)
    {
        return left.Current < right;
    }
    public static bool operator >(int left, ScopeInt right)
    {
        return left > right.Current;
    }
    public static bool operator <(int left, ScopeInt right)
    {
        return left < right.Current;
    }
    public static bool operator >(float left, ScopeInt right)
    {
        return left > right.Current;
    }
    public static bool operator <(float left, ScopeInt right)
    {
        return left < right.Current;
    }
    #endregion

    #region 大于等于、小于等于
    public static bool operator >=(ScopeInt left, int right)
    {
        return left.Current >= right;
    }
    public static bool operator <=(ScopeInt left, int right)
    {
        return left.Current <= right;
    }
    public static bool operator >=(ScopeInt left, float right)
    {
        return left.Current >= right;
    }
    public static bool operator <=(ScopeInt left, float right)
    {
        return left.Current <= right;
    }
    public static bool operator >=(int left, ScopeInt right)
    {
        return left >= right.Current;
    }
    public static bool operator <=(int left, ScopeInt right)
    {
        return left <= right.Current;
    }
    public static bool operator >=(float left, ScopeInt right)
    {
        return left >= right.Current;
    }
    public static bool operator <=(float left, ScopeInt right)
    {
        return left <= right.Current;
    }
    #endregion

    #region 等于、不等于
    public static bool operator ==(ScopeInt left, int right)
    {
        return left.Current == right;
    }
    public static bool operator !=(ScopeInt left, int right)
    {
        return left.Current != right;
    }
    public static bool operator ==(ScopeInt left, float right)
    {
        return left.Current == right;
    }
    public static bool operator !=(ScopeInt left, float right)
    {
        return left.Current != right;
    }
    public static bool operator ==(int left, ScopeInt right)
    {
        return left == right.Current;
    }
    public static bool operator !=(int left, ScopeInt right)
    {
        return left != right.Current;
    }
    public static bool operator ==(float left, ScopeInt right)
    {
        return left == right.Current;
    }
    public static bool operator !=(float left, ScopeInt right)
    {
        return left != right.Current;
    }
    #endregion

    public static explicit operator float(ScopeInt original)
    {
        return original.Current;
    }
    public static explicit operator int(ScopeInt original)
    {
        return original.Current;
    }
    public static implicit operator ScopeFloat(ScopeInt original)
    {
        return new ScopeFloat(original.Min, original.Max) { Current = original.Current };
    }
    #endregion

    public override string ToString()
    {
        return Current + "/" + Max;
    }

    public string ToString(string format)
    {
        if (format == "//") return Min + "/" + Current + "/" + Max;
        else if (format == "[/]") return "[" + Current + "/" + Max + "]";
        else if (format == "[//]") return "[" + Min + "/" + Current + "/" + Max + "]";
        else if (format == "(/)") return "(" + Current + "/" + Max + ")";
        else if (format == "(//)") return "(" + Min + "/" + Current + "/" + Max + ")";
        else if (format == "/") return Current + "/" + Max;
        else return ToString();
    }

    public string ToString(string start, string split, string end, bool showMin = false)
    {
        if (showMin)
        {
            return start + Min + split + Current + split + Max + end;
        }
        return start + Current + split + Max + end;
    }

    public override bool Equals(object obj)
    {
        return obj is ScopeInt @int &&
               min == @int.min &&
               max == @int.max &&
               current == @int.current;
    }

    public override int GetHashCode()
    {
        var hashCode = 1173473123;
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        hashCode = hashCode * -1521134295 + Min.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        hashCode = hashCode * -1521134295 + Max.GetHashCode();
        hashCode = hashCode * -1521134295 + current.GetHashCode();
        hashCode = hashCode * -1521134295 + Current.GetHashCode();
        hashCode = hashCode * -1521134295 + IsMax.GetHashCode();
        hashCode = hashCode * -1521134295 + IsMin.GetHashCode();
        hashCode = hashCode * -1521134295 + Rest.GetHashCode();
        hashCode = hashCode * -1521134295 + Quarter.GetHashCode();
        hashCode = hashCode * -1521134295 + Half.GetHashCode();
        hashCode = hashCode * -1521134295 + Three_Fourths.GetHashCode();
        hashCode = hashCode * -1521134295 + One_Third.GetHashCode();
        return hashCode;
    }
}

[System.Serializable]
public class ScopeFloat
{
    [SerializeField]
    private float min;
    public float Min
    {
        get { return min; }
        set
        {
            if (max < value) min = max;
            else min = value;
        }
    }

    [SerializeField]
    private float max;
    public float Max
    {
        get { return max; }
        set
        {
            if (value < 0) max = 0;
            else if (value < min) max = min + 1;
            else max = value;
        }
    }

    private float current;
    public float Current
    {
        get
        {
            return current;
        }

        set
        {
            if (value > Max) current = Max;
            else if (value < Min) current = Min;
            else current = value;
        }
    }

    public ScopeFloat()
    {
        Min = 0;
        Max = 1;
    }

    public ScopeFloat(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public ScopeFloat(float max)
    {
        Min = 0;
        Max = max;
    }

    public bool IsMax
    {
        get
        {
            return current == Max;
        }
    }

    public bool IsMin
    {
        get
        {
            return current == Min;
        }
    }

    /// <summary>
    /// 余下部分
    /// </summary>
    public float Rest { get { return Max - Current; } }

    public void ToMin()
    {
        Current = Min;
    }

    public void ToMax()
    {
        Current = Max;
    }

    /// <summary>
    /// 四分之一
    /// </summary>
    public float Quarter { get { return Max * 0.25f; } }

    public float Half { get { return Max * 0.5f; } }

    /// <summary>
    /// 四分之三
    /// </summary>
    public float Three_Fourths { get { return Max * 0.75f; } }

    /// <summary>
    /// 三分之一
    /// </summary>
    public float One_Third { get { return Max / 3; } }

    #region 运算符重载
    #region 加减乘除
    public static ScopeFloat operator +(ScopeFloat left, int right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current + right };
    }
    public static ScopeFloat operator -(ScopeFloat left, int right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current - right };
    }
    public static ScopeFloat operator +(ScopeFloat left, float right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current + right };
    }
    public static ScopeFloat operator -(ScopeFloat left, float right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current - right };
    }
    public static int operator +(int left, ScopeFloat right)
    {
        return (int)(left + right.Current);
    }
    public static int operator -(int left, ScopeFloat right)
    {
        return (int)(left - right.Current);
    }
    public static float operator +(float left, ScopeFloat right)
    {
        return left + right.Current;
    }
    public static float operator -(float left, ScopeFloat right)
    {
        return left - right.Current;
    }

    public static ScopeFloat operator *(ScopeFloat left, int right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current * right };
    }
    public static ScopeFloat operator /(ScopeFloat left, int right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current / right };
    }
    public static ScopeFloat operator *(ScopeFloat left, float right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current * right };
    }
    public static ScopeFloat operator /(ScopeFloat left, float right)
    {
        return new ScopeFloat(left.Min, left.Max) { Current = left.Current / right };
    }
    public static int operator *(int left, ScopeFloat right)
    {
        return (int)(left * right.Current);
    }
    public static int operator /(int left, ScopeFloat right)
    {
        return (int)(left / right.Current);
    }
    public static float operator *(float left, ScopeFloat right)
    {
        return left * right.Current;
    }
    public static float operator /(float left, ScopeFloat right)
    {
        return left / right.Current;
    }

    public static ScopeFloat operator ++(ScopeFloat original)
    {
        original.Current++;
        return original;
    }
    public static ScopeFloat operator --(ScopeFloat original)
    {
        original.Current--;
        return original;
    }
    #endregion

    #region 大于、小于
    public static bool operator >(ScopeFloat left, int right)
    {
        return left.Current > right;
    }
    public static bool operator <(ScopeFloat left, int right)
    {
        return left.Current < right;
    }
    public static bool operator >(ScopeFloat left, float right)
    {
        return left.Current > right;
    }
    public static bool operator <(ScopeFloat left, float right)
    {
        return left.Current < right;
    }
    public static bool operator >(int left, ScopeFloat right)
    {
        return left > right.Current;
    }
    public static bool operator <(int left, ScopeFloat right)
    {
        return left < right.Current;
    }
    public static bool operator >(float left, ScopeFloat right)
    {
        return left > right.Current;
    }
    public static bool operator <(float left, ScopeFloat right)
    {
        return left < right.Current;
    }
    #endregion

    #region 大于等于、小于等于
    public static bool operator >=(ScopeFloat left, int right)
    {
        return left.Current >= right;
    }
    public static bool operator <=(ScopeFloat left, int right)
    {
        return left.Current <= right;
    }
    public static bool operator >=(ScopeFloat left, float right)
    {
        return left.Current >= right;
    }
    public static bool operator <=(ScopeFloat left, float right)
    {
        return left.Current <= right;
    }
    public static bool operator >=(int left, ScopeFloat right)
    {
        return left >= right.Current;
    }
    public static bool operator <=(int left, ScopeFloat right)
    {
        return left <= right.Current;
    }
    public static bool operator >=(float left, ScopeFloat right)
    {
        return left >= right.Current;
    }
    public static bool operator <=(float left, ScopeFloat right)
    {
        return left <= right.Current;
    }
    #endregion

    #region 等于、不等于
    public static bool operator ==(ScopeFloat left, int right)
    {
        return left.Current == right;
    }
    public static bool operator !=(ScopeFloat left, int right)
    {
        return left.Current != right;
    }
    public static bool operator ==(ScopeFloat left, float right)
    {
        return left.Current == right;
    }
    public static bool operator !=(ScopeFloat left, float right)
    {
        return left.Current != right;
    }
    public static bool operator ==(int left, ScopeFloat right)
    {
        return left == right.Current;
    }
    public static bool operator !=(int left, ScopeFloat right)
    {
        return left != right.Current;
    }
    public static bool operator ==(float left, ScopeFloat right)
    {
        return left == right.Current;
    }
    public static bool operator !=(float left, ScopeFloat right)
    {
        return left != right.Current;
    }
    #endregion
    public static explicit operator float(ScopeFloat original)
    {
        return original.Current;
    }
    public static explicit operator int(ScopeFloat original)
    {
        return (int)original.Current;
    }
    public static explicit operator ScopeInt(ScopeFloat original)
    {
        return new ScopeInt((int)original.Min, (int)original.Max) { Current = (int)original.Current };
    }
    #endregion

    public override string ToString()
    {
        return Current.ToString() + "/" + Max.ToString();
    }

    public string ToString(string format)
    {
        string amount = Regex.Replace(format, @"[^F^0-9]+", "");
        if (format.Contains("//"))
        {
            return Min.ToString(amount) + "/" + Current.ToString(amount) + "/" + Max.ToString(amount);
        }
        else if (format == "[/]")
        {
            return "[" + Current.ToString(amount) + "/" + Max.ToString(amount) + "]";
        }
        else if (format == "[//]")
        {
            return "[" + Min.ToString(amount) + "/" + Current.ToString(amount) + "/" + Max.ToString(amount) + "]";
        }
        else if (format == "(/)")
        {
            return "(" + Current.ToString(amount) + "/" + Max.ToString(amount) + ")";
        }
        else if (format == "(//)")
        {
            return "(" + Min.ToString(amount) + "/" + Current.ToString(amount) + "/" + Max.ToString(amount) + ")";
        }
        else if (!string.IsNullOrEmpty(amount)) return Current.ToString(amount) + "/" + Max.ToString(amount);
        else return ToString();
    }

    /// <summary>
    /// 转成字符串
    /// </summary>
    /// <param name="start">字符串开头</param>
    /// <param name="split">数字分隔符</param>
    /// <param name="end">字符串结尾</param>
    /// <param name="decimalDigit">小数保留个数</param>
    /// <param name="showMin">是否显示最小值</param>
    /// <returns>目标字符串</returns>
    public string ToString(string start, string split, string end, int decimalDigit, bool showMin = false)
    {
        if (showMin)
        {
            return start + Min.ToString("F" + decimalDigit) + split + Current.ToString("F" + decimalDigit) + split + Max.ToString("F" + decimalDigit) + end;
        }
        return start + Current.ToString("F" + decimalDigit) + split + Max.ToString("F" + decimalDigit) + end;
    }

    public override bool Equals(object obj)
    {
        return obj is ScopeFloat @float &&
               min == @float.min &&
               max == @float.max &&
               current == @float.current;
    }

    public override int GetHashCode()
    {
        var hashCode = 1173473123;
        hashCode = hashCode * -1521134295 + min.GetHashCode();
        hashCode = hashCode * -1521134295 + Min.GetHashCode();
        hashCode = hashCode * -1521134295 + max.GetHashCode();
        hashCode = hashCode * -1521134295 + Max.GetHashCode();
        hashCode = hashCode * -1521134295 + current.GetHashCode();
        hashCode = hashCode * -1521134295 + Current.GetHashCode();
        hashCode = hashCode * -1521134295 + IsMax.GetHashCode();
        hashCode = hashCode * -1521134295 + IsMin.GetHashCode();
        hashCode = hashCode * -1521134295 + Rest.GetHashCode();
        hashCode = hashCode * -1521134295 + Quarter.GetHashCode();
        hashCode = hashCode * -1521134295 + Half.GetHashCode();
        hashCode = hashCode * -1521134295 + Three_Fourths.GetHashCode();
        hashCode = hashCode * -1521134295 + One_Third.GetHashCode();
        return hashCode;
    }
}