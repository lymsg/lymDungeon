using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }

    

    private void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        // if(health.curValue < 0f)
        // {
        //     Die();
        // }
    }
    public void StartInfiniteStamina(float time)
    {
        StartCoroutine(InfiniteStamina(time));
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }


    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public bool useStamina(float amount)
    {
        if (stamina.curValue < amount)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }

    private IEnumerator InfiniteStamina(float time)
    {
        float startTime = 0f;
        
        while (startTime <= time)
        {
            startTime += Time.deltaTime;
            stamina.infinite();
            yield return null;
        }

        stamina.uiBar.color = new Color32(66, 117, 5, 255);
    }
}
