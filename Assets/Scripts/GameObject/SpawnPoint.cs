using CreatorKitCodeInternal;
using UnityEngine;

namespace CreatorKitCode
{
    public class SpawnPoint : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            //no need to check as only the player can trigger object in that layer, if it create a null ref it SHOULD
            //error as that mean this other object shouldn't be in that layer
            CharacterControl chr = other.GetComponent<CharacterControl>();
            chr.SetNewRespawn(this);

        }

        public void Activated()
        {
            enabled = false;
        }

        public void Deactivated()
        {
            enabled = true;
        }
    }
}
