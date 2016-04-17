using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    public static Combo Instance;

    public float Timer;
    public Text Text;
    public Animator Animator;
    int _Count;
    float _Deadline;

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (_Deadline != 0f && Time.time > _Deadline)
        {
            Animator.Play("Idle");
            _Count = 0;
            _Deadline = 0f;
        }
    }

    internal void AddCombo(float damage)
    {
        _Count++;
        _Deadline = Time.time + Timer;
        Animator.SetInteger("Count", _Count);
        Animator.Play("Hit");
        Text.text = _Count.ToString();
    }
}
