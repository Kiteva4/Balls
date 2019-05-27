using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallMover : MonoBehaviour, IPointerClickHandler
{
    private delegate void MoveToTrajectory();
    private MoveToTrajectory MoveHandler;

    private TrailRenderer _trail;
    private TrailRenderer trail
    {
        get
        {
            if(_trail == null)
                _trail = GetComponent<TrailRenderer>();
            return _trail;
        }
    }

    public BallPositionWrapper ballRecordedPositions;

    #region Variables
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

    [Tooltip("В данном поле необходимо указать путь к файлу в формате, например \"Resources/file_name\"")]
    public string dataPath;

    /// <summary>
    /// Счетчик текущего положения в массиве записанных позиций ширика
    /// </summary>
    private int counter;
    /// <summary>
    /// Путь к файлу
    /// </summary>
    private string path;

    Vector3 targetPos;
    private float doubleClickStart;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        ballRecordedPositions = new BallPositionWrapper();
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, dataPath + ".json");
#else
        path = Path.Combine(Application.dataPath, dataPath + ".json");
#endif
        if (File.Exists(path))
        {
            ReadFromJson();
        }
        else Debug.LogError("File dont exists");

        InitFirstTargetPosition();
    }

    private void Update()
    {
        MoveHandler?.Invoke();
    }
    #endregion

    private void ReadFromJson()
    {
        ballRecordedPositions = JsonUtility.FromJson<BallPositionWrapper>(File.ReadAllText(path));
        Debug.Log("readed");
        ballRecordedPositions.InitDictionaries();
        Debug.Log("Init");
    }

    private void Move()
    {
        t.position = Vector3.Lerp(t.position, GetNextPosition(), 40f * Time.deltaTime);
    }

    /// <summary>
    /// Метод возвращает следующее значение позиции из массива
    /// </summary>
    /// <returns>The position from records.</returns>
    private Vector3 GetTargetPosition()
    {

        float _x = ballRecordedPositions.x[counter];
        float _y = ballRecordedPositions.y[counter];
        float _z = ballRecordedPositions.z[counter];

        targetPos = new Vector3(_x, _y, _z);

        counter++;
        if (counter == ballRecordedPositions.x.Count)
        {
            MoveHandler = null;
        }

        return targetPos;
    }

    private Vector3 InitFirstTargetPosition()
    {
        GetTargetPosition();
        t.position = targetPos;
        return targetPos;
    }

    private Vector3 GetNextPosition()
    {
        if (Vector3.Distance(t.position, targetPos) > .1f)
            return targetPos;

        return GetTargetPosition();
    }

    /// <summary>
    /// Приостановить движение
    /// </summary>
    public void OnMovingPause()
    {
        MoveHandler = null;
    }

    /// <summary>
    /// Продолжить движение с тогоже места
    /// </summary>
    public void OnMovingcontinue()
    {
        MoveHandler = Move;
    }

    /// <summary>
    /// Сбросить позицию и остановить движение
    /// </summary>
    public void OnReset()
    {
        counter = 0;
        InitFirstTargetPosition();
        MoveHandler = null;
    }

    #region Clicks
    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - doubleClickStart) < 0.3f)
        {
            OnDoubleClick();
            doubleClickStart = -1;
        }
        else
        {
            doubleClickStart = Time.time;

            if (MoveHandler == null && counter < ballRecordedPositions.x.Count)
                MoveHandler = Move;
        }
    }

    void OnDoubleClick()
    {
        OnReset();
        trail.Clear();
        Debug.Log("DoubleCLicked!");
    }
    #endregion
}

/// <summary>
/// Для считывания исходный файлов формата json
/// </summary>
[System.Serializable]
public class BallPositionWrapper
{
    public Dictionary<string, List<float>> dict_X = new Dictionary<string, List<float>>();
    public Dictionary<string, List<float>> dict_Y = new Dictionary<string, List<float>>();
    public Dictionary<string, List<float>> dict_Z = new Dictionary<string, List<float>>();

    [HideInInspector]
    public List<float> x = new List<float>();
    [HideInInspector]
    public List<float> y = new List<float>();
    [HideInInspector]
    public List<float> z = new List<float>();

    public void InitDictionaries()
    {
        dict_X.Add("x", x);
        dict_Y.Add("x", y);
        dict_Z.Add("x", z);
    }
}