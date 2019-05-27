using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private delegate void CameraRotator();
    private CameraRotator CameraRotateHolder;

    public GameObject[] balls;
    Transform _t;
    Transform t
    {
        get
        {
            if (_t == null)
                _t = GetComponent<Transform>();
            return _t;
        }
    }


    private Vector2 startInputPos;
    private bool mouseIsHold;
    private Vector2 currInputPos;
    private Vector2 dirInputMove;
    private float currentAngle;
    private float targetAngle;
    private float rotationDamping;
    private float sens = 80f;

    private int _currentBall;
    private int currentBallIndex
    {
        get => _currentBall;
        set
        {
            if (value < 0)
                _currentBall = balls.Length + value;
            else if (value >= balls.Length)
                _currentBall = value % balls.Length;
            else
                _currentBall = value;
        }
    }

    private void Awake()
    {
        currentBallIndex = 0;
        SetCameraPos(currentBallIndex);
    }

    public void OnRightButtonClick()
    {
        balls[currentBallIndex].GetComponent<BallMover>().OnMovingPause();
        currentBallIndex++;
        SetCameraPos(currentBallIndex);
    }

    public void OnLeftButtonClick()
    {
        currentBallIndex--;
        SetCameraPos(currentBallIndex);
    }

    private void SetCameraPos(int index)
    {
        t.parent = balls[currentBallIndex].transform;
        t.localPosition = -Vector3.right * 1.4f;
        t.localRotation = Quaternion.Euler(90 * Vector3.up);
    }

    private void Start()
    {
        StartCoroutine(GameInput());
    }

    private void Update()
    {
        CameraRotateHolder?.Invoke();
    }

    private void RotateCamera()
    {
        t.RotateAround(balls[currentBallIndex].transform.position, Vector3.up, sens * dirInputMove.normalized.x * Time.deltaTime);
    }

    private IEnumerator GameInput()
    {
        yield return null;
        while (true)
        {
            yield return null;

#if UNITY_EDITOR
            #region mouse controll
            if (Input.GetMouseButtonDown(0))
            {
                CameraRotateHolder = RotateCamera;
                startInputPos = Input.mousePosition;
                mouseIsHold = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseIsHold = false;
                CameraRotateHolder = null;
                dirInputMove = Vector2.zero;
            }

            if (mouseIsHold)
            {
                currInputPos = Input.mousePosition;
                if (currInputPos == startInputPos) continue;
                dirInputMove = 0.2f * (currInputPos - startInputPos);
            }

            #endregion
#else
            #region touch controll
            if (Input.touchCount == 0)
            {
                CameraRotateHolder = null;
                continue;
            }

            else
            {
                CameraRotateHolder = RotateCamera;

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    startInputPos = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    currInputPos = Input.GetTouch(0).position;
                    if (currInputPos == startInputPos) continue;
                    dirInputMove = 0.2f * (currInputPos - startInputPos);
                }
            }
            #endregion
#endif
        }
    }
}
