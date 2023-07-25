using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Xml.Serialization;
using System.Threading;
using System.IO;

namespace Camera
{
    public delegate void EventoBool(bool e);
    public delegate void NewImageEvent(object sender, NewImageEventArgs a);
    public enum CameraAdquisitionMode
    {
        FROM_CAMERA,
        EMULATION,
    }

    public class Camera
    {
        [XmlIgnore]
        public HFramegrabber hfAcqHandle;
        [XmlIgnore]
        public HFramegrabber hfEmulationAcqHandle;
        [XmlIgnore]
        public HImage hiLastImage;
        [XmlIgnore]
        public HImage hiLastImageNIR;
        [XmlIgnore]
        public HImage hiLastImageRGB;
        [XmlIgnore]
        public HImage hiLastImageCalibrated;
        [XmlIgnore]
        public long lLastEncoder;
        [XmlIgnore]
        public int width;
        [XmlIgnore]
        public int height;
        [XmlIgnore]
        public CameraCalibration Calibration;
        [XmlElement("Name")]
        public string strName = "";
        [XmlElement("Type")]
        public string cameraType = "";
        [XmlIgnore]
        public int nCameraNumber = 0;
        [XmlIgnore]
        public string strDescription = "";
        [XmlIgnore]
        public bool nMode = false;
        [XmlIgnore]
        public string path = "";
        [XmlIgnore]
        public Thread adquisitionThread;
        [XmlIgnore]
        public double fps = 0;
        [XmlIgnore]
        public DateTime lastFrame;
        [XmlIgnore]
        private CameraAdquisitionMode adquMode = CameraAdquisitionMode.FROM_CAMERA;
        [XmlIgnore]
        public int interval = 0;
        [XmlElement("Configuration Path")]
        public string cameraConfigurationPath = "default";

        public event NewImageEvent NewImageEvent;
        public event EventoBool StatusChanged;

        [XmlIgnore]
        public ManualResetEvent StopLiveEvent;
        [XmlIgnore]
        public ManualResetEvent StartLiveEvent;
        
