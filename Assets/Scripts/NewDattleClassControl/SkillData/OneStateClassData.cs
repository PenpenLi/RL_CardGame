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
    Nature=5,
    Dizzy=6,
    Confuse=7,
    End,
}
public class StateExampleSet : MonoBehaviour
{
    
}



public class StateEffectStorage
{
    //用于视觉呈现
    public RoleAttributeList UniqueEffectInRA = new RoleAttributeList();
    public RoleBarChart BCArray = new RoleBarChart();
    public void ResetSelf()
    {
        UniqueEffectInRA = RoleAttributeList.zero;
        BCArray.ResetSelf();
    }
}