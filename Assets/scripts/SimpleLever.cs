using UnityEngine;
using System.Collections;

public class SimpleLever : MonoBehaviour
{
  public GameObject SpawnPrefab;
  public Transform SpawnAnchor;
  
  public void Impulse() {
    Instantiate(SpawnPrefab, SpawnAnchor.position, Quaternion.identity);
  }
}
