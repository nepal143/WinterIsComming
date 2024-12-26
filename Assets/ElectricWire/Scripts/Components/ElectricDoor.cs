
//(c8

namespace ElectricWire
{
    public class ElectricDoor : ElectricComponent
    {
        public bool oneSideOnlyConnection = false;

        #region IWire interface

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            // We unenergize by ourself
            GetSetIsEnergized = false;
            SetIsOn(GetSetIsEnergized);
        }

        public override bool IsWireConnected(bool isInput, int index)
        {
            if (!oneSideOnlyConnection)
                return base.IsWireConnected(isInput, index);
            else
                return wireInputIsConnected.Length > index && (wireInputIsConnected[0] || wireInputIsConnected[1]);
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            GetSetIsEnergized = onOff;
            SetIsOn(GetSetIsEnergized);
            //ActivateByWire(GetSetIsEnergized ? "1" : "");
        }

        #endregion

        private void OnMouseDown()
        {
            if (ElectricManager.electricManager.CanTriggerComponent())
                SetIsOn(!IsOn());
        }
    }
}
