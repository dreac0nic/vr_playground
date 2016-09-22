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
  protected Rigidbody m_HeldRigidbody = null;

  private GameObject __HeldObject = null;
  
  public bool IsHoldingObject { get { return m_HeldObject != null; } }
  public GameObject HeldObject { get { return m_HeldObject; } }
  
  public SteamVR_Controller.Device Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected GameObject m_HeldObject {
    get { return __HeldObject; }
    set {
      if(value == null) {
	m_HeldInteractable = null;
	m_HeldRigidbody = null;
      } else {
	m_HeldInteractable = value.GetComponent<VRInteractable>();
	m_HeldRigidbody = value.GetComponent<Rigidbody>();
      }

      __HeldObject = value;
    }
  }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }

  protected void Update() {
    if(IsHoldingObject) {
      // If holding a pure rigidbody, update its position
      if(!m_HeldInteractable) {
	m_HeldRigidbody.transform.position = this.transform.position;
	m_HeldRigidbody.transform.rotation = this.transform.rotation;
      }

      // Release the object if the release button is pressed
      if(Controller != null && Controller.GetPressDown(ReleaseButton)) {
	release();
      }
    } else {
      // Pickup a discovered object if the interact button is pushed
      if(Controller != null && Controller.GetPressDown(InteractButton)) {
	pickup(findInteractableObject());
      }
    }
  }
  
  protected void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, InteractRadius);
  }

  protected bool pickup(GameObject target) {
    if(target != null) {
      VRInteractable interactable = target.GetComponent<VRInteractable>();

      if(interactable && interactable.IsInteractable) {
	if(interactable.IsPickupable) {
	  m_HeldObject = interactable.gameObject;
	  interactable.Pickup(this);
	} else {
	  interactable.Interact(this);
	}
      } else if(!interactable) {
	Rigidbody body = target.GetComponent<Rigidbody>();

	if(body) {
	  body.isKinematic = true;
	  m_HeldObject = body.gameObject;
	}
      }

      return m_HeldObject != null;
    }

    return false;
  }

  protected bool release() {
    if(IsHoldingObject) {
      if(m_HeldInteractable) {
	m_HeldInteractable.Release(this);
      } else {
	// Release a pure rigidbody
	m_HeldRigidbody.isKinematic = false;

	if(Controller != null) {
	  m_HeldRigidbody.AddForce(Controller.velocity, ForceMode.VelocityChange);
	  m_HeldRigidbody.AddTorque(Controller.angularVelocity, ForceMode.VelocityChange);
	}
      }
      
      m_HeldObject = null;
      
      return true;
    }

    return false;
  }
  
  protected GameObject findInteractableObject() {
    double closest_distance = System.Double.MaxValue;
    GameObject target_object = null;
    
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
