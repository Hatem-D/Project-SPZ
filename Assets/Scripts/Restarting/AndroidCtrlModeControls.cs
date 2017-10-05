using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidCtrlModeControls : MonoBehaviour
{
    public SPZController spz;
    public Text ctrlModeTextComp;

    public GameObject FreeMoveParametersUI;
    public GameObject LinesParametersUI;
    public GameObject JetpackParametersUI;

    public Text swimUpAngleTxt;
    public Text swimUpOffsetTxt;
    public Text swimDownOffsetTxt;
    public Text swimUpSpeedTxt;
    public Text swimDownSpeedTxt;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ctrlModeTextComp.text = spz.GetCtrlMode().ToString();
    }

    void OnEnable()
    {
        UpdateUIText();
    }
    
    void UpdateUIText()
    {
        

        switch (spz.GetCtrlMode())
        {
            case SPZController.controllerModes.FreeMove:
                break;
            case SPZController.controllerModes.Lines:
                break;
            case SPZController.controllerModes.Jetpack:
                swimUpOffsetTxt.text = spz.swimUpOffset.ToString();
                swimDownOffsetTxt.text = spz.swimDownOffset.ToString();
                swimUpSpeedTxt.text = spz.swimUpSpeed.ToString();
                swimDownSpeedTxt.text = spz.swimDownSpeed.ToString();
                break;
            default:
                break;
        }        
    }

    //-----------------Ctrl Mode methods
    public void SetPreviousCtrlMode()
    {
        spz.SetPreviousCtrlMode();
        SelectUIByCtrlMode();
    }
    public void SetNextCtrlMode()
    {
        spz.SetNextCtrlMode();
        SelectUIByCtrlMode();
    }
    void SelectUIByCtrlMode()
    {
        switch (spz.GetCtrlMode())
        {
            case SPZController.controllerModes.FreeMove:
                FreeMoveParametersUI.SetActive(true);
                LinesParametersUI.SetActive(false);
                JetpackParametersUI.SetActive(false);
                break;
            case SPZController.controllerModes.Lines:
                FreeMoveParametersUI.SetActive(false);
                LinesParametersUI.SetActive(true);
                JetpackParametersUI.SetActive(false);
                break;
            case SPZController.controllerModes.Jetpack:
                FreeMoveParametersUI.SetActive(false);
                LinesParametersUI.SetActive(false);
                JetpackParametersUI.SetActive(true);
                break;
            default:
                FreeMoveParametersUI.SetActive(false);
                LinesParametersUI.SetActive(false);
                JetpackParametersUI.SetActive(false);
                break;

        }
        UpdateUIText();
    }

    #region Jetpack Style Controls
    //-----------------Swim Up Angle methods
    public void SwimUpAngleUp()
    {
        spz.swimUpAngle += 0.5f;
        UpdateUIText();
    }
    public void SwimUpAngleDown()
    {
        spz.swimUpAngle -= 0.5f;
        UpdateUIText();
    }

    //-----------------Swim Up Offset methods    
    public void SwimUpOffsetUp()
    {
        spz.swimUpOffset += 0.5f;
        UpdateUIText();
    }
    public void SwimUpOffsetDown()
    {
        spz.swimUpOffset -= 0.5f;
        UpdateUIText();
    }

    //-----------------Swim Down Offset methods
    public void SwimDownOffsetUp()
    {
        spz.swimDownOffset += 0.5f;
        UpdateUIText();
    }
    public void SwimDownOffsetDown()
    {
        spz.swimDownOffset -= 0.5f;
        UpdateUIText();
    }

    //-----------------Swim Up Speed methods
    public void SwimUpSpeedUp()
    {
        spz.swimUpSpeed += 0.5f;
        UpdateUIText();
    }
    public void SwimUpSpeedDown()
    {
        spz.swimUpSpeed += 0.5f;
        UpdateUIText();
    }

    //-----------------Swim Down Speed methods
    public void SwimDownSpeedUp()
    {
        spz.swimDownSpeed += 0.5f;
        UpdateUIText();
    }
    public void SwimDownSpeedDown()
    {
        spz.swimDownSpeed -= 0.5f;
        UpdateUIText();
    }

    //-----------------Fall Acceleration Rate methods
    public void FallAccelerationRateUp()
    {
        spz.fallAccelerationRate += 0.1f;
        UpdateUIText();
    }
    public void FallAccelerationRateDown()
    {
        spz.fallAccelerationRate -= 0.1f;
        UpdateUIText();
    }

    #endregion
}
