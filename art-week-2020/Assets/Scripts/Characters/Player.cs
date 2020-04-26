using System.Numerics;
using Assets.Scripts.Base.Characters;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
        private readonly float _rotationSpeed = 1.0F;

        private float _horizontal;
        private float _vertical;

        private Rigidbody _rigidbody;
        private readonly Vector3 _eulerAngleVelocity = new Vector3(0, 100, 0);

        [SerializeField]
        public float Inertia;

        public Rigidbody canonballPrefab;

        #endregion

        #region UnityEvents
        public new void Awake()
        {
            base.Awake(); // Call parent init
            _rigidbody = GetComponent<Rigidbody>();
            inertia = Inertia;
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

            inertia = Inertia;

            ManageInput();

            var pos = new Vector2(transform.position.x, transform.position.z);
            Vector3 normal;
            var y = water.getHeightAtPoint(pos, out normal) + 2;
            var newPos = new Vector3(pos.x, y, pos.y);
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime);
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

            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
            {
                // Instantiate the projectile at the position and rotation of this transform
				Rigidbody clone;
				clone = Instantiate(canonballPrefab, transform.position, transform.rotation);
				clone.transform.forward = Input.GetButtonDown("Fire1")?transform.right : transform.right * -1;
				clone.transform.parent = transform;
			}
        }
        #endregion
    }
}
