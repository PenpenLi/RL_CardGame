using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRace : BaseRank
{
#if UNITY_EDITOR
    [SerializeField,ReadOnly]
#endif
    private SDConstants.CharacterType characterType;
    public SDConstants.CharacterType CharacterType
    {
        get { return characterType; }
        protected set { characterType = value; }
    }
}
