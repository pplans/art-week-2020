using UnityEngine;
using UnityEngine.Rendering.UI;

namespace Assets.Scripts.Base
{
    public class WeatherManager : MonoBehaviour
    {
        #region Members

        protected float WindDirection;
        protected float WindTargetDirection;
        protected float WindStrength;
        protected float WindTargetStrength;

        [SerializeField]
        protected float WindMaxStrength;

        [SerializeField]
        protected float WindMinStrength;

        [SerializeField]
        protected float WindBaseDelay;

        protected float WindCurrentDelay;

        #endregion

        #region UnityEvents

        private void Awake()
        {
            InitWind();
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            UpdateWind();
        }

        #endregion

        #region Methods

        protected void InitWind()
        {
            WindDirection = WindGenerateDirection();
            WindStrength = WindGenerateStrength();
            WindCurrentDelay = 0f;
        }

        protected void UpdateWind()
        {
            WindCurrentDelay -= Time.deltaTime;
            if (WindCurrentDelay < 0) // New strength
            {
                WindTargetDirection = WindGenerateDirection();
                WindTargetStrength = WindGenerateStrength();
                WindCurrentDelay = WindBaseDelay;
            }
            WindStrength = Mathf.Lerp(WindStrength, WindTargetStrength, 0.8f * Time.deltaTime);
            WindDirection = Mathf.Lerp(WindDirection, WindTargetDirection, 0.8f * Time.deltaTime);
        }

        private float WindGenerateStrength()
        {
            var strength = Random.value * WindMaxStrength;
            if (strength < WindMinStrength)
                strength = WindMinStrength;
            return strength;
        }

        private float WindGenerateDirection()
        {
            return Random.value * 360;
        }

        #endregion
    }
}
