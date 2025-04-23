public class NextLevelLabel : PointsLabel {

    protected override void Start() {
        base.Start();
        _labelTextPrefix = "Next Level: {0}/{1}";
    }

    void Update() {
        UpdateLabel(_manager.CurrentLevelPoints, _manager.CurrentLevelTargetPoints);   
    }
}
