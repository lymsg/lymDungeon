
using UnityEngine;

public class JumpGround : MonoBehaviour
{
    public float forceJumpPower;
    public void OnTriggerActivated(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.controller.ForceJump(forceJumpPower);
        }
    }
}
