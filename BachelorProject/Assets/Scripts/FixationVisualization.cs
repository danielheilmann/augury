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
    [SerializeField, ReadOnly] private LineRenderer line;  //!< Line positions are in world space.
    // private Color lineColor;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        textField = GetComponentInChildren<TextMeshProUGUI>();
        line = GetComponentInChildren<LineRenderer>();

        line.positionCount = 2;
    }

    private void OnDisable() => RequestNextFixationLineUpdate(); //< To remove the line to the next fixation when this one is disabled.
    private void OnEnable() => RequestNextFixationLineUpdate(); //< To re-enable the line to the next fixation when this one is enabled again.

    private void FixedUpdate() //TODO: Rework this to only run whenever the position of the gameobject has changed, not always on Update. (event-based whenever one of the fixations is moved.)
    {
        if (fixation.dynamicObject == null) return; //< No need to check for position updates if this object is not intended to move anyways

        //TODO: Maybe I should rework the line system completely to be handled by the FixationVisualizer itself. This way, there would only be one Line Renderer for all the (global), which would probably be more efficient. This way, FixationVisualizations could always just send a notification to the Visualizer that they've moved and the Visualizer would handle the rest.
        //** The current implementation should allow for better flexibility for local gazeplots though, as it does not require all Fixations to be connected with each other (with lines). Local gazeplots require multiple line renderers anyways.

        //> To update lines between the 3 points (previous, this & next) whenever this fixation point is moved:

        //> Connection from this fixation to previous fixation
        UpdateLineToPrecedingFixation();

        //> Connection from next fixation to this fixation}
        RequestNextFixationLineUpdate();
    }

    public FixationVisualization Configure(Fixation fixation, int listIndex)
    {
        if (listIndex != fixation.gfid)
            Debug.LogError($"ListIndex ({listIndex}) and GFID ({fixation.gfid}) do not match. Something must have gone wrong.");

        this.fixation = fixation;

        //> Take apart Fixation for better readability down below
        Vector3 position = fixation.globalPosition;
        Vector3 surfaceNormal = fixation.surfaceNormal;
        DynamicObject dynObj = fixation.dynamicObject;

        //> Set up the GameObject
        gameObject.name = $"Fixation {fid + 1} {fixation.globalPosition}";
        gameObject.transform.position = position + (surfaceNormal * offsetFactor);  //< surfaceNormal is added here to prevent Z-Fighting
        gameObject.transform.SetParent(fixation.isLocal ? dynObj.transform : FixationVisualizer.Instance.transform, true);

        //> Set up the canvas and line
        canvas.transform.LookAt(position - surfaceNormal);
        textField.text = (fid + 1).ToString(); //< To display the numbers starting from 1 instead of 0.
        line.SetPosition(0, this.transform.position); //< Set the line to start at the current position.
        UpdateLineToPrecedingFixation();

        return this;
    }

    private void UpdateLineToPrecedingFixation()
    {
        if (fid <= 0)  //< Retract line if this is the first fixation or if the fixation is not set.
        {
            RetractLine();
            return;
        }

        FixationVisualization precedingFixation = FixationVisualizer.Instance.GetFixationVisualization(fid - 1);

        //> If the preceding fixation is not active, do not draw a line to it. 
        //  The null check should not be necessary, as the only FixationVisualization with a null predecessor should be the first one, which is already covered by the guard clause above. But just in case a FixationVisualization gets obliterated, this should prevent any errors.
        if (precedingFixation == null || !precedingFixation.isActiveAndEnabled)
        {
            RetractLine();
            return;
        }

        SetLineDestination(precedingFixation.transform.position);
    }

    private void RequestNextFixationLineUpdate()
    {
        if (fixation == null) return; //< If this object is not configured yet, skip this step.

        FixationVisualization nextFixation = FixationVisualizer.Instance.GetFixationVisualization(fid + 1);
        if (nextFixation != null && nextFixation.isActiveAndEnabled)
            nextFixation.UpdateLineToPrecedingFixation(); //< To propagate the changes to the next one as well (but only if there is another visualization after the current one)
    }

    /// <summary> Configures the line renderer so it points towards the given destination. </summary>
    /// <param name="destination"> The global position vector of the target location. </param>
    private void SetLineDestination(Vector3 destination) => line.SetPosition(1, destination); //< The line positions use world space coordinates.
    private void RetractLine() => line.SetPosition(1, this.transform.position); //< To retract the line to the current position.
}
