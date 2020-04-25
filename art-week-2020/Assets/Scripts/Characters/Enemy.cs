using Assets.Scripts.Base.Characters;

namespace Assets.Scripts.Characters
{
    public class Enemy : Character
    {
        #region Members


        #endregion

        #region UnityEvents
        public override void Awake()
        {
        }

        public void Start()
        {
        }
        #endregion

        #region Methods
        public override bool IsIA() { return true; }
        public override void UpdateCharacter()
        {
            base.UpdateCharacter();
        }
        #endregion
    }
}
