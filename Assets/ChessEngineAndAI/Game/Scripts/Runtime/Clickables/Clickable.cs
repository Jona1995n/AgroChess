using System;
using UnityEngine;
using UnityEngine.Events;

namespace ChessEngine.Game.Clickables
{
    /// <summary>
    /// A component intended to be attached to a GameObject that makes it 'Clickable'.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class Clickable : MonoBehaviour
    {
        /// <summary>
        /// Arg0: Clicker - The Clicker involved in the event.
        /// Arg1: Vector2 - The click position in screen space.
        /// </summary>
        [Serializable]
        public class ClickedUnityEvent : UnityEvent<Clicker, Vector2> { }

        // Clickable.
        #region Editor Serialized Events
        [Header("Events")]
        [Tooltip("An event that is invoked when this Clickable is primary clicked by a Clicker.\n\nArg0: Clicker - the Clicker that clicked this Clickable.\nArg1: Vector2 - The click position in screen space.")]
        public ClickedUnityEvent PrimaryClicked;
        [Tooltip("An event that is invoked when this Clickable is secondary clicked by a Clicker.\n\nArg0: Clicker - the Clicker that clicked this Clickable.\nArg1: Vector2 - The click position in screen space.")]
        public ClickedUnityEvent SecondaryClicked;
        [Tooltip("An event that is invoked when this Clickable becomes the hover target of a Clicker.")]
        public UnityEvent HoverEntered;
        [Tooltip("An event that is invoked when this Clickable is no longer the hover target of a Clicker.")]
        public UnityEvent HoverExited;
        #endregion
        #region Public Properties
        /// <summary>Returns true if the selector is hovering over this Clickable, otherwise false.</summary>
        public bool IsHovered { get; private set; }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void Start() { } // NOTE: This is only here to ensure the 'enabled' checkbox appears in the Unity editor.
        #endregion

        // Interal callback(s).
        #region Internal Click Callbacks
        internal void Internal_OnPrimaryClick(Clicker pClicker, Vector2 pClickPos)
        {
            // Invoke the PrimaryClicked Unity event.
            PrimaryClicked?.Invoke(pClicker, pClickPos);
        }

        internal void Internal_OnSecondaryClick(Clicker pClicker, Vector2 pClickPos)
        {
            // Invoke the SecondaryClicked Unity event.
            SecondaryClicked?.Invoke(pClicker, pClickPos);
        }
        #endregion
        #region Internal Hover Callbacks
        internal void Internal_OnHoverEnter()
        {
            // Is being hovered over.
            IsHovered = true;

            // Invoke the 'HoverEntered' unity event.
            HoverEntered?.Invoke();
        }

        internal void Internal_OnHoverExit()
        {
            // Is not being hovered over.
            IsHovered = false;

            // Invoke the 'HoverExited' unity event.
            HoverExited?.Invoke();
        }
        #endregion
    }
}
