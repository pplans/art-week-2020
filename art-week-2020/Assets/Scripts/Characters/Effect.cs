using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Base.Characters;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Characters
{
    public abstract class Effect
    {
        #region Members
        protected Character Holder;
        protected float Duration;
        protected bool IsInfinite;
        #endregion

        #region UnityEvents
        public void Update()
        {
            if (IsInfinite) return;
            var newDuration = Duration - Time.deltaTime; 
            if (newDuration > 0)
            {
                Duration = newDuration;
            }
            else
            {
                Duration = 0;
                Holder.RemoveEffect(this);
            }

        }
        #endregion

        #region Methods
        protected Effect(float duration, bool isInfinite, Character holder)
        {
            Holder = holder;
            Duration = duration;
            IsInfinite = isInfinite;
        }

        public abstract void Apply();
        public abstract void Cancel();
        #endregion
    }

    public class SpeedEffect : Effect
    {
        #region Members
        private readonly float _speedModifier;
        #endregion

        #region Methods
        public SpeedEffect(float modifier, float duration, bool isInfinite, Character holder) : base(duration, isInfinite, holder)
        {
            _speedModifier = modifier;
        }
        public override void Apply()
        {
            Holder.ModifySpeed(_speedModifier);
        }

        public override void Cancel()
        {
            Holder.ModifySpeed(-_speedModifier);
        }
        #endregion
    }
}