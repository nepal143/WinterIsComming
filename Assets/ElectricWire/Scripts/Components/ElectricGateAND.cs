﻿
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricGateAND : ElectricComponent
    {
        #region IWire interface

        public override void ConnectWire(GameObject wire, bool isInput, int index)
        {
            base.ConnectWire(wire, isInput, index);

            // When connect a wire to logic gate
            ActivateOutput();
        }

        public override void DisconnectWire(bool isInput, int index)
        {
            base.DisconnectWire(isInput, index);

            // When disconnect a wire of logic gate
            // TODO : Think if we need activate on output too?
            // Run function EnergizedByWire() to check if one of the input are energized
            if (isInput)
            {
                EnergizeByWire(false, index);
                ActivateOutput();
            }
        }

        public override bool IsOn()
        {
            // Logic gate do not have on/off state
            return IsEnergized();
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // To have energie with a off switch
            // Check all input and set energized true if 1 of them are true
            // Or set false is all of them are false
            bool result = false;
            for (int i = 0; i < wireInput.Length; i++)
            {
                if (wireInput[i] != null && wireInput[i].GetComponent<WireControl>().wireConnectorOutput.IsEnergized())
                {
                    result = true;
                    break;
                }
            }

            GetSetIsEnergized = result;
            ActivateOutput();
        }

        #endregion

        public override void ActivateOutput()
        {
            // Activating output mean activating the input of the other end of the wire
            for (int i = 0; i < wireOutput.Length; i++)
            {
                // Control output by checking input
                if (wireOutput[i] != null)
                {
                    if (GetSetIsEnergized &&
                        wireInput[0] != null &&
                        wireInput[0].GetComponent<WireControl>().wireConnectorOutput.IsOn() &&
                        wireInput[1] != null &&
                        wireInput[1].GetComponent<WireControl>().wireConnectorOutput.IsOn())
                    {
                        wireOutput[i].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(true);
                    }
                    else
                    {
                        wireOutput[i].GetComponent<WireControl>().wireConnectorInput.EnergizeByWire(false);
                    }
                }
            }
        }
    }
}