

using System.Collections;
using UnityEngine;

public class HeartBeatAnimationEffect : MonoBehaviour
{
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
		Vector3 localScale = base.transform.localScale;
		startScaleX = localScale.x;
		Vector3 localScale2 = base.transform.localScale;
		startScaleY = localScale2.y;
		endScaleX = startScaleX * intensity;
		endScaleY = startScaleY * intensity;
	}

	private void Update()
	{
		if (animationFlag)
		{
			animationFlag = false;
			StartCoroutine(animatePulse(base.gameObject));
		}
	}

	private IEnumerator animatePulse(GameObject _btn)
	{
		yield return new WaitForSeconds(0.1f);
		float t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime * 5.5f * animSpeed;
			Transform transform = _btn.transform;
			float x = Mathf.SmoothStep(startScaleX, endScaleX, t);
			float y = Mathf.SmoothStep(startScaleY, endScaleY, t);
			Vector3 localScale = _btn.transform.localScale;
			transform.localScale = new Vector3(x, y, localScale.z);
			yield return 0;
		}
		float r = 0f;
		Vector3 localScale2 = _btn.transform.localScale;
		if (localScale2.x >= endScaleX)
		{
			while (r <= 1f)
			{
				r += Time.deltaTime * 2f * animSpeed;
				Transform transform2 = _btn.transform;
				float x2 = Mathf.SmoothStep(endScaleX, startScaleX, r);
				float y2 = Mathf.SmoothStep(endScaleY, startScaleY, r);
				Vector3 localScale3 = _btn.transform.localScale;
				transform2.localScale = new Vector3(x2, y2, localScale3.z);
				yield return 0;
			}
		}
		Vector3 localScale4 = _btn.transform.localScale;
		if (localScale4.x <= startScaleX)
		{
			yield return new WaitForSeconds(0.1f);
			animationFlag = true;
		}
	}
}
