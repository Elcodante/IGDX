using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private bool isDialogueLocked = true;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Image portraitImage; 
    [SerializeField] private GameObject panelMulaiGameplay;

    [Header("Character Sprites (1 Sprite per Character)")]
    [SerializeField] private Sprite playerSprite;
    [SerializeField] private Sprite kuntilanakSprite;
    [SerializeField] private Sprite pocongSprite;
    [SerializeField] private Sprite genderuwoSprite;
    [SerializeField] private Sprite weweGombelSprite;
    [SerializeField] private Sprite tuyulSprite;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (panelMulaiGameplay != null)
        {
            panelMulaiGameplay.SetActive(false);
        }

        skipButton.SetActive(false);
    }

    public void StartCharacterDialogue(CharacterDialogueData data, int playerCurrentLevel)
    {
        dialogueQueue.Clear();
        playerMovement.SetCanMove(false);

       DialogueLine[] selectedLines;

       if(data.isDialogueLockedDoneReading || data.isDialogueUnlockedDoneReading)
        {
            skipButton.SetActive(true);
        }else
        {
            skipButton.SetActive(false);
        }

        if (playerCurrentLevel >= data.requiredLevel)
        {
            isDialogueLocked = false;
            selectedLines = data.unlockedDialogue;
            data.isDialogueUnlockedDoneReading = true;
        }
        else
        {
            isDialogueLocked = true; 
            selectedLines = data.lockedDialogue;
            data.isDialogueLockedDoneReading = true;
        }

        foreach (DialogueLine line in selectedLines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);

        
        if (panelMulaiGameplay != null)
        {
            panelMulaiGameplay.SetActive(false);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();

        speakerText.text = FormatCharacterName(currentLine.speaker);
        if (portraitImage != null)
        {
            portraitImage.sprite = GetCharacterSprite(currentLine.speaker);
        }
        messageText.text = currentLine.message;
    }

    public void EndDialogue()
    {
        if(!isDialogueLocked)
        {
            dialoguePanel.SetActive(false);
            if (panelMulaiGameplay != null)
            {
                panelMulaiGameplay.SetActive(true);
            }
        }
        else
        {
            dialoguePanel.SetActive(false);
            playerMovement.SetCanMove(true);
        }
    }


    private Sprite GetCharacterSprite(CharacterType character)
    {
        switch (character)
        {
            case CharacterType.Anak: return playerSprite;
            case CharacterType.Kuntilanak: return kuntilanakSprite;
            case CharacterType.Pocong: return pocongSprite;
            case CharacterType.Genderuwo: return genderuwoSprite;
            case CharacterType.WeweGombel: return weweGombelSprite;
            case CharacterType.Tuyul: return tuyulSprite;
            default: return null;
        }
    }
    private string FormatCharacterName(CharacterType character)
    {
        if (character == CharacterType.WeweGombel) return "Wewe Gombel";
        return character.ToString();
    }
}