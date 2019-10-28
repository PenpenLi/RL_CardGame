/// <summary>
/// Sounds used:
/// http://www.freesound.org/people/THE_bizniss/sounds/39459/
/// http://www.freesound.org/people/suonho/sounds/3176/
/// http://www.looperman.com/loops/detail/84579/distorted-reality-by-40a-free-80bpm-ambient-piano-loop
///
/// CC by 3.0: http://creativecommons.org/licenses/by/3.0/
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;

public class ReadDataScene : MonoBehaviour
{
    public Text BoolField;
    public Text BoolListField;
    public Text IntField;
    public Text IntListField;
    public Text FloatField;
    public Text FloatListField;
    public Text StringField;
    public Text StringListField;
    public Text Vector2Field;
    public Text Vector2LisField;
    public Text Vector3Field;
    public Text Vector3ListField;
    public Text Vector4Field;
    public Text Vector4ListField;
    public Text ColorField;
    public Text ColorListField;

    public Text GetAllDataBySchema;
    public Text GetAllKeysBySchema;

    public Text Status;

    public VideoPlayer vidPlayer;

    // This is the generated data class. These can be generated from the GDE menu
    GDEReadSceneData gde_data;

    void Start ()
    {
	// Initialize GDE
	// This will load the data file with this name. Do not specify the extension.
	// You can also pass a TextAsset instead of a string.
	if (!GDEDataManager.Init("read_scene_data"))
	{
	    Status.text = ReadDataSceneStrings.StatusErrorInitiliazing;
	    return;
	}

	// Here is how you load an item using the generated data class.
	// The constructor will load the item with ID "test_data" of the ReadScene schema.
	// The GDEReadSceneItemKeys class is also generated. It contains fields for every
	// item ID so that you can easily access item data without the need for hard coded
	// strings.
	gde_data = new GDEReadSceneData(GDEReadSceneItemKeys.ReadScene_test_data);
	if (gde_data == null)
	{
	    Status.text = ReadDataSceneStrings.StatusErrorLoadingData;
	    return;
	}

	try
	{
	    // The following shows how each field in the ReadScene schema is accessed.
	    // Each field in the schema will have a field of the correct type in the
	    // generated class.
	    // Lists are generic List<>, 2D lists are also generic List<List<>>
	    // You can modify these fields at will. To save your modifications to
	    // persistent data, call GDEDataManager.Save(); when it is convenient.
	    
	    // Bool
	    BoolField.text = ReadDataSceneStrings.BoolFieldLbl + " ";
	    BoolField.text += gde_data.bool_field;

	    // Bool List
	    BoolListField.text = ReadDataSceneStrings.BoolListFieldLbl + " ";
            foreach(bool boolVal in gde_data.bool_list_field)
		BoolListField.text += string.Format("{0} ", boolVal);

	    // Int
	    IntField.text = ReadDataSceneStrings.IntFieldLbl + " ";
	    IntField.text += gde_data.int_field;

	    // Int List
	    IntListField.text = ReadDataSceneStrings.IntListFieldLbl + " ";
            foreach(int intVal in gde_data.int_list_field)
		IntListField.text += string.Format("{0} ", intVal);

	    // Float
	    FloatField.text = ReadDataSceneStrings.FloatFieldLbl + " ";
            FloatField.text += gde_data.float_field;

	    // Float List
	    FloatListField.text = ReadDataSceneStrings.FloatListFieldLbl + " ";
            foreach(float floatVal in gde_data.float_list_field)
		FloatListField.text += string.Format("{0} ", floatVal);

	    // String
	    StringField.text = ReadDataSceneStrings.StringFieldLbl + " ";
            StringField.text += gde_data.string_field;

	    // String List
	    StringListField.text = ReadDataSceneStrings.StringListFieldLbl + " ";
            foreach(string stringVal in gde_data.string_list_field)
		StringListField.text += string.Format("{0} ", stringVal);

	    // Vector2
	    Vector2Field.text = ReadDataSceneStrings.Vec2FieldLbl + " ";
            Vector2Field.text += string.Format("({0}, {1})", gde_data.vector2_field.x, gde_data.vector2_field.y);

	    // Vector2 List
	    Vector2LisField.text = ReadDataSceneStrings.Vec2ListFieldLbl + " ";
            foreach(Vector2 vec2Val in gde_data.vector2_list_field)
		Vector2LisField.text += string.Format("({0}, {1}) ", vec2Val.x, vec2Val.y);

	    // Vector3
	    Vector3Field.text = ReadDataSceneStrings.Vec3FieldLbl + " ";
            Vector3Field.text += string.Format("({0}, {1}, {2})", gde_data.vector3_field.x, gde_data.vector3_field.y, gde_data.vector3_field.z);

	    // Vector3 List
	    Vector3ListField.text = ReadDataSceneStrings.Vec3ListFieldLbl + " ";
            foreach(Vector3 vec3Val in gde_data.vector3_list_field)
		Vector3ListField.text += string.Format("({0}, {1}, {2}) ", vec3Val.x, vec3Val.y, vec3Val.z);

	    // Vector4
	    Vector4Field.text = ReadDataSceneStrings.Vec4FieldLbl + " ";
            Vector4Field.text += string.Format("({0}, {1}, {2}, {3})", gde_data.vector4_field.x, gde_data.vector4_field.y, gde_data.vector4_field.z, gde_data.vector4_field.w);

	    // Vector4 List
	    Vector4ListField.text = ReadDataSceneStrings.Vec4ListFieldLbl + " ";
            foreach(Vector4 vec4Val in gde_data.vector4_list_field)
		Vector4ListField.text += string.Format("({0}, {1}, {2}, {3}) ", vec4Val.x, vec4Val.y, vec4Val.z, vec4Val.w);

	    // Color
	    ColorField.text = ReadDataSceneStrings.ColorFieldLbl + " ";
            ColorField.text += gde_data.color_field.ToString();

	    // Color List
	    ColorListField.text = ReadDataSceneStrings.ColorListFieldLbl + " ";
            foreach(Color colVal in gde_data.color_list_field)
		ColorListField.text += string.Format("{0}   ", colVal);

	    // Custom
	    // See LoadUnityTypes for a Custom field example

	    LoadUnityTypes();

	    // There is an updated generic method that will return all items
	    // of a particular schema. It looks like this:
	    // List<GDEReadSceneData> all = GDEDataManager.GetAllItems<GDEReadSceneData>();
	    // Specify the data class corresponding to the schema whose items you
	    // want to load all at once


	    /**
	     *
	     * The following two methods (GetAllDataBySchema and GetAllKeysBySchema) are part of the old version of the GDE API.
	     * They still work, but require a little more code to use.
	     *
	     **/

	    // Get All Data By Schema
            GetAllDataBySchema.text = ReadDataSceneStrings.GetAllBySchemaLbl + Environment.NewLine;
	    Dictionary<string, object> allDataByCustomSchema;
	    GDEDataManager.GetAllDataBySchema("ReadSceneCustom", out allDataByCustomSchema);
	    foreach(KeyValuePair<string, object> pair in allDataByCustomSchema)
	    {
		string description;
		Dictionary<string, object> customData = pair.Value as Dictionary<string, object>;
		customData.TryGetString("description", out description);
		GetAllDataBySchema.text += string.Format("     {0}{1}", description, Environment.NewLine);
	    }

	    // Get All Keys By Schema
	    GetAllKeysBySchema.text = ReadDataSceneStrings.GetAllKeysBySchemaLbl + " ";
	    List<string> customKeys;
	    GDEDataManager.GetAllDataKeysBySchema("ReadSceneCustom", out customKeys);
	    foreach(string key in customKeys)
		GetAllKeysBySchema.text += string.Format("{0} ", key);

	    Status.text = ReadDataSceneStrings.StatusSuccess;
	}
	catch(Exception ex)
	{
	    Status.text = ReadDataSceneStrings.StatusError;
	    Debug.LogError(ex);
	}
    }

