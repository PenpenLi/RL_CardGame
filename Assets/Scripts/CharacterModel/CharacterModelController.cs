using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using GameDataEditor;
using System.Linq;

/// <summary>
/// 角色模型管理器，用于控制使用哪种模型
/// </summary>
public class CharacterModelController : MonoBehaviour
{
    /// <summary>
    /// Used Development Environment
    /// </summary>
    public enum UDE
    {
        Graphic,Animation
    }
    public UDE thisCMCsUDE = UDE.Animation;
    [HideInInspector]
    public int heroHashcode;
    [HideInInspector]
    public bool notShowWeaponSlot =false;
    [HideInInspector]
    public string weaponId;
    public string id;
    [Space(25)]
    public SkeletonDataAsset SkeletonData;
    [SpineSkin(dataField: "SkeletonData")]
    public string skinName;
    public SDConstants.CharacterType CType;
    public Transform ModelParent;
    public CharacterModel CurrentCharacterModel
    {
        get
        {
            switch (thisCMCsUDE)
            {
                case UDE.Animation:return cm_a;
                case UDE.Graphic:return cm_g;
                default:return cm_a;
            }
        }
    }
    public CharacterModel_animation cm_a;
    public CharacterModel_graphic cm_g;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public float scaleRate = 1;
    //角色模型是否底部对齐显示，游戏中角色是否都是底部对齐
    public bool isModelAlignBottom = false;


    public void initHeroCharacterModel(int hashcode,float scale = 1,UDE _ude = UDE.Animation)
    {
        thisCMCsUDE = _ude;
        heroHashcode = hashcode;
        id = SDDataManager.Instance.getHeroIdByHashcode(hashcode);
        CType = SDConstants.CharacterType.Hero;
        skinName = SDDataManager.Instance.getHeroSkinNameInSkeleton(hashcode);

        //
        GDEEquipmentData weapon = SDDataManager.Instance.getHeroWeapon(hashcode);

        if(weapon ==null || weapon.hashcode<=0 || string.IsNullOrEmpty(weapon.id))
        {
            notShowWeaponSlot = true;
        }
        else
        {
            notShowWeaponSlot = false;
            weaponId = weapon.id;
        }
        
        //
        initCharacterModel(scale);
    }
    public void initEnemyCharacterModel(string enemyId,float scale = 1,UDE _ude = UDE.Animation)
    {
        thisCMCsUDE = _ude;
        id = enemyId;
        CType = SDConstants.CharacterType.Enemy;
        skinName = "default";
        initCharacterModel(scale);
    }
    /// <summary>
    /// 初始化可用模型
    /// </summary>
    /// <param name="hashcode"></param>
    /// <param name="animtype"></param>
    /// <param name="scale"></param>
    public void initCharacterModel(float scale = 1)
    {
        scaleRate = scale;
        if (CType == SDConstants.CharacterType.Hero)
        {
            //构建
            HeroInfo hero = SDDataManager.Instance.getHeroInfoById(id);

            //
            if (!hero.UseSpineData) return;
            SkeletonData = hero.SpineData.SkeletonData;
        }
        else if(CType == SDConstants.CharacterType.Enemy)
        {
            EnemyInfo enemy = SDDataManager.Instance.getEnemyInfoById(id);
            //构建
            if (!enemy.UseSpineData) return;
            SkeletonData = enemy.SpineData.SkeletonData;
        }

        if(thisCMCsUDE == UDE.Animation)
        {
            //StartCoroutine(IEInitAsset());
            IEInitAsset();
        }
        else if(thisCMCsUDE == UDE.Graphic)
        {
            InitAssetInUI();
        }

    }

