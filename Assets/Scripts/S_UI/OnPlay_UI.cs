using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlay_UI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 读取Json中Item内容
    /// </summary>
    [ContextMenu("TestLoadConfig")]
    public void TestLeadConfig()
    {
        NewItemManager.Instance.ReadData();
    }


// Update is called once per frame
void Update()
    {
        
    }
}
