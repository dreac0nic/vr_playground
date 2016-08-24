using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRInteractor : MonoBehaviour
{
  public float InteractRadius = 0.125f;
  public Valve.VR.EVRButtonId InteractButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
  public LayerMask InteractLayer = -1;

  protected SteamVR_TrackedObject m_TrackedObject;

  protected SteamVR_Controller.Device m_Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }

  protected void Update() {
    if(m_Controller != null) {
      double closest_distance = System.Double.MaxValue;
      Collider[] proximity_colliders = Physics.OverlapSphere(this.transform.position, InteractRadius, InteractLayer, QueryTriggerInteraction.Collide);
      VRInteractable target_interactable = null;

      // Find the closest interactable
      foreach(Collider possible_interactable in proximity_colliders) {
	VRInteractable interactable = possible_interactable.GetComponentInParent<VRInteractable>();

	if(interactable && interactable.IsInteractable(this)) {
	  double distance = (interactable.transform.position - this.transform.position).sqrMagnitude;

	  if(distance < closest_distance) {
	    closest_distance = distance;
	    target_interactable = interactable;
	  }
	}
      }

      // Interact with the found object
      if(target_interactable) {
	target_interactable.Interact(this);
      }
    }
  }

  protected void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(transform.position, InteractRadius);
  }
}
