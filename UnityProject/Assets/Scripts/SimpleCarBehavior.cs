using UnityEngine;
using System.Collections;

//KNOWN DEFECT: Current rigidbody velocity approach is inconsisatant and causes hiccups and unpredictable results. | If multiple walls are overlapping at collission, can bug out.
//This is object avoidance. To solve a maze, start it facing a left wall. It will then trace around the left side of the maze.

public enum State
{
    Base,
    GoForward,
    Avoidance,
    Side,
    Return,
    TurnLeft,
    TurnRight,
    TurnAround,
    BackUp,
    BAD
}

public class SimpleCarProperties
{
    public float timeSinceChange;
    //public float[] distanceTraveled = new float[5];
    public float previousDistanceTraveled = 0.0f;
    public float distanceTraveled = 0.0f;
    public float sideDistanceTraveled = 0.0f;
    public float avoidanceDistanceTraveled = 0.0f;
    public int iterationCount = 0;
    public State mainState;

    /*
    public SimpleCarProperties(float theTimeSinceChange, float theDistanceTraveled, float thePreviousDistanceTraveled, int theIterationCount, State theState)
    {
        theCar
        timeSinceChange = theTimeSinceChange;
        distanceTraveled = theDistanceTraveled;
        sideDistanceTraveled = 0.0f;
        avoidanceDistanceTraveled = 0.0f;
        previousDistanceTraveled = thePreviousDistanceTraveled;
        iterationCount = theIterationCount;
        mainState = theState;
    }
    */

    public SimpleCarProperties(SimpleCarBehavior theCar)
    {
        timeSinceChange = theCar.timeSinceChange;
        distanceTraveled = theCar.distanceTraveled;
        sideDistanceTraveled = theCar.sideDistanceTraveled;
        avoidanceDistanceTraveled = theCar.avoidanceDistanceTraveled;
        previousDistanceTraveled = theCar.previousDistanceTraveled;
        iterationCount = theCar.iterationCount;
        mainState = theCar.mainState;
    }
}

public class SimpleCarBehavior : MonoBehaviour
{

    public float moveSpeed = 100.0f;
    public float timeSinceChange = 0.0f;

    //public float[] distanceTraveled = new float[5];
    public float distanceTraveled = 0.0f;
    public float sideDistanceTraveled = 0.0f;
    public float previousDistanceTraveled = 0.0f;
    public float avoidanceDistanceTraveled = 0.0f;
    public int iterationCount = 0;
    public System.Collections.Generic.List<SimpleCarProperties> SimpleCarPreservation = new System.Collections.Generic.List<SimpleCarProperties>();
    const float AMINIMUMDISTANCE = 50.0f;
    const float AMINIMUMTIME = 0.2f;
    public State mainState;
    public State previousState;
    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
        timeSinceChange += Time.deltaTime;

        ForwardPulse();
        switch (mainState)
        {
            case State.Base:
                ForwardPulse();
                distanceTraveled += rigidbody.velocity.magnitude;
                break;

            case State.GoForward:
                ForwardPulse();
                distanceTraveled += rigidbody.velocity.magnitude;
                if (distanceTraveled >= previousDistanceTraveled)
                {
                
                    EndAvoidance();
                }
                break;
            
            case State.Avoidance:
                ForwardPulse();
                avoidanceDistanceTraveled += rigidbody.velocity.magnitude;
                distanceTraveled += rigidbody.velocity.magnitude;
                if (distanceTraveled >= AMINIMUMDISTANCE /*&& distanceTraveled < previousDistanceTraveled*/)
                {
                    print("Going from Avoidance to Side.");
                    TurnLeft();
                distanceTraveled = 0;
                    mainState = State.Side;
                    //EndAvoidance();
                }
                break;
            
            case State.Side:
                ForwardPulse();
                //distanceTraveled += rigidbody.velocity.magnitude;
                sideDistanceTraveled += rigidbody.velocity.magnitude;
                if (sideDistanceTraveled >= AMINIMUMDISTANCE) //We should try to get pass our expected minimum side length. A collision here just means backup, turn right (avoidance again) forward, and then back here.
                {
                    print("Going from Side to Return.");
                    TurnLeft();
                    mainState = State.Return;
                }
                break;
            
            case State.Return:
                ForwardPulse();
                distanceTraveled += rigidbody.velocity.magnitude;
                if (distanceTraveled >= avoidanceDistanceTraveled)
                {
                    print("Successfully completed return trip.");
                    TurnRight();
                    EndAvoidance();
                }
                break;

            case State.BackUp:
                BackwardPulse();
                distanceTraveled -= rigidbody.velocity.magnitude;
                if (sideDistanceTraveled > 0) {sideDistanceTraveled -= rigidbody.velocity.magnitude;}
                if (timeSinceChange >= AMINIMUMTIME)
                {
                    timeSinceChange = 0;
                    print("Reversal/Backup Complete."); //Two Beeps/lights. Pause.
                    mainState = previousState;
                    print("Previous State Restored"); //Three Beeps/lights. Pause.
                    handleCollision();
                }
                break;
            default: 
            rigidbody.velocity = Vector3.zero;
                break;
        }

        //Back up slightly. (Make sure to deduct from distance traveled (beforeAvoidance) & store how much we backed up.)

        //AvoidObstacle by turning to the right, going (a minimum distance) turn left, and go forward (store this distance traveled duringAvoidance)

        //If we go forward farther than we backed up (maybe with some fudge amount) - beep

        //We're at the side of the obstacle. Go forward (a minimum distance), turn left, and go forward (distanceTraveledDuringAvoidance) while tracking distance (subtracting).
                    
        //If we hit a wall now, do the whole Avoidance thing kinda recursively.
            
