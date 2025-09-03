using ArabicSupport;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerInput : MonoBehaviour
{
    public DialogueController dialogueController; 
    public float activationRange = 3f;
    public GameObject EButton;
    private Transform playerTransform;
    private bool triggered = false;
    public DialogueState state;
    public TMP_Text errorText;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || dialogueController == null || triggered)
            return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        Debug.Log(DialogueController.currentState);
        if (distance <= activationRange && Input.GetKeyDown(KeyCode.E) && state == DialogueController.currentState)
        {
            if(state == DialogueState.Dialogue2)
            {
                if(CoinCollectors.CollectedCoins < 4)
                {
                    errorText.text = ArabicFixer.Fix("لا يمكنني مساعدتك حتى تجمع كل النقود يا ولدي", showTashkeel: true, useHinduNumbers: true);
                }
                else
                {
                    dialogueController.gameObject.SetActive(true);
                    EButton.SetActive(false);
                    triggered = true;
                    errorText.text = "";    
                }
            }
            else
            {
                dialogueController.gameObject.SetActive(true);
                EButton.SetActive(false);
                triggered = true;
            }
        }
        else
        {
            if(state == DialogueState.Dialogue2) 
                errorText.text = "";
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
