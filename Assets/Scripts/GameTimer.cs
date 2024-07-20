using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessEngine.Game.UI
{


    public class GameTimer : MonoBehaviour
    {

        float TimerDurationF = 60f;
        string currentPlayer;
        float WhiteTimeLeftVal, BlackTimeLeftVal;
        public bool GameInProgress = false;
        // Start is called before the first frame update
        void Start()
        {
            GameInProgress = true;
        }

        // Update is called once per frame
        void Update()
        {
            switch (currentPlayer)
            {
                case "white":

                    WhiteTimeLeftVal -= Time.deltaTime;

                    break;
                case "black":

                    BlackTimeLeftVal -= Time.deltaTime;
            
                    break;
            }

        }
        public float WhiteTimeLeft()
        {
            return WhiteTimeLeftVal;
        }
        public float BlackTimeLeft()
        {
            return BlackTimeLeftVal;
        }
        public void SetTimerDuration(float TimerDuration)
        {
            TimerDurationF = TimerDuration;
            WhiteTimeLeftVal = BlackTimeLeftVal = TimerDurationF;
        }
        void TimerDurationOver()
        {
            GameInProgress = false;
            print("game overrrrrr");
        }
        public void OnTurnStarted(ChessColor pTurn)
        {
            if (pTurn == ChessColor.White)
            {
                currentPlayer = "white";
            }
            else
            {
                currentPlayer = "black";
            }
        }
    }
}
