using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnsnarePikmin : MonoBehaviour
{
    private Removable removable;

    private void Awake() => removable = GetComponent<Removable>();

    IEnumerator StartEnsnare()
    {
        float time = 0;

        while (time < 1f)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 3.5f);

            foreach (Collider col in cols)
            {
                if (!col.CompareTag("Pikmin")) continue;

                Pikmin _pikmin = col.GetComponent<Pikmin>();
                if (_pikmin.state != PikminState.STAY) continue;
                if (_pikmin.PikminTarget != null) continue;
                if (_pikmin.transform.parent != null) continue;

                removable.Expansion();

                _pikmin.removable = this.removable;
                _pikmin.WorkPikmin();
            }

            time += Time.deltaTime;
            yield return null;
        }

        this.enabled = false;
    }

    public void Ensnare() => StartCoroutine(StartEnsnare());

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 3.5f);
    }
}