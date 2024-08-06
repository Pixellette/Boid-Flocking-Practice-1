using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;
    bool turning = false; 


    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed); // Call from flock manager

    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2); // setting a box they have to stay in - kinda like a collision

        if (!b.Contains(transform.position)) // tests the fish's location
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction= FlockManager.FM.transform.position - transform.position;  // working out the vector that the fish has to go to get back to the centre
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                                    Quaternion.LookRotation(direction),
                                                    FlockManager.FM.rotationSpeed * Time.deltaTime);

        }
        else 
        {
            if (Random.Range(0, 100) < 10) // randomly reset speed
            {
                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            }

            if (Random.Range(0, 100) < 50) // Randomise chance to apply rules
            {
                ApplyRules();
            }
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = FlockManager.FM.allFish;

        Vector3 vcentre = Vector3.zero; // average center of group? 
        Vector3 vavoid = Vector3.zero; // avoid others 
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0; 

        foreach(GameObject go in gos) // for each Game Object in Game Objects 
        {
            if(go != this.gameObject)
            {
                nDistance= Vector3.Distance(go.transform.position, this.transform.position);
                if(nDistance <= FlockManager.FM.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f) // if we're too close to that particular fish
                    {
                        vavoid = vavoid +(this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcentre = vcentre/groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = gSpeed/groupSize;
            if (speed > FlockManager.FM.maxSpeed)
            {
                speed = FlockManager.FM.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - transform.position;
            if(direction != Vector3.zero) 
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                                        Quaternion.LookRotation(direction),
                                        FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
