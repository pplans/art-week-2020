using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;

using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;                   //Allows us to use UI.

public abstract class Game : MonoBehaviour
{
	public static Game instance = null;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		//DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		UpdateGame();
	}

	public abstract void UpdateGame();
}