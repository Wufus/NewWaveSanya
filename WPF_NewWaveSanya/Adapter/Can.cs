using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Debug = System.Diagnostics.Debug;

namespace WPF_NewWaveSanya.Adapter
{
    class Can
    {
        const byte ERR = (byte)0;
        const byte OK = (byte)1;
        const byte BAUDRATE_125 = 125;
        const byte BAUDRATE_250 = 250;

        #region Ixxat member variables

        /// <summary>
        ///   Reference to the used VCI device.
        /// </summary>
        static IVciDevice mDevice;

        /// <summary>
        ///   Reference to the CAN controller.
        /// </summary>
        static ICanControl mCanCtl;

        /// <summary>
        ///   Reference to the CAN message communication channel.
        /// </summary>
        static ICanChannel mCanChn;

        /// <summary>
        ///   Reference to the CAN message scheduler.
        /// </summary>
        static ICanScheduler mCanSched;

        /// <summary>
        ///   Reference to the message writer of the CAN message channel.
        /// </summary>
        static ICanMessageWriter mWriter;

        /// <summary>
        ///   Reference to the message reader of the CAN message channel.
        /// </summary>
        static ICanMessageReader mReader;

        /// <summary>
        ///   Thread that handles the message reception.
        /// </summary>
        static System.Threading.Thread rxThread;

        /// <summary>
        ///   Quit flag for the receive thread.
        /// </summary>
        static long mMustQuit = 0;

        /// <summary>
        ///   Event that's set if at least one message was received.
        /// </summary>
        static System.Threading.AutoResetEvent mRxEvent;
        static System.Threading.AutoResetEvent mTxEvent;

        #endregion
        
