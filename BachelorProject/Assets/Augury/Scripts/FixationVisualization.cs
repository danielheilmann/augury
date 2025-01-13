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

    private void OnEnable() => RequestNextFixationLineUpdate(); //< To re-enable the line to the next fixation when this one is enabled again.
    private void OnDisable() => RequestNextFixationLineUpdate(); //< To remove the line to the next fixation when this one is disabled.

    private void OnDestroy()
    {
        if (fixation != null && fixation.isLocal) //< The fixation reference will still be null for all preloaded FixationVisualizations that were never configured / used.
        {
            fixation.dynamicObject.OnPositionUpdate.RemoveListener(OnDynamicObjectPositionUpdate);
            fixation.dynamicObject.OnRotationUpdate.RemoveListener(OnDynamicObjectRotationUpdate);
            fixation.dynamicObject.OnScaleUpdate.RemoveListener(OnDynamicObjectScaleUpdate);
        }
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
        canvas.transform.rotation = Quaternion.LookRotation(-surfaceNormal);
        textField.text = (fid + 1).ToString(); //< To display the numbers starting from 1 instead of 0.

        UpdateLine();

        if (fixation.isLocal)
        {
            fixation.dynamicObject.OnPositionUpdate.AddListener(OnDynamicObjectPositionUpdate);
            fixation.dynamicObject.OnRotationUpdate.AddListener(OnDynamicObjectRotationUpdate);
            fixation.dynamicObject.OnScaleUpdate.AddListener(OnDynamicObjectScaleUpdate);
        }

        return this;
    }

    #region DynamicObject Update Event Handlers
    private void OnDynamicObjectPositionUpdate() => UpdateLine();
    private void OnDynamicObjectRotationUpdate() => UpdateLine();
    private void OnDynamicObjectScaleUpdate()
    {
        UpdateLine(); //< Because changing the scale of the object will likely change the position of the fixation point attached to its surface.
        //TODO: Implement counter-scaling of canvas here to keep the visualization bubble the same size, even when the DynamicObject is scaled. 
    }
    #endregion

    #region Line Update Methods
    private void UpdateLine()
    {
        //> To update lines between the 3 points (previous, this & next) whenever this fixation point is moved:
        UpdateLineOrigin();
        UpdateLineToPrecedingFixation();
        RequestNextFixationLineUpdate();
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
        if (this.fixation == null) return; //< If this object is not configured yet, skip this step. (This is necessary because this method is called by OnEnable and OnDisable as well.)

        FixationVisualization nextFixation = FixationVisualizer.Instance.GetFixationVisualization(fid + 1);
        if (nextFixation != null && nextFixation.isActiveAndEnabled)
            nextFixation.UpdateLineToPrecedingFixation(); //< To propagate the changes to the next one as well (but only if there is another visualization after the current one)
    }
    #endregion

    #region Line Modification Methods
    /// <summary> 
    /// Configures the line renderer so it points towards the given destination. 
    /// </summary>
    /// <param name="destination"> The global position vector of the target location. </param>
    private void SetLineDestination(Vector3 destination) => line.SetPosition(1, destination);

    /// <summary>
    /// Updates line origin to equal current transform position. 
    /// </summary>
    private void UpdateLineOrigin() => line.SetPosition(0, this.transform.position);

    /// <summary> 
    /// Retracts the line by setting current transform position as the destination. 
    /// </summary>
    private void RetractLine() => SetLineDestination(this.transform.position);
    #endregion
}
