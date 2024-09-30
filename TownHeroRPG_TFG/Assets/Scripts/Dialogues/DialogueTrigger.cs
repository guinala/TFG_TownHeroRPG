using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ConversationSO conversation;
    //public ConversationSO normal_conversation;

    public void setConversation(ConversationSO conversation)
    {
        Debug.Log("stand proud you are strong");
        this.conversation = conversation;
        TriggerConversationQuest();
    }

    public void TriggerConversationQuest()
    {
        //this.conversationRequestEvent.Raise(this.conversation);
    }

    public void TriggerConversationNormal()
    {
        //this.conversationRequestEvent.Raise(this.normal_conversation);
        DialogueManager.Instance.StartConversation(conversation);
        Debug.Log("Hecho");
    }


}