        static IVciDevice GetDeviceByString(String adapterSerial)
        {
            IVciDevice _return = null;
            IVciDeviceManager deviceManager = null;
            IVciDeviceList deviceList = null;
            System.Collections.IEnumerator deviceEnum = null;
            String SerialNumber = String.Empty;

            try
            {
                deviceManager = VciServer.Instance().DeviceManager;
                deviceList = deviceManager.GetDeviceList();
                deviceEnum = deviceList.GetEnumerator();


                while (deviceEnum.MoveNext())
                {
                    _return = deviceEnum.Current as IVciDevice;

                    object serialNumberGuid = _return.UniqueHardwareId;
                    if (GetSerialNumberText(serialNumberGuid) == adapterSerial)
                        break;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error: " + exc.Message + "\n");
            }
            finally
            {
                DisposeVciObject(deviceManager);
                DisposeVciObject(deviceList);
                DisposeVciObject(deviceEnum);
            }
            return _return;
        }
        static public bool InitSocket(String adapter, uint baudrate)
        {
            FinalizeApp();
            IBalObject bal = null;
            bool succeeded = false;
            IVciDevice device = null;
            

            try
            {
                device = GetDeviceByString(adapter);
                //
                // Open bus access layer
                //
                bal = device.OpenBusAccessLayer();

                //
                // Open a message channel for the CAN controller
                //
                mCanChn = bal.OpenSocket(0, typeof(ICanChannel)) as ICanChannel;

                /*//
                // Open the scheduler of the CAN controller
                //
                Log("4"); // не проходит переинициализацию, что-то надо сделать
                mCanSched = bal.OpenSocket(0, typeof(ICanScheduler)) as ICanScheduler;*/

                // Initialize the message channel
                mCanChn.Initialize(1024, 128, false);

                // Get a message reader object
                mReader = mCanChn.GetMessageReader();

                // Initialize message reader
                mReader.Threshold = 1;

                // Create and assign the event that's set if at least one message
                // was received.
                mRxEvent = new System.Threading.AutoResetEvent(false);
                mReader.AssignEvent(mRxEvent);

                // Get a message wrtier object
                mWriter = mCanChn.GetMessageWriter();

                // Initialize message writer
                mWriter.Threshold = 1;

                mTxEvent = new System.Threading.AutoResetEvent(false);
                mWriter.AssignEvent(mTxEvent);

                // Activate the message channel
                mCanChn.Activate();


                //
                // Open the CAN controller
                //
                mCanCtl = bal.OpenSocket(0, typeof(ICanControl)) as ICanControl;
                CanBitrate _cb = new CanBitrate();
                switch (baudrate)
                {
                    case BAUDRATE_125:
                        _cb = CanBitrate.Cia125KBit; break;
                    case BAUDRATE_250:
                        _cb = CanBitrate.Cia250KBit; break;
                    default:
                        _cb = CanBitrate.Cia125KBit; break;
                }
                // Initialize the CAN controller
                mCanCtl.InitLine(CanOperatingModes.Standard |
                    CanOperatingModes.Extended |                    //extended отключить наверняка стоит
                    CanOperatingModes.ErrFrame,
                    _cb);

                //
                // print line status
                //
                Debug.WriteLine(" LineStatus: " + mCanCtl.LineStatus + "\n|");

                // Set the acceptance filter for std identifiers
                mCanCtl.SetAccFilter(CanFilter.Std,
                                        (uint)CanAccCode.All, (uint)CanAccMask.All);

                // Set the acceptance filter for ext identifiers
                mCanCtl.SetAccFilter(CanFilter.Ext,
                                        (uint)CanAccCode.All, (uint)CanAccMask.All);

                // Start the CAN controller
                mCanCtl.StartLine();

                succeeded = true;
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error: Initializing socket failed : " + exc.Message + "\n|");
                succeeded = false;
            }
            finally
            {
                DisposeVciObject(device);
                DisposeVciObject(bal);
            }

            return succeeded;
        }
        static public void FinalizeApp()
        {
            //
            // Dispose all hold VCI objects.
            //

            // Dispose message reader
            DisposeVciObject(mReader);

            // Dispose message writer 
            DisposeVciObject(mWriter);

            // Dispose CAN channel
            DisposeVciObject(mCanChn);

            // Dispose CAN controller
            DisposeVciObject(mCanCtl);

            // Dispose VCI device
            DisposeVciObject(mDevice);
        }
        static public String GetSerialNumberText(object serialNumberGuid)
        {
            string resultText;

            // check if the object is really a GUID type
            if (serialNumberGuid.GetType() == typeof(System.Guid))
            {
                // convert the object type to a GUID
                System.Guid tempGuid = (System.Guid)serialNumberGuid;

                // copy the data into a byte array
                byte[] byteArray = tempGuid.ToByteArray();

                // serial numbers starts always with "HW"
                if (((char)byteArray[0] == 'H') && ((char)byteArray[1] == 'W'))
                {
                    // run a loop and add the byte data as char to the result string
                    resultText = "";
                    int i = 0;
                    while (true)
                    {
                        // the string stops with a zero
                        if (byteArray[i] != 0)
                            resultText += (char)byteArray[i];
                        else
                            break;
                        i++;

                        // stop also when all bytes are converted to the string
                        // but this should never happen
                        if (i == byteArray.Length)
                            break;
                    }
                }
                else
                {
                    // if the data did not start with "HW" convert only the GUID to a string
                    resultText = serialNumberGuid.ToString();
                }
            }
            else
            {
                // if the data is not a GUID convert it to a string
                string tempString = (string)(string)serialNumberGuid;
                resultText = "";
                for (int i = 0; i < tempString.Length; i++)
                {
                    if (tempString[i] != 0)
                        resultText += tempString[i];
                    else
                        break;
                }
            }

            return resultText;
        }
        static public void DisposeVciObject(object obj)
        {
            if (null != obj)
            {
                IDisposable dispose = obj as IDisposable;
                if (null != dispose)
                {
                    dispose.Dispose();
                    obj = null;
                }
            }
        }

        public delegate void MyMethod(ICanMessage mes);
        MyMethod ReceiveMethod;
        void BeginReceive(MyMethod function)
        {
            Debug.WriteLine("Включение режима " + function.ToString());
            ReceiveMethod = function;
            rxThread = new Thread(new ThreadStart(ReceiveCanPacks));
            rxThread.Start();
        }

        static public bool TransmitData8(uint id, byte[] data)
        {
            IMessageFactory factory = VciServer.Instance().MsgFactory;
            ICanMessage canMsg = (ICanMessage)factory.CreateMsg(typeof(ICanMessage));

            canMsg.TimeStamp = 0;
            canMsg.Identifier = id;
            canMsg.FrameType = CanMsgFrameType.Data;
            canMsg.DataLength = 8;
            canMsg.SelfReceptionRequest = true;  // show this message in the console window

            for (Byte i = 0; i < data.Length; i++)
            {
                canMsg[i] = data[i];
            }

            // Write the CAN message into the transmit FIFO
            return mWriter.SendMessage(canMsg);

        }
        //bool TransmitData8(uint id, byte[] data, int ms)
        //{
        //    IMessageFactory factory = VciServer.Instance().MsgFactory;
        //    ICanMessage canMsg = (ICanMessage)factory.CreateMsg(typeof(ICanMessage));

        //    canMsg.TimeStamp = 0;
        //    canMsg.Identifier = id;
        //    canMsg.FrameType = CanMsgFrameType.Data;
        //    canMsg.DataLength = 8;
        //    canMsg.SelfReceptionRequest = true;     // show this message in the console window

        //    for (Byte i = 0; i < data.Length; i++)
        //    {
        //        canMsg[i] = data[i];
        //    }


        //    //Stopwatch sw = new Stopwatch();
        //    /*if(*/
        //    var cmt = new CanMessTimed { Time = StopWatch.Elapsed, Mess = canMsg };
        //    CanMessTimedBuffer.Add(cmt);
        //    mWriter.SendMessage(canMsg);
        //    //sw.Start();
        //    //while (sw.ElapsedMilliseconds == 2000)

        //    if (cmt.MRES.Wait(ms))
        //    {
        //        Log("-------true-----\n");
        //    }
        //    else
        //        Log("-------false-----\n");


        //    //ICanMessage mes;
        //    /*if (mRxEvent.WaitOne(1000))
        //    {
        //        while (mReader.ReadMessage(out mes))
        //        {
        //            if (mes.SelfReceptionRequest)
        //            {
        //                Log("-------true-----\n");
        //            }
        //            else
        //            {
        //                Log("-------another-----\n");
        //                //GetPackMes(mes);
        //            }
        //        }
        //    }
        //    else
        //        Log("-------false-----\n");*/


        //    //Log("3 " + mWriter.FreeCount.ToString().PadRight(10) + mWriter.Threshold.ToString().PadRight(10) + mWriter.Capacity.ToString().PadRight(10) + "\n");

        //    return false;
        //}

        void ReceiveCanPacks()
        {
            /*ICanMessage mes;
            srsMRE = new ManualResetEvent(false);

            do
            {
                if (mRxEvent.WaitOne())
                {
                    while (mReader.ReadMessage(out mes))
                    {
                        if (mes.SelfReceptionRequest) //обработка как после ожидания ответа
                        {

                        }
                        ReceiveMethod(mes); //обработка как обычного входящего
                    }
                }
            } while (0 == mMustQuit);*/
        }
    }

