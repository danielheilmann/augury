using UnityEngine;

public class CameraFade : MonoBehaviour
{
    [SerializeField] private AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1, 0));
    [SerializeField] private Color color;
    [SerializeField] private float duration = 1;
    private float _alpha = 1;
    private Texture2D _texture;
    private bool _done;
    private float _time;

    public void Reset()
    {
        _done = false;
        _alpha = 1;
        _time = 0;
    }

    private void OnEnable()
    {
        RedoFade();
    }

#pragma warning disable UNT0015
    // [RuntimeInitializeOnLoadMethod]
    public void RedoFade() => Reset();
#pragma warning restore UNT0015

    public void OnGUI()
    {
        if (_done) return;
        if (_texture == null) _texture = new Texture2D(1, 1);

        _texture.SetPixel(0, 0, new Color(color.r, color.g, color.b, _alpha));
        _texture.Apply();

        _time += Time.deltaTime;
        if (duration == 0) return;
        else _alpha = FadeCurve.Evaluate(_time / duration);
        _alpha = FadeCurve.Evaluate(_time / duration);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);

        if (_alpha <= 0) _done = true;
    }
}