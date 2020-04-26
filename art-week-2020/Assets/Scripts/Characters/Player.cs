using System.Numerics;
using Assets.Scripts.Base.Characters;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

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
        private Quaternion IntentRotation;

        private Rigidbody _rigidbody;
        private readonly Vector3 _eulerAngleVelocity = new Vector3(0, 1, 0);

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
            IntentRotation = Quaternion.identity;
        }

        public void Start()
        {
        }
        #endregion

        #region Methods
        public override bool IsPlayer() { return true; }

		private Vector2 PlayerOffset = new Vector2(0, 0);

        public override void UpdateCharacter()
        {
            base.UpdateCharacter(); // Call parent update

            inertia = Inertia;

            ManageInput();

            ManageWaves();

            var pos = new Vector2(transform.position.x, transform.position.z);
            Vector3 normal;
            var y = water.getHeightAtPoint(pos, out normal) + 2;
            var newPos = new Vector3(pos.x, y, pos.y);
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime);

            water.Offset = new Vector3(PlayerOffset.x, 0f, PlayerOffset.y);
            //water.DirectionSpeed = new Vector2(Speed, Speed);
			Vector3 dir = GetDirection();
			//water.Direction = new Vector2(dir.x, dir.z);

			water.Direction = new Vector2(transform.forward.x, transform.forward.z);
			water.DirectionSpeed = new Vector2(0.00001f, 0.00001f);

			PlayerOffset += (new Vector2(transform.forward.x, transform.forward.z)) * (new Vector2(0.001f, 0.001f));
        }

        public void AddScore(int amount)
        {
            Score += amount;
        }

        public Vector3 GetDirection()
        {
            return Quaternion.Euler(0, -90, 0) * transform.forward;
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
                IntentRotation = Quaternion.Euler(_eulerAngleVelocity * _horizontal * Time.deltaTime * _rotationSpeed);
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

        private void ManageWaves()
        {
            var wavesRotation = GetWavesRotation();
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, wavesRotation, 0.5f) * IntentRotation);
        }

        #endregion
    }
}
