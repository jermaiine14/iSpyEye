using System.IO.Ports;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    private bool[] previousButtonStates = new bool[5]; // Tracks previous states
    public string portName = "COM4"; // Change this to your Arduino's port name
    public int baudRate = 9600; // Change this to your Arduino's baud rate
    private SerialPort serialPort;
    public ScrollingBackground2 SB2;

    public ObjectSpawner objectSpawner; // Reference to the ObjectSpawner script

    private int numberOfButtons = 5;
    private string[] buttonStates = new string[5]; // Array to hold button states
    // Start is called before the first frame update
    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open(); // Open the serial port
        serialPort.ReadTimeout = 5000; // Set a read timeout to avoid blocking
    }

    // Update is called once per frame
    void Update()
    {
        if(serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine(); // Read a line from the serial port
                buttonStates = data.Split(',');
                HandleInput(buttonStates); // Handle the input from the Arduino
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
            }
        }
    }

   void HandleInput(string[] states)
{
    if (SB2.ObjectSpawned == false){
    for (int i = 0; i < numberOfButtons; i++)
    {
        bool isPressed = states[i] == "1";
        bool wasPressed = previousButtonStates[i];

        //press logic
        if (isPressed && !wasPressed)
        {
            switch (i)
            {
                case 0:
                    objectSpawner.SpawnKoe();
                    break;
                case 1:
                    objectSpawner.SpawnCloud(); // Initial spawn
                    break;
                case 2:
                    objectSpawner.SpawnFlower();
                    break;
                case 3:
                    objectSpawner.SpawnMolen();
                    break;
                case 4:
                    objectSpawner.SpawnTree();
                    break;
            }
        }

        // holding it causes it to grow
        if (isPressed && i < 5)
        {
            switch (i)
            {
                case 0:
                    objectSpawner.SpawnKoe();
                    break;
                case 1:
                    objectSpawner.SpawnCloud(); // Initial spawn
                    break;
                case 2:
                    objectSpawner.SpawnFlower();
                    break;
                case 3:
                    objectSpawner.SpawnMolen();
                    break;
                case 4:
                    objectSpawner.SpawnTree();
                    break;
            }
        }

        // if released
        if (i < 5 && !isPressed && wasPressed)
        {
            objectSpawner.ResetState();
        }

        previousButtonStates[i] = isPressed; // save state
    }
}

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close(); // Close the serial port when the application quits
        }
    }
}
}
