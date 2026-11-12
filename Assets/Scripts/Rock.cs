using UnityEngine;

public class Rock : MonoBehaviour
{
    public float SoundRadius = 100f;
    private bool hasTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasTriggered) return;
        if (collision.gameObject.CompareTag("Player")) return;
        hasTriggered = true;

        Vector3 impactPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;

        Blackboard[] allGuards=Object.FindObjectsByType<Blackboard>(FindObjectsSortMode.None);
        foreach(Blackboard guard in allGuards)
        {
            if (guard == null)
            continue;
            float distance = Vector3.Distance(guard.transform.position, impactPoint);
            if(distance<=SoundRadius)
            {
                guard.TriggerDistraction(impactPoint);
            }
        }
    }
}
