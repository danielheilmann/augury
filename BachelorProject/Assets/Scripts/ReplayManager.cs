using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

//TODO | Feature List:
// 1. Think about how to handle scene changes to accurately apply the correct data from each session

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance { get; private set; }
    public static UnityEvent OnReplayBegin = new();

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

    private void OnEnable()
    {
        SessionManager.OnReplayStart.AddListener(Initialize);
    }

    private void OnDisable()
    {
        SessionManager.OnReplayStart.RemoveListener(Initialize);
    }

    void Initialize()
    {
        validSessions = new List<SessionFileReference>();
        selectedSession = null;
        timeline = null;
        FetchValidSessions();
    }

    private void FetchValidSessions()
    {
        var allSessions = FileSystemHandler.FetchAllSessions();

        foreach (var session in allSessions)
        {
            if (session.appVersion != Application.version)
                continue;
            else if (session.sceneName != SceneManager.GetActiveScene().name)
                continue;

            validSessions.Add(session);
        }
        Debug.Log($"Valid Sessions:\n{validSessions.ToSeparatedString("\n")}");

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

        GazePointManager.Instance.LoadGazePoints(selectedSession.GetGazePoints());
    }

    [ContextMenu("Begin Replay")]
    public void BeginReplay()
    {
        //TODO: ONLY FOR DEBUG, remove this later!
        SelectSession(validSessions.Count - 1);
        LoadSelectedSession();
        //TODO: ^^^^

        Debug.Log($"Beginning Replay of Session {selectedSession}.");
        // OnReplayBegin?.Invoke();
    }
}
