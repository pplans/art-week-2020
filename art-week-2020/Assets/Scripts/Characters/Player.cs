using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
	#region Members

	public GameObject cam;
	
    CapsuleCollider m_collider;

	#endregion

	#region UnityEvents
	public override void Awake()
    {
        m_collider = this.gameObject.GetComponent<CapsuleCollider>();
    }

	public void Start()
	{
	}
	#endregion

	#region Methods

	public Player() : base()
	{
	}

	public override bool IsPlayer() { return true; }

	public override void UpdateCharacter()
    {
	}
	#endregion
}
