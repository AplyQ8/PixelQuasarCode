using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTimeScale : MonoBehaviour
{
    
    [Range(0, 1)][SerializeField] private float timeScaleWhileStop;
    private bool _paused;

    void Start()
    {
        _paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _paused = !_paused;
            Time.timeScale = _paused ? timeScaleWhileStop :  1;
        }
    }
}