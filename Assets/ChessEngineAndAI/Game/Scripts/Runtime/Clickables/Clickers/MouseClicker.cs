using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ChessEngine.Game.Clickables
{
    /// <summary>
    /// An implementation of a Clicker that uses a simple primary and secondary click inputs and the mouse position to fire click actions.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class MouseClicker : Clicker
    {
        #region Editor Serialized Settings
        [Header("Settings - Mouse Clicker")]
        [Tooltip("(Optional) A reference to the Camera to use when raycasting. If not set CameraReference will be used.")]
        [SerializeField] private Camera m_CameraOverride;
        [Tooltip("Should Clickables with their 'enabled' field set to false still be clickable?")]
        public bool clickDisabledClickables;
        [Tooltip("A layer mask of layers to be ignored when looking for layers raycasting from the mouse pointer.")]
        public LayerMask ignorePointerRaycastLayers;
        [Tooltip("The maximum distance this Clicker may click Clickables at.")]
        public float maxClickDistance = 100f;

#if ENABLE_INPUT_SYSTEM
        [Header("Inputs")]
        [Tooltip("Should this component bind default actions on start if none are set?")]
        public bool bindDefaultActions = true;
        [Tooltip("A reference to an input action or property that fires the 'primary click' input for this component.")]
        public InputActionProperty primaryClickInput;
        [Tooltip("A reference to an input action or property that fires the 'secondary click' input for this component.")]
        public InputActionProperty secondaryClickInput;
#elif ENABLE_LEGACY_INPUT_MANAGER
        [Header("Inputs")]
        [Tooltip("A reference to the button that fires the 'primary click' input for this component.")]
        public KeyCode primaryClickInput = KeyCode.Mouse0;
        [Tooltip("A reference to the button that fires the 'secondary click' input for this component.")]
        public KeyCode secondaryClickInput = KeyCode.Mouse1;
#endif
        #endregion
        #region Public Properties
        /// <summary>A reference to the Camera that is used in raycasting done by this component.</summary>
        public Camera CameraReference { get { return m_CameraOverride != null ? m_CameraOverride : Camera.main; } }
        /// <summary>Returns the current mouse position.</summary>
        public Vector2 MousePosition
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#elif ENABLE_LEGACY_INPUT_MANAGER
                return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
            }
        }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        protected virtual void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            // Bind actions.
            BindActions();
#endif
        }

        protected override void OnDisable()
        {
            // Invoke the base type 'OnDisable()' callback.
            base.OnDisable();

            // Unhighlight on disable if set.
            if (unhoverOnDisable)
                ForceStopHover();

#if ENABLE_INPUT_SYSTEM
            // Unbind actions.
            UnbindActions();
#endif
        }

        void Update()
        {
            // Determine if the mouse if hovering over the Clickable.
            if (CameraReference != null)
            {
                // Check if something was hit in a non-ignored layer.
                Ray ray = CameraReference.ScreenPointToRay(MousePosition);
                bool rayHit = Physics.Raycast(ray, out RaycastHit hitInfo, maxClickDistance, ~ignorePointerRaycastLayers, QueryTriggerInteraction.Ignore);
                if (rayHit)
                {
                    // Check for Clickable component collider's gameObject or Rigidbody (if valid).
                    Clickable clickable = hitInfo.collider.GetComponent<Clickable>();
                    if (clickable == null && hitInfo.collider.attachedRigidbody != null)
                        clickable = hitInfo.collider.attachedRigidbody.GetComponent<Clickable>();

                    // Check if 'HoveringOver' has changed.
                    if (HoveringOver != clickable)
                    {
                        // Store last hovering over.
                        Clickable lastHoveringOver = HoveringOver;

                        // Check if a hover was left.
                        if (HoveringOver != null)
                            OnHoverExit(HoveringOver);
                        if (clickable != null && (clickable.enabled || clickDisabledClickables))
                        {
                            OnHoverEnter(clickable);
                        }
                        else { HoveringOver = null; }

                        // Invoke the 'HoverTargetChanged' event.
                        HoverTargetChanged?.Invoke(lastHoveringOver, HoveringOver);
                    }
                }
                else if (HoveringOver != null) { OnHoverExit(HoveringOver); }
            }

#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            // Check for clicks on legacy input system.
            if (Input.GetKeyDown(primaryClickInput))
            {
                // Ignore touches.
                if (!primaryClickInput.Equals(KeyCode.Mouse0) || !Input.touchSupported || Input.touchCount == 0)
                    OnPrimaryClick(MousePosition);
            }
            if (Input.GetKeyDown(secondaryClickInput))
            {
                OnSecondaryClick(MousePosition);
            }
#endif
        }
        #endregion

        // Public method(s).
        #region Public Setting Override Methods
        /// <summary>A public method that sets the maxClickDistance field of this component. Useful for use with Unity editor events.</summary>
        /// <param name="pDistance"></param>
        public void SetMaxClickDistance(float pDistance) { maxClickDistance = pDistance; }
        #endregion

        // Private method(s).
        #region New Input System Methods & Callbacks
