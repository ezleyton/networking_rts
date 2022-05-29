using System;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform unitSpawnPoint;

    #region Server

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
