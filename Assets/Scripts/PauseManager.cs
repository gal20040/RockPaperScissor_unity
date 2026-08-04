using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private enum Page
    {
        PLAY,
        PAUSE
    }

    public static bool isPaused;

    private readonly float savedTimeScale;

    public GameObject pausePlane;

    public AudioClip tapSfx;

    private GameObject AdManagerObject;

    private Page currentPage;

    private void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        pausePlane.SetActive(value: false);
        AdManagerObject = GameObject.FindGameObjectWithTag("AdManager");
    }

    private void Update()
    {
        touchManager();
        if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            switch (currentPage)
            {
                case Page.PLAY:
                    PauseGame();
                    break;
                case Page.PAUSE:
                    UnPauseGame();
                    break;
                default:
                    currentPage = Page.PLAY;
                    break;
            }
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void touchManager()
    {
        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo))
        {
            return;
        }

        var name = hitInfo.transform.gameObject.name;
        if (name == null)
        {
            return;
        }

        if (name == "BtnPause")
        {
            playSfx(tapSfx);
            switch (currentPage)
            {
                case Page.PLAY:
                    PauseGame();
                    break;
                case Page.PAUSE:
                    UnPauseGame();
                    break;
                default:
                    currentPage = Page.PLAY;
                    break;
            }
        }
        else
        {
            if (name == "BtnResume")
            {
                playSfx(tapSfx);
                switch (currentPage)
                {
                    case Page.PLAY:
                        PauseGame();
                        break;
                    case Page.PAUSE:
                        UnPauseGame();
                        break;
                    default:
                        currentPage = Page.PLAY;
                        break;
                }
            }
            else
            {
                if (name == "BtnRestart")
                {
                    UnPauseGame();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }

    private void PauseGame()
    {
        if ((bool)AdManagerObject)
        {
            AdManagerObject.GetComponent<AdManager>().showInterstitial();
        }
        isPaused = true;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
        AudioListener.volume = 0f;
        pausePlane.SetActive(value: true);
        currentPage = Page.PAUSE;
    }

    private void UnPauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        AudioListener.volume = 1f;
        pausePlane.SetActive(value: false);
        currentPage = Page.PLAY;
    }

    private void playSfx(AudioClip _clip)
    {
        GetComponent<AudioSource>().clip = _clip;
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
