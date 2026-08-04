

using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour
{
	private void Start()
	{
		SceneManager.LoadScene("Game");
	}
}
