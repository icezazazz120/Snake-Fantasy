using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PartyManager : MonoBehaviour
{
    public Queue<Vector3> positionHistory = new Queue<Vector3>();
    public class Party
    {
        public Vector3 position;
        public Quaternion rotation;

        public Party(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    public List<Party> partyList = new List<Party>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePartyList();
    }

    public void UpdatePartyList()
    {
        partyList.Add(new Party(transform.position, transform.rotation));
    }

    public void ClearPartyList()
    {
        partyList.Clear();
        partyList.Add(new Party(transform.position, transform.rotation));
    }
    public void RecordPosition(Vector3 pos)
    {
        positionHistory.Enqueue(pos);
        if (positionHistory.Count > 50)
        {
            positionHistory.Dequeue(); // keep size reasonable
        }
    }
    public Vector3? GetDelayedPosition(int delay)
    {
        if (positionHistory.Count >= delay)
        {
            Vector3[] arr = positionHistory.ToArray();
            return arr[delay - 1];
        }
        return null;
    }
}
