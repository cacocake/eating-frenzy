using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLabel : PointsLabel{
    protected override void Start() {
        base.Start();
        _labelTextPrefix = "Time\n{0}";
    }

    void Update() {
        UpdateLabel((ushort)Mathf.Floor(_manager.StageTimeLimit));   
    }
}
