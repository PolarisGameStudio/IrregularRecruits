using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        public void Awake()
        {

            GetComponent<Button>().onClick.AddListener(Restart);

        }
        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


    }
}