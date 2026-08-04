using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);
    public static bool cpuHasMadeTheMove;

    public static bool cpuHandInsideScene;

    public static int cpuHandIndex;

    public GameObject GC;

    public GameObject hand;

    public Texture2D[] handTextures;

    private Vector3 startingPosition;

    private Vector3 targetPosition;

    private float moveSpeed;

    private bool animFlag;

    private void Awake()
    {
        cpuHasMadeTheMove = false;
        cpuHandInsideScene = false;
        startingPosition = new Vector3(0f, 10f, 0f);
        targetPosition = new Vector3(0f, 3f, 0f);
        base.transform.position = targetPosition;
        moveSpeed = 1.5f;
        animFlag = false;
        cpuHandIndex = 0;
    }

    private IEnumerator Start()
    {
        yield return _waitForSeconds1;
    }

    private void Update()
    {
        if (!GameController.gameIsFinished && GameController.gameIsStarted && GameController.canCpuMoveHand)
        {
            _ = StartCoroutine(playHand());
        }
    }

    public void getBackToStartPosition() => StartCoroutine(goToPosition(base.transform.position, startingPosition, moveSpeed));

    private IEnumerator playHand()
    {
        if (!animFlag)
        {
            animFlag = true;
            GameController.canCpuMoveHand = false;
            cpuHandIndex = UnityEngine.Random.Range(0, 3);
            hand.GetComponent<Renderer>().material.mainTexture = handTextures[cpuHandIndex];
            _ = StartCoroutine(goToPosition(startingPosition, targetPosition, moveSpeed));
            yield return _waitForSeconds1;
            cpuHasMadeTheMove = true;
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
            cpuHandInsideScene = position.y < 8f;
        }
    }
}
