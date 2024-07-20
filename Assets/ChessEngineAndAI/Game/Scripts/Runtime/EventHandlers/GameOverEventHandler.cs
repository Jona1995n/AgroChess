using UnityEngine;
using UnityEngine.UI;

namespace ChessEngine.Game.EventHandlers
{
    /// <summary>
    /// A simple component that is attached to the same GameObject as a ChessGameManager component
    /// that listens for game over events and handles them in a game-specific way.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(ChessGameManager))]
    public class GameOverEventHandler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the Text that represents the game over message.")]
        public Text gameOverText;

        /// <summary>A reference to the ChessGameManager associated with this component.</summary>
        public ChessGameManager GameManager { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find ChessGameManager reference.
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            GameManager.GameOver.AddListener(OnGameOver);
        }

        void OnDisable()
        {
            GameManager.GameOver.RemoveListener(OnGameOver);
        }

        // Public method(s).
        /// <summary>
        /// Sets the 'gameOverText.text' value for this component if a valid gameOverText reference is set.
        /// </summary>
        /// <param name="pText"></param>
        public void SetGameOverText(string pText)
        {
            if (gameOverText != null)
                gameOverText.text = pText;
        }

        // Private callback(s).
        /// <summary>
        /// A private callback that is invoked when the related ChessGameManager's GameOver event is invoked.
        /// </summary>
        /// <param name="pTurn">The ChessColor representing the team whose turn it was when the game ended.</param>
        /// <param name="pReason">The GameOverReason that represents the reason why the game ended.</param>
        void OnGameOver(ChessColor pTurn, GameOverReason pReason)
        {
            switch (pReason)
            {
                case GameOverReason.Won:
                    // Set game over text to display current turn color as winner.
                    SetGameOverText("Game Over!\n" + pTurn.ToString() + " wins!");
                    break;
                case GameOverReason.Draw:
                    // Set game over text to display draw.
                    SetGameOverText("Game Over!\nDraw");
                    break;
                case GameOverReason.Forfeit:
                    // Set game over text to display opposite color as winner.
                    if (pTurn == ChessColor.Black)
                    {
                        SetGameOverText("Game Over!\nWhite wins!");
                    }
                    else { SetGameOverText("Game Over!\nBlack wins!"); }
                    break;
                case GameOverReason.TimeExpired:
                    if (pTurn == ChessColor.Black)
                    {
                        SetGameOverText("Game Over!\nWhite wins!");
                    }
                    else { SetGameOverText("Game Over!\nBlack wins!"); }
                    break;
                default:
                    // Set game over text.
                    SetGameOverText("Game Over!");

                    // Log unhandled game over reason warning.
                    Debug.LogWarning("Unhandled GameOverReason found '" + pReason.ToString() + "'!");
                    break;
            }
        }
    }
}
