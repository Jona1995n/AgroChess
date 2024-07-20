using System;
using UnityEngine;
using UnityEngine.Events;
using ChessEngine.Undo;

namespace ChessEngine.Game
{
    /// <summary>
    /// A component that allows moves to be undone and redone for a ChessGameManager that is attached to the same GameObject as this component.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(ChessGameManager))]
    public class ChessUndoManager : MonoBehaviour
    {
        // HistoryEntryUnityEvent.
        [Serializable]
        public class HistoryEntryUnityEvent : UnityEvent<HistoryEntry> { }

        // ChessUndoManager.
        [Header("Events - History")]
        [Tooltip("An event that is invoked when the last move is undone.")]
        public HistoryEntryUnityEvent MoveUndone;
        [Tooltip("An event that is invoked when the last undone move was redone.")]
        public HistoryEntryUnityEvent MoveRedone;
        [Tooltip("An event that is invoked when this undo managers move history stack becomes empty.")]
        public UnityEvent MoveHistoryEmptied;
        [Tooltip("An event that is invoked when this undo managers move history stack becomes non-empty after being empty.")]
        public UnityEvent MoveHistoryValid;
        [Tooltip("An event that is invoked when this undo managers undo history stack becomes empty.")]
        public UnityEvent UndoHistoryEmptied;
        [Tooltip("An event that is invoked when this undo managers undo history stack becomes non-empty after being empty.")]
        public UnityEvent UndoHistoryValid;

        /// <summary>A reference to the ChessGameManager associated with this component.</summary>
        public ChessGameManager GameManager { get; private set; }
        /// <summary>A reference to the chess InstanceHistory component that provides undo and redo support.</summary>
        public InstanceHistory HistoryManager { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find the 'ChessGameManager' reference.
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            GameManager.GameInitialized.AddListener(OnGameInitialized);
        }

        void OnDisable()
        {
            // If the GameManager is still valid...
            if (GameManager != null)
            {
                // Unsubscribe from relevant event(s).
                GameManager.GameInitialized.RemoveListener(OnGameInitialized);
            }
        }

        // Public method(s).
        /// <summary>Undoes the last move if there is a valid last move tracked.</summary>
        public void Undo()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.MoveHistoryCount > 0)
                HistoryManager.UndoMove();
        }

        /// <summary>Redoes the next move if there is a valid next move tracked.</summary>
        public void Redo()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.UndoneMovesCount > 0)
                HistoryManager.RedoMove();
        }
		
		/// <summary>Clears all history entries.</summary>
        public void ClearHistory()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.MoveHistoryCount > 0)
                HistoryManager.ClearHistory();
        }

        /// <summary>Clears all undone move entries.</summary>
        public void ClearUndoneMoves()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.UndoneMovesCount > 0)
                HistoryManager.ClearUndoneMoves();
        }

        // Private callback(s).
        /// <summary>Invoked whenever a chess game is initialized on the relevant chess game manager.</summary>
        void OnGameInitialized()
        {
            // Instantiate a new 'HistoryManager'.
            HistoryManager = new InstanceHistory(GameManager.ChessInstance);

            // Subscribe to relevant 'HistoryManager' event(s).
            HistoryManager.MoveUndone += OnMoveUndone;
            HistoryManager.MoveRedone += OnMoveRedone;
            HistoryManager.MoveHistoryEmptied += OnMoveHistoryEmptied;
            HistoryManager.MoveHistoryValid += OnMoveHistoryValid;
            HistoryManager.UndoHistoryEmptied += OnUndoHistoryEmptied;
            HistoryManager.UndoHistoryValid += OnUndoHistoryValid;
        }

        /// <summary>Invoked whenever this undo manager undoes a chess move.</summary>
        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        /// <param name="pEntry">The HistoryEntry describing the move that was undone.</param>
        void OnMoveUndone(Instance pChessInstance, HistoryEntry pEntry)
        {
            // Invoke the 'MoveUndone' Unity event.
            MoveUndone?.Invoke(pEntry);
        }

        /// <summary>Invoked whenever this undo manager redoes a chess move.</summary>
        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        /// <param name="pEntry">The HistoryEntry describing the move that was redone.</param>
        void OnMoveRedone(Instance pChessInstance, HistoryEntry pEntry)
        {
            // Invoke the 'MoveRedone' Unity event.
            MoveRedone?.Invoke(pEntry);
        }

        /// <summary>Invokde whenever the 'move history' stack of the relevant HistoryManager for this component becomes empty.</summary>
        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        void OnMoveHistoryEmptied(Instance pChessInstance)
        {
            // Invoke the 'MoveHistoryEmptied' Unity event.
            MoveHistoryEmptied?.Invoke();
        }

        /// <summary>Invokde whenever the 'move history' stack of the relevant HistoryManager for this component becomes non-empty after being empty.</summary>
        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        void OnMoveHistoryValid(Instance pChessInstance)
        {
            // Invoke the 'MoveHistoryValid' Unity event.
            MoveHistoryValid?.Invoke();
        }

        /// <summary>Invokde whenever the 'undo history' stack of the relevant HistoryManager for this component becomes empty.</summary>
        /// <param name="pChessInstance">The chess engine Instance whose undo history is being tracked.</param>
        void OnUndoHistoryEmptied(Instance pChessInstance)
        {
            // Invoke the 'UndoHistoryEmpteid' Unity event.
            UndoHistoryEmptied?.Invoke();
        }

        /// <summary>Invokde whenever the 'undo history' stack of the relevant HistoryManager for this component becomes non-empty after being empty.</summary>
        /// <param name="pChessInstance">The chess engine Instance whose undo history is being tracked.</param>
        void OnUndoHistoryValid(Instance pChessInstance)
        {
            // Invoke the 'UndoHistoryValid' Unity event.
            UndoHistoryValid?.Invoke();
        }
    }
}
