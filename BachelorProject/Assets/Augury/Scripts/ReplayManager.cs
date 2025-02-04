using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance { get; private set; }
    public UnityEvent OnReplayBegin = new();
    public UnityEvent OnReplayPause = new();
    public UnityEvent OnReplayUnpause = new();
    public UnityEvent OnReplayFinished = new();

    public bool isActivelyReplaying { get; private set; } = false;
    public bool isPaused { get; private set; } = false;

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
        SessionManager.OnReplayStop.AddListener(Clear);
    }

    private void OnDestroy()
    {
        SessionManager.OnReplayStart.RemoveListener(Initialize);
        SessionManager.OnReplayStop.RemoveListener(Clear);
    }

    private void Clear()
    {
        isActivelyReplaying = false;
        isPaused = false;

        selectedSession = null;
        if (timeline != null)
            Destroy(timeline);

        validSessions = null;
    }

    private void Initialize()
    {
        Clear();
        validSessions = FetchValidSessions();
    }

    private List<SessionFileReference> FetchValidSessions()
    {
        List<SessionFileReference> onlyValidSessions = new();

        var allSessions = FileSystemHandler.FetchAllSessions();
        foreach (var session in allSessions)
        {
            if (session.appVersion != Application.version) continue;
            if (session.sceneName != SceneManager.GetActiveScene().name) continue;

            onlyValidSessions.Add(session);
        }

        if (onlyValidSessions.Count == 0)
            Debug.Log("There are no valid sessions in the data directory.");
        else
            Debug.Log($"Fetched the following valid sessions:\n{onlyValidSessions.ToSeparatedString("\n")}");

        return onlyValidSessions;
    }

    public void SelectSession(int index)
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

    public void LoadSelectedSession()
    {
        if (selectedSession == null)
        {
            Debug.LogWarning("No session selected. Please select a session and try again.");
            return;
        }

        timeline = gameObject.AddComponent<ReplayTimeline>();

        timeline.GenerateActionsFromGazePointsJSON(selectedSession.gazePointsJSON);

        foreach (var dynObjJSON in selectedSession.dynamicObjectJSONs)
            timeline.GenerateActionsFromDynamicObjectJSON(dynObjJSON);

        timeline.Initialize(selectedSession.sessionStartTime);
        timeline.OnTimelineFinished.AddListener(ReplayFinished);

        validSessions.Clear(); //< Clear the list of valid sessions once a session has been loaded.

        Debug.Log($"Loaded Session: {selectedSession}");
    }

    [ContextMenu("Begin Replay")]
    public void BeginReplay()
    {
        if (timeline == null)
        {
            Debug.LogWarning($"No session loaded. {(selectedSession == null ? "Please select a session" : "Please load the currently selected session")} and try again.");
            return;
        }

        Debug.Log($"Begun replaying Session: {selectedSession}");
        isActivelyReplaying = true;

        timeline.Play();
        OnReplayBegin.Invoke();
    }

    private void ReplayFinished()
    {
        Debug.Log($"Finished replaying Session: {selectedSession}");
        timeline.OnTimelineFinished.RemoveListener(ReplayFinished);
        Clear();  //< Delete Timeline and reset all values once the replay has finished.
        OnReplayFinished.Invoke();
    }

    [ContextMenu("Pause Replay")]
    public void PauseReplay()
    {
        Debug.Log("Pausing Replay.");
        isPaused = true;

        timeline.Pause();
        OnReplayPause.Invoke();
    }

    [ContextMenu("Unpause Replay")]
    public void UnpauseReplay()
    {
        Debug.Log("Unpausing Replay.");
        isPaused = false;

        timeline.Play();
        OnReplayUnpause.Invoke();
    }
}
