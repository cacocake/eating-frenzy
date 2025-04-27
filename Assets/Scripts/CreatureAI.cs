using UnityEngine;
public class CreatureAI : MonoBehaviour {
    
    [SerializeField] private float _alertRange = 5.0f;
    [SerializeField] private float _timeUntilCalmDown = 2.0f;
    [SerializeField] private float _movementChangeCooldown = 0.5f;
    
    private float _elapsedTimeSinceMovementChange = 0.0f;
    private Controller.CreatureMover _mover;
    private Vector2 _axis;
    private Vector3 _target;
    private GameObject _playerHole;
    private bool _isRunningAway = false;
    
    private void Awake() {
        _mover = GetComponent<Controller.CreatureMover>();
    }
    
    private void Start() {
        _playerHole = GameObject.FindWithTag("PlayerHoleCharacter");
    }
    
    private void Update() {
        TrySetInputToRunAwayFromHole();
        SetInput();
    }
    
    public void TrySetInputToRunAwayFromHole() {
        _elapsedTimeSinceMovementChange += Time.deltaTime;
        if(_playerHole == null) {
            return;
        }
        if(Vector3.Distance(_playerHole.transform.position, transform.position) > _alertRange) {
            if(_isRunningAway) {
                _timeUntilCalmDown -= Time.deltaTime;
                if(_timeUntilCalmDown <= 0) {
                    _axis = Vector2.zero;
                    _target = Vector3.zero;
                    _isRunningAway = false;
                }
            }
            return;
        }
        if(_elapsedTimeSinceMovementChange >= _movementChangeCooldown) {
            Vector3 direction = (transform.position - _playerHole.transform.position).normalized;
            _axis = new Vector2(direction.x, direction.z);
            _target = new Vector3(direction.x, 0.0f, direction.z);
            _timeUntilCalmDown = 2.0f;
            _isRunningAway = true;
            _elapsedTimeSinceMovementChange = 0.0f;
        }           
    }
    
    public void BindMover(Controller.CreatureMover mover) {
        _mover = mover;
    }
    
    public void SetInput() {
        if (_mover != null) {
            _mover.SetInput(in _axis, in _target, true, false);
        }
    }

}