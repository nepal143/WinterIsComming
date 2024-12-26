
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricSplitter : ElectricComponent
    {
        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            // When connect a wire
            ActivateOutput();
        }

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            // When disconnect a wire
            // TODO : Think if we need activate on output too?
            if (isInput)
            {
                GetSetIsEnergized = false;
                GetSetIsOn = false;
                ActivateOutput();
            }
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            base.EnergizeByWire(onOff, index);

            GetSetIsOn = onOff;

            ActivateOutput();
        }

        #endregion
    }
}
