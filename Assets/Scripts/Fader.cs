

using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
	public static bool isFading;

	private float fadeSpeed = 0.75f;

	public Color startColor;

	private void Awake()
	{
		isFading = false;
		base.transform.position = new Vector3(0f, 0f, 2f);
		startColor = GetComponent<Renderer>().material.color;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.F))
		{
			StartCoroutine(fade());
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.G))
		{
			StartCoroutine(fadeToBlack());
		}
	}

	public IEnumerator fade()
	{
		if (isFading)
		{
			yield break;
		}
		isFading = true;
		base.transform.position = new Vector3(0f, 0f, -2f);
		GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * fadeSpeed;
			if (t <= 0.5f)
			{
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.SmoothStep(startColor.a, 1f, t * 2f));
			}
			else
			{
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.SmoothStep(1f, startColor.a, (t - 0.5f) * 2f));
			}
			if (t >= 1f)
			{
				MonoBehaviour.print("Fade Completed.");
				isFading = false;
				base.transform.position = new Vector3(0f, 0f, 2f);
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
			}
			yield return 0;
		}
	}

	public IEnumerator fadeToBlack()
	{
		if (isFading)
		{
			yield break;
		}
		isFading = true;
		base.transform.position = new Vector3(0f, 0f, -2f);
		GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * fadeSpeed;
			if (t <= 1f)
			{
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.SmoothStep(0f, 1f, t));
			}
			if (t >= 1f)
			{
				MonoBehaviour.print("Fade Completed.");
				isFading = false;
				base.transform.position = new Vector3(0f, 0f, 2f);
				GetComponent<Renderer>().material.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
			}
			yield return 0;
		}
	}
}
