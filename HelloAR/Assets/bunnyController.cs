using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bunnyController : MonoBehaviour {

    // Use this for initialization
    private GameObject[] chaseList;
    private float mindistance;
    private GameObject toChase;
    //public TextMesh text;
    private Animator animator;

	//find strawberry

	public float speed;

	//rotation - set in the Inspector
	public Transform Target;
	public float RotationSpeed;

	//values rotation internal use
	private Quaternion _walkRotation;
	private Vector3 _direction; 


	//particle system
	//public static SearchAnimController Instance;
	public ParticleSystem hearts; 
	public float timer;

	void Start () {
        mindistance = float.MaxValue;
        toChase = null;
        hearts = gameObject.GetComponent<ParticleSystem>();
        //text = GameObject.Find("catch").GetComponent<TextMesh>();
		speed = 0.1f;
		RotationSpeed = 1;
		hearts.Stop();
		timer = 0;
        animator = gameObject.GetComponent<Animator>();
        //text.text = "began";
	
	}
	
	// Update is called once per frame
	void Update () {
        //text.text = animator.ToString();
        animator.SetBool("isWalking", false);
        //text.text = "end?";
        mindistance = float.MaxValue;
        //this is the list off all objects the character can chase
        chaseList = GameObject.FindGameObjectsWithTag("Interactable");
        //text.text = "no";
        //if the list is less than 1 quit
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }
        if (timer <= 0)
        {
            animator.SetBool("hasEaten", false);
            hearts.Stop();
        }

        //text.text = chaseList.Length.ToString();
        if (chaseList.Length < 1)
        {
            return;
        }
        //check each chasable object and pick teh one that is closest
        foreach(GameObject chase in chaseList)
        {
            float tmpdist = Vector3.Distance(transform.position, chase.transform.position);
            if (tmpdist < mindistance && chase.name.Contains("Candy"))
            {
                //set min distance
                mindistance = tmpdist;
                //set gameobject to chase
                toChase = chase;
            }
        }

		if (toChase.Equals(null)) {
			return;
		}
        animator.SetBool("isWalking", true);
        //text.text = toChase.transform.name.ToString();
        

		hearts = gameObject.GetComponent<ParticleSystem> ();

		if (Vector3.Distance (toChase.transform.position, transform.position) > 0.01f) {

			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, toChase.transform.position, step);

			//find the vector pointing from our position to the target
			//_direction = (Target.position - transform.position).normalized;
			_direction = (toChase.transform.position - transform.position).normalized;

			//create the rotation needed to match the target

			//_walkRotation = Quaternion.LookRotation (_direction);
			_walkRotation = Quaternion.LookRotation (_direction);

			//rotate charcter over time according to speed until required rotation
			transform.rotation = Quaternion.Slerp (transform.rotation, _walkRotation, Time.deltaTime * RotationSpeed);

		} else {
			Destroy (toChase);
			hearts.Play();
            animator.SetBool("hasEaten", true);
			timer = 2;
		}
	}
		
}
