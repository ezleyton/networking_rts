using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;
    
    
    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDestroy += ServerHandleDestroy;
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDestroy -= ServerHandleDestroy;
        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client



    #endregion
}
