using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarSetting : MonoBehaviour {

    protected const float moveSpeed = 0.1f;
    protected const float stopMoveDist = 0.015f;

    public PolygonImage polygonImage;
    public Text[] subjects;

    List<float> targetValues = new List<float>();

    int weightCount = 6;
    // Start is called before the first frame update
    void Awake() {
        setWeightCount(weightCount);
    }
    public void setWeightCount(int cnt) {
        polygonImage.setWeightCount(weightCount = cnt);
        targetValues = polygonImage.getWeights();
    }
    public void setValues(List<float> values) {
        targetValues = values;
    }
    public void setValue(int index, float value) {
        if (index < targetValues.Count)
            targetValues[index] = value;
    }
    public void setNames(List<string> names) {
        for(int i = 0; i < subjects.Length; i++) 
            subjects[i].text = names[i];
    }
    // Update is called once per frame
    void Update() {
        if(isAnimationPlaying())
            updateMoveAnimation();
        else resetAnimation();
    }

    void updateMoveAnimation() {
        for (int i = 0; i < weightCount; i++)
            polygonImage.addWeight(i, (targetValues[i] - 
                polygonImage.getWeight(i)) * moveSpeed);
    }
    public bool isAnimationPlaying() {
        return !isAnimationStopping();
    }
    bool isAnimationStopping() {
        float delta = 0;
        for (int i = 0; i < weightCount; i++)
            delta += Mathf.Abs(targetValues[i] - polygonImage.getWeight(i));
        return delta < stopMoveDist;
    }
    public void resetAnimation() {
        for (int i = 0; i < weightCount; i++)
            polygonImage.setWeight(i, targetValues[i]);
        targetValues = polygonImage.getWeights();
        //targetValues = polygonImage.edgeWeights.getWeights();
    }
    public void clear() {
        int max = polygonImage.getWeightCount();
        for (int i = 0; i < max; i++) {
            polygonImage.setWeight(i, 0);
            setValue(i, 0);
        }
    }
}
