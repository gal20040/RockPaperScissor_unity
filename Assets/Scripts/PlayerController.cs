

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
		yield return new WaitForSeconds(1f);
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
		GameObject gameObject = hitInfo.transform.gameObject;
		string name = gameObject.name;
		if (name == null)
		{
			return;
		}
		if (!(name == "Btn-Rock"))
		{
			if (!(name == "Btn-Paper"))
			{
				if (name == "Btn-Scissor")
				{
					StartCoroutine(animateButton(gameObject));
					StartCoroutine(playHand(2));
				}
			}
			else
			{
				StartCoroutine(animateButton(gameObject));
				StartCoroutine(playHand(1));
			}
		}
		else
		{
			StartCoroutine(animateButton(gameObject));
			StartCoroutine(playHand(0));
		}
	}

	public void getBackToStartPosition()
	{
		StartCoroutine(goToPosition(base.transform.position, startingPosition, moveSpeed));
	}

	private IEnumerator playHand(int _index)
	{
		if (!animFlag)
		{
			animFlag = true;
			hand.GetComponent<Renderer>().material.mainTexture = handTextures[_index];
			StartCoroutine(goToPosition(startingPosition, targetPosition, moveSpeed));
			yield return new WaitForSeconds(0.75f);
			StartCoroutine(GC.GetComponent<GameController>().checkResult(_index, AIController.cpuHandIndex));
		}
	}

	private IEnumerator goToPosition(Vector3 from, Vector3 to, float s)
	{
		animFlag = true;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * s;
			base.transform.position = new Vector3(Mathf.SmoothStep(from.x, to.x, t), Mathf.SmoothStep(from.y, to.y, t), Mathf.SmoothStep(from.z, to.z, t));
			yield return 0;
		}
		if (t >= 1f)
		{
			animFlag = false;
			Vector3 position = base.transform.position;
			if (position.y > -8f)
			{
				playerHandInsideScene = true;
			}
			else
			{
				playerHandInsideScene = false;
			}
		}
	}

	private IEnumerator animateButton(GameObject _btn)
	{
		Vector3 startingScale = _btn.transform.localScale;
		Vector3 destinationScale = startingScale * 1.1f;
		StartCoroutine(playSfx(tapSfx, 0f));
		float t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime * 8f;
			Transform transform = _btn.transform;
			float x = Mathf.SmoothStep(startingScale.x, destinationScale.x, t);
			float y = Mathf.SmoothStep(startingScale.y, destinationScale.y, t);
			Vector3 localScale = _btn.transform.localScale;
			transform.localScale = new Vector3(x, y, localScale.z);
			yield return 0;
		}
		float r = 0f;
		Vector3 localScale2 = _btn.transform.localScale;
		if (localScale2.x >= destinationScale.x)
		{
			while (r <= 1f)
			{
				r += Time.deltaTime * 10f;
				Transform transform2 = _btn.transform;
				float x2 = Mathf.SmoothStep(destinationScale.x, startingScale.x, r);
				float y2 = Mathf.SmoothStep(destinationScale.y, startingScale.y, r);
				Vector3 localScale3 = _btn.transform.localScale;
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
