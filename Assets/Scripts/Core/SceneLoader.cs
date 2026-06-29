using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BulletHeaven.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private CanvasGroup fadeCanvas;
        [SerializeField] private float fadeDuration = 0.4f;

        public const string SceneMainMenu = "MainMenu";
        public const string SceneHub       = "Hub";
        public const string SceneRun       = "Run";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(Transition(sceneName));
        }

        private IEnumerator Transition(string sceneName)
        {
            yield return Fade(1f);
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Fade(0f);
        }

        private IEnumerator Fade(float target)
        {
            float start = fadeCanvas.alpha;
            float elapsed = 0f;
            fadeCanvas.blocksRaycasts = target > 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                fadeCanvas.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
                yield return null;
            }

            fadeCanvas.alpha = target;
            fadeCanvas.blocksRaycasts = target > 0f;
        }
    }
}
