using UnityEngine;

/// <summary>
///     Rotates parent transform to look at target transform.
/// </summary>
public class LookAtTarget : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform = null;

    private void Update()
    {
        if (targetTransform != null)
        {
            transform.LookAt(targetTransform);
        }
    }
}