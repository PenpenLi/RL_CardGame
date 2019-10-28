using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

#if GDE_PLAYMAKER_SUPPORT 

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(GDMConstants.ActionCategory)]
	[Tooltip(GDMConstants.SetBoolArrayActionTooltip)]
	public class GDESetBoolArray : GDESetActionBase
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
                List<bool> vals = Variable.boolValues != null ? Variable.boolValues.ToList() : null;
                GDEDataManager.SetBoolList(ItemName.Value, FieldName.Value, vals);
			}
			catch(UnityException ex)
			{
				LogError(string.Format(GDMConstants.ErrorSettingValue, GDMConstants.BoolType, ItemName.Value, FieldName.Value));
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

