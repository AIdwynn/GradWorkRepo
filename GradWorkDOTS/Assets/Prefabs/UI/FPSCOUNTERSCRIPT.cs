using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCOUNTERSCRIPT : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    // Update is called once per frame
    void Update()
    {
        _textMeshPro.text = (1f / Time.deltaTime).ToString("0");
    }
}
