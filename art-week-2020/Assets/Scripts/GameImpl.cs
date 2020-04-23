using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameImpl : Game
{
	public InputAction action;

	public LoadLevel loadLevel;

	public List<Character> m_characters;

    public Player m_player = null;

	public void Start()
	{
		action.performed += InputPlayer;
		action.Enable();
	}

	private void InputPlayer(InputAction.CallbackContext callbackContext)
	{
	}

	public override void UpdateGame()
	{
		// update dudes
		m_player.UpdateCharacter();

		if (m_player.IsAlive() == false)
		{
			loadLevel.LoadLevelX(0);
		}

		foreach (Character p in m_characters)
		{
			p.UpdateCharacter();
		}
	}
}
