using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu( "Layout/CanvasScaleBroadcaster" )]
[ExecuteInEditMode]
[RequireComponent( typeof( Canvas ) )]
public class CanvasScaleBroadcast : CanvasScaler {

    [SerializeField]
    public string[] m_Messages = new string[] { "SetDirty" };

    protected override void Handle() {
        base.Handle();

        foreach( string msg in m_Messages )
            BroadcastMessage( msg, SendMessageOptions.DontRequireReceiver );
    }
}