        //ELSE If we made another collision before (distance traveled duringAvoidance) hit zero...
                
        //Do another level of the avoidance thing.
    
    }

    void OnCollisionEnter()
    {
        /*--------------------------------------------------------------------------------
        //This is pretty much the design for this thing. Somewhere between HLD and LLD, - Moore
        //Back up slightly. (Make sure to deduct from distance traveled (beforeAvoidance) & store how much we backed up.)

        //AvoidObstacle by turning to the right, going (a minimum distance) turn left, and go forward (store this distance traveled duringAvoidance)

            //If we go forward farther than we backed up (maybe with some fudge amount) - beep

                //We're at the side of the obstacle. Go forward (a minimum distance), turn left, and go forward (distanceTraveledDuringAvoidance) while tracking distance (subtracting).
                    
                    //If we hit a wall now, do the whole Avoidance thing kinda recursively.
            
            //ELSE If we made another collision before (distance traveled duringAvoidance) hit zero...
                
                //Do another level of the avoidance thing.
        --------------------------------------------------------------------------------*/
        if (mainState != State.BackUp)
        {
        timeSinceChange = 0;
        previousState = mainState;
            mainState = State.BackUp;}
        else
        {
            //Do a thing? Maybe?
        }
    }

    void handleCollision()
    {
        //Back up slightly. (Make sure to deduct from distance traveled (beforeAvoidance) & store how much we backed up.)

        //AvoidObstacle by turning to the right, going (a minimum distance) turn left, and go forward (store this distance traveled duringAvoidance)

        //If we go forward farther than we backed up (maybe with some fudge amount) - beep

        //We're at the side of the obstacle. Go forward (a minimum distance), turn left, and go forward (distanceTraveledDuringAvoidance) while tracking distance (subtracting).
                    
        //If we hit a wall now, do the whole Avoidance thing kinda recursively.
            
        //ELSE If we made another collision before (distance traveled duringAvoidance) hit zero...
                
        //Do another level of the avoidance thing.
        if (mainState == State.Base)
        {
            //Normally, copypasta is bad, but I'm gonna do it right now because reasons.
            //Pretty much, this means we need to save our state, switch to a new set of behaviors, and then return to this one afterwards.
            print("Going from Base to Avoidance.");
            SimpleCarProperties temp = new SimpleCarProperties(this); //CAREFUL, replaced distanceTraveled with the minimum for a quick recovery.
            temp.distanceTraveled = AMINIMUMDISTANCE * 2;
            SimpleCarPreservation.Insert(iterationCount, temp);


            iterationCount = iterationCount + 1;

            timeSinceChange = 0;
            distanceTraveled = 0;

            TurnRight();
            mainState = State.Avoidance;
        } else if (mainState == State.Avoidance)
        {
            print("Hit a wall during initial dodge.");
            //DO SOMETHING!!?

            //This is the part where we start a new phase of dodging.
            TurnRight();
            AvoidanceRoutine();

        } else if (mainState == State.Side)
        {
            print("Wall is still here! From Side redoing Avoidance.");
            TurnRight();
            mainState = State.Avoidance;
        } else if (mainState == State.Return)
        {
            //print("Hit wall during return trip! Avoiding...");
            print("Hit wall during return trip! From Return redoing Side.");
            TurnRight();
            sideDistanceTraveled = 0;
            mainState = State.Side;
            //AvoidanceRoutine();
        } else if (mainState == State.BackUp)
        {
            print("Car collided when backing up?");
            mainState = State.BAD;
        }

        else
        {
            print("Fix it later");
        }

        /*
        else if (distanceTraveled < AMINIMUMDISTANCE)
        {
            AvoidanceRoutine();

        } 

        else if (distanceTraveled < previousDistanceTraveled)
        {

        }
        */
    }

    public void AvoidanceRoutine()
    {
        print("Doing Avoidance Routine.");
        float distanceTraveledDuringAvoidance = 0.0f;
        //Pretty much, this means we need to save our state, switch to a new set of behaviors, and then return to this one afterwards.

        SimpleCarProperties temp = new SimpleCarProperties(this); 
        SimpleCarPreservation.Insert(iterationCount, temp);


        iterationCount = iterationCount + 1;

        timeSinceChange = 0;
        distanceTraveled = 0;
        avoidanceDistanceTraveled = 0;
        previousDistanceTraveled = temp.distanceTraveled;
        mainState = State.Avoidance;

    }

    public void EndAvoidance() //Restores the values before last change, then decrements and gets rid of the stored state.
    {
        print("Ending Avoidance Routine.");
        iterationCount = iterationCount - 1;
        SimpleCarProperties temp = SimpleCarPreservation [iterationCount];

        timeSinceChange = temp.timeSinceChange;
        distanceTraveled = temp.distanceTraveled;
        avoidanceDistanceTraveled = temp.avoidanceDistanceTraveled;
        sideDistanceTraveled = temp.sideDistanceTraveled;

        previousDistanceTraveled = temp.distanceTraveled;
        //iterationCount = temp.iterationCount;
        mainState = temp.mainState;


        SimpleCarPreservation.RemoveAt(iterationCount);
            
    }

    public void ForwardPulse()
    {
        rigidbody.velocity = moveSpeed * transform.forward * Time.deltaTime;
    }

    public void BackwardPulse()
    {
        rigidbody.velocity = -moveSpeed * transform.forward * Time.deltaTime;
    }

    public void TurnLeft()
    {
        transform.Rotate(new Vector3(0.0f, -90.0f, 0.0f));
        print("Turning Left");
    }

    public void TurnRight()
    {
        //transform.Rotate(Vector3.right * 90);
        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
        print("Turning Right");
    }
}
