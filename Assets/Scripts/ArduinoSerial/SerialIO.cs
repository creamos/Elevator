/* 
 * Version 0.4.0, 2018-04-13, Pierre Rossel
 * 
 * This component helps sending and receiving data from a serial port. 
 * It detects line breaks and notifies the attached gameObject of new lines as they arrive.
 * 
 * Usage 1: Receive data when you expect line breaks
 * -------
 * 
 * - drop this script to a gameObject
 * - select "Notify Lines" checkbox on Serial script
 * - create a script on the same gameObject to receive new line notifications
 * - add the OnSerialLine() function, here is an example
 * 
 * 	void OnSerialLine(string line) {
 *		Debug.Log ("Got a line: " + line);
 *	}
 * 
 * Usage 2: Receive data (when you don't expect line breaks)
 * -------
 * 
 * - drop this script to a gameObject
 * - from any script, use the static props ReceivedBytesCount, ReceivedBytes 
 *   and don't forget to call ClearReceivedBytes() to avoid overflowing the buffer
 * 
 * Usage 3: Send data
 * -------
 * 
 * - from any script, call the static functions Serial.Write() or Serial.WriteLn()
 * - if not not already, the serial port will be opened automatically.
 * 
 * Configuration
 * -------------
 * 
 * Drop the SerialConfig component to an empty GameObject in your scene and configure:
 * 
 * - the preferred ports
 * - the speed
 * - whether you wand debug informations in console
 * 
 * 
 * Troubleshooting
 * ---------------
 * 
 * You may get the following error:
 *     error CS0234: The type or namespace name `Ports' does not exist in the namespace `System.IO'. 
 *     Are you missing an assembly reference?
 * Solution: 
 *     Menu Edit | Project Settings | Player | Other Settings | API Compatibility Level: .Net 2.0
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// If you get error CS0234 on this line, see troubleshooting section above
using System.IO.Ports;

namespace ArduinoSerial
{
    public class SerialIO : MonoBehaviour
    {

        /// <summary>
        /// Enable notification of data as it arrives
        /// Sends OnSerialData(string data) message
        /// </summary>
        public bool NotifyData = false;

        /// <summary>
        /// Discard all received data until first line.
        /// Do not enable if you do not expect a \n as 
        /// this would prevent the notification of any line or value.
        /// Data notification is not impacted by this parameter.
        /// </summary>
        public bool SkipFirstLine = false;

        /// <summary>
        /// Enable line detection and notification on received data.
        /// Message OnSerialLine(string line) is sent for every received line
        /// </summary>
        public bool NotifyLines = false;

        /// <summary>
        /// Maximum number of lines to remember. Get them with GetLines() or GetLastLine()
        /// </summary>
        public int RememberLines = 0;

        /// <summary>
        /// Enable lines detection, values separation and notification.
        /// Each line is split with the value separator (TAB by default)
        /// Sends Message OnSerialValues(string [] values)
        /// </summary>
        public bool NotifyValues = false;

        /// <summary>
        /// The values separator.
        /// </summary>
        public char ValuesSeparator = '\t';

        /// <summary>
        /// The first line has been received.
        /// </summary>
        bool FirstLineReceived = false;

        //string serialOut = "";
        private List<string> linesIn = new List<string> ();

        /// <summary>
        /// Gets the received bytes count.
        /// </summary>
        /// <value>The received bytes count.</value>
        public int ReceivedBytesCount { get { return BufferIn.Length; } }

        /// <summary>
        /// Gets the received bytes.
        /// </summary>
        /// <value>The received bytes.</value>
        public string ReceivedBytes { get { return BufferIn; } }

        /// <summary>
        /// Clears the received bytes. 
        /// Warning: This prevents line detection and notification. 
        /// To be used when no \n is expected to avoid keeping unnecessary big amount of data in memory
        /// You should normally not call this function if \n are expected.
        /// </summary>
        public void ClearReceivedBytes ()
        {
            BufferIn = "";
        }

        /// <summary>
        /// Gets the lines count.
        /// </summary>
        /// <value>The lines count.</value>
        public int linesCount { get { return linesIn.Count; } }

        #region Private vars

        // buffer data as they arrive, until a new line is received
        private string BufferIn = "";

        // flag to detect whether coroutine is still running to workaround coroutine being stopped after saving scripts while running in Unity
        private int nCoroutineRunning = 0;

        #endregion

        #region Static vars

        // Serial configuration
        private SerialConfig config = null;

        // Only one serial port shared among all instances and living after all instances have been destroyed
        private SerialPort serialPort;

        // All instances of this component
        // private static List<SerialIO> s_instances = new List<SerialIO> ();

        // Enable debug info. Do not change here. Use parameter on SerialConfig component
        private bool debug = false;

        private float lastDataIn = 0;
        private float lastDataCheck = 0;

        #endregion

        void OnValidate ()
        {
            if (RememberLines < 0)
                RememberLines = 0;
        }

        void OnEnable ()
        {
            // s_instances.Add (this);

            if (GetConfig ().logDebugInfos && !debug) {
                Debug.LogWarning ("Serial debug informations enabled by " + GetConfig ());
                debug = true;
            }

            checkOpen ();
        }

        void OnDisable ()
        {
            // s_instances.Remove (this);
        }

        public void OnApplicationQuit ()
        {
            if (serialPort != null) {
                if (serialPort.IsOpen) {
                    if (debug) {
                        Debug.Log ("closing serial port");
                    }
                    serialPort.Close ();
                }

                serialPort = null;
            }
        }

        void Update ()
        {
            if (serialPort != null) {

                // Will (re)open if device disconnected and reconnected (or Leonardo reset)
                checkOpen ();

                if (nCoroutineRunning == 0) {

                    //Debug.Log ("starting ReadSerialLoop* coroutine from " + this.name);

                    switch (Application.platform) {

                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                            //				case RuntimePlatform.OSXEditor:
                            //				case RuntimePlatform.OSXPlayer:

                            // Each instance has its own coroutine but only one will be active
                        StartCoroutine (ReadSerialLoopWin ());
                        break;

                    default:
                            // Each instance has its own coroutine but only one will be active
                        StartCoroutine (ReadSerialLoop ());
                        break;

                    }
                } else {
                    if (nCoroutineRunning > 1) {
                        if (debug) {
                            Debug.Log (nCoroutineRunning + " coroutines in " + name);
                        }
                    }

                    nCoroutineRunning = 0;
                }
            }
        }

        public IEnumerator ReadSerialLoop ()
        {

            while (true) {

                if (!enabled) {
                    if (debug) {
                        Debug.Log ("behaviour not enabled, stopping coroutine");
                    }
                    yield break;
                }

                //Debug.Log("ReadSerialLoop ");
                nCoroutineRunning++;

                lastDataCheck = Time.time;
                try {
                    while (serialPort.BytesToRead > 0) {  // BytesToRead crashes on Windows -> use ReadLine or ReadByte in a Thread or Coroutine

                        string serialIn = serialPort.ReadExisting ();

                        //Debug.Log("just read some data: " + serialIn);

                        // Dispatch new data to each instance
                        this.receivedData(serialIn);
                        /*
                        foreach (SerialIO inst in s_instances) {
                            inst.receivedData (serialIn);
                        }
                        */

                        lastDataIn = lastDataCheck;
                    }

                } catch (System.Exception e) {
                    if (debug) {
                        Debug.LogError ("System.Exception in serial.ReadExisting: " + e.ToString ());
                        //Debug.Log ("isOpen: " + s_serial.IsOpen);
                    }
                }

                if (serialPort.IsOpen && serialPort.BytesToRead == -1) {
                    // This happens when Leonardo is reset
                    // Close the serial port here, it will be reopened later when available
                    serialPort.Close ();
                }

                yield return null;
            }

        }

        public IEnumerator ReadSerialLoopWin ()
        {
            if (debug) {
                Debug.Log ("Start listening on com port: " + serialPort.PortName);
            }

            while (true) {

                if (!enabled) {
                    Debug.Log ("behaviour not enabled, stopping coroutine");
                    yield break;
                }

                //Debug.Log ("ReadSerialLoopWin ");
                nCoroutineRunning++;
                //Debug.Log ("nCoroutineRunning: " + nCoroutineRunning);
                //Debug.Log ("Still listening on com port: " + s_serial.PortName + " open (" + s_serial.IsOpen + ") with coroutine from " + this);

                string serialIn = "";
                lastDataCheck = Time.time;
                try {
                    serialPort.ReadTimeout = 1;
                    //Debug.Log ("isOpen: " + s_serial.IsOpen);

                    while (serialPort.IsOpen) {  // BytesToRead crashes on Windows -> use ReadLine or ReadByte in a Thread or Coroutine
                        char c = (char)serialPort.ReadByte (); // ReadByte crashes on mac if device is removed (or Leonardo reset)
                        serialIn += c;
                    }
                } catch (System.TimeoutException) {
                    //Debug.Log ("System.TimeoutException in serial.ReadByte: " + e.ToString ());
                } catch (System.IO.IOException e) {
                    Debug.LogError (e);
                    // may happen when device is reset or disconnected. Close the port to attempt reopening later
                    serialPort.Close ();
                } catch (System.Exception e) {
                    Debug.LogError ("System.Exception in serial.ReadByte: " + e.ToString ());
                }

                if (serialIn.Length > 0) {

                    //Debug.Log("just read some data: " + serialIn);

                    // Dispatch new data to each instance
                    this.receivedData(serialIn);
                    /*
                    foreach (SerialIO inst in s_instances) {
                        inst.receivedData (serialIn);
                    }
                    */
                    lastDataIn = lastDataCheck;
                }

                yield return null;
            }

        }

        /// return all received lines and clear them
        /// Useful if you need to process all the received lines, even if there are several since last call
        public List<string> GetLines (bool keepLines = false)
        {

            List<string> lines = new List<string> (linesIn);

            if (!keepLines)
                linesIn.Clear ();

            return lines;
        }

        /// return only the last received line and clear them all
        /// Useful when you need only the last received values and can ignore older ones
        public string GetLastLine (bool keepLines = false)
        {

            string line = "";
            if (linesIn.Count > 0)
                line = linesIn [linesIn.Count - 1];

            if (!keepLines)
                linesIn.Clear ();

            return line;
        }

        /// <summary>
        /// Send data to the serial port.
        /// </summary>
        public void Write (string message)
        {
            if (checkOpen ())
                serialPort.Write (message);
        }

        /// <summary>
        /// Send data to the serial port and append a new line character (\n)
        /// </summary>
        public void WriteLn (string message = "")
        {
            serialPort.Write (message + "\n");
        }

        /// <summary>
        /// Act as if the serial port has received data followed by a new line.
        /// </summary>
        public void SimulateDataReceptionLn (float data)
        {
            this.receivedData(data + "\n");
            /*
            foreach (SerialIO inst in s_instances) {
                inst.receivedData (data + "\n");
            }
            */
        }

        /// <summary>
        /// Act as if the serial port has received data followed by a new line.
        /// </summary>
        public void SimulateDataReceptionLn (string data)
        {
            this.receivedData(data + "\n");
            /*
            foreach (SerialIO inst in s_instances) {
                inst.receivedData (data + "\n");
            }
            */
        }

        /// <summary>
        /// Verify if the serial port is opened and opens it if necessary
        /// </summary>
        /// <returns><c>true</c>, if port is opened, <c>false</c> otherwise.</returns>
        /// <param name="portSpeed">Port speed.</param>
        public bool checkOpen ()
        {

            if (serialPort == null) {

                int portSpeed = GetConfig ().speed;
                string portName = GetPortName ();
                if (portName == "") {
                    if (debug) {
                        Debug.Log ("Error: Couldn't find serial port.");
                    }
                    return false;
                } else {

                    switch (Application.platform) {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                            // Needed to open port above COM9 on Windows
                            // Note: only possible with new SerialPort(). Changing portName of an existing SerialPort will throw a ArgumentException: value
                        portName = @"\\.\" + portName;
                        break;
                    }

                    if (debug) {
                        Debug.Log ("Opening serial port: " + portName + " at " + portSpeed + " bauds");
                    }
                }

                if (serialPort != null && serialPort.IsOpen) {
                    serialPort.Close ();
                }

                serialPort = new SerialPort (portName, portSpeed);
            }

            if (!serialPort.IsOpen) {

                try {

                    serialPort.Open ();
                    serialPort.DtrEnable = true;

                    //Debug.Log ("default ReadTimeout: " + s_serial.ReadTimeout);
                    //s_serial.ReadTimeout = 10;

                    // clear input buffer from previous garbage
                    serialPort.DiscardInBuffer ();

                } catch (System.Exception e) {
                    if (debug) {
                        Debug.LogError ("System.Exception in serial.Open(): " + e.ToString ());
                    }
                }
            }

            return serialPort.IsOpen;
        }

        // Data has been received, do what this instance has to do with it
        protected void receivedData (string data)
        {

            if (NotifyData) {
                SendMessage ("OnSerialData", data);
            }

            // Detect lines
            if (NotifyLines || NotifyValues) {

                // prepend pending buffer to received data and split by line
                string[] lines = (BufferIn + data).Split ('\n');

                // If last line is not empty, it means the line is not complete (new line did not arrive yet), 
                // We keep it in buffer for next data.
                int nLines = lines.Length;
                BufferIn = lines [nLines - 1];

                // Loop until the penultimate line (don't use the last one: either it is empty or it has already been saved for later)
                for (int iLine = 0; iLine < nLines - 1; iLine++) {
                    string line = lines [iLine];
                    //Debug.Log ("Received a line: " + line);

                    // skip first line 
                    if (!FirstLineReceived) {
                        FirstLineReceived = true;

                        if (SkipFirstLine) {
                            if (debug) {
                                Debug.Log ("First line skipped: " + line);
                            }
                            continue;
                        }
                    }

                    // Buffer line
                    if (RememberLines > 0) {
                        linesIn.Add (line);

                        // trim lines buffer
                        int overflow = linesIn.Count - RememberLines;
                        if (overflow > 0) {
                            Debug.Log ("Serial removing " + overflow + " lines from lines buffer. Either consume lines before they are lost or set RememberLines to 0.");
                            linesIn.RemoveRange (0, overflow);
                        }
                    }

                    // notify new line
                    if (NotifyLines) {
                        SendMessage ("OnSerialLine", line);
                    }

                    // Notify values
                    if (NotifyValues) {
                        string[] values = line.Split (ValuesSeparator);
                        SendMessage ("OnSerialValues", values);
                    }

                }
            }
        }

        string GetPortName ()
        {
            SerialConfig config = GetConfig ();

            if (debug) {
                Debug.Log ("Prefered port names:\n" + string.Join ("\n", config.portNames));
            }

            List<string> portNames = new List<string> ();

            switch (Application.platform) {

            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.LinuxPlayer:

                portNames.AddRange (System.IO.Ports.SerialPort.GetPortNames ());

                if (portNames.Count == 0) {
                    portNames.AddRange (System.IO.Directory.GetFiles ("/dev/", "cu.*"));
                }
                break;

            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            default:

                portNames.AddRange (System.IO.Ports.SerialPort.GetPortNames ());
                break;
            }

            if (debug) {
                Debug.Log (portNames.Count + "available ports: \n" + string.Join ("\n", portNames.ToArray ()));
            }

            // Looking for preferred names in config
            foreach (string name in config.portNames) {

                string foundName = portNames.Find (s => s.Contains (name));
                if (foundName != null) {
                    if (debug) {
                        Debug.Log ("Found port " + foundName);
                    }
                    return foundName;
                }
            }

            // Defaults to last port in list (most chance to be an Arduino port)
            if (portNames.Count > 0)
                return portNames [portNames.Count - 1];
            else
                return "";
        }


        SerialConfig GetConfig ()
        {

            if (config == null) {
                config = GetComponentInChildren<SerialConfig> ();
                if (!config) {
                    Debug.LogWarning ("Serial configuration not found. Using default values. To configure the prefered port names and speed, add the SerialConfig component to an empty GameObject", config);

                    // Provide a default config compatible with old version of UnitySerial
                    GameObject goConfig = new GameObject ("Serial configuration");
                    config = goConfig.AddComponent<SerialConfig> ();
                }
                DontDestroyOnLoad (config.gameObject);
            }

            return config;
        }


        void OnGUI ()
        {
            // Show debug only if enabled and by the first instance to avoid overwrite same data
            if (debug && this) {
                GUILayout.Label ("Serial last data: " + lastDataIn + " (last check: " + lastDataCheck + ")");
            }
        }

    }
}
