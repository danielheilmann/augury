using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError($"{this} cannot set itself as instance as one has already been set by {Instance.gameObject}. Deleting self.");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FetchUniqueSessionIDs();
    }

    private void FetchUniqueSessionIDs()
    {
        //TODO: Make sure to catch null exceptions here in case there are no recorded sessions yet.
        var sessions = FileSystemHandler.FetchSessions();
        List<string> sessionIDs = new();
        foreach (var session in sessions)
        {
            if (!sessionIDs.Contains(session.Key))
                sessionIDs.Add(session.Key);
        }
        Debug.Log($"{sessionIDs.ToCommaSeparatedString()}");

        LoadSession(sessionIDs[0]);
    }

    private void LoadSession(string sessionID)
    {
        var sessionData = FileSystemHandler.GetSessionContents(sessionID);
        foreach (var item in sessionData)
        {
            Debug.Log($"{item.Key}: {item.Value}");
        }
    }
}
