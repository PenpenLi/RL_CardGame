using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateTag
{
    Bleed=0,
    Mind=1,
    Fire=2,
    Frost=3,
    Corrosion=4,
    Hush=5,
    Dizzy=6,
    Confuse=7,
    End=8,
}
public class RegendStateController
{
    public SDConstants.BCType TAG;
    public int Condition;
    public int Data;
    public int LastTime;
    public void clear()
    {
        Condition = 0;
        Data = 0;
        LastTime = 0;
    }
}
public class StateExampleSet : MonoBehaviour
{
    
}


