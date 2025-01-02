using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO | Feature List:
// 1. Think about how to handle scene changes to accurately apply the correct data from each session

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance { get; private set; }
    public List<SessionFileReference> validSessions { get; private set; }
    public SessionFileReference selectedSession { get; private set; } = null;
    public ReplayTimeline timeline { get; private set; }

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

    private void OnEnable() => SessionManager.OnReplayStart.AddListener(Initialize);

    private void OnDisable() => SessionManager.OnReplayStart.RemoveListener(Initialize);

    void Initialize()
    {
        validSessions = new List<SessionFileReference>();
        selectedSession = null;
        timeline = null;
        FetchValidSessions();
    }

    private void FetchValidSessions()
    {
        //TODO: Make sure to catch null exceptions here in case there are no recorded sessions yet.
        var allSessions = FileSystemHandler.FetchAllSessions();

        foreach (var session in allSessions)
        {
            if (session.appVersion != Application.version)
                continue;
            else if (session.sceneName != SceneManager.GetActiveScene().name)
                continue;

            validSessions.Add(session);
            Debug.Log($"{session}");
        }
    }

    private void SelectSession(int index)
    {
        if (validSessions.Count == 0)
        {
            Debug.LogWarning("No valid sessions available.");
            return;
        }
        if (index < 0 || index >= validSessions.Count)
        {
            Debug.LogError("Invalid index.");
            return;
        }

        selectedSession = validSessions[index];
        Debug.Log($"Selected Session: {selectedSession}");
    }

    private void LoadSelectedSession()
    {
        if (selectedSession == null)
        {
            Debug.LogWarning("No session selected.");
            return;
        }

        timeline = new ReplayTimeline();
    }
}
