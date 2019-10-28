using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    public bool _isDead;
    public bool isEnemy;
    public bool isBoss;
    public SDConstants.CharacterType _Tag;
    public void ChangeModelAnim(string TriggerName, bool IsLoop = false)
    {
        if (_isDead) return;
        if(_Tag == SDConstants.CharacterType.Hero 
            || _Tag == SDConstants.CharacterType.Enemy)
        {
            //Animator a = GetComponent<Animator>();
            //if(a)   a.SetTrigger(TriggerName);
        }
        
    }
    public void SetReplaceImgState(bool state)
    {

    }
}
