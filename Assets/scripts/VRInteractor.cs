using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRInteractor : MonoBehaviour
{
  [Header("Controls")] 
  public Valve.VR.EVRButtonId InteractButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
  public Valve.VR.EVRButtonId ReleaseButton = Valve.VR.EVRButtonId.k_EButton_Grip;

  [Header("Configuration")]
  public bool AllowRigidbodies = true;
  public bool StickyRigidbodies = false; // Requires the release button be pushed to drop a rigidbody.
  public bool AlignRigidbodies = false;
  public float RigidbodyReleaseForceFactor = 100.0f;

  [Header("Filtering")]
  public float InteractRadius = 0.125f;
  public LayerMask InteractFilter = -1;

  protected SteamVR_TrackedObject m_TrackedObject;
  protected Rigidbody m_HeldRigidbody = null; // XXX:         This could cause some discrepency as an interactable could also have a rigidbody.
  protected VRInteractable m_HeldInteractable = null; // XXX: ... Consider more elegant solution in the future.
  
  public bool IsHoldingObject { get { return m_HeldRigidbody || m_HeldInteractable; } }
  
  protected SteamVR_Controller.Device m_Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }

  protected void Update() {
    if(m_HeldRigidbody) {
      // If rigidbody is let go, release it, otherwise lock the rigidbody to the controller
      if(false && (StickyRigidbodies && m_Controller != null && m_Controller.GetPressDown(ReleaseButton)) || (m_Controller == null || !m_Controller.GetPressDown(InteractButton))) {
	release();
      } else {
	Debug.Log("Holding rigidbody: " + m_HeldRigidbody.gameObject.name);
	m_HeldRigidbody.transform.position = this.transform.position;
	m_HeldRigidbody.transform.rotation = this.transform.rotation;
      }
    } else {
      // If attempting to interact, check for objects
      if(m_Controller != null && m_Controller.GetPressDown(InteractButton)) {
	pickup();
      }
    }
  }

  protected void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, InteractRadius);
  }

  protected void pickup() {
    Debug.Log("PICKING UP");
    
    // TODO: Make public and allow to give an item specifically
    double interactable_distance = System.Double.MaxValue;
    double rigidbody_distance = System.Double.MaxValue;
    VRInteractable target_interactable = null;
    Rigidbody target_rigidbody = null;

    Collider[] proximity_colliders = Physics.OverlapSphere(this.transform.position, InteractRadius, InteractFilter, QueryTriggerInteraction.Collide);

    // Find the closest interactable
    foreach(Collider possible_interactable in proximity_colliders) {
      Rigidbody rigidbody = possible_interactable.GetComponentInParent<Rigidbody>();
      VRInteractable interactable = possible_interactable.GetComponentInParent<VRInteractable>();

      if(interactable && interactable.IsInteractable(this)) {
	double distance = (interactable.transform.position - this.transform.position).sqrMagnitude;

	if(distance < interactable_distance) {
	  interactable_distance = distance;
	  target_interactable = interactable;
	}
      } else if(rigidbody) {
	double distance = (rigidbody.transform.position - this.transform.position).sqrMagnitude;

	if(distance < rigidbody_distance) {
	  rigidbody_distance = distance;
	  target_rigidbody = rigidbody;
	}
      }
    }

    // Interact with the found object
    if(target_interactable) {
      m_HeldInteractable = target_interactable;
      target_interactable.Interact(this);
    } else if(target_rigidbody) {
      Debug.Log("Picking up rigidbody: " + target_rigidbody.gameObject.name);
      m_HeldRigidbody = target_rigidbody;
      m_HeldRigidbody.isKinematic = true;
    }
  }

  protected void release() {
    Debug.Log("Releasing ");
    if(m_HeldRigidbody) {
      m_HeldRigidbody.isKinematic = false;
    }

    m_HeldRigidbody = null;
    m_HeldInteractable = null;
  }
}
