using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity.AttachmentTools;
using Spine.Unity;
using GameDataEditor;

[System.Serializable]
public class Chara_atlasRegionAttacher 
{
	[System.Serializable]
	public class SlotRegionPair
	{
		[SpineSlot] public string slot;
		//[SpineAtlasRegion]
		public string region;

		public bool UseOriginalData;
	}
	[SerializeField]
	protected SpineAtlasAsset AtlasAsset;
	public SpineAtlasAsset atlasAsset { get { return AtlasAsset; } }

	[SerializeField]protected bool inheritProperties = true;

	[SerializeField] private List<SlotRegionPair> attachments = new List<SlotRegionPair>();
	public List<SlotRegionPair> Attachments { get { return attachments; } set { attachments = value; } }

	Atlas atlas;
    [HideInInspector]
    public CharacterModel CM;
	public void StartToApply( SkeletonAnimation skeletonRenderer)
	{
		skeletonRenderer.OnRebuild += Apply;
		if (skeletonRenderer.valid) Apply(skeletonRenderer);
	}
	void Apply(SkeletonRenderer skeletonRenderer)
	{
		atlas = AtlasAsset.GetAtlas();
		if (atlas == null) return;
		float scale = skeletonRenderer.skeletonDataAsset.scale;
        foreach (var entry in attachments)
        {
            Slot slot = skeletonRenderer.Skeleton.FindSlot(entry.slot);
            Attachment originalAttachment = slot.Attachment;
            AtlasRegion region = atlas.FindRegion(entry.region);


            if (region == null)
            {
                slot.Attachment = null;
            }
            else if (inheritProperties && originalAttachment != null)
            {
                slot.Attachment = originalAttachment.GetRemappedClone(region, true, true, scale);
            }
            else
            {
                var newRegionAttachment = region.ToRegionAttachment(region.name, scale);
                slot.Attachment = newRegionAttachment;
            }

            if (!entry.UseOriginalData)
            {
                RegionAttachment ra = slot.Attachment as RegionAttachment;
                if (ra == null) return;
                ra = (RegionAttachment)ra.Copy();

                //HeroAnimImgList.SlotRegionPair PAIR = SDDataManager.Instance
                   // .GetPairBySlotAndRegion(CM.SkeletonIndex, entry.slot, entry.region);
                //Vector2 _pos = PAIR.PositionOffset;
                //Vector2 _scale = PAIR.Scale;

                //ra.Width = ra.RegionWidth * scale * _scale.x;
                //ra.Height = ra.RegionHeight * scale * _scale.y;

                //ra.SetPositionOffset( _pos * scale * _scale );
                ra.UpdateOffset();
                slot.Attachment = ra;
            }
		}
	}

	public Chara_atlasRegionAttacher(SpineAtlasAsset asset)
	{
		AtlasAsset = asset;
	}
}

public class ReadAnimImgList : MonoBehaviour
{
    public static void initCharaModelByGDE(CharacterModel _CM)
    {
        CharacterModel CM;
        GDEHeroData HD;
        /*
        HeroAnimImgList hail;
        CM = _CM;
        int hashcode = CM.hashcode;
        HD = SDDataManager.Instance.getHeroByHashcode(hashcode);
        GDEAnimData AD = HD.AnimData;
        if (AD.isRare) return;
        //
        hail = null;
        for (int i = 0; i < allHAIL.Length; i++)
        {
            if (allHAIL[i].Skeleton == AD.skeletonIndex)
            {
                hail = allHAIL[i];
                break;
            }
        }
        if (hail == null) return;
        //hair
        replaceSlotImg(AD.hair, "hair1",CM,HD,hail);
        //body
        replaceSlotImg(AD.body, "body",CM,HD,hail);
        */


    }
    public static void replaceSlotImg(string _region, string _slot
        ,CharacterModel CM, GDEHeroData HD, RoleSkeletonData hail)
    {
        GDEAnimData AD = HD.AnimData;
        if (AD.isRare) return;
        Chara_atlasRegionAttacher ara;
        if (!string.IsNullOrEmpty(_region))
        {
            RoleSkeletonData.SlotRegionPairList list 
                = hail.AllEnableList.Find(x => x.slot == _slot);
            RoleSkeletonData.SlotRegionPair pair = list.AllRegionList
                .Find(x => x.Region == _region);
            if (list != null && list.AtlasAsset != null)
            {

            }
        }
    }
}