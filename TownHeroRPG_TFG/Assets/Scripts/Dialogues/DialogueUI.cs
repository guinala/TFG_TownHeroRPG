using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("Audio Configuration")]
    public float textSpeed;

    [Header("Dialogue Dependencies")]
    public GameObject leftCharacter;
    public GameObject rightCharacter;
    public TextMeshProUGUI leftCharacterName;
    public Image leftCharacterPortrait;
    public TextMeshProUGUI rightCharacterName;
    public Image rightCharacterPortrait;
    public TextMeshProUGUI dialogueBox;

    private string _currentSenence;

    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private DialogueAudioInfoSO defaultAudio;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
    private DialogueAudioInfoSO currentAudio;
    [SerializeField] private bool makePredictable = false;

    

    private void Awake()
    {
        Instance = this;
        audioSource = this.gameObject.GetComponent<AudioSource>();
        if(defaultAudio.id == null)
        {
            Debug.Log("Aqui es donde falla");
        }
        InitializeAudioDictionary();
        currentAudio = defaultAudio;
        
    }


    public void StartConversation(
        string leftCharacterName,
        Sprite leftCharacterPortrait,
        string rightCharacterName,
        Sprite rightCharacterPortrait)
    {
        // Reset UI
        this.CleanUI();

        // Set images and names
        this.leftCharacterName.text = leftCharacterName;
        this.leftCharacterPortrait.sprite = leftCharacterPortrait;
        this.rightCharacterName.text = rightCharacterName;
        this.rightCharacterPortrait.sprite = rightCharacterPortrait;

        // Clean dialogue just in case
        this.dialogueBox.text = "";

        // Hide everything
        this.ToggleLeftCharacter(false);
        this.ToggleRightCharacter(false);
        SetCurrentAudioinfo(defaultAudio.id);
    }

    public void DisplaySentence(string characterName, string sentenceText, string audioID)
    {
        if (characterName == leftCharacterName.text)
        {
            // Left character is talking
            this.ToggleLeftCharacter(true);
            this.ToggleRightCharacter(false);
        }
        else
        {
            // Right character is talking
            this.ToggleLeftCharacter(false);
            this.ToggleRightCharacter(true);
        }

        this._currentSenence = sentenceText;
        SetCurrentAudioinfo(audioID);
        StartCoroutine(TypeCurrentSentence());
    }

    public void EndConversation()
    {
        this.CleanUI();
        //Go back to default audio
        SetCurrentAudioinfo(defaultAudio.id);
    }

    public bool IsSentenceInProcess()
    {
        return this._currentSenence != null;
    }

    public void FinishDisplayingSentence()
    {
        StopAllCoroutines();
        this.dialogueBox.text = this._currentSenence;
        this._currentSenence = null;
    }

    // Private Logic

    private IEnumerator TypeCurrentSentence()
    {
        this.dialogueBox.text = "";
        

        foreach (char letter in this._currentSenence.ToCharArray())
        {
            if (audioSource != null)
            {
                Debug.Log("Quiero reproducir");
                PlayDialogueSound(dialogueBox.text.Length, letter); //Quiza tambi�n se puede usar dialogueBox.text.Length
            }

            this.dialogueBox.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        this.dialogueBox.text = this._currentSenence;
        this._currentSenence = null;
    }


    private void PlayDialogueSound(int currentDisplayedCharacter, char currentCharacter)
    {
        //set variables from config SO
        AudioClip[] dialogueSounds = currentAudio.dialogueSounds;
        bool stopAudio = currentAudio.stopAudio;
        int frequencyLevel = currentAudio.frequencyLevel;
        float minPitch = currentAudio.minPitch;
        float maxPitch = currentAudio.maxPitch;

        


        if(currentDisplayedCharacter % frequencyLevel == 0)
        {
            if (stopAudio)
            {
                audioSource.Stop();
            }
            AudioClip clip = null;

            //Create predictable audio with hash codes
            if(makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                //Select sound clip
                int predictableIndexSound = hashCode % dialogueSounds.Length;
                clip = dialogueSounds[predictableIndexSound];
                //Pitch
                int maxPitchInt = (int)(maxPitch * 100);
                int minPitchInt = (int)(minPitch * 100);
                int pinchRangeInt = maxPitchInt - minPitchInt;
                //Cannot divide by 0, so if there is no range we skip the selection
                if (pinchRangeInt != 0)
                {
                    int predictablePitch_I = (hashCode % pinchRangeInt) + minPitchInt;
                    float predictablePitch_F = predictablePitch_I / 100f;
                    audioSource.pitch = predictablePitch_F;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            //Otherwise, play random sound
            else
            {
                //Select Sound
                int randomSound = Random.Range(0, dialogueSounds.Length);
                clip = dialogueSounds[randomSound];

                //Pitch
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }



            

            //Play Sound
            audioSource.PlayOneShot(clip);
        }
    }

    public void InitializeAudioDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudio.id, defaultAudio);
        foreach(DialogueAudioInfoSO audioInfo in audioInfos)
        {
            Debug.Log("Hola me llamo: " + audioInfo.id + "y tengo: " + audioInfo.dialogueSounds.Length + " sonidos");
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }

        Debug.Log("He creado el diccionario de audios");
    }

    private void SetCurrentAudioinfo(string id)
    {
        Debug.Log("y el diccionario tiene: " + audioInfoDictionary.Count + " elementos");
        DialogueAudioInfoSO info = null;
        audioInfoDictionary.TryGetValue(id, out info);
        if(info != null)
        {
            this.currentAudio = info;
        }
        else
        {
            Debug.LogWarning("Audio Info not found for id: " + id);
        }
    }

    private void CleanUI()
    {
        this.leftCharacterName.text = "";
        this.leftCharacterPortrait.sprite = null;
        this.rightCharacterName.text = "";
        this.rightCharacterPortrait.sprite = null;

        this.dialogueBox.text = "";
    }

    private void ToggleLeftCharacter(bool status)
    {
        this.leftCharacter.SetActive(status);
    }

    private void ToggleRightCharacter(bool status)
    {
        this.rightCharacter.SetActive(status);
    }
}
