
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    private JumpGround _jumpGround;
    void Start()
    {
        _jumpGround = GetComponentInParent<JumpGround>();
    }

    void OnCollisionEnter(Collision other)
    {
        _jumpGround.OnTriggerActivated(other.gameObject);
    }
}
