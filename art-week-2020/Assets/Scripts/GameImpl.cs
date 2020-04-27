using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Base.Characters;
using Assets.Scripts.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameImpl : Game
{
	public InputAction action;

	public LoadLevel loadLevel;

	public List<Character> m_characters;

    public Player m_player = null;

    public GameObject Barrel;
    public GameObject Crate;

    public int NbBarrels;
    public int NbCrates;

	public void Start()
	{
		action.performed += InputPlayer;
		action.Enable();

        InstatiateObjects(NbCrates, NbBarrels);
    }

	private void InputPlayer(InputAction.CallbackContext callbackContext)
	{
	}

	public override void UpdateGame()
	{
        if (Input.GetAxis("Cancel") > 0)
            Application.Quit();

		// update dudes
		m_player.UpdateCharacter();

		if (m_player.IsAlive == false)
		{
			
		}

		foreach (Character p in m_characters)
		{
			p.UpdateCharacter();
		}
	}

    private void InstatiateObjects(int crates, int barrels)
    {
        float minRange = -900;
        float maxRange = 900;
        for (int i = 0; i < crates; i++)
        {
            Instantiate(Crate, new Vector3(Random.Range(minRange, maxRange), 0f, Random.Range(minRange, maxRange)), Crate.transform.rotation);
        }
        for (int i = 0; i < barrels; i++)
        {
            Instantiate(Barrel, new Vector3(Random.Range(minRange, maxRange), 0f, Random.Range(minRange, maxRange)), Barrel.transform.rotation);
        }
    }
}
