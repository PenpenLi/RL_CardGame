using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="npc_info",menuName ="Wun/角色/NPC信息")]
public class TalkerInfo : CharacterInfo
{
    [SerializeField]
    private Dialogue defaultDialogue;
    public Dialogue DefaultDialogue
    {
        get { return defaultDialogue; }
    }

    [SerializeField]
    private Dialogue specialDialogue;
    public Dialogue SpecialDialogue
    {
        get { return specialDialogue; }
    }


    [SerializeField]
    private bool isQuestGiver;
    public bool IsQuestGiver
    {
        get { return isQuestGiver; }
    }

    [SerializeField]
    private List<Quest> questsStored = new List<Quest>();
    public List<Quest> QuestsStored
    {
        get { return questsStored; }
    }


}

[System.Serializable]
public class Relationship
{
    [SerializeField]
    private ScopeInt relationshipValue = new ScopeInt(-500, 1000);
    public ScopeInt RelationshipValue
    {
        get { return relationshipValue; }
        set
        {
            relationshipValue = value;

        }
    }
}