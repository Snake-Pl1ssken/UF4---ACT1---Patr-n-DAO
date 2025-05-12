using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int index;

    LevelManager levelManagerC;

    // Start is called before the first frame update
    void Start()
    {
        levelManagerC = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Constants.layerPlayerCar)
        {
            Car car = other.transform.parent.gameObject.GetComponent<Car>();

            levelManagerC.OnGatePassed(index, car.GetSpeed());
        }
    }
}
