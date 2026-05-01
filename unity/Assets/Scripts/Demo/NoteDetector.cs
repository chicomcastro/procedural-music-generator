using UnityEngine;

namespace PMM.Demo
{
    public class NoteDetector : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            NoteId nid = other.gameObject.GetComponent<NoteId>();

            if (nid == null)
                return;

            if (!nid.info.isActive)
                return;

            nid.Play();
        }

        void OnTriggerExit2D(Collider2D other)
        {
            NoteId nid = other.gameObject.GetComponent<NoteId>();
            if (nid == null)
                return;

            if (nid.info.isActive)
            {
                nid.TurnOff();
            }
        }
    }
}
