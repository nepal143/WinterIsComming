
//(8

using UnityEngine;

namespace ElectricWire
{
    public class ElectricMoreInOne : ElectricComponent
    {
        public ElectricComponent[] electricComponents;

        public override void ReconnectAllConnector()
        {
            for (int i = 0; i < electricComponents.Length; i++)
            {
                for (int ii = 0; ii < electricComponents[i].wireConnectorInput.Length; ii++)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(electricComponents[i].wireConnectorInput[ii].transform.position, 0.01f);
                    for (int iii = 0; iii < hitColliders.Length; iii++)
                    {
                        WireControl wireControlTemp = hitColliders[iii].GetComponentInParent<WireControl>();
                        if (wireControlTemp != null)
                        {
                            // TODO : Find why found 4 seg0
                            //print("Input: " + iii + " : " + hitColliders[i].name);
                            wireControlTemp.ReSetupWire();
                            break;
                        }
                    }
                }
                for (int ii = 0; ii < electricComponents[i].wireConnectorOutput.Length; ii++)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(electricComponents[i].wireConnectorOutput[ii].transform.position, 0.01f);
                    for (int iii = 0; iii < hitColliders.Length; iii++)
                    {
                        WireControl wireControlTemp = hitColliders[iii].GetComponentInParent<WireControl>();
                        if (wireControlTemp != null)
                        {
                            // TODO : Find why found 4 seg0
                            //print("Output: " + iii + " : " + hitColliders[i].name);
                            wireControlTemp.ReSetupWire();
                            break;
                        }
                    }
                }
            }
        }

        public override void StructureDestroyed()
        {
            for (int i = 0; i < electricComponents.Length; i++)
            {
                for (int ii = 0; ii < electricComponents[i].wireInput.Length; ii++)
                {
                    if (electricComponents[i].wireInput[ii] != null)
                        electricComponents[i].wireInput[ii].GetComponent<WireControl>().wireConnectorOutput.DisconnectWire();
                }
                for (int ii = 0; ii < electricComponents[i].wireOutput.Length; ii++)
                {
                    if (electricComponents[i].wireOutput[ii] != null)
                        electricComponents[i].wireOutput[ii].GetComponent<WireControl>().wireConnectorInput.DisconnectWire();
                }
            }

            Destroy(gameObject);
        }

        public override void StructureDisabled()
        {
            for (int i = 0; i < electricComponents.Length; i++)
            {
                for (int ii = 0; ii < electricComponents[i].wireInput.Length; ii++)
                {
                    if (electricComponents[i].wireInput[ii] != null)
                        electricComponents[i].wireInput[ii].GetComponent<WireControl>().wireConnectorOutput.DisconnectWire();
                }
                for (int ii = 0; ii < electricComponents[i].wireOutput.Length; ii++)
                {
                    if (electricComponents[i].wireOutput[ii] != null)
                        electricComponents[i].wireOutput[ii].GetComponent<WireControl>().wireConnectorInput.DisconnectWire();
                }
            }

            gameObject.SetActive(false);
        }
    }
}
