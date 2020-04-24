using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : WorldObject
{
	#region Members

	protected bool m_isAlive;

    [SerializeField]
    protected int m_maxLife;
    
    protected int m_life;

    [SerializeField]
    protected float m_baseSpeed;


	#endregion

	#region UnityEvents

	public virtual void Awake()
	{
		m_isAlive = true;
        m_life = m_maxLife;
	}

    #endregion

    #region Methods

    public virtual void UpdateCharacter()
	{
       if(!m_isAlive)
       {
           Die();
       }
    }

    public bool IsAlive()
    {
        return m_isAlive;
    }

    public void Die()
    {
        Debug.Log("Character is dead");
        Destroy(gameObject);
	}

    public void IncreaseLife(int amount)
    {
        if (amount < 0)
            return;

        var new_life = m_life + amount;
        if (new_life > m_maxLife)
            new_life = m_maxLife;

        m_life = new_life;
        m_isAlive = m_life > 0;
    }

    public void DecreaseLife(int amount)
    {
        if (amount < 0)
            return;

        var newLife = m_life - amount;
        if (newLife < 0)
            newLife = 0;
        m_life = newLife;
        m_isAlive = m_life > 0;
    }

    #endregion
}
