using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    public TextMeshProUGUI EVPercentageTextField;
    public TextMeshProUGUI CLPowerTextField;
    public TextMeshProUGUI MaxSpeedTextField;
    public TextMeshProUGUI TrafficDensityTextField;
    public TextMeshProUGUI ChargingLaneSizeTextField;
    public TextMeshProUGUI GlobalChargingLaneResultsTextField;
    public TextMeshProUGUI GlobalCarNumberTextField;
    public TextMeshProUGUI GlobalRealEVPercentageTextField;
    public TextMeshProUGUI GlobalChargingLaneLengthTextField;
    public TextMeshProUGUI SimulationTimeTextField;
    public TextMeshProUGUI CLPriceInputTextField;
    public TextMeshProUGUI TotalCostTextField;

    public float GlobalChargingLaneResults = 0;
    public int GlobalCarNumber = 0;
    public float GlobalRealEVPercentage = 0;
    public int GlobalChargingLaneLength = 0;
    public float Seconds = 0;
    public int Minutes = 0;

    public float EVPercentage = 0;
    public float CLPower = 0;
    public float maxSpeed = 0;
    public float TrafficDensity = 0;
    public int ChargingLaneSize = 0;

    private string GlobalChargingLaneResultsSaved = "GlobalChargingLaneResults";
    private string GlobalCarNumberSaved = "GlobalCarNumber";
    private string GlobalRealEVPercentageSaved = "GlobalRealEVPercentage";
    private string GlobalChargingLaneLengthSaved = "GlobalChargingLaneLength";
    private string SecondsSaved = "Seconds";
    private string MinutesSaved = "Minutes";

    private string EVPercentageSaved = "EVPercentage";
    private string CLPowerSaved = "CLPower";
    private string MaxSpeedSaved = "MaxSpeed";
    private string TrafficDensitySaved = "TrafficDensity";
    private string ChargingLaneSizeSaved = "ChargingLaneSize";

    private string TrafficDensityString;

    public Slider CLPriceSlider;
    public float CLPriceSliderFactor;
    public float CLPrice;
    public float CLTotalPrice;


    void Start()
    {
        LoadSimulationData();
        SimulationTime();
        Operations();
    }

    public void Update()
    {
        TotalCost();
        ResultsTexts2();
    }


    public void ResultsTexts2()
    {
        EVPercentageTextField.text =  System.Math.Round(EVPercentage, 2) + "%.";
        CLPowerTextField.text =  System.Math.Round(CLPower/1000, 2) + "kW.";
        MaxSpeedTextField.text =  System.Math.Round(maxSpeed, 2) + "km/h.";
        TrafficDensityTextField.text =  TrafficDensityString + ".";
        ChargingLaneSizeTextField.text =  ChargingLaneSize + ".";
        GlobalChargingLaneResultsTextField.text =  System.Math.Round(GlobalChargingLaneResults, 2) + "kWh.";
        GlobalCarNumberTextField.text =  GlobalCarNumber + " vehículos.";
        GlobalRealEVPercentageTextField.text =  System.Math.Round(GlobalRealEVPercentage, 2) + "%.";
        GlobalChargingLaneLengthTextField.text =  GlobalChargingLaneLength + "m.";
        SimulationTimeTextField.text = Minutes + "min " + System.Math.Round(Seconds, 0) + "s.";
    }

    public void TotalCost()
    {
        CLPriceInputTextField.text = System.Math.Round(CLPrice, 0) + ".";
        CLTotalPrice = (int)System.Math.Round(CLPrice, 0) * GlobalChargingLaneLength;
        TotalCostTextField.text = CLTotalPrice + "$.";
    }

    public void Operations()
    {
        if (TrafficDensity == 3)
        {
            TrafficDensityString = "Baja";
        }
        if (TrafficDensity == 2.5)
        {
            TrafficDensityString = "Media";
        }
        if (TrafficDensity == 2)
        {
            TrafficDensityString = "Alta";
        }

        GlobalChargingLaneResults = GlobalChargingLaneResults / (1000 * 3600); // J -> kWh
    }

    public void CLPriceValue(float Value)
    {
        CLPriceSliderFactor = Value;
        CLPrice = CLPriceSliderFactor * 900 + 100f;
    }

    private void SimulationTime()
    {
        if (Seconds < 10 && Minutes > 0)
        {
            Seconds = 50.0f + Seconds;
            Minutes = Minutes - 1;
        }
        else
        {
            Seconds = Seconds - 10.0f;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadSimulationData()
    {
        GlobalChargingLaneResults = PlayerPrefs.GetFloat(GlobalChargingLaneResultsSaved, 0);
        GlobalCarNumber = PlayerPrefs.GetInt(GlobalCarNumberSaved);
        GlobalRealEVPercentage = PlayerPrefs.GetFloat(GlobalRealEVPercentageSaved, 0);
        GlobalChargingLaneLength = PlayerPrefs.GetInt(GlobalChargingLaneLengthSaved);
        Seconds = PlayerPrefs.GetFloat(SecondsSaved);
        Minutes = PlayerPrefs.GetInt(MinutesSaved);

        EVPercentage = PlayerPrefs.GetFloat(EVPercentageSaved);
        CLPower = PlayerPrefs.GetFloat(CLPowerSaved);
        maxSpeed = PlayerPrefs.GetFloat(MaxSpeedSaved, 40);
        TrafficDensity = PlayerPrefs.GetFloat(TrafficDensitySaved);
        ChargingLaneSize = PlayerPrefs.GetInt(ChargingLaneSizeSaved);
    }
}
