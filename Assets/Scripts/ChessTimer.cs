using UnityEngine;
using UnityEngine.UI;

namespace ChessEngine.Game.UI
{


    public class ChessTimer : MonoBehaviour
    {
        public Text timerText; // Assign this in the inspector
        private float timeLeft;
        private float maxTime = 300f; // Default max time, can be set dynamically
        private bool timerIsActive = false;
        public bool White;
        bool GameStarted=false;
        public GameTimer BoardController;
        GameObject ChessController;

        void Update()
        {
            
                if (White)
                {
                    timeLeft = BoardController.WhiteTimeLeft();
                }
                else
                {
                    timeLeft = BoardController.BlackTimeLeft();
                }
            if (timeLeft > 0)
            {
                UpdateTimerDisplay();
                if (ChessController == null)
                {
                    ChessController = GameObject.FindGameObjectWithTag("GameController");
                }
                GameStarted = true;
            }
            else if(GameStarted)
            {
                TimerOver();
            }
            
        }



        private void UpdateTimerDisplay()
        {
            timerText.text = Mathf.FloorToInt(timeLeft / 60).ToString("00") + ":" + Mathf.FloorToInt(timeLeft % 60).ToString("00");
        }

        private void TimerOver()
        {
            if (ChessController)
            {
                if (White)
                {
                    ChessController.GetComponent<ChessGameManager>().EndGame(ChessColor.White);
                }
                else
                {
                    ChessController.GetComponent<ChessGameManager>().EndGame(ChessColor.Black);
                }
                
                
                
            }
        }

    }
}