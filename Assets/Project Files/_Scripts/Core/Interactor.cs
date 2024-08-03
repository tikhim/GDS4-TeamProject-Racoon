using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable {
    public void OnInteract(GameObject source);
}

public class Interactor : MonoBehaviour
{
    [Header("References")]
    private PlayerInput _input;
    private Inventory _inv;

    [Header("Interactor")]
    [SerializeField] private LayerMask layer;
    [SerializeField] private Vector3 offset = new(0f, 1f, 0f);
    [SerializeField] private Vector3 halfExtents = new(.5f, 1f, .5f);
    [SerializeField] private float distance = 5f;
    



    private void Awake()
    {
        _input = new PlayerInput();
        _inv = GetComponent<Inventory>(); // not sure if best to access inventory in this code
    }

    #region Input

    private void OnEnable() {
        _input.Enable();

        //Subscribe Player Input to Events
        _input.Player.Consume.started += OnInteract;
    }

    private void OnDisable() {
        _input.Disable();

        //Unsubscribe Player Input to Events
        _input.Player.Consume.started -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext value) {

        Vector3 centre = transform.position + offset;
        
        if (Physics.BoxCast(centre, halfExtents, transform.up, out RaycastHit hit, transform.rotation, distance, layer) && hit.collider.gameObject.TryGetComponent(out IInteractable other)) {
            other.OnInteract(gameObject);
        } else if (_inv.heldItem != null) {
            // not sure if this is the best approach
            _inv.DiscardItem();
            return;
        }
    }
    #endregion
}
