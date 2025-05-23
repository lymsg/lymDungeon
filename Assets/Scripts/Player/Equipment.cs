using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipBack;

    private PlayerController controller;
    private PlayerCondition condition;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipBack).GetComponent<Equip>();
        Rigidbody rb = curEquip.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;      // 중력 끄기
            rb.isKinematic = true;      // 물리 계산 제거 (충돌/힘 없음)
        }
        EquipTool tool = curEquip as EquipTool;
        if (tool != null)
        {
            condition.AddBonusHealth(tool.BonusHealth);
            controller.DecreaseSpeed(tool.decreaseSpeed);
        }
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            EquipTool tool = curEquip as EquipTool;
            if (tool != null)
            {
                condition.RemoveBonusHealth(tool.BonusHealth);
                controller.UnDecreaseSpeed(tool.decreaseSpeed);
            }
            
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }
}

