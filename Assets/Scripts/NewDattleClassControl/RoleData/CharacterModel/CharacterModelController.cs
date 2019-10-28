using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色模型管理器，用于控制使用哪种模型
/// </summary>
public class CharacterModelController : MonoBehaviour
{
    public int heroHashcode;
    [Space(25)]
    public SDConstants.CharacterModelType type;
    public CharacterModel CharacterModelType0;
    public CharacterModel CharacterModelType1;
    public CharacterModel CharacterModelType2;
    public CharacterModel CharacterModelType3;
    public CharacterModel CharacterModelType4;
    public CharacterModel CharacterModelType5;
    public CharacterModel CharacterModelType6;
    public CharacterModel CharacterModelType7;
    public CharacterModel CharacterModelType8;
    public CharacterModel CharacterModelType9;
    public CharacterModel CharacterModelType10;
    public CharacterModel CharacterModelType11;
    public CharacterModel CharacterModelType12;
    public CharacterModel CharacterModelType13;
    public CharacterModel CharacterModelType14;
    public CharacterModel CharacterModelType15;

    public CharacterModel CurrentCharacterModel;
    //角色模型是否底部对齐显示，游戏中角色是否都是底部对齐
    public bool isModelAlignBottom = false;

    private void Start()
    {
        CurrentCharacterModel = this.gameObject.AddComponent<CharacterModel>();
    }

    /// <summary>
    /// 初始化可用英雄模型
    /// </summary>
    /// <param name="hashcode"></param>
    /// <param name="animtype"></param>
    /// <param name="scale"></param>
    public void initCharacterModel(int hashcode,SDConstants.CharacterAnimType animtype
        ,float scale = 1)
    {
        int id = hashcode;
        if (animtype != SDConstants.CharacterAnimType.Enemy)
        {
            id = SDDataManager.Instance.getHeroIdByHashcode(hashcode);
            SDConstants.CharacterModelType modeltype
                = (SDConstants.CharacterModelType)(id / 100000);
            //构建
            int healId;
            int bodyId;
        }
        else
        {
            List<Dictionary<string, string>> itemDatas;
            //构建
        }
    }
    /// <summary>
    /// 初始化怪物和菜单栏英雄模型
    /// </summary>
    /// <param name="id"></param>
    /// <param name="animType"></param>
    /// <param name="scale"></param>
    /// <param name="isFacingRight"></param>
    public void initCharacterModelById(int id,SDConstants.CharacterAnimType animType
        , float scale=1, bool isFacingRight = true)
    {
        SDConstants.CharacterModelType modelType= (SDConstants.CharacterModelType)(id / 100000);
        //构建

    }
}
