using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class WaterBucket : MonoBehaviour
{
    [SerializeField] private GameObject waterInside;
    private XRGrabInteractable grabInteractable;
    private Rigidbody bucketBody;

    public bool HasWater { get; private set; }
    public bool IsHeld => grabInteractable != null && grabInteractable.isSelected;
    public int LastFilledFrame { get; private set; } = -1;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        bucketBody = GetComponent<Rigidbody>();
        // Move through physics while held; do not throw the bucket on release.
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.throwOnDetach = false;
        bucketBody.centerOfMass = new Vector3(0f, 0.08f, 0f);
        bucketBody.linearDamping = 1f;
        bucketBody.angularDamping = 5f;
        StandUpright();
        HasWater = false;
        UpdateVisual();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        bucketBody.constraints = RigidbodyConstraints.None;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (!IsHeld) StandUpright();
    }

    private void StandUpright()
    {
        bucketBody.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        bucketBody.constraints = RigidbodyConstraints.FreezeRotation;
        if (!bucketBody.isKinematic)
        {
            bucketBody.linearVelocity = Vector3.zero;
            bucketBody.angularVelocity = Vector3.zero;
        }
    }

    public bool Fill()
    {
        if (HasWater) return false;
        HasWater = true;
        LastFilledFrame = Time.frameCount;
        UpdateVisual();
        return true;
    }

    public bool Empty()
    {
        if (!HasWater) return false;
        HasWater = false;
        UpdateVisual();
        return true;
    }

    private void UpdateVisual()
    {
        if (waterInside != null) waterInside.SetActive(HasWater);
    }
}
