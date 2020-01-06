using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using GameDataEditor;

[CreateAssetMenu(fileName ="heroAnimImgList",menuName ="Wun/模型/模型替换素材列表")]
public class HeroAnimImgList : ScriptableObject
{
    [SerializeField]
    private int skeleton;
    public int Skeleton { get { return skeleton; } }


    [System.Serializable]
    public class SlotRegionPairList
    {
        public string slot;
        [SerializeField] private SpineAtlasAsset atlasAsset;
        public SpineAtlasAsset AtlasAsset { get { return atlasAsset; } }
        //[DisplayName("对应备选图片列表")]
        public List<SlotRegionPair> AllRegionList;
    }
    [System.Serializable]
    public class SlotRegionPair
    {
        #region 图片链接
        //[SpineAtlasRegion]
        public string region;
        public string Region { get { return region; } }
        #endregion
        #region 图像修正
        [SerializeField] private bool useExtraData;
        public bool UseExtraData { get { return useExtraData; } }

        [SerializeField,ConditionalHide("useExtraData",true)]
        private Vector2 positionOffset = Vector2.zero;
        public Vector2 PositionOffset { get { return positionOffset; } }

        [SerializeField,ConditionalHide("useExtraData",true)] 
        private Vector2 scale = Vector2.one;
        public Vector2 Scale { get { return scale; } }
        #endregion
    }
    [SerializeField]
    private List<SlotRegionPairList> allEnableList = new List<SlotRegionPairList>();
    public List<SlotRegionPairList> AllEnableList { get { return allEnableList; } }
}