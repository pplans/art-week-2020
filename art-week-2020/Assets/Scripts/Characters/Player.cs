﻿using Assets.Scripts.Base.Characters;
using UnityEngine;

namespace Assets.Scripts.Characters
{
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
            if (Input.GetKeyDown("space"))
                EffectManager.AddSpeedBuff(0.1f, this, isInfinite: true);
        }
        #endregion
    }
}
