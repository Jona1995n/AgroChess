using UnityEngine;
using ChessEngine.Game.TouchScreens;

namespace ChessEngine.Game.Clickables
{
    /// <summary>
    /// An implementation of a Clicker that uses touch inputs to click on Clickables.
    /// NOTE: This component does not implement the 'hover over' functionality.
    /// NOTE: Touch holds will fire secondary click events after TouchInvoker.touchHoldDelay seconds.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(InvokeEventOnTouch))]
    public class TouchClicker : Clicker
    {
        #region Editor Serialized Settings
        [Header("Settings - Touch Clicker")]
        [Tooltip("(Optional) A reference to the Camera to use when raycasting. If not set CameraReference will be used.")]
        [SerializeField] private Camera m_CameraOverride;
        [Tooltip("Should Clickables with their 'enabled' field set to false still be clickable?")]
        public bool clickDisabledClickables;
        [Tooltip("A layer mask of layers to be ignored when looking for layers raycasting from the mouse pointer.")]
        public LayerMask ignorePointerRaycastLayers;
        [Tooltip("The maximum distance this Clicker may click Clickables at.")]
        public float maxClickDistance = 100f;
        #endregion
        #region Public Properties
        /// <summary>A reference to the InvokeEventOnTouch component that drives this clicker.</summary>
        public InvokeEventOnTouch TouchInvoker { get; private set; }
        /// <summary>A reference to the Camera that is used in raycasting done by this component.</summary>
        public Camera CameraReference { get { return m_CameraOverride != null ? m_CameraOverride : Camera.main; } }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        protected virtual void Awake()
        {
            // Find 'TouchInvoker' reference.
            TouchInvoker = GetComponent<InvokeEventOnTouch>();
        }

        protected virtual void OnEnable()
        {
            // Subscribe to 'TouchInvoker' events.
            TouchInvoker.Touched.AddListener(OnTouched);
            TouchInvoker.TouchHeld.AddListener(OnTouchHeld);
        }

        protected override void OnDisable()
        {
            // Invoke the base type 'OnDisable()' callback.
            base.OnDisable();

            // Unsubscribe from 'TouchInvoker' events.
            if (TouchInvoker != null)
            {
                TouchInvoker.Touched.RemoveListener(OnTouched);
                TouchInvoker.TouchHeld.RemoveListener(OnTouchHeld);
            }

            // Unhighlight on disable if set.
            if (unhoverOnDisable)
                ForceStopHover();
        }
        #endregion

        // Public method(s).
        #region Public Setting Override Methods
        /// <summary>A public method that sets the maxClickDistance field of this component. Useful for use with Unity editor events.</summary>
        /// <param name="pDistance"></param>
        public void SetMaxClickDistance(float pDistance) { maxClickDistance = pDistance; }
        #endregion

        // Private callback(s).
        #region Protected Touch Callbacks
        /// <summary>Invoked whenever a new touch is detected.</summary>
        /// <param name="pTouchPos"></param>
        protected void OnTouched(Vector2 pTouchPos)
        {
            // Ensure there is a valid camera reference.
            if (CameraReference != null)
            {
                // Check if something was hit in a non-ignored layer.
                Ray ray = CameraReference.ScreenPointToRay(pTouchPos);
                bool rayHit = Physics.Raycast(ray, out RaycastHit hitInfo, maxClickDistance, ~ignorePointerRaycastLayers, QueryTriggerInteraction.Ignore);
                if (rayHit)
                {
                    // Check for Clickable component collider's gameObject or Rigidbody (if valid).
                    Clickable clickable = hitInfo.collider.GetComponent<Clickable>();
                    if (clickable == null && hitInfo.collider.attachedRigidbody != null)
                        clickable = hitInfo.collider.attachedRigidbody.GetComponent<Clickable>();

                    // Fire primary clicked event.
                    if (clickable != null && (clickable.enabled || clickDisabledClickables))
                        PrimaryClickClickable(clickable, pTouchPos);
                }
            }
        }

        /// <summary>Invoked whenever a touch is held for TouchInvoker.touchHoldDelay seconds.</summary>
        /// <param name="pTouchPos"></param>
        protected void OnTouchHeld(Vector2 pTouchPos)
        {
            // Ensure there is a valid camera reference.
            if (CameraReference != null)
            {
                // Check if something was hit in a non-ignored layer.
                Ray ray = CameraReference.ScreenPointToRay(pTouchPos);
                bool rayHit = Physics.Raycast(ray, out RaycastHit hitInfo, maxClickDistance, ~ignorePointerRaycastLayers, QueryTriggerInteraction.Ignore);
                if (rayHit)
                {
                    // Check for Clickable component collider's gameObject or Rigidbody (if valid).
                    Clickable clickable = hitInfo.collider.GetComponent<Clickable>();
                    if (clickable == null && hitInfo.collider.attachedRigidbody != null)
                        clickable = hitInfo.collider.attachedRigidbody.GetComponent<Clickable>();

                    // Fire secondary clicked event.
                    if (clickable != null && (clickable.enabled || clickDisabledClickables))
                        SecondaryClickClickable(clickable, pTouchPos);
                }
            }
        }
        #endregion
    }
}
