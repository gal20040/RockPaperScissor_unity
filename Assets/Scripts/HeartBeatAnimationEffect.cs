using System.Collections;
using UnityEngine;

public class HeartBeatAnimationEffect : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    public float intensity = 1.1f;

    public float animSpeed = 1f;

    private bool animationFlag;

    private float startScaleX;

    private float startScaleY;

    private float endScaleX;

    private float endScaleY;

    private void Start()
    {
        animationFlag = true;
        var localScale = base.transform.localScale;
        startScaleX = localScale.x;
        var localScale2 = base.transform.localScale;
        startScaleY = localScale2.y;
        endScaleX = startScaleX * intensity;
        endScaleY = startScaleY * intensity;
    }

    private void Update()
    {
        if (animationFlag)
        {
            animationFlag = false;
            _ = StartCoroutine(animatePulse(base.gameObject));
        }
    }

    private IEnumerator animatePulse(GameObject _btn)
    {
        yield return _waitForSeconds0_1;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * 5.5f * animSpeed;
            var transform = _btn.transform;
            var x = Mathf.SmoothStep(startScaleX, endScaleX, t);
            var y = Mathf.SmoothStep(startScaleY, endScaleY, t);
            var localScale = _btn.transform.localScale;
            transform.localScale = new Vector3(x, y, localScale.z);
            yield return 0;
        }
        var r = 0f;
        var localScale2 = _btn.transform.localScale;
        if (localScale2.x >= endScaleX)
        {
            while (r <= 1f)
            {
                r += Time.deltaTime * 2f * animSpeed;
                var transform2 = _btn.transform;
                var x2 = Mathf.SmoothStep(endScaleX, startScaleX, r);
                var y2 = Mathf.SmoothStep(endScaleY, startScaleY, r);
                var localScale3 = _btn.transform.localScale;
                transform2.localScale = new Vector3(x2, y2, localScale3.z);
                yield return 0;
            }
        }
        var localScale4 = _btn.transform.localScale;
        if (localScale4.x <= startScaleX)
        {
            yield return _waitForSeconds0_1;
            animationFlag = true;
        }
    }
}
