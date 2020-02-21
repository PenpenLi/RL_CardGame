using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PerAttritubeBox : MonoBehaviour
{
    //public Text NameText;
    public Image ICON;
    public Text FigureText;
    public Color basicFigureTextColor;
    public Color decreaseFigureTextColor;
    public Color increaseFigureTextColor;
    public OneAttritube DATA = new OneAttritube();
    private float figureChangeAnimInterval = 0.15f;
    #region AD
    public void initThisBoxInAD(int Ra,AttributeData tag)
    {
        DATA.isAD = true;DATA.index = (int)tag;
        //
        Sprite s = SDDataManager.Instance.GetIconInRAL(tag);
        if (s)
        { 
            ICON.sprite = s;
            ICON.SetNativeSize();
        }
        //
        //NameText.text = SDGameManager.T(tag.ToString());
        RefreshData_AD(Ra);
    }

    public void RefreshData_AD(int RA)
    {
        StartCoroutine(FigureChangeAnim(RA));
    }


    #endregion
    #region SR
    public void initThisBoxInSR(int Ra, StateTag tag)
    {
        DATA.isAD = false;DATA.index = (int)tag;
        //
        Sprite s = SDDataManager.Instance.GetIconInRAL(tag);
        if (s)
        {
            ICON.sprite = s;
            ICON.SetNativeSize();
        }
        //
        //NameText.text = SDGameManager.T(tag.ToString());
        RefreshData_SR(Ra);
    }
    public void RefreshData_SR(int RA)
    {
        StartCoroutine(FigureChangeAnim(RA));
    }
    #endregion

    IEnumerator FigureChangeAnim(int newFigure)
    {
        int oldFigure = DATA.figure;
        FigureText.color = basicFigureTextColor;
        if(newFigure > oldFigure)
        {
            int length = newFigure - oldFigure;
            FigureText.color = increaseFigureTextColor;
            FigureText.text = oldFigure + "+" + length;
            yield return new WaitForSeconds(figureChangeAnimInterval);
        }
        else if(newFigure < oldFigure)
        {
            int length = oldFigure - newFigure;
            FigureText.color = decreaseFigureTextColor;
            FigureText.text = oldFigure + "-" + length;
            yield return new WaitForSeconds(figureChangeAnimInterval);
        }
        FigureText.text = "" + newFigure;
        FigureText.color = basicFigureTextColor;
    }

}
