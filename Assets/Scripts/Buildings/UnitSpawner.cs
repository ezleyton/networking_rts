using System;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform unitSpawnPoint;

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDestroy += ServerHandleOnDestroy;
    }

    public override void OnStopServer()
    {
        health.ServerOnDestroy -= ServerHandleOnDestroy;
    }
    [Server]
    private void ServerHandleOnDestroy()
    {
        //NetworkServer.Destroy(gameObject); //moved to UnitBase
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }
    

    #endregion

    #region Client
    
    public void OnPointerClick(PointerEventData eventData) //llama a este metodo cuando se hace click sobre este objeto
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        
        if (!hasAuthority) {return;}
        
        CmdSpawnUnit();
    }
    

    #endregion

    
}
