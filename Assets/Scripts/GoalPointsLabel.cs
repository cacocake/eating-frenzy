public class GoalPointsLabel : PointsLabel {

    protected override void Start() {
        base.Start();
        _labelTextPrefix = "Goal: {0}/{1}";
    }

    void Update() {
        UpdateLabel(_manager.TotalPoints, _manager.StageTargetPoints);   
    }
}
