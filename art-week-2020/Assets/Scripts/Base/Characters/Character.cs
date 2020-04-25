using System.Collections.Generic;
using Assets.Scripts.Characters;
using UnityEngine;

namespace Assets.Scripts.Base.Characters
{
    public abstract class Character : WorldObject
    {
        #region Members

        public bool IsAlive
        {
            get;
            set;
        }

        [SerializeField]
        protected int m_maxLife;

        protected int m_life;

        [SerializeField]
        protected readonly float m_baseSpeed;

        [SerializeField]
        protected float m_minSpeed;

        protected float m_speed;

        [SerializeField]
        protected float m_bonusSpeed;

        protected List<Effect> effects;
        #endregion

        #region UnityEvents
        public virtual void Awake()
        {
            IsAlive = true;
            m_life = m_maxLife;
            m_speed = m_baseSpeed;
            effects = new List<Effect>();
        }
        #endregion

        #region Methods
        public virtual void UpdateCharacter()
        {
            UpdateEffects();
            if (!IsAlive)
            {
                Die();
            }
        
            UpdateSpeed();
        }

        private void UpdateEffects()
        {
            for (var i = effects.Count - 1; i >= 0; i--)
            {
                effects[i].Update();
            }
        }

        public void Die()
        {
            Debug.Log("Character is dead");
            Destroy(gameObject);
        }

        public void ModifyLife(int amount)
        {
            var newLife = m_life + amount;
            if (newLife > m_maxLife)
                newLife = m_maxLife;
            else if (newLife < 0)
                newLife = 0;

            m_life = newLife;
            IsAlive = m_life > 0;
        }

        public void ModifySpeed(float amount)
        {
            m_bonusSpeed += amount;
        }

        public void UpdateSpeed()
        {
            var newSpeed = m_baseSpeed + m_bonusSpeed;
            m_speed = (newSpeed < m_minSpeed) ? m_minSpeed : newSpeed;
        }

        public void AddEffect(Effect effect)
        {
            effects.Add(effect);
            effect.Apply();
        }

        public void RemoveEffect(Effect effect)
        {
            effect.Cancel();
            effects.Remove(effect);
        }
        #endregion
    }
}
