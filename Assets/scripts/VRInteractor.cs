using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRInteractor : MonoBehaviour
{
  [Header("Controls")] 
  public Valve.VR.EVRButtonId InteractButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
  public Valve.VR.EVRButtonId ReleaseButton = Valve.VR.EVRButtonId.k_EButton_Grip;

  [Header("Filtering")]
  public float InteractRadius = 0.125f;
  public LayerMask InteractFilter = -1;

  protected SteamVR_TrackedObject m_TrackedObject;
  protected VRInteractable m_HeldInteractable = null;
  
  public bool IsHoldingObject { get { return m_HeldInteractable != null; } }
  public GameObject HeldObject { get { return m_HeldInteractable.gameObject; } }
  
  public SteamVR_Controller.Device Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }

  protected void Update() {
    if(IsHoldingObject) {
      if(Controller != null && Controller.GetPressDown(ReleaseButton)) {
	release();
      }
    } else {
      if(Controller != null && Controller.GetPressDown(InteractButton)) {
	pickup(findInteractable());
      }
    }
  }
  
  protected void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, InteractRadius);
  }

  protected bool pickup(VRInteractable target) {
    if(target != null && target.IsInteractable(this)) {
      target.BroadcastMessage("Interact", this, SendMessageOptions.DontRequireReceiver);
      
      if(target.IsPickupable) {
	m_HeldObject = target.gameObject
	m_HeldInteractable = target;
	m_HeldRigidbody = target.GetComponentInParent<Rigidbody>();
      }

      return true;
    }

    return false;
  }

  protected bool release() {
    if(IsHoldingObject) {
      m_HeldInteractable.BroadcastMessage("Release", this, SendMessageOptions.DontRequireReceiver);
      m_HeldObject = null;
      m_HeldInteractable = null;
      m_HeldRigidbody = null;
      
      return true;
    }

    return false;
  }
  
  protected GameObject findInteractableObject() {
    double interactable_closest_distance = System.Double.MaxValue;
    double rigibody_closest_distance = System.Double.MaxValue;
    GameObject target_object;
    
    Collider[] colliders = Physics.OverlapSphere(this.transform.position, InteractRadius, InteractFilter, QueryTriggerInteraction.Collide);

    // Find the closest interactable object
    foreach(Collider possible_target in colliders) {
      GameObject possible_object = null;
      VRInteractable marked_interactable = possible_target.GetComponentInParent<VRInteractable>();

      // Check for a possible interactable or a rigidbody
      if(marked_interactable) {
	if(marked_interactable.IsInteractable) {
	  possible_object = marked_interactable.gameObject;
	}
      } else {
	Rigidbody marked_rigidbody = possible_target.GetComponentInParent<Rigidbody>();

	if(marked_rigidbody) {
	  possible_object = marked_rigidbody.gameObject;
	}
      }

      // If we found an acceptable object, calculate and compare the distance
      if(possible_object) {
	double distance = (possible_object.transform.position - this.transform.position).sqrMagnitude;

	if(distance < closest_distance) {
	  closest_distance = distance;
	  target_object = possible_object;
	}
      }
    }
    
    return target_object;
  }
}
