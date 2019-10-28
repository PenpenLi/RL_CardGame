using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum NumberUnits
{
    N=0,
    K=1,
    M=2,
    B=3,
    T=4,
    AA=5,AB=6,AC=7,AD=8,AE=9,AF=10,AG=11,AH=12,AI=13,AJ=14,AK=15,AL=16,AM=17,AN=18,AO=19,AP=20,
}
public class Number
{
    public NumberUnits units;
    public List<ushort> numbers;
    public static Number Zero = new Number("0 N");
    //公开索引器通过单位名获取某单位值
    public ushort this[NumberUnits uni]
    {
        get { return numbers[(int)uni]; }
        set { numbers[(int)uni] = value; }
    }
    public ushort this[int index]
    {
        get { return numbers[index]; }
        set { numbers[index] = value; }
    }
    public Number(string num)
    {
        if (num == null || num == "") num = "0";
        num = num.Replace("_", "");
        if (num.Contains(" "))
        {
            units = (NumberUnits)Enum.Parse(typeof(NumberUnits), num.Split(' ')[1]);
            numbers = new List<ushort>(20);
            for (int i = 0; i <= (int)units; i++)
            {
                numbers.Add(0);
            }
            decimal number = decimal.Parse(num.Split(' ')[0]);
            numbers[(int)units] = (ushort)number;
            if (!num.Contains("N")) numbers[(int)units - 1] = (ushort)((number - (short)number) * 1000);
        }
        else
        {
            int length = num.Length;
            units = (NumberUnits)(length % 3 == 0 ? length / 3 - 1 : length / 3);
            numbers = new List<ushort>(20);
            for (int i = 1; i <= (int)units + 1; i++)
            {
                int index = length - i * 3 < 0 ? 0 : length - i * 3;
                int count = length - i * 3 < 0 ? length % 3 : 3;
                string clip = num.Substring(index, count);
                numbers.Add(ushort.Parse(clip));
            }
        }
    }
    public Number(int numInt)
    {
        string num = "" + numInt;
        num = num.Replace("-", "");
        if (num.Contains(" "))
        {
            units = (NumberUnits)Enum.Parse(typeof(NumberUnits), num.Split(' ')[1]);
            numbers = new List<ushort>(20);
            for (int i = 0; i <= (int)units; i++)
            {
                numbers.Add(0);
            }
            decimal number = decimal.Parse(num.Split(' ')[0]);
            numbers[(int)units] = (ushort)number;
            if (!num.Contains("N"))
                numbers[(int)units - 1] = (ushort)((number - (short)number) * 1000);
        }
        else
        {
            int length = num.Length;
            units = (NumberUnits)(length % 3 == 0 ? length / 3 - 1 : length / 3);
            numbers = new List<ushort>(20);
            for (int i = 1; i <= (int)units + 1; i++)
            {
                int index = length - i * 3 < 0 ? 0 : length - i * 3;
                int count = length - i * 3 < 0 ? length % 3 : 3;
                string clip = num.Substring(index, count);
                numbers.Add(ushort.Parse(clip));
            }
        }
    }
    //给集合里加数据 
    private void AddUnits(ushort num)
    {
        if (numbers == null)
            numbers = new List<ushort>();
        numbers.Add(num);
    }
    //复制一个Number
    public void CopyNumber(Number num)
    {
        units = num.units;
        numbers = new List<ushort>(num.numbers);
    }
    public override string ToString()
    {
        //return this[units].ToString() + units.ToString().Replace("N", "");
        if (this == Zero)
            return "0";
        /*
        //cx:1亿以下的数字保留原来的单位
        Number number1 = new Number("1000000000");
        if (this < number1)
        {
            return BigNumber.getBigNumber(ConvertNumberToInt(this));
        }
        */
        string result = this[units].ToString();
        if (units != NumberUnits.N)
            result = this[(units - 1)] != 0 ? result + "." + ((float)this[(units - 1)] / 1000).ToString().Replace("0.", "").TrimEnd('0') : result;
        result += units.ToString();

        /*
        if (this.units == NumberUnits.T)
        {
            if (LocalizationManager.CurrentLanguage == "Chinese (Simplified)")
            {
                result = result.Replace("T", "万亿");
            }
            else if (LocalizationManager.CurrentLanguage == "Chinese (Traditional)")
            {
                result = result.Replace("T", "萬億");
            }
        }
        else if (this.units == NumberUnits.B)
        {
            if (LocalizationManager.CurrentLanguage == "Chinese (Simplified)")
            {
                result = result.Replace("B", "十亿");
            }
            else if (LocalizationManager.CurrentLanguage == "Chinese (Traditional)")
            {
                result = result.Replace("B", "十億");
            }
        }
        */
        return result.Replace("N", "");

    }
}
    
