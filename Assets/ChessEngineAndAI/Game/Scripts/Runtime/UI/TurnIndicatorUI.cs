using UnityEngine;
using UnityEngine.UI;

namespace ChessEngine.Game.UI
{
    /// <summary>
    /// A simple component that displays UI for turn starts and stops in singleplayer games.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class TurnIndicatorUI : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the Text that displays the turn indicator.")]
        public Text turnText;
        [Tooltip("A reference to the GameObject that represents the turn indicator.")]
        public GameObject turnIndicatorObject;
        [Tooltip("The number of seconds the turn indicator is displayed for.")]
        public float turnIndicatorTimeout = 2f;

        /// <summary>A reference to the ChessGameManager component driving this turn indicator UI component.</summary>
        public ChessGameManager GameManager { get; private set; }
        /// <summary>The next Time.time the turn indicator wil lbe disabled.</summary>
        public float TurnIndicatorDisableTime { get; private set; } = float.NegativeInfinity;

        // Unity callback(s).
        void Awake()
        {
            // Find ChessGameManager reference.
            GameManager = FindObjectOfType<ChessGameManager>();
            if (GameManager == null)
                Debug.LogError("TurnIndicatorUI component was unable to find a ChessGameManager component in the scene!", gameObject);
        }

        void Update()
        {
            // If set to be disabled, and the disable time has been reached disable the turn indicator.
            if (!float.IsNegativeInfinity(TurnIndicatorDisableTime) && Time.time >= TurnIndicatorDisableTime)
                DisableTurnIndicator();
        }

        void OnEnable()
        {
            if (GameManager != null)
            {
                // Subscribe to relevant event(s).
                GameManager.TurnStarted.AddListener(OnTurnStarted);
            }
        }

        void OnDisable()
        {
            if (GameManager != null)
            {
                // Unsubscribe from event(s).
                GameManager.TurnStarted.RemoveListener(OnTurnStarted);
            }
        }

        // Public method(s).
        /// <summary>Enables the turn indicator for turnIndicatorTimeout seconds.</summary>
        public void EnableTurnIndicator()
        {
            // Enable the turn indicator object.
            if (turnIndicatorObject != null)
                turnIndicatorObject.SetActive(true);

            // Set the turn indicator disable time to the current time + turnIndicatorTimeout seconds.
            TurnIndicatorDisableTime = Time.time + turnIndicatorTimeout;
        }

        /// <summary>Disables the turn indicator.</summary>
        public void DisableTurnIndicator()
        {
            // Disable the turn indicator object.
            if (turnIndicatorObject != null)
                turnIndicatorObject.SetActive(false);

            // Set the turn indicator disable time to float.NegativeInfinity meaning never set to disable.
            TurnIndicatorDisableTime = float.NegativeInfinity;
        }

        // Private callback(s).
        /// <summary>Invoked by the GameManager.TurnStarted event.</summary>
        /// <param name="pTurn"></param>
        void OnTurnStarted(ChessColor pTurn)
        {
            // Set turn text.
            if (turnText != null)
                turnText.text = pTurn.ToString() + "'s Turn";

            // Enable the turn indicator.
            EnableTurnIndicator();
        }
    }
}
