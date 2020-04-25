using Assets.Scripts.Base.Characters;
using UnityEngine;

namespace Assets.Scripts.Characters
{
    public class Player : Character
    {
        #region Members

        public int Score
        {
            get;
            private set;
        }

        [SerializeField]
        private readonly float _rotationSpeed = 100.0F;

        private float _horizontal;
        private float _vertical;

        private Rigidbody _rigidbody;
        private readonly Vector3 _eulerAngleVelocity = new Vector3(0, 100, 0);

        #endregion

        #region UnityEvents
        public override void Awake()
        {
            base.Awake(); // Call parent init
            _rigidbody = GetComponent<Rigidbody>();
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
            
            ManageInput();
        }

        public void AddScore(int amount)
        {
            Score += amount;
        }

        private void ManageInput()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            if (_vertical != 0)
            {
                _rigidbody.MovePosition(transform.position + transform.forward * Speed * _vertical * Time.deltaTime);
            }

            if (_horizontal != 0)
            {
                Quaternion deltaRotation = Quaternion.Euler(_eulerAngleVelocity * _horizontal * Time.deltaTime * _rotationSpeed);
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
            }
        }
        #endregion
    }
}
