using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool gameIsFinished;
    public static bool gameIsStarted;
    public static bool canCpuMoveHand;

    public GameObject playerHand;
    public GameObject cpuHand;
    public float availableTime = 25f;
    public float bonusTime = 2f;
    public float lossTime = 4f;
    public GameObject uiGameScore;
    public GameObject uiBestScore;
    public GameObject uiNewBestScore;
    public GameObject statusText;
    public GameObject scoreUI;
    public GameObject timerBar;
    public GameObject finishPlane;
    public GameObject fader;
    public AudioClip correctGuess;
    public AudioClip wrongGuess;
    public GameObject[] startAnimObject;

    private float timer;
    private int currentScore;
    private int bestScore;
    private Vector3 timerBarStartScale;
    private AudioSource audioSource;

    private AIController aiController;
    private PlayerController playerController;

    private void Awake()
    {
        gameIsFinished = false;
        gameIsStarted = false;
        canCpuMoveHand = false;
        
        timer = availableTime;
        currentScore = 0;
        
        if (timerBar != null)
        {
            timerBarStartScale = timerBar.transform.localScale;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (cpuHand != null) aiController = cpuHand.GetComponent<AIController>();
        if (playerHand != null) playerController = playerHand.GetComponent<PlayerController>();
    }

    private void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        
        if (uiGameScore != null)
        {
            var textMesh = uiGameScore.GetComponent<TextMesh>();
            if (textMesh != null) textMesh.text = "0";
        }
        if (uiBestScore != null)
        {
            var textMesh = uiBestScore.GetComponent<TextMesh>();
            if (textMesh != null) textMesh.text = bestScore.ToString();
        }

        if (uiNewBestScore != null) uiNewBestScore.SetActive(false);
        if (finishPlane != null) finishPlane.SetActive(false);

        SetMenuStateActive(true);
    }

    private void Update()
    {
        if (!gameIsStarted && !gameIsFinished)
        {
            DetectStartTouch();
            return;
        }

        if (gameIsStarted && !gameIsFinished)
        {
            UpdateTimer();
        }
    }

    private void SetMenuStateActive(bool inMenu)
    {
        if (startAnimObject == null) return;

        foreach (var obj in startAnimObject)
        {
            if (obj == null) continue;
            
            string name = obj.name;
            if (name == "Logo" || name == "StartButton")
            {
                obj.SetActive(inMenu);
            }
            else if (name == "AI-Hand" || name == "Player-Hand")
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(!inMenu);
            }
        }
    }

    private void DetectStartTouch()
    {
        Ray ray;
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            return;
        }

        if (Physics.Raycast(ray, out var hitInfo))
        {
            if (hitInfo.transform.gameObject.name == "StartButton")
            {
                _ = StartCoroutine(OnStartButtonPressed(hitInfo.transform.gameObject));
            }
        }
    }

    private IEnumerator OnStartButtonPressed(GameObject startBtn)
    {
        yield return StartCoroutine(AnimateScaleDown(startBtn));
        
        if (aiController != null) aiController.getBackToStartPosition();
        if (playerController != null) playerController.getBackToStartPosition();

        SetMenuStateActive(false);

        yield return new WaitForSeconds(0.5f);

        gameIsStarted = true;
        canCpuMoveHand = true;
    }

    private IEnumerator AnimateScaleDown(GameObject obj)
    {
        var startScale = obj.transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            obj.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        obj.SetActive(false);
    }

    private void UpdateTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            GameOver();
        }

        if (timerBar != null)
        {
            float fillRatio = timer / availableTime;
            fillRatio = Mathf.Clamp01(fillRatio);
            timerBar.transform.localScale = new Vector3(timerBarStartScale.x * fillRatio, timerBarStartScale.y, timerBarStartScale.z);
        }
    }

    public IEnumerator checkResult(int playerHandIndex, int cpuHandIndex)
    {
        string resultText = "";
        if (playerHandIndex == cpuHandIndex)
        {
            resultText = "Draw!";
            if (correctGuess != null) PlaySfx(correctGuess);
        }
        else if (playerHandIndex == (cpuHandIndex + 1) % 3)
        {
            resultText = "Good!";
            currentScore++;
            timer += bonusTime;
            if (timer > availableTime) timer = availableTime;

            if (uiGameScore != null)
            {
                var textMesh = uiGameScore.GetComponent<TextMesh>();
                if (textMesh != null) textMesh.text = currentScore.ToString();
            }

            if (correctGuess != null) PlaySfx(correctGuess);
        }
        else
        {
            resultText = "Oops!";
            timer -= lossTime;
            if (timer < 0f) timer = 0f;

            if (wrongGuess != null) PlaySfx(wrongGuess);
        }

        Vector3 spawnPos = new Vector3(0f, 1f, -1f);
        if (statusText != null)
        {
            var statusObj = Instantiate(statusText, spawnPos, Quaternion.identity);
            var statusController = statusObj.GetComponent<StatusTextController>();
            if (statusController != null)
            {
                statusController.myText = resultText;
            }
        }

        yield return new WaitForSeconds(1f);

        if (!gameIsFinished)
        {
            if (aiController != null) aiController.getBackToStartPosition();
            if (playerController != null) playerController.getBackToStartPosition();

            yield return new WaitForSeconds(0.8f);

            canCpuMoveHand = true;
        }
    }

    private void GameOver()
    {
        gameIsFinished = true;
        gameIsStarted = false;

        if (finishPlane != null)
        {
            finishPlane.SetActive(true);
        }

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
            
            if (uiBestScore != null)
            {
                var textMesh = uiBestScore.GetComponent<TextMesh>();
                if (textMesh != null) textMesh.text = bestScore.ToString();
            }

            if (uiNewBestScore != null)
            {
                uiNewBestScore.SetActive(true);
            }
        }

        var adManagerObj = GameObject.FindGameObjectWithTag("AdManager");
        if (adManagerObj != null)
        {
            var adManager = adManagerObj.GetComponent<AdManager>();
            if (adManager != null)
            {
                adManager.showInterstitial();
            }
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
