using Assets.Scripts.Base.Characters;

namespace Assets.Scripts.Characters
{
    public static class EffectManager
    {
        #region Members

        #endregion

        #region Methods
        public static void AddSpeedBuff(float amount, Character character, float duration = 1f, bool isInfinite = false)
        {
            character.AddEffect(new SpeedEffect(amount, duration, isInfinite, character));
        }
        #endregion
    }
}
