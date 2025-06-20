using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Threading.Tasks;

public class PlayerCamera : MonoBehaviour {

    [SerializeField] private float _pullBackIncreaseFactor = 0.2f;
    [SerializeField] private float _pullBackUponPlayerSizeIncreaseDuration = 0.75f;
    [SerializeField] private AnimationCurve _pullbackAnimationCurve;
    
    private CinemachineTransposer _transposer;
    private ConsumableObject _consumableObjectInBetweenPlayerAndCamera;
    private Vector3 _baseOffset;

    private void Awake() {
        _transposer = null;
        if(TryGetComponent<CinemachineVirtualCamera>(out var cinemachineCamera)){
            _transposer = cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
        }
        _baseOffset = _transposer.m_FollowOffset;
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

        Vector3 direction = player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.collider == null) {
                return;
            }

            if (hit.collider.CompareTag("PlayerHoleCharacter")) {
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
        if(_transposer == null) {
            Debug.LogError("_transposer is null!");
            return;
        }
        
        StartCoroutine(PanOutCameraOverTimeDependingOnLevel());
    }

    private IEnumerator PanOutCameraOverTimeDependingOnLevel() {
        Vector3 targetOffset = _baseOffset + new Vector3(0.0f, 
                                                  _pullBackIncreaseFactor * GameManager.Instance.CurrentLevel, 
                                                  -_pullBackIncreaseFactor * GameManager.Instance.CurrentLevel);
        Vector3 currentOffset = _transposer.m_FollowOffset;
        float elapsedTime = 0.0f;
        while(elapsedTime <= _pullBackUponPlayerSizeIncreaseDuration) {
            float progressPercentage = elapsedTime / _pullBackUponPlayerSizeIncreaseDuration;
            _transposer.m_FollowOffset = Vector3.Lerp(currentOffset, 
                                                      targetOffset, 
                                                      _pullbackAnimationCurve.Evaluate(progressPercentage));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _transposer.m_FollowOffset = targetOffset;
    }

}
