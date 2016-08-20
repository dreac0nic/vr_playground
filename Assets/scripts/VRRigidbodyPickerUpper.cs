using UnityEngine;
using System.Collections;

public class VRRigidbodyPickerUpper : MonoBehaviour
{
  public float GrabRadius = 0.5f;
  
  protected bool m_IsGrabbing = false;
  protected Rigidbody m_TargetRigidbody;
  
  protected void Update() {
    m_IsGrabbing = false; // TODO: Retrieve trigger state.

    if(m_IsGrabbing && !m_TargetRigidbody) {
      double closest_distance = Double.MaxValue;
      Collider[] colliders = Physics.OverlapSphere(this.transform.position);

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

  protected void FixedUpdate() {
    if(m_TargetRigidbody) {
      m_TargetRigidbody.transform.position = this.transform.position;
      m_TargetRigidbody.transform.rotation = this.transform.rotation;
    }
  }
}
