using Assets.Scripts.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ScoreText : MonoBehaviour
    {
        public Player Player;
        private Text _score;

        // Start is called before the first frame update
        private void Start()
        {
            _score = GetComponent<Text>();
        }

        // Update is called once per frame
        private void Update() => _score.text = "Score: "+ Player.Score;
    }
}
