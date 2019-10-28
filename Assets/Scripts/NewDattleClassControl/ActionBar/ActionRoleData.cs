using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 行动单元类
/// </summary>
public class ActionRoleData : MonoBehaviour
{
    public CharacterModelController headImage;
    public Image stateBgImage;
    public Sprite[] stateSprites;
    public float speed;
    public bool isActed=false;
    public BattleRoleData battleUnit;
    public bool isEnemy;
    [Space(25)]
    public Text nameText;
    private void Start()
    {
        GetComponent<Canvas>().sortingLayerName = "UI";
    }
    public void initActionUnit()
    {

    }
}
