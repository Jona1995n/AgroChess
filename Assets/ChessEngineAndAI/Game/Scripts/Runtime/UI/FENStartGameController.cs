using UnityEngine;
using UnityEngine.UI;
using ChessEngine.Utility;

namespace ChessEngine.Game.UI
{
    /// <summary>
    /// A simple component that controls the behaviours of the FEN state demo UI.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class FENStartGameController : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the InputField that will hold the FEN string.")]
        public InputField fenInputField;
        [Tooltip("A reference to the ChessGameManager that is being driven by this component.")]
        public ChessGameManager gameManager;

        // Public method(s).
        /// <summary>Performs basic validation on the FEN string before starting the game.</summary>
        public void StartGame()
        {
            // Ensure the gameManager reference, fenText reference, and the underlying text is valid.
            if (gameManager != null && fenInputField != null && fenInputField.text != null && fenInputField.text.Length > 0)
            {
                // Validate the fen string.
                string fen = fenInputField.text.Trim();
                if (FENUtility.IsFENStringValid(fen))
                {
                    gameManager.LoadGameFromFEN(fen);
                }
                else { Debug.LogWarning("Unable to start game! FEN string is invalid.", gameObject); }
            }
            else { Debug.LogWarning("Unable to start game! Check the 'fenTextField' and 'gameManager' settings in the FENStartGameController components 'Inspector'.", gameObject); }
        }
    }
}
