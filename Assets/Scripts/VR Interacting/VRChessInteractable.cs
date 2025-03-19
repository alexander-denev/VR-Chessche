using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRChessInteractable : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor) 
        {
            Debug.Log(gameObject.name + " grabbed by socket!");
        } else
        {
            Debug.Log(gameObject.name + " grabbed by " + args.interactorObject);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            Debug.Log(gameObject.name + " released by socket!");
        }
        else
        {
            Debug.Log(gameObject.name + " released by " + args.interactorObject);
        }
    }
}
