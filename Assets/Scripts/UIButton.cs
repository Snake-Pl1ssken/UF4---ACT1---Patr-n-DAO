using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton: MonoBehaviour
{
    public delegate void OnPressed();
    public OnPressed onPressed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        onPressed.Invoke();
    }
}
