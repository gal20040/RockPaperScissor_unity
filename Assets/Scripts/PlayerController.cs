using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_75 = new WaitForSeconds(0.75f);
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);
    public static bool playerHasMadeTheMove;

    public static bool playerHandInsideScene;

    public static int playerHandIndex;

    public GameObject GC;

    public GameObject hand;

    public Texture2D[] handTextures;

    private Vector3 startingPosition;

    private Vector3 targetPosition;

    private float moveSpeed;

    private bool animFlag;

    public AudioClip tapSfx;

    private RaycastHit hitInfo;

    private Ray ray;

    private void Awake()
    {
        playerHasMadeTheMove = false;
        playerHandInsideScene = false;
        startingPosition = new Vector3(0f, -8f, 0f);
        targetPosition = new Vector3(0f, -1f, 0f);
        base.transform.position = targetPosition;
        moveSpeed = 1.5f;
        animFlag = false;
        playerHandIndex = 0;
    }

    private IEnumerator Start()
    {
        yield return _waitForSeconds1;
    }

    private void Update()
    {
        if (!GameController.gameIsFinished && GameController.gameIsStarted && GameController.gameIsStarted && AIController.cpuHandInsideScene && !animFlag && !playerHandInsideScene)
        {
            touchManager();
        }
    }

    private void touchManager()
    {
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
        }
        else
        {
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }
            ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        }
        if (!Physics.Raycast(ray, out hitInfo))
        {
            return;
        }
        var gameObject = hitInfo.transform.gameObject;
        var name = gameObject.name;
        if (name == null)
        {
            return;
        }
        if (name == "Btn-Rock")
        {
            _ = StartCoroutine(animateButton(gameObject));
            _ = StartCoroutine(playHand(0));
        }
        else
        {
            if (name == "Btn-Paper")
            {
                _ = StartCoroutine(animateButton(gameObject));
                _ = StartCoroutine(playHand(1));
            }
            else
            {
                if (name == "Btn-Scissor")
                {
                    _ = StartCoroutine(animateButton(gameObject));
                    _ = StartCoroutine(playHand(2));
                }
            }
        }
    }

    public void getBackToStartPosition() => StartCoroutine(goToPosition(base.transform.position, startingPosition, moveSpeed));

    private IEnumerator playHand(int _index)
    {
        if (!animFlag)
        {
            animFlag = true;
            hand.GetComponent<Renderer>().material.mainTexture = handTextures[_index];
            _ = StartCoroutine(goToPosition(startingPosition, targetPosition, moveSpeed));
            yield return _waitForSeconds0_75;
            StartCoroutine(GC.GetComponent<GameController>().checkResult(_index, AIController.cpuHandIndex));
        }
    }

    private IEnumerator goToPosition(Vector3 from, Vector3 to, float s)
    {
        animFlag = true;
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * s;
            base.transform.position = new Vector3(Mathf.SmoothStep(from.x, to.x, t), Mathf.SmoothStep(from.y, to.y, t), Mathf.SmoothStep(from.z, to.z, t));
            yield return 0;
        }

        if (t >= 1f)
        {
            animFlag = false;
            var position = base.transform.position;
            playerHandInsideScene = position.y > -8f;
        }
    }

    private IEnumerator animateButton(GameObject _btn)
    {
        var startingScale = _btn.transform.localScale;
        var destinationScale = startingScale * 1.1f;
        _ = StartCoroutine(playSfx(tapSfx, 0f));
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * 8f;
            var transform = _btn.transform;
            var x = Mathf.SmoothStep(startingScale.x, destinationScale.x, t);
            var y = Mathf.SmoothStep(startingScale.y, destinationScale.y, t);
            var localScale = _btn.transform.localScale;
            transform.localScale = new Vector3(x, y, localScale.z);
            yield return 0;
        }
        var r = 0f;
        var localScale2 = _btn.transform.localScale;
        if (localScale2.x >= destinationScale.x)
        {
            while (r <= 1f)
            {
                r += Time.deltaTime * 10f;
                var transform2 = _btn.transform;
                var x2 = Mathf.SmoothStep(destinationScale.x, startingScale.x, r);
                var y2 = Mathf.SmoothStep(destinationScale.y, startingScale.y, r);
                var localScale3 = _btn.transform.localScale;
                transform2.localScale = new Vector3(x2, y2, localScale3.z);
                yield return 0;
            }
        }
    }

    private IEnumerator playSfx(AudioClip _sfx, float _d)
    {
        yield return new WaitForSeconds(_d);
        GetComponent<AudioSource>().clip = _sfx;
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
