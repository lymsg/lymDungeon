using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;
    [CanBeNull] public Image bonusBar;
    public UICondition uiCondition;
    private void Start()
    {
        curValue = startValue;
        bonusBar?.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GetPercentage() <= 1f)
        {
            uiBar.fillAmount = GetPercentage();
        }
        else if (GetPercentage() > 1f && bonusBar != null)
        {
            bonusBar.fillAmount = GetPercentage() - 1f;
        }

        if (curValue <= maxValue && bonusBar != null && uiCondition != null)
        {
            uiCondition.RemoveBonusBarUi();
        }
    }

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    public void infinite()
    {
        curValue = maxValue;
        uiBar.color=new Color32(255, 233, 90, 255);
    }
    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}
