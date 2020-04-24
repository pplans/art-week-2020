using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
	#region Members



	#endregion

	#region UnityEvents
	public override void Awake()
    {
        base.Awake(); // Call parent init

    }

	public void Start()
	{
	}
	#endregion

	#region Methods

    public override bool IsPlayer() { return true; }

    public override void UpdateCharacter()
    {
        base.UpdateCharacter(); // Call parent update

    }

    #endregion
}
