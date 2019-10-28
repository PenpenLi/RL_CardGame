using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.SetFloatArrayActionTooltip)]
	public class GDESetFloatArray : GDESetActionBase
	{   
		[UIHint(UIHint.Variable)]
		public FsmArray Variable;
		
		public override void Reset()
		{
			base.Reset();
			
			if (Variable != null)
				Variable.Reset();
		}
		
		public override void OnEnter()
		{
            base.OnEnter();
            
			try
			{
				List<float> vals = Variable.floatValues != null ? Variable.floatValues.ToList() : null;
				GDEDataManager.SetFloatList(ItemName.Value, FieldName.Value, vals);
			}
			catch(UnityException ex)
			{
				LogError(string.Format(GDMConstants.ErrorSettingValue, GDMConstants.FloatType, ItemName.Value, FieldName.Value));
				LogError(ex.ToString());
			}
			finally
			{
				Finish();
			}
		}
	}
}

#endif

