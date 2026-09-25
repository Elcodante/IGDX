using UnityEngine;

public class MinyakAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _MinyakAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Minyak()
    {
        _MinyakAnimation.SetActive(true);
    }
    // Update is called once per frame
    public void MatikanMinyak()
    {
        gameObject.SetActive(false);
    }
}
