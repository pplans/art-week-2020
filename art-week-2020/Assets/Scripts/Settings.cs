using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	#region GlobalVariables
	public Toggle SkipTutorials = null;
	#endregion

	// Start is called before the first frame update
	void Start()
    {
    }

	void SetPrefInt(string id, int v)
	{
		PlayerPrefs.SetInt(id, v);
	}
	void SetPrefBool(string id, bool v)
	{
		PlayerPrefs.SetInt(id, v?1:0);
	}

	int GetPrefInt(string id)
	{
		return PlayerPrefs.GetInt(id);
	}
	bool GetPrefBool(string id)
	{
		return PlayerPrefs.GetInt(id) == 1;
	}
}
