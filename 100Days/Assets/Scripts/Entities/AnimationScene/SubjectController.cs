using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubjectController : AnimatableLayer{
    public Toggle arts;
    public Toggle science;

    public int getSubjectSelection() {
        if (arts.isOn) return 0;
        if (science.isOn) return 1;
        return -1;
    }
}
