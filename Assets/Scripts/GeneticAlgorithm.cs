 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject titanBase;

    public int numAgents = 10;
    public int numParameters = 3;

    [Header("SpeedSettings")]
    public float maxSpeed = 10;
    public float minSpeed = 2;

    [Header("SizeSettings")]
    public float maxSize = 15;
    public float minSize = 2;

    int numBestAgents;
    float[,] agents;
    float[,] bestAgents;
    float[] successRates;
    float[] bestSuccessRates;

    List<GameObject> agentObjects;

    void Start()
    {
        numBestAgents = (int) 0.1f * numAgents;
        agents = new float[numAgents, numParameters];
        bestAgents = new float[numAgents, numParameters];
        successRates = new float[numAgents];
        bestSuccessRates = new float[numBestAgents];
        agentObjects = new List<GameObject>();

        for (int i = 0; i < numAgents; i++)
        {
            float agentSpeed = Random.Range(minSpeed, maxSpeed);
            float agentSize = Random.Range(minSize, maxSize);

            agents[i, 0] = agentSpeed;
            agents[i, 1] = agentSize;
            successRates[i] = 0.0f;

            GameObject agent = Instantiate(titanBase);
            agent.name = "Agent #" + i;
            // agent.tag = "Titan";

            agent.AddComponent<Agent>();
            agent.GetComponent<Agent>().speed = agentSpeed;
            // agent.transform.localScale *= agentSize;
            agent.GetComponent<Agent>().health = agentSize;
            float offsetX = i * 10.0f;
            float offsetZ = Random.Range(0, 0);
            agent.transform.localPosition = new Vector3(offsetX, 2, offsetZ);
            agentObjects.Add(agent);
        }
    }

    void FindBestAgents()
    {
        int bestValue;
        float[,] tempAgents = new float[numAgents, numParameters];
        float[] tempSuccessRates = new float[numAgents];
        float swapHolder;

        for (int i = 0; i < numAgents; i++)
        {
            for (int j = 0; j < numParameters; j++)
            {
                tempAgents[i, j] = agents[i, j];
            }
            tempSuccessRates[i] = successRates[i];
        }

        for (int i = 0; i < numAgents-1; i++)
        {
            bestValue = i;
            for (int j = i + 1; j < numAgents; j++)
            {
                if (successRates[bestValue] < successRates[j])
                {
                    bestValue = j;
                }
            }
            for (int k = 0; k < numParameters; k++)
            {
                swapHolder = tempAgents[i, k];
                tempAgents[i, k] = tempAgents[bestValue, k];
                tempAgents[bestValue, k] = swapHolder;
            }
            swapHolder = tempSuccessRates[i];
            tempSuccessRates[i] = tempSuccessRates[bestValue];
            tempSuccessRates[bestValue] = swapHolder;
        }

        for (int i = 0; i < numBestAgents; i++)
        {
            for (int j = 0; j < numParameters; j++)
            {
                bestAgents[i, j] = tempAgents[i, j];
            }
            bestSuccessRates[i] = tempSuccessRates[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // hit-to-stop controls - should probably be moved to its own script
        // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.transform.gameObject.tag == "TitanArms")
                {
                    hitInfo.transform.gameObject.GetComponentInParent<Agent>().TakeDamage(0);
                }
                if (hitInfo.transform.gameObject.tag == "TitanBody")
                {
                    hitInfo.transform.gameObject.GetComponentInParent<Agent>().TakeDamage(1);
                }
                if (hitInfo.transform.gameObject.tag == "TitanLegs")
                {
                    hitInfo.transform.gameObject.GetComponentInParent<Agent>().TakeDamage(2);
                }
            }
        }


        for (int i = 0; i < numAgents; i++)
        {
            // Stop the agent at Vector3(x, y, -100)
            GameObject agent = agentObjects[i];
            Vector3 agentVel = new Vector3(0f, 0f, agent.GetComponent<Agent>().speed) * Time.deltaTime;
            //agent.transform.localPosition += (agentVel * Time.deltaTime);
            agent.transform.Translate(agentVel);
            if (agent.transform.localPosition.z > 100)
            {
                agent.GetComponent<Agent>().speed = 0f;
                agent.GetComponent<Agent>().goalReached = true;
            }

        }
        float velSum = 0;
        float successSum = 0;
        for (int i = 0; i < numAgents; i++)
        {
            GameObject agent = agentObjects[i];
            velSum += agent.GetComponent<Agent>().speed;
            // agent_vel[i].length();
            // instead of 1280, our max in Z-axis is 100
            successRates[i] = 100 - agent.transform.localPosition.z;
            successSum += successRates[i];
        }

        // When all the titans have been stopped
        if (velSum == 0)
        {
            FindBestAgents();

            float[] averageParameters = new float[numParameters];
            for (int i = 0; i < numParameters; i++)
            {
                averageParameters[i] = 0;
                for (int k = 0; k < numBestAgents; k++)
                {
                    averageParameters[i] += bestAgents[k, i];
                }
                averageParameters[i] = averageParameters[i] / numParameters;
            }




            for (int i = 0; i < numAgents; i++)
            {
                for (int j = 0; j < numParameters; j++)
                {
                    // mutation rate - random.range
                    agents[i, j] = averageParameters[j] + Random.Range(-5, 5);
                }

                // Setting MaxLimits and MinLimits
                Debug.Log("Before");
                Debug.Log(agents[i, 0]);
                Debug.Log(agents[i, 1]);
                if (agents[i, 0] > maxSpeed)
                {
                    agents[i, 0] = maxSpeed;
                }
                if (agents[i, 1] > maxSize)
                {
                    agents[i, 1] = maxSize;
                }
                if (agents[i, 0] < minSpeed)
                {
                    agents[i, 0] = maxSpeed;
                }
                if (agents[i, 1] < minSize)
                {
                    agents[i, 1] = minSize;
                }
                Debug.Log("After");
                Debug.Log(agents[i, 0]);
                Debug.Log(agents[i, 1]);
                /*
                if (agents[i, 2] < minSpeed)
                {
                    agents[i, 2] = maxSpeed;
                }
                if (agents[i, 2] < minSize)
                {
                    agents[i, 2] = minSize;
                }
                */

                // Resseting the simulation


                GameObject agent = agentObjects[i];
                float offsetX = i * 10.0f;
                float offsetZ = Random.Range(0, 0);
                agent.transform.localPosition = new Vector3(offsetX, 1, offsetZ);
                agent.GetComponent<Agent>().speed = agents[i, 0];
                agent.GetComponent<Agent>().health = agents[i, 1];
                agent.GetComponent<Agent>().UpdateAgent();
                successRates[i] = 0.0f;
            }

        }
    }
}
