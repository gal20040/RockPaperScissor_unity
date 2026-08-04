

using System.Collections;
using UnityEngine;

public class StatusTextController : MonoBehaviour
{
	internal Vector3 startingSize;

	public string myText = "Good!";

	private IEnumerator Start()
	{
		GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(0.2f);
		GetComponent<Renderer>().enabled = true;
		startingSize = base.transform.localScale;
		StartCoroutine(scaleUp());
	}

	private IEnumerator scaleUp()
	{
		GetComponent<TextMesh>().text = myText;
		while (true)
		{
			Vector3 localScale = base.transform.localScale;
			if (!(localScale.x < 1f))
			{
				break;
			}
			Transform transform = base.transform;
			Vector3 localScale2 = base.transform.localScale;
			float x = localScale2.x + 0.045f;
			Vector3 localScale3 = base.transform.localScale;
			float y = localScale3.y + 0.045f;
			Vector3 localScale4 = base.transform.localScale;
			transform.localScale = new Vector3(x, y, localScale4.z);
			Transform transform2 = base.transform;
			Vector3 position = base.transform.position;
			float x2 = position.x;
			Vector3 position2 = base.transform.position;
			float y2 = position2.y + 0.025f;
			Vector3 position3 = base.transform.position;
			transform2.position = new Vector3(x2, y2, position3.z);
			yield return 0;
		}
		float t = 1f;
		while (t > 0f)
		{
			t -= Time.deltaTime;
			Transform transform3 = base.transform;
			Vector3 position4 = base.transform.position;
			float x3 = position4.x;
			Vector3 position5 = base.transform.position;
			float y3 = position5.y + 0.01f;
			Vector3 position6 = base.transform.position;
			transform3.position = new Vector3(x3, y3, position6.z);
			if (t <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			yield return 0;
		}
	}
}
