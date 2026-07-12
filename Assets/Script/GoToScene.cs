using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public void GoScene(string namaScene)
    {
        SceneManager.LoadScene(namaScene);
    }
}
