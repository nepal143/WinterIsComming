
//(c8

using UnityEngine;

namespace ElectricWire
{
    public class TurretTarget : MonoBehaviour
    {
        public bool invulnerable = false;
        public int health = 100;
        public Transform targetPoint;

        public void AttackByTurret()
        {
            if (invulnerable)
                return;

            health -= 10;

            if (health <= 0)
            {
                ElectricComponent electricComponent = GetComponentInParent<ElectricComponent>();

                // Undo/Redo listing
                if (ElectricManager.electricManager.electricUndoRedo != null)
                {
                    if (!electricComponent.skipUndoRedo)
                    {
                        ElectricManager.electricManager.electricUndoRedo.AddToUndoRedo(false, gameObject, electricComponent.instantiatedFrom, transform.position, transform.rotation);
                        electricComponent.StructureDisabled();
                    }
                    else
                        electricComponent.StructureDestroyed();
                }
                else
                    electricComponent.StructureDestroyed();
            }
        }
    }
}
