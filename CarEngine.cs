using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    [Header("General")]
    public Transform path;
    public Transform path1;
    public Transform path2;
    public Transform path3;
    public float MaxWheelAngle = 45f;
    public float turnSpeed = 25f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    private float distanceToNextnode;
    public bool braking = false;
    public float MotorTorque = 50f;
    public float BrakeTorque = 150f;
    public float currentSpeed = 0f;
    public float maxSpeed; //m/s
    private float mass = 1300; //kg
    private Rigidbody rb;
    public int ChoosePath = 0;

    [Header("ElectricCar")]
    public GameObject Identifier;
    public float EVpercentage;
    private float chance = 0;
    public bool ElectricCar = false;
    private float MaxBattery = 200000000f;
    public float BatteryCharge = 0;
    private float CurrentBattery = 0;
    private float BatteryLimit = 0;
    private float delay = 1f;
    private float ActionTime = 0.0f;
    private float ChargingLaneEnergy = 0;
    private float VehicleEfficiency = 0;
    public bool recharge = false;
    public Texture2D TextureIdentifier;
    public Texture2D TextureRecharge;
    public Texture2D TextureCL;
    public Renderer rend;
    public bool OnStation = false;
    private Transform Station;
    public float ChargingFactor = 0.1f;
    public bool CL = false;
    public float ChargingLanePower;
    public float ChargingLaneEnergyResults = 0;

    [Header("Sensors")]
    public float SensorObjectsLength = 1f;
    public float SensorTrafficLength = 0f;
    public float sensorVelAngLength = 0.5f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.05f, 0.2f);
    public float frontsideSensorPosition = 0.1f;
    public float Sensor_Angle = 45;
    private bool avoiding = false;
    private float targetSteerAngle = 0;
    private float VehiclesDetectionfactor = 0;

    private List<Transform> nodes;
    private int currentNode = 0;

    private string EVPercentageSaved = "EVPercentage";
    private string CLPowerSaved = "CLPower";
    private string MaxSpeedSaved = "MaxSpeed";

    //SaveSimulationData:

    private string ChargingLaneEnergyResultsSaved = "ChargingLaneEnergyResults";
    private int CarNumber = 0;
    private string CarNumberSaved = "CarNumber";
    private int RealEVPercentage = 0;
    private string RealEVPercentageSaved = "RealEVPercentage";



    //______________________________________________MAIN_______________________________________________//

    void Awake()
    {
        LoadMenuData();     //Load the values from the User's settings in menu
    }

    void Start()
    {
        ChooseElectric();   //Choose if the vehicle is electric or not
        Path();             //Choose the path
    }

    private void FixedUpdate()
    {
        if (OnStation)
        {
            Recharge();     //Recharge the battery
        }
        else
        {
            ApplySteer();
            Sensors();      //Raycast settings for distance sensors
            Drive();
            CheckNodes();
            LerpToSteerAngle();
            Braking();
            if (ElectricCar)
            {
                ChargingLane();
                if (Time.time > ActionTime)
                {
                    Electric();
                    ActionTime = Time.time + delay;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Traffic" || collider.gameObject.tag == "Objects")
        {
            Destroy();
        }
    }





                 //______________________________________________FUNCTIONS______________________________________________//

    private void ChargingLane()
    {
        RaycastHit hit;
        Vector3 SensorStartPosition = transform.position;
        SensorStartPosition += transform.up * frontSensorPosition.y;
        if (Physics.Raycast(SensorStartPosition, Quaternion.AngleAxis(Sensor_Angle, transform.right) * transform.forward, out hit, SensorObjectsLength))
        {
            if (hit.collider.CompareTag("ChargingLane"))
            {
                CL = true;
            }
            else
            {
                CL = false;
            }
        }
    }

    private void Recharge()
    {
        float TopCharge;
        Renderer[] rendCar = GetComponentsInChildren<Renderer>();
        Collider[] collCar = GetComponentsInChildren<Collider>();
        Rigidbody[] rbCar = GetComponentsInChildren<Rigidbody>();
        TopCharge = Random.Range(MaxBattery * 3 / 4, MaxBattery);
        if (CurrentBattery < TopCharge)
        {
            CurrentBattery = CurrentBattery + (ChargingFactor * MaxBattery / 100f);
            BatteryCharge = CurrentBattery * 100 / MaxBattery;
            this.transform.position = Station.transform.position;
            this.transform.rotation = Station.transform.rotation;
            foreach (var r in rendCar)
            {
                r.enabled = false;
            }
            foreach (var s in collCar)
            {
                s.enabled = false;
            }
            foreach (var t in rbCar)
            {
                t.useGravity = false;
            }
            wheelRL.brakeTorque = BrakeTorque;
            wheelRR.brakeTorque = BrakeTorque;
        }
        else
        {
            foreach (var r in rendCar)
            {
                r.enabled = true;
            }
            foreach (var s in collCar)
            {
                s.enabled = true;
            }
            foreach (var t in rbCar)
            {
                t.useGravity = true;
            }
            OnStation = false;
        }

    }

    private void Electric()
    {
        StartCoroutine(waiter());
    }
    IEnumerator waiter()
    {
        if (CL)
        {
            ChargingLaneEnergy = 1;
        }
        else
        {
            ChargingLaneEnergy = 0;
        }
        BatteryCharge = CurrentBattery * 100 / MaxBattery;
        ChargingLaneEnergyResults = ChargingLaneEnergyResults + (ChargingLaneEnergy * ChargingLanePower * 0.225f);

        if (braking)
        {
            CurrentBattery = CurrentBattery + (0.35f * mass * currentSpeed * currentSpeed) - VehicleEfficiency * (0.098f * mass * currentSpeed + 0.33f * currentSpeed * currentSpeed * currentSpeed) + (ChargingLaneEnergy * ChargingLanePower * 0.225f);
        }
        else
        {
            CurrentBattery = CurrentBattery - VehicleEfficiency * (0.5f * mass * currentSpeed * currentSpeed + 0.098f * mass * currentSpeed + 0.33f * currentSpeed * currentSpeed * currentSpeed) + (ChargingLaneEnergy * ChargingLanePower * 0.225f);
        }
        yield return new WaitForSeconds(delay);

        if (CurrentBattery < BatteryLimit)
        {
  //          recharge = true;/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            rend.material.mainTexture = TextureRecharge;
        }
        else
        {
            recharge = false;
            if (CL)
            {
                rend.material.mainTexture = TextureCL;
            }
            else
            {
                rend.material.mainTexture = TextureIdentifier;
            }
        }
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 SensorStartPosition = transform.position;
        SensorStartPosition += transform.forward * frontSensorPosition.z;
        SensorStartPosition += transform.up * frontSensorPosition.y;
        float avoidMultipler = 0;
        avoiding = false;
        braking = false;
        VehiclesDetectionfactor = MotorTorque / 33;

        if (currentSpeed > 10f)
        {
            SensorTrafficLength = currentSpeed / 60;
        }
        else
        {
            SensorTrafficLength = 0.5f;
        }


        //Sensor delantero central
        if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, SensorObjectsLength))
        {
            if (hit.collider.CompareTag("Objects"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.red);
                avoiding = true;
            }
        }
        if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, VehiclesDetectionfactor * SensorTrafficLength))
        {
            if (hit.collider.CompareTag("Traffic"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.blue);
                braking = true;
            }
        }

        //Sensor delantero derecho
        SensorStartPosition += transform.right * frontsideSensorPosition;
        if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, SensorObjectsLength))
        {
            if (hit.collider.CompareTag("Objects"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.red);
                avoiding = true;
                avoidMultipler -= 1f;
            }
        }
        if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, VehiclesDetectionfactor * SensorTrafficLength))
        {
            if (hit.collider.CompareTag("Traffic"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.blue);
                braking = true;
            }
        }

        //Sensor angular derecho
        else if (Physics.Raycast(SensorStartPosition, Quaternion.AngleAxis(Sensor_Angle, transform.up) * transform.forward, out hit, SensorObjectsLength))
        {
            if (hit.collider.CompareTag("Objects"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.red);
                avoiding = true;
                avoidMultipler -= 0.5f;
            }
            else if (hit.collider.CompareTag("Traffic"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.blue);
                avoiding = true;
                avoidMultipler -= 0.5f;
            }
        }

        //Sensor delantero izquierdo
        SensorStartPosition -= transform.right * 2 * frontsideSensorPosition;
        if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, SensorObjectsLength))
        {
            if (hit.collider.CompareTag("Objects"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.red);
                avoiding = true;
                avoidMultipler += 1f;
            }
        }
        if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, VehiclesDetectionfactor * SensorTrafficLength))
        {
            if (hit.collider.CompareTag("Traffic"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.blue);
                braking = true;
            }
        }

        //Sensor angular izquierdo
        else if (Physics.Raycast(SensorStartPosition, Quaternion.AngleAxis(-Sensor_Angle, transform.up) * transform.forward, out hit, SensorObjectsLength))
        {
            if (hit.collider.CompareTag("Objects"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.red);
                avoiding = true;
                avoidMultipler += 0.5f;
            }
            else if (hit.collider.CompareTag("Traffic"))
            {
                Debug.DrawLine(SensorStartPosition, hit.point, Color.blue);
                avoiding = true;
                avoidMultipler += 0.5f;
            }
        }

        //AVOID
        if (avoidMultipler == 0)
        {
            if (Physics.Raycast(SensorStartPosition, transform.forward, out hit, SensorObjectsLength))
            {
                if (hit.collider.CompareTag("Objects"))
                {
                    Debug.DrawLine(SensorStartPosition, hit.point, Color.red);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultipler = -1;
                    }
                    else
                    {
                        avoidMultipler = 1;
                    }
                }
            }
        }

        if (avoiding)
        {
            targetSteerAngle = MaxWheelAngle * avoidMultipler;
        }

    }

    private void ApplySteer()
    {
        if (ElectricCar && recharge)
        {
            float distanceToClosestStation = Mathf.Infinity;
            ChargingStation closestStation = null;
            ChargingStation[] allStations = GameObject.FindObjectsOfType<ChargingStation>();
            foreach (ChargingStation CurrentStation in allStations)
            {
                float distanceToStation = (CurrentStation.transform.position - this.transform.position).sqrMagnitude;
                if (distanceToStation < distanceToClosestStation)
                {
                    distanceToClosestStation = distanceToStation;
                    closestStation = CurrentStation;
                    if (distanceToClosestStation < 2)
                    {
                        Station = closestStation.transform;
                        OnStation = true;
                    }
                }
            }
            Debug.DrawLine(this.transform.position, closestStation.transform.position, Color.green);

            if (avoiding) return;
            Vector3 RelVector = transform.InverseTransformPoint(closestStation.transform.position);
            float NewAngle = (RelVector.x / RelVector.magnitude) * MaxWheelAngle;
            targetSteerAngle = NewAngle;
        }
        else
        {
            if (avoiding) return;
            Vector3 RelVector = transform.InverseTransformPoint(nodes[currentNode].position);
            float NewAngle = (RelVector.x / RelVector.magnitude) * MaxWheelAngle;
            targetSteerAngle = NewAngle;
        }

    }
    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm / 60; //m/s
        if (currentSpeed < maxSpeed && !braking)
        {
            wheelFL.motorTorque = MotorTorque;
            wheelFR.motorTorque = MotorTorque;
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }
    private void CheckNodes()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < distanceToNextnode)
        {
            distanceToNextnode = Vector3.Distance(transform.position, nodes[currentNode].position);
        }
        else if (Vector3.Distance(transform.position, nodes[currentNode].position) > distanceToNextnode + 1f)
        {
            Destroy();
        }

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            distanceToNextnode = Mathf.Infinity;
            currentNode++;
        }
        if (currentNode == nodes.Count)
        {
            Destroy();
        }
    }
    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
    private void Braking()
    {
        if (braking)
        {
            wheelRL.brakeTorque = BrakeTorque;
            wheelRR.brakeTorque = BrakeTorque;
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    public void Path()
    {
        distanceToNextnode = Mathf.Infinity;
        ChoosePath = Random.Range(1, 4);

        if (ChoosePath == 1)
        {
            path = path1;
        }
        else if (ChoosePath == 2)
        {
            path = path2;
        }
        else if (ChoosePath == 3)
        {
            path = path3;
        }

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
    }

    public void ChooseElectric()
    {
        chance = Random.Range(0, 100);
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;

        if (chance < EVpercentage)
        {
            ElectricCar = true;
            CurrentBattery = Random.Range(MaxBattery / 3, MaxBattery);
            BatteryLimit = MaxBattery / 4;
            VehicleEfficiency = Random.Range(0.85f, 0.95f);
            Identifier.SetActive(true);
        }
    }

    public void Destroy()
    {   
        SaveSimulationData();
        Destroy(this.gameObject);
    }

    public void SaveSimulationData()
    {
        if (ChargingLaneEnergyResults > 1.0f)
        {
            PlayerPrefs.SetFloat(ChargingLaneEnergyResultsSaved, ChargingLaneEnergyResults);
        }
        CarNumber=Random.Range(0, 1000);
        PlayerPrefs.SetInt(CarNumberSaved, CarNumber);
        if (ElectricCar)
        {
            RealEVPercentage = Random.Range(0, 1000);
            PlayerPrefs.SetInt(RealEVPercentageSaved, RealEVPercentage);
        }

    }

    public void LoadMenuData()
    {
        EVpercentage = PlayerPrefs.GetFloat(EVPercentageSaved, 50);
        ChargingLanePower = PlayerPrefs.GetFloat(CLPowerSaved, 0);
        maxSpeed = PlayerPrefs.GetFloat(MaxSpeedSaved, 60);
    }
}
