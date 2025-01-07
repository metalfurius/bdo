using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class Quit : MonoBehaviour
    {
        public void Restart()
        {
            // Recargar la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}