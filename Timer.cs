using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Header("Time")]
    public Text TimeTextField;
    public Text TenSecondsTextField;
    public float Seconds = 0f;
    public int Minutes = 0;

    [Header("Simulation Data")]
    private float ChargingLaneEnergyResults = 0;
    private float ChargingLaneEnergyPrevious = 0;
    public float ChargingLaneEnergyGlobal = 0;
    private string ChargingLaneEnergyResultsSaved = "ChargingLaneEnergyResults";
    private string GlobalChargingLaneResultsSaved = "GlobalChargingLaneResults";

    public int CarNumberGlobal = 0;
    private int CarNumber = 0;
    private int CarNumberPrevious = 0;
    private string CarNumberSaved = "CarNumber";
    private string GlobalCarNumberSaved = "GlobalCarNumber";

    private int RealEVPercentage = 0;
    public float RealEVPercentageGlobal = 0;
    private int RealEVPercentagePrevious = 0;
    private string RealEVPercentageSaved = "RealEVPercentage";
    private string GlobalRealEVPercentageSaved = "GlobalRealEVPercentage";
    public float ElectricCars = 0;

    public int GlobalChargingLaneSize = 0;
    private int ChargingLaneSize=0;
    private string ChargingLaneSizeSaved = "ChargingLaneSize";
    public GameObject ChargingLanes;
    private string GlobalChargingLaneLengthSaved = "GlobalChargingLaneLength";

    private string SecondsSaved = "Seconds";
    private string MinutesSaved = "Minutes";

    [Header("Cameras")]
    public GameObject Cameras;
    private int i = 0;

    void Start()
    {
        LoadSimulationData();
        Seconds = 0;
        Minutes = 0;
        TenSecondsTextField.text = "(El estudio de las variables comenzará a los 10 segundos)";

        Transform[] ChargingLanesArray = ChargingLanes.GetComponentsInChildren<Transform>();
        foreach (var r in ChargingLanesArray)
        {
            GlobalChargingLaneSize = GlobalChargingLaneSize + (int)r.localScale[0]*2;
        }
        GlobalChargingLaneSize = (GlobalChargingLaneSize - 1) * ChargingLaneSize;
    }

    public void Update()
    {
        Seconds = Time.timeSinceLevelLoad - Minutes * 60;
        TimeTextField.text = "Tiempo de simulación: " + Minutes + "min, " + System.Math.Round(Seconds, 0) + "s.";
        if (Seconds > 60.0f)
        {
            Seconds = 0;
            Minutes = Minutes + 1;
        }
        if (Seconds < 10.0f && Minutes == 0)
        {
            TenSecondsTextField.text = "(El estudio de las variables comenzará a los 10 segundos)";
        }

        else
        {
            TenSecondsTextField.text = "";
            LoadSimulationData();
            SumUpResults();
        }
    }
    
    public void CameraMovement()
    {
        Camera[] CamerasArray = Cameras.GetComponentsInChildren<Camera>();

        if (i == CamerasArray.Length - 1)
        {
            i = 1;
        }
        else
        {
            i = i + 1;
        }
        CamerasArray[i - 1].enabled = false;
        CamerasArray[i].enabled = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void EndSimulation()
    {
        SaveSimulationData();
        SceneManager.LoadScene(4);
    }

    private void LoadSimulationData()
    {
        ChargingLaneEnergyResults = PlayerPrefs.GetFloat(ChargingLaneEnergyResultsSaved); //J
        CarNumber = PlayerPrefs.GetInt(CarNumberSaved);
        RealEVPercentage = PlayerPrefs.GetInt(RealEVPercentageSaved);
        ChargingLaneSize = PlayerPrefs.GetInt(ChargingLaneSizeSaved);
    }
    
    private void SumUpResults()
    {
        if (ChargingLaneEnergyResults != ChargingLaneEnergyPrevious)
        {
            ChargingLaneEnergyGlobal = ChargingLaneEnergyGlobal + ChargingLaneEnergyResults;
            ChargingLaneEnergyPrevious = ChargingLaneEnergyResults;
        }

        if (CarNumber != CarNumberPrevious)
        {
            CarNumberGlobal = CarNumberGlobal + 1;
            CarNumberPrevious = CarNumber;
            if (RealEVPercentage != RealEVPercentagePrevious)
            {
                RealEVPercentagePrevious = RealEVPercentage;
                ElectricCars = ElectricCars + 1f;
            }
            RealEVPercentageGlobal = (ElectricCars) * 100 / CarNumberGlobal;
        }
    }

    public void SaveSimulationData()
    {
        PlayerPrefs.SetFloat(GlobalChargingLaneResultsSaved, ChargingLaneEnergyGlobal);
        PlayerPrefs.SetInt(GlobalCarNumberSaved, CarNumberGlobal);
        PlayerPrefs.SetFloat(GlobalRealEVPercentageSaved, RealEVPercentageGlobal);
        PlayerPrefs.SetInt(GlobalChargingLaneLengthSaved, GlobalChargingLaneSize);
        PlayerPrefs.SetFloat(SecondsSaved, Seconds);
        PlayerPrefs.SetInt(MinutesSaved, Minutes);
    }
}

