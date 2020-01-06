using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRace : BaseRank
{
    [SerializeField,ReadOnly]
    private SDConstants.CharacterType characterType;
    public SDConstants.CharacterType CharacterType
    {
        get { return characterType; }
        protected set { characterType = value; }
    }
}
