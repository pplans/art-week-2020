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
            protected set;
        }

        [SerializeField]
        protected int MaxLife;

        protected int Life;

        [SerializeField]
        protected readonly float BaseSpeed;

        [SerializeField]
        protected float MinSpeed;

        protected float Speed;

        [SerializeField]
        protected float BonusSpeed;

        protected List<Effect> Effects;
        #endregion

        #region UnityEvents
        public virtual void Awake()
        {
            IsAlive = true;
            Life = MaxLife;
            Speed = BaseSpeed;
            Effects = new List<Effect>();
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

        protected void UpdateEffects()
        {
            for (var i = Effects.Count - 1; i >= 0; i--)
            {
                Effects[i].Update();
            }
        }

        public void Die()
        {
            Debug.Log("Character is dead");
            Destroy(gameObject);
        }

        public void ModifyLife(int amount)
        {
            var newLife = Life + amount;
            if (newLife > MaxLife)
                newLife = MaxLife;
            else if (newLife < 0)
                newLife = 0;

            Life = newLife;
            IsAlive = Life > 0;
        }

        public void ModifySpeed(float amount)
        {
            BonusSpeed += amount;
        }

        public void UpdateSpeed()
        {
            var newSpeed = BaseSpeed + BonusSpeed;
            Speed = (newSpeed < MinSpeed) ? MinSpeed : newSpeed;
        }

        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
            effect.Apply();
        }

        public void RemoveEffect(Effect effect)
        {
            effect.Cancel();
            Effects.Remove(effect);
        }
        #endregion
    }
}
