using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshAllPropsZ : MonoBehaviour
{
    public PlayerControl BaseData;
    // Start is called before the first frame update
    void Start()
    {
        var all = GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < all.Length; i++)
        {
            if (all[i].transform.parent != null 
                && !all[i].GetComponentInParent<PlayerControl>() 
                && !all[i].transform.parent.GetComponent<SpriteRenderer>())
            {
                all[i].transform.parent.localPosition 
                    = new Vector3(all[i].transform.parent.position.x
                    , all[i].transform.parent.position.y
                    , BaseData.PlayerZ(all[i].transform.parent));
            }
        }
    }

}
