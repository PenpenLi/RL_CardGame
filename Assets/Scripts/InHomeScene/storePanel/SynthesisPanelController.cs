using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynthesisPanelController : MonoBehaviour
{
    public ScrollRect scollrect;
    public Transform SynthesisCard;

    public void WhenOpenThisPanel()
    {
        scollrect.horizontalNormalizedPosition = 0;
    }

    public void CloseThisPanel()
    {
        UIEffectManager.Instance.hideAnimFadeOut(transform);
    }


}
