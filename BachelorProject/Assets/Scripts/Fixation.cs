using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fixation : MonoBehaviour
{
    public int number { get; private set; }
    public Vector3 surfaceNormal { get; private set; }
    public Vector3 position => transform.position;
    [SerializeField] private Fixation precedingFixation;
    [SerializeField] private DynamicObject connectedDynamicObject;

    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private LineRenderer line;

    private void Awake()
    {
        // textField = GetComponentInChildren<TextMeshProUGUI>(); //< Opted for manual assignment instead.
        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);  //< This way, both pos 0 and 1 are Vector3.zero, resulting in no line being drawn.
    }

    private void LateUpdate()
    {
        if (connectedDynamicObject != null && precedingFixation != null)
        {
            //> To update line between the points whenever this fixation point is moved 
            //TODO: To update this properly on both sides, the entire fixation architecture needs to be reworked so that a manager can handle these updates.
            Vector3 vectorToPrecedingFixation = precedingFixation.transform.position - this.transform.position;
            line.SetPosition(1, vectorToPrecedingFixation);
        }
    }

    public Fixation Configure(int id, DynamicObject dynamicObject, Fixation precedingFixation) //< Allows for configuration without having to call 3 separate methods.
    {
        SetID(id);
        SetDynamicObject(dynamicObject);
        ConnectToPrecedingFixation(precedingFixation);
        return this;
    }

    public void SetID(int number)
    {
        this.number = number;
        textField.text = number.ToString();
    }

    public void SetDynamicObject(DynamicObject dynamicObject)
    {
        this.connectedDynamicObject = dynamicObject;
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
