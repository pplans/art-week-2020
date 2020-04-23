using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : WorldObject
{
	#region Members

	protected bool m_isAlive;

	#endregion

	#region UnityEvents

	public virtual void Awake()
	{
		m_isAlive = true;
	}

    #endregion

    #region Methods

	public Character()
	{
		m_isAlive = true;
	}

	~Character()
	{
		m_isAlive = false;
	}

	public void Init()
	{
	}

	public virtual void UpdateCharacter()
	{
       if(!m_isAlive)
	   {
	   }
    }

    public bool IsAlive()
    {
        return m_isAlive;
    }

    public void Die()
    {
        Debug.Log("Character is dead");
        m_isAlive = false;
	}
    #endregion
}
