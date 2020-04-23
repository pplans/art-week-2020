using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
	public string[] levels;

	public void GameOverDelayed(float delay)
	{
		Invoke("GameOver", delay);
	}

	public void GameOver()
	{
		LoadLevelX(0);
	}

	public void LoadLevelX(int level)
	{
		SceneManager.LoadScene(levels[level], LoadSceneMode.Single);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
