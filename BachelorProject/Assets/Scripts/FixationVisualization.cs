using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A class that serves as an interface to all components on the FixationVisualization GameObject Prefab.
/// </summary>
public class FixationVisualization : MonoBehaviour
{
    private const float offsetFactor = 0.01f;  //< During placement, the visualization canvas is lifted from the surface by an amount equal to surfaceNormal * offsetFactor.
    public Fixation fixation { get; private set; } = null;
    public int fid => fixation != null ? fixation.gfid : -1;
    [SerializeField, ReadOnly] private Canvas canvas;
    [SerializeField, ReadOnly] private TextMeshProUGUI textField;
    [SerializeField, ReadOnly] private LineRenderer line;
    // private Color lineColor;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        textField = GetComponentInChildren<TextMeshProUGUI>();
        line = GetComponentInChildren<LineRenderer>();

        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        SetLineDestination(Vector3.zero); //< This way, if both pos 0 and 1 are Vector3.zero (the origin of this gameobject), resulting in no line being drawn.
    }

    private void FixedUpdate() //TODO: Rework this to only run whenever the position of the gameobject has changed, not always on Update. (event-based whenever one of the fixations is moved.)
    {
        if (fixation.dynamicObject == null) return; //< No need to check for position updates if this object is not intended to move anyways

        //TODO: Maybe I should rework the line system completely to be handled by the FixationVisualizer itself. This way, there would only be one Line Renderer for all the (global), which would probably be more efficient. This way, FixationVisualizations could always just send a notification to the Visualizer that they've moved and the Visualizer would handle the rest.
        //** The current implementation should allow for better flexibility for local gazeplots though, as it does not require all Fixations to be connected with each other (with lines). Local gazeplots require multiple line renderers anyways.

        //> To update lines between the 3 points (previous, this & next) whenever this fixation point is moved:

        //> Connection from this fixation to previous fixation
        UpdateLineToPrecedingFixation();

        //> Connection from next fixation to this fixation
        FixationVisualization nextFixation = FixationVisualizer.Instance.GetFixationVisualization(fixation.gfid + 1);
        if (nextFixation.isActiveAndEnabled)
            nextFixation.UpdateLineToPrecedingFixation(); //< To propagate the changes to the next one as well (but only if there is another visualization after the current one)
    }

    public FixationVisualization Configure(Fixation fixation, int listIndex)
    {
        if (listIndex != fixation.gfid)
            Debug.LogError($"ListIndex and GFID do not match. Something must have gone wrong.");

        this.fixation = fixation;

        //> Take apart Fixation for better readability down below
        Vector3 position = fixation.rawPosition;
        Vector3 surfaceNormal = fixation.surfaceNormal;
        DynamicObject dynObj = fixation.dynamicObject;

        gameObject.name = $"Fixation {fid + 1} {fixation.rawPosition}";
        gameObject.transform.position = position + (surfaceNormal * offsetFactor);  //< surfaceNormal is added here to prevent Z-Fighting
        gameObject.transform.SetParent(fixation.isLocal ? dynObj.transform : FixationVisualizer.Instance.transform, true);
        canvas.transform.LookAt(position - surfaceNormal);
        textField.text = (fid + 1).ToString();
        UpdateLineToPrecedingFixation();

        return this;
    }

    private void UpdateLineToPrecedingFixation()
    {
        if (fid <= 0)  //< Reset line to zero if this is the first fixation or if the fixation is not set.
        {
            SetLineDestination(Vector3.zero);
            return;
        }
        FixationVisualization precedingFixation = FixationVisualizer.Instance.GetFixationVisualization(fid - 1);
        Vector3 vectorToPrecedingFixation = precedingFixation.transform.position - this.transform.position;
        SetLineDestination(vectorToPrecedingFixation);
    }

    private void SetLineDestination(Vector3 destination) => line.SetPosition(1, destination);
}
