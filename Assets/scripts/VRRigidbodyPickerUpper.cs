using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRRigidbodyPickerUpper : MonoBehaviour
{
  public float GrabRadius = 0.125f;
  public Valve.VR.EVRButtonId GrabButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
  public LayerMask GrabLayer = -1;
  
  protected bool m_IsGrabbing = false;
  protected Rigidbody m_TargetRigidbody;
  protected SteamVR_TrackedObject m_TrackedObject;

  protected SteamVR_Controller.Device m_Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }
  
  protected void Update() {
    if(m_Controller != null) {
      m_IsGrabbing = m_Controller.GetPress(GrabButton);
    } else if(m_IsGrabbing) {
      m_IsGrabbing = false; // Release object if controller is lost

      Debug.Log("Error: Could not find controller.");
    }

    // Put thing in position
    if(m_TargetRigidbody) {
      m_TargetRigidbody.transform.position = this.transform.position;
      m_TargetRigidbody.transform.rotation = this.transform.rotation;
    }
  }

  protected void FixedUpdate() {
    // Attempt to find a thing to grab
    if(m_IsGrabbing && !m_TargetRigidbody) {
      double closest_distance = System.Double.MaxValue;
      Collider[] colliders = Physics.OverlapSphere(this.transform.position, GrabRadius, GrabLayer, QueryTriggerInteraction.Ignore);

      // Find a new rigidbody to target
      foreach(Collider possible_target in colliders) {
	Rigidbody body = possible_target.GetComponentInParent<Rigidbody>();

	// Test distance of the rigidbody, if it exists
	if(body) {
	  double distance = (body.transform.position - this.transform.position).sqrMagnitude;

	  if(distance < closest_distance) {
	    closest_distance = distance;
	    m_TargetRigidbody = body;
	  }
	}
      }

      // Setup the rigidbody for absolute tracking
      if(m_TargetRigidbody) {
	m_TargetRigidbody.isKinematic = true;
      }
    } else if(m_TargetRigidbody) {
      m_TargetRigidbody.isKinematic = false;
      m_TargetRigidbody = null;
    }
  }
}
