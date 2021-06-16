using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Panel
{
    public class LoadingPanel : PanelBase
    {
        public override void Close()
        {
            Debug.Log("closedxxx");
            base.Close();
        }
    }

}
