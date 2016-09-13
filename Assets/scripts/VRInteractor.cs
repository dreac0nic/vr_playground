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
  
  protected SteamVR_Controller.Device m_Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }

  protected void Update() {
    if(IsHoldingObject) {
    } else {
      if(m_Controller != null && m_Controller.GetPressDown(InteractButton)) {
      }
    }
  }
  protected void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, InteractRadius);
  }

  protected bool pickup(VRInteractable target) {
    if(target != null && target.IsInteractable(this)) {
      m_HeldInteractable = target;
      target.Interact(this);

      return true;
    }

    return false;
  }

  protected bool release() {
    if(IsHoldingObject) {
      m_HeldInteractable.Release(this);
      m_HeldInteractable = null;
      
      return true;
    }

    return false;
  }

  protected VRInteractable findInteractable() {
    double closest_distance = System.Double.MaxValue;
    Collider[] colliders = Physics.OverlapSphere(this.transform.position, InteractRadius, InteractFilter, QueryTriggerInteraction.Collider);
    VRInteractable target_interactable = null;

    // Find the closest interactable object
    foreach(Collider possible_interactable in colliders) {
      VRInteractable marked_interactable = possible_interactable.GetComponentInParrent<VRInteractable>();

      if(marked_interactable && marked_interactable.IsInteractable(this)) {
	double distance = (interactable.transform.position - this.transform.position).sqrMagnitude;

	if(distance < closest_distance) {
	  closest_distance = distance;
	  target_interactable = marked_interactable;
	}
      }
    }
    
    return target_interactable;
  }
}
