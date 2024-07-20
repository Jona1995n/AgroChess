using UnityEngine;
using ChessEngine;
using ChessEngine.Game;

namespace ChessEngine.Game
{
	/// <summary>
	/// A component that may be added to the same GameObject as a ChessGameManager to add turn time limits.
	/// Intended for use with singleplayer games.
	/// </summary>
	/// Author: Intuitive Gaming Solutions
	[RequireComponent(typeof(ChessGameManager))]
	public class TestTurnTimeLimit : MonoBehaviour
	{
		#region Editor Serialized Setting(s)
		[Header("Settings")]
		[Tooltip("The total number of seconds each team has to make move(s) before automatically forfeitting the game.")]
		public float maxTeamTimeLimit = 120f;

		[Header("Instance - White")]
		[Tooltip("The time left in seconds for the white team.")]
		public float whiteTimeLeft;

		[Header("Instance - Black")]
		[Tooltip("The time left in seconds for the black team.")]
		public float blackTimeLeft;

		#endregion
		#region Public Properties
		/// <summary>A reference to the relevant ChessGameManager.</summary>
		public ChessGameManager GameManager { get; private set; }
		/// <summary>Tracks whether or not a turn is currently started.</summary>
		public bool IsTurnStarted { get; private set; }
		#endregion

		// Unity callback(s).
		#region Unity Callback(s)
		void Awake()
		{
			// Find 'GameManager' reference.
			GameManager = GetComponent<ChessGameManager>();

			// No turn started.
			IsTurnStarted = false;

			// Set default time left.
			ResetTimeLeft();
		}

		void Update()
		{
			// Track turn time left.
			// TURN: White.
			if (GameManager.ChessInstance.turn == ChessColor.White)
			{
				// Subtract the time that has elapsed since last frame from the 'white time left'.
				whiteTimeLeft = Mathf.Max(whiteTimeLeft - Time.deltaTime, 0f);

				// Check if white team forfeits due to time out.
				if (whiteTimeLeft <= 0)
				{
					// Forfeit white.
					GameManager.ChessInstance.EndGame(GameManager.ChessInstance.turn, GameOverReason.TimeExpired);
				}
			}
			// TURN: Black.
			else
			{
				// Subtract the time that has elapsed since last frame from the 'black time left'.
				blackTimeLeft = Mathf.Max(whiteTimeLeft - Time.deltaTime, 0f);

				// Check if blacwk team forfeits due to time out.
				if (blackTimeLeft <= 0)
				{
					// Forfeit black.
					GameManager.ChessInstance.EndGame(GameManager.ChessInstance.turn, GameOverReason.TimeExpired);
				}
			}
		}

		void OnEnable()
		{
			// Subscribe to relevant event(s).
			if (GameManager != null)
			{
				GameManager.PreGameReset.AddListener(OnPreGameReset);
				GameManager.TurnStarted.AddListener(OnTurnStarted);
				GameManager.TurnEnded.AddListener(OnTurnEnded);
			}
		}

		void OnDisable()
		{
			// Unsubscribe from event(s).
			if (GameManager != null)
			{
				GameManager.PreGameReset.RemoveListener(OnPreGameReset);
				GameManager.TurnStarted.RemoveListener(OnTurnStarted);
				GameManager.TurnEnded.RemoveListener(OnTurnEnded);
			}
		}
		#endregion

		#region Public Method(s)
		/// <summary>Resets the time left for both teams to maximum.</summary>
		public void ResetTimeLeft()
		{
			whiteTimeLeft = maxTeamTimeLimit;
			blackTimeLeft = maxTeamTimeLimit;
		}
		#endregion

		#region Private Callback(s)
		/// <summary>Invoked just before the chess game is reset.</summary>
		void OnPreGameReset()
		{
			// Reset time left for both teams.
			ResetTimeLeft();
		}

		/// <summary>Invoked whenever a turn starts in the relevant chess game manager.</summary>
		/// <param name="pTurn">The color whose turn started.</param>
		void OnTurnStarted(ChessColor pTurn)
		{
			// Someones turn is started.
			IsTurnStarted = true;
		}

		/// <summary>Invoked whenever a turn ends in the relevant chess game manager.</summary>
		/// <param name="pTurn">The color whose turn ended</param>
		/// <param name="pMove">The move that ended the turn.</param>
		void OnTurnEnded(ChessColor pTurn, MoveInfo pMove)
		{
			// Noones turn is started.
			IsTurnStarted = false;
		}
		#endregion
	}
}
