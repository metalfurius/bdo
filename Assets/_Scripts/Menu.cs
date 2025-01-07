using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts
{
    public class Menu : MonoBehaviour
    {
        public Button basicMode;
        public Button advancedMode;
        public GameObject basicCanvas;
        public GameObject advancedCanvas;

        private void Start()
        {
            basicMode.onClick.AddListener(() => ActivateCanvas(basicCanvas));
            advancedMode.onClick.AddListener(() => ActivateCanvas(advancedCanvas));
        }

        private void ActivateCanvas(GameObject canvas)
        {
            basicCanvas.SetActive(false);
            advancedCanvas.SetActive(false);
            this.gameObject.SetActive(false);
            canvas.SetActive(true);
        }
    }
}