using UnityEngine;
using System.Collections;

public class RandomBehaviour : MonoBehaviour {

    GlobalVars gameVars;
    GameSpeedManager gsm;
    Animator myAnimator;
    Collider2D myCollider;
    System.Random rnd = new System.Random();
    float mySpeed = 0.1f;
       
    Vector3 initLocalPosition;
    public Vector3 initPosition;  

    public Vector2 myDirection = new Vector2(0.0f, 0.0f);
    public float initAnimatorSpeed;
    public bool allowXInversion = true;
    public bool verticalOnly = false;
    public bool pursuitSPZ = false;
    float myBoostSpeed = 5.0f;
    public Transform boostPosition;

    delegate void moveFunctionPointer();
    moveFunctionPointer randomMove;

    static bool inPursuit = false;    
    

    // Use this for initialization
    void Awake ()
    {
        myAnimator = gameObject.GetComponent<Animator>();
        if (myAnimator != null) myAnimator.speed = initAnimatorSpeed;
        initLocalPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);        
    }
    void Start()
    {
        //gameVars.StepsChanged += RandomizeBehaviour;
        if (verticalOnly) {
            randomMove = VerticalMove;
        }else
        {
            randomMove = Move;
        }
        if (myDirection.x < 0.0f && allowXInversion)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if (pursuitSPZ)
        {
            myCollider = gameObject.GetComponent<Collider2D>();
            if (myCollider == null) { Debug.Log("Warning no collider set for " + gameObject.name); }
            GameObject gameManager = GameObject.FindGameObjectWithTag(GlobalTags.GameManager);
            gameVars = gameManager.GetComponent<GlobalVars>();
            gameVars.StepsChanged += this.OnStepsChanged;
            gsm = gameManager.GetComponent<GameSpeedManager>();
            if (boostPosition == null) { Debug.Log("Warning boostPosition not set for " + gameObject.name); }
        }


    }
	
	// Update is called once per frame
	void Update () {
        if (randomMove != null) { 
            randomMove();
        }
    }

    void RandomizeBehaviour(int stepsAhead)
    {
        Debug.Log("Randomisation en cours pour "+gameObject.name +"resultats : ");

        float sp = rnd.Next(10) / 20.0f;
        myAnimator.speed += sp;
        Debug.Log("Speed : " + sp);

        sp = rnd.Next(-10, 10) / 10.0f;
        myDirection.x = sp;
        Debug.Log("X : " + sp);

        sp = rnd.Next(-10, 10) / 10.0f;
        myDirection.y = sp;
        Debug.Log("Y : " + sp);

    }

    float TrimX(float x)
    {
        if (x > initLocalPosition.x + 0.3f) return initLocalPosition.x + 0.3f;
        if (x < initLocalPosition.x - 0.3f) return initLocalPosition.x - 0.3f;
        return x;
    }
    float TrimY(float y)
    {
        if (y > initLocalPosition.y + 0.3f) return initLocalPosition.y + 0.3f;
        if (y < initLocalPosition.y - 0.3f) return initLocalPosition.y - 0.3f;
        return y;
    }

    void Move()
    {
        transform.localPosition = new Vector3(TrimX(Mathf.Lerp(transform.localPosition.x, transform.localPosition.x + myDirection.x, mySpeed * Time.deltaTime)), TrimY(Mathf.Lerp(transform.localPosition.y, transform.localPosition.y+myDirection.y, mySpeed * Time.deltaTime)), transform.localPosition.z);

        if (Vector2.Distance(new Vector2(TrimX(initLocalPosition.x + myDirection.x), TrimY(initLocalPosition.y + myDirection.y)), transform.localPosition) < 0.05f)
        {
            myDirection.x = -myDirection.x;
            myDirection.y = -myDirection.y;
        }
    }

    void VerticalMove()
    {
        transform.position = new Vector3(transform.position.x, TrimY(Mathf.Lerp(transform.position.y, transform.position.y + myDirection.y, mySpeed * Time.deltaTime)), transform.position.z);

        if (Vector2.Distance(new Vector2(transform.position.x, TrimY(initLocalPosition.y + myDirection.y)), transform.localPosition) < 0.05f)
        {
            myDirection.x = -myDirection.x;
            myDirection.y = -myDirection.y;
        }
    }

    void AddGoToMid()
    {
        RandomBehaviour.inPursuit = false;
        myAnimator.SetBool("PreBoost", false);
        myAnimator.speed = myAnimator.speed * 2.0f;
        initPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        randomMove = GoToMid;
        
    }

    void GoToMid()
    {
        if (transform.position.x < boostPosition.position.x) { 
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + gameVars.linearTranslationOffset, gsm.GameSpeed * myBoostSpeed * Time.deltaTime), 
                                            transform.position.y , transform.position.z);
        }else{
            randomMove = GoBack;
            myAnimator.speed = myAnimator.speed / 2.0f;
        }
    }

    void GoBack()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, initPosition.x, gsm.GameSpeed * myBoostSpeed / 20.0f * Time.deltaTime),
                                            transform.position.y, transform.position.z);
        if (Mathf.Abs(transform.position.x - initPosition.x) < 0.1f)
        {
            randomMove = Move;
            gameVars.StepsChanged += this.OnStepsChanged;
        }
        
    }

    public void OnStepsChanged(int steps)
    {
       
       if (steps < -2) { 
           if (!RandomBehaviour.inPursuit){
                RandomBehaviour.inPursuit = true;
                myAnimator.SetBool("PreBoost", true);
                Invoke("AddGoToMid", 2.0f);
                gameVars.StepsChanged -= this.OnStepsChanged;
           }
        }
    }

    void StopBoost()
    {
        myAnimator.SetBool("PreBoost", false);
        RandomBehaviour.inPursuit = false;
    }
    

}
