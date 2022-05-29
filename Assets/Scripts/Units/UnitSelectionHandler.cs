using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    
    public List<Unit> SelectedUnits { get; } = new List<Unit>();
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea = null;

    private Vector2 startPosition; //drag start position
    private RTSPlayer _player;
    
    
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>(); //the problem with this is that the player is not ready by the time this is called
        //for this there's a workaround in the update checking if the player is null
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
            
        } else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        } else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }

            SelectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition 
                                             + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {

        unitSelectionArea.gameObject.SetActive(false);
        if (unitSelectionArea.sizeDelta.magnitude == 0) //selecting one single unit
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;
        
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
        
            if (!unit.hasAuthority) return;

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);
        foreach (Unit unit in _player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)) continue;
            
            //screenspace to worldspace
            Vector3 screePosition = _mainCamera.WorldToScreenPoint(unit.transform.position); //unit to screen position
            if (screePosition.x > min.x 
                && screePosition.x < max.x
                && screePosition.y > min.y 
                && screePosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }

    }
}
