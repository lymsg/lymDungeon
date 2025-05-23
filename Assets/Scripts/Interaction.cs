using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;
    
    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public GameObject prompt;
    [SerializeField]private TextMeshProUGUI promptText;
    public Image keep;
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
        promptText = prompt.GetComponentInChildren<TextMeshProUGUI>();
        
    }

    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if(hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText(layerMask);
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                prompt.gameObject.SetActive(false);
                keep.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText(LayerMask layerMask)
    {
        prompt.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
        if (curInteractGameObject.layer == LayerMask.NameToLayer("Interactable")) 
        {
            keep.gameObject.SetActive(true);
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
        }
    }
}