using UnityEngine;
using UnityEngine.Events;

namespace ChessEngine.Game.Invokers
{
    /// <summary>
    /// A component that invokes an event, Triggered, on Start().
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InvokeEventOnStart : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked on Start().")]
        public UnityEvent Triggered;
		
        // Unity callback(s).
        void Start()
        {
			// Invoke the 'Triggered' unity event.
			Triggered?.Invoke();
        }
    }
}
