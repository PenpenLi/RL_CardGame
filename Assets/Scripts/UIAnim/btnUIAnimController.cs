using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class btnUIAnimController : MonoBehaviour
{
    public enum btnUIAnimTag
    {
        bounce,
        none,
    }
    public btnUIAnimTag AnimTAG = btnUIAnimTag.none;
    public Button btnSelf
    {
        get { return GetComponent<Button>(); }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(AnimTAG != btnUIAnimTag.none)
        {
            if (AnimTAG == btnUIAnimTag.bounce)
            {
                btnSelf.onClick.AddListener
                    (delegate ()
                    {
                        UIEffectManager.Instance.bounceAnim(transform);
                    }
                    );
            }
        }
    }
}
