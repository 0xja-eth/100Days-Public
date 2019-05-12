using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PolygonImageEdge {
    public int EdgeCount {
        get {
            if (m_Weights == null) return 0;
            return m_Weights.Count;
        }
    }

    public List<float> Weights {
        get { return m_Weights; }
        set { m_Weights = value; }
    }

    [SerializeField] private List<float> m_Weights;
    /*
    public void setWeights(List<float> weights) {
        m_Weights = weights;
    }
    public void setWeight(int index, float value) {
        if (index < m_Weights.Count)
            m_Weights[index] = value;
    }
    public void addWeight(int index, float value) {
        if (index < m_Weights.Count)
            m_Weights[index] += value;
    }
    public List<float> getWeights() {
        return m_Weights;
    }
    public float getWeight(int index) {
        return m_Weights[index];
    }
    public int getWeightCount() {
        return m_Weights.Count;
    }
    public void setWeightCount(int count) {
        if(count > m_Weights.Count)
            for (int i = m_Weights.Count; i < count; i++)
                m_Weights.Add(0);
        if (count < m_Weights.Count)
            m_Weights.RemoveRange(count, m_Weights.Count - count);
    }*/
}
