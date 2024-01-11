using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCOUNTERSCRIPT : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    private float togetherFPS;
    private float frames;

    // Update is called once per frame
    void Update()
    {
        var fps = 1f / Time.deltaTime;
        togetherFPS += fps;
        frames++;
        _textMeshPro.text = $"FPS:         {fps} \nAverageFPS: {togetherFPS/frames}";
    }
}
