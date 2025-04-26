using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float _pullBackIncreaseFactor = 0.2f;
    private CinemachineTransposer _transposer;
    private ConsumableObject _consumableObjectInBetweenPlayerAndCamera;
    private Vector3 _baseOffset;
    private Vector3 _targetOffset;

    private void Awake() {
        _transposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        _baseOffset = _transposer.m_FollowOffset;
        _targetOffset = _baseOffset;
    }
    void OnEnable() {
        GameManager.OnLevelUp += TriggerAdaptToLevelUp;
    }

    void OnDisable() {
        GameManager.OnLevelUp -= TriggerAdaptToLevelUp;
    }

    void Update() {
        PlayerHole player = GameObject.FindWithTag("PlayerHoleCharacter").GetComponent<PlayerHole>();
        if (player == null) {
            return;
        }

        if(_targetOffset != _transposer.m_FollowOffset) {
            _transposer.m_FollowOffset = Vector3.Lerp(_transposer.m_FollowOffset, _targetOffset, Time.deltaTime);
        }

        Vector3 direction = player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.collider == null) {
                return;
            }

            if (hit.collider.CompareTag("PlayerHoleCharacter"))
            {
                if (_consumableObjectInBetweenPlayerAndCamera != null) {
                    _consumableObjectInBetweenPlayerAndCamera.ReturnTransparencyToNormal();
                    _consumableObjectInBetweenPlayerAndCamera = null;
                }
            } else if (hit.collider.CompareTag("Consumable")) {
                _consumableObjectInBetweenPlayerAndCamera = hit.collider.GetComponent<ConsumableObject>();
                if (_consumableObjectInBetweenPlayerAndCamera != null) {
                    _consumableObjectInBetweenPlayerAndCamera.MakeObjectTransparent();
                }
            }
        }
    }
    private void TriggerAdaptToLevelUp() {
        _targetOffset = _baseOffset + new Vector3(0.0f, 
                                                  _pullBackIncreaseFactor * GameManager.Instance.CurrentLevel, 
                                                  -_pullBackIncreaseFactor * GameManager.Instance.CurrentLevel);
    }
}
