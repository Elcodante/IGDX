using UnityEngine;

// Enum 6 Karakter
public enum CharacterType
{
    Anak,
    Kuntilanak,
    Pocong,
    Genderuwo,
    WeweGombel,
    Tuyul
}

[System.Serializable]
public struct DialogueLine
{
    public CharacterType speaker;
    [TextArea(2, 5)] public string message;
}

[CreateAssetMenu(fileName = "NewCharacterDialogue", menuName = "Dialogue/Character Dialogue Data")]
public class CharacterDialogueData : ScriptableObject
{
    public CharacterType characterOwner;
    public int requiredLevel;   

    [Header("Dialogue Content")]
    public bool isDialogueUnlockedDoneReading = false;
    public DialogueLine[] unlockedDialogue;
    public bool isDialogueLockedDoneReading = false;
    public DialogueLine[] lockedDialogue;
}