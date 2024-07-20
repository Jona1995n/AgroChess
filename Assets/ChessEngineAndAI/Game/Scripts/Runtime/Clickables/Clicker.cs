using System;
using UnityEngine;
using UnityEngine.Events;
using ChessEngine.Game.Clickables.Events;

namespace ChessEngine.Game.Clickables
{
    /// <summary>
    /// The base class for all Clickers that can click on Clickables.
    /// Provides public methods for clicking on a Clickable.
    /// Intended to be overridden by specialized 'Clickers' that parse user input to click Clickables.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class Clicker : MonoBehaviour
    {
        // ClickedUnityEvent.
        /// <summary>
        /// Arg0: Clickable - The Clickable that was clicked.
        /// Arg1: Vector3 - The screen space click position.
        /// </summary>
        [Serializable]
        public class ClickedUnityEvent : UnityEvent<Clickable, Vector2> { }

        // Clicker.
        #region Editor Serialized Settings & Events
        [Header("Settings - Clicker")]
        [Tooltip("Should the highlighted component be unhighlighted when this component is diabled?")]
        public bool unhoverOnDisable = true;

        [Header("Events")]
        [Tooltip("An event that is invoked whenver this Clicker primary clicks a Clickable.\n\nArg0: Clickable - The Clickable that was clicked.")]
        public ClickedUnityEvent PrimaryClicked;
        [Tooltip("An event that is invoked whenver this Clicker secondary clicks a Clickable.\n\nArg0: Clickable - The Clickable that was clicked.")]
        public ClickedUnityEvent SecondaryClicked;
        [Tooltip("An event that is invoked when the hover target of this Clicker is changed.\n\nArg0: Clickable - The last hovered CLickable, or null.\nArg1: Clickable - The new hovered Clickable, or null.")]
        public HoverTargetChangedUnityEvent HoverTargetChanged;
        #endregion
        #region Public Properties
        /// <summary>Returns the Clickable being hovered over, or null.</summary>
        public Clickable HoveringOver { get; protected set; }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        protected virtual void OnDisable()
        {
            // Unhighlight on disable if set.
            if (unhoverOnDisable)
                ForceStopHover();
        }
        #endregion

        // Public method(s).
        #region Hover Methods
        /// <summary>A public method that sets the unhoverOnDisable field of this component. Useful for use with Unity editor events.</summary>
        /// <param name="pUnhover"></param>
        public void SetUnhoverOnDisable(bool pUnhover) { unhoverOnDisable = pUnhover; }

        /// <summary>Forcefully stops the current hover.</summary>
        public void ForceStopHover()
        {
            if (HoveringOver != null)
            {
                Clickable lastHoveringOver = HoveringOver;
                OnHoverExit(HoveringOver);

                // Invoke the 'HoverTargetChanged' event.
                HoverTargetChanged?.Invoke(lastHoveringOver, HoveringOver);
            }
        }
        #endregion
        #region Click Methods
        /// <summary>Performs a primary click on the given Clickable at the given screen space 'click pos'.</summary>
        /// <param name="pClickable"></param>
        /// <param name="pClickPos"></param>
        public void PrimaryClickClickable(Clickable pClickable, Vector2 pClickPos)
        {
            // Invoke the 'PrimaryClicked' unity event.
            PrimaryClicked?.Invoke(pClickable, pClickPos);

            // Invoke the Clickable's internal OnPrimaryClick event.
            pClickable.Internal_OnPrimaryClick(this, pClickPos);
        }

        public void SecondaryClickClickable(Clickable pClickable, Vector2 pClickPos)
        {
            // Invoke the 'SecondaryClicked' unity event.
            SecondaryClicked?.Invoke(pClickable, pClickPos);

            // Invoke the Clickable's internal OnSecondaryClick event.
            pClickable.Internal_OnSecondaryClick(this, pClickPos);
        }
        #endregion

        // Protected callback(s).
        #region Protected Hover Callbacks
        protected void OnHoverEnter(Clickable pClickable)
        {
            // Update 'HoveringOver'.
            HoveringOver = pClickable;

            // Invoke internal OnHoverEnter for pClickable.
            HoveringOver.Internal_OnHoverEnter();
        }

        protected void OnHoverExit(Clickable pClickable)
        {
            // Invoke internal OnHoverExit for the previous Clickable.
            HoveringOver.Internal_OnHoverExit();

            // Nullify 'HoveringOver'.
            HoveringOver = null;
        }
        #endregion
    }
}
