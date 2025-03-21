using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MobileActionKit
{
    public class AnimatorLayerWeightController : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is responsible for controlling the speed of changing the animator layers weight to achieve the smooth layer transitioning effect from one layer to another.";

        private class LayerWeightChangeData
        {
            public int layerIndex;
            public float targetWeight;
            public bool fillToZero;
        }

        private Animator anim;
        private bool isWeightChanging = false;
        private List<LayerWeightChangeData> changeDataList = new List<LayerWeightChangeData>();

        [Tooltip("The duration (in seconds) used to interpolate between current and target animator layer weights.")]
        public float AnimatorWeightInterpolationDuration = 0.2f;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void ChangeLayerWeight(int layerIndex, float targetWeight, bool fillToZero = false)
        {
            LayerWeightChangeData changeData = new LayerWeightChangeData
            {
                layerIndex = layerIndex,
                targetWeight = targetWeight,
                fillToZero = fillToZero
            };

            changeDataList.Add(changeData);

            if (!isWeightChanging)
            {
                StartCoroutine(ChangeLayerWeightCoroutine());
            }
        }

        private IEnumerator ChangeLayerWeightCoroutine()
        {
            isWeightChanging = true;

            while (changeDataList.Count > 0)
            {
                LayerWeightChangeData currentChangeData = changeDataList[0];
                int layerIndex = currentChangeData.layerIndex;
                float targetWeight = currentChangeData.targetWeight;
                bool fillToZero = currentChangeData.fillToZero;

                float currentWeight = anim.GetLayerWeight(layerIndex);

                if (Mathf.Approximately(currentWeight, targetWeight))
                {
                    changeDataList.RemoveAt(0);
                    continue;
                }

                float duration = Mathf.Abs(targetWeight - currentWeight) * AnimatorWeightInterpolationDuration;

                if (fillToZero)
                {
                    LeanTween.value(gameObject, (weight) => SetLayerWeight(layerIndex, weight), currentWeight, 0f, duration);
                }
                else
                {
                    LeanTween.value(gameObject, (weight) => SetLayerWeight(layerIndex, weight), currentWeight, targetWeight, duration);
                }

                yield return new WaitForSeconds(duration);

                changeDataList.RemoveAt(0);
            }

            isWeightChanging = false;
        }

        private void SetLayerWeight(int layerIndex, float weight)
        {
            anim.SetLayerWeight(layerIndex, weight);
        }
    }


    //using UnityEngine;
    //using System.Collections;

    //public class AnimatorLayerWeightController : MonoBehaviour
    //{
    //    [TextArea]
    //    [ContextMenuItem("Reset Description", "ResettingDescription")]
    //    public string ScriptInfo = "This script is responsible for controlling the speed of changing the animator layers weight to achieve the smooth layer transitioning effect from one layer to another.";

    //    private class LayerWeightChangeData
    //    {
    //        public int layerIndex;
    //        public float targetWeight;
    //        public bool fillToZero;
    //    }

    //    private Animator anim;
    //    private bool isWeightChanging = false;
    //    private LayerWeightChangeData currentChangeData;

    //    public float AnimatorWeightInterpolationDuration = 0.1f;

    //    private void Awake()
    //    {
    //        anim = GetComponent<Animator>();
    //    }

    //    public void ChangeLayerWeight(int layerIndex, float targetWeight, bool fillToZero = false)
    //    {
    //        if (isWeightChanging)
    //        {
    //            return; // Ignore the request if a weight change is already in progress
    //        }

    //        StartCoroutine(ChangeLayerWeightCoroutine(layerIndex, targetWeight, fillToZero));
    //    }

    //    private IEnumerator ChangeLayerWeightCoroutine(int layerIndex, float targetWeight, bool fillToZero)
    //    {
    //        isWeightChanging = true;

    //        float currentWeight = anim.GetLayerWeight(layerIndex);

    //        if (Mathf.Approximately(currentWeight, targetWeight))
    //        {
    //            isWeightChanging = false;
    //            yield break;
    //        }

    //        currentChangeData = new LayerWeightChangeData
    //        {
    //            layerIndex = layerIndex,
    //            targetWeight = targetWeight,
    //            fillToZero = fillToZero
    //        };

    //        //   float duration = Mathf.Abs(targetWeight - currentWeight) * AiAgentAnimatorParameters.AnimatorWeightInterpolationDuration;
    //        float duration = Mathf.Abs(targetWeight - currentWeight) * AnimatorWeightInterpolationDuration; 

    //        if (fillToZero)
    //        {
    //            LeanTween.value(gameObject, (weight) => SetLayerWeight(layerIndex, weight), currentWeight, 0f, duration);
    //        }
    //        else
    //        {
    //            LeanTween.value(gameObject, (weight) => SetLayerWeight(layerIndex, weight), currentWeight, targetWeight, duration);
    //        }

    //        yield return new WaitForSeconds(duration);

    //        isWeightChanging = false;
    //    }

    //    private void SetLayerWeight(int layerIndex, float weight)
    //    {
    //        anim.SetLayerWeight(layerIndex, weight);
    //    }
    //}
}