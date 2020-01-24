using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMove : MonoBehaviour {

    public enum States
    {
        Idle, 
        Walking,
        Sleeping,
        Playing,
        Eating,
        Bathing
    }

    public enum AudioClips
    {
        Normal,
        Distressed,
        Purr
    }

    public States CurrentState;
    float walkSpeed = 30.0f;
    float rotateSpeed = 2.0f;
    public Vector3 waypoint;
    bool isWaypoint = false;

    Vector3 headPosition;
    GameObject targetObject;

    public AudioClips CurrentSound;
    

    float min_distance = 50.0f;
    float max_distance = 200.0f;

    Animator anim;

    AudioSource sound;
    AudioClip normal;
    AudioClip purring;
    AudioClip distressed;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();

        waypoint = transform.position;
        headPosition = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z - 60);

        normal = Resources.Load("Cat Meow (Normal)") as AudioClip;
        purring = Resources.Load("Cat Purr") as AudioClip;
        distressed = Resources.Load("Cat Meow (Upset)") as AudioClip;
    }
	
	// Update is called once per frame
	void Update ()
    {
        changeAudioClip();
        senseObjects();

        float rand = Random.Range(1.0f, 750.0f);
		if(CurrentState == States.Idle)
        {
            //anim.SetBool("isMoving", false);
           
            if((int)rand == 2)
            {
                sound.Play();
                CurrentState = States.Walking;
            }
        }
        else if(CurrentState == States.Walking)
        {
            sound.Play();
            //anim.SetBool("isMoving", true);

            createWaypoint();
            MoveTo(waypoint);
            
        }
        else if(CurrentState == States.Sleeping)
        {
            print(CurrentState);
            createWaypoint();
            MoveTo(waypoint);
            sound.Play();
            //anim.SetBool("isSleeping", true);
            //run sleep animation
            //wait 20s

            // anim.SetBool("isSleeping", false);
        }
        else if(CurrentState == States.Bathing)
        {
            print(CurrentState);
            //move to tub
            createWaypoint();
            MoveTo(waypoint);

            sound.Play();
        }
        else if(CurrentState == States.Eating)
        {
            print(CurrentState);
            //move to bowl
            createWaypoint();
            MoveTo(waypoint);

            //when bowl is empty, meow insessently
            sound.Play();
        }
        else if(CurrentState == States.Playing)
        {
            print(CurrentState);
            sound.Play();

            //while toy is in play, chase it
            while (targetObject != null)
            {
                //print("Loop ?");
                createWaypoint();
                MoveTo(waypoint);
            }

            //on toy capture destory it - see OnTriggerEnter below
        }
        else
        {
            CurrentState = States.Idle;
        }
	}

    void createWaypoint()
    {
        if (!isWaypoint)
        {
            GameObject target;

            if (CurrentState == States.Walking)
            {
                float exp = Random.Range(1.0f, 4.0f);
                float dist_x = transform.position.x + Random.Range(min_distance, max_distance)*(Mathf.Pow(-1.0f,(int)exp));
                exp = Random.Range(1.0f, 4.0f);
                float dist_z = transform.position.z + Random.Range(min_distance, max_distance)* (Mathf.Pow(-1.0f, (int)exp));

                waypoint = new Vector3(dist_x, transform.position.y, dist_z);
            }
            else if (CurrentState == States.Bathing)
            {
                //waypoint = tub
                target = GameObject.Find("Shower Tub");
                if(target != null)
                {
                    waypoint = target.transform.position;
                }
                else
                {
                    CurrentState = States.Idle;
                }
            }
            else if (CurrentState == States.Eating)
            {
                //waypoint = bowl
                target = GameObject.Find("Bowl");
                if (target != null)
                {
                    waypoint = target.transform.position;
                }
                else
                {
                    CurrentState = States.Idle;
                }
            }
            else if (CurrentState == States.Playing)
            {
                //waypoint = toy
                target = GameObject.Find("Toy");
                if (target != null)
                {
                    waypoint = target.transform.position;
                }
                else
                {
                    CurrentState = States.Idle;
                }
            }
            else if (CurrentState == States.Sleeping)
            {
                //waypoint = toy
                target = GameObject.Find("Bed");
                if (target != null)
                {
                    waypoint = target.transform.position;
                }
                else
                {
                    CurrentState = States.Idle;
                }
            }

            isWaypoint = true;
        }
    }

    void MoveTo(Vector3 wP)
    {
        CurrentState = States.Walking;

        transform.position = Vector3.MoveTowards(transform.position, wP, walkSpeed * Time.deltaTime);
        GetComponent<Rigidbody>().position = Vector3.MoveTowards(transform.position, wP, walkSpeed * Time.deltaTime);
        headPosition.x = wP.x;
        headPosition.z = wP.z - 60;

        var wpRotation = Quaternion.LookRotation(transform.position-waypoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, wpRotation, rotateSpeed * Time.deltaTime);

        if(transform.position == wP)
        {
            isWaypoint = false;

            if(CurrentState != States.Playing)
            {
                CurrentState = States.Idle;
            }
        }
    }

    void changeAudioClip()
    {
        if (CurrentState == States.Idle)
        {
            CurrentSound = AudioClips.Normal;
            sound.clip = normal;
        }
        else if (CurrentState == States.Walking)
        {
            CurrentSound = AudioClips.Normal;
            sound.clip = normal;
        }
        else if (CurrentState == States.Sleeping)
        {
            CurrentSound = AudioClips.Purr;
            sound.clip = purring;
        }
        else if (CurrentState == States.Bathing)
        {
            CurrentSound = AudioClips.Distressed;
            sound.clip = distressed;
        }
        else if (CurrentState == States.Eating)
        {
            CurrentSound = AudioClips.Purr;
            sound.clip = purring;
        }
        else if (CurrentState == States.Playing)
        {
            CurrentSound = AudioClips.Normal;
            sound.clip = normal;
        }
        else
        {
            CurrentSound = AudioClips.Normal;
            sound.clip = normal;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Toy"))
        {
            Destroy(other.gameObject);
            sound.Play();
            CurrentState = States.Idle;
        }
    }

    void senseObjects()
    {
        //ray can be refined
        Ray myRay = new Ray(headPosition, transform.TransformDirection(-transform.position));

        Debug.DrawRay(headPosition, transform.TransformDirection(-transform.position));

        RaycastHit hitInfo = new RaycastHit();
        bool hits = Physics.Raycast(myRay, out hitInfo, 100.0f);

        if(hits)
        {
            targetObject = hitInfo.collider.gameObject;

            if(hitInfo.collider.CompareTag("Toy"))
            {
                CurrentState = States.Playing;
            }
            else if (hitInfo.collider.name == "Shower Tub")
            {
                CurrentState = States.Bathing;
            }
            else if (hitInfo.collider.name == "Bed")
            {
                CurrentState = States.Sleeping;
            }
            else if (hitInfo.collider.name == "Bowl")
            {
                CurrentState = States.Eating;
            }
            else
            {
                //CurrentState = States.Idle;
            }
        }
    }
}
