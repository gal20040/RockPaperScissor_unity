

using System.Collections;
using UnityEngine;

public class StatusTextController : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_2 = new(0.2f);
    internal Vector3 startingSize;

    public string myText = "Good!";

    private IEnumerator Start()
    {
        GetComponent<Renderer>().enabled = false;
        yield return _waitForSeconds0_2;
        GetComponent<Renderer>().enabled = true;
        startingSize = base.transform.localScale;
        _ = StartCoroutine(scaleUp());
    }

    private IEnumerator scaleUp()
    {
        GetComponent<TextMesh>().text = myText;
        while (true)
        {
            var localScale = base.transform.localScale;
            if (!(localScale.x < 1f))
            {
                break;
            }
            var transform = base.transform;
            var localScale2 = base.transform.localScale;
            var x = localScale2.x + 0.045f;
            var localScale3 = base.transform.localScale;
            var y = localScale3.y + 0.045f;
            var localScale4 = base.transform.localScale;
            transform.localScale = new Vector3(x, y, localScale4.z);
            var transform2 = base.transform;
            var position = base.transform.position;
            var x2 = position.x;
            var position2 = base.transform.position;
            var y2 = position2.y + 0.025f;
            var position3 = base.transform.position;
            transform2.position = new Vector3(x2, y2, position3.z);
            yield return 0;
        }
        var t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            var transform3 = base.transform;
            var position4 = base.transform.position;
            var x3 = position4.x;
            var position5 = base.transform.position;
            var y3 = position5.y + 0.01f;
            var position6 = base.transform.position;
            transform3.position = new Vector3(x3, y3, position6.z);
            if (t <= 0f)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            yield return 0;
        }
    }
}
