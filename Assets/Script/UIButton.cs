using UnityEngine;

public class UIButton : MonoBehaviour
{
    public void ClickSound()
    {
        AudioManager.instance.PlaySFXClick();
    }

     public void GhostSound()
    {
        AudioManager.instance.PlaySFXGhost();
    }

     public void TingSound()
    {
        AudioManager.instance.PlaySFXTing();
    }
}
