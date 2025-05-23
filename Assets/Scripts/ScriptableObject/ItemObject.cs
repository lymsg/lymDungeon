using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n\n{data.description}";
        return str;
    }

    public void OnInteract()
    {
        //Player 스크립트 먼저 수정
        if (data.type == ItemType.Consumable )
        {
            switch (data.displayName)
            {
                case "초록토마토":
                    CharacterManager.Instance.Player.condition.StartInfiniteStamina(data.time);
                    break;
				case "사과":
                    CharacterManager.Instance.Player.controller.StartOnDoubleJump(data.time);
                    break;
                    
            }
        }
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