    #region NewSkeletonAnimation
    public void IEInitAsset()
    {
        //yield return new WaitForSeconds(0.1f);
        SkeletonData.GetSkeletonData(false); // Preload SkeletonDataAsset.
        //yield return new WaitForSeconds(0.25f); // Pretend stuff is happening.

        if (ModelParent.childCount == 0)
        {
            var sa = SkeletonAnimation.NewSkeletonAnimationGameObject(SkeletonData);
            sa.name = "MODE_SINGLEROLE";
            setNewModel(sa);
        }
        else
        {
            if (ModelParent.GetComponentInChildren<CharacterModel_animation>())
            {
                ModelParent.GetComponentInChildren<CharacterModel_animation>()
                    .initCharacterModel();
            }
        }
    }
    void setNewModel(SkeletonAnimation sa)
    {
        sa.transform.SetParent(ModelParent, false);
        Vector2 p0 = new Vector2(transform.localPosition.x / transform.localScale.x
            , transform.localPosition.y / transform.localScale.y);
        Vector2 p1 = new Vector2(ModelParent.transform.localPosition.x / transform.localScale.x
            , ModelParent.transform.localPosition.y / transform.localScale.y);
        sa.transform.localPosition = isModelAlignBottom ? p0-p1 : Vector2.zero;
        Vector3 lpos = sa.transform.localPosition;
        sa.transform.localPosition = new Vector3(lpos.x, lpos.y, -5);
        sa.transform.localScale = Vector3.one * scaleRate;

        sa.zSpacing = -0.1f;

        sa.gameObject.AddComponent<Model_ColorInitialize>();
        cm_a = sa.gameObject.AddComponent<CharacterModel_animation>();
        CurrentCharacterModel.CMC = this;
        CurrentCharacterModel.DataAsset = SkeletonData;
        CurrentCharacterModel.initCharacterModel();
    }
    #endregion

    #region NewSkeletonGraphic
    public void InitAssetInUI()
    {
        Debug.Log("INITCharacter_UI");

        SkeletonData.GetSkeletonData(false); // Preload SkeletonDataAsset.

        if (ModelParent.childCount == 0)
        {
            Material m = Resources.LoadAll<Material>("Spine/spineGraphic").ToList()
                .Find(x => x.name.Contains("default"));
            var sa = SkeletonGraphic.NewSkeletonGraphicGameObject
                (SkeletonData,ModelParent,m);
            sa.name = "MODE_SINGLEROLE";
            sa.rectTransform.localScale = Vector3.one * scaleRate;
            Vector2 p0 = new Vector2(this.GetComponent<RectTransform>().anchoredPosition.x
                / this.GetComponent<RectTransform>().localScale.x
                , this.GetComponent<RectTransform>().anchoredPosition.y
                / this.GetComponent<RectTransform>().localScale.y
                );
            Vector2 p1 = new Vector2(ModelParent.GetComponent<RectTransform>().anchoredPosition.x
                / ModelParent.GetComponent<RectTransform>().localScale.x
                , ModelParent.GetComponent<RectTransform>().anchoredPosition.y
                / ModelParent.GetComponent<RectTransform>().localScale.y
                );
            sa.rectTransform.anchoredPosition =isModelAlignBottom? p0 - p1:Vector2.zero;
            //
            sa.gameObject.AddComponent<Model_ColorInitialize>();
            cm_g = sa.gameObject.AddComponent<CharacterModel_graphic>();
            CurrentCharacterModel.CMC = this;
            CurrentCharacterModel.DataAsset = SkeletonData;

            //
            CurrentCharacterModel.initCharacterModel();
        }
        else
        {
            if (ModelParent.GetComponentInChildren<CharacterModel_graphic>())
            {
                ModelParent.GetComponentInChildren<CharacterModel_graphic>()
                    .initCharacterModel();
            }
        }
    }
    #endregion

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
    public void SpawnFromSpineData(Transform spawnPlace, SkeletonDataAsset SpineData, string Name)
    {
        if (SpineData == null) return;
        SpineData.GetSkeletonData(false);
        var anim = SpineData.GetSkeletonData(false)
            .FindAnimation(SDConstants.AnimName_IDLE);
        var sa = SkeletonAnimation.NewSkeletonAnimationGameObject(SpineData);
        //
        sa.gameObject.name = Name + "_Mode";
        Transform s = sa.transform;
        s.SetParent(spawnPlace);
        s.localScale = Vector3.one;
        s.position = spawnPlace.position;
        //
        sa.Initialize(false);
        sa.AnimationState.SetAnimation(0, anim, true);
    }

    public void SetCurrentCMSortingOrder(int index)
    {
        if (CurrentCharacterModel)
        {
            CurrentCharacterModel.GetComponent<MeshRenderer>().sortingOrder = index;
        }
    }
}
