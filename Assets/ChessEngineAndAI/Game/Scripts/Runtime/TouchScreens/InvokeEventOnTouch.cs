using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ChessEngine.Game.TouchScreens
{
    /// <summary>
    /// A simple component that invokes an event, 'Touched', whenever a new touchscreen touch is recorded.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InvokeEventOnTouch : MonoBehaviour
    {
        // TouchEvent
        /// <summary>
        /// Arg0: Vector2 - the screen space touch position.
        /// </summary>
        [Serializable]
        public class TouchUnityEvent : UnityEvent<Vector2> { };

        // InvokeEventOnTouch.
        #region Editor Serialized Settings & Events
        [Header("Settings")]
        [Tooltip("The number of seconds a touch must be held for before firing the 'TouchHeld' event.")]
        public float touchHoldDelay = 1f;

        [Header("Events")]
        [Tooltip("An event that is invoked whenver a new touch event begins.\n\nArg0: Vector2 - the screen space touch position.")]
        public TouchUnityEvent Touched;
        [Tooltip("An event that is invoked whenever a touch is held for touchHoldDelay seconds.\n\nArg0: Vector2 - the screen space touch position now.")]
        public TouchUnityEvent TouchHeld;
        #endregion
        #region Private Fields
        /// <summary>Has the touch held event been invoked for the current touch yet?</summary>
        bool m_TouchHeldInvoked;
        /// <summary>A coroutine to track a held touch.</summary>
        Coroutine m_HeldTouchCoroutine;
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                foreach (UnityEngine.InputSystem.Controls.TouchControl touch in Touchscreen.current.touches)
                {
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        // Invoke the 'Touched' Unity event.
                        Touched?.Invoke(touch.position.ReadValue());

                        // Stop existing held touch coroutine.
                        StopHeldTouchCoroutine();

                        // Start held touch check.
                        StartHeldTouchCoroutine(touch);
                    }
                }
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Invoke the 'Touched' Unity event.
                        Touched?.Invoke(touch.position);

                        // Stop existing held touch coroutine.
                        StopHeldTouchCoroutine();

                        // Start held touch check.
                        StartHeldTouchCoroutine(touch);
                    }
                }
            }
#endif
        }
        #endregion

        // Private method(s).
        #region Held Touch Checks
#if ENABLE_INPUT_SYSTEM
        void StartHeldTouchCoroutine(UnityEngine.InputSystem.Controls.TouchControl touch)
        {
            m_TouchHeldInvoked = false;
            m_HeldTouchCoroutine = StartCoroutine(Coroutine_HeldTouchCheck(touch));
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        void StartHeldTouchCoroutine(Touch pTouch)
        {
            m_TouchHeldInvoked = false;
            m_HeldTouchCoroutine = StartCoroutine(Coroutine_HeldTouchCheck(pTouch));
        }
#endif

        /// <summary>Stops the held touch coroutine.</summary>
        void StopHeldTouchCoroutine()
        {
            if (m_HeldTouchCoroutine != null)
            {
                StopCoroutine(m_HeldTouchCoroutine);
                m_HeldTouchCoroutine = null;
            }
        }
        #endregion
        #region Coroutine(s)
#if ENABLE_INPUT_SYSTEM
        IEnumerator Coroutine_HeldTouchCheck(UnityEngine.InputSystem.Controls.TouchControl touch)
        {
            float startTime = Time.time;
            while (Time.time - startTime < touchHoldDelay && touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                yield return null;
            }

            if (!m_TouchHeldInvoked && touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                m_TouchHeldInvoked = true;
                TouchHeld.Invoke(touch.position.ReadValue());
            }
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        IEnumerator Coroutine_HeldTouchCheck(Touch pTouch)
        {
            float startTime = Time.time;
            while (Time.time - startTime < touchHoldDelay && pTouch.phase == TouchPhase.Moved)
            {
                yield return null;
            }

            if (!m_TouchHeldInvoked && pTouch.phase == TouchPhase.Moved)
            {
                m_TouchHeldInvoked = true;
                TouchHeld.Invoke(pTouch.position);
            }
        }
#endif
#endregion
    }
}