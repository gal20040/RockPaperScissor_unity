
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
	private enum Page
	{
		PLAY,
		PAUSE
	}

	public static bool isPaused;

	private float savedTimeScale;

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
		if (UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyUp(KeyCode.Escape))
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
		if (UnityEngine.Input.GetKeyDown(KeyCode.R))
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
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (!Physics.Raycast(ray, out RaycastHit hitInfo))
		{
			return;
		}
		string name = hitInfo.transform.gameObject.name;
		if (name == null)
		{
			return;
		}
		if (!(name == "BtnPause"))
		{
			if (!(name == "BtnResume"))
			{
				if (name == "BtnRestart")
				{
					UnPauseGame();
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
				return;
			}
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