    class DeviceListMonitor
    {
        static Thread ARE_Thread;
        static public List<String> Devices;
        static AutoResetEvent ARE_Devices;

        public delegate void MethodContainer();
        static public event MethodContainer ListChanged;

        static public void StartDeviceListMonitor()
        {
            IVciDeviceManager deviceManager = null;
            IVciDeviceList deviceList = null;
            deviceManager = VciServer.Instance().DeviceManager;
            deviceList = deviceManager.GetDeviceList();
            ARE_Devices = new AutoResetEvent(true);         //true запустит метод сразу
            deviceList.AssignEvent(ARE_Devices);

            ARE_Thread = new Thread(ARE_Thread_Devices);
            ARE_Thread.Start();
        }

        static public void StopDeviceListMonitor()
        {
            ARE_Thread.Abort();
        }

        static void ARE_Thread_Devices()
        {
            while (ARE_Thread.ThreadState == ThreadState.Running)
            {
                if (ARE_Devices.WaitOne(500))
                    DeviceListEvent();
            }
        }

        static void DeviceListEvent()
        {
            IVciDeviceManager _deviceManager = null;
            IVciDeviceList _deviceList = null;
            System.Collections.IEnumerator _deviceEnum = null;
            IVciDevice _mDevice = null;

            Devices = new List<string>();

            Debug.WriteLine("DeviceListEvent\n");
            try
            {
                _deviceManager = VciServer.Instance().DeviceManager;
                _deviceList = _deviceManager.GetDeviceList();
                _deviceEnum = _deviceList.GetEnumerator();

                for (int i = 1; _deviceEnum.MoveNext(); i++)
                {
                    _mDevice = _deviceEnum.Current as IVciDevice;
                    Devices.Add(Can.GetSerialNumberText(_mDevice.UniqueHardwareId));
                    Debug.WriteLine(" " + i + ":" + Can.GetSerialNumberText(_mDevice.UniqueHardwareId) + "\n");
                }
                ListChanged();
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error: " + exc.Message + "\n");
            }
            finally
            {
                Can.DisposeVciObject(_mDevice);
                Can.DisposeVciObject(_deviceManager);
                Can.DisposeVciObject(_deviceList);
                Can.DisposeVciObject(_deviceEnum);
            }
        }
    }

    struct CanMessTimed
    {
        public TimeSpan Time;
        public ICanMessage Mess;
        public ManualResetEvent MRES;

        public CanMessTimed(TimeSpan time, ICanMessage mess)
        {
            Time = time;
            Mess = mess;
            MRES = new ManualResetEvent(false);
        }
    }
}
