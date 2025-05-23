using UnityEngine;
using UnityEngine.UI;


public class UICondition : MonoBehaviour
    {
        public Condition health;
        public Condition stamina;
        public Image doubleJumpIcon;
        private void Start()
        {
            CharacterManager.Instance.Player.condition.uiCondition = this;
        }

        void Update()
        {
            if (CharacterManager.Instance.Player.controller.doubleJumpAble)
            {
                doubleJumpIcon.gameObject.SetActive(true);
            }
            else
            {
                doubleJumpIcon.gameObject.SetActive(false);
            }
        }
        public void AddBonusBarUi()
        {
            health.bonusBar?.gameObject.SetActive(true);
        }

        public void RemoveBonusBarUi()
        {
            health.bonusBar?.gameObject.SetActive(false);
        }
    }

