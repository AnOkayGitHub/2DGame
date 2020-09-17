using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    
    public TextMeshProUGUI speakerName, dialogue, buttonText;
    public Image speakerSprite;
    public bool inSpeech = false;
    public GameObject npc;
    public bool isStopped = false;

    private static DialogueManager instance;
    private Conversation currentConversation;
    private Animator anim;
    private int currentIndex;
    private Coroutine typing;

    private void Awake() {
        if(instance == null) {
            instance = this;
            anim = this.GetComponent<Animator>();
        } else {
            Destroy(this);
        }
    }

    public static void StartConversation(Conversation conversation) {
        instance.inSpeech = true;
        instance.anim.SetBool("isOpen", true);
        instance.currentConversation = conversation;
        instance.speakerName.text = "";
        instance.dialogue.text = "";
        instance.buttonText.text = ">";
        instance.currentIndex = 0;
        instance.ReadNext();
    }

    public void ReadNext() {
        if(currentIndex > currentConversation.GetLength() || isStopped) {
            instance.anim.SetBool("isOpen", false);
            instance.inSpeech = false;
            instance.isStopped = false;
            return;
        }

        speakerName.text = currentConversation.GetLineByIndex(currentIndex).speaker.GetName();
        speakerSprite.sprite = currentConversation.GetLineByIndex(currentIndex).speaker.GetSprite()[currentConversation.GetLineByIndex(currentIndex).portraitIndex];

        if(typing == null) {
            typing = instance.StartCoroutine(TypeText(currentConversation.GetLineByIndex(currentIndex).dialogue));
        } else {
            instance.StopCoroutine(typing);
            typing = null;
            typing = instance.StartCoroutine(TypeText(currentConversation.GetLineByIndex(currentIndex).dialogue));
        }

        currentIndex++;

        if(currentIndex > currentConversation.GetLength()) {
            buttonText.text = "X";
        }
    }

    public static bool GetSpeechStatus() {
        return instance.inSpeech;
    }
  
    private IEnumerator TypeText(string text) {
        dialogue.text = "";
        bool isFinished = false;
        int index = 0;

        while(!isFinished) {
            dialogue.text += text[index];
            index++;

            yield return new WaitForSeconds(0.02f);

            if(index == text.Length) {
                isFinished = true;
            }
        }

        typing = null;
    }
}
