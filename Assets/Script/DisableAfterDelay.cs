using System.Collections;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 3.0f;

    private void OnEnable()
    {
        
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
   
        yield return new WaitForSeconds(delaySeconds);

   
        gameObject.SetActive(false);
    }
}