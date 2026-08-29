using UnityEngine;

public class SetLevel : MonoBehaviour
{
    [SerializeField] private int levelIndex;

    public void Set(int levelIndex)
    {
        PlayerPrefs.SetInt("PlayerLevel", levelIndex);
    }
}
