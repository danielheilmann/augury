using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MachinePartAcceptor : MonoBehaviour
{
    [SerializeField] private GameObject previewCardboardBox;
    private bool isCardboardBoxPlaced = false;
    [SerializeField] private GameObject previewBrokenMicrowave;
    private bool isBrokenMicrowavePlaced = false;
    [SerializeField] private GameObject previewHamsterWheel;
    private bool isHamsterWheelPlaced = false;
    [SerializeField] private GameObject previewBattery;
    private bool isBatteryPlaced = false;
    [SerializeField] private GameObject previewAntennaTV;
    private bool isAntennaTVPlaced = false;
    [SerializeField] private GameObject previewLever;
    //< no need for a isLeverPlaced bool, as the game ends when the lever is placed.
    private bool isMachineComplete = false;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        previewCardboardBox.SetActive(true);
        previewBrokenMicrowave.SetActive(false);
        previewHamsterWheel.SetActive(false);
        previewBattery.SetActive(false);
        previewAntennaTV.SetActive(false);
        previewLever.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMachineComplete) return;

        QuestItem questItem = other.transform.parent?.GetComponent<QuestItem>();
        if (questItem == null) return;

        // Debug.Log($"QuestItem {questItem.type} entered the MachinePartAcceptor.");

        GameObject selectedPreview;

        switch (questItem.type)
        {
            case QuestItem.Type.CardboardBox:
                if (!previewCardboardBox.activeInHierarchy) break;
                selectedPreview = previewCardboardBox;
                isCardboardBoxPlaced = true;
                StartCoroutine(LerpQuestItemToPreview(questItem, selectedPreview, previewBrokenMicrowave, previewHamsterWheel, previewAntennaTV));
                // Debug.Log("Cardboard Box accepted!");
                break;
            case QuestItem.Type.FixedMicrowave:
                Debug.Log($"Microwave is not broken yet.");
                break;
            case QuestItem.Type.BrokenMicrowave:
                if (!previewBrokenMicrowave.activeInHierarchy) break;
                selectedPreview = previewBrokenMicrowave;
                isBrokenMicrowavePlaced = true;
                StartCoroutine(LerpQuestItemToPreview(questItem, selectedPreview, previewBattery));
                // Debug.Log("Broken Microwave accepted!");
                break;
            case QuestItem.Type.HamsterWheel:
                if (!previewHamsterWheel.activeInHierarchy) break;
                selectedPreview = previewHamsterWheel;
                isHamsterWheelPlaced = true;
                StartCoroutine(LerpQuestItemToPreview(questItem, selectedPreview));
                // Debug.Log("Hamster Wheel accepted!");
                break;
            case QuestItem.Type.Battery:
                if (!previewBattery.activeInHierarchy) break;
                selectedPreview = previewBattery;
                isBatteryPlaced = true;
                StartCoroutine(LerpQuestItemToPreview(questItem, selectedPreview));
                // Debug.Log("Battery accepted!");
                break;
            case QuestItem.Type.AntennaTV:
                if (!previewAntennaTV.activeInHierarchy) break;
                selectedPreview = previewAntennaTV;
                isAntennaTVPlaced = true;
                StartCoroutine(LerpQuestItemToPreview(questItem, selectedPreview));
                // Debug.Log("Antenna TV accepted!");
                break;
            case QuestItem.Type.Lever:
                if (!previewLever.activeInHierarchy || isMachineComplete) break; //< Second argument was added to prevent a second coroutine from being started for some reason.
                isMachineComplete = true;
                selectedPreview = previewLever;
                StartCoroutine(LerpQuestItemToPreview(questItem, selectedPreview));
                // Debug.Log("Lever accepted!");
                break;
        }
    }

    private IEnumerator LerpQuestItemToPreview(QuestItem questItem, GameObject selectedPreview, params GameObject[] previewsToEnableAfterLerp)
    {
        questItem.Strip(); //< To prevent the quest item from being grabbed while lerping.

        float lerpDuration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = questItem.transform.position;
        Quaternion startRotation = questItem.transform.rotation;
        Vector3 startScale = questItem.transform.localScale;

        while (elapsedTime < lerpDuration)
        {
            questItem.transform.position = Vector3.Lerp(startPosition, selectedPreview.transform.position, elapsedTime / lerpDuration);
            questItem.transform.rotation = Quaternion.Lerp(startRotation, selectedPreview.transform.rotation, elapsedTime / lerpDuration);
            questItem.transform.localScale = Vector3.Lerp(startScale, selectedPreview.transform.localScale, elapsedTime / lerpDuration);

            elapsedTime += Time.deltaTime;
            yield return null; //< Resume with loop in the next frame.
        }

        selectedPreview.SetActive(false);

        //> Ensure the quest item is placed exactly where the preview is, even if the final step of the lerp was skipped due to the loop condition.
        questItem.transform.position = selectedPreview.transform.position;
        questItem.transform.rotation = selectedPreview.transform.rotation;
        questItem.transform.localScale = selectedPreview.transform.localScale;

        if (isMachineComplete)
        {
            yield return new WaitForSeconds(2f);
            questItem.GetComponent<Rigidbody>().isKinematic = false;  //< Lever falls to the ground, comically.
            yield return new WaitForSeconds(2f);
            GameManager.Instance.RollCredits();
            yield break;
        }

        if (previewsToEnableAfterLerp != null)
            foreach (GameObject preview in previewsToEnableAfterLerp)
            {
                preview.SetActive(true);
            }

        if (isCardboardBoxPlaced && isBrokenMicrowavePlaced && isHamsterWheelPlaced && isBatteryPlaced && isAntennaTVPlaced) //< Once everything else was placed, allow the lever to be placed.
            previewLever.SetActive(true);
    }

    private void OnMachineCompleted()
    {

    }
}
