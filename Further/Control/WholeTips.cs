using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 调用全局提示，只需要调用InputTips动态赋值即可
/// </summary>
public class WholeTips : MonoBehaviour
{
    /// <summary>
    /// UI组件
    /// </summary>
    public GameObject Fork;
    public Text ForkTip;
    public GameObject Caution;
    public Text CautionTip;
    public GameObject Tips;
    public Text TipsTip;
    /// <summary>
    /// 操作变量.
    /// </summary>
    public string TipsText;
    public string CautionOrFalse;
    public float Timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WholeWorldTips();
    }

    public void WholeWorldTips()
    {
        if (TipsText != null && CautionOrFalse != null && Timer != 0f)
        {
            if (CautionOrFalse == "Caution")
            {
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    Caution.SetActive(true);
                    CautionTip.text = TipsText;
                }
                if (Timer <= 0)
                {
                    Caution.SetActive(false);
                    InitializationTips();
                }
            }
            else if (CautionOrFalse == "False")
            {
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    Fork.SetActive(true);
                    ForkTip.text = TipsText;
                }
                if (Timer <= 0)
                {
                    Fork.SetActive(false);
                    InitializationTips();
                }
            }
            else if (CautionOrFalse == "Tips")
            {
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    Tips.SetActive(true);
                    TipsTip.text = TipsText;
                }
                if (Timer <= 0)
                {
                    Tips.SetActive(false);
                    InitializationTips();
                }
            }
        }
    }
    /// <summary>
    /// 调用全局错误信息提醒按钮
    /// </summary>
    /// <param name="CTF">Caution或者False或者Tips</param>
    /// <param name="Texture">要提示的内容</param>
    /// <param name="T">显示时长，内容较少时建议2f</param>
    public void InputTips(string CTF, string Texture, float T)
    {
        CautionOrFalse = CTF;
        TipsText = Texture;
        Timer = T;
    }
    public void InitializationTips()
    {
        TipsText = null;
        CautionOrFalse = null;
        Timer = 0f;
    }
}
