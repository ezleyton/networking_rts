using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private UnitMovement _unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private Targeter targeter = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;


    public Targeter GetTargeter()
    {
        return targeter;
    }
    public UnitMovement GetUnitMovement()
    {
        return _unitMovement;
    }
    
    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        _health.ServerOnDestroy += ServerHandleOnDestroy;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        _health.ServerOnDestroy -= ServerHandleOnDestroy;
    }

    private void ServerHandleOnDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnUnitDespawned?.Invoke(this);
    }


    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;
        
        onDeselected?.Invoke();
    }
    #endregion
}
