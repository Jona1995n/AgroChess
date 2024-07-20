using UnityEngine;
using UnityEngine.UI;

namespace ChessEngine.Game.Networking.Demo.UI
{
    /// <summary>
    /// A simple component that displays UI for turn starts and stops with more specific information for multiplayer games (and at appropriate times for multiplayer games).
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class NetworkTurnIndicatorUI : MonoBehaviour
    {
        // TurnIndicatorMode.
        public enum TurnIndicatorMode
        {
            /// <summary>Only display turn indicator UI on local player turn.</summary>
            LocalPlayerTurn,
            /// <summary>Display turn indicator UI on any turn start.</summary>
            AllTurns
        }

        // NetworkTurnIndicatorUI.
        [Header("Settings")]
        [Tooltip("A reference to the Text that displays the turn indicator.")]
        public Text turnText;
        [Tooltip("A reference to the GameObject that represents the turn indicator.")]
        public GameObject turnIndicatorObject;
        [Tooltip("The number of seconds the turn indicator is displayed for.")]
        public float turnIndicatorTimeout = 2f;
        [Tooltip("The turn indicator display mode.\n\nLocalPlayerTurn - Only display turn indicator UI on local player turn.\nAllTurns - Display turn indicator UI on any turn start.")]
        public TurnIndicatorMode turnIndicatorMode;

        /// <summary>A reference to the NetworkChessGameManager component driving this turn indicator UI component.</summary>
        public NetworkChessGameManager GameManager { get; private set; }
        /// <summary>The next Time.time the turn indicator wil lbe disabled.</summary>
        public float TurnIndicatorDisableTime { get; private set; } = float.NegativeInfinity;

        // Unity callback(s).
        void Awake()
        {
            // Find NetworkChessGameManager reference.
            GameManager = FindObjectOfType<NetworkChessGameManager>();
            if (GameManager == null)
                Debug.LogError("TurnIndicatorUI component was unable to find a NetworkChessGameManager component in the scene!", gameObject);
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
                GameManager.NetworkGameStarted.AddListener(OnNetworkGameStarted);
            }
        }

        void OnDisable()
        {
            if (GameManager != null)
            {
                // Unsubscribe from event(s).
                GameManager.TurnStarted.RemoveListener(OnTurnStarted);
                GameManager.NetworkGameStarted.RemoveListener(OnNetworkGameStarted);
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
        /// <summary>Invoked by the GameManager.NetworkGameStarted event.</summary>
        void OnNetworkGameStarted()
        {
            // Set turn text.
            if (turnText != null)
            {
                turnText.text = GameManager.ChessInstance.turn.ToString() + "'s Turn";
                if (GameManager.ChessInstance.turn == GameManager.team)
                    turnText.text += "\n(Your Turn)";
            }

            // Enable the turn indicator.
            EnableTurnIndicator();
        }

        /// <summary>Invoked by the GameManager.TurnStarted event.</summary>
        /// <param name="pTurn"></param>
        void OnTurnStarted(ChessColor pTurn)
        {
            // Only fire if the network game has started.
            if (GameManager.GameStarted)
            {
                // Set turn text.
                if (turnText != null)
                {
                    turnText.text = pTurn.ToString() + "'s Turn";
                    turnText.text = pTurn.ToString() + "'s Turn";
                    if (pTurn == GameManager.team)
                        turnText.text += "\n(Your Turn)";
                }

                // Only fire the turn indicator UI to show the turn start for your team.
                if (pTurn == GameManager.team)
                {
                    OnLocalPlayerTurnStarted();
                }
                else { OnRemotePlayerTurnStarted(); }
            }
        }

        /// <summary>A private callback that is invoked when the local players turn starts.</summary>
        void OnLocalPlayerTurnStarted()
        {
            EnableTurnIndicator();
        }

        /// <summary>A private callback that is invoked when the remote players turn starts.</summary>
        void OnRemotePlayerTurnStarted()
        {
            if (turnIndicatorMode == TurnIndicatorMode.AllTurns)
                EnableTurnIndicator();
        }
    }
}