        public Camera()
        {
            try
            {
                StartLiveEvent = new ManualResetEvent(false);
                StopLiveEvent = new ManualResetEvent(false);
                Calibration = new CameraCalibration();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        public Camera(string name)
        {
            try
            {
                strName = name;
                nMode = false;//Modo normal
                strDescription = "";
                //Openframegrabber...


                StartLiveEvent = new ManualResetEvent(false);
                StopLiveEvent = new ManualResetEvent(false);
                Calibration = new CameraCalibration();
            }
            catch (Exception e)
            {
                throw (e);
            }

        }


        public Camera(string name, string type)
        {
            try
            {
                strName = name;
                nMode = false;//Modo normal
                strDescription = "";
                cameraType = type;


                StartLiveEvent = new ManualResetEvent(false);
                StopLiveEvent = new ManualResetEvent(false);
                Calibration = new CameraCalibration();
            }
            catch (Exception e)
            {
                throw (e);
            }

        }


        public Camera(string name, string type, string imagesPath, bool emulation)
        {
            try
            {
                strName = name;
                nMode = false;//Modo normal
                strDescription = "";
                cameraType = type;
                path = imagesPath;


                StartLiveEvent = new ManualResetEvent(false);
                StopLiveEvent = new ManualResetEvent(false);
                Calibration = new CameraCalibration();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        public Camera(string name, string type, string confPath)
        {
            try
            {
                strName = name;
                nMode = false;//Modo normal
                strDescription = "";
                cameraType = type;
                cameraConfigurationPath = confPath;


                StartLiveEvent = new ManualResetEvent(false);
                StopLiveEvent = new ManualResetEvent(false);
                Calibration = new CameraCalibration();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public string CameraName
        {
            get { return strName; }
            set { strName = value; }
        }

        public int CameraNumber
        {
            get { return nCameraNumber; }
            set { nCameraNumber = value; }
        }

        public string CameraDescription
        {
            get { return strDescription; }
            set { strDescription = value; }
        }

        public bool CameraStatus
        {
            get { return (hfAcqHandle != null); }
        }


        public bool CameraMode
        {
            get { return nMode; }
            set { nMode = value; if (StatusChanged != null) StatusChanged(nMode); }
        }

        public string ConfigurationPath
        {
            get { return cameraConfigurationPath; }
            set { cameraConfigurationPath = value; }
        }

        public CameraAdquisitionMode AdquisitionMode
        {
            get { return adquMode; }
            set { adquMode = value; }
        }


        public void Connect()
        {
            try
            {
                if (!this.IsConnected() && (adquMode == CameraAdquisitionMode.FROM_CAMERA))
                {

                    if (cameraType == "GigEVision")
                    {
                        //Openframegrabber...
                        hfAcqHandle = new HFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", cameraConfigurationPath, this.CameraName, 0, -1);
                    }

                    if (cameraType == "SaperaLT")
                    {
                        //Openframegrabber...
                        hfAcqHandle = new HFramegrabber("SaperaLT", 1, 1, 0, 0, 0, 0, "default", 8, "default", -1, "false", cameraConfigurationPath, this.CameraName, -1, -1);
                    }
                    else if (cameraType == "pylon")
                    {
                        //Openframegrabber...
                        hfAcqHandle = new HFramegrabber("pylon", 1, 1, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", cameraConfigurationPath, this.CameraName, -1, -1);
                    }
                    else if (cameraType == "uEye")
                    {
                        //Openframegrabber...this.CameraName
                        //hfAcqHandle = new HFramegrabber("uEye", 1, 1, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", cameraConfigurationPath, this.CameraNumber.ToString(), -1, -1);
                        string[] parameters = this.CameraName.Split(' ');
                        string deviceNumStr = parameters[0].Replace("device:", "");
                        int deviceNum = int.Parse(deviceNumStr);
                        hfAcqHandle = new HFramegrabber("uEye", 1, 1, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", cameraConfigurationPath, deviceNum.ToString(), -1, -1);

                    }
                    else if (cameraType == "File")
                    {
                        //Openframegrabber...
                        hfAcqHandle = new HFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "false", this.path, "default", 1, -1);
                        
                        // hfAcqHandle = new HFramegrabber()("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "false",this.path, "default", 1, -1);
                    }

                    //hfEmulationAcqHandle = new HFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "false", this.path, "default", 1, -1);

                    hiLastImage = AcquireImage(false);
                    if (Calibration != null)
                        hiLastImageCalibrated = Calibration.ImageCalibrated(hiLastImage, true);

                    hiLastImage.GetImageSize(out width, out height);
                    StartLiveEvent.Reset();
                    StopLiveEvent.Set();
                }
                else if (adquMode == CameraAdquisitionMode.EMULATION)
                {
                    //ARRANCA EN EMULACION
                    // para que fucione he tenido que cambiar al framework 2.0 y cerrarlo. Luego cambiar otra vez al Framework 4.5.1, porque sino no compila.
                    hfEmulationAcqHandle = new HFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", this.path, "default", -1, -1);

                    hiLastImage = AcquireImage(false);
                    if (Calibration != null)
                        hiLastImageCalibrated = Calibration.ImageCalibrated(hiLastImage, true);

                    hiLastImage.GetImageSize(out width, out height);
                    StartLiveEvent.Reset();
                    StopLiveEvent.Set();

                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        public bool IsConnected()
        {

            return (hfAcqHandle != null);

        }

        public void DisConnect()
        {
            //Closeframegrabber...
            hfAcqHandle.Dispose();
            hfAcqHandle = null;
        }


        public HImage AcquireImage(bool async)
        {
            try
            {
                HFramegrabber framegrabber;
                if (adquMode == CameraAdquisitionMode.FROM_CAMERA)
                    framegrabber = hfAcqHandle;
                else
                {              
                    framegrabber = hfEmulationAcqHandle;
                    /* lgcorzo lectura del timestamp del fichero
                    FileInfo fileInfo = new FileInfo(@"D:\PROYECTOS\CLASIFICACION_ESPECIES_GENERAL\IMAGES\20170228_092846_RAW\NIR_17.jpg");
                    // local times
                    DateTime creationTime = fileInfo.CreationTime;
                    */
                }

                if (async)
                    hiLastImage = framegrabber.GrabImageAsync(-1);
                else
                    hiLastImage = framegrabber.GrabImage();

              
                DateTime now = DateTime.Now;

                if (Calibration.OtherParameters != null)
                    hiLastImageCalibrated = Calibration.ImageCalibrated(hiLastImage, true);
                if (NewImageEvent != null)
                    NewImageEvent(this, new NewImageEventArgs(this.CameraName, hiLastImage));

                TimeSpan deltaT = (now - lastFrame);
                lastFrame       = now;
                fps             = 1 / deltaT.TotalSeconds;            
                return hiLastImage;
            }
            catch (Exception e)
            {
               
                throw (e);
               
            }
        }


        public void Live(bool async)
        {
            try
            {
                StartLiveEvent.Set();
                StopLiveEvent.Reset();


                HFramegrabber framegrabber;
                if (adquMode == CameraAdquisitionMode.FROM_CAMERA)
                    framegrabber = hfAcqHandle;
                else
                    framegrabber = hfEmulationAcqHandle;



                if (this.CameraMode == false)
                {
                    this.CameraMode = true;//Modo live
                }

                if (async)
                    framegrabber.GrabImageStart(-1);

                while (true)
                {

                    AcquireImage(async);

                    if (StopLiveEvent.WaitOne(0, true))
                    {
                        StopLiveEvent.Reset();
                        StartLiveEvent.Reset();
                        break;
                    }
       
                    Thread.Sleep(interval);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }


        public void StopLive()
        {
            try
            {
                StartLiveEvent.Reset();
                StopLiveEvent.Set();

                if (this.CameraMode == true)
                {
                    this.CameraMode = false;//Modo normal
                }
            
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void StartLive(bool async)
        {
            try
            {
                adquisitionThread = new Thread(() => Live(async));
                adquisitionThread.Priority = ThreadPriority.Highest;
                adquisitionThread.Start();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public HImage ProvideImage()
        {
            return hiLastImage;
        }


        public static void Serialize(Camera cam, string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Camera));
                StreamWriter myWriter = new StreamWriter(path);
                serializer.Serialize(myWriter, cam);
                myWriter.Close();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public static Camera Deserialize(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Camera));
                FileStream myFileStream = new FileStream(path, FileMode.Open);
                Camera cam = new Camera();
                cam = (Camera)serializer.Deserialize(myFileStream);
                myFileStream.Close();
                return cam;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }

    public class NewImageEventArgs : EventArgs
    {
        public NewImageEventArgs(string cam_name, HImage img)
        {
            camName = cam_name;
            this.image = img;
        }
        private string camName;
        public string CamName
        {
            get { return camName; }
        }
        private HImage image;
        public HImage Image
        {
            get { return image; }
        }
    }
}
