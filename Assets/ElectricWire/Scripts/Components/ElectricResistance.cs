
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricResistance : ElectricComponent
    {
        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            // If connect to input 0 .. activate only input 0 for this wire

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

        public override void ActivateOutput()
        {
            // Activating output mean activating the input of the other end of the wire

            for (int i = 0; i < wireOutput.Length; i++)
            {
                if (wireOutput[i] != null)
                    if (IsWireConnected(false, i))
                        wireOutput[i].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(IsEnergized() && IsOn());
            }
        }
    }
}
