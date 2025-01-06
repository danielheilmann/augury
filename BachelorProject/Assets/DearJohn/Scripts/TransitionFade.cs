using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransitionFade : MonoBehaviour
{
    [SerializeField] private UnityEvent OnObscured = new();
    [SerializeField] private Material material;
    [SerializeField] private AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1, 0));
    [SerializeField] private Color color;
    [SerializeField] private float duration = 1;
    private float _alpha = 0;
    private bool _done;
    private float _time;
    private bool hasTriggeredObscuredAlready = false;

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        hasTriggeredObscuredAlready = false;
        _done = false;
        _alpha = 0;
        _time = 0;
    }

    public void Update()
    {
        if (_done) return;

        material.color = new Color(color.r, color.g, color.b, _alpha);

        _time += Time.deltaTime;

        if (duration == 0) return; //< Prevent division by zero
        else _alpha = FadeCurve.Evaluate(_time / duration);

        _alpha = FadeCurve.Evaluate(_time / duration);
        if (_alpha >= 1 && !hasTriggeredObscuredAlready) OnObscured.Invoke();

        if (_alpha < 0) 
        {
            _done = true;
            this.gameObject.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        material.color = new Color(color.r, color.g, color.b, 0);
    }
}