#if ENABLE_INPUT_SYSTEM
        void BindActions()
        {
            // Bind primary click input.
            if (primaryClickInput != null && primaryClickInput.action != null && primaryClickInput.action.bindings.Count > 0)
            {
                BindPrimaryClickInput();
            }
            else if (bindDefaultActions)
            {
                InputAction action = new InputAction("PrimaryClick");
                action.AddBinding("<Mouse>/leftButton");
                primaryClickInput = new InputActionProperty(action);
                BindPrimaryClickInput();
            }

            // Bind secondary click input.
            if (secondaryClickInput != null && secondaryClickInput.action != null && secondaryClickInput.action.bindings.Count > 0)
            {
                BindSecondaryClickInput();
            }
            else if (bindDefaultActions)
            {
                InputAction action = new InputAction("SecondaryClick");
                action.AddBinding("<Mouse>/rightButton");
                secondaryClickInput = new InputActionProperty(action);
                BindSecondaryClickInput();
            }
        }

        void UnbindActions()
        {
            // Unbind primary click input.
            if (primaryClickInput != null && primaryClickInput.action != null)
            {
                primaryClickInput.action.canceled -= OnPrimaryClickInput;
            }

            // Unbind secondary click input.
            if (secondaryClickInput != null && secondaryClickInput.action != null)
            {
                secondaryClickInput.action.canceled -= OnSecondaryClickInput;
            }
        }

        void BindPrimaryClickInput()
        {
            primaryClickInput.action.Enable();
            primaryClickInput.action.canceled += OnPrimaryClickInput;
        }

        void BindSecondaryClickInput()
        {
            secondaryClickInput.action.Enable();
            secondaryClickInput.action.canceled += OnSecondaryClickInput;
        }

        // Private callback(s) for new input system.
        void OnPrimaryClickInput(InputAction.CallbackContext pContext) { if (Mouse.current != null) { OnPrimaryClick(Mouse.current.position.ReadValue()); } }
        void OnSecondaryClickInput(InputAction.CallbackContext pContext) { if (Mouse.current != null) { OnSecondaryClick(Mouse.current.position.ReadValue()); } }
#endif
        #endregion

        // Private callback(s).
        #region Private Click Callbacks
        void OnPrimaryClick(Vector2 pClickPos)
        {
            // Determine if the mouse if hovering over the Clickable (only works if there is a valid camera reference).
            if (CameraReference != null)
            {
                // Check if something was hit in a non-ignored layer.
                Ray ray = CameraReference.ScreenPointToRay(pClickPos);
                bool rayHit = Physics.Raycast(ray, out RaycastHit hitInfo, maxClickDistance, ~ignorePointerRaycastLayers, QueryTriggerInteraction.Ignore);
                if (rayHit)
                {
                    // Check for Clickable component collider's gameObject or Rigidbody (if valid).
                    Clickable clickable = hitInfo.collider.GetComponent<Clickable>();
                    if (clickable == null && hitInfo.collider.attachedRigidbody != null)
                        clickable = hitInfo.collider.attachedRigidbody.GetComponent<Clickable>();

                    // Fire primary clicked event.
                    if (clickable != null && (clickable.enabled || clickDisabledClickables))
                        PrimaryClickClickable(clickable, pClickPos);
                }
            }
        }

        void OnSecondaryClick(Vector2 pClickPos)
        {
            // Determine if the mouse if hovering over the Clickable (only works if there is a valid camera reference).
            if (CameraReference != null)
            {
                // Check if something was hit in a non-ignored layer.
                Ray ray = CameraReference.ScreenPointToRay(pClickPos);
                bool rayHit = Physics.Raycast(ray, out RaycastHit hitInfo, maxClickDistance, ~ignorePointerRaycastLayers, QueryTriggerInteraction.Ignore);
                if (rayHit)
                {
                    // Check for Clickable component collider's gameObject or Rigidbody (if valid).
                    Clickable clickable = hitInfo.collider.GetComponent<Clickable>();
                    if (clickable == null && hitInfo.collider.attachedRigidbody != null)
                        clickable = hitInfo.collider.attachedRigidbody.GetComponent<Clickable>();

                    // Fire secondary clicked event.
                    if (clickable != null && (clickable.enabled || clickDisabledClickables))
                        SecondaryClickClickable(clickable, pClickPos);
                }
            }
        }
        #endregion
    }
}
