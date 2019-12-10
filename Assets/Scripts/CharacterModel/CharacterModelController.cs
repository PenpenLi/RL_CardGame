using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

/// <summary>
/// 角色模型管理器，用于控制使用哪种模型
/// </summary>
public class CharacterModelController : MonoBehaviour
{
    public int heroHashcode;
    [Space(25)]
    //public SDConstants.CharacterModelType type;
    public List<SkeletonDataAsset> AllSkeletonDataAssets;
    public Transform ModelParent;
    [Space(25)]
    public CharacterModel CurrentCharacterModel;
    public float scaleRate = 1;
    //角色模型是否底部对齐显示，游戏中角色是否都是底部对齐
    public bool isModelAlignBottom = false;


    public void initHeroCharacterModel(int hashcode,float scale = 1)
    {
        string id = SDDataManager.Instance.getHeroIdByHashcode(hashcode);
        
        int j = SDDataManager.Instance.getHeroCareerById(id);
        SDConstants.CharacterAnimType CAT = (SDConstants.CharacterAnimType)j;
        initCharacterModel(hashcode, CAT, scale);
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
        heroHashcode = hashcode;
        string id = string.Empty;
        int DAIndex = 0;
        scaleRate = scale;
        if (animtype != SDConstants.CharacterAnimType.Enemy)
        {
            id = SDDataManager.Instance.getHeroIdByHashcode(heroHashcode);
            DAIndex = SDDataManager.Instance.getHeroSkeletonById(id);
            //构建
            

        }
        else
        {
            //id = hashcode;
            List<Dictionary<string, string>> itemDatas;
            //构建
        }
        if (AllSkeletonDataAssets.Count <= 0)
        {
            CurrentCharacterModel = gameObject.AddComponent<CharacterModel>();
            CurrentCharacterModel.CMC = this;
        }
        else
        {
            StartCoroutine(IEInitAsset(DAIndex));
        }
    }
    public IEnumerator IEInitAsset(int DAIndex)
    {
        yield return new WaitForSeconds(0.1f);

        SkeletonDataAsset ThisAsset = AllSkeletonDataAssets[DAIndex];
        ThisAsset.GetSkeletonData(false); // Preload SkeletonDataAsset.
        yield return new WaitForSeconds(0.5f); // Pretend stuff is happening.

        var sa = SkeletonAnimation.NewSkeletonAnimationGameObject(ThisAsset);
        // Spawn a new SkeletonAnimation GameObject.
        setNewModel(sa);
    }
    void setNewModel(SkeletonAnimation sa)
    {
        sa.transform.SetParent(ModelParent,false);
        sa.transform.position = transform.position;
        sa.transform.localScale = Vector3.one * scaleRate;

        sa.gameObject.AddComponent<Chara_mixAndMatch>();
        sa.gameObject.AddComponent<Model_ColorInitialize>();
        CurrentCharacterModel = sa.gameObject.AddComponent<CharacterModel>();
        CurrentCharacterModel .CMC = this;
        CurrentCharacterModel.initCharacterModel();
    }

    /// <summary>
    /// 初始化怪物和菜单栏英雄模型
    /// </summary>
    /// <param name="id"></param>
    /// <param name="animType"></param>
    /// <param name="scale"></param>
    /// <param name="isFacingRight"></param>
    public void initCharacterModelById(string id,SDConstants.CharacterAnimType animType
        , float scale=1, bool isFacingRight = true)
    {
        int _id = SDDataManager.Instance.getInteger(id.Split('#')[1]);
        SDConstants.CharacterModelType modelType= (SDConstants.CharacterModelType)(_id / 100000);
        //构建

    }

}
