using UnityEngine;

public class UIButton : MonoBehaviour
{
    public void ClickSound()
    {
        AudioManager.instance.PlaySFXClick();
    }
}
