using UnityEngine;

public class DebugClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Menembakkan raycast manual dari posisi mouse
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Mouse Anda sebenarnya menabrak objek: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Mouse Anda tidak menabrak objek ber-collider apa pun di dunia 2D.");
            }
        }
    }
}
