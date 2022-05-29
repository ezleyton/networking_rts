using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    public Targetable GetTarget => target; // => c# lambda operator
    //public void SetTarget(Targetable t) => target = t; 

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        Debug.Log("Setting target!");
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable hitTarget)) return;
        Debug.Log("Target acquired!");
        target = hitTarget;
        Debug.Log(target);
    }

    #endregion

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #region Client

    #endregion
}