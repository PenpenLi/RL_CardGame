// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the Game Data Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//
//      This file was generated from this data file:
//      E:\RL_CardGame_Build\Assets/GameDataEditor/Resources/gde_data.txt
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections.Generic;

using GameDataEditor;

namespace GameDataEditor
{
    public class GDEMissionData : IGDEData
    {
        static string IDKey = "ID";
		string _ID;
        public string ID
        {
            get { return _ID; }
            set {
                if (_ID != value)
                {
                    _ID = value;
					GDEDataManager.SetString(_key, IDKey, _ID);
                }
            }
        }

        public GDEMissionData(string key) : base(key)
        {
            GDEDataManager.RegisterItem(this.SchemaName(), key);
        }
        public override Dictionary<string, object> SaveToDict()
		{
			var dict = new Dictionary<string, object>();
			dict.Add(GDMConstants.SchemaKey, "Mission");
			
            dict.Merge(true, ID.ToGDEDict(IDKey));
            return dict;
		}

        public override void UpdateCustomItems(bool rebuildKeyList)
        {
        }

        public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
        {
            _key = dataKey;

			if (dict == null)
				LoadFromSavedData(dataKey);
			else
			{
                dict.TryGetString(IDKey, out _ID);
                LoadFromSavedData(dataKey);
			}
		}

        public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			
            _ID = GDEDataManager.GetString(_key, IDKey, _ID);
        }

        public GDEMissionData ShallowClone()
		{
			string newKey = Guid.NewGuid().ToString();
			GDEMissionData newClone = new GDEMissionData(newKey);

            newClone.ID = ID;

            return newClone;
		}

        public GDEMissionData DeepClone()
		{
			GDEMissionData newClone = ShallowClone();
            return newClone;
		}

        public void Reset_ID()
        {
            GDEDataManager.ResetToDefault(_key, IDKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(IDKey, out _ID);
        }

        public void ResetAll()
        {
             #if !UNITY_WEBPLAYER
             GDEDataManager.DeregisterItem(this.SchemaName(), _key);
             #else

            GDEDataManager.ResetToDefault(_key, IDKey);


            #endif

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            LoadFromDict(_key, dict);
        }
    }
}