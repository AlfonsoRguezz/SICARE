using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    private GameObject Prefab;
    public GameObject Vehicle1;
    public GameObject Vehicle2;
    public GameObject Vehicle3;
    public GameObject Vehicle4;
    public GameObject Vehicle5;
    public GameObject Vehicle6;
    public GameObject Vehicle7;
    public GameObject Vehicle8;
    public GameObject Vehicle9;
    public GameObject Vehicle10;
    public GameObject Vehicle11;
    public GameObject Vehicle12;

    public float delay;

    public CarEngine script;
    public float timeToSpawn;
    private float currentTimeToSpawn;
    public Transform path1;
    public Transform path2;
    public Transform path3;
    private List<GameObject> Vehicles;
    int i = 0;
    private string TrafficDensitySaved = "TrafficDensity";

    void Awake()
    {
        LoadMenuData();
    }

    void Start()
    {
        Vehicles = new List<GameObject>();

        Vehicles.Add(Vehicle1);
        Vehicles.Add(Vehicle2);
        Vehicles.Add(Vehicle3);
        Vehicles.Add(Vehicle4);
        Vehicles.Add(Vehicle5);
        Vehicles.Add(Vehicle6);
        Vehicles.Add(Vehicle7);
        Vehicles.Add(Vehicle8);
        Vehicles.Add(Vehicle9);
        Vehicles.Add(Vehicle10);
        Vehicles.Add(Vehicle11);
        Vehicles.Add(Vehicle12);
    }

    void Update()
    {
        if (currentTimeToSpawn > 0)
        {
            currentTimeToSpawn -= Time.deltaTime;
        }
        else
        {
            Spawn();
            currentTimeToSpawn = timeToSpawn;
        }

    }
    public void Spawn()
    {

        i = Random.Range(0, Vehicles.Count);
        Prefab = Vehicles[i];
        script = Prefab.GetComponent<CarEngine>();
        script.path1 = path1;
        script.path2 = path2;
        script.path3 = path3;

        Instantiate(Prefab, transform.position, transform.rotation);
    }

    public void LoadMenuData()
    {
        timeToSpawn = PlayerPrefs.GetFloat(TrafficDensitySaved, 2);
        timeToSpawn = timeToSpawn + delay;
    }
}