    void LoadUnityTypes()
    {
	// I created simple prefabs to demonstrate how to use unity assets.
	// Note that any unity assets must be in a Resources folder or subfolder.
	// You cannot load unity assets from AssetBundles. However, you can
	// script a helper that will load an asset from a bundle and the asset
	// path is retrieved from GDE.
	
	// GameObject
	// This is the light
	if (gde_data.custom_field.go_field != null)
	    Instantiate(gde_data.custom_field.go_field);

	// Game Object List
	// This list contains the cube and sphere
	GameObject cube = Instantiate(gde_data.custom_field.go_list_field[0]) as GameObject;
	GameObject sphere = Instantiate(gde_data.custom_field.go_list_field[1]) as GameObject;

	// Texture2D
	Renderer rendererCmp = cube.GetComponent<Renderer>();
	if (rendererCmp)
	    rendererCmp.material.mainTexture = gde_data.custom_field.tex_field;

	// Material
	rendererCmp = sphere.GetComponent<Renderer>();
	if (rendererCmp)
	    rendererCmp.material = gde_data.custom_field.mat_field;

	// AudioClip
	AudioSource audioSource = GetComponent<AudioSource>();
	if (audioSource == null)
	    audioSource = gameObject.AddComponent<AudioSource>();

	audioSource.loop = true;
	audioSource.clip = gde_data.custom_field.aud_field;
	audioSource.PlayDelayed(1);

	// VideoClip
	vidPlayer.clip = gde_data.custom_field.vid_field;
	vidPlayer.Play();

	// Material, Texture2D, and AudioClip lists work exactly like the GameObject list above
    }
}
