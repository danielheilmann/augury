using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fixation : MonoBehaviour
{
    public int number { get; private set; }
    private Fixation precedingFixation;

    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private LineRenderer line;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Awake()
    {
        // textField = GetComponentInChildren<TextMeshProUGUI>(); //< Opted for manual assignment instead.
        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);  //< This way, both pos 0 and 1 are Vector3.zero, resulting in no line being drawn.
    }

    public void SetID(int number)
    {
        this.number = number;
        textField.text = number.ToString();
    }

    public void ConnectToPrecedingFixation(Fixation other)
    {
        precedingFixation = other;
        if (number > 1)
        {
            Vector3 vectorToPrecedingFixation = precedingFixation.transform.position - this.transform.position;
            line.SetPosition(1, vectorToPrecedingFixation);
        }
    }
}
