using UnityEngine;
using UnityEngine.UI;
using ChessEngine.AI;
using ChessEngine.Game.AI;

namespace ChessEngine.Game.Demo.UI
{
    /// <summary>
    /// A demo component that displays who is playing against who via UI images.
    /// TODO: Replace with AI registration with scriptable object and ClassTypeReference array so image, class type reference, etc, are all registered for all possible AIs instead of hard coding them.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PlayerIndicator : MonoBehaviour
    {
        [Header("Instance")]
        [Tooltip("The current image being displayed/driven for the white team.")]
        public Image whiteImage;
        [Tooltip("The current image being displayed/driven for the black team.")]
        public Image blackImage;

        [Header("Settings")]
        [Tooltip("The sprite for a human player.")]
        public Sprite humanSprite;
        [Tooltip("The sprite for AI.")]
        public Sprite aiSprite;

        [Header("References")]
        [Tooltip("A reference to the ChessGameManager that drives this indicator. (If null AI will not be considered.)")]
        public ChessGameManager chessGameManager;

        // Unity callback(s).
        void Awake()
        {
            // Ensure there is a valid chessGameManager reference.
            if (chessGameManager == null)
            {
                chessGameManager = GetComponent<ChessGameManager>();
                if (chessGameManager == null)
                    chessGameManager = FindObjectOfType<ChessGameManager>();
            }

            // Subscribe to relevant chessGameManager event(s).
            if (chessGameManager != null)
            {
                chessGameManager.TurnStarted.AddListener(OnTurnStarted);
                chessGameManager.PreGameReset.AddListener(OnPreGameReset);
            }
            else { Debug.LogWarning("No ChessGameManager found to drive NetworkPlayerIndicator component!", gameObject); }
        }

        void OnDestroy()
        {
            // Unsubscribe from relevant event(s).
            if (chessGameManager != null)
            {
                chessGameManager.TurnStarted.RemoveListener(OnTurnStarted);
                chessGameManager.PreGameReset.RemoveListener(OnPreGameReset);
            }
        }

        // Private callback(s).
        /// <summary>Invoked whenever the chess game manager signals that a turn has started.</summary>
        /// <param name="pTurnColor"></param>
        void OnTurnStarted(ChessColor pTurnColor)
        {
            // Decide white team image.
            if (whiteImage != null)
            {
                // Decide what image to use.
                // If the chess game manager is a ChessAIGameManager then check for AI.
                if (chessGameManager is ChessAIGameManager)
                {
                    // If there is a valid AI on this team use an AI image.
                    ChessAIGameManager chessAIGameManager = chessGameManager as ChessAIGameManager;
                    if (chessAIGameManager.IsWhiteAIEnabled)
                    {
                        if (chessAIGameManager.WhiteAIInstance is ChessAI)
                            whiteImage.sprite = aiSprite;
                    }
                }

                // If team image sprite is still null set to human.
                if (whiteImage.sprite == null)
                    whiteImage.sprite = humanSprite;

                // If there is a valid white image activate the indicator.
                if (whiteImage != null && whiteImage.sprite != null)
                    whiteImage.gameObject.SetActive(true);
            }

            // Decide black team image.
            if (blackImage != null)
            {
                // Decide what image to use.
                // If the chess game manager is a ChessAIGameManager then check for AI.
                if (chessGameManager is ChessAIGameManager)
                {
                    // If there is a valid AI on this team use an AI image.
                    ChessAIGameManager chessAIGameManager = chessGameManager as ChessAIGameManager;
                    if (chessAIGameManager.IsBlackAIEnabled)
                    {
                        if (chessAIGameManager.BlackAIInstance is ChessAI)
                            blackImage.sprite = aiSprite;
                    }
                }

                // If team image sprite is still null set to human.
                if (blackImage.sprite == null)
                    blackImage.sprite = humanSprite;

                // If there is a valid black image activate the indicator.
                if (blackImage != null && blackImage.sprite != null)
                    blackImage.gameObject.SetActive(true);
            }
        }

        /// <summary>Invoked whenever the chess game manager signals the game has been reset.</summary>
        void OnPreGameReset()
        {
            // Hide indicators.
            if (whiteImage != null)
                whiteImage.gameObject.SetActive(false);
            if (blackImage != null)
                blackImage.gameObject.SetActive(false);
        }
    }
}
