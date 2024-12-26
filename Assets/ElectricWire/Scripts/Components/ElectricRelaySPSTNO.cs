
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricRelaySPSTNO : ElectricComponent
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
            // Run function EnergizedByWire() to check if one of the input are energized
            if (isInput)
            {
                EnergizeByWire(false, index);
                ActivateOutput();
            }
        }

        public override float IsDrainEnergy(int index)
        {
            // If request is from input 0, return drain, otherwise return 0f
            if (index == 0)
                return base.IsDrainEnergy();
            else
                return 0f;
        }

        public override void EnergizeByWire(bool onOff, int index)
        {
            // The first input is use to energize other component
            // The second input is use to toggle on the relay
            if (index == 0)
            {
                GetSetIsEnergized = onOff;
                ActivateOutput();
            }
            else if (index == 1)
            {
                GetSetIsOn = onOff;
                ActivateOutput();
            }
        }

        #endregion
    }
}
