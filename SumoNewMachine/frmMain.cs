using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text;
using static EndmillHMI.MyStatic;
//using static EndmillHMI.frmMain;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Collections;
using System.Windows.Forms.DataVisualization.Charting;
using static EndmillHMI.Beckhoff;
using System.Windows.Threading;
using System.Reflection;
using System.Net.Http;
using TwinCAT.TypeSystem;
using System.Data.Odbc;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.Collections.Concurrent;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static EndmillHMI.RobotFunctions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.AxHost;

//using System.Windows.Threading;
//using System.Reflection;
//using System.Security.Cryptography;

//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using System.Security.Cryptography;
//using TwinCAT.PlcOpen;
//using EndmillHMI.Properties;
//using static System.Windows.Forms.LinkLabel;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//using Newtonsoft.Json;
//using System.Runtime.InteropServices.ComTypes;



//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace EndmillHMI
{
    public partial class frmMain : Form
    {

        static public frmMain newFrmMain = new frmMain();
        static public Template newTemplate = new Template();

        private DataIniFile dFile = new DataIniFile();
        RobotFunctions RFload = new RobotFunctions();

        DataIniFile RobotData = new DataIniFile();


        public struct position
        {
            public Double x;
            public Double y;
            public Double z;
            public Double r;
            public Double corrX;
            public Double corrY;
            public Double corrZ;
            public Double corrR;

            public int Rotate;
            public int EndOfTray;
            public string Error;
            public int id;
        }
        public struct Points
        {
            public position Home;
            public position PickTrayOrg;
            public position PickTray;
            public position PlaceTrayOrg;
            public position PlaceTray;
            public position PlaceInspect;
            public position PickInspect;
            public position PlaceRejectOrg;
            public position PlaceReject;
            public position Maint;
            public position Currpos;
            public position P0;//pick-place tray
            public position P1;
            public position P2;
            public position P3;
            public position P_0;//reject tray
            public position P_1;
            public position P_2;
            public position P_3;
            public position ToolOff;
            public position TempPos;
            public position ToolOffTest;
            public position Air;
            //public position ZeroCorr;


        }
        Points RobotLoadPoints = new Points();
        //Points RobotUnloadPoints = new Points();

        public struct CommSet
        {
            public string ComPort;
            public string RobotComBaudrate;
            public string RobotComDataBits;
            public string RobotComParity;
            public string RobotComStopbits;
        }

        //      'PLC Mbits
        //'fini and error start from 100 (PLC 100+ address)

        bool EndOfTray = false;
        bool FlagNicolisEmpting = false;
        MyStatic.RobotData Robot1data = new MyStatic.RobotData();
        MyStatic.RobotData Robot2data = new MyStatic.RobotData();

        Beckhoff Beckhoff_Cam1 = new Beckhoff();
        Beckhoff Beckhoff_Cam2 = new Beckhoff();
        Beckhoff Beckhoff_Gen_1 = new Beckhoff();
        Beckhoff Beckhoff_IO = new Beckhoff();
        Beckhoff Beckhoff_IOlink = new Beckhoff();


        public string PlcNetID = "5.68.201.84.1.1";
        public int PlcPort = 851;
        public int StartAddressSendCam1 = 10;
        public int StartAddressSendCam2 = 110;
        //public int StartAddressSendGen = 210;
        public int StartAddressSendGen_1 = 310;
        public int StartAddressSendReadIO = 410;
        public int StartAddressSendReadIOlink = 510;

        private static TcpListener myListener;
        private static int port = 8227;
        private static IPAddress localAddr = IPAddress.Parse("192.168.0.100");
        private string CameraAddr = "http://192.168.0.200:5000/";
        private string FrontCamAddr = "http://192.168.0.200:5555/";
        private string CognexAddr = "http://192.168.0.200:5001/";
        private static string WebServerPath = @"WebServer";
        private static string serverEtag = Guid.NewGuid().ToString("N");
        HttpListener _httpListener = new HttpListener();
        CancellationTokenSource cancelToken = new CancellationTokenSource();
        Label[] lblSelect = new Label[5];
        //public MyStatic.PartData PartData = new MyStatic.PartData();
        MyStatic.RobotAction RobotLoadAct = new MyStatic.RobotAction();
        //MyStatic.RobotAction InspectStationAct = new MyStatic.RobotAction();

        //MyStatic.CncStation cncSation = new MyStatic.CncStation();
        MyStatic.PartData[] partData = null;
        MyStatic.VisionAction InspectStationAct = new MyStatic.VisionAction();

        MyStatic.AxisAction FooterStationAct = new MyStatic.AxisAction();

        public struct IO_State
        {
            public int AI_IN0_Robot_DO101;
            public int AI_IN1_Robot_DO102;
            public int AI_IN2_Esafe;
            public int AI_IN3_Doors1;
            public int AI_IN4_Doors2;
            public int AI_IN5_Doors3;
            public int AI_IN6_Doors4;
            public int AI_IN7_Doors5;
            public int AI_IN8_Doors6;
            public int AI_IN9_Pressure;

            public int BI_IN0_SB200;
            public int BI_IN1_SB201;
            public int BI_IN2_SB202;
            public int BI_IN3_SB203;
            public int BI_IN4_SB204;

            public int AO_Out0_Air;
            public int AO_Out1_V1;
            public int AO_Out2_V2;
            public int AO_Out3_V3;
            public int AO_Out4_KLV1;
            public int AO_Out5_KLV2;
            public int AO_Out6_KLV3;

            public int AO_Out8_Green;
            public int AO_Out9_Yellow;
            public int AO_Out10_Red;
            public int AO_Out11_Buzzer;
            public int AO_Out12_RobotDIN101;
            public int AO_Out13_RobotDIN102;

        }
        public static IO_State io_state = new IO_State();
        public MyStatic.Master master = new MyStatic.Master();
        public MyStatic.Weldone weldone = new MyStatic.Weldone();
        public Single LengthLUmax = 40; //for innspect long parts
        
        public frmMain()
        {
            InitializeComponent();
            newFrmMain = this;
            MyStatic.Robot = MyStatic.RobotLoad;
            lblSelect[0] = lblState1;
            lblSelect[1] = lblState2;
            lblSelect[2] = lblState3;
            lblSelect[3] = lblState4;
            lblSelect[4] = lblState5;

            for (int i = 0; i < lblSelect.Length; i++) lblSelect[i].Click += new System.EventHandler(this.lblSelect_Click);
            InspectStationAct.Reject = new bool[16];
            InspectStationAct.State = new int[16];
            




        }

        private void Label22_Click(object sender, EventArgs e)
        {

        }
        DataIniFile IniData = new DataIniFile();
        private void LoadIni()
        {
            string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\RobotIni.ini";

            string[][] arrnew = new string[1][];
            arrnew[0] = new string[0];
            //create vars array
            string mess = "";
            if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ ROBOT FILE"); return; }
            try
            {
                //create vars array
                //robot load

                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Normal Speed", arrnew), out Robot1data.NormalSpeed)) mess = mess + "Robot1 Normal Speed" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Pick Speed", arrnew), out Robot1data.PickSpeed)) mess = mess + "Robot1 Pick Speed" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Place Speed", arrnew), out Robot1data.PlaceSpeed)) mess = mess + "Robot1 Place Speed " + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Delay Pick", arrnew), out Robot1data.DelayPick)) mess = mess + "Robot1 Delay Pick" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Delay Place", arrnew), out Robot1data.DelayPlace)) mess = mess + "Robot1 Delay Place" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Above", arrnew), out Robot1data.RobotAbove)) mess = mess + "Robot1 above" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Above Pick", arrnew), out Robot1data.RobotAbovePick)) mess = mess + "Robot1 Above Pick" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Above Place", arrnew), out Robot1data.RobotAbovePlace)) mess = mess + "Robot1 Above Place" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Inverse Lock", arrnew), out Robot1data.InvLock)) mess = mess + "Robot1 Inv Lock" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("RobotLoad", "Check Grip", arrnew), out Robot1data.CheckGrip)) mess = mess + "Robot1 Check Grip" + "\r\n";
                UpDwnX3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrX", arrnew));
                UpDwnY3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrY", arrnew));
                UpDwnZ3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrZ", arrnew));
                UpDwnR3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrR", arrnew));
                UpDwnX4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrX", arrnew));
                UpDwnY4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrY", arrnew));
                UpDwnZ4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrZ", arrnew));
                UpDwnR4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrR", arrnew));
                UpDwnX5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrX", arrnew));
                UpDwnY5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrY", arrnew));
                UpDwnZ5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrZ", arrnew));
                UpDwnR5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrR", arrnew));
                UpDwnX6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrX", arrnew));
                UpDwnY6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrY", arrnew));
                UpDwnZ6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrZ", arrnew));
                UpDwnR6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrR", arrnew));
                UpDwnX7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrX", arrnew));
                UpDwnY7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrY", arrnew));
                UpDwnZ7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrZ", arrnew));
                UpDwnR7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrR", arrnew));

                if (FirstTime)
                {
                    RobotLoadPoints.PickTrayOrg.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickX", arrnew));
                    RobotLoadPoints.PickTrayOrg.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickY", arrnew));
                    RobotLoadPoints.PickTrayOrg.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickZ", arrnew));
                    RobotLoadPoints.PickTrayOrg.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickR", arrnew));
                    RobotLoadPoints.PlaceInspect.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInspX", arrnew));
                    RobotLoadPoints.PlaceInspect.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInspY", arrnew));
                    RobotLoadPoints.PlaceInspect.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInspZ", arrnew));
                    RobotLoadPoints.PlaceInspect.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInspR", arrnew));
                    RobotLoadPoints.PickInspect.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInspX", arrnew));
                    RobotLoadPoints.PickInspect.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInspY", arrnew));
                    RobotLoadPoints.PickInspect.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInspZ", arrnew));
                    RobotLoadPoints.PickInspect.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInspR", arrnew));
                    RobotLoadPoints.PlaceTrayOrg.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceX", arrnew));
                    RobotLoadPoints.PlaceTrayOrg.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceY", arrnew));
                    RobotLoadPoints.PlaceTrayOrg.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceZ", arrnew));
                    RobotLoadPoints.PlaceTrayOrg.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceR", arrnew));
                    RobotLoadPoints.PlaceRejectOrg.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRejX", arrnew));
                    RobotLoadPoints.PlaceRejectOrg.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRejY", arrnew));
                    RobotLoadPoints.PlaceRejectOrg.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRejZ", arrnew));
                    RobotLoadPoints.PlaceRejectOrg.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRejR", arrnew));
                    RobotLoadPoints.Home.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "HomeX", arrnew));
                    RobotLoadPoints.Home.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "HomeY", arrnew));
                    RobotLoadPoints.Home.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "HomeZ", arrnew));
                    RobotLoadPoints.Home.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "HomeR", arrnew));

                    RobotLoadPoints.P0.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P0X", arrnew));
                    RobotLoadPoints.P0.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P0Y", arrnew));
                    RobotLoadPoints.P0.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P0Z", arrnew));
                    RobotLoadPoints.P0.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P0R", arrnew));
                    RobotLoadPoints.P1.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P1X", arrnew));
                    RobotLoadPoints.P1.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P1Y", arrnew));
                    RobotLoadPoints.P1.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P1Z", arrnew));
                    RobotLoadPoints.P1.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P1R", arrnew));
                    RobotLoadPoints.P2.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P2X", arrnew));
                    RobotLoadPoints.P2.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P2Y", arrnew));
                    RobotLoadPoints.P2.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P2Z", arrnew));
                    RobotLoadPoints.P2.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P2R", arrnew));
                    RobotLoadPoints.P3.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P3X", arrnew));
                    RobotLoadPoints.P3.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P3Y", arrnew));
                    RobotLoadPoints.P3.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P3Z", arrnew));
                    RobotLoadPoints.P3.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P3R", arrnew));
                    RobotLoadPoints.P_0.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_0X", arrnew));
                    RobotLoadPoints.P_0.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_0Y", arrnew));
                    RobotLoadPoints.P_0.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_0Z", arrnew));
                    RobotLoadPoints.P_0.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_0R", arrnew));
                    RobotLoadPoints.P_1.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_1X", arrnew));
                    RobotLoadPoints.P_1.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_1Y", arrnew));
                    RobotLoadPoints.P_1.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_1Z", arrnew));
                    RobotLoadPoints.P_1.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_1R", arrnew));
                    RobotLoadPoints.P_2.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_2X", arrnew));
                    RobotLoadPoints.P_2.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_2Y", arrnew));
                    RobotLoadPoints.P_2.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_2Z", arrnew));
                    RobotLoadPoints.P_2.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_2R", arrnew));
                    RobotLoadPoints.P_3.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_3X", arrnew));
                    RobotLoadPoints.P_3.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_3Y", arrnew));
                    RobotLoadPoints.P_3.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_3Z", arrnew));
                    RobotLoadPoints.P_3.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "P_3R", arrnew));

                    RobotLoadPoints.Maint.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "MaintX", arrnew));
                    RobotLoadPoints.Maint.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "MaintY", arrnew));
                    RobotLoadPoints.Maint.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "MaintZ", arrnew));
                    RobotLoadPoints.Maint.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "MaintR", arrnew));

                    RobotLoadPoints.Air.x = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "AirX", arrnew));
                    RobotLoadPoints.Air.y = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "AirY", arrnew));
                    RobotLoadPoints.Air.z = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "AirZ", arrnew));
                    RobotLoadPoints.Air.r = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "AirR", arrnew));
                    

                    CameraAddr = IniData.GetKeyValueArrINI("Camera", "address", arrnew);
                    master.Diameter = Single.Parse(IniData.GetKeyValueArrINI("Master", "D", arrnew));
                    master.diameter = Single.Parse(IniData.GetKeyValueArrINI("Master", "d", arrnew));
                    master.Length = Single.Parse(IniData.GetKeyValueArrINI("Master", "L", arrnew));
                    master.LU = Single.Parse(IniData.GetKeyValueArrINI("Master", "LU", arrnew));
                    master.Ax1_Work = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax1_WorkTop", arrnew));
                    master.Ax2_Work = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax2_WorkTop", arrnew));
                    master.Ax3_Work = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax3_WorkTop", arrnew));
                    master.Ax3_Front = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax3_WorkFront", arrnew));
                    master.Ax4_Work = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax4_WorkTop", arrnew));
                    master.Ax4_Front = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax4_Front", arrnew));
                    master.Ax5_Diam = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax5_Diam", arrnew));
                    master.Ax5_Weldone = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax5_Weldone", arrnew));
                    master.Ax5_Work = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax5_WorkTop", arrnew));

                    master.Ax5_Front = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax5_Front", arrnew));
                    master.Ax5_RightEdge = Single.Parse(IniData.GetKeyValueArrINI("Master", "Ax5_RightEdge", arrnew));
                    master.kRightEdge = Single.Parse(IniData.GetKeyValueArrINI("Master", "kRightEdge", arrnew));

                    upDwnCam2z.UpDownValue = 0;// master.Ax1_Work;
                    upDwnLamp1Z.UpDownValue = 0;// master.Ax2_Work;
                    upDwnCam1x.UpDownValue = 0;// master.Ax3_Work;
                    upDwnFootWorkR.UpDownValue = 0;// master.Ax4_Work;
                    upDwnFootWorkTopX.UpDownValue = 0;// master.Ax5_Work;
                    upDwnFootWeldX.UpDownValue = 0;// master.Ax5_Weldone;
                    upDwnFootDX.UpDownValue = 0;// master.Ax5_Diam;


                }

                RobotLoadPoints.PickTrayOrg.corrX = (Single)UpDwnX3.UpDownValue;
                RobotLoadPoints.PickTrayOrg.corrY = (Single)UpDwnY3.UpDownValue;
                RobotLoadPoints.PickTrayOrg.corrZ = (Single)UpDwnZ3.UpDownValue;
                RobotLoadPoints.PickTrayOrg.corrR = (Single)UpDwnR3.UpDownValue;

                RobotLoadPoints.PlaceInspect.corrX = (Single)UpDwnX4.UpDownValue;
                RobotLoadPoints.PlaceInspect.corrY = (Single)UpDwnY4.UpDownValue;
                RobotLoadPoints.PlaceInspect.corrZ = (Single)UpDwnZ4.UpDownValue;
                RobotLoadPoints.PlaceInspect.corrR = (Single)UpDwnR4.UpDownValue;

                RobotLoadPoints.PickInspect.corrX = (Single)UpDwnX5.UpDownValue;
                RobotLoadPoints.PickInspect.corrY = (Single)UpDwnY5.UpDownValue;
                RobotLoadPoints.PickInspect.corrZ = (Single)UpDwnZ5.UpDownValue;
                RobotLoadPoints.PickInspect.corrR = (Single)UpDwnR5.UpDownValue;

                RobotLoadPoints.PlaceTrayOrg.corrX = (Single)UpDwnX6.UpDownValue;
                RobotLoadPoints.PlaceTrayOrg.corrY = (Single)UpDwnY6.UpDownValue;
                RobotLoadPoints.PlaceTrayOrg.corrZ = (Single)UpDwnZ6.UpDownValue;
                RobotLoadPoints.PlaceTrayOrg.corrR = (Single)UpDwnR6.UpDownValue;

                RobotLoadPoints.PlaceRejectOrg.corrX = (Single)UpDwnX7.UpDownValue;
                RobotLoadPoints.PlaceRejectOrg.corrY = (Single)UpDwnY7.UpDownValue;
                RobotLoadPoints.PlaceRejectOrg.corrZ = (Single)UpDwnZ7.UpDownValue;
                RobotLoadPoints.PlaceRejectOrg.corrR = (Single)UpDwnR7.UpDownValue;


                Robot1data.Gripper = IniData.GetKeyValueArrINI("RobotLoad", "Gripper", arrnew);

                //communication
                surlwrite1 = IniData.GetKeyValueArrINI("RobotLoadComm", "surlwrite1", arrnew);
                surlread1 = IniData.GetKeyValueArrINI("RobotLoadComm", "surlread1", arrnew);
                surlrobot1 = IniData.GetKeyValueArrINI("RobotLoadComm", "surlrobot1", arrnew);
                surlswrite1 = IniData.GetKeyValueArrINI("RobotLoadComm", "surlswrite1", arrnew);
                surlregwrite1 = IniData.GetKeyValueArrINI("RobotLoadComm", "surlregwrite1", arrnew);
                lblRobot1Address.Text = IniData.GetKeyValueArrINI("RobotLoadComm", "address1", arrnew);
                PlcNetID = IniData.GetKeyValueArrINI("PLC", "PlcNetID", arrnew).Trim();
                txtPlcAddess.Text = PlcNetID;

                txtNormSpeed.Text = Robot1data.NormalSpeed.ToString();
                //txtSpeed.Text = Robot1data.SpeedOvr.ToString();
                txtPickSpeed.Text = Robot1data.PickSpeed.ToString();
                txtPlaceSpeed.Text = Robot1data.PlaceSpeed.ToString();
                txtAbove.Text = Robot1data.RobotAbove.ToString();
                txtAbovePick.Text = Robot1data.RobotAbovePick.ToString();
                txtAbovePlace.Text = Robot1data.RobotAbovePlace.ToString();
                txtDelayPick.Text = Robot1data.DelayPick.ToString();
                txtDelayPlace.Text = Robot1data.DelayPlace.ToString();
                if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                if (chkCheckGripper.Checked) Robot1data.CheckGrip = 1; else Robot1data.CheckGrip = 0;
                //if (chkInverseLock.Checked) Robot1data.InvLock = 1; else Robot1data.InvLock = 0;
                Robot1data.Gripper = cmbGripper.Text;






            }

            catch (Exception err)
            { MessageBox.Show("ERROR READ ROBOT FILE " + err); return; }
        }
        public void LoadIO(string Robot)
        {
            try
            {
                string robot_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\RobotIO.ini";

                string[][] arrnew1 = new string[1][];
                arrnew1[0] = new string[0];
                DataIniFile RobotData = new DataIniFile();
                //user control
                if (!RobotData.ReadIniFile(robot_inifile, ref arrnew1))
                {
                    dFile.WriteLogFile("ERROR READ ROBOT INIFILE");
                    MessageBox.Show("ERROR READ ROBOT INIFILE", "ERROR", MessageBoxButtons.OK,
                                                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                for (int i = 0; i < 24; i++)
                {
                    controlsArrayDI.ControlIndex = i;
                    controlsArrayDI.ControlBackColor = Color.LightGray;
                    string s = (RobotData.GetKeyValueArrINI(Robot, "DIN" + i.ToString(), arrnew1)).Substring(0, 6);
                    if ((RobotData.GetKeyValueArrINI(Robot, "DIN" + i.ToString(), arrnew1)).Length > 6)
                    {
                        controlsArrayDI.ControlText = s + "\n" +
                            (RobotData.GetKeyValueArrINI(Robot, "DIN" + i.ToString(), arrnew1)).Substring(7);
                    }
                    else controlsArrayDI.ControlText = s + "\n";
                }
                for (int i = 0; i < 24; i++)
                {
                    controlsArrayDOUT.ControlIndex = i;
                    controlsArrayDOUT.ControlBackColor = Color.LightGray;
                    string s = (RobotData.GetKeyValueArrINI(Robot, "DOUT" + i.ToString(), arrnew1)).Substring(0, 5);
                    if ((RobotData.GetKeyValueArrINI(Robot, "DOUT" + i.ToString(), arrnew1)).Length > 5)
                    {
                        controlsArrayDOUT.ControlText = s + "\n" +
                            (RobotData.GetKeyValueArrINI(Robot, "DOUT" + i.ToString(), arrnew1)).Substring(6);
                    }
                    else controlsArrayDOUT.ControlText = s + "\n";
                }
                for (int i = 0; i < 8; i++)
                {
                    controlsArrayRI.ControlIndex = i;
                    controlsArrayRI.ControlBackColor = Color.LightGray;
                    string s = (RobotData.GetKeyValueArrINI(Robot, "HIN" + i.ToString(), arrnew1)).Substring(0, 3);
                    if ((RobotData.GetKeyValueArrINI(Robot, "HIN" + i.ToString(), arrnew1)).Length > 3)
                    {
                        controlsArrayRI.ControlText = s + "\n" +
                            (RobotData.GetKeyValueArrINI(Robot, "HIN" + i.ToString(), arrnew1)).Substring(3);
                    }
                    else controlsArrayRI.ControlText = s + "\n";
                }
                for (int i = 0; i < 8; i++)
                {
                    controlsArrayRO.ControlIndex = i;
                    controlsArrayRO.ControlBackColor = Color.LightGray;
                    string s = (RobotData.GetKeyValueArrINI(Robot, "HO" + i.ToString(), arrnew1)).Substring(0, 3);
                    if ((RobotData.GetKeyValueArrINI(Robot, "HO" + i.ToString(), arrnew1)).Length > 3)
                    {
                        controlsArrayRO.ControlText = s + "\n" +
                            (RobotData.GetKeyValueArrINI(Robot, "HO" + i.ToString(), arrnew1)).Substring(3);
                    }
                    else controlsArrayRO.ControlText = s + "\n";
                }
                for (int i = 0; i < 8; i++)
                {
                    controlsArrayOPOUT.ControlIndex = i;
                    controlsArrayOPOUT.ControlBackColor = Color.LightGray;
                    string s = (RobotData.GetKeyValueArrINI(Robot, "OPO" + i.ToString(), arrnew1)).Substring(0, 3);
                    if ((RobotData.GetKeyValueArrINI(Robot, "OPO" + i.ToString(), arrnew1)).Length > 3)
                    {
                        controlsArrayOPOUT.ControlText = s + "\n" +
                            (RobotData.GetKeyValueArrINI(Robot, "OPO" + i.ToString(), arrnew1)).Substring(3);
                    }
                    else controlsArrayOPOUT.ControlText = s + "\n";
                }
                for (int i = 0; i < 8; i++)
                {
                    controlsArrayOPIN.ControlIndex = i;
                    controlsArrayOPIN.ControlBackColor = Color.LightGray;
                    string s = (RobotData.GetKeyValueArrINI(Robot, "OPIN" + i.ToString(), arrnew1)).Substring(0, 3);
                    if ((RobotData.GetKeyValueArrINI(Robot, "OPIN" + i.ToString(), arrnew1)).Length > 3)
                    {
                        controlsArrayOPIN.ControlText = s + "\n" +
                            (RobotData.GetKeyValueArrINI(Robot, "OPIN" + i.ToString(), arrnew1)).Substring(3);
                    }
                    else controlsArrayOPIN.ControlText = s + "\n";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR LOAD ROBOT IO");
            }
        }

        public void SaveIni()
        {
            string filename = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\RobotIni.ini";
            string[][] arrsave = new string[1][];
            arrsave[0] = new string[1];
            string[] s;
            s = new string[9];
            string mess = "";


            //robot load
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Normal Speed", Robot1data.NormalSpeed.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick Speed", Robot1data.PickSpeed.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place Speed", Robot1data.PlaceSpeed.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Delay Pick", Robot1data.DelayPick.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Delay Place", Robot1data.DelayPlace.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Above", Robot1data.RobotAbove.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Above Pick", Robot1data.RobotAbovePick.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Above Place", Robot1data.RobotAbovePlace.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Inverse Lock", Robot1data.InvLock.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Check Grip", Robot1data.CheckGrip.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrX", UpDwnX3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrY", UpDwnY3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrZ", UpDwnZ3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrR", UpDwnR3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrX", UpDwnX4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrY", UpDwnY4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrZ", UpDwnZ4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrR", UpDwnR4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrX", UpDwnX5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrY", UpDwnY5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrZ", UpDwnZ5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrR", UpDwnR5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrX", UpDwnX6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrY", UpDwnY6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrZ", UpDwnZ6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrR", UpDwnR6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrX", UpDwnX7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrY", UpDwnY7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrZ", UpDwnZ7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrR", UpDwnR7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            RobotLoadPoints.PickTrayOrg.corrX = (Single)UpDwnX3.UpDownValue;
            RobotLoadPoints.PickTrayOrg.corrY = (Single)UpDwnY3.UpDownValue;
            RobotLoadPoints.PickTrayOrg.corrZ = (Single)UpDwnZ3.UpDownValue;
            RobotLoadPoints.PickTrayOrg.corrR = (Single)UpDwnR3.UpDownValue;

            RobotLoadPoints.PlaceInspect.corrX = (Single)UpDwnX4.UpDownValue;
            RobotLoadPoints.PlaceInspect.corrY = (Single)UpDwnY4.UpDownValue;
            RobotLoadPoints.PlaceInspect.corrZ = (Single)UpDwnZ4.UpDownValue;
            RobotLoadPoints.PlaceInspect.corrR = (Single)UpDwnR4.UpDownValue;

            RobotLoadPoints.PickInspect.corrX = (Single)UpDwnX5.UpDownValue;
            RobotLoadPoints.PickInspect.corrY = (Single)UpDwnY5.UpDownValue;
            RobotLoadPoints.PickInspect.corrZ = (Single)UpDwnZ5.UpDownValue;
            RobotLoadPoints.PickInspect.corrR = (Single)UpDwnR5.UpDownValue;

            RobotLoadPoints.PlaceTrayOrg.corrX = (Single)UpDwnX6.UpDownValue;
            RobotLoadPoints.PlaceTrayOrg.corrY = (Single)UpDwnY6.UpDownValue;
            RobotLoadPoints.PlaceTrayOrg.corrZ = (Single)UpDwnZ6.UpDownValue;
            RobotLoadPoints.PlaceTrayOrg.corrR = (Single)UpDwnR6.UpDownValue;

            RobotLoadPoints.PlaceRejectOrg.corrX = (Single)UpDwnX7.UpDownValue;
            RobotLoadPoints.PlaceRejectOrg.corrY = (Single)UpDwnY7.UpDownValue;
            RobotLoadPoints.PlaceRejectOrg.corrZ = (Single)UpDwnZ7.UpDownValue;
            RobotLoadPoints.PlaceRejectOrg.corrR = (Single)UpDwnR7.UpDownValue;


            if (!RobotData.CreateKeyValueArr("RobotLoad", "Gripper", Robot1data.Gripper, ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }



            string error = "";
            if (!RobotData.WriteIniFile(filename.Trim(), arrsave, out error))
            {
                MessageBox.Show("ERROR SAVE ORDER CODE");
            }

        }
        public void SavePosIni()
        {
            string filename = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\RobotIni.ini";
            string[][] arrsave = new string[1][];
            arrsave[0] = new string[1];
            string[] s;
            s = new string[9];
            string mess = "";


            //robot load

            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickX", RobotLoadPoints.PickTrayOrg.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickY", RobotLoadPoints.PickTrayOrg.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickZ", RobotLoadPoints.PickTrayOrg.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickR", RobotLoadPoints.PickTrayOrg.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInspX", RobotLoadPoints.PlaceInspect.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInspY", RobotLoadPoints.PlaceInspect.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInspZ", RobotLoadPoints.PlaceInspect.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInspR", RobotLoadPoints.PlaceInspect.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInspX", RobotLoadPoints.PickInspect.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInspY", RobotLoadPoints.PickInspect.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInspZ", RobotLoadPoints.PickInspect.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInspR", RobotLoadPoints.PickInspect.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceX", RobotLoadPoints.PlaceTrayOrg.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceY", RobotLoadPoints.PlaceTrayOrg.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceZ", RobotLoadPoints.PlaceTrayOrg.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceR", RobotLoadPoints.PlaceTrayOrg.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRejX", RobotLoadPoints.PlaceRejectOrg.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRejY", RobotLoadPoints.PlaceRejectOrg.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRejZ", RobotLoadPoints.PlaceRejectOrg.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRejR", RobotLoadPoints.PlaceRejectOrg.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "HomeX", RobotLoadPoints.Home.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "HomeY", RobotLoadPoints.Home.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "HomeZ", RobotLoadPoints.Home.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "HomeR", RobotLoadPoints.Home.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P0X", RobotLoadPoints.P0.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P0Y", RobotLoadPoints.P0.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P0Z", RobotLoadPoints.P0.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P0R", RobotLoadPoints.P0.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P1X", RobotLoadPoints.P1.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P1Y", RobotLoadPoints.P1.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P1Z", RobotLoadPoints.P1.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P1R", RobotLoadPoints.P1.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P2X", RobotLoadPoints.P2.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P2Y", RobotLoadPoints.P2.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P2Z", RobotLoadPoints.P2.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P2R", RobotLoadPoints.P2.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P3X", RobotLoadPoints.P3.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P3Y", RobotLoadPoints.P3.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P3Z", RobotLoadPoints.P3.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P3R", RobotLoadPoints.P3.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_0X", RobotLoadPoints.P_0.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_0Y", RobotLoadPoints.P_0.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_0Z", RobotLoadPoints.P_0.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_0R", RobotLoadPoints.P_0.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_1X", RobotLoadPoints.P_1.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_1Y", RobotLoadPoints.P_1.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_1Z", RobotLoadPoints.P_1.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_1R", RobotLoadPoints.P_1.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_2X", RobotLoadPoints.P_2.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_2Y", RobotLoadPoints.P_2.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_2Z", RobotLoadPoints.P_2.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_2R", RobotLoadPoints.P_2.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_3X", RobotLoadPoints.P_3.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_3Y", RobotLoadPoints.P_3.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_3Z", RobotLoadPoints.P_3.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "P_3R", RobotLoadPoints.P_3.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("RobotLoad", "MaintX", RobotLoadPoints.Maint.x.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "MaintY", RobotLoadPoints.Maint.y.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "MaintZ", RobotLoadPoints.Maint.z.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "MaintR", RobotLoadPoints.Maint.r.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            string error = "";
            if (!RobotData.WriteIniFile(filename.Trim(), arrsave, out error))
            {
                MessageBox.Show("ERROR SAVE");
            }

        }
        public void SaveItem()
        {
            string filename = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Items\\" + txtItem.Text.Trim() + ".ini";
            string[][] arrsave = new string[1][];
            arrsave[0] = new string[1];
            string[] s;
            s = new string[9];
            string mess = "";


            //robot load
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Normal Speed", Robot1data.NormalSpeed.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick Speed", Robot1data.PickSpeed.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place Speed", Robot1data.PlaceSpeed.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Delay Pick", Robot1data.DelayPick.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Delay Place", Robot1data.DelayPlace.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Above", Robot1data.RobotAbove.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Above Pick", Robot1data.RobotAbovePick.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Above Place", Robot1data.RobotAbovePlace.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Inverse Lock", Robot1data.InvLock.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Check Grip", Robot1data.CheckGrip.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrX", UpDwnX3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrY", UpDwnY3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrZ", UpDwnZ3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Pick CorrR", UpDwnR3.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrX", UpDwnX4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrY", UpDwnY4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrZ", UpDwnZ4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceInsp CorrR", UpDwnR4.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrX", UpDwnX5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrY", UpDwnY5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrZ", UpDwnZ5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PickInsp CorrR", UpDwnR5.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrX", UpDwnX6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrY", UpDwnY6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrZ", UpDwnZ6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "Place CorrR", UpDwnR6.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrX", UpDwnX7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrY", UpDwnY7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrZ", UpDwnZ7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("RobotLoad", "PlaceRej CorrR", UpDwnR7.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }



            RobotLoadPoints.PickTrayOrg.corrX = (Single)UpDwnX3.UpDownValue;
            RobotLoadPoints.PickTrayOrg.corrY = (Single)UpDwnY3.UpDownValue;
            RobotLoadPoints.PickTrayOrg.corrZ = (Single)UpDwnZ3.UpDownValue;
            RobotLoadPoints.PickTrayOrg.corrR = (Single)UpDwnR3.UpDownValue;

            RobotLoadPoints.PlaceInspect.corrX = (Single)UpDwnX4.UpDownValue;
            RobotLoadPoints.PlaceInspect.corrY = (Single)UpDwnY4.UpDownValue;
            RobotLoadPoints.PlaceInspect.corrZ = (Single)UpDwnZ4.UpDownValue;
            RobotLoadPoints.PlaceInspect.corrR = (Single)UpDwnR4.UpDownValue;

            RobotLoadPoints.PickInspect.corrX = (Single)UpDwnX5.UpDownValue;
            RobotLoadPoints.PickInspect.corrY = (Single)UpDwnY5.UpDownValue;
            RobotLoadPoints.PickInspect.corrZ = (Single)UpDwnZ5.UpDownValue;
            RobotLoadPoints.PickInspect.corrR = (Single)UpDwnR5.UpDownValue;

            RobotLoadPoints.PlaceTrayOrg.corrX = (Single)UpDwnX6.UpDownValue;
            RobotLoadPoints.PlaceTrayOrg.corrY = (Single)UpDwnY6.UpDownValue;
            RobotLoadPoints.PlaceTrayOrg.corrZ = (Single)UpDwnZ6.UpDownValue;
            RobotLoadPoints.PlaceTrayOrg.corrR = (Single)UpDwnR6.UpDownValue;

            RobotLoadPoints.PlaceRejectOrg.corrX = (Single)UpDwnX7.UpDownValue;
            RobotLoadPoints.PlaceRejectOrg.corrY = (Single)UpDwnY7.UpDownValue;
            RobotLoadPoints.PlaceRejectOrg.corrZ = (Single)UpDwnZ7.UpDownValue;
            RobotLoadPoints.PlaceRejectOrg.corrR = (Single)UpDwnR7.UpDownValue;


            if (!RobotData.CreateKeyValueArr("RobotLoad", "Gripper", Robot1data.Gripper, ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            //if (!RobotData.CreateKeyValueArr("RobotLoad", "Tray", cmbOrderTray.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "Order", txtOrder.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "Item", txtItem.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "Tray", cmbOrderTray.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "BladesNumber", upDwnCount.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "Diam", txtPartDiam.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "d", txtPartDiamd.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "W", txtPartWeight.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "L", txtPartLength.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "LU", txtPartLengthU.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Item", "Comment", txtComment.Text.Trim(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            if (!RobotData.CreateKeyValueArr("Ax1", "Ax_WorkTop", upDwnCam2z.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax2", "Ax_WorkTop", upDwnLamp1Z.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax3", "Ax_WorkFront", upDwnCam1x.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax4", "Ax_WorkTop", upDwnFootWorkR.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_WorkTop", upDwnFootWorkTopX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_Weldone", upDwnFootWeldX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_Diam", upDwnFootDX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_WorkFront", upDwnFootWorkFrontX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }



            string error = "";
            if (!RobotData.WriteIniFile(filename.Trim(), arrsave, out error))
            {
                MessageBox.Show("ERROR SAVE ORDER CODE");
            }

        }
        public void SaveAxis()
        {
            string filename = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\PLC_Data.ini";
            string[][] arrsave = new string[1][];
            arrsave[0] = new string[1];
            string[] s;
            s = new string[9];
            string mess = "";


            //robot load
            if (!RobotData.CreateKeyValueArr("Ax1", "Ax_WorkTop", upDwnCam2z.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax2", "Ax_WorkTop", upDwnLamp1Z.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax3", "Ax_WorkFront", upDwnCam1x.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax4", "Ax_WorkTop", upDwnFootWorkR.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_WorkTop", upDwnFootWorkTopX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_Weldone", upDwnFootWeldX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_Diam", upDwnFootDX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
            if (!RobotData.CreateKeyValueArr("Ax5", "Ax_WorkFront", upDwnFootWorkFrontX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

            string error = "";
            if (!RobotData.WriteIniFile(filename.Trim(), arrsave, out error))
            {
                MessageBox.Show("ERROR SAVE Axis Data");
            }

        }

        public static string surlwrite1 = "";//"D:/test6.html";// html file on D:write reg
        public static string surlread1 = "";//"http://192.168.0.20/fr/example.stm";// html file on FR:read reg
        public static string surlrobot1 = "";//"http://192.168.0.20/";// html file on FR:read reg
        public static string surlswrite1 = "";//"D:/test7.html";// html file on D:write reg
        public static string surlregwrite1 = "";// "http://192.168.0.20/karel/mpnlsrv ";

        Uri urlrobot1 = null;
        public bool FirstTime = false;

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //lblStation[].Click += new System.EventHandler(this.LblShp12_Click);
            //lblChar = label102;
            //Controls.Add(label102);
            ControlsEnable(false);
            //timer1.Tick += new System.EventHandler(timer1_Tick);
            //timer1.Interval = 10;

            cmbPosStations.Items.Clear();
            cmbPosStations.Text = "Select Station";
            cmbPosStations.Items.Add("Top Inspection");
            cmbPosStations.Items.Add("Front Inspection");
            cmbPosStations.Items.Add("Measure Weldone");
            cmbPosStations.Items.Add("Measure Diameter");


            Beckhoff_Cam1.SetText(1, txtMess, this, true);
            Beckhoff_Cam2.SetText(1, txtMess, this, true);
            Beckhoff_Gen_1.SetText(1, txtMess, this, true);
            Beckhoff_IO.SetText(1, txtMess, this, true);
            Beckhoff_IOlink.SetText(1, txtMess, this, true);
            Beckhoff_Cam1.bwName = "Beckhoff_Cam1";
            Beckhoff_Cam2.bwName = "Beckhoff_Cam2";
            Beckhoff_Gen_1.bwName = "Beckhoff_Gen1";
            Beckhoff_IO.bwName = "Beckhoff_ReadIO";
            Beckhoff_IOlink.bwName = "Beckhoff_IOlink";
            //Beckhoff.tcAds.Connect(PlcNetID, PlcPort);

            if (frmMain.newFrmMain.chkDebug.Checked == true) MyStatic.chkDebug = true; else MyStatic.chkDebug = false;

            tabPage10.Text = "ROBOT" + '\r' + "SETUP";
            tabPage9.Text = "ROBOT" + '\r' + "COMM";

            tabPage1.Text = "ROBOT 1" + '\r' + " Web";

            tabPage3.Text = "ROBOT" + '\r' + "Data";

            //tabPage1.Text = "PICK" + '\r' + "VISION";
            //tabPage4.Text = "PLACE" + '\r' + "VISION";
            //tabPage6.Text = "Pocket" + '\r' + "IN";
            //tabPage7.Text = "Pocket" + '\r' + "OUT";
            System.Drawing.Size size = new System.Drawing.Size(1000, 1000);
            tabPage10.ClientSize = size;
            //this.tabControl1.TabPages.Remove(this.tabPage17);


            //FW1.FanucWebInit(surlwrite1, surlread1, surlrobot1, surlswrite1, surlregwrite1);
            //FW2.FanucWebInit(surlwrite2, surlread2, surlrobot2, surlswrite2, surlregwrite2);
            //lblRobot1Address.Text = "192.168.0.100";
            //lblRobot2Address.Text = "192.168.0.101";
            FW1.SetControls(txtRobot, this, null);

            //robot positions

            RobotLoadPoints.Home.id = 1;
            RobotLoadPoints.PickTrayOrg.id = 2;
            RobotLoadPoints.PlaceTrayOrg.id = 3;
            RobotLoadPoints.PlaceInspect.id = 4;
            RobotLoadPoints.PickInspect.id = 5;
            RobotLoadPoints.PlaceReject.id = 6;
            RobotLoadPoints.P0.id = 7;
            RobotLoadPoints.P1.id = 8;
            RobotLoadPoints.P2.id = 9;
            RobotLoadPoints.P3.id = 10;
            RobotLoadPoints.P_0.id = 14;
            RobotLoadPoints.P_1.id = 15;
            RobotLoadPoints.P_2.id = 16;
            RobotLoadPoints.P_3.id = 17;

            //RobotLoadPoints.ToolOff.id = 10;
            //RobotLoadPoints.Maint.id = 10;
            RobotLoadPoints.Currpos.id = 19;
            RobotLoadPoints.TempPos.id = 20;




            RobotLoadPoints.PickTrayOrg.x = 273.5;
            RobotLoadPoints.PickTrayOrg.y = -270.5;
            RobotLoadPoints.PickTrayOrg.z = 180;
            RobotLoadPoints.PickTrayOrg.r = -10;






            FirstTime = true;
            LoadIni();
            Beckhoff.tcAds.Connect(PlcNetID.Trim(), PlcPort);
            FirstTime = false;
            urlrobot1 = new Uri(surlrobot1);
            FW1.FanucWebInit(surlwrite1, surlread1, surlrobot1, surlswrite1, surlregwrite1);

            LoadIO("RobotLoadIO");

            MyStatic.Robot = MyStatic.RobotLoad;

            cmbSingleMove.Items.Clear();
            cmbSingleMove.Items.Add("Pick Tray");
            cmbSingleMove.Items.Add("Air Clean");
            cmbSingleMove.Items.Add("Place Inspection");
            cmbSingleMove.Items.Add("Pick Inspection");
            cmbSingleMove.Items.Add("Place Tray");
            cmbSingleMove.Items.Add("Place Reject");

            cmbPosition.Items.Clear();
            cmbPosition.Items.Add("PR[02] Pick Tray");
            //cmbPosition.Items.Add("PR[25] Air Clean");
            cmbPosition.Items.Add("PR[04] Place Inspection");
            cmbPosition.Items.Add("PR[05] Pick Inspection");
            cmbPosition.Items.Add("PR[03] Place Tray");
            cmbPosition.Items.Add("PR[06] Place Reject");

            LoadRobotData(MyStatic.RobotLoad);
            string[] files = new string[1];
            GetGrippers(ref files);
            cmbGripper.Items.Clear();
            for (int i = 0; i < files.Length; i++) { cmbGripper.Items.Add(files[i]); }
            inv.settxt(txtSpeedSt, Math.Abs(trackBarSpeedSt.Value).ToString());
            LoadPlcData();
            LoadIniIO(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\" + "PlcIO.ini");

            frmMain.newFrmMain.trackSpeed.Value = 5;
            FanucSpeed = 5;
            txtSpeed.Text = FanucSpeed.ToString();
            nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
            nFrontCountUpDwn = (int)upDwnCount.UpDownValue;
            nColorUpDwn=(int)upDwnColor.UpDownValue;

            InitGridReject();

            btnLoadOrder_Click(null, null);






        }
        private void LoadRobotData(int robot)
        {
            if (robot == MyStatic.RobotLoad)
            {
                txtNormSpeed.Text = Robot1data.NormalSpeed.ToString();
                txtPickSpeed.Text = Robot1data.PickSpeed.ToString();
                txtPlaceSpeed.Text = Robot1data.PlaceSpeed.ToString();
                txtDelayPick.Text = Robot1data.DelayPick.ToString();
                txtDelayPlace.Text = Robot1data.DelayPlace.ToString();
                txtAbove.Text = Robot1data.RobotAbove.ToString();
                txtAbovePick.Text = Robot1data.RobotAbovePick.ToString();
                txtAbovePlace.Text = Robot1data.RobotAbovePlace.ToString();
                // if (Robot1data.InvLock == 1) chkInverseLock.Checked = true; else chkInverseLock.Checked = false;
                if (Robot1data.CheckGrip == 1) chkCheckGripper.Checked = true; else chkCheckGripper.Checked = false;
                cmbGripper.Text = Robot1data.Gripper;
                string[] files = new string[1];
                GetGrippers(ref files);
                cmbGripper.Items.Clear();
                for (int i = 0; i < files.Length; i++) { if (files[i] != null) cmbGripper.Items.Add(files[i]); }
                cmbSelectTeachGripper.Items.Clear();
                cmbSelectTeachGripper.Text = "";
                for (int i = 0; i < files.Length; i++) { if (files[i] != null) cmbSelectTeachGripper.Items.Add(files[i]); }
            }

        }
        //static public frmMain newFrmMain = new frmMain();
        private void TabControl2_DrawItem(object sender, DrawItemEventArgs e)
        {
            //var g = e.Graphics;
            //tabPage10.Text = "ROBOT" + '\r' + "SETUP";
            //var text = tabPage10.Text;// this.tabControl2.TabPages[10].Text;
            //var sizeText = g.MeasureString(text, this.tabControl2.Font);

            //var x = e.Bounds.Left + 3;
            //var y = e.Bounds.Top + (0 - sizeText.Height) / 2;

            //g.DrawString(text, this.tabControl2.Font, Brushes.Black, x, y);

        }

        private void TabPage10_Click(object sender, EventArgs e)
        {

        }
        DateTime NowTime = DateTime.Now;
        private async void BtnCycleStart_Click(object sender, EventArgs e)
        {
            //return;//in debug
            System.Console.Beep();
            System.Media.SystemSounds.Beep.Play();
            chkStep.Checked = false;
            inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
            FanucSpeed = trackSpeed.Value;
            ErrorMess = "";
            try
            {
                DialogResult res = MessageBox.Show("Start Cycle?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (res != DialogResult.Yes)
                {
                    btnCycleStart.Enabled = true;
                    return;
                }

                if (trackBarSpeedSt.Value < 70) { trackBarSpeedSt.Value = 70; txtSpeedSt.Text = "70"; }
                //MyStatic.bExitcycle = false;
                //MyStatic.bExitMaincycle = false;
                //MyStatic.bEmpty = false;
                btnCycleStop.BackColor = Color.LightGray;
                //while (!MyStatic.bExitMaincycle && !MyStatic.bExitMaincycle)
                //{
                //start cycle     
                ControlsEnable(false);
                MyStatic.bExitcycle = false;
                MyStatic.bExitcycleNow = false;
                MyStatic.bEmpty = false;
                MyStatic.bOneCycle = false;
                cntLongPart = 0;
               
                nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
                nFrontCountUpDwn = (int)upDwnCount.UpDownValue;
                nColorUpDwn = (int)upDwnColor.UpDownValue;
                DeltaFront = 0;
                FrontRotate = Single.Parse(txtFrontRotate.Text);

                var task = Task.Run(() => Task.Run(() => RunMain()));
                await task;
                if (!task.Result)
                {
                    MessageBox.Show("RUN CYCLE Stopped");
                    return;
                }
                if (MyStatic.bExitcycleNow)
                {
                    MessageBox.Show("EXIT CYCLE WITH ERROR " + "\r\n" + ErrorMess);
                    return;
                }
                Thread.Sleep(1000);
                //}
            }
            catch (Exception err)
            {
                MessageBox.Show("RUN CYCLE ERROR:" + err.Message);
            }
        }
        //MyStatic.RobotAction RobotLoadAct = new MyStatic.RobotAction();
        ////MyStatic.RobotAction InspectStationAct = new MyStatic.RobotAction();

        ////MyStatic.CncStation cncSation = new MyStatic.CncStation();
        //MyStatic.PartData[] partData = null;
        //MyStatic.VisionAction InspectStationAct = new MyStatic.VisionAction();

        public const int Robot2cmdOffset = 100;
        Task TaskRobot = null;
        Task TaskVision = null;
        Task TaskVisionFront = null;
        Task TaskRefresh = null;
        Task TaskSua = null;
        Task TaskWeldon = null;
        public bool bPartInFooter = false;
        public string ErrorMess = "";
        private async Task<bool> RunMain()
        {
            Single t1 = 0;
            RobotLoadAct.InAction = false;
            InspectStationAct.InAction = false;
            InspectStationAct.VisionInAction = false;
            InspectStationAct.VisionFrontInAction = false;
            InspectStationAct.SuaInAction = false;
            FooterStationAct.AxisInAction = false;
            //MyStatic.bExitcycleNow = false;
            InspectStationAct.WeldonInAction = false;
            bInspectLongPart = false;
            MyStatic.InitFini = false;
            FrontRotate = Single.Parse(txtFrontRotate.Text);
            DeltaFront = 0;
            ErrorFront = 0;



            try
            {
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0

                Task.Run(() => frmMain.newFrmMain.ListAdd3("=========Start Cycle============" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                dFile.WriteLogFile("===========Start Cycle==============");
                if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                {
                    MessageBox.Show("ERROR IN PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    return false;
                }
                //check data
                if(chkDiam.Checked && txtDtolerance.Text =="" && Single.Parse(txtDtolerance.Text)>0.2)
                {
                    MessageBox.Show("CHECK DIAMETER TOLERANCE!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    return false;
                }
                //check robot
                var task1 = Task.Run(() => CheckComm());
                await task1;
                RobotFunctions.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    return false;
                }
                //send data to robot
                var task = Task.Run(() => SendData());
                await task;
                reply = task.Result;
                if (!reply.result)
                {

                    MessageBox.Show("ERROR SEND DATA TO ROBOT!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return false;
                }
                //home
                var task2 = Task.Run(() => RobotHome());
                await task2;

                bool rep1 = task2.Result;

                if (!rep1)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Robot HOME Error!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return false;
                }
                //check communication with vision
                //if (!chkVisionSim.Checked)
                //{
                    WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                    var task3 = Task.Run(() => CheckComm1(1));
                    await task3;
                    WebComm.CommReply rep2 = task3.Result;
                    inv.set(btnRead, "Enabled", true);
                    if (!rep2.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("Error Vision Communication!", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        ControlsEnable(true);
                        return false;
                    }
                    WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                    var task31 = Task.Run(() => CheckComm1(2));
                    await task31;
                    WebComm.CommReply rep21 = task31.Result;
                    inv.set(btnRead, "Enabled", true);
                    if (!rep21.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("Error Front cam Communication!", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        ControlsEnable(true);
                        return false;
                    }
                    //cognex
                    WC3.SetControls1(txtClient, this, null, "CognexComm", CognexAddr);
                    var task32 = Task.Run(() => CheckComm1(3));
                    await task32;
                    rep21 = task32.Result;
                    inv.set(btnRead, "Enabled", true);
                    if (!rep21.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("Error Cognex Communication!", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        ControlsEnable(true);
                        return false;
                    }
                //}
                //disconnect vision "Ethernet 2" Iscar
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<=  disconnect Iscar Ethernet" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                var task35 = Task.Run(() => VisionIscarPort(0));
                await task35;
                if (!task35.Result) { Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Error disconnect Iscar Ethernet" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false)); }
                //save vision data 
                var task33 = Task.Run(() => DataVisionSave());
                await task33;
                WebComm.CommReply reply33 = task33.Result;
                if (!reply33.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Error Save Vision Data!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return false;
                }
                var task34 = Task.Run(() => DataFrontSave());
                await task34;
                reply33 = task34.Result;
                if (!reply33.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Error save front data!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return false;
                }
                //set lights
                CommReply rep = new CommReply();
                
                //check tray
                int TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
                int TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);
                if (TrayPartId >= TrayInsertsOnX * TrayInsertsOnY)
                
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Place Tray3 " + TrayPartId.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("END OF TRAY", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    MyStatic.bExitcycle = true;
                    EndOfTray = true;
                    RobotLoadAct.InAction = false;
                    return false;
                }
                else EndOfTray = false;
                //check beckhoff
                var task12 = Task.Run(() => RunAxisStatus(6));
                await task12;
                rep = task12.Result;
                if (!rep.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR READ BECKHOFF STATUS " + "\r");
                    return false;
                }
                if (rep.data[8] == 0)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR AIR PRESSURE " + "\r");
                    return false;
                }
                if (rep.data[9] == 0)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR MOTORS POWER " + "\r");
                    return false;
                }
                //move footer home position
                FooterStationAct.AxisInAction = true;
                Single speed5 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed4 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[3].Ax_Vmax) / 100.0f;
                var task6 = Task.Run(() => MoveFooterHome(speed5, speed4));
                await task6;
                if (!task6.Result.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER TO HOME POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    MessageBox.Show("ERROR MOVE FOOTER TO HOME POSITION! Exit cycle", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    return false;
                }
                else
                {
                    //footer with part
                    if (task6.Result.data.Length >= 11 && task6.Result.data[7] == 1) bPartInFooter = true; else bPartInFooter = false;
                }
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InHome;

                //main loop
                while (true)
                {
                    SetTraficLights(1, 0, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    Thread.Sleep(20);

                    if (MyStatic.bReset)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========RESET Cycle============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        return false;
                    }
                    if (MyStatic.bExitcycleNow && !RobotLoadAct.InAction && !InspectStationAct.VisionInAction && !InspectStationAct.SuaInAction)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========Exit Cycle WITH ERROR============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        return false;
                    }
                    if (FooterStationAct.State == (int)MyStatic.E_State.InError)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========Footer Error!============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        return false;
                    }

                    if ((!RobotLoadAct.InAction && !MyStatic.bReset) || TaskRobot == null || TaskRobot.Status != TaskStatus.Running) { TaskRobot = Task.Run(() => RobotMainTask()); }
                    if ((!InspectStationAct.VisionInAction && !MyStatic.bReset) || TaskVision == null || TaskVision.Status != TaskStatus.Running) { TaskVision = Task.Run(() => VisionMainTask()); }
                    if ((!InspectStationAct.SuaInAction && !MyStatic.bReset) || TaskSua == null || TaskSua.Status != TaskStatus.Running) { TaskSua = Task.Run(() => CognexMainTask()); }
                    if ((!InspectStationAct.WeldonInAction && !MyStatic.bReset) || TaskWeldon == null || TaskWeldon.Status != TaskStatus.Running) { TaskWeldon = Task.Run(() => WeldonMainTask()); }
                    if (!MyStatic.bReset && (TaskRefresh == null || (TaskRefresh.Status != TaskStatus.Running))) TaskRefresh = Task.Run(() => RefreshLables());
                    if ((!InspectStationAct.VisionFrontInAction && !MyStatic.bReset) || TaskVisionFront == null || TaskVisionFront.Status != TaskStatus.Running) { TaskVisionFront = Task.Run(() => VisionFrontMainTask()); }

                    if (MyStatic.bReset)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========RESET Cycle============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        return false;
                    }


                    //await Task.WhenAll(task1, task2,task3);

                    Thread.Sleep(100);
                    //if (MyStatic.bExitcycle) break;
                    if (MyStatic.bExitcycle &&
                    FooterStationAct.State == (int)MyStatic.E_State.InHome && !RobotLoadAct.InAction) break;
                    //if (MyStatic.bExitcycleNow && !RobotLoadAct.InAction) break;
                    if (MyStatic.bExitcycleNow) break;
                    if (EndOfTray && FooterStationAct.State == (int)MyStatic.E_State.InHome &&
                        InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty &&
                        RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Empty &&
                        RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Empty)
                    {
                        MyStatic.bExitcycle = true;
                        break;
                    }




                }
                Task.Run(() => frmMain.newFrmMain.ListAdd3("EXIT CYCLE" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                MyStatic.bRobotLoadRunning = false;
                //MyStatic.bExitcycleNow = true;
                MyStatic.bExitcycle = true;
                //home
                var task4 = Task.Run(() => RobotHome());
                await task4;
                rep1 = task2.Result;

                if (!rep1)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Robot HOME Error!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    RefreshLables();
                    return false;
                }
                //ControlsEnable(true);
                MyStatic.bStartcycle = false;
                Task.Run(() => newFrmMain.ListAdd3("==========Stop Cycle============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));

                RefreshLables();
                if (EndOfTray)
                {
                    SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    newFrmMain.ListAdd3("==========End of tray. Exit============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false);
                    //MessageBox.Show("End Of Tray!", "ERROR", MessageBoxButtons.OK,
                    //      MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    //Thread.Sleep(1000);
                    TrayPartId = 0;
                    ControlsEnable(true);
                    return true;

                }
                ControlsEnable(true);
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                return true;

            }
            catch (Exception e)
            {
                SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                MessageBox.Show("ERROR MAIN CYCLE:" + e.Message); return false;
            }


        }

        public bool bInspectLongPart = false;
        int cntLongPart = 0;
        private async void VisionMainTask()
        {
            //MyStatic.E_State.Occupied - part in footer
            //MyStatic.E_State.PartReady - snaps for sua ready
            //MyStatic.E_State.SuaReady  - sua finished
            //MyStatic.E_State.WeldonReady - weldone finished
            //MyStatic.E_State.DiamReady - diameter finished
            try
            {
                if (InspectStationAct.VisionInAction) return;
                if (MyStatic.bReset) return;
                if (FooterStationAct.State == (int)MyStatic.E_State.InError) return;
                if (MyStatic.bExitcycleNow) return;

                //-------------inspect cam2 top -----------------------
                 
                if (InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Occupied &&
                    !InspectStationAct.VisionInAction &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] != (int)MyStatic.E_State.TopSnapFini &&
                    //!chkVisionSim.Checked &&
                    !FooterStationAct.AxisInAction)
                //&& 
                //!InspectStationAct.SuaInAction)
                {
                    inv.settxt(lblInspect, "Vision Inspect");
                    DeltaFront = 0;
                    ErrorFront = 0;
                    InspectStationAct.VisionInAction = true;
                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionTop] = false;
                    if (txtPartLength.Text != "" && Single.Parse(txtPartLength.Text) - master.Length > 0)
                    {
                        bInspectLongPart = false;// true;//long part
                    }
                    else bInspectLongPart = false;
                    //move footer to work top
                    int axis = 0;
                    Single Pos5 = 0;
                    if (bInspectLongPart && cntLongPart > 0)
                    {
                         
                        Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length + LengthLUmax - Single.Parse(txtPartLengthU.Text));
                    }
                    else
                    {
                        Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length); 
                    }
                    Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                    Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                    //lamp1
                    BitArray lamp = new BitArray(new bool[8]);
                    lamp[0] = true; //lamp1
                    byte[] Lamps = new byte[1];
                    lamp.CopyTo(Lamps, 0);
                    MyStatic.InitFini = false;

                    Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                    Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                    if (FooterStationAct.AxisInAction) { InspectStationAct.VisionInAction = false; return; }
                    FooterStationAct.AxisInAction = true;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff<= Move work" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                    //-------vision init cycle-------------
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Init Cycle" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    MyStatic.InitFini = false;
                    var task0 = Task.Run(() => InitCycle());
                    await task0;
                    WebComm.CommReply reply = task0.Result;

                    if (!reply.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Error Init Cycle" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionTop] = true;

                        InspectStationAct.VisionInAction = false;
                        MyStatic.bExitcycleNow = true;
                        return;
                    }
                    MyStatic.InitFini = true;
                    //front camera off
                    var task01 = Task.Run(() => FrontCamOnOff(0));
                    await task01;
                    reply = task01.Result;

                    if (!reply.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error1 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                        InspectStationAct.VisionInAction = false;
                        MyStatic.bExitcycleNow = true;
                        return;
                    }
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> fini init cycle" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    //fini vision init cyccle
                    await task;
                    if (!task.Result.result)
                    {

                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER TO WORK POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        InspectStationAct.VisionInAction = false;
                        MyStatic.bReset = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ERROR MOVE FOOTER TO WORK!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        inv.settxt(lblInspect, "Vision Inspect Error Move");
                        MessageBox.Show("ERROR MOVE FOOTER TO WORK POSITION! Exit cycle", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        return;
                    }
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> fini Move work" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    if (task.Result.data.Length >= 9 && task.Result.data[8] != 1)
                    {
                        ErrorMess = "NO PART IN FOOTER " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        MyStatic.bExitcycleNow = true;
                        MyStatic.bExitcycle = true;
                        Stopwatch sw1 = new Stopwatch();
                        sw1.Restart();
                        while (RobotLoadAct.OnGrip2_State != (int)(int)MyStatic.E_State.Empty || sw1.ElapsedMilliseconds < 10000) { Thread.Sleep(500); }
                        Thread.Sleep(1000);

                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> NO PART IN FOOTER" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        ErrorMess = "NO PART IN FOOTER " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        ClearStations();
                        //InspectStationAct.Reject[(int)Reject.Beckhoff] = true;
                        MyStatic.bExitcycleNow = true;
                        InspectStationAct.VisionInAction = false;
                        return;
                    }
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InWork;
                   
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Run Inspect" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    bool withcognex = false;// true;
                    
                       
                    var task1 = Task.Run(() => StartCycleInspectVision(withcognex));
                    await task1;
                    reply = task1.Result;

                    if (reply.result)
                    {
                        inv.settxt(lblInspect, "Vision Inspect Fini");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Snaps for Inspect Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        if (reply.comment != null && reply.comment.StartsWith("cmd"))
                        {
                            string s = reply.comment.Remove(0, 3);
                            string[] ss = s.Split(',');
                            if (ss.Length >= 3 && ss[0] != ((int)MyStatic.InspectCmd.Startvision).ToString())
                            {
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Top Error1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionTop] = true;

                                InspectStationAct.VisionInAction = false;
                                return;

                            }
                            if (ss.Length >= 3 && ss[1] != "1")
                            {
                                inv.settxt(lblInspect, "Vision Top Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Top Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionTop] = true;

                                InspectStationAct.VisionInAction = false;
                                return;

                            }
                            else
                            {
                                //vision ok
                                inv.settxt(lblInspect, "Vision Top Ready");
                                //InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                                //InspectStationAct.SuaState = (int)MyStatic.E_State.PartReady;

                                InspectStationAct.VisionInAction = false;
                                //long part
                                if (bInspectLongPart && cntLongPart == 0)
                                {
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Long part inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    cntLongPart = 1;
                                    //inspection finished
                                    if(InspectStationAct.SuaState == (int)MyStatic.E_State.SuaFini &&
                                    (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.SuaFini ||
                                     InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] == true) &&  !InspectStationAct.SuaInAction)
                                    {
                                 
                                        InspectStationAct.VisionInAction = false;
                                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.Occupied;
                                        return;
                  
                                    }
                                    //next top vision
                                    
                                }
                                else
                                {
                                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                                    InspectStationAct.SuaState = (int)MyStatic.E_State.PartReady;
                                    InspectStationAct.VisionInAction = false;
                                }
                                return;
                            }

                        }
                        else
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Error3" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionTop] = true;

                            InspectStationAct.VisionInAction = false;
                            return;
                        }

                    }
                    else
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Inspect Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] = (int)MyStatic.E_State.TopSnapFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionTop] = true;

                        InspectStationAct.VisionInAction = false;
                    }

                }


                //-------------diameter cam1 after inspection-----------------------
                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] != (int)MyStatic.E_State.DiamFini &&
                    !InspectStationAct.VisionInAction &&
                    !chkDiam.Checked &&
                    !FooterStationAct.AxisInAction)
                {
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                    InspectStationAct.VisionInAction = false;
                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = false;
                    return;
                }
                

                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] != (int)MyStatic.E_State.DiamFini &&
                    !InspectStationAct.VisionInAction &&
                    chkDiam.Checked &&
                    !FooterStationAct.AxisInAction)
                {

                    InspectStationAct.VisionInAction = true;
                    inv.settxt(lblInspect, "Vision Diameter");
                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = false;
                    Thread.Sleep(100);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Run Inspect Diameter" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //if (FooterStationAct.AxisInAction) { InspectStationAct.VisionInAction = false; return; }
                    var task1 = Task.Run(() => StartCycleInspectDiam());
                    await task1;
                    WebComm.CommReply reply = task1.Result;

                    if (reply.result)
                    {
                        inv.settxt(lblInspect, "Vision Diameter Fini");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Diameter Inspect Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        if (reply.comment != null && reply.comment.StartsWith("cmd"))
                        {
                            string s = reply.comment.Remove(0, 3);
                            string[] ss = s.Split(',');
                            if (ss.Length >= 3 && ss[0] != ((int)MyStatic.InspectCmd.CheckDiam).ToString())
                            {
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=>Diameter Vision Error1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                                InspectStationAct.VisionInAction = false;
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;
                                return;

                            }
                            if (ss.Length >= 3 && ss[1] != "1")
                            {
                                inv.settxt(lblInspect, "Vision Diameter Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Diameter Vision Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;
                                InspectStationAct.VisionInAction = false;
                                return;

                            }
                            else
                            {
                                inv.settxt(lblInspect, "Vision Diameter Ready");
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;

                                InspectStationAct.VisionInAction = false;
                                return;
                            }

                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Diameter Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Error5" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;

                            InspectStationAct.VisionInAction = false;
                            return;
                        }

                    }
                    else
                    {
                        inv.settxt(lblInspect, "Vision Diameter Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Inspect Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;
                        InspectStationAct.VisionInAction = false;
                        return;
                    }

                }
                //-------------front cam2 after diam and color same time-----------------------
                /*
                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] != (int)MyStatic.E_State.FrontSnapFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                    !InspectStationAct.VisionInAction &&
                    !chkFront.Checked &&
                    !FooterStationAct.AxisInAction)
                {
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                    InspectStationAct.VisionInAction = false;
                    return;
                }
                */
                /*
                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] != (int)MyStatic.E_State.FrontSnapFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                    !InspectStationAct.VisionInAction &&
                    chkFront.Checked &&
                    !FooterStationAct.AxisInAction)
                {

                    InspectStationAct.VisionInAction = true;
                    inv.settxt(lblInspect, "Vision Front");
                    nfrontCount = 0;
                    Thread.Sleep(300);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Run Front snap" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    var task1 = Task.Run(() => StartCycleInspectFront());
                    //-----------check color if selected----------------------
                    
                    if (chkColor.Checked && InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] != (int)MyStatic.E_State.ColorFini)
                    {
                        InspectStationAct.VisionInAction = true;
                       
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Run Check Color" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                            var task2 = Task.Run(() => CheckColor());
                            await task2;
                            WebComm.CommReply reply2 = task2.Result;

                            if (reply2.result)
                            {
                                inv.settxt(lblInspect, "Vision Check Color Fini");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Check Color Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                inv.settxt(lblInspect, "Vision Check Color Ready");
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] = (int)MyStatic.E_State.ColorFini;
                               

                            }
                            else
                            {
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] = (int)MyStatic.E_State.ColorFini;
                                InspectStationAct.Reject[(int)Reject.VisionColor] = true;
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Check Color Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                            }
                       
                        
                    }

                    //fini check color
                    await task1;
                    WebComm.CommReply reply = task1.Result;

                    if (reply.result)
                    {
                        inv.settxt(lblInspect, "Vision Front Fini");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Snap Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        if (reply.comment != null && reply.comment.StartsWith("cmd"))
                        {
                            string s = reply.comment.Remove(0, 3);
                            string[] ss = s.Split(',');
                            if (ss.Length >= 4 && ss[0] == ((int)MyStatic.InspectCmd.FrontCamCount).ToString() && ss[1] == "1")
                            {
                                inv.settxt(lblInspect, "Vision Front Ready");
                                nfrontCount = int.Parse(ss[2]);
                                InspectStationAct.VisionInAction = false;
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Vision Count=" + nfrontCount.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                partData[InspectStationAct.OnFooterGrip3_PartID].Count = nfrontCount;
                                if (nfrontCount == upDwnCount.UpDownValue)
                                {
                                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                    InspectStationAct.Reject[(int)Reject.VisionCount] = false;
                                    InspectStationAct.VisionInAction = false;
                                    return;
                                }
                                else
                                {
                                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                    InspectStationAct.Reject[(int)Reject.VisionCount] = true;
                                    InspectStationAct.VisionInAction = false;
                                    return;
                                }

                               

                            }
                            else
                            {
                                inv.settxt(lblInspect, "Vision Front Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Vision Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;

                                InspectStationAct.Reject[(int)Reject.VisionFront] = true;
                                InspectStationAct.VisionInAction = false;
                                return;

                            }
                            

                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Front Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Error5" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                            InspectStationAct.Reject[(int)Reject.VisionFront] = true;

                            InspectStationAct.VisionInAction = false;
                            return;
                        }

                    }
                    else
                    {
                        inv.settxt(lblInspect, "Vision Front Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;

                        InspectStationAct.Reject[(int)Reject.VisionFront] = true;
                        InspectStationAct.VisionInAction = false;
                        return;
                    }

                }
                */
                // -------------check color ------------------------
                
                if //(InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] == (int)MyStatic.E_State.WeldonFini &&
                  (InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                   //InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini &&
                   InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                    !InspectStationAct.VisionInAction &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] != (int)MyStatic.E_State.ColorFini)
                    //&&
                    //!chkFront.Checked)
                {
                    InspectStationAct.VisionInAction = true;
                    if (chkColor.Checked)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Run Check Color" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionColor] = false;
                        var task1 = Task.Run(() => CheckColor());
                        await task1;
                        WebComm.CommReply reply = task1.Result;

                        if (reply.result)
                        {
                            inv.settxt(lblInspect, "Vision Check Color Fini");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Check Color Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            inv.settxt(lblInspect, "Vision Check Color Ready");
                            InspectStationAct.VisionInAction = false;
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] = (int)MyStatic.E_State.ColorFini;
                            InspectStationAct.VisionInAction = false;
                            return;

                        }
                        else
                        {
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] = (int)MyStatic.E_State.ColorFini;
                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionColor] = true;
                            InspectStationAct.VisionInAction = false;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Check Color Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                        }
                    }
                    else
                    {
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] = (int)MyStatic.E_State.ColorFini;
                        InspectStationAct.VisionInAction = false;
                    }
                }
                if //(InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] == (int)MyStatic.E_State.WeldonFini &&
                 (InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                  //InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini &&
                  InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                   !InspectStationAct.VisionInAction &&
                   InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] != (int)MyStatic.E_State.ColorFini &&
                   !chkColor.Checked)
                {
                    InspectStationAct.VisionInAction = true;
                    
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] = (int)MyStatic.E_State.ColorFini;
                        InspectStationAct.VisionInAction = false;
                    
                }
                //move footer home after vision fiished
                //move footer to home
                if (InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] == (int)MyStatic.E_State.WeldonFini &&
                InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini &&
                InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                InspectStationAct.State[(int)MyStatic.InspectSt.VisionColor] == (int)MyStatic.E_State.ColorFini &&
                !InspectStationAct.VisionInAction &&
                !InspectStationAct.VisionFrontInAction &&
                 !FooterStationAct.AxisInAction &&
                 FooterStationAct.State == (int)MyStatic.E_State.InWork)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Move Footer Home" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    int axis = 0;
                    Single speed5 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                    Single speed4 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[3].Ax_Vmax) / 100.0f;
                    InspectStationAct.VisionInAction = true;
                    FooterStationAct.AxisInAction = true;
                    var task2 = Task.Run(() => MoveFooterHome(speed5, speed4));
                    await task2;
                    if (!task2.Result.result)
                    {
                        inv.settxt(lblInspect, "Vision Error Move Home");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER TO HOME POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                        //InspectStationAct.State = (int)MyStatic.E_State.FrontSnapFini;

                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        InspectStationAct.VisionInAction = false;
                        MyStatic.bReset = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        MessageBox.Show("ERROR MOVE FOOTER TO HOME POSITION! Exit cycle", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        InspectStationAct.VisionInAction = false;
                        return;
                    }
                    else
                    {
                        //footer with part
                        if (task2.Result.data.Length >= 11 && task2.Result.data[7] == 1) bPartInFooter = true; else bPartInFooter = false;
                    }
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Footer In Home" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InHome;
                    InspectStationAct.VisionInAction = false;
                    return;
                }

                //run robot pick from inspect
            }
            catch (Exception ex)
            {
                MyStatic.bExitcycleNow = true;
                frmMain.newFrmMain.ListAdd3("Vision=> ERROR " + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false);
                ErrorMess = "Vision=> ERROR " + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                MyStatic.bExitcycle = true;
                MyStatic.bExitcycleNow = true;

            }
        }
        int ErrorFront = 0;
        int ErrorFrontMax = 2;
        Single FrontRotate = 0;
         
        private async void VisionFrontMainTask()
        {
            
            try
            {
                if (InspectStationAct.VisionFrontInAction) return;
                if (MyStatic.bReset) return;
                if (FooterStationAct.State == (int)MyStatic.E_State.InError) return;
                if (MyStatic.bExitcycleNow) return;
                bool countOK = false;

               
                //-------------front cam1 after diam -----------------------
                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] != (int)MyStatic.E_State.FrontSnapFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                    !InspectStationAct.VisionFrontInAction &&
                    !chkFront.Checked &&
                    !chkInspectFront.Checked &&
                    !FooterStationAct.AxisInAction)
                {
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                    InspectStationAct.VisionFrontInAction = false;
                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = false;
                    DeltaFront = 0;
                    ErrorFront = 0;
                    return;
                }

                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] != (int)MyStatic.E_State.FrontSnapFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                    !InspectStationAct.VisionFrontInAction &&
                    (chkFront.Checked || chkInspectFront.Checked) &&
                    !FooterStationAct.AxisInAction)
                {

                    InspectStationAct.VisionFrontInAction = true;
                    inv.settxt(lblInspect, "Vision Front");
                    nfrontCount = 0;
                    //camera1 on
                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = false;
                    var task10 = Task.Run(() => FrontCamOnOff(1));
                    await task10;
                    WebComm.CommReply reply10 = task10.Result;
                    if (!reply10.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Error Front camera ON" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                        InspectStationAct.VisionInAction = false;
                        MyStatic.bExitcycleNow = true;
                        return;
                    }
                    Thread.Sleep(300);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Run Front1 " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    //-----------first time---------------------
                    //-----count front---------------
                    if (chkFront.Checked)
                    {
                        DeltaFront = 0;
                        var task1 = Task.Run(() => StartCycleInspectFront(DeltaFront, 1));
                        await task1;
                        WebComm.CommReply reply = task1.Result;

                        if (reply.result)
                        {
                            inv.settxt(lblInspect, "Vision Front Fini");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front1 Snap Fini" + "delta=" + DeltaFront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            if (reply.comment != null && reply.comment.StartsWith("cmd"))
                            {
                                string s = reply.comment.Remove(0, 3);
                                string[] ss = s.Split(',');
                                if (ss.Length >= 4 && ss[0] == ((int)MyStatic.InspectCmd.FrontCamCount).ToString() && ss[1] == "1")
                                {
                                    inv.settxt(lblInspect, "Vision Front1 Ready");
                                    nfrontCount = int.Parse(ss[2]);
                                    //InspectStationAct.VisionFrontInAction = false;
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front1 Vision Count=" + nfrontCount.ToString() + " delta=" + DeltaFront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    partData[InspectStationAct.OnFooterGrip3_PartID].Count = nfrontCount;
                                    if (nfrontCount == nFrontCountUpDwn)
                                    {
                                        
                                        DeltaFront = 0;
                                        ErrorFront = 0;
                                        countOK = true;
                                        //return;
                                    }




                                }
                                else
                                {
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front1 Vision Error1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                                }


                            }


                        }
                        else
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front1 Vision Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                        }
                    }
                    //------first time snap front
                    else if (chkInspectFront.Checked)
                    {
                        DeltaFront = 0;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Run Front1 snap" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        var task30 = Task.Run(() => StartSnapFront(DeltaFront, 1));
                        await task30;
                        WebComm.CommReply reply2 = task30.Result;

                        if (reply2.result)
                        {
                            inv.settxt(lblInspect, "Vision Front1 Fini");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front1 Snap Fini" + "delta=" + DeltaFront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                            DeltaFront = 0;
                            ErrorFront = 0;
                            countOK = true;
                            //return;

                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Front Snap1 Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front1 snap Error Reject" + "delta=" + DeltaFront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                                       
                            InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                            countOK = true;
                           

                        }
                    }
                    
                    //-------------------------second time-----------------------
                    Thread.Sleep(200);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Run Front2 " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (master.Ax4_Front + DeltaFront > 250) DeltaFront = DeltaFront - FrontRotate;//rotate 90 deg
                    else DeltaFront = DeltaFront + FrontRotate;
                    Single deltafront = DeltaFront;
                    if (!countOK)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Run Front2 count+snap" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        var task2 = Task.Run(() => StartCycleInspectFront(deltafront, 2));
                        await task2;
                        WebComm.CommReply reply2 = task2.Result;

                        if (reply2.result)
                        {
                            inv.settxt(lblInspect, "Vision Front Fini");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 Snap Fini " + "delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //if (countOK)
                            //{
                            //    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                            //    InspectStationAct.Reject[(int)Reject.VisionCount] = false;
                            //    InspectStationAct.VisionFrontInAction = false;
                            //    //DeltaFront = 0;
                            //    ErrorFront = 0;
                            //    //camera2 off
                            //    //front camera off
                            //    var task20 = Task.Run(() => FrontCamOnOff(0));
                            //    await task20;
                            //    WebComm.CommReply reply20 = task10.Result;
                            //    return;
                            //}

                            if (reply2.comment != null && reply2.comment.StartsWith("cmd"))
                            {
                                string s = reply2.comment.Remove(0, 3);
                                string[] ss = s.Split(',');
                                if (ss.Length >= 4 && ss[0] == ((int)MyStatic.InspectCmd.FrontCamCount).ToString() && ss[1] == "1")
                                {
                                    inv.settxt(lblInspect, "Vision Front Ready");
                                    nfrontCount = int.Parse(ss[2]);
                                    //InspectStationAct.VisionFrontInAction = false;
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 Vision Count=" + nfrontCount.ToString() + " delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    partData[InspectStationAct.OnFooterGrip3_PartID].Count = nfrontCount;
                                    if (nfrontCount == nFrontCountUpDwn)
                                    {
                                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionCount] = false;
                                        InspectStationAct.VisionFrontInAction = false;
                                        //DeltaFront = 0;
                                        ErrorFront = 0;
                                        //camera1 off
                                        //front camera off
                                        Thread.Sleep(200);
                                        var task30 = Task.Run(() => FrontCamOnOff(0));
                                        await task30;
                                        WebComm.CommReply reply30 = task10.Result;
                                        if (!reply30.result)
                                        {
                                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error2 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                            InspectStationAct.VisionInAction = false;
                                            MyStatic.bExitcycleNow = true;
                                            return;
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 Vision count Error1 " + "delta=" + DeltaFront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionCount] = true;
                                        InspectStationAct.VisionFrontInAction = false;
                                        ErrorFront = 0;
                                        //camera1 off
                                        //front camera off
                                        Thread.Sleep(200);
                                        var task40 = Task.Run(() => FrontCamOnOff(0));
                                        await task40;
                                        WebComm.CommReply reply40 = task40.Result;
                                        if (!reply40.result)
                                        {
                                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error3 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                            InspectStationAct.VisionInAction = false;
                                            MyStatic.bExitcycleNow = true;
                                            return;
                                        }
                                        return;

                                    }



                                }
                                else
                                {
                                    inv.settxt(lblInspect, "Vision Front Reject");
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 Vision Error2 " + "delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;

                                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                    //InspectStationAct.Reject[(int)Reject.VisionCount] = true;
                                    InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                                    InspectStationAct.VisionFrontInAction = false;
                                    ErrorFront = 0;
                                    //camera1 off
                                    //front camera off
                                    Thread.Sleep(200);
                                    var task50 = Task.Run(() => FrontCamOnOff(0));
                                    await task50;
                                    WebComm.CommReply reply50 = task50.Result;
                                    if (!reply50.result)
                                    {
                                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error4 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                        InspectStationAct.VisionInAction = false;
                                        MyStatic.bExitcycleNow = true;
                                        return;
                                    }
                                    return;



                                }


                            }
                            else
                            {
                                inv.settxt(lblInspect, "Vision Front Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Vision2 Error5" + "delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                //InspectStationAct.Reject[(int)Reject.VisionCount] = true;
                                InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                                InspectStationAct.VisionFrontInAction = false;
                                ErrorFront = 0;
                                //camera1 off
                                //front camera off
                                Thread.Sleep(200);
                                var task60 = Task.Run(() => FrontCamOnOff(0));
                                await task60;
                                WebComm.CommReply reply60 = task60.Result;
                                if (!reply60.result)
                                {
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error5 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                    InspectStationAct.VisionInAction = false;
                                    MyStatic.bExitcycleNow = true;
                                    return;
                                }
                                return;

                            }

                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Front Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 Reject " + "delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;

                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                            //InspectStationAct.Reject[(int)Reject.VisionCount] = true;
                            InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                            InspectStationAct.VisionFrontInAction = false;
                            ErrorFront = 0;
                            //camera1 off
                            //front camera off
                            Thread.Sleep(200);
                            var task70 = Task.Run(() => FrontCamOnOff(0));
                            await task70;
                            WebComm.CommReply reply70 = task70.Result;
                            if (!reply70.result)
                            {
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error6 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                InspectStationAct.VisionInAction = false;
                                MyStatic.bExitcycleNow = true;
                                return;
                            }
                            return;

                        }
                    }
                    else
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Run Front2 snap" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        var task2 = Task.Run(() => StartSnapFront(deltafront,2));
                        await task2;
                        WebComm.CommReply reply2 = task2.Result;

                        if (reply2.result)
                        {
                            inv.settxt(lblInspect, "Vision Front Fini");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 Snap Fini" + "delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                           
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionCount] = false;
                                InspectStationAct.VisionFrontInAction = false;
                                DeltaFront = 0;
                                ErrorFront = 0;
                                //camera1 off
                                //front camera off
                                Thread.Sleep(200);
                                var task20 = Task.Run(() => FrontCamOnOff(0));
                                await task20;
                                WebComm.CommReply reply20 = task20.Result;
                                if (!reply20.result)
                                {
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error7 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                    InspectStationAct.VisionInAction = false;
                                    MyStatic.bExitcycleNow = true;
                                    return;
                                }
                                return;
                            
                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Front Snap Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front2 snap Error Reject" + "delta=" + deltafront.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;

                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                            //InspectStationAct.Reject[(int)Reject.VisionCount] = true;
                            InspectStationAct.Reject[(int)MyStatic.InspectSt.VisionFront] = true;
                            InspectStationAct.VisionFrontInAction = false;
                            ErrorFront = 0;
                            //camera1 off
                            //front camera off
                            Thread.Sleep(200);
                            var task70 = Task.Run(() => FrontCamOnOff(0));
                            await task70;
                            WebComm.CommReply reply70 = task70.Result;
                            if (!reply70.result)
                            {
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Error8 Front camera OFF" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.VisionFront] = true;

                                InspectStationAct.VisionInAction = false;
                                MyStatic.bExitcycleNow = true;
                                return;
                            }
                            return;

                        }
                    }
                }
               
               
            }
            catch (Exception ex)
            {
                MyStatic.bExitcycleNow = true;
                frmMain.newFrmMain.ListAdd3("VisionFront => ERROR " + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false);
                ErrorMess = "Vision Front=> ERROR " + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                MyStatic.bExitcycle = true;
                MyStatic.bExitcycleNow = true;

            }
        }

        private void CheckDeviceReply(WebComm.CommReply reply, string sErrorMessageToDisplay)
        {
            if (!reply.result || reply.status == "" || reply.status == null)
            {
                MessageBox.Show(sErrorMessageToDisplay + " \r" + reply.status);
                return;
            }

        }

        private void CheckDeviceReply(EndmillHMI.CommReply reply, string sErrorMessageToDisplay)
        {
            if (!reply.result || reply.status == "" || reply.status == null)
            {
                MessageBox.Show(sErrorMessageToDisplay + " \r" + reply.status);
                return;
            }

        }


        private async void CognexMainTask()
        {
            try
            {
                //if (InspectStationAct.VisionInAction) return;
                if (InspectStationAct.SuaInAction) return;
                if (MyStatic.bReset) return;
                if (MyStatic.bExitcycleNow) return;
                //int wasState = 0;
                //-------------inspect top-----------------------
                

                if (InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Occupied &&
                    FooterStationAct.State == (int)MyStatic.E_State.InWork &&
                    //!chkVisionSim.Checked &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] != (int)MyStatic.E_State.SuaFini &&
                !InspectStationAct.SuaInAction && MyStatic.InitFini)
                {
                    InspectStationAct.SuaInAction = true;
                    InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = false;
                    Thread.Sleep(100);
                    inv.settxt(lblInspect, "Run Cognex");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Run Top Inspect" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //InspectStationAct.VisionInAction = true;

                    //if (chkSuaSim.Checked)
                    //{
                    //    Thread.Sleep(2000);
                    //    InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                    //    inv.settxt(lblInspect, "Cognex Ready");
                    //    InspectStationAct.Reject[(int)Reject.SuaTop] = true;
                    //    InspectStationAct.SuaInAction = false;

                    //    inv.settxt(lblInspect, "Inspect Fini");
                    //    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Simulation Inspect Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //    return;
                    //}
                    var task1 = Task.Run(() => StartCycleInspectCognex());
                    await task1;
                    WebComm.CommReply reply = task1.Result;
                    if (reply.result)
                    {
                        inv.settxt(lblInspect, "Sua Fini");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect top Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        if (reply.comment != null && reply.comment.StartsWith("cmd"))
                        {
                            string s = reply.comment.Remove(0, 3);
                            string[] ss = s.Split(',');
                            if (ss.Length >= 4 && ss[0] != ((int)MyStatic.InspectCmd.StartCognex).ToString())
                            {
                                inv.settxt(lblInspect, "Cognex Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Sua Error1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                //wasState = InspectStationAct.State;
                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                                string status = reply.status;

                                InspectStationAct.SuaInAction = false;

                                return;

                            }
                            if (ss.Length >= 3 && ss[1] != "1")
                            {
                                inv.settxt(lblInspect, "Cognex Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Sua Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                                InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                                string status = reply.status;

                                InspectStationAct.SuaInAction = false;

                                return;

                            }
                            else if (ss.Length >= 4 && ss[1] == "1" && ss[3] == "0" &&  ss[2] == "0")
                            {
                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                                inv.settxt(lblInspect, "Cognex Ready");
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = false;
                                InspectStationAct.SuaInAction = false;
                                return;

                            }
                            else if (ss.Length >= 4 && ss[1] == "1" && (ss[3] != "0" || ss[2] != "0"))
                            {
                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                                inv.settxt(lblInspect, "Cognex Ready");
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Sua Reject Breaks=" + ss[2] +" Peels="+ ss[3] + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                InspectStationAct.SuaInAction = false;
                                return;

                            }
                            else
                            {
                                inv.settxt(lblInspect, "Cognex Reject");
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Sua Reject " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                                InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                                string status = reply.status;

                                InspectStationAct.SuaInAction = false;

                                return;

                            }

                        }

                        InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                        InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                        inv.settxt(lblInspect, "Cognex Ready");
                        InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = false;
                        InspectStationAct.SuaInAction = false;
                    }
                    else
                    {
                        inv.settxt(lblInspect, "Run Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //wasState = InspectStationAct.State;
                        InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;//ss[0]=break count,ss[1]=peels count
                        InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                        InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;


                        InspectStationAct.SuaInAction = false;
                    }
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Top Finished" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                }

                //-----------------front inspection--------------------------------------------------
                
                if (//chkInspectFront.Checked &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini &&
                    //!chkVisionSim.Checked &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] != (int)MyStatic.E_State.SuaFrontFini &&
                !InspectStationAct.SuaInAction)
                {
                    if (!chkInspectFront.Checked)
                    {
                        InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;
                        InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                        InspectStationAct.SuaInAction = false;
                        InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = false;
                        return;
                    }

                    //------------------inspect first front---------------
                    InspectStationAct.SuaInAction = true;
                    inv.settxt(lblInspect, "Run Cognex Front1");
                    InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = false;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Run Inspect Front1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
            
                    int bmpnum = 1;//bmp snap-inspect 2-snap-inspect_Prev
                    var task20 = Task.Run(() => InspectFront(bmpnum));
                    await task20;

                    WebComm.CommReply fini20 = task20.Result;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> got " + fini20.comment + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (!fini20.result)
                    {
                        //InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                        //InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = true;
                        //InspectStationAct.SuaInAction = false;
                        string status = fini20.comment;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front1 Finished Reject or error"  + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    }
                    else
                    {
                        string status = fini20.comment;
                        if (chkSuaNoReject.Checked) status = "0,0";//debug

                        string[] ss = status.Split(',');
                        // rep1.status = ss[2] + "," + ss[3];
                        if (ss.Length > 3 && int.Parse(ss[2]) == 0 && int.Parse(ss[3]) == 0)//ss[2] breaks; ss[3] peels
                        {
                            //InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;
                            //InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                            //InspectStationAct.SuaInAction = false;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front1 Finished" +  " breaks:"+ss[2]+ " peels:"+ss[3] + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        }
                        else
                        {
                            //InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;//ss[2]=break count,ss[3]=peels count
                            InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = true;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front1 Finished" + " breaks:" + ss[2] + " peels:" + ss[3] + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                            //InspectStationAct.SuaInAction = false;
                            //GetReject(RejectPartId, PartIndex, (int)MyStatic.E_State.Reject, "", status, "", fini1.comment, true);
                        }


                    }

                   



                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front1 Finished" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));


                    //------------------inspect second front---------------
                    InspectStationAct.SuaInAction = true;
                    inv.settxt(lblInspect, "Run Cognex Front2");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Run Inspect Front2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    bmpnum = 2;//bmp snap-inspect 2-snap-inspect_Prev
                    var task21 = Task.Run(() => InspectFront(bmpnum));
                    await task21;

                    WebComm.CommReply fini21 = task21.Result;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> got "+ fini21.comment + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (!fini21.result)
                    {
                        InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                        InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = true;
                        InspectStationAct.SuaInAction = false;
                        string status = fini21.comment;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front2 Finished Reject or error"+ "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    }
                    else
                    {
                        string status = fini21.comment;
                        //Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=>"+ fini21.comment + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        if (chkSuaNoReject.Checked) status = "0,0";//debug

                        string[] ss = status.Split(',');
                        
                        // rep1.status = ss[2] + "," + ss[3];
                        if (ss.Length > 3 && int.Parse(ss[2]) == 0 && int.Parse(ss[3]) == 0)//ss[2] breaks; ss[3] peels
                        {
                            
                            InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;
                            InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                            InspectStationAct.SuaInAction = false;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front2 Finished" + " breaks:" + ss[2] + " peels:" + ss[3] + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        }
                        else
                        {
                            InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;//ss[2]=break count,ss[3]=peels count
                            InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = true;
                            InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] = (int)MyStatic.E_State.SuaFrontFini;
                            InspectStationAct.SuaInAction = false;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front2 Finished" + " breaks:" + ss[2] + " peels:" + ss[3] + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //GetReject(RejectPartId, PartIndex, (int)MyStatic.E_State.Reject, "", status, "", fini1.comment, true);
                        }


                    }
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Inspect Front2 Finished" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                }





            }
            catch (Exception ex)
            {
                MessageBox.Show("Cognex Inspect ERROR" + ex.Message);
                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Cognex Error" + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                return;
            }
        }
        private async Task<WebComm.CommReply> StartCycleInspectCognex()
        {

            WebComm.CommReply reply = new WebComm.CommReply();

            try
            {
                ;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Top inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.StartCognex).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.StartCognex;
                Parms.comment = "start Cognex";
                Parms.timeout = 60;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                //16
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = 60.0f;// 0.5f;//timeout

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.StartCognex;
                reply = WC3.RunCmd(Parms);

                if (!reply.result)
                {
                    MyStatic.bExitcycleNow = true;
                    MessageBox.Show("Cognex Top ERROR!", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }


                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Top Inspect fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Cognex Top ERROR:" + err.Message);
                return reply;
            }
        }
        private async Task<WebComm.CommReply> InitCycle()
        {

            WebComm.CommReply reply = new WebComm.CommReply();

            try
            {
                ;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Init Cycle" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.InitCycle).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.InitCycle;
                Parms.comment = "init cycle";
                Parms.timeout = 5;
                Array.Resize<Single>(ref Parms.SendParm, 4);
                //16
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = InspectStationAct.OnFooterGrip3_PartID;// part num
                Parms.SendParm[3] = 5.0f;// 0.5f;//timeout

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.InitCycle;
                reply = WC1.RunCmd(Parms);

                if (!reply.result)
                {
                    MyStatic.bExitcycleNow = true;
                    MessageBox.Show("ERROR INIT CYCLE!", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }


                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> INIT CYCLE fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Init Cycle ERROR:" + err.Message);
                return reply;
            }
        }
        private async void WeldonMainTask()
        {
            try
            {

                if (InspectStationAct.WeldonInAction) return;
                if (MyStatic.bReset) return;
                if (MyStatic.bExitcycleNow) return;

                if ((InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini) &&
                      //InspectStationAct.WeldonState != (int)MyStatic.E_State.WeldonDataFini &&
                      InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] != (int)MyStatic.E_State.WeldonFini &&
                !InspectStationAct.WeldonInAction &&
                    !chkWeldon.Checked &&
                    !FooterStationAct.AxisInAction)
                {
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.WeldonState = (int)MyStatic.E_State.WeldonDataFini;
                    InspectStationAct.WeldonInAction = false;
                    return;
                }

                if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini &&
                     //InspectStationAct.WeldonState != (int)MyStatic.E_State.WeldonDataFini &&
                     InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] != (int)MyStatic.E_State.WeldonFini &&
                    !InspectStationAct.WeldonInAction &&
                    chkWeldon.Checked &&
                    !FooterStationAct.AxisInAction)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon<= Weldon Start" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.WeldonInAction = true;
                    InspectStationAct.WeldonState = (int)MyStatic.E_State.Occupied;
                    FooterStationAct.AxisInAction = true;
                    inv.settxt(lblInspect, "Run Weldon");
                    //move to weldon ax5 and ax4
                    int axis = 0;
                    if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                    if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());

                    Single Pos5 = master.Ax5_Weldone + upDwnFootWeldX.UpDownValue + weldone.weldX;// + (Single.Parse(txtPartLength.Text) - master.Length);
                    Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos2 = master.Ax2_Work + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                    Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                    //lamp1
                    BitArray lamp = new BitArray(new bool[8]);
                    lamp[0] = true; //lamp1
                    byte[] Lamps = new byte[1];
                    lamp.CopyTo(Lamps, 0);

                    Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                    Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                    Thread.Sleep(200);
                    var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                    await task;
                    CommReply reply1 = task.Result;
                    if (!reply1.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> ERROR MOVE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        //InspectStationAct.VisionInAction = false;
                        //MyStatic.bReset = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "Error move Footer to Weldon Position!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;

                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        return;
                    }

                    //run weldon io-link
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon<= run measure" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    var task1 = Task.Run(() => RunWeldonFileInv_1());//io link data
                    await task1;
                    if (!task1.Result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> ERROR ROTATE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        //InspectStationAct.VisionInAction = false;
                        //MyStatic.bReset = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "Error Rotate Footer to Weldon Position!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        return;
                    }
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InWork;
                    InspectStationAct.WeldonState = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    partData[InspectStationAct.OnFooterGrip3_PartID].Weldone = weldone.angle;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon<= get measure results" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    //run weldon graph
                    var task2 = Task.Run(() => RunWeldonFileInv_2());//graph
                    await task2;
                    CommReply reply = task2.Result;
                    if (reply.result && reply.data != null)//weldon ok
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Weeldon=> Weldon Data Ready " + reply.data[0].ToString("0.00") + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        if (Math.Abs(reply.data[0] - weldone.angle) < 20)
                        {

                            inv.settxt(lblInspect, "Weldon Ready");
                            InspectStationAct.WeldonState = (int)MyStatic.E_State.WeldonDataFini;
                            InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                            inv.settxt(lblInspect, "weldon Ready");
                            InspectStationAct.WeldonInAction = false;
                        }
                        else
                        {
                            inv.settxt(lblInspect, "Weldon Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Weeldon=> Weldon Reject " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            InspectStationAct.WeldonState = (int)MyStatic.E_State.RejectWeldon;
                            InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                            InspectStationAct.Reject[(int)MyStatic.Reject.Weldone] = true;
                            InspectStationAct.WeldonInAction = false;

                        }
                    }
                    else
                    {
                        inv.settxt(lblInspect, "Weldon Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Weeldon=> Weldon Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.WeldonState = (int)MyStatic.E_State.RejectWeldon;
                        InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Weldone] = true;
                        InspectStationAct.WeldonInAction = false;

                    }

                }
            }
            catch (Exception ex)
            {
                MyStatic.bExitcycleNow = true;
                //MessageBox.Show("WELDON ERROR" + ex.Message);
                InspectStationAct.WeldonInAction = false;
                InspectStationAct.WeldonState = (int)MyStatic.E_State.RejectWeldon;
                InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                InspectStationAct.Reject[(int)MyStatic.Reject.Weldone] = true;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> Weldon Error" + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                ErrorMess = "Weldon Error!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                return;
            }
        }
        Stopwatch swcycle = new Stopwatch();
        int Inspected_PartID = 0;
        private async void RobotMainTask()
        {
            //-----------------RobotMainTask Load Action------------------
            //grip1-pick tray, place inspect
            //grip2-pick inspect,place tray
            try
            {
                inv.settxt(txtCycleTimeRun, (swcycle.ElapsedMilliseconds / 1000.0).ToString("00.00"));
                if (RobotLoadAct.InAction) return;
                if (MyStatic.bReset) return;
                if (FooterStationAct.State == (int)MyStatic.E_State.InError) return;
                if (MyStatic.bExitcycleNow) return;

                //-------------------------pick part from tray---------------------------------------------
                if (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Empty &&
                    RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Empty &&
                    !MyStatic.bExitcycle && !MyStatic.bExitcycleNow && !EndOfTray && !MyStatic.bEmpty)
                {

                    RobotLoadAct.InAction = true;
                    //
                    int TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
                    int TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);
                    if (TrayPartId >= TrayInsertsOnX * TrayInsertsOnY)
                    //((TrayInsertsOnX == 2 && TrayInsertsOnY == 10 && TrayPartId >= 20) ||  //
                    //     (TrayInsertsOnX == 3 && TrayInsertsOnY == 18 && TrayPartId >= 51) ||
                    //      (TrayInsertsOnX == 2 && TrayInsertsOnY == 18 && TrayPartId >= 34))
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Pick Tray " + TrayPartId.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //MessageBox.Show("END OF TRAY", "ERROR", MessageBoxButtons.OK,
                        //           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        //MyStatic.bExitcycle = true;
                        EndOfTray = true;
                        RobotLoadAct.InAction = false;
                        return;
                    }
                    else EndOfTray = false;
                    //
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<= Pick Tray Grip1 id=" + TrayPartId.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Pick Tray");
                    inv.settxt(txtCycleTime, (swcycle.ElapsedMilliseconds / 1000.0).ToString("0.00"));
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("---- CYCLE TIME = " + txtCycleTime.Text + "  speed = " +
                        frmMain.newFrmMain.FanucSpeed.ToString() + "-----", frmMain.newFrmMain.txtAutoLog, false));
                    swcycle.Restart();


                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    position basepos = new position();

                    int partid = TrayPartId;
                    RobotFunctions.CommReply commreply = GetPickTray(partid, false);

                    if (commreply.result && commreply.data.Length >= 4)
                    {
                        basepos.x = commreply.data[0] + (Single)UpDwnX3.UpDownValue;
                        basepos.y = commreply.data[1] + (Single)UpDwnY3.UpDownValue;
                        basepos.z = commreply.data[2] + (Single)UpDwnZ3.UpDownValue;
                        basepos.r = commreply.data[3] + (Single)UpDwnR3.UpDownValue;
                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        //MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                        //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT GET POSITION ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        return;
                    }
                    //fini = PickFromTray(basepos);
                    var task11 = Task.Run(() => PickFromTray(basepos));
                    await task11;
                    fini = task11.Result;
                    if(fini.result && chkCleanAir.Checked)
                    {
                        Single length = 0;
                        if (txtPartLength.Text.Trim() != "")
                        {
                            length = Single.Parse(txtPartLength.Text);
                        }
                        if (length == 0) length = 150;
                        Single speedclean = 300;
                        var task21 = Task.Run(() => AirClean(length, speedclean));
                        await task21;
                        fini = task21.Result;
                    }
                    if (fini.result)
                    {
                        inv.settxt(txtGrip1num, TrayPartId.ToString());
                        RobotLoadAct.OnGrip1_PartID = TrayPartId;
                        RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.Occupied;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Fini Pick Tray Grip1 id=" + TrayPartId.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        inv.settxt(lblRobot, "Fini Pick Tray");
                        partData[RobotLoadAct.OnGrip1_PartID].State[(int)MyStatic.InspectSt.Robot] = (int)MyStatic.E_State.Occupied;
                        partData[RobotLoadAct.OnGrip1_PartID].Position = (int)MyStatic.E_State.OnGrip1;
                        for (int i = 0; i < partData[RobotLoadAct.OnGrip1_PartID].Reject.Length; i++) partData[RobotLoadAct.OnGrip1_PartID].Reject[i] = false;


                        TrayPartId++;
                        TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
                        TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);
                        if (TrayPartId >= TrayInsertsOnX * TrayInsertsOnY ||
                                  //((TrayInsertsOnX == 2 && TrayInsertsOnY == 10 && TrayPartId >= 20) ||  //
                                  //     (TrayInsertsOnX == 3 && TrayInsertsOnY == 18 && TrayPartId >= 51) ||
                                  //      (TrayInsertsOnX == 2 && TrayInsertsOnY == 18 && TrayPartId >= 34)   || 
                                  MyStatic.bOneCycle)
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Pick Tray1 " + TrayPartId.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //MessageBox.Show("END OF TRAY", "ERROR", MessageBoxButtons.OK,
                            //           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                            //MyStatic.bExitcycle = true;
                            EndOfTray = true;
                            //TrayPartId = (TrayInsertsOnX * TrayInsertsOnY) ;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        else EndOfTray = false;
                        int partindex = TrayPartId;
                        Panel panel = panelTrayOut;
                        position outpos = new position();
                        if (!EndOfTray) DrawTrayOut(panel, ref partindex, ref outpos);
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Next Pick id=" + TrayPartId.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //above insp
                        if (!(InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty))
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Above Inspect Grip2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            inv.settxt(lblRobot, "Above Pick Inspect");

                            fini = new RobotFunctions.CommReply();
                            basepos = new position();

                            basepos.x = RobotLoadPoints.PickInspect.x + (Single)UpDwnX5.UpDownValue;
                            basepos.y = RobotLoadPoints.PickInspect.y + (Single)UpDwnY5.UpDownValue;
                            basepos.z = RobotLoadPoints.PickInspect.z + (Single)UpDwnZ5.UpDownValue;
                            basepos.r = RobotLoadPoints.PickInspect.r + (Single)UpDwnR5.UpDownValue;
                            fini = AboveInsp(basepos);

                            if (fini.result)
                            {
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Above Inspect Grip2 fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                RobotLoadAct.InAction = false;
                                return;
                            }
                            else
                            {
                                MyStatic.bExitcycle = true;
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Above Inspect Grip2 Error" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                //MessageBox.Show("ROBOT ABOVE INSPECT ERROR!", "ERROR", MessageBoxButtons.OK,
                                //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                inv.settxt(lblRobot, "Error Above iInspect");
                                MyStatic.bReset = true;
                                RobotLoadAct.InAction = false;
                                return;
                            }
                            //fini aboveinsp
                        }
                        RobotLoadAct.InAction = false;
                        return;


                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PICK FROM TRAY ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //MessageBox.Show("ROBOT PICK FROM TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                        //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        inv.settxt(lblRobot, "Error Pick Tray");
                        MyStatic.bReset = true;
                        MyStatic.bExitcycleNow = true;
                        RobotLoadAct.InAction = false;
                        return;
                    }
                    return;
                }
                //------------------------------place inspect-------------------------------------------- 
                if (RobotLoadAct.InAction) return;

                if (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Occupied &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty &&
                    FooterStationAct.State == (int)MyStatic.E_State.InHome)
                {

                    if (bPartInFooter)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Error Place Inspect. Footer not empty! " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                        //MyStatic.bReset = true;
                        MyStatic.bExitcycleNow = true;
                        RobotLoadAct.InAction = false;
                        InspectStationAct.State[(int)MyStatic.InspectSt.Footer] = (int)MyStatic.E_State.InError;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Robot] = true;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        MessageBox.Show("ROBOT PLACE INSPECT ERROR! Footer not EMPTY!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        ErrorMess = "Robot=>Error Place Inspect. Footer not empty!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        return;
                    }
                    if (InspectStationAct.InAction) return;
                    if (RobotLoadAct.InAction) return;
                    RobotLoadAct.InAction = true;
                    InspectStationAct.InAction = true;

                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Place Inspect Grip1 id=" + RobotLoadAct.OnGrip1_PartID.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Place Inspect");
                    cntLongPart = 0;

                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    position basepos = new position();

                    basepos.x = RobotLoadPoints.PlaceInspect.x + (Single)UpDwnX4.UpDownValue;
                    basepos.y = RobotLoadPoints.PlaceInspect.y + (Single)UpDwnY4.UpDownValue;
                    basepos.z = RobotLoadPoints.PlaceInspect.z + (Single)UpDwnZ4.UpDownValue;
                    basepos.r = RobotLoadPoints.PlaceInspect.r + (Single)UpDwnR4.UpDownValue;
                    //fini = PlacePartInsp(basepos);
                    var task12 = Task.Run(() => PlacePartInsp(basepos));
                    await task12;
                    fini = task12.Result;

                    if (fini.result)
                    {
                        RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.Empty;

                        InspectStationAct.State[(int)MyStatic.InspectSt.Footer] = (int)MyStatic.E_State.Occupied;
                        InspectStationAct.SuaState = (int)MyStatic.E_State.Occupied;
                        inv.settxt(txtGrip3num, txtGrip1num.Text);
                        InspectStationAct.OnFooterGrip3_PartID = RobotLoadAct.OnGrip1_PartID;
                        if(txtGrip3num.Text.Trim() != "") Inspected_PartID = int.Parse(txtGrip3num.Text);// RobotLoadAct.OnGrip1_PartID;
                        inv.settxt(txtGrip1num, "");
                        RobotLoadAct.OnGrip1_PartID = -1;
                        partData[InspectStationAct.OnFooterGrip3_PartID].Position = (int)MyStatic.E_State.OnInsp;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Fini Place Inspect Grip1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("inspect=>Part id=" + txtGrip3num.Text + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        inv.settxt(lblRobot, "Fini Place Inspect");
                        //send data to vision
                        //WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                        var task2 = Task.Run(() => DataToVision());
                        await task2;
                        WebComm.CommReply reply = task2.Result;
                        if (!reply.result)
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision <=" + "ERROR DEND DATA TO VISION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        }
                        ///////////////////
                        RobotLoadAct.InAction = false;
                        InspectStationAct.InAction = false;

                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        inv.settxt(lblRobot, "Place Inspect Error");
                        MessageBox.Show("ROBOT PLACE INSPECT ERROR ERROR!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        MyStatic.bReset = true;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Robot] = true;
                        MyStatic.bExitcycleNow = true;
                        RobotLoadAct.InAction = false;
                        return;
                    }
                }

                //------------------------------pick inspect------after vision ready-------------------------------------- 
                if (RobotLoadAct.InAction) return;

                if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Empty &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.TopSnapFini &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] == (int)MyStatic.E_State.FrontSnapFini &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] == (int)MyStatic.E_State.DiamFini &&
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] == (int)MyStatic.E_State.WeldonFini &&
                    //InspectStationAct.WeldonState == (int)MyStatic.E_State.WeldonDataFini   &&
                    FooterStationAct.State == (int)MyStatic.E_State.InHome)
                {
                    if (InspectStationAct.InAction) return;
                    if (RobotLoadAct.InAction) return;
                    RobotLoadAct.InAction = true;
                    InspectStationAct.InAction = true;

                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Pick Inspect Grip2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Pick Inspect");

                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    position basepos = new position();

                    basepos.x = RobotLoadPoints.PickInspect.x + (Single)UpDwnX5.UpDownValue;
                    basepos.y = RobotLoadPoints.PickInspect.y + (Single)UpDwnY5.UpDownValue;
                    basepos.z = RobotLoadPoints.PickInspect.z + (Single)UpDwnZ5.UpDownValue;
                    basepos.r = RobotLoadPoints.PickInspect.r + (Single)UpDwnR5.UpDownValue;
                    var task10 = Task.Run(() => PickPartInsp(basepos));
                    await task10;
                    fini = task10.Result;

                    if (fini.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Wait Inspect Top  Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //==========wait sua fini InspectStationAct.State=InspectStationAct.SuaState================
                        Stopwatch sw = new Stopwatch();
                        sw.Restart();
                        //run wait cognex top fini
                        while (!MyStatic.bReset)
                        {
                            if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.SuaFini) break;
                            if (sw.ElapsedMilliseconds > 60000)
                            {
                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaTop] = true;
                                MyStatic.bExitcycleNow = true;
                                MyStatic.bExitcycle = true;
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Cognex Error" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                return;
                            }
                            Thread.Sleep(20);
                        }
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=>  Inspect Top  Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex<= Wait Inspect Front  Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                        //------run wait cognex front fini-------------
                        sw.Restart();
                        while (!MyStatic.bReset)
                        {
                            if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaFront] == (int)MyStatic.E_State.SuaFrontFini) break;
                            if (sw.ElapsedMilliseconds > 10000)
                            {
                                InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFrontFini;
                                InspectStationAct.Reject[(int)MyStatic.Reject.SuaFront] = true;
                                MyStatic.bExitcycleNow = true;
                                MyStatic.bExitcycle = true;
                                Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=> Cognex Front Error" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                return;
                            }
                            Thread.Sleep(20);
                        }
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Cognex=>  Inspect Front  Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                        RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Occupied;// InspectStationAct.State;
                        InspectStationAct.State[(int)MyStatic.InspectSt.Footer] = (int)MyStatic.E_State.Empty;
                        bPartInFooter = false;
                        inv.settxt(txtGrip2num, txtGrip3num.Text);
                        RobotLoadAct.OnGrip2_PartID = InspectStationAct.OnFooterGrip3_PartID;
                        partData[RobotLoadAct.OnGrip2_PartID].Position = (int)MyStatic.E_State.OnGrip2;
                        //partData[RobotLoadAct.OnGrip2_PartID].State = InspectStationAct.State;
                        //partData[RobotLoadAct.OnGrip2_PartID].Reject = InspectStationAct.Reject;
                        Array.Copy(InspectStationAct.Reject, partData[RobotLoadAct.OnGrip2_PartID].Reject, 16);
                        Array.Copy(InspectStationAct.State, partData[RobotLoadAct.OnGrip2_PartID].State, 16);

                        inv.settxt(txtGrip3num, "");
                        InspectStationAct.OnFooterGrip3_PartID = -1;
                        //clecn inspect station data
                        for (int i = 0; i < InspectStationAct.State.Length; i++) InspectStationAct.State[i] = (int)MyStatic.E_State.Empty;
                        for (int i = 0; i < InspectStationAct.State.Length; i++) InspectStationAct.Reject[i] = false;
                        InspectStationAct.WeldonState = 0;
                        InspectStationAct.SuaState = 0;


                        inv.settxt(lblRobot, "Fini Pick Inspect");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Fini Pick Inspect Grip2 id=" + RobotLoadAct.OnGrip2_PartID.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        RobotLoadAct.InAction = false;
                        InspectStationAct.InAction = false;


                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        inv.settxt(lblRobot, "Pick Inspect Error");
                        //MessageBox.Show("ROBOT PICK INSPECT ERROR ERROR!", "ERROR", MessageBoxButtons.OK,
                        //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> PICK INSPECT ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        InspectStationAct.InAction = false;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Robot] = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ERROR ROBOT PICK INSPECT!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        inv.settxt(lblRobot, "Pick Inspect Error");

                        return;
                    }
                }

                //-------------------------place part on tray-after cognex ready--------------------------------------------
                //if (((RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.SuaReady && !chkWeldon.Checked)|| (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.WeldonReady) && chkWeldon.Checked)
                //    && (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Empty))
                bool rejectall = false;
                if (RobotLoadAct.OnGrip2_PartID >= 0)
                {
                    for (int i = 0; i < partData[RobotLoadAct.OnGrip2_PartID].Reject.Length; i++) { if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i]) rejectall = true; }
                }
                //rejectall = false;//test
                if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Occupied &&
                    RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Empty &&
                    !rejectall)// &&
                               //InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty)
                {
                    if (RobotLoadAct.InAction) return;
                    RobotLoadAct.InAction = true;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<= Place Tray Grip2 id=" + txtGrip2num.Text + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Place on Tray");

                    int partid = RobotLoadAct.OnGrip2_PartID;

                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    position basepos = new position();
                    RobotFunctions.CommReply commreply = GetPlaceTray(partid, false);

                    if (commreply.result && commreply.data.Length >= 4)
                    {
                        basepos.x = commreply.data[0] + (Single)UpDwnX6.UpDownValue;
                        basepos.y = commreply.data[1] + (Single)UpDwnY6.UpDownValue;
                        basepos.z = commreply.data[2] + (Single)UpDwnZ6.UpDownValue;
                        basepos.r = commreply.data[3] + (Single)UpDwnR6.UpDownValue;
                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT GET POSITION ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                        //MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                        //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        return;
                    }
                    //fini = PlaceTray(basepos);
                    var task13 = Task.Run(() => PlaceTray(basepos));
                    await task13;
                    fini = task13.Result;

                    if (fini.result)
                    {
                        position outpos = new position();
                        outpos.Error = "";

                        RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Empty;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Fini Place Tray Grip2 id=" + RobotLoadAct.OnGrip2_PartID.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        partData[RobotLoadAct.OnGrip2_PartID].Position = (int)MyStatic.E_State.OnTrayFini;
                        partData[RobotLoadAct.OnGrip2_PartID].State[(int)MyStatic.InspectSt.SuaTop] = (int)MyStatic.E_State.SuaFini;
                        inv.settxt(txtGrip2num, "");
                        RobotLoadAct.OnGrip2_PartID = -1;


                        inv.settxt(lblRobot, "Fini Place on Tray");
                        RobotLoadAct.InAction = false;
                        int TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
                        int TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);

                        if (MyStatic.bOneCycle)
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Place Tray2 One cycle" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            RobotLoadAct.InAction = false;
                            MyStatic.bExitcycle = true;
                            return;
                        }
                        if (TrayPartId >= TrayInsertsOnX * TrayInsertsOnY)
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Place Tray2 " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //MessageBox.Show("END OF TRAY", "ERROR", MessageBoxButtons.OK,
                            //           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                            //MyStatic.bExitcycle = true;

                            RobotLoadAct.InAction = false;
                            //return;
                        }
                        if (EndOfTray && InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty)
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Place Tray3 " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            MyStatic.bExitcycle = true;
                            //MessageBox.Show("END OF TRAY", "ERROR", MessageBoxButtons.OK,
                            //           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);



                            RobotLoadAct.InAction = false;
                            return;
                        }

                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        inv.settxt(lblRobot, "Place on Tray Error");
                        MessageBox.Show("ROBOT PLACE TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        //MyStatic.bReset = true;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Robot] = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ROBOT PLASE TRAY ERROR!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                        RobotLoadAct.InAction = false;
                        return;
                    }
                    return;
                }
                bool RejectAll = false;
                if (RobotLoadAct.OnGrip2_PartID >= 0)
                {
                    for (int i = 0; i < partData[RobotLoadAct.OnGrip2_PartID].Reject.Length; i++) { if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i]) RejectAll = true; }
                }
                //RejectAll = false;//test
                //-------------------------place reject---after cognex ready------------------------------------------

                if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Occupied &&
                    RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Empty &&
                    RejectAll)// &&
                              //InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty)
                {
                    if (RobotLoadAct.InAction) return;
                    RobotLoadAct.InAction = true;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<= Place Reject Grip2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Place Reject");
                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    position basepos = new position();
                    //RobotFunctions.CommReply commreply = GetRejectTray(false);
                    var task3 = Task.Run(() => GetRejectTray(false));
                    await task3;
                    RobotFunctions.CommReply commreply = task3.Result;

                    if (commreply.result && commreply.data.Length >= 4)
                    {
                        basepos.x = commreply.data[0] + (Single)UpDwnX7.UpDownValue;
                        basepos.y = commreply.data[1] + (Single)UpDwnY7.UpDownValue;
                        basepos.z = commreply.data[2] + (Single)UpDwnZ7.UpDownValue;
                        basepos.r = commreply.data[3] + (Single)UpDwnR7.UpDownValue;
                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        //MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                        //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT GET POSITION ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        return;
                    }

                    //fini = PlaceReject(basepos);
                    var task14 = Task.Run(() => PlaceReject(basepos));
                    await task14;
                    fini = task14.Result;

                    if (fini.result)
                    {
                        position outpos = new position();
                        outpos.Error = "";
                        //Array.Copy(InspectStationAct.Reject, partData[RobotLoadAct.OnGrip2_PartID].Reject, 16);
                        //InspectStationAct.Reject[(int)Reject.VisionTop] = true;
                        string sRejected = "";
                        //VisionTop = 0,
                        //VisionFront = 1,
                        //VisionDiam = 2,
                        //SuaTop = 3,
                        //SuaFront = 4,
                        //Weldone = 5,
                        //VisionCount = 6,
                        //VisionColor = 7,
                        //Beckhoff = 8,
                        //Robot = 9
                        string[] srej = new string[6];
                        for (int i = 0; i < partData[RobotLoadAct.OnGrip2_PartID].Reject.Length; i++)
                        {
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.VisionTop) { sRejected = sRejected + "Vision Top" + ";"; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.VisionFront) { sRejected = sRejected + "Vision Front" + ";"; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.VisionDiam) { sRejected = sRejected + "Vision Diam" + ";";  srej[0] = "False " + partData[RobotLoadAct.OnGrip2_PartID].Diam.ToString(); }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.SuaTop) { sRejected = sRejected + "Cognex Top" + ";"; srej[1] = "False "; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.SuaFront) { sRejected = sRejected + "Cognex Front" + ";"; srej[2] = "False"; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.Weldone) { sRejected = sRejected + "Weldone" + ";"; srej[4] = "False"; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.VisionCount) { sRejected = sRejected + "Vision Count" + ";"; srej[3] = "False " + partData[RobotLoadAct.OnGrip2_PartID].Count.ToString(); }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.VisionColor) { sRejected = sRejected + "Vision Color" + ";"; srej[5] = "False"; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.Beckhoff) { sRejected = sRejected + "Beckhoff" + ";"; }
                            if (partData[RobotLoadAct.OnGrip2_PartID].Reject[i] && i == (int)MyStatic.Reject.Robot) { sRejected = sRejected + "Robot" + ";"; }
                        }
                        GetReject(TrayPartIdRej, RobotLoadAct.OnGrip2_PartID, srej[0], srej[1], srej[2], srej[3], srej[4], srej[5], sRejected, true);
                        //NoPlace();
                        TrayPartIdRej++;
                        inv.settxt(txtPartIDRej, TrayPartIdRej.ToString());
                        int partindex = TrayPartIdRej;
                        Panel panel = panelTrayRej;
                        DrawTrayOut(panel, ref partindex, ref outpos);
                        TrayPartIdRej = partindex;
                        RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Empty;
                        partData[RobotLoadAct.OnGrip2_PartID].Position = (int)MyStatic.E_State.OnTrayReject;
                        //partData[RobotLoadAct.OnGrip2_PartID].State = RobotLoadAct.OnGrip2_State;
                        //Array.Copy(RobotLoadAct.OnGrip2_State, partData[RobotLoadAct.OnGrip2_PartID].State, 16);
                        inv.settxt(txtGrip2num, "");
                        RobotLoadAct.OnGrip2_PartID = -1;

                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>Fini Place Reject Grip2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        inv.settxt(lblRobot, "Fini Place Reject");
                        if (MyStatic.bOneCycle)
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>End OF Place Reject One cycle" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            RobotLoadAct.InAction = false;
                            MyStatic.bExitcycle = true;
                            return;
                        }
                        RobotLoadAct.InAction = false;

                    }
                    else
                    {
                        MyStatic.bExitcycle = true;
                        inv.settxt(lblRobot, "Place Reject Error");

                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PLACE REJECT ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Robot] = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ERROR ROBOT Place Reject!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        inv.settxt(lblRobot, "Place Reject Error");
                        return;
                    }
                    return;
                }


            }
            catch (Exception err)
            {
                MessageBox.Show("Fanuc Load MAIN TASK ERROR" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT  ERROR" + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                InspectStationAct.Reject[(int)MyStatic.Reject.Robot] = true;
                MyStatic.bExitcycle = true;
                MyStatic.bExitcycleNow = true;
                ErrorMess = "ERROR ROBOT! " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                inv.settxt(lblRobot, "Error");
                return;
            }
        }
        private RobotFunctions.CommReply PickFromTray(position pos)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Pick Tray";
                parms1.cmd = ((int)MyStatic.RobotCmd.PickTray).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                //parms1.SendParm[1] = Robot1data.SpeedOvr;
                //MyStatic.bStartcycle = false;
                Array.Resize<Single>(ref parms1.SendParm, 10);
                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 50;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.PickTray;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                parms1.SendParm[2] = (Single)pos.x;
                parms1.SendParm[3] = (Single)pos.y;
                parms1.SendParm[4] = (Single)pos.z;
                parms1.SendParm[7] = (Single)pos.r;
                if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                parms1.SendParm[8] = Robot1data.Step;
                parms1.SendParm[9] = 2;// 0.5f;//timeout

                ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1);


                if (rep.result && rep.data != null && rep.data[1] != 0)
                {
                    return rep;

                }
                else
                {
                    MyStatic.bExitcycle = true;
                    //MessageBox.Show("ROBOT PICK FROM TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> PICK FROM TRAY ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    return rep;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT PICK FROM TRAY ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PICK TRAY ERROR " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }
        private RobotFunctions.CommReply AirClean(Single length, Single speed)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Air Clean";
                parms1.cmd = ((int)MyStatic.RobotCmd.AirClean).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                //parms1.SendParm[1] = Robot1data.SpeedOvr;
                //MyStatic.bStartcycle = false;
                Array.Resize<Single>(ref parms1.SendParm, 10);
                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 60;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.AirClean;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                parms1.SendParm[2] = length;
                parms1.SendParm[3] = speed;
                parms1.SendParm[4] = 0;
                parms1.SendParm[7] = 0;
                if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                parms1.SendParm[8] = Robot1data.Step;
                parms1.SendParm[9] = 60.0f;// 0.5f;//timeout

                ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1);


                if (rep.result && rep.data != null && rep.data[1] != 0)
                {
                    return rep;

                }
                else
                {
                    MyStatic.bExitcycle = true;
                    //MessageBox.Show("ROBOT PICK FROM TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> AIR CLEAN ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    return rep;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT PICK FROM TRAY ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT AIR CLEAN ERROR " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }
        private RobotFunctions.CommReply PlacePartInsp(position pos)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Place Part Insspection";
                parms1.cmd = ((int)MyStatic.RobotCmd.PlaceInspection).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                Array.Resize<Single>(ref parms1.SendParm, 10);

                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 30;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.PlaceInspection;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed * 1.5f;// * 3;// general speed
                parms1.SendParm[2] = (Single)pos.x;
                parms1.SendParm[3] = (Single)pos.y;
                parms1.SendParm[4] = (Single)pos.z;
                parms1.SendParm[7] = (Single)pos.r;
                if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                parms1.SendParm[8] = Robot1data.Step;
                parms1.SendParm[9] = 2;// 0.5f;//timeout
                ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1);

                if (rep.result && rep.data != null && rep.data[1] != 0)
                {
                    return rep;

                }
                else
                {
                    MyStatic.bExitcycle = true;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PLACE PART ERROR ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //MessageBox.Show("ROBOT Place Part Inspection ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return rep;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT Place Part Inspection ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PLACE PART ERROR" + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }
        private RobotFunctions.CommReply PickPartInsp(position pos)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Pick Part Insspection";
                parms1.cmd = ((int)MyStatic.RobotCmd.PickInspection).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                Array.Resize<Single>(ref parms1.SendParm, 10);

                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 30;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.PickInspection;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed * 1.5f;// * 3;// general speed
                parms1.SendParm[2] = (Single)pos.x;
                parms1.SendParm[3] = (Single)pos.y;
                parms1.SendParm[4] = (Single)pos.z;
                parms1.SendParm[7] = (Single)pos.r;
                if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                parms1.SendParm[8] = Robot1data.Step;
                parms1.SendParm[9] = 2;// 0.5f;//timeout
                //ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1);

                if (rep.result && rep.data != null && rep.data[1] != 0)
                {
                    return rep;

                }
                else
                {
                    MyStatic.bExitcycle = true;
                    //MessageBox.Show("ROBOT Pick Part Inspection ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PICK INSPECTION ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    return rep;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT Pick Part Inspection ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PICK INSPECTION ERROR " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }
        private RobotFunctions.CommReply AboveInsp(position pos)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Pick Part Insspection";
                parms1.cmd = ((int)MyStatic.RobotCmd.AboveInsp).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                Array.Resize<Single>(ref parms1.SendParm, 10);

                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 30;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.AboveInsp;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                parms1.SendParm[2] = (Single)pos.x;
                parms1.SendParm[3] = (Single)pos.y;
                parms1.SendParm[4] = (Single)pos.z;
                parms1.SendParm[7] = (Single)pos.r;
                if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                parms1.SendParm[8] = Robot1data.Step;
                parms1.SendParm[9] = 2;// 0.5f;//timeout
                //ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1);

                if (rep.result)
                {

                    commreply.result = true;
                    return commreply;

                }
                else
                {
                    MyStatic.bExitcycle = true;
                    //MessageBox.Show("ROBOT Above Pick Part Inspection ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT MOVE ABOVE INSPECTION  ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    commreply.result = false;
                    return commreply;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT ABOVE Pick Part Inspection ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT MOVE ABOVE INSPECTION  ERROR " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }
        private async Task<RobotFunctions.CommReply> RunSetup(int pos, int robot)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Setup";
                parms1.cmd = ((int)MyStatic.RobotCmd.teach).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                //parms1.SendParm[1] = Robot1data.SpeedOvr;
                //MyStatic.bStartcycle = false;
                Array.Resize<Single>(ref parms1.SendParm, 6);
                parms1.timeout = 1000;


                parms1.SendParm[0] = (int)MyStatic.RobotCmd.teach;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                parms1.SendParm[2] = pos;
                parms1.SendParm[3] = 0;
                parms1.SendParm[4] = 0;
                parms1.SendParm[5] = 20;// 0.5f;//timeout

                ControlsEnable(false);
                MyStatic.bReset = false;
                RobotFunctions.CommReply rep = new RobotFunctions.CommReply();
                if (robot == MyStatic.RobotLoad)
                {
                    var task1 = Task.Run(() => FW1.RunCmdFanuc(parms1));
                    await task1;

                    rep = task1.Result;
                }

                if (rep.result)
                {
                    if (rep.data != null) commreply.data = rep.data;
                    commreply.result = true;
                    return commreply;

                }
                else
                {
                    MyStatic.bExitcycle = true;
                    //MessageBox.Show("ROBOT SETUP ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    commreply.result = false;
                    return commreply;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                MessageBox.Show("ROBOT SETUP ERROR:" + err.Message);
                commreply.result = false;
                return commreply;
            }

        }


        private RobotFunctions.CommReply PlaceTray(position pos)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Place Tray";
                parms1.cmd = ((int)MyStatic.RobotCmd.PlaceTray).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                Array.Resize<Single>(ref parms1.SendParm, 10);

                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 30;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.PlaceTray;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                parms1.SendParm[2] = (Single)pos.x;
                parms1.SendParm[3] = (Single)pos.y;
                parms1.SendParm[4] = (Single)pos.z;
                parms1.SendParm[7] = (Single)pos.r;
                if (chkStep.Checked) Robot2data.Step = 1; else Robot2data.Step = 0;
                parms1.SendParm[8] = Robot2data.Step;
                parms1.SendParm[9] = 10;// 0.5f;//timeout

                //MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1, false);//debug=true


                if (rep.result && rep.data != null && rep.data[1] != 0)
                {

                    return rep;

                }
                else
                {
                    //MessageBox.Show("ROBOT Place Tray ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> PLACE TRAY ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    return rep;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT Place Tray ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PLACE TRAY ERROR " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }
        private RobotFunctions.CommReply PlaceReject(position pos)
        {
            FanucWeb.SendRobotParms parms1 = new FanucWeb.SendRobotParms();
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();

            try
            {


                parms1.comment = "Place Tray";
                parms1.cmd = ((int)MyStatic.RobotCmd.PlaceReject).ToString();
                parms1.DebugTime = (int)(200000 / FanucSpeed);
                Array.Resize<Single>(ref parms1.SendParm, 10);

                if (chkStep.Checked) parms1.timeout = 1000;
                else parms1.timeout = 60;

                parms1.SendParm[0] = (int)MyStatic.RobotCmd.PlaceReject;
                parms1.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                parms1.SendParm[2] = (Single)pos.x;
                parms1.SendParm[3] = (Single)pos.y;
                parms1.SendParm[4] = (Single)pos.z;
                parms1.SendParm[7] = (Single)pos.r;
                if (chkStep.Checked) Robot2data.Step = 1; else Robot2data.Step = 0;
                parms1.SendParm[8] = Robot2data.Step;
                parms1.SendParm[9] = 60;// 0.5f;//timeout

                //MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;


                RobotFunctions.CommReply rep = FW1.RunCmdFanuc(parms1, false);//debug=true

                if (rep.result && rep.data != null && rep.data[1] != 0)
                {


                    return rep;

                }
                else
                {
                    //MessageBox.Show("ROBOT Place Reject ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PLACE REJECT ERROR" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    return rep;
                }
                //ControlsEnable(true);
            }
            catch (Exception err)
            {
                //MessageBox.Show("ROBOT Place Reject ERROR:" + err.Message);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=> ROBOT PLACE REJECT ERROR " + err.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                commreply.result = false;
                return commreply;
            }

        }


        #region ---------Delegate Procedures--------------
        public void ListAdd3(string st, System.Windows.Forms.TextBox txt, bool clear = false)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => SetLst3(st, clear, txt)));
                return;
            }
            else
            {
                SetLst3(st, clear, txt);
            }
        }
        private void SetLst3(string text, bool clear, System.Windows.Forms.TextBox txt)
        {


            try
            {
                if (text != "")
                {

                    //txt.Visible = false;
                    txt.AppendText(text + "\r\n");
                    if (txt.TextLength > 5000)
                    {
                        WriteToFile(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Log\\Mainlog " + DateTime.Now.ToString("yyyy-MM-dd HH") + ".ini", txtAutoLog.Text);
                        txt.Text = "";
                    }
                    //txt.Visible = true;

                }

            }
            catch (Exception ex)
            { txt.Visible = true; MessageBox.Show("LIST EXEPTION " + ex.Message); }

        }


        delegate void ControlsEnableInvoked(Boolean state);
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        public void ControlsEnable(Boolean state)
        {//enable/disable buttons
            if (!InvokeRequired)
            {
                if (state) panel1.BackColor = SystemColors.Control;
                else panel1.BackColor = Color.Gray;

                btnCloseGripper1.Enabled = state;

                btnCycleStart.Enabled = state;
                //btnCycleStop.Enabled = state;
                btnEmpty.Enabled = state;
                btnExecute.Enabled = state;
                btnHome.Enabled = state;
                btnMaintenance.Enabled = state;
                //btnNextStep.Enabled = state;
                btnOpenGripper1.Enabled = state;


                btnReadCurr.Enabled = state;
                btnReadPos.Enabled = state;
                btnReadRobotIO.Enabled = state;

                //btnRobotReset.Enabled = state;
                btnRobotStart.Enabled = state;
                btnSaveGripper.Enabled = state;
                btnSendRobot.Enabled = state;

                btnTeachTool.Enabled = state;
                btnTrayCalib.Enabled = state;
                btnNextStep.Enabled = true;
                btnReadCurPos.Enabled = state;
                btnAbort.Enabled = true;
                btnRun.Enabled = true;
                btnOpenGripper1.Enabled = state;
                btnOpenGripper2.Enabled = state;
                btnCloseGripper1.Enabled = state;
                btnCloseGripper2.Enabled = state;
                btnPlus.Enabled = state;
                btnMin.Enabled = state;
                btnRead.Enabled = state;
                btnFooterWork.Enabled = state;
                btnFooterHome.Enabled = state;
                btnFooterWork1.Enabled = state;
                btnFooterHome1.Enabled = state;
                btnToDiameter.Enabled = state;
                btnToFront.Enabled = state; 
                btnToWeldone.Enabled = state;
                btnCycleNoRobot.Enabled = state;    
                btnOneCycle.Enabled = state;
                btnCyclicTest.Enabled = state;
                panel6.Enabled = state;
                panel5.Enabled = state;
                panel8.Enabled = state;
                panel3.Enabled = state;
                btnPutInspect.Enabled = state;
                groupBox4.Enabled = state;
                btnClearAll.Enabled = state;

            }
            else
            {
                Invoke(new ControlsEnableInvoked(ControlsEnable), new object[] { state });
            }
        }
        delegate void UserControlsRefreshInvoked(UserControl.ControlsArrayControl user, Color color);
        public void UserControlsRefresh(UserControl.ControlsArrayControl user, Color color)
        {
            if (!InvokeRequired)
            {

                user.ControlBackColor = color;
                // user.Refresh();

            }
            else
            {
                Invoke(new UserControlsRefreshInvoked(UserControlsRefresh), new object[] { user, color });
            }
        }
        public void TextAdd(string st, System.Windows.Forms.TextBox txt)
        {
            if (txt.InvokeRequired)
            {
                txt.Invoke((MethodInvoker)delegate { txt.Text = st; });
                return;
            }
            else
            {
                txt.Text = st;
            }
        }
        public void LblAdd(string st, Label lbl, Color color)
        {

            if (lbl.InvokeRequired)
            {
                lbl.Invoke((MethodInvoker)delegate
                {
                    if (st != null) lbl.Text = st;
                    if (color != null) lbl.BackColor = color;
                }
                );
                return;
            }
            else
            {
                if (st != null) lbl.Text = st;
                if (color != null) lbl.BackColor = color;
            }
        }

        public void Pict(PictureBox pict, bool vis)
        {

            if (pict.InvokeRequired)
            {
                pict.Invoke((MethodInvoker)delegate
                {
                    pict.Visible = vis;
                }
                );
                return;
            }
            else
            {

                pict.Visible = vis;
            }
        }
        #endregion

        private async void BtnHome_Click(object sender, EventArgs e)
        {
            System.Console.Beep();
            System.Media.SystemSounds.Beep.Play();
            try
            {

                MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                var task = Task.Run(() => RobotHome());
                await task;

                bool rep1 = task.Result;

                if (rep1)
                {


                    ControlsEnable(true);


                }
                else
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }

                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                ControlsEnable(true);
            }
            catch (Exception err)
            {
                SetTraficLights(0, 0, 1, 0); MessageBox.Show("ROBOT HOME ERROR:" + err.Message);
            }
        }
        private async Task<bool> RobotHome()
        {
            System.Console.Beep();
            System.Media.SystemSounds.Beep.Play();
            try
            {
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<= Home" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                inv.settxt(lblRobot, "Move Home");
                if (RobotLoadAct.InAction) return false;
                RobotLoadAct.InAction = true;
                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();
                Parms.cmd = ((int)MyStatic.RobotCmd.MoveHome).ToString();
                Parms.DebugTime = 1000;
                Parms.comment = "Home";
                Array.Resize<Single>(ref Parms.SendParm, 10);

                Parms.timeout = 30;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.MoveHome;
                Parms.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                Parms.SendParm[2] = (Single)RobotLoadPoints.Home.x;
                Parms.SendParm[3] = (Single)RobotLoadPoints.Home.y;
                Parms.SendParm[4] = (Single)RobotLoadPoints.Home.z;
                Parms.SendParm[7] = (Single)RobotLoadPoints.Home.r;

                Parms.SendParm[8] = Robot1data.Step;
                Parms.SendParm[9] = 20;// 0.5f;//timeout

                MyStatic.bReset = false;
                var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));

                await task1;

                RobotFunctions.CommReply rep1 = task1.Result;
                RobotLoadAct.InAction = false;
                if (rep1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Home Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Move Home Fini");
                    return true;
                }
                else
                {
                    //MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.settxt(lblRobot, "Move Home Error");
                    return false;
                }

            }
            catch (Exception err)
            {
                return false;
            }
        }

        private async void BtnMaintenance_Click(object sender, EventArgs e)
        {
            System.Console.Beep();
            System.Media.SystemSounds.Beep.Play();
            inv.set(btnMaintenance, "Enabled", false);
            SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
            ControlsEnable(false);
            try
            {
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<= Maint" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                //
                var task2 = Task.Run(() => RobotHome());
                await task2;

                bool rep2 = task2.Result;

                if (!rep2)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                //
                inv.settxt(lblRobot, "Move Maint");
                if (RobotLoadAct.InAction) return;
                RobotLoadAct.InAction = true;
                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();
                Parms.cmd = ((int)MyStatic.RobotCmd.MoveMaint).ToString();
                Parms.DebugTime = 1000;
                Parms.comment = "Maint";
                Array.Resize<Single>(ref Parms.SendParm, 10);

                Parms.timeout = 30;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.MoveMaint;
                Parms.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                Parms.SendParm[2] = (Single)RobotLoadPoints.Maint.x;
                Parms.SendParm[3] = (Single)RobotLoadPoints.Maint.y;
                Parms.SendParm[4] = (Single)RobotLoadPoints.Maint.z;
                Parms.SendParm[7] = (Single)RobotLoadPoints.Maint.r;

                Parms.SendParm[8] = Robot1data.Step;
                Parms.SendParm[9] = 20;// 0.5f;//timeout

                MyStatic.bReset = false;
                var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                await task1;

                RobotFunctions.CommReply rep1 = task1.Result;
                RobotLoadAct.InAction = false;
                if (rep1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Maint Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    //inv.settxt(lblRobot, "Move Maint Fini");
                    //inv.set(btnMaintenance, "Enabled", true);
                    //ControlsEnable(true);
                    //return;
                }
                else
                {
                    //MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    inv.settxt(lblRobot, "Move Maint Error");
                    //inv.set(btnMaintenance, "Enabled", true);
                    //ControlsEnable(true);
                    return;
                }

                //footer home
                Single speed5 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed4 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[3].Ax_Vmax) / 100.0f;
                int axis = 0;
                var task = Task.Run(() => MoveFooterHome(speed5, speed4));
                await task;
                if (!task.Result.result)
                {
                    //SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("FOOTER HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    //ControlsEnable(true);
                    return;
                }
                
                inv.set(btnMaintenance, "Enabled", true);
                ControlsEnable(true);
                return;

            }
            catch (Exception err)
            {
                SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                MessageBox.Show("ROBOT1 MOVE MAINTENANCE ERROR!", "ERROR", MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                ControlsEnable(true);
                inv.set(btnMaintenance, "Enabled", true);
                return;
            }
        }

        private void BtnCycleStop_Click(object sender, EventArgs e)
        {
            MyStatic.bExitcycle = true;
            bExitcycleNoRobot = true;
            Task.Run(() => frmMain.newFrmMain.ListAdd3("=====STOP CYCLE PRESSED=========" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
            //MyStatic.bExitMaincycle = true;
        }

        private async void BtnRobotReset_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                SetTraficLights(0, 0, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                Task.Run(() => frmMain.newFrmMain.ListAdd3("RESET" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                btnRobotStart.Enabled = false;
                RobotLoadAct.InAction = false;
                InspectStationAct.InAction = false;
                InspectStationAct.VisionInAction = false;
                InspectStationAct.VisionFrontInAction = false;
                InspectStationAct.SuaInAction = false;
                FooterStationAct.AxisInAction = false;
                InspectStationAct.WeldonInAction = false;
                DeltaFront = 0;
                ErrorFront = 0;
                MyStatic.InitFini = false;
                inv.set(btnRobotStart, "BackColor", Color.LightGray);
                inv.set(btnCheckDiam, "Enabled", true);
                System.Console.Beep();
                //System.Media.SystemSounds.Beep.Play();
                ControlsEnable(false);
                btnSaveSt.Enabled = true;


                MyStatic.bExitcycle = true;
                MyStatic.bExitcycleNow = true;
                MyStatic.bEmpty = false;
                MyStatic.bReset = true;
                MyStatic.bStartcycle = false;
                MyStatic.RobotLoadInAction = false;
                MyStatic.RobotUnLoadInAction = false;
                Thread.Sleep(500);

                MyStatic.TaskExecute = false;
                //abort
                var task1 = Task.Run(() => AbortProgram("*ALL*"));
                await task1;
                reply = task1.Result;
                if (!reply.result && reply.comment == "Unable to connect to the remote server")
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR abort ROBOT TASK! " + reply.comment);
                    //ControlsEnable(true);
                    return;
                }
                //reset
                var task2 = Task.Run(() => ResetProgram());
                await task2;
                reply = task1.Result;

                if (!reply.result && reply.comment != "The remote server returned an error: (404) Not Found.")
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR RESET ROBOT TASK!");
                    ControlsEnable(true);
                    btnRobotStart.Enabled = true;
                    return;
                }
                //reset beckhoff
                ControlsEnable(false);
                MyStatic.bReset = false;
                Beckhoff.tcAds.Disconnect();
                Thread.Sleep(100);
                Beckhoff.tcAds.Connect(PlcNetID.Trim(), PlcPort);
                btnPwrOnSt.Enabled = false;
                int axis = 0;
                var task9 = Task.Run(() => RunRstSt(axis));
                await task9;

                MyStatic.bPower = false;

                CommReply reply1 = new CommReply();
                reply1.result = false;
                reply1 = task9.Result;

                if (!reply1.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR RESET! " + "\r" + reply.status);
                    return;
                }
                if (reply1.data[1] != 0) { MessageBox.Show("ERROR RESET BECKHOFF"); return; };
                Thread.Sleep(500);
                //power on
                axis = 0;
                var task10 = Task.Run(() => RunPwrSt(true, axis));
                await task10;

                MyStatic.bPower = false;

                btnPwrOnSt.Enabled = true;

                reply1.result = false;
                reply1 = task10.Result;
                //ControlsEnable(true);

                if (!(reply1.status == "" || reply1.status == null))
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR BECKHOFF POWER ON! " + "\r" + reply1.status);
                    return;
                }
                if (reply1.data[1] != 0) { MessageBox.Show("ERROR POWER ON"); return; };
                //send parameters
                var task11 = Task.Run(() => SendPlcParameters());
                await task11;
                if (!task11.Result.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR SEND PLC PARAMETERS!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                //axis max speed

                var task12 = Task.Run(() => RunAxisStatus(6));
                await task12;
                reply1 = task12.Result;
                if (!reply1.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR READ STATUS " + "\r");
                    return;
                }
                AxStatus[0].Vmax = reply1.data[3];
                AxStatus[1].Vmax = reply1.data[4];
                AxStatus[2].Vmax = reply1.data[5];
                AxStatus[3].Vmax = reply1.data[6];
                AxStatus[4].Vmax = reply1.data[7];

                Thread.Sleep(200);
                btnRobotStart.Enabled = true;
                inv.set(btnRobotStart, "BackColor", Color.LimeGreen);
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                return;

            }
            catch (Exception ex) { SetTraficLights(0, 0, 1, 0); }//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
            //ControlsEnable(true);

        }


        private async Task<RobotFunctions.CommReply> CheckComm()
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();
                Parms.cmd = ((int)MyStatic.RobotCmd.CheckComm).ToString();
                Parms.DebugTime = 1000;

                Parms.comment = "CheckComm";
                Array.Resize<Single>(ref Parms.SendParm, 10);
                Parms.timeout = 2;
                Parms.SendParm[0] = (int)MyStatic.RobotCmd.CheckComm;
                var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                await task1;

                reply = task1.Result;

                if (reply.result)
                {

                }
                else
                {

                    MessageBox.Show("ROBOT1 COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return reply;
                }

                //send parameters


                reply.result = true;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("ROBOT COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply CheckComm1(int comm = 1)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {


                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.CheckComm).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckComm;
                Parms.comment = "Check comm";
                Parms.timeout = 3;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                //16
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = 3.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();
                WebComm.CommReply rep2 = new WebComm.CommReply();
                if (comm == 1)
                {
                    Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CheckComm;
                    rep1 = WC1.RunCmd(Parms);

                    if (!rep1.result)
                    {
                        reply.result = false;
                        MessageBox.Show("Vision COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else reply.result = true;
                }
                else if (comm == 2)
                {
                    Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CheckFrontCam;
                    rep2 = WC2.RunCmd(Parms);

                    if (!rep2.result)
                    {
                        reply.result = false;
                        MessageBox.Show("FRONT CAMERA COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else reply.result = true;
                }
                else if (comm == 3)
                {
                    Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CheckCognex;
                    rep2 = WC3.RunCmd(Parms);

                    if (!rep2.result)
                    {
                        reply.result = false;
                        MessageBox.Show("COGNEX COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else reply.result = true;
                }

                //if (rep1.result) reply.result = true;
                //else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("vision COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply DataToVision()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                //if(chkVisionSim.Checked)
                //{
                //    reply.result = true;
                //    return reply;
                //}
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.DataToVision).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.DataToVision;
                Parms.comment = "data to vision";
                Parms.timeout = 3;
                //NPNPP
                //Array.Resize<Single>(ref Parms.SendParm, 8);
                Array.Resize<Single>(ref Parms.SendParm, 9);
                //16
                Parms.SendParm[1] = Single.Parse(txtOrder.Text);
                Parms.SendParm[2] = Single.Parse(txtItem.Text);
                Parms.SendParm[3] = Single.Parse(txtPartDiam.Text);
                Parms.SendParm[4] = Single.Parse(txtPartLength.Text);
                Parms.SendParm[5] = Single.Parse(txtPartLengthU.Text);
                Parms.SendParm[6] = Inspected_PartID + 1;
                Parms.SendParm[7] = (chkColor.Checked ? 100f : 0f);
                Parms.SendParm[8] = 3.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();
                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.DataToVision;
                rep1 = WC1.RunCmd(Parms);

                if (!rep1.result)
                {
                    reply.result = false;
                    MessageBox.Show("DataToVision ERROR!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else reply.result = true;


                //if (rep1.result) reply.result = true;
                //else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("data to vision ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply DataVisionSave()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                //if (chkVisionSim.Checked)
                //{
                //    reply.result = true;
                //    return reply;
                //}
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.DataVisionSave).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.DataVisionSave;
                Parms.comment = "data vision save";
                Parms.timeout = 3;
                Array.Resize<Single>(ref Parms.SendParm, 8);
                //16
                
                Parms.SendParm[7] = 3.0f;// 0.5f;//timeout
                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                WebComm.CommReply rep1 = new WebComm.CommReply();


                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.DataVisionSave;
                rep1 = WC1.RunCmd(Parms);

                if (!rep1.result)
                {
                    reply.result = false;
                    MessageBox.Show("DataVisionSave ERROR!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else reply.result = true;


                //if (rep1.result) reply.result = true;
                //else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("data vision save ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply DataToFront()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                //if (chkVisionSim.Checked)
                //{
                //    reply.result = true;
                //    return reply;
                //}
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.DataToFront).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.DataToFront;
                Parms.comment = "data to front";
                Parms.timeout = 2;
                Array.Resize<Single>(ref Parms.SendParm, 8);
                //16
                Parms.SendParm[1] = Single.Parse(txtOrder.Text);
                Parms.SendParm[2] = Single.Parse(txtItem.Text);
                Parms.SendParm[3] = Single.Parse(txtPartDiam.Text);
                Parms.SendParm[4] = Single.Parse(txtPartLength.Text);
                Parms.SendParm[5] = Single.Parse(txtPartLengthU.Text);
                Parms.SendParm[6] = Inspected_PartID + 1;
                Parms.SendParm[7] = 2.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();
                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.DataToFront;
                rep1 = WC2.RunCmd(Parms);

                if (!rep1.result)
                {
                    reply.result = false;
                    MessageBox.Show("DataToFront ERROR!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else reply.result = true;


                //if (rep1.result) reply.result = true;
                //else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("data to front ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply DataFrontSave()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                //if (chkVisionSim.Checked)
                //{
                //    reply.result = true;
                //    return reply;
                //}
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.DataFrontSave).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.DataFrontSave;
                Parms.comment = "data front save";
                Parms.timeout = 2;
                Array.Resize<Single>(ref Parms.SendParm, 8);
               
                Parms.SendParm[7] = 2.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();
                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.DataFrontSave;
                rep1 = WC2.RunCmd(Parms);

                if (!rep1.result)
                {
                    reply.result = false;
                    MessageBox.Show("DataFrontSave ERROR!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else reply.result = true;


                //if (rep1.result) reply.result = true;
                //else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("data front save ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply StartCycleInspectVision(bool withCognex)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.StartCycle).ToString();
                Parms.DebugTime = 1000;

                int cmd = 0;
                if (withCognex) cmd = (int)MyStatic.InspectCmd.StartCycle; else cmd = (int)MyStatic.InspectCmd.Startvision;

                Parms.FunctionCode = cmd;
                Parms.comment = "Start Cycle Vision";
                Parms.timeout = 20;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                Parms.SendParm[0] = cmd;
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = 20.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = WC1.RunCmd(Parms);

                if (rep1.result)
                {

                }
                else
                {

                    MessageBox.Show("Vision COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return rep1;
                }

                //send parameters


                //reply.result = true;
                return rep1;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Vision COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply StartWaitInspectCognex()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.StartWaitSua).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.StartWaitSua;
                Parms.comment = "Start Cycle Cognex";
                Parms.timeout = 40;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.StartWaitSua;
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = 40.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = WC1.RunCmd(Parms);

                if (rep1.result && rep1.comment != null)
                {
                    string[] ss = rep1.comment.Split(',');
                    rep1.status = "";
                    if (ss.Length == 5 && ss[0] == "cmd83" && ss[1] == "1")
                    {
                        rep1.status = ss[2] + "," + ss[3];
                        return rep1;
                    }
                    else
                    {
                        rep1.status = "";
                        rep1.result = false;
                        return rep1;
                    }

                }
                else
                {

                    MessageBox.Show("Cognex COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return rep1;
                }

                //send parameters


                //reply.result = true;
                return rep1;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Cognex COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply StopCycleInspect()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {


                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.StopCycle).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.StopCycle;
                Parms.comment = "Stop Cycle Vision";
                Parms.timeout = 20;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.StopCycle;
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = 20.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = WC1.RunCmd(Parms);

                if (rep1.result)
                {

                }
                else
                {

                    MessageBox.Show("Vision COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return rep1;
                }
                //WebComm.CommReply rep2 = WC2.RunCmd(Parms);

                //if (rep2.result)
                //{

                //}
                //else
                //{

                //    MessageBox.Show("Cognex COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                //                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //    return rep1;
                //}

                //send parameters


                reply.result = true;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("vision COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }
        //private WebComm.CommReply StartSua()
        //{

        //    WebComm.CommReply reply = new WebComm.CommReply();
        //    try
        //    {


        //        WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
        //        Parms.cmd = ((int)MyStatic.InspectCmd.StartSua).ToString();
        //        Parms.DebugTime = 1000;
        //        Parms.FunctionCode = (int)MyStatic.InspectCmd.StartSua;
        //        Parms.comment = "Start Sua";
        //        Parms.timeout = 2;
        //        Array.Resize<Single>(ref Parms.SendParm, 3);
        //        Parms.SendParm[0] = (Single)MyStatic.InspectCmd.StartSua;
        //        Parms.SendParm[1] = 1;// general speed
        //        Parms.SendParm[2] = 20.0f;// 0.5f;//timeout
        //        WebComm.CommReply rep1 = WC2.RunCmd(Parms);

        //        if (rep1.result)
        //        {

        //        }
        //        else
        //        {

        //            MessageBox.Show("Cognex COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        //            return rep1;
        //        }

        //        //send parameters


        //        reply.result = true;
        //        return reply;

        //    }
        //    catch (Exception err)
        //    {
        //        reply.result = false;

        //        MessageBox.Show("Cognex COMMUNICATION ERROR:" + err.Message);
        //        return reply;
        //    }
        //}
        //private async Task<WebComm.CommReply> StopSua()
        //{

        //    WebComm.CommReply reply = new WebComm.CommReply();
        //    try
        //    {


        //        WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
        //        Parms.cmd = ((int)MyStatic.InspectCmd.StopSua).ToString();
        //        Parms.DebugTime = 1000;
        //        Parms.FunctionCode = (int)MyStatic.InspectCmd.StopSua;
        //        Parms.comment = "Stop Sua";
        //        Parms.timeout = 2;
        //        Array.Resize<Single>(ref Parms.SendParm, 3);
        //        Parms.SendParm[0] = (Single)MyStatic.InspectCmd.StopSua;
        //        Parms.SendParm[1] = 1;// general speed
        //        Parms.SendParm[2] = 20.0f;// 0.5f;//timeout
        //        WebComm.CommReply rep1 = WC2.RunCmd(Parms);

        //        if (rep1.result)
        //        {

        //        }
        //        else
        //        {

        //            MessageBox.Show("Cognex COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        //            return rep1;
        //        }

        //        //send parameters


        //        reply.result = true;
        //        return reply;

        //    }
        //    catch (Exception err)
        //    {
        //        reply.result = false;

        //        MessageBox.Show("Cognex COMMUNICATION ERROR:" + err.Message);
        //        return reply;
        //    }
        //}

        private async Task<RobotFunctions.CommReply> SendData()
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 13);
                Parms.cmd = ((int)MyStatic.RobotCmd.Parm).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.Parm;//10
                Parms.comment = "Send Parameteres";
                Parms.timeout = 2;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.Parm;//10
                Parms.SendParm[1] = (Single)frmMain.newFrmMain.FanucSpeed;// general speed;
                Parms.SendParm[2] = Robot1data.NormalSpeed;
                Parms.SendParm[3] = Robot1data.PickSpeed;
                Parms.SendParm[4] = Robot1data.PlaceSpeed;
                Parms.SendParm[5] = Robot1data.RobotAbove;
                Parms.SendParm[6] = Robot1data.RobotAbovePick;
                Parms.SendParm[7] = Robot1data.RobotAbovePlace;
                Parms.SendParm[8] = Robot1data.CheckGrip;
                Parms.SendParm[9] = Robot1data.DelayPick;
                Parms.SendParm[10] = Robot1data.DelayPlace;
                if (chkCleanAir.Checked) Parms.SendParm[11] = 1;// Robot1data.Step;//clean air
                else Parms.SendParm[11] = 0;
                Parms.SendParm[12] = 2;// 0.5f;//timeout
                var task3 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                await task3;
                RobotFunctions.CommReply rep3 = task3.Result;
                if (rep3.result)
                {

                }
                else
                {
                    //MyStatic.bExitcycle = true;
                    //MyStatic.bReset = true;
                    rep.result = false;
                    //MessageBox.Show("ERROR SEND ROBOT1 DATA!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    //RobotLoadAct.InAction = false;

                    return rep3;
                }

                //ControlsEnable(true);

            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ROBOT SEND DATA ERROR:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> SendTool()
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {

                if (MyStatic.Robot == MyStatic.RobotLoad)
                {

                    FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                    Array.Resize<Single>(ref Parms.SendParm, 6);
                    Parms.cmd = ((int)MyStatic.RobotCmd.SetTool).ToString();
                    Parms.FunctionCode = (int)MyStatic.RobotCmd.SetTool;//10
                    Parms.comment = "Send Tool";
                    Parms.timeout = 2;

                    Parms.SendParm[0] = (int)MyStatic.RobotCmd.SetTool;//10
                    Parms.SendParm[1] = (Single)Robot1data.ToolOffX;
                    Parms.SendParm[2] = (Single)Robot1data.ToolOffY;
                    Parms.SendParm[3] = (Single)Robot1data.ToolOffZ;
                    Parms.SendParm[4] = (Single)Robot1data.ToolOffR;
                    Parms.SendParm[5] = 2;// 0.5f;//timeout
                    var task3 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                    await task3;
                    RobotFunctions.CommReply rep3 = task3.Result;
                    if (rep3.result)
                    {

                    }
                    else
                    {
                        rep.result = false;
                        return rep3;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ROBOT SEND DATA ERROR:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }

        private RobotFunctions.CommReply ReadPosition(int robot, int id)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 3);
                Parms.cmd = ((int)MyStatic.RobotCmd.ReadPos).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.ReadPos;//18
                Parms.comment = "READ POSITION";
                Parms.timeout = 2;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.ReadPos;//18
                Parms.SendParm[1] = id;
                Parms.SendParm[2] = 2;// 0.5f;//timeout
                if (robot == MyStatic.RobotLoad)
                {
                    RobotFunctions.CommReply rep1 = FW1.RunCmdFanuc(Parms);

                    if (rep1.result)
                    {
                        return rep1;
                    }
                    else
                    {

                        rep.result = false;
                        MessageBox.Show("ERROR READ POSITION " + id.ToString() + "!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                        return rep1;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR READ POSITION:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> WritePosition(int robot, int id, position pos)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 9);
                Parms.cmd = ((int)MyStatic.RobotCmd.WritePos).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.WritePos;//11
                Parms.comment = "WRITE POSITION";
                Parms.timeout = 2;
                if (MyStatic.Robot == MyStatic.RobotLoad)
                    Parms.SendParm[0] = (int)MyStatic.RobotCmd.WritePos;//11
                Parms.SendParm[1] = id;
                Parms.SendParm[2] = (Single)pos.x;
                Parms.SendParm[3] = (Single)pos.y;
                Parms.SendParm[4] = (Single)pos.z;
                Parms.SendParm[7] = (Single)pos.r;

                Parms.SendParm[8] = 2;// 0.5f;//timeout
                if (robot == MyStatic.RobotLoad)
                {
                    var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                    await task1;
                    RobotFunctions.CommReply rep1 = task1.Result;
                    if (rep1.result)
                    {
                        return rep1;
                    }
                    else
                    {

                        rep.result = false;
                        MessageBox.Show("ERROR WRITE POSITION " + id.ToString() + "!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                        return rep1;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR WRITE POSITION:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> WriteCurrentPosition(int robot, int id)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 9);
                Parms.cmd = ((int)MyStatic.RobotCmd.WritePos).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.WritePos;//11
                Parms.comment = "SAVE POSITION";
                Parms.timeout = 2;
                if (MyStatic.Robot == MyStatic.RobotLoad)
                    Parms.SendParm[0] = (int)MyStatic.RobotCmd.WritePos;//11
                Parms.SendParm[1] = id;
                Parms.SendParm[2] = 19;
                

                Parms.SendParm[8] = 2;// 0.5f;//timeout
                if (robot == MyStatic.RobotLoad)
                {
                    var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                    await task1;
                    RobotFunctions.CommReply rep1 = task1.Result;
                    if (rep1.result)
                    {
                        return rep1;
                    }
                    else
                    {

                        rep.result = false;
                        MessageBox.Show("ERROR SAVE CURRENT POSITION IN PR" + id.ToString() + "!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                        return rep1;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR SAVE POSITION:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> WriteReg(int robot, int id, Single data)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 4);
                Parms.cmd = ((int)MyStatic.RobotCmd.WriteReg).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.WriteReg;//22
                Parms.comment = "WRITE REG";
                Parms.timeout = 2;
                if (MyStatic.Robot == MyStatic.RobotLoad)
                    Parms.SendParm[0] = (int)MyStatic.RobotCmd.WriteReg;//22
                Parms.SendParm[1] = id;
                Parms.SendParm[2] = data;


                Parms.SendParm[3] = 2;// 0.5f;//timeout
                if (robot == MyStatic.RobotLoad)
                {
                    var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                    await task1;
                    RobotFunctions.CommReply rep1 = task1.Result;
                    if (rep1.result)
                    {
                        return rep1;
                    }
                    else
                    {

                        rep.result = false;
                        MessageBox.Show("ERROR WRITE REG " + id.ToString() + "!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                        return rep1;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR WRITE REG:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> ReadIO(int robot)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 2);
                Parms.cmd = ((int)MyStatic.RobotCmd.readIO).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.readIO;//14
                Parms.comment = "READ IO";
                Parms.timeout = 2;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.readIO;//14
                Parms.SendParm[1] = 2;// 0.5f;//timeout
                if (robot == MyStatic.RobotLoad)
                {
                    var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                    await task1;
                    RobotFunctions.CommReply rep1 = task1.Result;
                    if (rep1.result)
                    {
                        return rep1;
                    }
                    else
                    {

                        rep.result = false;
                        MessageBox.Show("ERROR READ IO " + "!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                        return rep1;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR READ IO:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> SetOutput1(int robot, int output, int state, int ro, int rostate)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();

            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                Array.Resize<Single>(ref Parms.SendParm, 6);
                Parms.cmd = ((int)MyStatic.RobotCmd.setIO).ToString();
                Parms.FunctionCode = (int)MyStatic.RobotCmd.setIO;//15
                Parms.comment = "SET IO";
                Parms.timeout = 2;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.setIO;//15
                Parms.SendParm[1] = output;
                Parms.SendParm[2] = state;
                Parms.SendParm[3] = ro;
                Parms.SendParm[4] = rostate;
                Parms.SendParm[5] = 2;// 0.5f;//timeout
                if (robot == MyStatic.RobotLoad)
                {
                    var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));
                    await task1;
                    RobotFunctions.CommReply rep1 = task1.Result;
                    if (rep1.result)
                    {
                        return rep1;
                    }
                    else
                    {

                        rep.result = false;
                        MessageBox.Show("ERROR READ IO " + "!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        MyStatic.ReadingIO = false;
                        return rep1;
                    }
                }


            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR READ IO:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        private async Task<RobotFunctions.CommReply> SetOutput(int robot, int output, int state, int ro, int rostate)
        {
            RobotFunctions.CommReply rep = new RobotFunctions.CommReply();
            RobotFunctions.CommReply rep1 = new RobotFunctions.CommReply();

            try
            {
                if (output > 0)
                {
                    rep = FW1.ReadFanucIOAsync("dout", (output + 100).ToString(), "set", state.ToString(), 500);

                }
                else rep.result = true;
                if (ro > 0)
                {
                    rep1 = FW1.ReadFanucIOAsync("dout", ro.ToString(), "setro", rostate.ToString(), 500);

                }
                else rep1.result = true;
                if (rep.result && rep1.result) rep.result = true; else rep.result = false;
                return rep;



            }
            catch (Exception err)
            {
                //MyStatic.bExitcycle = true;
                //MyStatic.bReset = true;
                rep.result = false;
                MessageBox.Show("ERROR READ IO:" + err.Message);
                return rep;
            }
            //ControlsEnable(true);
            rep.result = true;
            return rep;
        }
        public Single FanucSpeed = 5;

        private void TrackSpeed_ValueChanged(object sender, EventArgs e)
        {
            Single speed = 0;
            FanucSpeed = 5;
            if (!Single.TryParse(frmMain.newFrmMain.trackSpeed.Value.ToString(), out speed))
            {
                FanucSpeed = 5;
                txtSpeed.Text = FanucSpeed.ToString();
                //frmMain.newFrmMain.trackSpeed.Value = 5;
                return;
            }
            if (speed >= 5 && speed <= 100) FanucSpeed = Single.Parse(frmMain.newFrmMain.trackSpeed.Value.ToString());
            else
            {
                FanucSpeed = 5;
                txtSpeed.Text = FanucSpeed.ToString();
                //frmMain.newFrmMain.trackSpeed.Value = 5;
                return;
            }
            txtSpeed.Text = FanucSpeed.ToString();
        }

        //public MyStatic.Station[] CNCstation = new MyStatic.Station[12];
        //public MyStatic.Station FlipStation = new MyStatic.Station();
        public int StIndex = 0;


        public bool Rob2PlaceFlip = false;
        public string CurrentPlcCommand = "";


        private void ChkDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (frmMain.newFrmMain.chkDebug.Checked == true) MyStatic.chkDebug = true; else MyStatic.chkDebug = false;
        }







        private void LstRobotsLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Uri urlrobot1 = new Uri(surlrobot1);//("http://192.168.0.20/");// html file on FR:read reg

        int RegStartWrite = 150;//11 data reg
        int RegStartRead = 170;//11 data reg
        FanucWeb FW1 = new FanucWeb();
        static WebComm WC1 = new WebComm();//vision
        static WebComm WC2 = new WebComm();//front
        static WebComm WC3 = new WebComm();//cognex
        //FanucWeb FW2 = new FanucWeb();
        private void btnRobot_Click(object sender, EventArgs e)
        {

            try
            {

                WB1.AllowNavigation = true;
                WB1.Navigate(urlrobot1);//show fanuc form
            }
            catch (Exception ex)
            {
                MessageBox.Show("error Robot:" + ex.Message);
                WB1.Stop();
                WB1.Url = new Uri("about:blank");
            }
        }

        private void btnGOB_Click(object sender, EventArgs e)
        {
            WB1.GoBack();
        }

        private void btnGOF_Click(object sender, EventArgs e)
        {
            WB1.GoForward();
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                var task = Task.Run(() => FW1.WriteFanucRegAsync(cmbReg.Text, txtRegNum.Text, txtRegVal.Text, 500));
                await task;
                reply = task.Result;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error swrite:" + ex.Message);

            }
            return;
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            try
            {
                RobotFunctions.CommReply reply = new RobotFunctions.CommReply();

                var task = Task.Run(() => FW1.ReadFanucS2RegAsync(1000));
                await task;
                reply = task.Result;
                //RobotFunctions.CommReply reply1 =  FW.ReadFanucS2Reg();
            }
            catch (Exception ex)
            {
            }

            return;
        }
        public bool StopCycle = false;
        private RobotFunctions.CommReply RunCmd(string cmd, Single[] data, int timeout = 0, int robot = 0)
        {//start robot cycle
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            if (robot == 0)
            {
                MessageBox.Show("ERROR ROBOT CMD");
                reply.result = false;
                return reply;
            }
            try
            {
                reply.result = false;
                string reg = "strreg";
                string num = "1";
                string val = "";
                for (int i = 0; i < data.Length; i++)
                {
                    val = val + data[i].ToString() + ",";
                }
                val = cmd + "," + val + "end";
                // string val = "cmd10,5,6,7,end";//robot command
                if (robot == 1)
                {
                    reply = FW1.WriteFanucRegAsync(reg, num, val, 1000);

                    if (!reply.result) { return reply; }

                    //wait result
                    reply = FW1.ReadFanucS2RegAsync(10000);

                    return reply;
                }
                else if (robot == 2)
                {
                    reply = FW1.WriteFanucRegAsync(reg, num, val, 1000);

                    if (!reply.result) { return reply; }

                    //wait result
                    reply = FW1.ReadFanucS2RegAsync(10000);

                    return reply;
                }
            }
            catch (Exception ex)
            {
                reply.result = false;
                return reply;

            }
            return reply;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            //start robot cycle
            StopCycle = false;
            Single j = 1.1f;
            try
            {
                RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
                while (!StopCycle)
                {
                    j++;
                    string cmd = "cmd10";
                    Single[] data = new Single[5];
                    data[0] = 5 + j;
                    data[1] = 6 + j;
                    data[2] = 7 + j;
                    data[3] = 8 + j;
                    data[4] = 9 + j;
                    int timeout = 10000;//10 sec
                    var task = Task.Run(() => RunCmd(cmd, data, timeout, 1));
                    await task;
                    reply = task.Result;
                    if (reply.result)
                    {
                        if ("cmd" + reply.data[0].ToString() != cmd)
                        {
                            MessageBox.Show("ERROR1 CMD");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("ERROR CMD");
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error swrite:" + ex.Message);
                return;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StopCycle = true;
        }

        private void WB1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WB1.Document.Body.Style = "zoom:" + comboBox1.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WB1.Document.Body.Style = "zoom:" + comboBox1.Text;
        }









        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyStatic.Robot == MyStatic.RobotLoad)
                {
                    Robot1data.NormalSpeed = Single.Parse(txtNormSpeed.Text);
                    Robot1data.SpeedOvr = Single.Parse(txtSpeed.Text);
                    Robot1data.PickSpeed = Single.Parse(txtPickSpeed.Text);
                    Robot1data.PlaceSpeed = Single.Parse(txtPlaceSpeed.Text);
                    Robot1data.RobotAbove = Single.Parse(txtAbove.Text);
                    Robot1data.RobotAbovePick = Single.Parse(txtAbovePick.Text);
                    Robot1data.RobotAbovePlace = Single.Parse(txtAbovePlace.Text);
                    Robot1data.DelayPick = Single.Parse(txtDelayPick.Text);
                    Robot1data.DelayPlace = Single.Parse(txtDelayPlace.Text);
                    if (chkStep.Checked) Robot1data.Step = 1; else Robot1data.Step = 0;
                    if (chkCheckGripper.Checked) Robot1data.CheckGrip = 1; else Robot1data.CheckGrip = 0;
                    //if (chkInverseLock.Checked) Robot1data.InvLock = 1; else Robot1data.InvLock = 0;
                    Robot1data.Gripper = cmbGripper.Text;

                }


                SaveIni();
                SaveItem();
                //SaveAxis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Save:" + ex.Message);
            }
        }

        private async void btnReadCurr_Click(object sender, EventArgs e)
        {
            try
            {
                txtCurrX.Text = "";
                txtCurrY.Text = "";
                txtCurrZ.Text = "";
                txtCurrR.Text = "";

                MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;
                int robot = 0;
                int id = 0;
                if (MyStatic.Robot == MyStatic.RobotLoad)
                {
                    robot = MyStatic.RobotLoad;
                    id = RobotLoadPoints.Currpos.id;
                }

                //var task1 = ReadPosition(robot, id);
                //await task1;
                //RobotFunctions.CommReply reply = task1.Result;
                //if (!reply.result)
                //{
                //    MyStatic.bExitcycle = true;
                //    MyStatic.bReset = true;
                //    RobotLoadAct.InAction = false;
                //    RobotUnloadAct.InAction = false;
                //    return;
                //}
                var task1 = Task.Run(() => ReadCurPos(RobotLoadPoints.Currpos.id));
                await task1;
                RobotFunctions.CommReply reply = task1.Result;
                if (!reply.result || reply.data == null || reply.data[0] != 71 || reply.data[1] != 1)
                {
                    MyStatic.bExitcycle = true;
                    MyStatic.bReset = true;
                    RobotLoadAct.InAction = false;

                    MessageBox.Show("READ POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return;
                }
                position pos = new position();
                pos.x = reply.data[2];
                pos.y = reply.data[3];
                pos.z = reply.data[4];
                pos.r = reply.data[7];
                txtCurrX.Text = reply.data[2].ToString("0.00");
                txtCurrY.Text = reply.data[3].ToString("0.00");
                txtCurrZ.Text = reply.data[4].ToString("0.00");
                txtCurrR.Text = reply.data[7].ToString("0.00");
                ControlsEnable(true);

            }
            catch (Exception err)
            {
                MessageBox.Show("ROBOT READ POSITION ERROR:" + err.Message);
            }
            //ControlsEnable(true);

        }

        private async void btnTrayCorr_Click(object sender, EventArgs e)
        {
            //-----read tray corrections--------
            //execute in step:send to robot PR[temp] coord
            //and send menu button R[65] R[66]
            //go to TP and run Macro
            //jog robot to position
            //read current pos and compare with send coord

            try
            {

                MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;
                int robot = 0;
                int id = 0;//base position
                position basepos = new position();
                if (MyStatic.Robot == MyStatic.RobotLoad)
                {
                    robot = MyStatic.RobotLoad;
                    id = RobotLoadPoints.TempPos.id;
                }

                var task1 = WritePosition(robot, id, basepos);
                await task1;
                RobotFunctions.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    MyStatic.bExitcycle = true;
                    MyStatic.bReset = true;
                    RobotLoadAct.InAction = false;

                    return;
                }
                txtCurrX.Text = reply.data[2].ToString("0.00");
                txtCurrY.Text = reply.data[3].ToString("0.00");
                txtCurrZ.Text = reply.data[4].ToString("0.00");
                txtCurrR.Text = reply.data[7].ToString("0.00");
                ControlsEnable(true);

            }
            catch (Exception err)
            {
                MessageBox.Show("ROBOT READ POSITION ERROR:" + err.Message);
            }
            //ControlsEnable(true);
        }
        position basepos = new position();
        position baseOrg = new position();
        int action = 0;
        private async void btnExecute_Click(object sender, EventArgs e)
        {
            //-----read tray corrections--------
            //execute in step:send to robot PR[temp] coord
            //and send menu button R[65] R[66]
            //go to TP and run Macro
            //jog robot to position
            //read current pos and compare with send coord

            try
            {

                MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;
                inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
                FanucSpeed = trackSpeed.Value;
                int robot = 0;
                int id = 0;//base position
                int teach = 0;
                action = 0;
                

                if (cmbSingleMove.Text == "Pick Tray") action = 1;
                else if (cmbSingleMove.Text == "Place Inspection") action = 2;
                else if (cmbSingleMove.Text == "Pick Inspection") action = 3;
                else if (cmbSingleMove.Text == "Place Tray") action = 4;
                else if (cmbSingleMove.Text == "Place Reject") action = 5;
                else if (cmbSingleMove.Text == "Air Clean") action = 6;

                basepos = new position();
                baseOrg = new position();
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                switch (action)
                {
                    case 1://pick tray

                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickTrayOrg.id;
                        RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                        RobotLoadPoints.PickTray = RobotLoadPoints.PickTrayOrg;
                        int partid = TrayPartId;
                        var task1 = Task.Run(() => GetPickTray(partid, false));
                        await task1;
                        RobotFunctions.CommReply commreply = task1.Result;
                        if (commreply.result && commreply.data.Length >= 4)
                        {
                            baseOrg = RobotLoadPoints.PickTrayOrg;//with pallet without corrections
                            baseOrg.x = commreply.data[0];
                            baseOrg.y = commreply.data[1];
                            baseOrg.z = commreply.data[2];
                            baseOrg.r = commreply.data[3];
                            basepos.x = baseOrg.x + (Single)UpDwnX3.UpDownValue;
                            basepos.y = baseOrg.y + (Single)UpDwnY3.UpDownValue;
                            basepos.z = baseOrg.z + (Single)UpDwnZ3.UpDownValue;
                            basepos.r = baseOrg.r + (Single)UpDwnR3.UpDownValue;
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        var task = Task.Run(() => PickFromTray(basepos));
                        await task;
                        fini = task.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = 1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Pick Tray" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Pick Tray");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            if (chkStep.Checked)
                            {
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            }
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PICK FROM TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 2://Place Inspection
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PlaceInspect.id;
                        fini = new RobotFunctions.CommReply();
                        baseOrg = RobotLoadPoints.PlaceInspect;//with pallet without corrections
                        basepos.x = RobotLoadPoints.PlaceInspect.x + (Single)UpDwnX4.UpDownValue;
                        basepos.y = RobotLoadPoints.PlaceInspect.y + (Single)UpDwnY4.UpDownValue;
                        basepos.z = RobotLoadPoints.PlaceInspect.z + (Single)UpDwnZ4.UpDownValue;
                        basepos.r = RobotLoadPoints.PlaceInspect.r + (Single)UpDwnR4.UpDownValue;
                        var task10 = Task.Run(() => PlacePartInsp(basepos));
                        await task10;
                        fini = task10.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Place Inspection");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            if (chkStep.Checked)
                            {
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            }
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLace Inspection ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 3://Pick Inspection
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickInspect.id;
                        fini = new RobotFunctions.CommReply();
                        baseOrg = RobotLoadPoints.PickInspect;//with pallet without corrections
                        basepos.x = RobotLoadPoints.PickInspect.x + (Single)UpDwnX5.UpDownValue;
                        basepos.y = RobotLoadPoints.PickInspect.y + (Single)UpDwnY5.UpDownValue;
                        basepos.z = RobotLoadPoints.PickInspect.z + (Single)UpDwnZ5.UpDownValue;
                        basepos.r = RobotLoadPoints.PickInspect.r + (Single)UpDwnR5.UpDownValue;
                        var task11 = Task.Run(() => PickPartInsp(basepos));
                        await task11;
                        fini = task11.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip2_PartID = 1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Pick Inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Pick Inspection");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            if (chkStep.Checked)
                            {
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            }
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PICK INSPECTION ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;

                    case 4://place tray
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PlaceTrayOrg.id;

                        fini = new RobotFunctions.CommReply();

                        /////////////////////
                        RobotLoadPoints.PlaceTray = RobotLoadPoints.PlaceTrayOrg;
                        partid = TrayPartId;
                        var task2 = Task.Run(() => GetPlaceTray(partid, false));
                        await task2;
                        commreply = task2.Result;
                        if (commreply.result && commreply.data.Length >= 4)
                        {
                            baseOrg = RobotLoadPoints.PlaceTrayOrg;//with pallet without corrections
                            baseOrg.x = commreply.data[0];
                            baseOrg.y = commreply.data[1];
                            baseOrg.z = commreply.data[2];
                            baseOrg.r = commreply.data[3];
                            basepos.x = baseOrg.x + (Single)UpDwnX6.UpDownValue;
                            basepos.y = baseOrg.y + (Single)UpDwnY6.UpDownValue;
                            basepos.z = baseOrg.z + (Single)UpDwnZ6.UpDownValue;
                            basepos.r = baseOrg.r + (Single)UpDwnR6.UpDownValue;
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        //////////////
                        var task12 = Task.Run(() => PlaceTray(basepos));
                        await task12;
                        fini = task12.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip2_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Tray" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Place Tray");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            if (chkStep.Checked)
                            {
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            }
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLACE TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 5://place reject
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickTrayOrg.id;
                        fini = new RobotFunctions.CommReply();

                        RobotLoadPoints.PlaceReject = RobotLoadPoints.PlaceRejectOrg;
                        var task3 = Task.Run(() => GetRejectTray(false));
                        await task3;
                        commreply = task3.Result;
                        if (commreply.result && commreply.data.Length >= 4)
                        {
                            baseOrg = RobotLoadPoints.PlaceTrayOrg;//with pallet without corrections
                            baseOrg.x = commreply.data[0];
                            baseOrg.y = commreply.data[1];
                            baseOrg.z = commreply.data[2];
                            baseOrg.r = commreply.data[3];
                            basepos.x = baseOrg.x + (Single)UpDwnX7.UpDownValue;
                            basepos.y = baseOrg.y + (Single)UpDwnY7.UpDownValue;
                            basepos.z = baseOrg.z + (Single)UpDwnZ7.UpDownValue;
                            basepos.r = baseOrg.r + (Single)UpDwnR7.UpDownValue;
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        var task13 = Task.Run(async () => PlaceReject(basepos));
                        await task13;
                        fini = task13.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip2_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Pick Tray");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            if (chkStep.Checked)
                            {
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            }
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLACE REJECT ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 6://air clean

                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickTrayOrg.id;
                        fini = new RobotFunctions.CommReply();
                        //RobotLoadPoints.PickTray = RobotLoadPoints.PickTrayOrg;

                        Single length = 0;
                        if(txtPartLength.Text.Trim() !="" )
                        {
                            length = Single.Parse(txtPartLength.Text);
                        }
                        if (length == 0) length = 150;
                        Single speedclean = 300;
                        var task14 = Task.Run(() => AirClean(length , speedclean));
                        await task14;
                        fini = task14.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = 1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Air clean" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Air Clean");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT Air Clean ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                }
                //read current pos
                //return;
                if (chkStep.Checked && teach == 1)
                {

                    if (MyStatic.Robot == MyStatic.RobotLoad)
                    {
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.Currpos.id;
                    }

                    //var task1 = ReadPosition(robot, id);
                    //await task1;
                    var task1 = Task.Run(() => ReadCurPos(RobotLoadPoints.Currpos.id));
                    await task1;
                    RobotFunctions.CommReply reply = task1.Result;
                    if (!reply.result)
                    {
                        MyStatic.bExitcycle = true;
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0

                        MessageBox.Show("READ POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    position pos = new position();
                    pos.x = reply.data[2];
                    pos.y = reply.data[3];
                    pos.z = reply.data[4];
                    pos.r = reply.data[7];
                    Double dx = -baseOrg.x + pos.x;
                    Double dy = -baseOrg.y + pos.y;
                    Double dz = -baseOrg.z + pos.z;
                    Double dr = -baseOrg.r + pos.r;
                    if (Math.Abs(dx) > 30 || Math.Abs(dy) > 30 || Math.Abs(dz) > 50)
                    {

                        MessageBox.Show("CORRECTIONS TOO LARGE. EXIT!" + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r'
                            , "ERROR", MessageBoxButtons.OK,
                                                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        return;
                    }
                    switch (action)
                    {

                        case 1:

                            DialogResult res = MessageBox.Show("Save Pick Corrections? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res == DialogResult.Yes)
                            {
                                inv.set(UpDwnX3, "UpDownValue", (Single)dx);
                                inv.set(UpDwnY3, "UpDownValue", (Single)dy);
                                inv.set(UpDwnZ3, "UpDownValue", (Single)dz);
                                inv.set(UpDwnR3, "UpDownValue", (Single)dr);
                                RobotLoadPoints.PickTrayOrg.corrX = Single.Parse(dx.ToString("0.00"));
                                RobotLoadPoints.PickTrayOrg.corrY = Single.Parse(dy.ToString("0.00"));
                                RobotLoadPoints.PickTrayOrg.corrZ = Single.Parse(dz.ToString("0.00"));
                                RobotLoadPoints.PickTrayOrg.corrR = Single.Parse(dr.ToString("0.00"));
                            }
                            break;
                        case 2:

                            DialogResult res1 = MessageBox.Show("Save Place Inspection Corrections? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res1 == DialogResult.Yes)
                            {
                                UpDwnX4.UpDownValue = (Single)dx;
                                UpDwnY4.UpDownValue = (Single)dy;
                                UpDwnZ4.UpDownValue = (Single)dz;
                                UpDwnR4.UpDownValue = (Single)dr;
                                RobotLoadPoints.PlaceInspect.corrX = Single.Parse(dx.ToString("0.00"));
                                RobotLoadPoints.PlaceInspect.corrY = Single.Parse(dy.ToString("0.00"));
                                RobotLoadPoints.PlaceInspect.corrZ = Single.Parse(dz.ToString("0.00"));
                                RobotLoadPoints.PlaceInspect.corrR = Single.Parse(dr.ToString("0.00"));
                            }
                            break;
                        case 3:

                            res1 = MessageBox.Show("Save Pick Inspection Corrections? " + '\r' +
                           "Correction X = " + dx.ToString("0.00") + '\r' +
                           "Correction Y = " + dy.ToString("0.00") + '\r' +
                           "Correction Z = " + dz.ToString("0.00") + '\r' +
                           "Correction R = " + dr.ToString("0.00") + '\r'
                               ,
                                  "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res1 == DialogResult.Yes)
                            {
                                UpDwnX5.UpDownValue = (Single)dx;
                                UpDwnY5.UpDownValue = (Single)dy;
                                UpDwnZ5.UpDownValue = (Single)dz;
                                UpDwnR5.UpDownValue = (Single)dr;
                                RobotLoadPoints.PickInspect.corrX = Single.Parse(dx.ToString("0.00"));
                                RobotLoadPoints.PickInspect.corrY = Single.Parse(dy.ToString("0.00"));
                                RobotLoadPoints.PickInspect.corrZ = Single.Parse(dz.ToString("0.00"));
                                RobotLoadPoints.PickInspect.corrR = Single.Parse(dr.ToString("0.00"));
                            }
                            break;

                        case 4:

                            DialogResult res3 = MessageBox.Show("Save Place Tray Corrections? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res3 == DialogResult.Yes)
                            {
                                UpDwnX6.UpDownValue = (Single)dx;
                                UpDwnY6.UpDownValue = (Single)dy;
                                UpDwnZ6.UpDownValue = (Single)dz;
                                UpDwnR6.UpDownValue = (Single)dr;
                                RobotLoadPoints.PlaceTrayOrg.corrX = Single.Parse(dx.ToString("0.00"));
                                RobotLoadPoints.PlaceTrayOrg.corrY = Single.Parse(dy.ToString("0.00"));
                                RobotLoadPoints.PlaceTrayOrg.corrZ = Single.Parse(dz.ToString("0.00"));
                                RobotLoadPoints.PlaceTrayOrg.corrR = Single.Parse(dr.ToString("0.00"));
                            }
                            break;
                        case 5:

                            res3 = MessageBox.Show("Save Place Reject Corrections? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res3 == DialogResult.Yes)
                            {
                                UpDwnX7.UpDownValue = (Single)dx;
                                UpDwnY7.UpDownValue = (Single)dy;
                                UpDwnZ7.UpDownValue = (Single)dz;
                                UpDwnR7.UpDownValue = (Single)dr;
                                RobotLoadPoints.PlaceRejectOrg.corrX = Single.Parse(dx.ToString("0.00"));
                                RobotLoadPoints.PlaceRejectOrg.corrY = Single.Parse(dy.ToString("0.00"));
                                RobotLoadPoints.PlaceRejectOrg.corrZ = Single.Parse(dz.ToString("0.00"));
                                RobotLoadPoints.PlaceRejectOrg.corrR = Single.Parse(dr.ToString("0.00"));
                            }
                            break;
                    }
                    //BtnHome_Click(null, null);//move home
                    ControlsEnable(false);
                    var task3 = Task.Run(() => ResetProgram());
                    await task3;
                    reply = task3.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR RESET TASK!");
                        ControlsEnable(true);
                        btnRobotStart.Enabled = true;
                        return;
                    }

                    var task4 = Task.Run(() => RunProgram("TP_MAIN"));
                    await task4;
                    reply = task4.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR RUN ROBOT!");
                        ControlsEnable(true);
                        btnRobotStart.Enabled = true;
                        return;
                    }
                    ControlsEnable(true);
                }

                ControlsEnable(true);



            }
            catch (Exception err)
            {
                SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                MessageBox.Show("ROBOT READ POSITION ERROR:" + err.Message);
            }
        }



        private async void btnTrayCalib_Click(object sender, EventArgs e)
        {

            if (MyStatic.Robot == MyStatic.RobotLoad)
            {

                try
                {
                    txtP0x.Text = "";
                    txtP0y.Text = "";
                    txtP1x.Text = "";
                    txtP1y.Text = "";
                    txtP2x.Text = "";
                    txtP2y.Text = "";
                    txtP3x.Text = "";
                    txtP3y.Text = "";
                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    var task1 = Setup(1, 0);
                    fini = await task1;
                    if (fini.result)
                    {
                        //MessageBox.Show("SAVE POSITIONS? ", "SETUP", MessageBoxButtons.YesNo,
                        //                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        DialogResult res1 = MessageBox.Show("SAVE POSITIONS? ", "SETUP", MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        if (res1 == DialogResult.Yes) //save positions
                        {
                            RobotLoadPoints.P0.x = Single.Parse(txtP0x.Text);
                            RobotLoadPoints.P0.y = Single.Parse(txtP0y.Text);
                            RobotLoadPoints.P1.x = Single.Parse(txtP1x.Text);
                            RobotLoadPoints.P1.y = Single.Parse(txtP1y.Text);
                            RobotLoadPoints.P2.x = Single.Parse(txtP2x.Text);
                            RobotLoadPoints.P2.y = Single.Parse(txtP2y.Text);
                            SavePosIni();
                            int err = 0;
                            var task3 = WritePosition(RobotLoad, RobotLoadPoints.P0.id, RobotLoadPoints.P0);
                            await task3;
                            RobotFunctions.CommReply reply = task3.Result;
                            if (!reply.result) { err++; }
                            task3 = WritePosition(RobotLoad, RobotLoadPoints.P1.id, RobotLoadPoints.P1);
                            await task3;
                            reply = task3.Result;
                            if (!reply.result) { err++; }
                            task3 = WritePosition(RobotLoad, RobotLoadPoints.P2.id, RobotLoadPoints.P2);
                            await task3;
                            reply = task3.Result;
                            if (!reply.result) { err++; }
                            if (err != 0)
                            {
                                MessageBox.Show("ERROR SEND POSITIONS TO ROBOT! ", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                return;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("ERROR TEACH CALIBRATION POINTS! ", "ERROR", MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    var task2 = Task.Run(() => RobotHome());
                    await task2;
                    ControlsEnable(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CALIBRATION ERROR! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }

        }
        private async Task<RobotFunctions.CommReply> Setup(int action, int pallet = 0)
        {
            //-----read tray corrections--------
            //execute in step:send to robot PR[temp] coord
            //and send menu button R[65] R[66]
            //go to TP and run Macro
            //jog robot to position
            //read current pos and compare with send coord
            //RobotLoadPoints.P0.id = 6;
            //RobotLoadPoints.P1.id = 7;
            //RobotLoadPoints.P2.id = 8;
            //RobotLoadPoints.P3.id = 9;
            //RobotLoadPoints.ToolOff.id = 10;
            //RobotLoadPoints.PlacePocketOrg.id=3;
            //RobotLoadPoints.ToolOffTest.id=24;
            //action=1 calib pick tray
            //action=2 calib pocketIn
            //action=3 calib pocketOut
            //action=4 calib place tray
            //action=5 tool offset load
            //action=6 tool offset unload
            //action=7 tool offset test load
            //action=8 tool offset test unload
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {


                int id = 0;//base position
                RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                switch (action)
                {
                    case 1://calib pick tray
                        position[] pos1 = new position[4];
                        for (int i = 0; i < 3; i++)
                        {
                            if (pallet == 0)
                            {
                                if (i == 0) id = RobotLoadPoints.P0.id;
                                else if (i == 1) id = RobotLoadPoints.P1.id;
                                else if (i == 2) id = RobotLoadPoints.P2.id;
                            }
                            else
                            {
                                if (i == 0) id = RobotLoadPoints.P_0.id;
                                else if (i == 1) id = RobotLoadPoints.P_1.id;
                                else if (i == 2) id = RobotLoadPoints.P_2.id;
                            }
                            //else if (i == 3) id = RobotLoadPoints.P3.id;
                            var task2 = Task.Run(() => RunSetup(id, MyStatic.RobotLoad));
                            await task2;
                            fini = task2.Result;
                            //tp
                            if (fini.data != null && fini.data.Length > 3 && fini.data[1] == 1)
                            {
                                MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                            }
                            else
                            {
                                MessageBox.Show("READ POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                                return fini;
                            }
                            //read data
                            var task1 = Task.Run(() => ReadCurPos(RobotLoadPoints.Currpos.id));
                            await task1;
                            RobotFunctions.CommReply reply1 = task1.Result;
                            if (!reply1.result)
                            {
                                MyStatic.bExitcycle = true;
                                MyStatic.bReset = true;
                                RobotLoadAct.InAction = false;
                                SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0

                                MessageBox.Show("READ POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                return fini;
                            }
                            position pos = new position();
                            pos.x = reply1.data[2];
                            pos.y = reply1.data[3];
                            pos.z = reply1.data[4];
                            pos.r = reply1.data[7];
                            pos1[i] = pos;
                            //restart robot
                            var task3 = Task.Run(() => ResetProgram());
                            await task3;
                            reply = task3.Result;
                            if (!reply.result)
                            {
                                MessageBox.Show("ERROR RESET TASK!");
                                ControlsEnable(true);
                                btnRobotStart.Enabled = true;
                                return fini;
                            }

                            var task4 = Task.Run(() => RunProgram("TP_MAIN"));
                            await task4;
                            reply = task4.Result;
                            if (!reply.result)
                            {
                                MessageBox.Show("ERROR RUN ROBOT!");
                                ControlsEnable(true);
                                btnRobotStart.Enabled = true;
                                return fini;
                            }

                        }
                        if (pallet == 0)
                        {
                            inv.settxt(txtP0x, pos1[0].x.ToString("0.000"));
                            inv.settxt(txtP0y, pos1[0].y.ToString("0.000"));
                            inv.settxt(txtP1x,pos1[1].x.ToString("0.000"));
                            inv.settxt(txtP1y,pos1[1].y.ToString("0.000"));
                            inv.settxt(txtP2x, pos1[2].x.ToString("0.000"));
                            inv.settxt(txtP2y, pos1[2].y.ToString("0.000"));
                            //txtP3x.Text = pos1[3].x.ToString("0.000");
                            //txtP3y.Text = pos1[3].y.ToString("0.000");
                        }
                        else
                        {
                            inv.settxt(txtPr0x, pos1[0].x.ToString("0.000"));
                            inv.settxt(txtPr0y, pos1[0].y.ToString("0.000"));
                            inv.settxt(txtPr1x, pos1[1].x.ToString("0.000"));
                            inv.settxt(txtPr1y, pos1[1].y.ToString("0.000"));
                            inv.settxt(txtPr2x, pos1[2].x.ToString("0.000"));
                            inv.settxt(txtPr2y, pos1[2].y.ToString("0.000"));
                        }

                        //save data
                        fini.result = true;
                        return fini;

                    case 5://tool offset load
                        id = RobotLoadPoints.ToolOff.id;
                        fini = await RunSetup(id, MyStatic.RobotLoad);
                        return fini;
                    case 7://tool offset test rob1
                        id = RobotLoadPoints.ToolOffTest.id;
                        fini = await RunSetup(id, MyStatic.RobotLoad);
                        return fini;


                    case 6://tool offset unload
                        id = RobotLoadPoints.ToolOff.id;
                        fini = await RunSetup(id, MyStatic.RobotLoad);
                        return fini;

                }
                //read current pos


            }
            catch (Exception err)
            {
                MessageBox.Show("ROBOT READ POSITION ERROR:" + err.Message);
                reply.result = false;
                return reply;
            }
            reply.result = true;
            return reply;
            //BtnHome_Click(null, null);//move home
            //ControlsEnable(true);
        }


        private async void btnTeachTool_Click(object sender, EventArgs e)
        {
            if (MyStatic.Robot == MyStatic.RobotLoad)
            {
                try
                {
                    txtToolX.Text = "";
                    txtToolY.Text = "";

                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    var task1 = Setup(5);
                    fini = await task1;
                    if (fini.result)
                    {
                        MessageBox.Show("TEACH TOOL OFFSET AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    }
                    else
                    {
                        MessageBox.Show("ERROR TEACH TOOL OFFSET! ", "ERROR", MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    //continue read calib points from PR[90]-[91]
                    position[] pos = new position[2];
                    for (int i = 0; i < 2; i++)
                    {
                        var task2 = Task.Run(() => ReadPosition(MyStatic.RobotLoad, 90 + i));
                        await task2;
                        RobotFunctions.CommReply reply = task2.Result;
                        if (!reply.result)
                        {
                            MessageBox.Show("READ POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return;
                        }

                        pos[i].x = reply.data[2];
                        pos[i].y = reply.data[3];
                        pos[i].z = reply.data[4];
                        pos[i].r = reply.data[7];

                        Single dx = (Single)((pos[1].x - pos[0].x) / 2);
                        Single dy = (Single)((pos[0].y - pos[1].y) / 2);
                        txtToolX.Text = dx.ToString("0.00");
                        txtToolY.Text = dy.ToString("0.00");

                    }

                    ControlsEnable(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CALIBRATION ERROR! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }

        }

        private async void btnTestTool_Click(object sender, EventArgs e)
        {
            if (MyStatic.Robot == MyStatic.RobotLoad)
            {
                try
                {

                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    var task1 = Setup(7);
                    fini = await task1;
                    if (fini.result)
                    {


                    }
                    else
                    {
                        MessageBox.Show("ERROR TOOL TEST! ", "ERROR", MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }


                    ControlsEnable(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("TOOL TEST ERROR! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }

        }
        private void GetGrippers(ref string[] files)
        {
            try
            {
                string[] files1 = new string[1];
                if (MyStatic.Robot == MyStatic.RobotLoad)
                {
                    files1 = Directory.GetFiles(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\LoadGrippers");

                }

                string[] s = new string[files1.Length];
                files = new string[files1.Length];
                for (int i = 0; i < files1.Length; i++)
                {
                    string[] ss = files1[i].Split('\\');
                    int ii = ss[ss.Length - 1].IndexOf(".ini");
                    if (ii > 0)
                    {
                        s[i] = ss[ss.Length - 1].Substring(0, ii);
                        files[i] = s[i];
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }
        private async void BtnLoadGripper_Click(object sender, EventArgs e)
        {

            string[][] arrnew = new string[1][];
            arrnew[0] = new string[0];
            //create vars array
            string mess = "";

            try
            {
                //create vars array
                //robot load
                if (MyStatic.Robot == MyStatic.RobotLoad)

                {
                    string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\LoadGrippers\\" + cmbGripper.Text + ".ini";
                    if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ GRIPPER FILE"); return; }
                    if (!Single.TryParse(IniData.GetKeyValueArrINI("Gripper", "ToolX", arrnew), out Robot1data.ToolOffX))
                    {
                        MessageBox.Show("ERROR TOOL OFFSET X! ", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    if (!Single.TryParse(IniData.GetKeyValueArrINI("Gripper", "ToolY", arrnew), out Robot1data.ToolOffY))
                    {
                        MessageBox.Show("ERROR TOOL OFFSET Y! ", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    if (!Single.TryParse(IniData.GetKeyValueArrINI("Gripper", "ToolZ", arrnew), out Robot1data.ToolOffZ))
                    {

                    }
                    if (!Single.TryParse(IniData.GetKeyValueArrINI("Gripper", "ToolR", arrnew), out Robot1data.ToolOffR))
                    {

                    }
                    txtTooloffX.Text = Robot1data.ToolOffX.ToString();
                    txtTooloffY.Text = Robot1data.ToolOffY.ToString();
                    txtTooloffZ.Text = Robot1data.ToolOffZ.ToString();
                    Robot1data.Gripper = cmbGripper.Text;
                    //send to robot
                    try
                    {

                        MyStatic.bStartcycle = false;
                        ControlsEnable(false);
                        MyStatic.bReset = false;

                        var task = SendTool();
                        await task;
                        RobotFunctions.CommReply reply = task.Result;
                        if (!reply.result)
                        {
                            MyStatic.bExitcycle = true;
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;

                            return;
                        }
                        ControlsEnable(true);

                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("ROBOT SET GRIPPER ERROR:" + err.Message);
                    }
                }


            }

            catch (Exception err)
            { MessageBox.Show("ERROR READ GRIPPER FILE " + err); return; }
        }

        private async void BtnReadRobotIO_Click(object sender, EventArgs e)
        {
            ControlsEnable(false);
            MyStatic.ReadingIO = true;
            //await Task.Run(() => ReadRobotInfo());
            var task = Task.Run(() => ReadRobotInfo());

            ControlsEnable(true);
            for (int i = 0; i < controlsArrayDI.ControlRows * controlsArrayDI.ControlColumns; i++)
            {
                controlsArrayDI.ControlIndex = i;
                UserControlsRefresh(controlsArrayDI, Color.LightGray);
            }
            for (int i = 0; i < controlsArrayDOUT.ControlRows * controlsArrayDOUT.ControlColumns; i++)
            {
                controlsArrayDOUT.ControlIndex = i;
                UserControlsRefresh(controlsArrayDOUT, Color.LightGray);
            }
            for (int i = 0; i < controlsArrayRI.ControlRows * controlsArrayRI.ControlColumns; i++)
            {
                controlsArrayRI.ControlIndex = i;
                UserControlsRefresh(controlsArrayRI, Color.LightGray);
            }
            for (int i = 0; i < controlsArrayRO.ControlRows * controlsArrayRO.ControlColumns; i++)
            {
                controlsArrayRO.ControlIndex = i;
                UserControlsRefresh(controlsArrayRO, Color.LightGray);
            }
            for (int i = 0; i < controlsArrayOPOUT.ControlRows * controlsArrayOPOUT.ControlColumns; i++)
            {
                controlsArrayOPOUT.ControlIndex = i;
                UserControlsRefresh(controlsArrayOPOUT, Color.LightGray);
            }
            for (int i = 0; i < controlsArrayOPIN.ControlRows * controlsArrayOPIN.ControlColumns; i++)
            {
                controlsArrayOPIN.ControlIndex = i;
                UserControlsRefresh(controlsArrayOPIN, Color.LightGray);
            }

        }
        private async Task<bool> ReadRobotInfo()
        {
            int din1_16 = 0;
            int din17_24 = 0;
            int state = 0;

            int dout1_16 = 0;
            int dout17_24 = 0;
            int ri1_8 = 0;
            int ro1_8 = 0;
            int OPOUT0_7 = 0;
            int OPIN0_7 = 0;

            int robot = 0;
            int dout = -1;
            int ro = -1;
            int rostate = 0;
            MyStatic.ReadingIO = true;
            while (MyStatic.ReadingIO)
            {
                if (MyStatic.bReset) return false;
                //Thread.Sleep(200);
                await Task.Delay(200);
                if (MyStatic.bReset) return false;
                //Application.DoEvents();
                try
                {

                    MyStatic.bStartcycle = false;



                    {

                        //setoutputs


                        SetDOutput(dout, state, ro, rostate);//reaD AND WRITE


                        if (dout > 0 || ro > 0)
                        {
                            dout = -1;
                            ro = -1;
                            MyStatic.SetOut = -1;
                            MyStatic.SetHandOut = -1;
                        }
                        FW1.ReadFanucIOAsync("dout", "0", "get", "0", 500);

                        //Thread.Sleep(50);
                        await Task.Delay(50);
                        RobotFunctions.CommReply reply = FW1.ReadFanucS2RegAsync(500);

                        if (reply.result && reply.data.Length > 15 && reply.data[0] == 151 && reply.data[1] == 1) reply.result = true;

                        din1_16 = (int)reply.data[2];
                        din17_24 = (int)reply.data[3];
                        dout1_16 = (int)reply.data[4];
                        dout17_24 = (int)reply.data[5];
                        ri1_8 = (int)reply.data[6];
                        ro1_8 = (int)reply.data[7];
                        OPOUT0_7 = (int)reply.data[8];
                        OPIN0_7 = (int)reply.data[9];
                        int[] val = { din1_16, din17_24 };
                        RefreshIO1(val, controlsArrayDI, Color.LightGreen, Color.Yellow);
                        int[] val1 = { dout1_16, dout17_24 };
                        RefreshIO1(val1, controlsArrayDOUT, Color.Red, Color.LightBlue);
                        int[] val2 = { ri1_8 };
                        RefreshIO1(val2, controlsArrayRI, Color.LightGreen, Color.Yellow);
                        int[] val3 = { ro1_8 };
                        RefreshIO1(val3, controlsArrayRO, Color.Red, Color.LightBlue);
                        int[] val4 = { OPOUT0_7 };
                        RefreshIO1(val4, controlsArrayOPOUT, Color.Red, Color.LightBlue);
                        int[] val5 = { OPIN0_7 };
                        RefreshIO1(val5, controlsArrayOPIN, Color.LightGreen, Color.Yellow);


                        //RefreshIO(dout1_16, dout17_24, controlsArrayDOUT, Color.Red);
                        //RefreshIO(ri1_8, 0, controlsArrayRI, Color.LightGreen);
                        //RefreshIO(ro1_8, 0, controlsArrayRO, Color.Red);


                        int bit = (Int32)Math.Pow(2, MyStatic.SetOut);
                        //int rostate = 0;
                        if (MyStatic.SetOut >= 0 && MyStatic.SetOut < 16)
                        {
                            if ((dout1_16 & bit) == bit)
                            {
                                state = 0;
                                dout = MyStatic.SetOut + 1;
                            }
                            else
                            {
                                state = 1;
                                dout = MyStatic.SetOut + 1;
                            }


                        }
                        else if (MyStatic.SetOut >= 16 && MyStatic.SetOut < 20)
                        {
                            // if ((dout9_16 & MyStatic.SetOut) == MyStatic.SetOut)
                            bit = (Int32)Math.Pow(2, MyStatic.SetOut - 16);
                            if ((dout17_24 & bit) == bit)
                            {
                                state = 0;
                                dout = MyStatic.SetOut + 1;
                            }
                            else
                            {
                                state = 1;
                                dout = MyStatic.SetOut + 1;
                            }
                        }

                        if (MyStatic.SetHandOut >= 0 && MyStatic.SetHandOut < 8)
                        {
                            bit = (Int32)Math.Pow(2, MyStatic.SetHandOut);
                            if ((ro1_8 & bit) == bit)
                            {
                                rostate = 0;
                                ro = MyStatic.SetHandOut + 1;
                            }
                            else
                            {
                                rostate = 1;
                                ro = MyStatic.SetHandOut + 1;
                            }


                        }



                    }

                    //return true;
                }
                //end cycle
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR READ IO " + ex.Message);
                    return false;
                }

            }
            return false;
        }
        public void RefreshIO(int values1, int values2, UserControl.ControlsArrayControl user, Color color)
        {
            try
            {

                int ii;


                for (int i = 0; i < 16; i++)
                {
                    if (user.ControlRows * user.ControlColumns < i) break;
                    ii = (Int32)Math.Pow(2, i);
                    if ((Convert.ToInt32(values1) & ii) == ii)

                    //lbl[i + 16 * j].BackColor = Color.LightGreen;
                    {
                        user.ControlIndex = i;
                        if (user.ControlBackColor != Color.LightGreen) UserControlsRefresh(user, color);// user.ControlBackColor = Color.LightGreen;


                    }
                    else
                    {
                        //lbl[i + 16 * j].BackColor = Color.Yellow;

                        user.ControlIndex = i;
                        if (user.ControlBackColor != Color.Yellow) UserControlsRefresh(user, Color.Yellow); ;// user.ControlBackColor = Color.Yellow;

                    }
                }
                for (int i = 16; i < 32; i++)
                {
                    if (user.ControlRows * user.ControlColumns < i) break;
                    ii = (Int32)Math.Pow(2, i - 16);
                    if ((Convert.ToInt32(values2) & ii) == ii)
                    {
                        //lbl[i + 16 * j].BackColor = Color.LightGreen;
                        user.ControlIndex = i;
                        if (user.ControlBackColor != Color.LightGreen) UserControlsRefresh(user, color); ;// user.ControlBackColor = Color.LightGreen;
                    }



                    else
                    {
                        //lbl[i + 16 * j].BackColor = Color.Yellow;

                        user.ControlIndex = i;
                        if (user.ControlBackColor != Color.Yellow) UserControlsRefresh(user, Color.Yellow); ;// user.ControlBackColor = Color.Yellow;

                    }
                }

                //user.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR REFRESH IO " + ex.Message);
            }

        }
        public void RefreshIO1(int[] values, UserControl.ControlsArrayControl user, Color color, Color colorout)
        {
            try
            {

                int ii;

                for (int j = 0; j < values.Length; j++)
                {
                    for (int i = 16 * j; i < 16 + 16 * j; i++)
                    {
                        if (user.ControlRows * user.ControlColumns <= i) break;
                        ii = (Int32)Math.Pow(2, i - 16 * j);
                        if ((Convert.ToInt32(values[j]) & ii) == ii)


                        {
                            user.ControlIndex = i;
                            if (user.ControlBackColor != Color.LightGreen) UserControlsRefresh(user, color);// user.ControlBackColor = Color.LightGreen;


                        }
                        else
                        {
                            user.ControlIndex = i;
                            if (user.ControlBackColor != Color.Yellow) UserControlsRefresh(user, colorout); ;// user.ControlBackColor = Color.Yellow;

                        }
                    }

                }

                //user.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR REFRESH IO " + ex.Message);
            }

        }
        private async Task<RobotFunctions.CommReply> SetDOutput(int output, int state, int ro, int rostate)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {

                int robot = 0;
                if (output > 0 || ro > 0)
                {
                    var task1 = SetOutput(MyStatic.Robot, output, state, ro, rostate);
                    await task1;
                    reply = task1.Result;
                    if (!reply.result)
                    {

                        return reply;
                    }
                }
                else
                {
                    reply.result = true;
                    return reply;
                }



            }
            catch (Exception err)
            {
                MessageBox.Show("ROBOT SET OUTPUT ERROR:" + err.Message);
                reply.result = false;
                return reply;
            }
            return reply;
        }
        private void Button10_Click(object sender, EventArgs e)
        {
            MyStatic.ReadingIO = false;
        }
        private void controlsArrayDOUT_Control_Click(int index)
        {
            MyStatic.SetOut = index;

        }
        private void controlsArrayRO_Control_Click(int index)
        {
            MyStatic.SetHandOut = index;

        }

        //bool EndOfTray = false;
        //bool FlagNicolisEmpting = false;
        //bool WaitBit = false;
        //bool bResetConv = false;
        //string ErrorLift = "";



        private void BtnStop_Click(object sender, EventArgs e)
        {
            MyStatic.ReadingIO = false;
            MyStatic.WaitReady = false;
            Thread.Sleep(100);

        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyStatic.ReadingIO = false;
            Thread.Sleep(100);

        }











        private void ControlsArrayPLC2OUT_Control_Click(int index)
        {
            MyStatic.SetPlc2Out = index;
        }












        private async void BtnReadPos_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            ControlsEnable(false);
            try
            {
                var task = Task.Run(() => ReadPos());
                await task;
                reply = task.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR READ ROBOT POSITION!");
                    ControlsEnable(true);

                    return;
                }
                ControlsEnable(true);

            }
            catch (Exception err)
            {
                MessageBox.Show("ROBOT READ POSITION ERROR:" + err.Message);
            }
            ControlsEnable(true);

        }
        private async Task<RobotFunctions.CommReply> ReadPos()
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            reply.result = false;
            try
            {
                //RobotLoadPoints.Home.id = 1;
                //RobotLoadPoints.PickTrayOrg.id = 2;
                //RobotLoadPoints.PlaceTrayOrg.id = 3;
                //RobotLoadPoints.PlaceInspect.id = 4;
                //RobotLoadPoints.PickInspect.id = 5;
                //RobotLoadPoints.PlaceReject.id = 6;
                //RobotLoadPoints.P0.id = 7;
                //RobotLoadPoints.P1.id = 8;
                //RobotLoadPoints.P2.id = 9;
                //RobotLoadPoints.P3.id = 10;
                //RobotLoadPoints.P_0.id = 14;
                //RobotLoadPoints.P_1.id = 15;
                //RobotLoadPoints.P_2.id = 16;
                //RobotLoadPoints.P_3.id = 17;
                //RobotLoadPoints.ToolOff.id = 10;
                //RobotLoadPoints.Currpos.id = 19;
                //RobotLoadPoints.TempPos.id = 23;
                //RobotLoadPoints.ToolOffTest.id = 24;

                position[] p = new position[25];
                string s = "";
                int err = 0;
                for (int i = 1; i < 18; i++)
                {
                    //MyStatic.bStartcycle = false;
                    //ControlsEnable(false);
                    MyStatic.bReset = false;
                    int robot = 0;
                    int id = 0;
                    //if (MyStatic.Robot == MyStatic.RobotLoad)
                    //{
                    robot = MyStatic.RobotLoad;
                    id = i;
                    //}

                    var task1 = Task.Run(() => ReadPosition(robot, id));
                    await task1;
                    reply = task1.Result;
                    if (!reply.result)
                    {
                        MyStatic.bExitcycle = true;
                        //MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;

                        return reply;
                    }
                    p[i].x = reply.data[2];
                    p[i].y = reply.data[3];
                    p[i].z = reply.data[4];
                    p[i].r = reply.data[7];
                    string s1 = "\t" + "x=" + p[i].x.ToString("0.0") + "\t" + "y=" + p[i].y.ToString("0.0") + "\t" +
                         "z=" + p[i].z.ToString("0.0") + "\t" + "r=" + p[i].r.ToString("0.0");
                    string s2 = "";

                    if (robot == MyStatic.RobotLoad)
                    {
                        switch (i)
                        {
                            case 1:
                                s2 = "\t" + "x=" + RobotLoadPoints.Home.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.Home.y.ToString("0.0") + "\t" +
                                "z=" + RobotLoadPoints.Home.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.Home.r.ToString("0.0");
                                s = s + "Home ROBOT:    " + "\t" + s1 + '\r';
                                s = s + "Home INI  :" + "\t" + s2 + '\r';
                                position pp = RobotLoadPoints.Home;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions HOME not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 2:

                                s2 = "\t" + "x=" + RobotLoadPoints.PickTrayOrg.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.PickTrayOrg.y.ToString("0.0") + "\t" +
                                "z=" + RobotLoadPoints.PickTrayOrg.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.PickTrayOrg.r.ToString("0.0");
                                s = s + "PickTrayOrg ROBOT:    " + s1 + '\r';
                                s = s + "PickTrayOrg INI  :" + "\t" + s2 + '\r';
                                pp = RobotLoadPoints.PickTrayOrg;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions PickTrayOrg not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 3:

                                s2 = "\t" + "x=" + RobotLoadPoints.PlaceTrayOrg.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.PlaceTrayOrg.y.ToString("0.0") + "\t" +
                                "z=" + RobotLoadPoints.PlaceTrayOrg.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.PlaceTrayOrg.r.ToString("0.0");
                                s = s + "PlaceTrayOrg ROBOT:    " + s1 + '\r';
                                s = s + "PlaceTrayOrg INI  :" + "\t" + s2 + '\r';
                                pp = RobotLoadPoints.PlaceTrayOrg;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions PlaceTrayOrg not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 4:

                                s2 = "\t" + "x=" + RobotLoadPoints.PlaceInspect.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.PlaceInspect.y.ToString("0.0") + "\t" +
                                "z=" + RobotLoadPoints.PlaceInspect.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.PlaceInspect.r.ToString("0.0");
                                s = s + "PlaceInspect ROBOT:    " + s1 + '\r';
                                s = s + "PlaceInspect INI  :" + "\t" + s2 + '\r';
                                pp = RobotLoadPoints.PlaceInspect;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions PlaceInspect not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 5:

                                s2 = "\t" + "x=" + RobotLoadPoints.PickInspect.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.PickInspect.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.PickInspect.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.PickInspect.r.ToString("0.0");
                                s = s + "PickInspect ROBOT:    " + s1 + '\r';
                                s = s + "PickInspect INI  :" + "\t" + s2 + '\r';
                                pp = RobotLoadPoints.PickInspect;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions PickInspect not the same!!! *************" + '\r'; err++;
                                }
                                break;

                            case 6:

                                s2 = "\t" + "x=" + RobotLoadPoints.PlaceRejectOrg.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.PlaceRejectOrg.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.PlaceRejectOrg.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.PlaceRejectOrg.r.ToString("0.0");
                                s = s + "PlaceRejectOrg ROBOT:    " + s1 + '\r';
                                s = s + "PlaceRejectOrg INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.PlaceRejectOrg;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions PlaceRejectOrg not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 7:

                                s2 = "\t" + "x=" + RobotLoadPoints.P0.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P0.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P0.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P0.r.ToString("0.0");
                                s = s + "P0 ROBOT:    " + s1 + '\r';
                                s = s + "P0 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P0;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P0 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 8:

                                s2 = "\t" + "x=" + RobotLoadPoints.P1.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P1.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P1.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P1.r.ToString("0.0");
                                s = s + "P1 ROBOT:    " + s1 + '\r';
                                s = s + "P1 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P1;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P1 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 9:

                                s2 = "\t" + "x=" + RobotLoadPoints.P2.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P2.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P2.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P2.r.ToString("0.0");
                                s = s + "P2 ROBOT:    " + s1 + '\r';
                                s = s + "P2 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P2;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P2 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 10:

                                s2 = "\t" + "x=" + RobotLoadPoints.P3.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P3.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P3.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P3.r.ToString("0.0");
                                s = s + "P3 ROBOT:    " + s1 + '\r';
                                s = s + "P3 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P3;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P3 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 11:
                                s2 = "\t" + "x=" + RobotLoadPoints.Maint.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.Maint.y.ToString("0.0") + "\t" +
                                "z=" + RobotLoadPoints.Maint.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.Maint.r.ToString("0.0");
                                s = s + "Maint ROBOT:    " + "\t" + s1 + '\r';
                                s = s + "Maint INI  :" + "\t" + s2 + '\r';
                                pp = RobotLoadPoints.Maint;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions Mantenance not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 14:

                                s2 = "\t" + "x=" + RobotLoadPoints.P_0.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P_0.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P_0.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P_0.r.ToString("0.0");
                                s = s + "P_0 ROBOT:    " + s1 + '\r';
                                s = s + "P_0 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P_0;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P_0 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 15:

                                s2 = "\t" + "x=" + RobotLoadPoints.P_1.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P_1.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P_1.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P_1.r.ToString("0.0");
                                s = s + "P_1 ROBOT:    " + s1 + '\r';
                                s = s + "P_1 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P_1;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P_1 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 16:

                                s2 = "\t" + "x=" + RobotLoadPoints.P_2.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P_2.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P_2.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P_2.r.ToString("0.0");
                                s = s + "P_2 ROBOT:    " + s1 + '\r';
                                s = s + "P_2 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P_2;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P_2 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            case 17:

                                s2 = "\t" + "x=" + RobotLoadPoints.P_3.x.ToString("0.0") + "\t" + "y=" + RobotLoadPoints.P_3.y.ToString("0.0") + "\t" +
                               "z=" + RobotLoadPoints.P_3.z.ToString("0.0") + "\t" + "r=" + RobotLoadPoints.P_3.r.ToString("0.0");
                                s = s + "P_3 ROBOT:    " + s1 + '\r';
                                s = s + "P_3 INI  :" + s2 + '\r';
                                pp = RobotLoadPoints.P_3;
                                if (Math.Abs(pp.x - p[i].x) > 0.3 || Math.Abs(pp.y - p[i].y) > 0.3 || Math.Abs(pp.z - p[i].z) > 0.3 || Math.Abs(pp.r - p[i].r) > 0.3)
                                {
                                    s = s + "********* Positions P_3 not the same!!! *************" + '\r'; err++;
                                }
                                break;
                            default: break;
                        }
                    }


                }
                if (err > 0)
                {
                    DialogResult res = MessageBox.Show("Save Robot Positions?" + '\r' + '\r' + s, "SAVE", MessageBoxButtons.OKCancel,
                                                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //ControlsEnable(true);
                    if (res == DialogResult.OK)
                    {
                        //save pos
                        RobotLoadPoints.Home = p[1];
                        RobotLoadPoints.PickTrayOrg = p[2];
                        RobotLoadPoints.PlaceTrayOrg = p[3];
                        RobotLoadPoints.PlaceInspect = p[4];
                        RobotLoadPoints.PickInspect = p[5];
                        RobotLoadPoints.PlaceRejectOrg = p[6];
                        RobotLoadPoints.P0 = p[7];
                        RobotLoadPoints.P1 = p[8];
                        RobotLoadPoints.P2 = p[9];
                        RobotLoadPoints.P3 = p[10];
                        RobotLoadPoints.Maint = p[11];
                        RobotLoadPoints.P_0 = p[14];
                        RobotLoadPoints.P_1 = p[15];
                        RobotLoadPoints.P_2 = p[16];
                        RobotLoadPoints.P_3 = p[17];

                        SavePosIni();
                    }
                }
                reply.result = true;
                return reply;
            }
            catch (Exception err)
            {
                return reply;
            }
            //ControlsEnable(true);

        }
        bool FirstStart = false;
        private async Task<bool> RobotStart()
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                MyStatic.bReset = false;
                Thread.Sleep(100);

                reply.comment = "";
                var task3 = Task.Run(() => ReadIO_asinc());
                //check TP enable false and Auto true
                bool ok = false;
                Stopwatch st = new Stopwatch();
                st.Restart();
                while (!ok)
                {
                    FW1.ReadFanucIOAsync("dout", "0", "get", "0", 500);

                    Thread.Sleep(50);
                    reply = FW1.ReadFanucS2RegAsync(500);

                    if (reply.result && reply.data.Length > 15 && reply.data[0] == 15 && reply.data[1] == 1) { reply.result = true; ok = true; break; }
                    else
                    {
                        if (st.ElapsedMilliseconds > 5000)
                        {

                            MessageBox.Show("ERROR READ ROBOT IO!  ", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                            return false;
                        }
                    }
                    Thread.Sleep(200);
                }


                int OPOUT0_7 = (int)reply.data[8];
                int OPIN0_7 = (int)reply.data[9];


                BitArray b = new BitArray(new int[] { OPOUT0_7 });
                if (!b[0] && !b[7])
                {

                    MessageBox.Show("ERROR RUN ROBOT!!! TURN ROBOT CONTROLLER KEY TO <AUTO> MODE !  ", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return false;
                }
                if (b[7])
                {

                    MessageBox.Show("ERROR RUN ROBOT!!! TURN TEACH PENDANT SWITCH TO <OFF> POSITION !  ", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return false;
                }

                //
                var task1 = Task.Run(() => RunProgram("TP_MAIN"));
                await task1;
                reply = task1.Result;
                if (!reply.result)
                {

                    MessageBox.Show("ERROR READ INFO TASK!  " + reply.comment, "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return false;
                }


                
                var task = Task.Run(() => SendData());
                await task;
                reply = task.Result;
                if (!reply.result)
                {
                    Thread.Sleep(500);
                    var task2 = Task.Run(() => SendData());
                    await task2;
                    reply = task2.Result;
                    if (!reply.result)
                    {

                        MessageBox.Show("ERROR SEND DATA TO ROBOT!", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        return false;
                    }


                }
                Thread.Sleep(100);
                if (!FirstStart)
                {
                    var task2 = Task.Run(() => ReadPos());
                    await task2;
                    reply = task2.Result;
                    if (!reply.result)
                    {

                        MessageBox.Show("ERROR READ ROBOT POSITION!", "ERROR", MessageBoxButtons.OK,
                           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        return false;
                    }
                    FirstStart = true;
                }

                return true;


            }
            catch (Exception ex) { inv.set(btnRobotStart, "BackColor", Color.LightGray); SetTraficLights(0, 0, 1, 0); return false; }//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
        }


        private async void btnCheckComm_Click(object sender, EventArgs e)
        {
            try
            {
                var task = Task.Run(() => CheckComm());
                await task;
            }
            catch (Exception ex) { }
        }

        private async void btnReadReg_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                var task1 = Task.Run(() => FW1.ReadFanucS2RegAsync3(2000));
                await task1;
                reply = task1.Result;
            }
            catch (Exception ex) { }
        }

        private void txtRobot_DoubleClick(object sender, EventArgs e)
        {
            txtRobot.Text = "";
        }

        private async void btnNextStep_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                var task2 = Task.Run(() => FW1.WriteFanucRegIntAsync("numreg", "22", "1", 1000));
                await task2;
                return;
            }
            catch (Exception ex) { }
        }

        private async void btnReadCurPos_Click(object sender, EventArgs e)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                ControlsEnable(false);
                for (int i = 1; i < 87; i++)
                {
                    Thread.Sleep(1);
                    var task1 = Task.Run(() => InfoProgram(i.ToString().Trim()));
                    await task1;
                    reply = task1.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR READ INFO TASK!");
                        ControlsEnable(true);
                        btnRobotStart.Enabled = true;
                        return;
                    }
                    if (reply.data != null && reply.data.Length > 15 && reply.data[0] == (int)MyStatic.RobotCmd.Info && reply.data[1] == 1 && reply.data[2] == 0)
                    {
                        Single tp_main = reply.data[4];
                        Single status = reply.data[3];
                        Single task_no = reply.data[5];
                        if (tp_main == 1 && status == 0)
                        {
                            MessageBox.Show("Task TP_MAIN Running. Task No = " + task_no.ToString());
                            ControlsEnable(true);
                            return;
                        }
                        if (tp_main == 1 && status != 0)
                        {
                            MessageBox.Show("Task TP_MAIN Stopped. Task No = " + task_no.ToString());
                            ControlsEnable(true);
                            return;
                        }
                    }
                    ;
                }
                MessageBox.Show("Error read TP_MAIN status");
                ControlsEnable(true);
                return;

            }
            catch (Exception ex) { ControlsEnable(true); }
        }
        private RobotFunctions.CommReply CheckMainRun()
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {

                for (int i = 1; i < 87; i++)
                {
                    Thread.Sleep(1);
                    reply = InfoProgram(i.ToString().Trim());

                    if (!reply.result)
                    {
                        reply.result = false;
                        return reply;
                    }
                    if (reply.data != null && reply.data.Length > 15 && reply.data[0] == (int)MyStatic.RobotCmd.Info && reply.data[1] == 1 && reply.data[2] == 0)
                    {
                        Single tp_main = reply.data[4];
                        Single status = reply.data[3];
                        Single task_no = reply.data[5];
                        if (tp_main == 1 && status == 0)
                        {
                            //Thread.Sleep(2000);
                            //var task2 = Task.Run(() => InfoProgram(i.ToString().Trim()));
                            //await task2;
                            //reply = task2.Result;
                            //tp_main = reply.data[4];
                            //status = reply.data[3];
                            //task_no = reply.data[5];
                            if (tp_main == 1 && status == 0)
                            {
                                reply.result = true;
                                return reply;
                            }
                            else
                            {
                                reply.result = false;
                                return reply;
                            }
                        }
                        if (tp_main == 1 && status != 0)
                        {
                            //MessageBox.Show("Task TP_MAIN Stopped. Task No = " + task_no.ToString());
                            reply.result = false;
                            return reply;
                        }
                    }
                    ;
                }
                MessageBox.Show("Error read TP_MAIN status");
                reply.result = false;
                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                return reply;
            }
        }
        private RobotFunctions.CommReply RunProgram(string name)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                //set main program
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Run PNS0101" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                FW1.ReadFanucIOAsync("prog", "PNS0101", "MAIN", "0", 1000);

                reply = FW1.ReadFanucS2RegAsync(1000);

                if (reply.result && reply.data != null && reply.data.Length > 15 && reply.data[0] == (int)MyStatic.RobotCmd.MainProg && reply.data[1] == 1) { reply.result = true; } else { reply.result = false; }
                if (!reply.result)
                {
                    frmMain.newFrmMain.ListAdd3("Robot=>ERROR SET PNS0101" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false);
                    return reply;
                }
                //abort
                reply = AbortProgram("*ALL*");

                //if (!reply.result) return reply;
                Thread.Sleep(500);
                //run
                FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);

                FW1.ReadFanucIOAsync("prog", "PNS0101", "run", "0", 1000);

                reply = FW1.ReadFanucS2RegAsync(1000);

                if (reply.result && reply.data != null && reply.data.Length > 15 && reply.data[0] == (int)MyStatic.RobotCmd.RunProgram && reply.data[1] == 1 && reply.data[2] == 0) { reply.result = true; } else { reply.result = false; }
                //var task2 = FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);
                //await task2;
                //reply.result = false;
                if (!reply.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>ERROR RUN PNS0101" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    return reply;
                }
                //task info
                Thread.Sleep(500);
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                while (!MyStatic.bReset)
                {
                    reply = CheckMainRun();

                    //if (!reply.result)
                    //{
                    //    frmMain.newFrmMain.ListAdd3("Robot=>ERROR!  TP_MAIN not running" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.lstRobotsLog, false);
                    //    return reply;
                    //}
                    if (sw.ElapsedMilliseconds > 5000)
                    {
                        break;
                    }
                    if (reply.result)
                        break;
                    Thread.Sleep(200);

                }

                if (!reply.result || reply.data == null || reply.data[3] != 0)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot=>ERROR!  TP_MAIN not running " + reply.comment + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    return reply;
                }
                FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);

                reply.result = true;
                return reply;
            }
            catch (Exception ex) { return reply; }
        }
        private RobotFunctions.CommReply AbortProgram(string name)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                reply = FW1.ReadFanucIOAsync("prog", "ALL", "ABORT", "0", 500);

                if (!reply.result && reply.comment != "") return reply;
                //var task1 = Task.Run(() => FW1.ReadFanucS2RegAsync(1000));
                //await task1;
                //reply = task1.Result;
                //if (reply.result && reply.data != null && reply.data.Length > 15 && reply.data[0] == (int)MyStatic.RobotCmd.AbortProgram && reply.data[1] == 1 && reply.data[2] == 0) reply.result = true; else reply.result = false;
                reply = FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);

                reply.result = true;
                return reply;
            }
            catch (Exception ex) { return reply; }
        }
        private async Task<RobotFunctions.CommReply> ResetProgram()
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                reply = FW1.ReadFanucIOAsync("prog", "ALL", "RESET", "0", 500);

                //if (reply.result && reply.data != null && reply.data.Length > 15 && reply.data[0] == (int)MyStatic.RobotCmd.AbortProgram && reply.data[1] == 1 && reply.data[2] == 0) reply.result = true; else reply.result = false;
                if (!reply.result && reply.comment != "")
                {
                    MessageBox.Show("ERROR RESET ROBOT! " + reply.comment, "ERROR", MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return reply;
                }
                reply = FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);

                return reply;
            }
            catch (Exception ex) { return reply; }
        }

        private RobotFunctions.CommReply InfoProgram(string name)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                //name = "TP_MAIN";
                FW1.ReadFanucIOAsync("prog", name, "INFO", "0", 500);

                reply = FW1.ReadFanucS2RegAsync(1000);

                if (reply.result && reply.data != null && reply.data.Length > 15 && reply.data[0] == 95 && reply.data[1] == 1 && reply.data[2] == 0) reply.result = true; else reply.result = false;
                //var task2 = FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);
                //await task2;
                // TSK_STATUS is the task status: The return values are:
                //PG_RUNACCEPT: Run request has been accepted 10
                //PG_ABORTING : Abort has been accepted 11
                //PG_RUNNING: Task is running 12
                //PG_PAUSED : Task is paused 13
                //PG_ABORTED : Task is aborted 14
                if (reply.data != null && reply.data[6] == 10) reply.comment = "PG_RUNACCEPT";
                else if (reply.data != null && reply.data[6] == 11) reply.comment = "PG_ABORTING";
                else if (reply.data != null && reply.data[6] == 12) reply.comment = "PG_RUNNING";
                else if (reply.data != null && reply.data[6] == 13) reply.comment = "PG_PAUSED";
                else if (reply.data != null && reply.data[6] == 14) reply.comment = "PG_ABORTED";
                else reply.comment = "";
                return reply;
            }
            catch (Exception ex) { return reply; }
        }
        private RobotFunctions.CommReply ReadCurPos(int index)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                FW1.ReadFanucPosRegAsync("posreg", index.ToString(), 500);

                reply = FW1.ReadFanucS2RegAsync(500);


                if (reply.result && reply.data.Length > 15 && reply.data[0] == 71 && reply.data[1] == 1) reply.result = true;
                FW1.WriteFanucRegAsync("numreg", "100", "0", 1000);

                //reply.result = true;
                return reply;
            }
            catch (Exception ex) { return reply; }
        }

        #region pallet //----------------------------------------------------------pallet--------------------------------------------------
        int TrayPartId = 0;
        int TrayPartIdRej = 0;
        private async void btnRefreshTray_Click(object sender, EventArgs e)
        {
            try
            {

                TrayPartId = int.Parse(txtPartID.Text);
                int partid = TrayPartId;
                var task02 = Task.Run(() => GetPickTray(partid));
                await task02;
                RobotFunctions.CommReply posrep2 = task02.Result;

                if (!posrep2.result)
                {
                    MyStatic.bExitcycle = true;
                    MessageBox.Show("ERROR3 Pick TRAY Pos COORDINATES!", "ERROR", MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                else
                {
                    Single x = posrep2.data[0];
                    Single y = posrep2.data[1];
                    Single z = posrep2.data[2];
                    Single r = posrep2.data[3];
                    //label225.Text = x.ToString() + "\r\n" +
                    //    y.ToString() + "\r\n" +
                    //    z.ToString() + "\r\n" +
                    //    r.ToString() + "\r\n";

                    txtPartID.Text = TrayPartId.ToString();

                }
            }
            catch (Exception ex)
            {
                Task.Run(() => frmMain.newFrmMain.ListAdd3("ERROR REFRESH PICK TRAY" + ex.Message + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
            };
        }
        public struct Cell
        {
            public int mark;
            public Double x;
            public Double y;
            public Double z;
            public Double r;
            public Double delay;
            public Double corrX;
            public Double corrY;
            public Double corrZ;
            public Double corrR;
            public int id;

        }
        public struct TrayPart
        {
            public Single x;
            public Single y;
            public Single z;
            public Single width;
            public Single height;
            public int Index;
            public int Row;
            public int Col;
            public int Rotate;
            public Boolean NoPlace;
            public Boolean Exist;
            public String Order;

        }
        public Cell[] cell = new Cell[1];
        public Cell[] TrayOutCell = new Cell[1];
        public Cell[] TrayOutCell1 = new Cell[1];
        public Cell[] TrayOutCell2 = new Cell[1];
        public int TrayInsertsOnY = 3;
        public int TrayInsertsOnX = 5;
        public Single TrayDeltaY = 30;
        public Single TrayDeltaYreal = 30;
        public Single TrayDeltaX = 30;
        public Single TrayOffsetX = 0;
        public Single TrayOffsetY = 0;
        public Single TrayOffsetZ = 0;
        public Single diamOffsetZ = 0;
        Single dxY = 0;
        Single dyX = 0;
        Single dzX = 0;
        Single dzY = 0;
        Single Xscale = 0.000f;
        Single Yscale = 0.000f;
        Single ZdiamoffsetPickTray = 0;

        public Single TrayEdgeX = 20;
        public Single TrayEdgeY = 30;
        public Single TrayInsertL = 0;
        public Single TrayInsertW = 0;
        public Single TrayHeight = 0;
        public int LastPlace = 0;
        public Single scale = 1;
        public position[] cellpos = new position[1];
        public TrayPart[] part;
        public TrayPart[] partRej;


        private RobotFunctions.CommReply GetPickTray(int partid, bool corr = false)
        {

            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();
            Array.Resize<Single>(ref commreply.data, 4);
            //TrayPartId = int.Parse(txtPartID.Text);
            // OrderPartId = int.Parse(txtOrderPartId.Text);
            Single x = 0;
            Single y = 0;

            //if (MyStatic.chkDebug)
            //{
            //    Array.Resize<Cell>(ref TrayOutCell1, TrayInsertsOnY * TrayInsertsOnX);

            //}

            position outpos = new position();
            outpos.Error = "";

            if (corr)
            {
                for (int i = 0; i < TrayOutCell1.Length; i++)

                {
                    TrayOutCell1[i].mark = 1;
                    TrayOutCell1[i].corrX = 0;
                    TrayOutCell1[i].corrY = 0;
                    TrayOutCell1[i].corrZ = 0;
                    TrayOutCell1[i].corrR = 0;


                }
            }



            if (partid >= TrayOutCell1.Length - 1)
            {
                commreply.comment = "last part";
            }
            if (partid >= TrayOutCell1.Length) partid = TrayOutCell1.Length - 1;
            int partindex = partid;
            Panel panel = panelTrayOut;
            DrawTrayOut(panel, ref partindex, ref outpos, 0, 0);
            int cntPartsLeft = 0;
            if (partindex + 1 < TrayOutCell1.Length)//how match parts left to pick
            {
                for (int i = partindex + 1; i < TrayOutCell1.Length; i++)
                {
                    if (TrayOutCell1[i].mark != null && TrayOutCell1[i].mark == 1) cntPartsLeft++;

                }
                if (cntPartsLeft == 0)
                    commreply.comment = "last part";
            }
            x = 0; y = 0;
            commreply.result = true;

            Single dz = ZdiamoffsetPickTray;

            commreply.data[0] = (Single)(RobotLoadPoints.PickTrayOrg.x + outpos.x) + TrayOffsetX;
            commreply.data[1] = (Single)(RobotLoadPoints.PickTrayOrg.y + outpos.y) + TrayOffsetY;
            commreply.data[2] = (Single)(RobotLoadPoints.PickTrayOrg.z + dz) + TrayOffsetZ;
            commreply.data[3] = (Single)(RobotLoadPoints.PickTrayOrg.r);




            inv.settxt(lblTrayX, commreply.data[0].ToString("0.0"));
            inv.settxt(lblTrayY, commreply.data[1].ToString("0.0"));
            inv.settxt(lblTrayZ, commreply.data[2].ToString("0.0"));
            inv.settxt(lblTrayR, commreply.data[3].ToString("0.0"));

            //inv.settxt(lblTraydX, (dxY * outpos.y + Xscale * outpos.x).ToString("0.0"));
            //inv.settxt(lblTraydY, (dyX * outpos.x + Yscale * outpos.y).ToString("0.0"));
            //inv.settxt(lblTraydZ, (dzX * outpos.x + dzY * outpos.y).ToString("0.0"));

            commreply.data[0] = (Single)(commreply.data[0] + dxY * outpos.y + Xscale * outpos.x);
            commreply.data[1] = (Single)(commreply.data[1] + dyX * outpos.x + Yscale * outpos.y);
            commreply.data[2] = (Single)(commreply.data[2] + dzX * outpos.x + dzY * outpos.y);
            commreply.data[3] = (Single)(RobotLoadPoints.PickTrayOrg.r + RobotLoadPoints.PickTrayOrg.corrR);


            if (commreply.data[0] == 0 && commreply.data[1] == 0) commreply.result = false;
            return commreply;
        }
        private RobotFunctions.CommReply GetPlaceTray(int partid, bool corr = false)
        {

            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();
            Array.Resize<Single>(ref commreply.data, 4);
            //TrayPartId = int.Parse(txtPartID.Text);
            // OrderPartId = int.Parse(txtOrderPartId.Text);
            Single x = 0;
            Single y = 0;

            //if (MyStatic.chkDebug)
            //{
            //    Array.Resize<Cell>(ref TrayOutCell1, TrayInsertsOnY * TrayInsertsOnX);

            //}
            //dont pick from first and last col
            if (TrayInsertsOnY == 18 && TrayInsertsOnX == 3)
            {
                if (partid == 0 || partid == 1 || partid == 2) partid = 3;
                if (partid == 50)
                {
                    commreply.comment = "last part";
                }
            }
            else if (TrayInsertsOnY == 18 && TrayInsertsOnX == 2)
            {

            }


            position outpos = new position();
            outpos.Error = "";

            if (corr)
            {
                for (int i = 0; i < TrayOutCell1.Length; i++)

                {
                    TrayOutCell1[i].mark = 1;
                    TrayOutCell1[i].corrX = 0;
                    TrayOutCell1[i].corrY = 0;
                    TrayOutCell1[i].corrZ = 0;
                    TrayOutCell1[i].corrR = 0;


                }
            }



            if (partid >= TrayOutCell1.Length - 1)
            {
                commreply.comment = "last part";
            }
            if (partid >= TrayOutCell1.Length) partid = TrayOutCell1.Length - 1;
            int partindex = partid;
            Panel panel = panelTrayOut;
            DrawTrayOut(panel, ref partindex, ref outpos, 0, 0);
            int cntPartsLeft = 0;
            if (partindex + 1 < TrayOutCell1.Length)//how match parts left to pick
            {
                for (int i = partindex + 1; i < TrayOutCell1.Length; i++)
                {
                    if (TrayOutCell1[i].mark != null && TrayOutCell1[i].mark == 1) cntPartsLeft++;

                }
                if (cntPartsLeft == 0)
                    commreply.comment = "last part";
            }
            x = 0; y = 0;
            commreply.result = true;

            Single dz = ZdiamoffsetPickTray;

            commreply.data[0] = (Single)(RobotLoadPoints.PlaceTrayOrg.x + outpos.x) + TrayOffsetX;
            commreply.data[1] = (Single)(RobotLoadPoints.PlaceTrayOrg.y + outpos.y) + TrayOffsetY;
            commreply.data[2] = (Single)(RobotLoadPoints.PlaceTrayOrg.z + dz) + TrayOffsetZ;
            commreply.data[3] = (Single)(RobotLoadPoints.PlaceTrayOrg.r);




            inv.settxt(lblTrayX, commreply.data[0].ToString("0.0"));
            inv.settxt(lblTrayY, commreply.data[1].ToString("0.0"));
            inv.settxt(lblTrayZ, commreply.data[2].ToString("0.0"));
            inv.settxt(lblTrayR, commreply.data[3].ToString("0.0"));

            //inv.settxt(lblTraydX, (dxY * outpos.y + Xscale * outpos.x).ToString("0.0"));
            //inv.settxt(lblTraydY, (dyX * outpos.x + Yscale * outpos.y).ToString("0.0"));
            //inv.settxt(lblTraydZ, (dzX * outpos.x + dzY * outpos.y).ToString("0.0"));

            commreply.data[0] = (Single)(commreply.data[0] + dxY * outpos.y + Xscale * outpos.x);
            commreply.data[1] = (Single)(commreply.data[1] + dyX * outpos.x + Yscale * outpos.y);
            commreply.data[2] = (Single)(commreply.data[2] + dzX * outpos.x + dzY * outpos.y);
            commreply.data[3] = (Single)(RobotLoadPoints.PlaceTrayOrg.r + RobotLoadPoints.PlaceTrayOrg.corrR);


            if (commreply.data[0] == 0 && commreply.data[1] == 0) commreply.result = false;
            return commreply;
        }
        public void DrawTrayOut(Panel panel, ref int partindex, ref position outpos, int from = 0, int to = 0)
        {
            int partIndex = partindex;
            position Outpos = outpos;

            if (InvokeRequired)
            {

                //FanucFunctions.position Outpos = new FanucFunctions.position();
                //this.Invoke(new Action(() => InvDrawTrayOutMatrix(panel, ref partIndex, ref Outpos, from, to)));
                this.Invoke(new Action(() => PickCoord(panel, ref partIndex, ref Outpos)));//(panel, ref partIndex, ref Outpos, from, to)); ;
                outpos = Outpos;
                return;
            }
            else
            {
                //InvDrawTrayOutMatrix(panel, ref partindex, ref outpos, from, to);
                PickCoord(panel, ref partIndex, ref Outpos);
            }
        }
        private void InvDrawTrayOut(Panel panel, ref int Partindex, ref position outpos, int from = 0, int to = 0)
        {

            Single scale = (Single)(panel.Height / 310.000);//panel.wi
            panel.Width = (int)(252 * scale);
            Single TrayDeltaX = Single.Parse(txtDx.Text);
            Single TrayDeltaY = Single.Parse(txtDy.Text);
            int TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
            int TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);
            //Single yoff = 0;
            //Single xoff = 0;
            int insertsY = 0;
            int insertsZ = 0;
            Single deltaY = 0;
            Single deltaX = 0;
            int last = 0;
            Single Xmax = -500, Xmin = 500, Ymax = -500, Ymin = 500;

            int partindex = 0;
            string[] arr = new string[100];

            partindex = Partindex;
            if (partindex < 0) partindex = 0;

            //if (tabControl3.SelectedTab == tabPage9) pallet = 1;

            Single MarkingArrayH = TrayDeltaX * (TrayInsertsOnX - 1);
            Single MarkingArrayW = TrayDeltaY * (TrayInsertsOnY - 1);
            outpos.Error = "";
            //xoff = TrayEdgeX;
            //yoff = TrayEdgeY;
            insertsY = TrayInsertsOnY;
            insertsZ = TrayInsertsOnX;
            deltaY = TrayDeltaY;
            deltaX = TrayDeltaX;
            part = new TrayPart[insertsY * insertsZ];
            if (partindex >= insertsY * insertsZ)
            {
                MessageBox.Show("ERROR IN TRAY OUT PART INDEX", "ERROR", MessageBoxButtons.OK,
                           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                //MyStatic.bExitcycle = true;
                partindex = (insertsY * insertsZ) - 1;
                //return;
            }

            //################### update####################################

            double dZ_origin = 0;// Xoff;
            double dY_origin = 0;
            //################### korea1 fini #############################
            Boolean inv_placeY = false;
            Boolean inv_placeZ = false;
            if (TrayDeltaY < 0) inv_placeY = true;
            if (TrayDeltaX < 0) inv_placeZ = true;
            try
            {
                using (Graphics g = panel.CreateGraphics())
                {
                    //BeginGraphics:
                    Pen pen = new Pen(Color.Black, 2);
                    Brush brush = new SolidBrush(Color.Yellow);

                    position pos = new position();

                    part[0].height = (float)(TrayDeltaY * scale * 0.95);//side
                    part[0].width = (float)(TrayDeltaX * scale * 0.95);

                    //Array.Resize<position>(ref cellpos, insertsY * insertsZ);
                    int i = 0;
                    for (int ii = 0; ii < insertsY; ii++)
                    {
                        for (int jj = 0; jj < insertsZ; jj++)
                        {
                            //sqr
                            part[i].Row = ii;
                            part[i].Col = jj;

                            //sqr
                            part[i].y = (ii * deltaY);
                            part[i].x = (jj * deltaX);
                            part[i].Rotate = 0;

                            if (Xmax < part[i].x) Xmax = part[i].x;
                            if (Xmin > part[i].x) Xmin = part[i].x;
                            if (Ymax < part[i].y) Ymax = part[i].y;
                            if (Ymin > part[i].y) Ymin = part[i].y;
                            i++;
                        }
                    }
                    if (Xmax - Xmin > panel.Width / scale)
                    {
                        dFile.WriteLogFile("ERROR CREATE PALLET ON Z AXIS");
                        MessageBox.Show("ERROR CREATE PALLET ON Z AXIS", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }


                    dY_origin = 2;
                    dZ_origin = 2;
                    position[] cellpos1 = new position[part.Length];
                    for (i = 0; i < part.Length; i++)
                    {

                        cellpos1[i].x = -part[i].y;
                        cellpos1[i].y = part[i].x;
                    }

                    ////draw
                    g.Clear(panel.BackColor);
                    //Array.Resize<Cell>(ref cell, part.Length);
                    for (i = 0; i < part.Length; i++)
                    {

                        //mark
                        brush = new SolidBrush(Color.LightGray);

                        last = LastPlace;


                        if (partindex == i)
                        {


                            //if ((TrayInsertsOnX == 2 && TrayInsertsOnY == 10 && partindex >= 20-1) ||  //TrayInsertsOnX * TrayInsertsOnY)
                            //         (TrayInsertsOnX == 3 && TrayInsertsOnY == 18 && partindex >= 51-1) ||
                            //          (TrayInsertsOnX == 2 && TrayInsertsOnY == 18 && partindex >= 34-1) && last>1)

                            if ((last > 1) && (partindex >= last - 1))
                            { outpos.EndOfTray = 1; }

                            outpos.x = part[i].y;
                            outpos.y = part[i].x;
                            outpos.z = 0;// part[i].z;
                            outpos.r = 0;


                        }




                        if (!inv_placeY)
                        {
                            part[i].y = part[i].y * scale + (float)dY_origin * scale;
                            if (part[i].y > panel.Height)
                            { outpos.Error = "Place position Y OUT OF TRAY!"; return; };
                        }
                        else
                        {
                            part[i].y = part[i].y * scale + (float)dY_origin * scale;//inverse place
                            part[i].y = panel.Width + part[i].y; //inverse place
                            if (part[i].y < -panel.Height)
                            { outpos.Error = "Place position Y OUT OF TRAY!"; return; };
                        }
                        if (!inv_placeZ)
                        {
                            part[i].x = part[i].x * scale + (float)dZ_origin * scale;
                            if (part[i].x > panel.Width)
                            { outpos.Error = "Place position X OUT OF TRAY!"; return; };
                        }
                        else
                        {
                            part[i].x = part[i].x * scale + (float)dZ_origin * scale;//inverse place
                            part[i].x = panel.Height + part[i].x; //inverse place
                            if (part[i].x < -panel.Width)
                            { outpos.Error = "Place position X OUT OF TRAY!"; return; };

                        }


                        if (part[i].x < 0)
                        { outpos.Error = "Place position X OUT OF TRAY!"; };
                        if (part[i].y < 0)
                        { outpos.Error = "Place position Y OUT OF TRAY!"; };
                        if (part[i].x > panel.Width)
                        { outpos.Error = "Place position X OUT OF TRAY!"; };
                        if (part[i].y > panel.Height)
                        { outpos.Error = "Place position Y OUT OF TRAY!"; };

                        //##################### korea1 fini ##################
                        if (panel.Name == panelTrayOut.Name)
                        {
                            if (partindex > i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.LightGray);
                            }
                            else if (partindex == i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Yellow);
                            }
                            else if (partindex < i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Lime);
                            }
                        }
                        else if (panel.Name == panelTrayRej.Name)
                        {
                            if (partindex > i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Red);
                            }
                            else if (partindex == i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Yellow);
                            }
                            else if (partindex < i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.LightGray);
                            }
                        }


                        g.FillRectangle(brush, part[i].x, part[i].y, part[0].width, part[0].height);
                        //g.DrawRectangle(pen, (part[i].x - part[0].height / 2), (part[i].y - part[0].width / 2),
                        //         part[0].height, part[0].width);
                        g.DrawRectangle(pen, part[i].x, part[i].y, part[0].width, part[0].height);
                        pen.Width = 1;
                        Single a = part[0].width;
                        if (a > part[0].height) a = part[0].width;

                        brush = new SolidBrush(Color.Silver);
                        //g.DrawString(i.ToString(), this.Font, Brushes.Black, (part[i].y - a / 2), (part[i].x - a / 2.0F));
                        g.DrawString(i.ToString(), this.Font, Brushes.Black, (part[i].x + part[0].width / 2.0F), (part[i].y + part[0].height / 2.0F - 5));


                    }//end for

                    pen.Dispose();
                    brush.Dispose();

                }

                //update texts


                lblTrayY.Text = (outpos.y).ToString("0.0");
                lblTrayX.Text = (outpos.x).ToString("0.0");
                if (panel.Name == panelTrayOut.Name)
                {
                    txtPartCurrRow.Text = (part[partindex].Col + 1).ToString();
                    txtPartCurrCol.Text = (part[partindex].Row + 1).ToString();
                    txtPartID.Text = partindex.ToString();
                }
                else if (panel.Name == panelTrayRej.Name)
                {
                    txtPartCurrRowRej.Text = (part[partindex].Col + 1).ToString();
                    txtPartCurrColRej.Text = (part[partindex].Row + 1).ToString();
                    txtPartIDRej.Text = partindex.ToString();
                }

                if ((outpos.Error != null) && (outpos.Error != ""))
                {
                    dFile.WriteLogFile("DROW PALLET ERROR:" + outpos.Error);
                    MessageBox.Show("DROW PALLET ERROR:" + outpos.Error, "ERROR", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                for (int jj = part.Length - 1; jj > 1; jj--)
                {
                    if (!part[jj].NoPlace) { LastPlace = (jj + 1); break; }
                }


                return;

            }
            catch (Exception er)
            {
                dFile.WriteLogFile("ERROR UPDATE PALLET:" + er.Message);
                MessageBox.Show("ERROR UPDATE PALLET:" + er.Message, "ERROR", MessageBoxButtons.OK,
                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }


        private void LoadTray(string tray)
        {
            string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Tray\\" + tray + ".ini";

            string[][] arrnew = new string[1][];
            arrnew[0] = new string[0];
            //create vars array
            string mess = "";
            if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ TRAY FILE"); return; }
            try
            {
                //create vars array
                //robot 1

                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "TrayDeltaX", arrnew), out TrayDeltaX)) mess = mess + "TrayDeltaX" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "TrayDeltaY", arrnew), out TrayDeltaY)) mess = mess + "TrayDeltaY" + "\r\n";
                if (!int.TryParse(IniData.GetKeyValueArrINI("tray", "PlaceNumRows", arrnew), out TrayInsertsOnX)) mess = mess + "TrayInsertsOnX" + "\r\n";
                if (!int.TryParse(IniData.GetKeyValueArrINI("tray", "PlaceNumCols", arrnew), out TrayInsertsOnY)) mess = mess + "TrayInsertsOnY" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "TrayHeight ", arrnew), out TrayHeight)) mess = mess + "TrayHeight " + "\r\n";
                //if (!int.TryParse(IniData.GetKeyValueArrINI("tray", "TrayId ", arrnew), out MyStatic.TrayId)) mess = mess + "TrayId " + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "TrayDeltaYReal", arrnew), out TrayDeltaYreal)) mess = mess + "TrayDeltaYReal" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "Xscale", arrnew), out Xscale)) mess = mess + "Xscale" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "Yscale", arrnew), out Yscale)) mess = mess + "Yscale" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "RobotTrayOffsetX", arrnew), out TrayOffsetX)) mess = mess + "TrayOffsetX" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "RobotTrayOffsetY", arrnew), out TrayOffsetY)) mess = mess + "TrayOffsetY" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("tray", "RobotTrayOffsetZ", arrnew), out TrayOffsetZ)) mess = mess + "TrayOffsetZ" + "\r\n";

                int diam = (int)Math.Round(Double.Parse(txtPartDiamd.Text), 0);
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Zoffset", "diam" + diam.ToString(), arrnew), out diamOffsetZ)) mess = mess + "diamOffsetZ" + "\r\n";
                ZdiamoffsetPickTray = diamOffsetZ;


                partData = new MyStatic.PartData[TrayInsertsOnX * TrayInsertsOnY];
                //public bool[] Reject;

                for (int i = 0; i < partData.Length; i++)
                {
                    partData[i].State = new int[16];
                    partData[i].Position = (int)MyStatic.E_State.OnTray;
                    partData[i].Reject = new bool[16];

                }
                txtGrip1num.Text = "";
                txtGrip2num.Text = "";
                txtGrip3num.Text = "";
                RobotLoadAct.OnGrip1_PartID = -1;
                RobotLoadAct.OnGrip2_PartID = -1;
                InspectStationAct.OnFooterGrip3_PartID = -1;
                InspectStationAct.State = new int[16];
                InspectStationAct.Reject = new bool[16];



            }
            catch (Exception err)
            { MessageBox.Show("ERROR READ TRAY FILE " + err); return; }
        }

        private async void panelTrayOut_MouseDown(object sender, MouseEventArgs e)
        {
            int pallet = 0;
            //int i = 0;

            //NoPlace = new int[100];
            string[] arr;
            arr = new string[100];
            List<string> lst;
            lst = new List<string>();
            //Single MarkingArrayW = 306;
            //Single MarkingArrayH = 240;


            try
            {
                Single TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
                Single TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
                Single scale1 = this.panelTrayOut.Width / (float)(TrayInsertsOnX * TrayDeltaX);
                Single scale2 = this.panelTrayOut.Height / (float)(TrayInsertsOnY * TrayDeltaY);

                for (int j = 0; j < pntsrot.Length; j++)
                {

                    //######### korea1 ################
                    if (((scale1 * pntsrot[j].X / 10 - scale1 * TrayDeltaX) <= e.X) && ((scale1 * pntsrot[j].X / 10 + scale1 * TrayDeltaX) >= e.X)
                        && ((scale2 * pntsrot[j].Y / 10 - scale2 * TrayDeltaY) <= e.Y) && ((scale2 * pntsrot[j].Y / 10 + scale2 * TrayDeltaY) >= e.Y))
                    {


                        TrayPartId = j;// TrayInsertsOnY * TrayInsertsOnX - i - 1;
                        //if (TrayInsertsOnX == 3 && TrayInsertsOnY == 18)
                        //{ 
                        //    if (TrayPartId == 0 || TrayPartId == 1 || TrayPartId == 2) TrayPartId = 3;
                        //    if (TrayPartId > 50) TrayPartId = 50;

                        //}
                        //if (TrayInsertsOnX == 2 && TrayInsertsOnY == 18)
                        //{
                        //    if (TrayPartId == 0 || TrayPartId == 1 ) TrayPartId = 2;
                        //    if (TrayPartId > 33) TrayPartId = 33;
                        //}

                        txtPartID.Text = TrayPartId.ToString();
                        bool corr = false;

                        int partid = TrayPartId;
                        var task02 = Task.Run(() => GetPlaceTray(partid, corr));
                        await task02;
                        RobotFunctions.CommReply posrep2 = task02.Result;
                        //position basepos = new position();
                        if (!posrep2.result)
                        {
                            MyStatic.bExitcycle = true;
                            MessageBox.Show("ERROR3 Pick Box Pos COORDINATES!", "ERROR", MessageBoxButtons.OK,
                                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return;
                        }
                        else
                        {
                            Single x = posrep2.data[0];
                            Single y = posrep2.data[1];
                            Single z = posrep2.data[2];
                            Single r = posrep2.data[3];
                            //label225.Text = x.ToString() + "\r\n" +
                            //    y.ToString() + "\r\n" +
                            //    z.ToString() + "\r\n" +
                            //    r.ToString() + "\r\n";
                            //TrayPartId++;

                            txtPartID.Text = TrayPartId.ToString();
                            // txtOrderPartId.Text = TrayPartId.ToString();
                        }
                        return;

                    }
                    //i++;

                }
            }
            catch (Exception ex)
            { }
        }

        private void panelTrayOut_Paint(object sender, PaintEventArgs e)
        {
            try
            {

                position outpos = new position();
                outpos.Error = "";
                //NoPlace();
                int partindex = TrayPartId;
                Panel panel = panelTrayOut;
                DrawTrayOut(panel, ref partindex, ref outpos);
                //TrayPartId = partindex;
            }
            catch (Exception ex) { }
        }

        private async void btnNextPick_Click(object sender, EventArgs e)
        {
            try
            {
                TrayPartId++;
                txtPartID.Text = TrayPartId.ToString();
                bool corr = false;

                int partid = TrayPartId;
                var task02 = Task.Run(() => GetPickTray(partid, corr));
                await task02;
                RobotFunctions.CommReply posrep2 = task02.Result;
                //position basepos = new position();
                if (!posrep2.result)
                {
                    MyStatic.bExitcycle = true;
                    MessageBox.Show("ERROR3 Pick Pos COORDINATES!", "ERROR", MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                else
                {
                    Single x = posrep2.data[0];
                    Single y = posrep2.data[1];
                    Single z = posrep2.data[2];
                    Single r = posrep2.data[3];
                    //label225.Text = x.ToString() + "\r\n" +
                    //    y.ToString() + "\r\n" +
                    //    z.ToString() + "\r\n" +
                    //    r.ToString() + "\r\n";
                    //TrayPartId++;

                    txtPartID.Text = TrayPartId.ToString();

                }
                return;
            }

            catch (Exception ex)
            {
            }
        }


        private void cmbTray_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                cmbOrderTray.Text = cmbTray.Text;
                TrayUpdate();
            }
            catch (Exception ex) { }
        }

        public Single rotation_ang = 0.0f;
        public Single ScaleWidth = 0.0f;
        public Single ScaleHeight = 0.0f;
        private void TrayUpdate()
        {
            try
            {
                LoadTray(cmbTray.Text);
                inv.settxt(txtDx, TrayDeltaX.ToString());
                inv.settxt(txtDy, TrayDeltaY.ToString());
                inv.settxt(txtPlaceNumRow, TrayInsertsOnX.ToString());
                inv.settxt(txtPlaceNumCol, TrayInsertsOnY.ToString());

                Array.Resize<Cell>(ref TrayOutCell1, TrayInsertsOnY * TrayInsertsOnX);
                Array.Resize<Cell>(ref TrayOutCell2, TrayInsertsOnY * TrayInsertsOnX);

                position outpos = new position();
                outpos.Error = "";
                //NoPlace();
                int partindex = 0;
                int partindexRej = 0;
                Panel panel = panelTrayOut;
                DrawTrayOut(panel, ref partindex, ref outpos);
                Panel panel1 = panelTrayRej;
                DrawTrayOut(panel1, ref partindexRej, ref outpos);
                //TrayPartId = partindex;

                TrayPartId = 0;
                TrayPartIdRej = 0;

                inv.settxt(txtPartID, TrayPartId.ToString());
                inv.settxt(txtPartIDRej, TrayPartIdRej.ToString());

                if (newFrmMain.cmbTray.Text == "10x2")//master tray for teaching p0-p3
                {
                    rotation_ang = -(Single)Math.Atan2(newFrmMain.RobotLoadPoints.P1.y - newFrmMain.RobotLoadPoints.P0.y, newFrmMain.RobotLoadPoints.P1.x - newFrmMain.RobotLoadPoints.P0.x);
                    rotation_ang = (Single)(rotation_ang * 360.0 / (2 * Math.PI));
                    ScaleWidth = (Single)Math.Abs(newFrmMain.RobotLoadPoints.P2.y - newFrmMain.RobotLoadPoints.P0.y) / ((TrayInsertsOnX - 1) * TrayDeltaX);
                    ScaleHeight = (Single)Math.Abs(newFrmMain.RobotLoadPoints.P1.x - newFrmMain.RobotLoadPoints.P0.x) / ((TrayInsertsOnY - 1) * TrayDeltaY);
                    //save
                    string filename = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Tray\\" + "10x2" + ".ini";
                    string[][] arrsave = new string[1][];
                    arrsave[0] = new string[1];
                    string[] s;
                    s = new string[9];
                    string mess = "";


                    //robot load

                    if (!RobotData.CreateKeyValueArr("tray", "rotation_ang", rotation_ang.ToString("0.0000000"), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr("tray", "ScaleWidth", ScaleWidth.ToString("0.00000"), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr("tray", "ScaleHeight", ScaleHeight.ToString("0.00000"), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }

                    string error = "";
                    if (!RobotData.WriteIniFile(filename.Trim(), arrsave, out error))
                    {
                        MessageBox.Show("ERROR SAVE");
                    }
                }
                else
                {
                    //load master data
                    string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Tray\\" + "10x2" + ".ini";

                    string[][] arrnew = new string[1][];
                    arrnew[0] = new string[0];
                    //create vars array
                    string mess = "";
                    if (!newFrmMain.IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ TRAY FILE"); return; }
                    if (!Single.TryParse(newFrmMain.IniData.GetKeyValueArrINI("tray", "rotation_ang", arrnew), out rotation_ang)) mess = mess + "rotation_ang" + "\r\n";
                    if (!Single.TryParse(newFrmMain.IniData.GetKeyValueArrINI("tray", "ScaleWidth", arrnew), out ScaleWidth)) mess = mess + "ScaleWidth" + "\r\n";
                    if (!Single.TryParse(newFrmMain.IniData.GetKeyValueArrINI("tray", "ScaleHeight", arrnew), out ScaleHeight)) mess = mess + "ScaleHeight" + "\r\n";
                }
                //TrayOutCell1 = new Cell[TrayOutCell.Length];
                //for (int i = 0; i < TrayOutCell1.Length; i++) TrayOutCell1[i] = TrayOutCell[i];
            }
            catch (Exception ex) { }
        }
        #endregion pallet
        #region beckhoff//----------------------beckhoff---------------------------------------------------------------------------
        public int Axis = 0;
        private async void btnCurrPosCams_Click(object sender, EventArgs e)
        {
            //current position
            try
            {

                ControlsEnable(false);
                Single x = 0;
                Single y = 0;
                Single z = 0;

                inv.settxt(txtCurrPosCams, "");
                inv.set(upDownControlPosSt, "UpDownValue", 0f);
                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;
                var task1 = Task.Run(() => RunCurrPosCams(axis));

                await task1;
                CommReply reply = new CommReply();
                reply.result = false;
                reply = task1.Result;
                ControlsEnable(true);

                if (!(reply.status == "" || reply.status == null))
                {
                    MessageBox.Show("ERROR READ COORDINATES! " + "\r" + reply.status);
                    return;
                }
                if (axis == 1)
                {
                    inv.settxt(txtCurrPosCams, reply.data[2].ToString("0.000"));
                    inv.set(upDownControlPosSt, "UpDownValue", Single.Parse(reply.data[2].ToString("0.000")));
                }
                else if (axis == 2)
                {
                    inv.settxt(txtCurrPosCams, reply.data[3].ToString("0.000"));
                    inv.set(upDownControlPosSt, "UpDownValue", Single.Parse(reply.data[3].ToString("0.000")));
                }
                else if (axis == 3)
                {
                    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));
                    inv.set(upDownControlPosSt, "UpDownValue", Single.Parse(reply.data[4].ToString("0.000")));
                }
                else if (axis == 4)
                {
                    inv.settxt(txtCurrPosCams, reply.data[5].ToString("0.000"));
                    inv.set(upDownControlPosSt, "UpDownValue", Single.Parse(reply.data[5].ToString("0.000")));
                }
                else if (axis == 5)
                {
                    inv.settxt(txtCurrPosCams, reply.data[6].ToString("0.000"));
                    inv.set(upDownControlPosSt, "UpDownValue", Single.Parse(reply.data[6].ToString("0.000")));
                }
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;


            }
        }
        private CommReply RunCurrPosCams(int axis = 0)//current position
        {
            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                //move jog

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.CurrentPos;//curr pos
                ParmsPlc.SendParm[1] = axis;//cam
                ParmsPlc.SendParm[10] = 0.5f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                ParmsPlc.SendParm = null;
                //wait fini async
                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }

        private void cmbAxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //1:CAM2 Z
                //2:Lamp1 Z
                //3:CAM1 X
                //4:FOOTER R
                //5:FOOTER X

                btnMin.Enabled = true;
                btnPlus.Enabled = true;

                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                if (Axis == 1)
                {
                    string pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwDown.ico";
                    btnPlus.Image = new Bitmap(pict);
                    pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwUp.ico";
                    btnMin.Image = new Bitmap(pict);
                    trackBarSpeedSt.Minimum = 2;

                }
                else if (Axis == 2)
                {
                    string pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwDown.ico";
                    btnPlus.Image = new Bitmap(pict);
                    pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwUp.ico";
                    btnMin.Image = new Bitmap(pict);
                    trackBarSpeedSt.Minimum = 2;
                }
                else if (Axis == 3)
                {
                    string pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwLeft.ico";
                    btnPlus.Image = new Bitmap(pict);
                    pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwRight.ico";
                    btnMin.Image = new Bitmap(pict);
                    trackBarSpeedSt.Minimum = 2;
                }
                else if (Axis == 4)
                {
                    string pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwRotLeft.ico";
                    btnPlus.Image = new Bitmap(pict);
                    pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwRotRight.ico";
                    btnMin.Image = new Bitmap(pict);
                    trackBarSpeedSt.Minimum = 2;
                }
                else if (Axis == 5)
                {
                    string pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwLeft.ico";
                    btnPlus.Image = new Bitmap(pict);
                    pict = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\ArwRight.ico";
                    btnMin.Image = new Bitmap(pict);
                    trackBarSpeedSt.Minimum = 5;
                }

                inv.settxt(txtSpeedMax, AxStatus[Axis - 1].Vmax.ToString());
            }
            catch (Exception ex) { }
        }

        private void txtMess_DockChanged(object sender, EventArgs e)
        {

        }

        private void txtMess_DoubleClick(object sender, EventArgs e)
        {
            
            WriteToFile(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Log\\Beckhofflog " + DateTime.Now.ToString("yyyy-MM-dd HH") + ".ini", txtMess.Text);
            inv.settxt(txtMess, "");
        }

        private async void btnMoveSt_Click(object sender, EventArgs e)
        {
            try
            {
                ControlsEnable(false);
                //if (trackBarSpeedSt.Value > 10) trackBarSpeedSt.Value = 10;
                Single dist = 0;
                inv.set(btnPlus, "Enabled", false);
                //Axis = int.Parse(cmbAxes.Text);
                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;
                Single Speed = AxStatus[axis - 1].Vmax;//7200;
                Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                dist = Single.Parse(txtCurrPosCams.Text);


                var task1 = Task.Run(() => MoveAbs(0, axis, dist, speed));
                await task1;


                CommReply reply = new CommReply();
                reply.result = false;
                reply = task1.Result;
                inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                if (!(reply.status == "" || reply.status == null))
                {
                    MessageBox.Show("ERROR MOVE FINE! " + "\r" + reply.status);
                    ControlsEnable(true);
                    inv.set(btnPlus, "Enabled", true);
                    return;
                }
                if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return; };

                ControlsEnable(true);
                inv.set(btnPlus, "Enabled", true);
            }
            catch (Exception ex) { MessageBox.Show("ERROR MOVE FINE! "); }
        }
        private CommReply MoveAbs(int device, Single ax, Single Coord, Single speed)//global power on
        {

            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                //move jog

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveAbs;
                //ParmsPlc.SendParm[1] = device;//move vel
                ParmsPlc.SendParm[1] = ax;//x=1 y=2 z=3
                ParmsPlc.SendParm[2] = Coord;//0=negative 1=positive
                ParmsPlc.SendParm[3] = speed;//speed
                ParmsPlc.SendParm[10] = 10.5f;//tmout


                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                inv.set(upDownControlPosSt, "UpDownValue", reply.data[4]);
                //Thread.Sleep(100);


                ParmsPlc.SendParm = null;
                device = 0;
                int axis = 0;

                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        private CommReply MoveAllAbs(Single AxEnable, Single Coord1, Single[] Coord, Single speed)//global power on
        {

            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                //move jog

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveAll;
                //ParmsPlc.SendParm[1] = device;//move vel
                ParmsPlc.SendParm[1] = AxEnable;//x=1 y=2 z=3
                ParmsPlc.SendParm[2] = Coord[0];
                ParmsPlc.SendParm[3] = Coord[1];
                ParmsPlc.SendParm[4] = Coord[2];
                ParmsPlc.SendParm[5] = Coord[3];
                ParmsPlc.SendParm[6] = Coord[4];
                ParmsPlc.SendParm[7] = speed;//speed from max
                ParmsPlc.SendParm[10] = 30.5f;//tmout


                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                //inv.set(upDownControlPosSt, "UpDownValue", reply.data[4]);
                //Thread.Sleep(100);


                ParmsPlc.SendParm = null;

                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        bool AxisMove = false;
        private async void btnPwrOnSt_Click(object sender, EventArgs e)
        {
            try
            {
                Single Speed = 1000;
                if (Axis == 1) Speed = 1000; else Speed = 7200;
                if (((System.Windows.Forms.Button)sender).Name == "btnPwrOnSt")
                {
                    ControlsEnable(false);

                    btnPwrOnSt.Enabled = false;
                    int axis = Axis;
                    if (chkAllAxises.Checked) axis = 0;
                    var task1 = Task.Run(() => RunPwrSt(true, axis));
                    await task1;

                    MyStatic.bPower = false;

                    btnPwrOnSt.Enabled = true;
                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    ControlsEnable(true);

                    if (!(reply.status == "" || reply.status == null))
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR POWER ON! " + "\r" + reply.status);
                        return;
                    }
                    if (!reply.result || reply.data[1] != 0)
                    {
                        SetTraficLights(0, 0, 1, 0); MessageBox.Show("ERROR POWER ON"); return;
                    };//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                }
                else if (((System.Windows.Forms.Button)sender).Name == "btnPwrOffSt")
                {
                    ControlsEnable(false);

                    btnPwrOnSt.Enabled = false;
                    int axis = Axis;
                    if (chkAllAxises.Checked) axis = 0;
                    var task1 = Task.Run(() => RunPwrSt(false, axis));
                    await task1;

                    MyStatic.bPower = false;

                    btnPwrOnSt.Enabled = true;
                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    ControlsEnable(true);

                    if (!(reply.status == "" || reply.status == null))
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR POWER ON! " + "\r" + reply.status);
                        return;
                    }
                    if (reply.data[1] != 0)
                    {
                        SetTraficLights(0, 0, 1, 0); MessageBox.Show("ERROR POWER ON"); return;
                    };//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                }
                else if (((System.Windows.Forms.Button)sender).Name == "btnStopSt")
                {
                    ControlsEnable(true);
                    AxisMove = false;
                    int device = 0;
                    int axis = Axis;

                    var task1 = Task.Run(() => StopStations_Jog(device, axis));
                    await task1;
                    Thread.Sleep(100);
                    //var task2 = Task.Run(() => ReadCurrent());
                    //await task2;
                }
                else if (((System.Windows.Forms.Button)sender).Name == "btnRstSt")
                {
                    ControlsEnable(false);
                    MyStatic.bReset = false;
                    Beckhoff.tcAds.Disconnect();
                    Thread.Sleep(100);
                    Beckhoff.tcAds.Connect(PlcNetID, PlcPort);
                    btnPwrOnSt.Enabled = false;
                    int axis = Axis;
                    var task1 = Task.Run(() => RunRstSt(axis));
                    await task1;

                    MyStatic.bPower = false;

                    btnPwrOnSt.Enabled = true;
                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    //btn_status(true);

                    if (!reply.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR RESET! " + "\r" + reply.status);
                        return;
                    }
                    if (reply.data[1] != 0)
                    {
                        SetTraficLights(0, 0, 1, 0); MessageBox.Show("ERROR RESET"); return;
                    };//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    Thread.Sleep(500);
                    //power on
                    axis = 0;
                    var task10 = Task.Run(() => RunPwrSt(true, axis));
                    await task10;

                    MyStatic.bPower = false;

                    btnPwrOnSt.Enabled = true;
                    reply = new CommReply();
                    reply.result = false;
                    reply = task10.Result;
                    ControlsEnable(true);

                    if (!(reply.status == "" || reply.status == null))
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR POWER ON! " + "\r" + reply.status);
                        return;
                    }
                    if (reply.data[1] != 0) { MessageBox.Show("ERROR POWER ON"); return; };
                    //send parameters
                    var task = Task.Run(() => SendPlcParameters());
                    await task;
                    if (!task.Result.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR SEND PLC PARAMETERS!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    //axis max speed

                    var task2 = Task.Run(() => RunAxisStatus(6));
                    await task2;
                    reply = task2.Result;
                    if (!reply.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR READ STATUS " + "\r");
                        return;
                    }
                    AxStatus[0].Vmax = reply.data[3];
                    AxStatus[1].Vmax = reply.data[4];
                    AxStatus[2].Vmax = reply.data[5];
                    AxStatus[3].Vmax = reply.data[6];
                    AxStatus[4].Vmax = reply.data[7];
                    SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                }


                else if (((System.Windows.Forms.Button)sender).Name == "btnMoveSt")
                {

                    ControlsEnable(false);
                    Single pos = 0;
                    btnMoveSt.Enabled = false;
                    int axis = Axis;

                    Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                    pos = Single.Parse(txtCurrPosCams.Text);


                    var task1 = Task.Run(() => MoveAbsSt(axis, pos, speed));
                    await task1;

                    btnMoveSt.Enabled = true;
                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    ControlsEnable(true);

                    if (!(reply.status == "" || reply.status == null))
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERRORMOVE MOVE! " + "\r" + reply.status);
                        return;
                    }
                    if (reply.data[1] != 0)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("ERROR MOVE"); return;
                    };
                    SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0


                }
            }
            catch (Exception ex)
            {
                SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                MessageBox.Show("ERROR EXECUTE " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private CommReply MoveAbsSt(int station, Single pos, Single speed)
        {

            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            reply.result = false;
            Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
            for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

            ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveAbs;
            ParmsPlc.SendParm[1] = station;
            ParmsPlc.SendParm[2] = pos;
            ParmsPlc.SendParm[3] = speed;
            ParmsPlc.SendParm[10] = 5f;//tmout

            reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc);

            ParmsPlc.SendParm = null;
            return reply;
        }
        private async Task<CommReply> RunPwrSt(bool set, int axis = 0)//stations power on
        {

            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            reply.result = false;
            Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
            for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
            if (set)
            {
                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Power;//power on
                ParmsPlc.SendParm[1] = axis;//camSt2
                ParmsPlc.SendParm[2] = 1;//on

                ParmsPlc.SendParm[10] = 12f;//tmout
            }
            else
            {
                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Power;//power on
                ParmsPlc.SendParm[1] = axis;//camSt2
                ParmsPlc.SendParm[2] = 2;//off

                ParmsPlc.SendParm[10] = 12f;//tmout


                ParmsPlc.SendParm[10] = 1f;//tmout
            }

            var task1 = Task.Run(() => Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc));
            await task1;
            //if (!Beckhoff_Gen.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, ref Error)) return false;
            ParmsPlc.SendParm = null;
            //wait fini async
            reply = task1.Result;
            if (reply.data != null && reply.data[3] == 0 && reply.data[0] != 0) reply.status = "EMERGENSY STOP PRESSED";
            return reply;
        }
        private CommReply RunRstSt(int axis)//stations power on
        {

            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            reply.result = false;
            Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
            for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

            ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Reset;//power on
            ParmsPlc.SendParm[1] = axis;//camSt2

            ParmsPlc.SendParm[10] = 12f;//tmout


            reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc);

            //if (!Beckhoff_Gen.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, ref Error)) return false;
            ParmsPlc.SendParm = null;
            //wait fini async

            return reply;
        }
        private CommReply StopStations_Jog(int device, Single ax)//global stop
        {

            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                //move jog

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Stop;//stop
                //ParmsPlc.SendParm[1] = device;//device
                ParmsPlc.SendParm[1] = ax;//x=1 y=2 z=3 all=0
                ParmsPlc.SendParm[10] = 0.5f;//tmout
                ax = 1;
                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                ParmsPlc.SendParm = null;
                //wait fini async

                if (reply.result && reply.data != null && reply.data.Length > 10) inv.set(upDownControlPosSt, "UpDownValue", Single.Parse(reply.data[Axis + 2].ToString("0.000")));
                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }

        public void LoadIniIO(string filename)
        {

            string[][] arrnew = new string[1][];
            arrnew[0] = new string[0];
            if (!RobotData.ReadIniFile(filename, ref arrnew)) { MessageBox.Show("ERROR INI READ FILE"); return; }
            try
            {

                for (int i = 0; i < 16; i++)
                {
                    ArrayDI1.ControlIndex = i;
                    string s = "In1." + i.ToString();
                    s = RobotData.GetKeyValueArrINI("IN1", s, arrnew);
                    s = s.Replace("_", "\r"); ArrayDI1.ControlText = s;
                    if (ArrayDI1.ControlText == "") ArrayDI1.ControlVisible = false; else ArrayDI1.ControlVisible = true;
                }
                for (int i = 0; i < 16; i++)
                {
                    ArrayDI2.ControlIndex = i;
                    string s = "In2." + i.ToString();
                    s = RobotData.GetKeyValueArrINI("IN2", s, arrnew);
                    s = s.Replace("_", "\r"); ArrayDI2.ControlText = s;
                    if (ArrayDI2.ControlText == "") ArrayDI2.ControlVisible = false; else ArrayDI2.ControlVisible = true;
                }

                for (int i = 0; i < 16; i++)
                {
                    ArrayDI3.ControlIndex = i;
                    string s = "Out1." + i.ToString();
                    s = RobotData.GetKeyValueArrINI("OUT1", s, arrnew);
                    s = s.Replace("_", "\r"); ArrayDI3.ControlText = s;
                    if (ArrayDI3.ControlText == "") ArrayDI3.ControlVisible = false; else ArrayDI3.ControlVisible = true;
                }


            }
            catch (Exception ex) { };
        }
        UserControl.ControlsArrayControl ArrayDI;
        UserControl.ControlsArrayControl ArrayDO;
        Label[] A1 = new Label[16];
        Label[] A2 = new Label[16];
        Label[] A3 = new Label[16];
        public void RefreshIO(ushort[] values)
        {

            try
            {
                ArrayDI = ArrayDI1;

                int ii;
                int j;
                for (j = 0; j < values.Length; j++)
                {

                    //switch (j)
                    //{
                    //    case 0: ArrayDI = ArrayDI1; break;
                    //    case 1: ArrayDI = ArrayDI2; break;
                    //    //case 2: ArrayDI = ArrayDI3; break;

                    //    default: break;
                    //}

                    if (j == 0)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            ii = (Int32)Math.Pow(2, i);
                            if ((Convert.ToInt32(values[j]) & ii) == ii)
                            {
                                ArrayDI1.ControlIndex = i;
                                if (ArrayDI1.ControlBackColor != Color.Lime) ArrayDI1.ControlBackColor = Color.Lime;

                            }
                            else
                            {
                                ArrayDI1.ControlIndex = i;
                                if (ArrayDI1.ControlBackColor != Color.Yellow) ArrayDI1.ControlBackColor = Color.Yellow;

                            }

                            //Thread.Sleep(1);

                        }
                    }
                    if (j == 1)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            ii = (Int32)Math.Pow(2, i);
                            if ((Convert.ToInt32(values[j]) & ii) == ii)
                            {
                                ArrayDI2.ControlIndex = i;
                                if (ArrayDI2.ControlBackColor != Color.Lime) ArrayDI2.ControlBackColor = Color.Lime;

                            }
                            else
                            {
                                ArrayDI2.ControlIndex = i;
                                if (ArrayDI2.ControlBackColor != Color.Yellow) ArrayDI2.ControlBackColor = Color.Yellow;

                            }

                            //Thread.Sleep(1);

                        }
                    }
                }

            }
            catch (Exception ex) { }


            Thread.Sleep(5);



        }


        public void RefreshOUT(ushort[] values)
        {

            try
            {

                ArrayDO = ArrayDI3;

                int ii;
                int j;
                for (j = 0; j < 1; j++)
                {

                    //switch (j)
                    //{
                    //    case 0: ArrayDO = ArrayDI3; break;

                    //    default: break;
                    //}


                    for (int i = 0; i < 16; i++)
                    {
                        ii = (Int32)Math.Pow(2, i);
                        if ((Convert.ToInt32(values[j]) & ii) == ii)
                        {
                            ArrayDI3.ControlIndex = i;

                            if (ArrayDI3.ControlBackColor != Color.LightPink) ArrayDI3.ControlBackColor = Color.LightPink;

                        }
                        else
                        {

                            ArrayDI3.ControlIndex = i;

                            if (ArrayDI3.ControlBackColor != Color.LightBlue) ArrayDI3.ControlBackColor = Color.LightBlue;

                        }



                    }

                }

                Thread.Sleep(5);
            }
            catch (Exception ex) { }

        }
        public CommReply RunGen419(Single cont)//read inputs
        {

            string Error = "";
            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            try
            {

                if (MyStatic.bReset) { reply.result = true; return reply; }
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                ParmsPlc.SendParm[0] = 419;//IO
                ParmsPlc.SendParm[1] = 1;//block1
                ParmsPlc.SendParm[2] = cont;// Convert.ToSingle(MyStatic.ReadIOcont);
                ParmsPlc.SendParm[10] = 0.5f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc);

                //if (!Beckhoff_Gen.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, ref Error)) return false;
                ParmsPlc.SendParm = null;
                //wait fini async

                //refreshio
                string IO = "";


                IO = IO + ((int)reply.data[2]).ToString() + "\r\n";
                IO = IO + ((int)reply.data[3]).ToString() + "\r\n";
                //IO = IO + ((int)reply.data[4]).ToString() + "\r\n";

                //MessageBox.Show(IO);
                ushort[] res = new ushort[7];
                for (int i = 0; i < 2; i++)
                {
                    res[i] = Convert.ToUInt16(reply.data[i + 2]);
                }
                RefreshIO(res);
                //
                //return reply;

                return reply;
            }
            catch
            {
                reply.result = false;
                return reply;
            }

        }
        public CommReply RunIO501(Single cont)//read inputs
        {

            string Error = "";
            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            try
            {

                if (MyStatic.bReset) { reply.result = true; return reply; }
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                ParmsPlc.SendParm[0] = 501;//IO
                ParmsPlc.SendParm[1] = 1;//block1
                ParmsPlc.SendParm[2] = cont;// Convert.ToSingle(MyStatic.ReadIOcont);
                ParmsPlc.SendParm[10] = 0.5f;//tmout
                                             //int StartAddressSend = 10;//cam1

                //if (!Beckhoff_Gen.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, ref Error)) return false;
                reply = Beckhoff_IO.PlcSendCmd(StartAddressSendReadIO, ParmsPlc);

                ParmsPlc.SendParm = null;
                //wait fini async

                //refreshio
                string IO = "";


                IO = IO + ((int)reply.data[2]).ToString() + "\r\n";
                IO = IO + ((int)reply.data[3]).ToString() + "\r\n";
                IO = IO + ((int)reply.data[4]).ToString() + "\r\n";

                //MessageBox.Show(IO);
                ushort[] res = new ushort[7];
                for (int i = 0; i < 3; i++)
                {
                    res[i] = Convert.ToUInt16(reply.data[i + 2]);
                }


                return reply;
            }
            catch
            {
                reply.result = false;
                return reply;
            }

        }
        public CommReply RunGen420(int output, int card)//read outputs
        {

            string Error = "";
            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            try
            {

                reply.result = false;
                if (MyStatic.bReset) { reply.result = true; return reply; }
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                ParmsPlc.SendParm[0] = 420;//IO
                ParmsPlc.SendParm[1] = 1;//block1
                ParmsPlc.SendParm[2] = output;// Convert.ToSingle(MyStatic.ReadIOcont);
                ParmsPlc.SendParm[3] = card;
                ParmsPlc.SendParm[4] = Convert.ToSingle(MyStatic.ReadIOcont);
                ParmsPlc.SendParm[10] = 0.5f;//tmout
                                             //int StartAddressSend = 10;//cam1

                //if (!Beckhoff_Gen.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, ref Error)) return false;
                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc);

                ParmsPlc.SendParm = null;
                //wait fini async

                //refreshio
                string IO = "";


                IO = IO + ((int)reply.data[2]).ToString() + "\r\n";

                //IO = IO + ((int)reply.data[10]).ToString() + "\r\n";
                //MessageBox.Show(IO);
                ushort[] res = new ushort[8];
                for (int i = 0; i < 1; i++)
                {
                    res[i] = Convert.ToUInt16(reply.data[i + 2]);
                }
                RefreshOUT(res);

                //
                return reply;
            }
            catch
            {
                reply.result = false;
                return reply;
            }

        }
        public CommReply RunGenIO()//read inputs
        {
            CommReply reply = new CommReply();
            CommReply reply1 = new CommReply();
            try
            {
                while (true)
                {
                    if (MyStatic.bReset)
                    {
                        reply.result = true;
                        return reply;
                    }
                    reply = RunGen419(1);

                    Thread.Sleep(50);
                    reply1 = RunGen420(MyStatic.SetOut, MyStatic.SetCard);
                    if (MyStatic.SetOut != -1) Thread.Sleep(100);
                    MyStatic.SetOut = -1; MyStatic.SetCard = -1;

                    reply1.result = reply1.result && reply.result;
                    //await Task.Delay(20);
                    if (!MyStatic.ReadIOcont)
                    {
                        reply = RunGen419(0);
                        return reply;
                    }
                    Thread.Sleep(100);
                }

                return reply1;
            }
            catch
            {
                reply.result = false;
                MyStatic.SetOut = -1; MyStatic.SetCard = -1;
                return reply;
            }

        }
        #endregion beckhoff//--------------------------------beckhoff------------------------------------------------------------------------
        #region client-server //----------------------/web client-server---------------------------------------------------------------------------
        private void btnStart1_Click(object sender, EventArgs e)
        {

        }
        bool bStop = false;
        private void btnStop1_Click(object sender, EventArgs e)
        {
            bStop = true;
            WC1.StopComm();
            //WC2.StopComm();
            inv.set(btnRead, "Enabled", true);
        }

        private void txtClient_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                txtClient.Text = "";
                txtServer.Text = "";
            }
            catch { }
        }

        private void txtServer_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                txtClient.Text = "";
                txtServer.Text = "";
            }
            catch { }
        }

        private async void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                //while (true)
                //{
                inv.set(btnRead, "Enabled", false);
                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                var task1 = Task.Run(() => CheckComm1(1));
                await task1;
                WebComm.CommReply reply = task1.Result;
                //inv.set(btnRead, "Enabled", true);

                Thread.Sleep(2);

                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                var task2 = Task.Run(() => CheckComm1(2));
                await task2;
                reply = task2.Result;
                //inv.set(btnRead, "Enabled", true);

                WC3.SetControls1(txtClient, this, null, "CognexComm", CognexAddr);
                var task3 = Task.Run(() => CheckComm1(3));
                await task3;
                reply = task3.Result;
                inv.set(btnRead, "Enabled", true);

                Thread.Sleep(2);
                //}
            }
            catch (Exception ex) { MessageBox.Show("ERROR COMMUNICATION" + ex.Message); inv.set(btnRead, "Enabled", true); }
        }

        private void btnStop2_Click(object sender, EventArgs e)
        {
            bStop = true;
            _httpListener.Stop();
            cancelToken.Cancel();
        }

        private async void btnSatrt2_Click(object sender, EventArgs e)
        {
            try
            {

                bStop = false;
                inv.set(btnSatrt2, "Enabled", false);
                //var task = Task.Run(() => Start());
                var task = Task.Run(() => HttpServer());
                await task;
                inv.set(btnSatrt2, "Enabled", true);


            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public RobotFunctions.HostReply ReadHttp()
        {
            RobotFunctions.HostReply reply = new RobotFunctions.HostReply();
            reply.result = false;
            reply.comment = "";
            try
            {
                if (cancelToken != null) cancelToken.Dispose();
                cancelToken = new CancellationTokenSource();
                CancellationToken cancel = cancelToken.Token;
                //_httpListener.Start();

                //HttpListenerContext context = await Task.Run(() => _httpListener.GetContext(), cancel);
                HttpListenerContext context = _httpListener.GetContext();


                inv.settxt(txtServer, txtServer.Text + "Url=>" + context.Request.Url + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");

                // Now, you'll find the request URL in context.Request.Url
                //byte[] _responseArray = Encoding.UTF8.GetBytes("<html><head><title>Localhost server -- port 5000</title></head>" +
                //"<body>Welcome to the <strong>Localhost server</strong> -- <em>port 5000!</em></body></html>"); // get the bytes to response

                string[] s = context.Request.Url.ToString().Split('?');
                if (s.Length > 1)
                {
                    Thread.Sleep(1);
                    //byte[] _responseArray = Encoding.UTF8.GetBytes(s[1] + "!!!"); // get the bytes to response
                    //inv.settxt(txtServer, txtServer.Text + "<=" + s[1] + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                    reply.comment = s[1];
                    //context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                }
                else
                {

                }
                //context.Response.KeepAlive = false; // set the KeepAlive bool to false
                //context.Response.Close(); // close the connection
                //inv.settxt(txtServer, txtServer.Text + "Respone given to a request." + "\r\n");
                Thread.Sleep(10);
                reply.result = true;
                reply.contex = context;
                return reply;


            }

            catch (TaskCanceledException ex) { _httpListener.Stop(); inv.settxt(txtServer, txtServer.Text + "Cancel HTTP server." + "\r\n"); return reply; }
            catch (Exception ex) { _httpListener.Stop(); inv.settxt(txtServer, txtServer.Text + ex.Message + "\r\n"); return reply; }



        }
        public RobotFunctions.HostReply SendHttp(HttpListenerContext context, string mess)
        {
            RobotFunctions.HostReply reply = new RobotFunctions.HostReply();
            reply.result = false;
            reply.comment = "";
            try
            {
                if (cancelToken != null) cancelToken.Dispose();
                cancelToken = new CancellationTokenSource();
                CancellationToken cancel = cancelToken.Token;



                //HttpListenerContext context = await Task.Run(() => _httpListener.GetContext(), cancel);


                //inv.settxt(txtServer, txtServer.Text + "Url=>" + context.Request.Url + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");

                // Now, you'll find the request URL in context.Request.Url
                //byte[] _responseArray = Encoding.UTF8.GetBytes("<html><head><title>Localhost server -- port 5000</title></head>" +
                //"<body>Welcome to the <strong>Localhost server</strong> -- <em>port 5000!</em></body></html>"); // get the bytes to response

                byte[] _responseArray = Encoding.UTF8.GetBytes(mess); // get the bytes to response
                inv.settxt(txtServer, txtServer.Text + "<=" + mess + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                reply.comment = mess;
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                context.Response.OutputStream.Flush();

                context.Response.KeepAlive = false; // set the KeepAlive bool to false
                context.Response.Close(); // close the connection
                                          //inv.settxt(txtServer, txtServer.Text + "Respone given to a request." + "\r\n");
                                          //_httpListener.Stop();
                reply.result = true;
                return reply;

            }

            catch (TaskCanceledException ex) { _httpListener.Stop(); inv.settxt(txtServer, txtServer.Text + "Cancel HTTP server." + "\r\n"); return reply; }
            catch (Exception ex) { _httpListener.Stop(); inv.settxt(txtServer, txtServer.Text + ex.Message + "\r\n"); return reply; }



        }
        public bool HttpServer()
        {
            RobotFunctions.HostReply reply1 = new RobotFunctions.HostReply();
            RobotFunctions.HostReply reply2 = new RobotFunctions.HostReply();
            try
            {
                inv.settxt(txtServer, txtServer.Text + "Http Server Start" + "\r\n");

                _httpListener.Prefixes.Add("http://127.0.0.1:5000/"); // add prefix "http://localhost:5000/"
                _httpListener.Start(); // start server (Run application as Administrator!)
                while (!bStop)
                {
                    reply1 = ReadHttp();

                    SendHttp(reply1.contex, reply1.comment + "!!!");

                    Thread.Sleep(1);
                    if (txtServer.Text.Length > 2000) inv.settxt(txtServer, "");
                    if (txtClient.Text.Length > 2000) inv.settxt(txtClient, "");

                }
                inv.settxt(txtServer, txtServer.Text + "Http Server Stopped" + "\r\n");
                _httpListener.Stop();
                return true;
            }
            catch (Exception ex) { return false; }
        }
        #endregion client-server//--------------------------------------web client-server--------------------------------------------------------------------

        private async void btnRun_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                ControlsEnable(false);
                var task1 = Task.Run(() => RunProgram("TP_MAIN"));
                await task1;
                reply = task1.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR READ INFO TASK!");
                    ControlsEnable(true);
                    btnRobotStart.Enabled = true;
                    return;
                }
                ControlsEnable(true);
                return;
            }
            catch (Exception ex) { }
        }

        private async void btnAbort_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {

                var task1 = Task.Run(() => AbortProgram("*ALL*"));
                await task1;
                reply = task1.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR READ INFO TASK!");
                    ControlsEnable(true);
                    btnRobotStart.Enabled = true;
                    return;
                }
                return;
            }
            catch (Exception ex) { }
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {

                var task1 = Task.Run(() => ResetProgram());
                await task1;
                reply = task1.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR RESET TASK!");
                    ControlsEnable(true);
                    btnRobotStart.Enabled = true;
                    return;
                }
                return;
            }
            catch (Exception ex) { }
        }

        private void ArrayDI3_Control_Click(int index)
        {
            MyStatic.SetOut = index;
            MyStatic.SetCard = 2;
        }

        private async void btnReadBeckhoffIO_Click(object sender, EventArgs e)
        {
            try
            {
                MyStatic.ReadIOcont = true;
                btnReadBeckhoffIO.Enabled = false;
                var task1 = Task.Run(() => RunGenIO());

                await task1;
                CommReply reply = new CommReply();
                reply = task1.Result;
                if (!reply.result) MessageBox.Show("ERROR READ IO");

                btnReadBeckhoffIO.Enabled = true;
            }
            catch (Exception ex) { }
        }

        private void btnStopReadIO_Click(object sender, EventArgs e)
        {
            MyStatic.ReadIOcont = false;
            btnReadBeckhoffIO.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.LoadIniIO(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\" + "PlcIO.ini");
        }
        Single Speed = 0;
        private async void btnPlus_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkFine.Checked)
                {
                    //ControlsEnable(false);
                    if (cmbAxes.Text == "") return;
                    if (trackBarSpeedSt.Value > 10) trackBarSpeedSt.Value = 10;
                    Single dist = 0;
                    inv.set(btnPlus, "Enabled", false);
                    string[] s = cmbAxes.Text.Split(':');
                    Axis = int.Parse(s[0]);
                    int axis = Axis;
                    Speed = AxStatus[axis - 1].Vmax;

                    Single speed = 0.2f * Speed * Single.Parse(txtSpeedSt.Text) / 100;

                    dist = Single.Parse(cmbFine.Text);


                    var task1 = Task.Run(() => MoveRelSt(axis, dist, speed));
                    await task1;


                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                    if (!(reply.status == "" || reply.status == null))
                    {
                        MessageBox.Show("ERROR MOVE FINE! " + "\r" + reply.status);
                        //ControlsEnable(true);
                        inv.set(btnPlus, "Enabled", true);
                        return;
                    }
                    if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return; };

                    //ControlsEnable(true);
                    inv.set(btnPlus, "Enabled", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR MOVE FINE! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                     MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //ControlsEnable(true);
                inv.set(btnPlus, "Enabled", true);
            }
        }

        private async void btnPlus_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (chkFine.Checked) return;
                ControlsEnable(true);
                AxisMove = false;
                int device = 0;
                int axis = 0;

                var task1 = Task.Run(() => StopStations_Jog(device, axis));
                await task1;
                Thread.Sleep(100);

            }
            catch (Exception ex) { }
        }

        private async void btnPlus_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (chkFine.Checked) return;
                if (cmbAxes.Text == "") return;
                Thread.Sleep(100);
                AxisMove = true;
                int device = 1;
                int direction = 1;
                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;
                Speed = AxStatus[axis - 1].Vmax;
                Single speed = 0;
                if (axis == 5)
                {
                     speed =  Speed * Single.Parse(txtSpeedSt.Text) / 100;
                    if (speed > Speed / 5.0f) speed = Speed / 5.0f;
                }
                else
                {
                     speed = 0.2f * Speed * Single.Parse(txtSpeedSt.Text) / 100;
                }

                var task1 = Task.Run(() => RunStations_Jog(device, axis, direction, speed));
                //await task1;
                //CommReply reply = new CommReply();
                //reply.result = false;
                //reply = task1.Result;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;


            }
        }

        private async void btnPlus_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                //stop
                if (chkFine.Checked) return;
                if (AxisMove)// || true)
                {
                    ControlsEnable(true);
                    AxisMove = false;
                    int device = 0;
                    int axis = 0;

                    var task1 = Task.Run(() => StopStations_Jog(device, axis));
                    await task1;
                    Thread.Sleep(100);
                    //var task2 = Task.Run(() => ReadCurrent());
                    //await task2;
                }
            }
            catch (Exception ex) { }
        }

        private async void btnMin_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkFine.Checked)
                {
                    if (cmbAxes.Text == "") return;
                    if (trackBarSpeedSt.Value > 10) trackBarSpeedSt.Value = 10;
                    Single dist = 0;
                    inv.set(btnMin, "Enabled", false);
                    string[] s = cmbAxes.Text.Split(':');
                    Axis = int.Parse(s[0]);
                    int axis = Axis;
                    Speed = AxStatus[axis - 1].Vmax;
                    Single speed = 0.2f * Speed * Single.Parse(txtSpeedSt.Text) / 100;

                    dist = Single.Parse(cmbFine.Text);


                    var task1 = Task.Run(() => MoveRelSt(axis, -dist, speed));
                    await task1;


                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                    if (!(reply.status == "" || reply.status == null))
                    {
                        MessageBox.Show("ERROR MOVE FINE! " + "\r" + reply.status);

                        inv.set(btnPlus, "Enabled", true);
                        return;
                    }
                    if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return; };

                    //ControlsEnable(true);
                    inv.set(btnMin, "Enabled", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR MOVE FINE! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                     MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                ControlsEnable(true);
                inv.set(btnPlus, "Enabled", true);
            }
        }

        private async void btnMin_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (chkFine.Checked)
                {
                    ControlsEnable(false);
                    if (trackBarSpeedSt.Value > 10) trackBarSpeedSt.Value = 10;
                    Single dist = 0;
                    inv.set(btnMin, "Enabled", false);
                    string[] s = cmbAxes.Text.Split(':');
                    Axis = int.Parse(s[0]);
                    int axis = Axis;
                    Speed = 1000;
                    Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                    dist = -Single.Parse(cmbFine.Text);


                    var task1 = Task.Run(() => MoveRelSt(axis, dist, speed));
                    await task1;


                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                    if (!(reply.status == "" || reply.status == null))
                    {
                        MessageBox.Show("ERROR MOVE FINE! " + "\r" + reply.status);
                        ControlsEnable(true);
                        btnMin.Enabled = true;
                        return;
                    }
                    if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return; };
                    //snap
                    int cam = 0;
                    int cam1 = 0, cam2 = 0, cam3 = 0, cam4 = 0, cam5 = 0, campick = 0, camplace = 0;
                    if (Axis == MyStatic.StationAxis.ROT_ST1) { cam = 1; }
                    else if (Axis == MyStatic.StationAxis.X_ST1) { cam = 1; }
                    else if (Axis == MyStatic.StationAxis.FZ_ST2) { cam = 1; }
                    else if (Axis == MyStatic.StationAxis.LZ_ST2) { cam = 1; }
                    else if (Axis == MyStatic.StationAxis.FX_ST3) { cam = 2; }
                    if (cam != 0)
                    {
                        int next = 0;
                        //var task3 = Task.Run(() => GetCameraCoord(cam, next, 0, 0, 0, 0, 0, 0, 1, (int)MyStatic.VisionCmd.snap, 1,(int)MyStatic.Vision.locator));
                        //await task3;
                        //visresult = task3.Result;
                        //btnTstStation_Click(null, null);
                        //int vistype = (int)MyStatic.Vision.locator;
                        //var task2 = Task.Run(() =>  GetCameraCoord(cam, next, 0, 0, 0, 0, 0, 0, 1, (int)MyStatic.VisionCmd.snap, 1, (int)MyStatic.Vision.locator));
                        //await task2;
                        //CommReply visreply = task2.Result;
                        //var task2 = Task.Run(() => TstStation(false));
                        //await task2;
                        //bool b = task2.Result;
                        Thread.Sleep(500);
                    }
                    ControlsEnable(true);
                    inv.set(btnMin, "Enabled", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR MOVE FINE! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                     MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                ControlsEnable(true);
                inv.set(btnMin, "Enabled", true);
            }
        }

        private async void btnMin_MouseUp(object sender, MouseEventArgs e)
        {
            //stop
            if (chkFine.Checked) return;
            ControlsEnable(true);
            AxisMove = false;
            int device = 0;
            int axis = 0;

            var task1 = Task.Run(() => StopStations_Jog(device, axis));
            await task1;
            Thread.Sleep(100);
        }

        private async void btnMin_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (chkFine.Checked) return;
                if (cmbAxes.Text == "") return;
                Thread.Sleep(100);
                AxisMove = true;
                int device = 1;
                int direction = -1;
                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;
                Speed = AxStatus[axis - 1].Vmax;
                Single speed = 0;
                if (axis == 5)
                {
                    speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;
                    if (speed > Speed / 5.0f) speed = Speed / 5.0f;
                }
                else
                {
                    speed = 0.2f * Speed * Single.Parse(txtSpeedSt.Text) / 100;
                }

                var task1 = Task.Run(() => RunStations_Jog(device, axis, direction, speed));
                //await task1;
                //CommReply reply = new CommReply();
                //reply.result = false;
                //reply = task1.Result;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;


            }
        }

        private async void btnMin_MouseLeave(object sender, EventArgs e)
        {
            //stop
            if (chkFine.Checked) return;
            if (AxisMove)// || true)
            {
                ControlsEnable(true);
                AxisMove = false;
                int device = 0;
                int axis = 0;

                var task1 = Task.Run(() => StopStations_Jog(device, axis));
                await task1;
                Thread.Sleep(100);
            }
        }
        private CommReply RunStations_Jog(int device, Single ax, Single dir, Single speed)//global power on
        {

            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                //move jog

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveVel;//move vel
                //ParmsPlc.SendParm[1] = device;//move vel
                ParmsPlc.SendParm[1] = ax;//x=1 y=2 z=3
                ParmsPlc.SendParm[2] = dir;//0=negative 1=positive
                ParmsPlc.SendParm[3] = speed;// * 36;//speed
                ParmsPlc.SendParm[10] = 0.5f;//tmout
                while (AxisMove)
                {

                    reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, false);

                    if (!AxisMove)
                    {
                        break;
                    }
                    Thread.Sleep(100);

                }
                ParmsPlc.SendParm = null;
                device = 0;
                int axis = 0;
                StopStations_Jog(device, axis);
                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        private CommReply Run_Jog(int device, Single ax, Single dir, Single speed, int mode = 0, Single pos = 0, int cont = 0)
        {

            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;
                //move jog

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveJog;//move vel
                //ParmsPlc.SendParm[1] = device;//move vel
                ParmsPlc.SendParm[1] = ax;//x=1 y=2 z=3
                if (dir == 0)
                {
                    ParmsPlc.SendParm[2] = 0;//1=negative 2=positive 0=stop
                    ParmsPlc.SendParm[3] = 0;
                }
                else if (dir == 1)
                {
                    ParmsPlc.SendParm[2] = 0;//1=negative 2=positive 0=stop
                    ParmsPlc.SendParm[3] = 1;
                }
                else if (dir == 2)
                {
                    ParmsPlc.SendParm[2] = 1;//1=negative 2=positive 0=stop
                    ParmsPlc.SendParm[3] = 0;
                }
                else
                {
                    ParmsPlc.SendParm[2] = 0;//1=negative 2=positive 0=stop
                    ParmsPlc.SendParm[3] = 0;
                }
                ParmsPlc.SendParm[4] = speed;//speed
                ParmsPlc.SendParm[5] = mode;
                ParmsPlc.SendParm[6] = pos;
                ParmsPlc.SendParm[7] = cont;
                ParmsPlc.SendParm[10] = 1.5f;//tmout
                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                inv.set(upDownControlPosSt, "UpDownValue", reply.data[4]);
                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        private CommReply MoveRelSt(int station, Single dist, Single speed)
        {
            CommReply reply = new CommReply();
            try
            {

                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveRel;
                ParmsPlc.SendParm[1] = station;
                ParmsPlc.SendParm[2] = dist;
                ParmsPlc.SendParm[3] = speed;


                ParmsPlc.SendParm[10] = 5f;//tmout



                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc);

                ParmsPlc.SendParm = null;

                inv.set(upDownControlPosSt, "UpDownValue", reply.data[4]);
                return reply;
            }
            catch (Exception ex)
            {

                reply.result = false;
                reply.comment = ex.Message;
                return reply;
            }
        }

        private void trackBarSpeedSt_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarSpeedSt.Value != 0) txtSpeedSt.Text = Math.Abs(trackBarSpeedSt.Value).ToString();
        }

        private void btnResetRobotComm_Click(object sender, EventArgs e)
        {

            FW1.StopComm();
        }

        private async void chkLight1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Lamps;
                if (chkLight1.Checked) ParmsPlc.SendParm[2] = 1; else ParmsPlc.SendParm[2] = 2;
                if (chkLight2.Checked) ParmsPlc.SendParm[3] = 1; else ParmsPlc.SendParm[3] = 2;
                if (chkLight3.Checked) ParmsPlc.SendParm[4] = 1; else ParmsPlc.SendParm[4] = 2;

                ParmsPlc.SendParm[10] = 15f;//tmout
                var task1 = Task.Run(() => Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc));
                await task1;
                ParmsPlc.SendParm = null;
                reply = task1.Result;

            }
            catch (Exception ex) { }
        }

        private async void btnInspectStart_Click(object sender, EventArgs e)
        {
            try
            {
                //if (chkVisionSim.Checked) return;
                inv.set(btnRead, "Enabled", false);
                inv.set(btnInspectStart, "Enabled", false);
                //WC1.SetControls1(txtClient, this, null, "Vision Start Inspect", CameraAddr);
                //
                int axis = 0;
                if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                {
                    MessageBox.Show("ERROR PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.set(btnInspectStart, "Enabled", true);
                    return;
                }
                inv.settxt(txtClient, txtClient.Text + "<=" + "MoveFooterWork" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                Single Pos5 = master.Ax5_Diam + upDwnFootDX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);

                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                if (!task.Result.result)
                {
                    //SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("FOOTER WORK ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.set(btnInspectStart, "Enabled", true);
                    return;
                }
                inv.settxt(txtClient, txtClient.Text + "=>" + "Fini MoveFooterWork" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                Thread.Sleep(20);
                //
                inv.settxt(txtClient, txtClient.Text + "<=" + "StartCycleInspectVision" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                bool withcognex = true;
                var task1 = Task.Run(() => StartCycleInspectVision(withcognex));
                await task1;
                WebComm.CommReply reply = task1.Result;
                inv.settxt(txtClient, txtClient.Text + "=>" + "Fini StartCycleInspectVision" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                if (reply.result)
                {
                    if (chkSuaSim.Checked)
                    {
                        inv.set(btnRead, "Enabled", true);
                        inv.set(btnInspectStart, "Enabled", true);
                        //if (bStop) break;
                        Thread.Sleep(2);
                        return;
                    }
                    //==========wait sua fini InspectStationAct.State=InspectStationAct.SuaState================
                    Thread.Sleep(100);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("SUA<= WAIT Sua Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    Stopwatch sw = new Stopwatch();
                    sw.Restart();
                    //run wait sua fini StartWaitInspectCognex()
                    var task11 = Task.Run(() => StartWaitInspectCognex());
                    await task11;
                    WebComm.CommReply fini1 = task11.Result;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("SUA=> Sua Finished" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (!fini1.result)
                    {
                        InspectStationAct.SuaState = (int)MyStatic.E_State.RejectSuaTop;
                        string status = fini1.status;
                    }
                    else
                    {
                        string status = fini1.status;
                        InspectStationAct.SuaState = (int)MyStatic.E_State.SuaFini;


                    }
                    //WC2.SetControls1(txtClient, this, null, "Cognex Start Inspect", CognexAddr);
                    //inv.settxt(txtClient, txtClient.Text + "<=" + "StartCycleInspectCognex" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                    //var task2 = Task.Run(() => StartCycleInspectCognex());
                    //await task2;
                    //reply = task1.Result;
                    //inv.settxt(txtClient, txtClient.Text + "=>" + "fini StartCycleInspectCognex" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")" + "\r\n");
                }
                inv.set(btnRead, "Enabled", true);
                inv.set(btnInspectStart, "Enabled", true);
                //if (bStop) break;
                Thread.Sleep(2);
                //}
            }
            catch (Exception ex) { inv.set(btnInspectStart, "Enabled", true); }
        }

        private void btnInspectStop_Click(object sender, EventArgs e)
        {

        }

        private void txtAutoLog_DoubleClick(object sender, EventArgs e)
        {

            WriteToFile(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Log\\Mainlog " + DateTime.Now.ToString("yyyy-MM-dd HH") + ".ini", txtAutoLog.Text);

            inv.settxt(txtAutoLog, "");
        }
        private void WriteToFile(string filebackup, string txt)
        {
            try
            {
                if (File.Exists(filebackup))
                {

                    FileStream aFile = new FileStream(filebackup, FileMode.Append, FileAccess.Write);
                    StreamWriter sw1 = new StreamWriter(aFile);
                    sw1.WriteLine(txt);
                    //for (int i = 0; i < txt.Length; i++) sw1.WriteLine(txt[i]);
                    sw1.Close();
                }
                else
                {
                    FileStream aFile = new FileStream(filebackup, FileMode.Create, FileAccess.Write);
                    StreamWriter sw1 = new StreamWriter(aFile);
                    sw1.WriteLine(txt);
                    //for (int i = 0; i < txt.Length; i++) sw1.WriteLine(txt[i]);
                    sw1.Close();
                }

            }
            catch (Exception err)
            {


            }
        }

        private async void panelTrayRej_MouseDown(object sender, MouseEventArgs e)
        {
            int pallet = 0;
            int i = 0;

            //NoPlace = new int[100];
            string[] arr;
            arr = new string[100];
            List<string> lst;
            lst = new List<string>();
            //Single MarkingArrayW = 306;
            //Single MarkingArrayH = 240;
            int rejectpartId = 0;


            try
            {

                Single TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
                Single TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
                Single scale1 = this.panelTrayRej.Width / (float)(TrayInsertsOnX * TrayDeltaX);
                Single scale2 = this.panelTrayRej.Height / (float)(TrayInsertsOnY * TrayDeltaY);


                //######### korea1 ################
                for (int j = 0; j < pntsrot.Length; j++)
                {

                    if (((scale1 * pntsrot[j].X / 10 - scale1 * TrayDeltaX) <= e.X) && ((scale1 * pntsrot[j].X / 10 + scale1 * TrayDeltaX) >= e.X)
                       && ((scale2 * pntsrot[j].Y / 10 - scale2 * TrayDeltaY) <= e.Y) && ((scale2 * pntsrot[j].Y / 10 + scale2 * TrayDeltaY) >= e.Y))
                    {
                        TrayPartIdRej = j;// TrayInsertsOnY * TrayInsertsOnX - i - 1;
                        txtPartIDRej.Text = TrayPartIdRej.ToString();
                        bool corr = false;

                        var task02 = Task.Run(() => GetRejectTray(corr));
                        await task02;
                        RobotFunctions.CommReply posrep2 = task02.Result;
                        //position basepos = new position();
                        if (!posrep2.result)
                        {
                            MyStatic.bExitcycle = true;
                            MessageBox.Show("ERROR3 Reject Pos COORDINATES!", "ERROR", MessageBoxButtons.OK,
                                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return;
                        }
                        else
                        {
                            Single x = posrep2.data[0];
                            Single y = posrep2.data[1];
                            Single z = posrep2.data[2];
                            Single r = posrep2.data[3];


                            txtPartIDRej.Text = TrayPartIdRej.ToString();
                            rejectpartId = i;

                            //find row in grid
                            //int rejecttray = int.Parse(txtTrayID.Text);
                            String searchValue = (rejectpartId).ToString();
                            int rowIndex = -1;
                            foreach (DataGridViewRow row in dataGridViewReject.Rows)
                            {
                                if (row.Cells[0].Value.ToString().Trim().Equals(searchValue))
                                {
                                    rowIndex = row.Index;
                                    break;
                                }
                            }
                            //foreach (DataGridViewRow row in dataGridViewReject.Rows)
                            //{
                            //    dataGridViewReject.Rows[row.Index].Selected = false;
                            //}
                            //dataGridViewReject.ClearSelection();
                            if (rowIndex > -1)
                            {
                                dataGridViewReject.Rows[rowIndex].Selected = true;
                                dataGridViewReject.FirstDisplayedScrollingRowIndex = rowIndex;
                                dataGridViewReject.Focus();
                            }

                        }
                        return;

                    }
                    //i++;

                }
            }
            catch (Exception ex)
            { }
        }

        private void panelTrayRej_Paint(object sender, PaintEventArgs e)
        {
            try
            {

                position outpos = new position();
                outpos.Error = "";
                //NoPlace();
                int partindex = TrayPartIdRej;
                Panel panel = panelTrayRej;
                DrawTrayOut(panel, ref partindex, ref outpos);
                TrayPartIdRej = partindex;
            }
            catch (Exception ex) { }
        }
        private RobotFunctions.CommReply GetRejectTray(bool corr = false)
        {

            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();
            Array.Resize<Single>(ref commreply.data, 4);
            TrayPartIdRej = int.Parse(txtPartIDRej.Text);
            // OrderPartId = int.Parse(txtOrderPartId.Text);
            Single x = 0;
            Single y = 0;

            //if (MyStatic.chkDebug)
            //{
            //    Array.Resize<Cell>(ref TrayOutCell2, TrayInsertsOnY * TrayInsertsOnX);

            //}

            position outpos = new position();
            outpos.Error = "";


            if (TrayPartIdRej >= TrayOutCell2.Length - 1)
            {
                commreply.comment = "last part";
            }
            if (TrayPartIdRej >= TrayOutCell2.Length) TrayPartIdRej = TrayOutCell2.Length - 1;
            int partindex = TrayPartIdRej;
            Panel panel = panelTrayRej;
            DrawTrayOut(panel, ref partindex, ref outpos, 0, 0);
            int cntPartsLeft = 0;
            if (partindex + 1 < TrayOutCell2.Length)//how match parts left to pick
            {
                for (int i = partindex + 1; i < TrayOutCell2.Length; i++)
                {
                    if (TrayOutCell2[i].mark != null && TrayOutCell2[i].mark == 1) cntPartsLeft++;

                }
                if (cntPartsLeft == 0)
                    commreply.comment = "last part";
            }
            x = 0; y = 0;
            commreply.result = true;

            Single dz = ZdiamoffsetPickTray;

            commreply.data[0] = (Single)(RobotLoadPoints.PlaceRejectOrg.x + outpos.x) + TrayOffsetX;
            commreply.data[1] = (Single)(RobotLoadPoints.PlaceRejectOrg.y + outpos.y) + TrayOffsetY;
            commreply.data[2] = (Single)(RobotLoadPoints.PlaceRejectOrg.z + dz) + TrayOffsetZ;
            commreply.data[3] = (Single)(RobotLoadPoints.PlaceRejectOrg.r);




            inv.settxt(lblTrayX, commreply.data[0].ToString("0.0"));
            inv.settxt(lblTrayY, commreply.data[1].ToString("0.0"));
            inv.settxt(lblTrayZ, commreply.data[2].ToString("0.0"));
            inv.settxt(lblTrayR, commreply.data[3].ToString("0.0"));


            commreply.data[0] = (Single)(commreply.data[0] + dxY * outpos.y + Xscale * outpos.x);
            commreply.data[1] = (Single)(commreply.data[1] + dyX * outpos.x + Yscale * outpos.y);
            commreply.data[2] = (Single)(commreply.data[2] + dzX * outpos.x + dzY * outpos.y);
            commreply.data[3] = (Single)(RobotLoadPoints.PlaceRejectOrg.r);


            if (commreply.data[0] == 0 && commreply.data[1] == 0) commreply.result = false;
            return commreply;
        }

        private void txtOrder_DoubleClick(object sender, EventArgs e)
        {
            txtOrder.Text = "";

            txtItem.Text = "";
            txtPartDiam.Text = "";
            txtPartDiamd.Text = "";
            txtPartWeight.Text = "";

            txtPartLength.Text = "";
            txtComment.Text = "";
        }

        string OrderTest = "";
        private void txtOrder_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int station2 = 0;
                //if (!int.TryParse(txtOrderStation.Text, out station2))
                //{

                //    txtOrder.Text = "";
                //    return;
                //}

                if (txtOrder.Text.Length > 7)
                {
                    txtOrder.Text = OrderTest;
                    Thread.Sleep(100);
                    return;
                }
                if (txtOrder.Text.Length == 7)
                {
                    OrderTest = txtOrder.Text;
                }
                return;

            }
            catch { }
        }

        private async void txtOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool noitem = false;
            try
            {

                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (txtOrder.Text.Length == 7)
                    {

                        var task1 = Task.Run(() => ReadAS400());
                        await task1;
                        bool b = task1.Result;

                        if (!b)
                        {
                            DialogResult res1 = MessageBox.Show("ERROR CONNECTION TO AS400! Use Local Copy ?", "Warning", MessageBoxButtons.OKCancel,
                           MessageBoxIcon.Warning);

                            if (res1 == DialogResult.OK) { }
                            else return;
                        }

                        //if file exist
                        if (txtItem.Text == "")
                        {
                            MessageBox.Show("ENTER ITEM NUMBER!", "Warning", MessageBoxButtons.YesNo,
                          MessageBoxIcon.Warning);
                            return;
                        }
                        string filename = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Items\\" + txtItem.Text.Trim() + ".ini";
                        if (File.Exists(filename))
                        {
                            DialogResult res1 = MessageBox.Show("ITEM EXIST. Load Item ?", "Warning", MessageBoxButtons.YesNo,
                               MessageBoxIcon.Warning);
                            if (res1 == DialogResult.Yes)
                            {
                                int station1 = 0;

                                if (!LoadItemIniBarcode(txtItem.Text.Trim())) noitem = true; else noitem = false;

                                //load laser data
                                decimal partdiam = decimal.Parse(txtPartDiamd.Text);
                                LoadDiamData((int)partdiam, noitem);


                                //check models

                                // var task01 = Task.Run(() => VisionCheckModels());
                                // await task01;
                                // RobotFunctions.CommReply visresult = task01.Result;
                                // if (!visresult.result)
                                // {
                                //     MyStatic.bExitcycle = true;
                                //     MessageBox.Show(visresult.comment, "ERROR", MessageBoxButtons.OK,
                                //MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                // }

                                //

                            }
                        }
                        else
                        {

                            MessageBox.Show("ITEM NOT EXIST!");
                        }
                        //send data to vision

                        //WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                        var task2 = Task.Run(() => DataToVision());
                        await task2;
                        WebComm.CommReply reply = task2.Result;
                        if (!reply.result)
                        {
                            MessageBox.Show("ERROR SEND DATA TO VISION!");
                        }
                        var task3 = Task.Run(() => DataToFront());
                        await task3;
                        reply = task3.Result;
                        if (!reply.result)
                        {
                            MessageBox.Show("ERROR SEND DATA TO FRONT!");
                        }

                    }
                }
            }
            catch { }

        }
        private bool LoadItemIniBarcode(string file)
        {


            try
            {

                string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Items\\" + file + ".ini";

                string[][] arrnew = new string[1][];
                arrnew[0] = new string[0];
                //create vars array
                string mess = "";
                if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ INI FILE"); return false; }
                //create vars array
                //robot load

                UpDwnX3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrX", arrnew));
                UpDwnY3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrY", arrnew));
                UpDwnZ3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrZ", arrnew));
                UpDwnR3.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Pick CorrR", arrnew));
                UpDwnX4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrX", arrnew));
                UpDwnY4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrY", arrnew));
                UpDwnZ4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrZ", arrnew));
                UpDwnR4.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceInsp CorrR", arrnew));
                UpDwnX5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrX", arrnew));
                UpDwnY5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrY", arrnew));
                UpDwnZ5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrZ", arrnew));
                UpDwnR5.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PickInsp CorrR", arrnew));
                UpDwnX6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrX", arrnew));
                UpDwnY6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrY", arrnew));
                UpDwnZ6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrZ", arrnew));
                UpDwnR6.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "Place CorrR", arrnew));
                UpDwnX7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrX", arrnew));
                UpDwnY7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrY", arrnew));
                UpDwnZ7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrZ", arrnew));
                UpDwnR7.UpDownValue = Single.Parse(IniData.GetKeyValueArrINI("RobotLoad", "PlaceRej CorrR", arrnew));



                RobotLoadPoints.PickTrayOrg.corrX = (Single)UpDwnX3.UpDownValue;
                RobotLoadPoints.PickTrayOrg.corrY = (Single)UpDwnY3.UpDownValue;
                RobotLoadPoints.PickTrayOrg.corrZ = (Single)UpDwnZ3.UpDownValue;
                RobotLoadPoints.PickTrayOrg.corrR = (Single)UpDwnR3.UpDownValue;

                RobotLoadPoints.PlaceInspect.corrX = (Single)UpDwnX4.UpDownValue;
                RobotLoadPoints.PlaceInspect.corrY = (Single)UpDwnY4.UpDownValue;
                RobotLoadPoints.PlaceInspect.corrZ = (Single)UpDwnZ4.UpDownValue;
                RobotLoadPoints.PlaceInspect.corrR = (Single)UpDwnR4.UpDownValue;

                RobotLoadPoints.PickInspect.corrX = (Single)UpDwnX5.UpDownValue;
                RobotLoadPoints.PickInspect.corrY = (Single)UpDwnY5.UpDownValue;
                RobotLoadPoints.PickInspect.corrZ = (Single)UpDwnZ5.UpDownValue;
                RobotLoadPoints.PickInspect.corrR = (Single)UpDwnR5.UpDownValue;

                RobotLoadPoints.PlaceTrayOrg.corrX = (Single)UpDwnX6.UpDownValue;
                RobotLoadPoints.PlaceTrayOrg.corrY = (Single)UpDwnY6.UpDownValue;
                RobotLoadPoints.PlaceTrayOrg.corrZ = (Single)UpDwnZ6.UpDownValue;
                RobotLoadPoints.PlaceTrayOrg.corrR = (Single)UpDwnR6.UpDownValue;

                RobotLoadPoints.PlaceRejectOrg.corrX = (Single)UpDwnX7.UpDownValue;
                RobotLoadPoints.PlaceRejectOrg.corrY = (Single)UpDwnY7.UpDownValue;
                RobotLoadPoints.PlaceRejectOrg.corrZ = (Single)UpDwnZ7.UpDownValue;
                RobotLoadPoints.PlaceRejectOrg.corrR = (Single)UpDwnR7.UpDownValue;


                Robot1data.Gripper = IniData.GetKeyValueArrINI("RobotLoad", "Gripper", arrnew);

                Thread.Sleep(500);
                txtPartDiam.Text = IniData.GetKeyValueArrINI("Item", "Diam", arrnew);
                txtPartDiamd.Text = IniData.GetKeyValueArrINI("Item", "d", arrnew);
                txtPartLength.Text = IniData.GetKeyValueArrINI("Item", "L", arrnew);
                txtPartLengthU.Text = IniData.GetKeyValueArrINI("Item", "LU", arrnew);
                txtPartWeight.Text = IniData.GetKeyValueArrINI("Item", "W", arrnew);
                cmbOrderTray.Text = IniData.GetKeyValueArrINI("Item", "Tray", arrnew);
                cmbTray.Text = IniData.GetKeyValueArrINI("Item", "Tray", arrnew);


                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_Home", arrnew), out axis_Parameters[0].Ax_Home)) mess = mess + "axis_Parameters[0].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_WorkTop", arrnew), out axis_Parameters[0].Ax_WorkTop)) mess = mess + "axis_Parameters[0].Ax_Work" + "\r\n";
                inv.set(upDwnCam2z, "UpDownValue", axis_Parameters[0].Ax_WorkTop);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_Home", arrnew), out axis_Parameters[1].Ax_Home)) mess = mess + "axis_Parameters[1].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_WorkTop", arrnew), out axis_Parameters[1].Ax_WorkTop)) mess = mess + "axis_Parameters[1].Ax_Work" + "\r\n";
                inv.set(upDwnLamp1Z, "UpDownValue", axis_Parameters[1].Ax_WorkTop);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_Home", arrnew), out axis_Parameters[2].Ax_Home)) mess = mess + "axis_Parameters[2].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_WorkFront", arrnew), out axis_Parameters[2].Ax_WorkFront)) mess = mess + "axis_Parameters[2].Ax_WorkFront" + "\r\n";
                inv.set(upDwnCam1x, "UpDownValue", axis_Parameters[2].Ax_WorkFront);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_Home", arrnew), out axis_Parameters[3].Ax_Home)) mess = mess + "axis_Parameters[3].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_WorkTop", arrnew), out axis_Parameters[3].Ax_WorkTop)) mess = mess + "axis_Parameters[3].Ax_Work" + "\r\n";
                inv.set(upDwnFootWorkR, "UpDownValue", axis_Parameters[3].Ax_Home);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Home", arrnew), out axis_Parameters[4].Ax_Home)) mess = mess + "axis_Parameters[4].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_WorkTop", arrnew), out axis_Parameters[4].Ax_WorkTop)) mess = mess + "axis_Parameters[4].Ax_Work" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Weldone", arrnew), out axis_Parameters[4].Ax_Weldone)) mess = mess + "axis_Parameters[4].Ax_Weldone" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Diam", arrnew), out axis_Parameters[4].Ax_Diameter)) mess = mess + "axis_Parameters[4].Ax_Diam" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_WorkFront", arrnew), out axis_Parameters[4].Ax_WorkFront)) mess = mess + "axis_Parameters[4].Ax_WorkFront" + "\r\n";
                inv.set(upDwnFootWorkTopX, "UpDownValue", axis_Parameters[4].Ax_WorkTop);
                inv.set(upDwnFootWeldX, "UpDownValue", axis_Parameters[4].Ax_Weldone);
                inv.set(upDwnFootDX, "UpDownValue", axis_Parameters[4].Ax_Diameter);
                inv.set(upDwnFootWorkFrontX, "UpDownValue", axis_Parameters[4].Ax_WorkFront);
                //txtPartDiam.Text = IniData.GetKeyValueArrINI("Ax5", "Ax_Diam", arrnew);
                return true;





            }

            catch (Exception err)
            { MessageBox.Show("ERROR READ INI FILE " + err); return false; }
        }

        public void ComposeException(Exception ex, [CallerMemberName] string sCallerName="")
        {
            MessageBox.Show($"Error in {sCallerName}: {ex.Message}");
        }

        public int ParseInt(string sValue)
        {
            try
            {
                int iValue = -1;
                if (int.TryParse(sValue, out iValue))
                {
                    return iValue;
                }
                else
                {
                    ComposeException(new Exception($"Error in ParseInt Value:{sValue}"));
                    return int.MaxValue;
                }
            }
            catch (Exception ex)
            {
                ComposeException(ex);
                return int.MaxValue;
            }
        }




private bool LoadItemData(string file)
        {


            try
            {

                string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Items\\" + file + ".ini";

                string[][] arrnew = new string[1][];
                arrnew[0] = new string[0];
                //create vars array
                string mess = "";
                if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ ITEM FILE"); return false; }
                //create vars array
                //robot load




                txtPartDiam.Text = IniData.GetKeyValueArrINI("Item", "Diam", arrnew);
                txtPartDiamd.Text = IniData.GetKeyValueArrINI("Item", "d", arrnew);
                txtPartLength.Text = IniData.GetKeyValueArrINI("Item", "L", arrnew);
                txtPartLengthU.Text = IniData.GetKeyValueArrINI("Item", "LU", arrnew);
                txtPartWeight.Text = IniData.GetKeyValueArrINI("Item", "W", arrnew);
                txtComment.Text = IniData.GetKeyValueArrINI("Item", "Comment", arrnew);
                cmbOrderTray.Text = IniData.GetKeyValueArrINI("Item", "Tray", arrnew);
                cmbTray.Text = IniData.GetKeyValueArrINI("Item", "Tray", arrnew);

                
                string supDwnCount = IniData.GetKeyValueArrINI("Item", "BladesNumber", arrnew);
                int iupDwnCount = ParseInt(supDwnCount);
                if (iupDwnCount != int.MaxValue)
                {
                    upDwnCount.UpDownValue = iupDwnCount;
                }
                else
                {
                    ComposeException(new Exception($"Error in LoadItemData BladesNumber Value: {supDwnCount} File: {file}"));
                }
                return true;
            }

            catch (Exception err)
            { MessageBox.Show("ERROR READ ITEM FILE " + err); return false; }
        }
        private bool ReadAS400()
        {
            try
            {
                if (txtOrder.Text.Length < 5)
                {
                    MessageBox.Show("ERROR ORDER NUMBER!");
                    return false;
                }
                RobotFunctions.CommReply reply = AS400.ReadHTTP(txtOrder.Text, 2000);

                //MessageBox.Show(reply.comment);
                if (reply.comment == null)
                {
                    MessageBox.Show("ERROR READ ORDER!");
                    return false;
                }
                string[] s = reply.comment.Split(',');
                if (s.Length < 2) return false;
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = s[i].Replace("{", string.Empty);
                    s[i] = s[i].Replace("[", string.Empty);
                    s[i] = s[i].Replace("]", string.Empty);
                    s[i] = s[i].Replace("}", string.Empty);
                    s[i] = s[i].Replace("\\", string.Empty);
                    s[i] = s[i].Replace("\"", string.Empty);
                    s[i] = s[i].Replace(('"').ToString(), string.Empty);
                }
                string stremark = "";
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].IndexOf("itemNumber:") > -1)
                    {
                        inv.settxt(txtItem, s[i].Substring(s[i].IndexOf(":") + 1, s[i].Length - s[i].IndexOf(":") - 1).Trim());

                    };
                    if ((s[i].Substring(s[i].IndexOf(":") + 1, s[i].Length - s[i].IndexOf(":") - 1).Trim()).Length > 0)
                    {
                        if (s[i].IndexOf("diameter:") > -1) inv.settxt(txtPartDiam, (Single.Parse(s[i].Substring(s[i].IndexOf(":") + 1, s[i].Length - s[i].IndexOf(":") - 1).Trim())).ToString("0.00"));
                    }
                    if (s[i].IndexOf("weightB:") > -1) inv.settxt(txtPartWeight, (Single.Parse(s[i].Substring(s[i].IndexOf(":") + 1, s[i].Length - s[i].IndexOf(":") - 1).Trim())).ToString("0.00"));
                    if (s[i].IndexOf("lentgh:") > -1) inv.settxt(txtPartLength, (Single.Parse(s[i].Substring(s[i].IndexOf(":") + 1, s[i].Length - s[i].IndexOf(":") - 1).Trim())).ToString("0.00"));
                    if (s[i].IndexOf("itemDescription:") > -1) inv.settxt(txtComment, s[i].Substring(s[i].IndexOf(":") + 1, s[i].Length - s[i].IndexOf(":") - 1).Trim());

                    if (s[i].IndexOf("remark:remark:") > -1)
                    {
                        stremark = s[i].Substring(s[i].IndexOf(":D") + 1, s[i].Length - s[i].IndexOf(":D") - 1).Trim();


                    }
                    else if (s[i].IndexOf("remark:") > -1)
                    {
                        stremark = s[i].Substring(s[i].IndexOf(":D") + 1, s[i].Length - s[i].IndexOf(":D") - 1).Trim();
                        string[] ss = s[i].Split(' ');
                        if (ss.Length >= 6)
                        {
                            inv.settxt(txtPartLengthU, ss[2].Substring(2, ss[2].Length - 2));
                        }


                    }

                }
                string[] sd = stremark.Split(' ');
                Single D = 0;
                Single d = 0;

                //inv.settxt(txtBarcode1,"http:/www.iscar.com/Q/?"+txtItem.Text+txtOrder.Text);

                for (int i = 0; i < sd.Length; i++)
                {
                    if (sd[i].IndexOf("D") > -1) D = Single.Parse(sd[i].Substring(sd[i].IndexOf("D") + 1, sd[i].Length - sd[i].IndexOf("D") - 1).Trim());
                    if (sd[i].IndexOf("d") > -1) d = Single.Parse(sd[i].Substring(sd[i].IndexOf("d") + 1, sd[i].Length - sd[i].IndexOf("d") - 1).Trim());
                }
                //inch
                if (D < 1) inv.settxt(txtPartDiam, (D * 25.4).ToString("0.00")); else inv.settxt(txtPartDiam, D.ToString());
                if (d < 1) inv.settxt(txtPartDiamd, (d * 25.4).ToString("0.00")); else inv.settxt(txtPartDiamd, d.ToString());

                int diam = (int)D;
                if (!LoadDiamData(diam, false)) return false;
                inv.settxt(txtWeldonAngle, weldone.angle.ToString("0.0"));
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        FanucWeb AS400 = new FanucWeb();

        private void cmbOrderTray_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbTray.Text = cmbOrderTray.Text;
            }
            catch (Exception ex) { }
        }
        public int lblStation_selected;
        //private void lblRobot1_Click(object sender, EventArgs e)
        //{

        //        if (((System.Windows.Forms.Label)sender).Name == "lblRobot") { lblStation_selected = 100; }
        //        else if (((System.Windows.Forms.Label)sender).Name == "lblGrip1") { lblStation_selected = 101; }
        //        else if (((System.Windows.Forms.Label)sender).Name == "lblGrip2") { lblStation_selected = 102; }
        //        else if (((System.Windows.Forms.Label)sender).Name == "lblInspect") { lblStation_selected = 103; }
        //        else if (((System.Windows.Forms.Label)sender).Name == "lblVision1") { lblStation_selected = 104; }
        //        else if (((System.Windows.Forms.Label)sender).Name == "lblVision2") { lblStation_selected = 105; }
        //        else if (((System.Windows.Forms.Label)sender).Name == "lblSua") { lblStation_selected = 106; }


        //    else lblStation_selected = -1;




        //}
        private async void lblSelect_Click(object sender, EventArgs e)
        {
            try
            {

                int i;

                //for (i = 0; i < lblSelect.Length; i++)
                //{
                //    if (lblSelect[i] == (Label)sender)
                //    {
                //        switch (i)
                //        {
                //            case 0://empty
                //                if (lblStation_selected == 100) { }//robot
                //                else if (lblStation_selected == 101) { RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.Empty; RobotLoadAct.OnGrip1_Ready = false; txtGrip1num.Text = ""; RobotLoadAct.OnGrip1_PartID = -1; }//grip1
                //                else if (lblStation_selected == 102) { RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Empty; RobotLoadAct.OnGrip2_Ready = false; txtGrip2num.Text = ""; RobotLoadAct.OnGrip2_PartID = -1; }//grip2
                //                else if (lblStation_selected == 103) { InspectStationAct.State = (int)MyStatic.E_State.Empty; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Empty; txtGrip3num.Text = ""; }//inspect
                //                else if (lblStation_selected == 104) { if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Empty; txtGrip3num.Text = ""; InspectStationAct.OnGrip3_PartID = -1; }//vis1
                //                else if (lblStation_selected == 105) { }//vis2
                //                else if (lblStation_selected == 106) { if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Empty; txtGrip3num.Text = ""; InspectStationAct.OnGrip3_PartID = -1; }//sua

                //                break;
                //            case 1:// occupied
                //                if (lblStation_selected == 100) { }
                //                else if (lblStation_selected == 101) { RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.Occupied; RobotLoadAct.OnGrip1_Ready = false; if (RobotLoadAct.OnGrip1_PartID >= 0) partData[RobotLoadAct.OnGrip1_PartID].State = (int)MyStatic.E_State.Occupied; }
                //                else if (lblStation_selected == 102) { RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Occupied; RobotLoadAct.OnGrip2_Ready = false; if (RobotLoadAct.OnGrip2_PartID >= 0) partData[RobotLoadAct.OnGrip2_PartID].State = (int)MyStatic.E_State.Occupied; }
                //                else if (lblStation_selected == 103) { InspectStationAct.State = (int)MyStatic.E_State.Occupied; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Occupied; }
                //                else if (lblStation_selected == 104) { InspectStationAct.State = (int)MyStatic.E_State.Occupied; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Occupied; }
                //                else if (lblStation_selected == 105) { InspectStationAct.State = (int)MyStatic.E_State.Occupied; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Occupied; }
                //                else if (lblStation_selected == 106) { InspectStationAct.State = (int)MyStatic.E_State.Occupied; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Occupied; }



                //                break;
                //            case 2:// ready
                //                if (lblStation_selected == 100) { }
                //                //else if (lblStation_selected == 101) {  RobotLoadAct.OnGrip1_Ready = true; RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.PartReady; }
                //                else if (lblStation_selected == 102) { RobotLoadAct.OnGrip2_Ready = true; RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.PartReady; if (RobotLoadAct.OnGrip2_PartID >= 0) partData[RobotLoadAct.OnGrip2_PartID].State = (int)MyStatic.E_State.SuaFini; }
                //                else if (lblStation_selected == 103) { if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.SuaFini; InspectStationAct.State = (int)MyStatic.E_State.PartReady; }
                //                else if (lblStation_selected == 104) { InspectStationAct.State = (int)MyStatic.E_State.PartReady; }
                //                else if (lblStation_selected == 105) { if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.SuaFini; }
                //                else if (lblStation_selected == 106) { InspectStationAct.State = (int)MyStatic.E_State.PartReady; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.SuaFini; }


                //                break;
                //            case 3://reject
                //                if (lblStation_selected == 100) { }//rob
                //                else if (lblStation_selected == 101) { }//grip1
                //                else if (lblStation_selected == 102) { RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Reject; if (RobotLoadAct.OnGrip2_PartID >= 0) partData[RobotLoadAct.OnGrip2_PartID].State = (int)MyStatic.E_State.Reject; }//grip2
                //                else if (lblStation_selected == 103) { InspectStationAct.State = (int)MyStatic.E_State.Reject; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.Reject; }//incpect
                //                else if (lblStation_selected == 104) { InspectStationAct.State = (int)MyStatic.E_State.Reject; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.RejectMeasure; }//vis1
                //                else if (lblStation_selected == 105) { InspectStationAct.State = (int)MyStatic.E_State.Reject; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.RejectMeasure; }//vis22
                //                else if (lblStation_selected == 106) { InspectStationAct.State = (int)MyStatic.E_State.Reject; if (InspectStationAct.OnGrip3_PartID >= 0) partData[InspectStationAct.OnGrip3_PartID].State = (int)MyStatic.E_State.RejectSua; }//sua

                //                break;
                //            case 4://disable
                //                break;
                //            default: break;



                //    }

                //    break;
                //}


                //}
                lblStation_selected = -1;
                RefreshLables();
            }
            catch (Exception ex) { }

        }
        private void RefreshLables()

        {
            try
            {


                if (RobotLoadAct.InAction) { if (lblRobot.BackColor != Color.Lime) inv.set(lblRobot, "BackColor", Color.Lime); }
                if (!RobotLoadAct.InAction) { if (lblRobot.BackColor != Color.White) inv.set(lblRobot, "BackColor", Color.White); }

                if (InspectStationAct.VisionInAction && !InspectStationAct.SuaInAction) { if (lblVision1.BackColor != Color.Blue) inv.set(lblVision1, "BackColor", Color.Blue); }
                else if (InspectStationAct.VisionInAction && InspectStationAct.SuaInAction) { if (lblVision1.BackColor != Color.Green) inv.set(lblVision1, "BackColor", Color.Green); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.VisionTop] == (int)MyStatic.E_State.RejectDiam) { if (lblVision1.BackColor != Color.Red) inv.set(lblVision1, "BackColor", Color.Red); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty) { if (lblVision1.BackColor != Color.White) inv.set(lblVision1, "BackColor", Color.White); }
                else { if (lblVision1.BackColor != Color.White) inv.set(lblVision1, "BackColor", Color.White); }


                if (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Empty) { if (lblGrip1.BackColor != Color.White) inv.set(lblGrip1, "BackColor", Color.White); }
                else if (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Occupied) { if (lblGrip1.BackColor != Color.Yellow) inv.set(lblGrip1, "BackColor", Color.Yellow); }
                else if (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.PartReady) { if (lblGrip1.BackColor != Color.LightGreen) inv.set(lblGrip1, "BackColor", Color.LightGreen); }
                else if (RobotLoadAct.OnGrip1_State == (int)MyStatic.E_State.Reject) { if (lblGrip1.BackColor != Color.Red) inv.set(lblGrip1, "BackColor", Color.Red); }

                if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Empty) { if (lblGrip2.BackColor != Color.White) inv.set(lblGrip2, "BackColor", Color.White); }
                else if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Occupied) { if (lblGrip2.BackColor != Color.Yellow) inv.set(lblGrip2, "BackColor", Color.Yellow); }
                else if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.PartReady) { if (lblGrip2.BackColor != Color.Yellow) inv.set(lblGrip2, "BackColor", Color.Yellow); }
                else if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.SuaFini) { if (lblGrip2.BackColor != Color.LightGreen) inv.set(lblGrip2, "BackColor", Color.LightGreen); }
                else if (RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.Reject || RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.RejectMeasure || RobotLoadAct.OnGrip2_State == (int)MyStatic.E_State.RejectSuaTop) { if (lblGrip2.BackColor != Color.Red) inv.set(lblGrip2, "BackColor", Color.Red); }

                if (InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Empty) { if (lblInspect.BackColor != Color.White) inv.set(lblInspect, "BackColor", Color.White); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.Occupied) { if (lblInspect.BackColor != Color.Yellow) inv.set(lblInspect, "BackColor", Color.Yellow); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.Footer] == (int)MyStatic.E_State.PartReady) { if (lblInspect.BackColor != Color.LightGreen) inv.set(lblInspect, "BackColor", Color.LightGreen); }
                //else if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.Reject || InspectStationAct.State == (int)MyStatic.E_State.RejectMeasure || InspectStationAct.State == (int)MyStatic.E_State.RejectSua) { if (lblInspect.BackColor != Color.Red) inv.set(lblInspect, "BackColor", Color.Red); }
                //else if (InspectStationAct.SuaInAction) { if (lblInspect.BackColor != Color.Blue) inv.set(lblInspect, "BackColor", Color.Blue); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.SuaFini) { if (lblInspect.BackColor != Color.LimeGreen) inv.set(lblInspect, "BackColor", Color.LimeGreen); }
                else { if (lblInspect.BackColor != Color.Blue) inv.set(lblInspect, "BackColor", Color.Blue); }

                if (InspectStationAct.SuaInAction) { if (lblSua.BackColor != Color.Blue) inv.set(lblSua, "BackColor", Color.Blue); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.RejectSuaTop) { if (lblSua.BackColor != Color.Red) inv.set(lblSua, "BackColor", Color.Red); }
                else if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.SuaFini) { if (lblSua.BackColor != Color.LimeGreen) inv.set(lblSua, "BackColor", Color.LimeGreen); }
                if (InspectStationAct.State[(int)MyStatic.InspectSt.SuaTop] == (int)MyStatic.E_State.Empty && !InspectStationAct.SuaInAction) { if (lblSua.BackColor != Color.White) inv.set(lblSua, "BackColor", Color.White); }
                //else { if (lblSua.BackColor != Color.White) inv.set(lblSua, "BackColor", Color.White); }







                Thread.Sleep(50);

            }
            catch { }




        }

        private void lblRobot_Click(object sender, EventArgs e)
        {

            if (((System.Windows.Forms.Label)sender).Name == "lblRobot") { lblStation_selected = 100; }
            else if (((System.Windows.Forms.Label)sender).Name == "lblGrip1") { lblStation_selected = 101; }
            else if (((System.Windows.Forms.Label)sender).Name == "lblGrip2") { lblStation_selected = 102; }
            else if (((System.Windows.Forms.Label)sender).Name == "lblInspect") { lblStation_selected = 103; }
            else if (((System.Windows.Forms.Label)sender).Name == "lblVision1") { lblStation_selected = 104; }
            else if (((System.Windows.Forms.Label)sender).Name == "lblVision2") { lblStation_selected = 105; }
            else if (((System.Windows.Forms.Label)sender).Name == "lblSua") { lblStation_selected = 106; }


            else lblStation_selected = -1;
        }

        private async void btnSaveOrder_Click(object sender, EventArgs e)
        {
            try
            {
                nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
                nFrontCountUpDwn = (int)upDwnCount.UpDownValue;
                nColorUpDwn = (int)upDwnColor.UpDownValue;
                FrontRotate = Single.Parse(txtFrontRotate.Text);
                //if (!chkFront.Checked) chkInspectFront.Checked = false;
                LoadTray(cmbTray.Text);
                SaveItem();
                //send data to vision

                //WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                var task1 = Task.Run(() => DataVisionSave());
                await task1;
                WebComm.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR SEND DATA TO VISION!");
                }
                var task2 = Task.Run(() => DataFrontSave());
                await task2;
                reply = task2.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR SEND DATA TO Front!");
                }
            }
            catch(Exception ex) { MessageBox.Show("ERROR SAVE DATA!"); }
        }

        private async void btnLoadOrder_Click(object sender, EventArgs e)
        {
            bool noitem = false;
            try
            {

                LoadIni();


                //txtItem.Text = "";
                //txtPartDiam.Text = "";
                //txtPartDiamd.Text = "";
                //txtPartLength.Text = "";
                txtPartWeight.Text = "";
                txtComment.Text = "";
                if (txtItem.Text == "")
                {
                    openFileDialog1.DefaultExt = "ini";
                    openFileDialog1.InitialDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Items\\";
                    openFileDialog1.Title = "Select Item";
                    openFileDialog1.ShowDialog();
                    string[] s = openFileDialog1.FileName.Split('\\'); ;
                    string[] ss = (s[s.Length - 1]).Split('.');
                    if (ss.Length == 2)
                    {
                        txtItem.Text = ss[0];

                        if (!LoadItemIniBarcode(txtItem.Text.Trim())) noitem = true; else noitem = false;
                        LoadItemData(txtItem.Text.Trim());
                        int diam = (int)Single.Parse(txtPartDiam.Text);
                        LoadDiamData(diam, noitem);
                    }
                }
                else
                {

                    if (!LoadItemIniBarcode(txtItem.Text.Trim())) noitem = true;  else noitem = false;
                    LoadItemData(txtItem.Text.Trim());
                    int diam = (int)Single.Parse(txtPartDiam.Text);
                    LoadDiamData(diam, noitem);

                }
                inv.settxt(txtWeldonAngle, weldone.angle.ToString("0.0"));
                Thread.Sleep(200);
                nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
                nFrontCountUpDwn = (int)upDwnCount.UpDownValue;
                nColorUpDwn = (int)upDwnColor.UpDownValue;
                FrontRotate = Single.Parse(txtFrontRotate.Text);
                //if (!chkFront.Checked) chkInspectFront.Checked = false;
                LoadTray(cmbTray.Text);
                SaveItem();

                //send data to vision

                //WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                var task1 = Task.Run(() => DataToVision());
                await task1;
                WebComm.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR SEND DATA TO VISION!");
                }
                var task2 = Task.Run(() => DataToFront());
                await task2;
                reply = task2.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR SEND DATA TO FRONT!");
                }

            }
            catch (Exception ex) { }
        }

        private void btnClearStations_Click(object sender, EventArgs e)
        {
            try
            {
                ClearStations();
            }
            catch (Exception ex) { }
        }
        private void ClearStations()
        {
            try
            {
                RobotLoadAct.OnGrip2_State = (int)MyStatic.E_State.Empty;
                RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.Empty;
                if (InspectStationAct.State != null) for (int i = 0; i < InspectStationAct.State.Length; i++) InspectStationAct.State[i] = (int)MyStatic.E_State.Empty;
                if (InspectStationAct.State != null) for (int i = 0; i < InspectStationAct.State.Length; i++) InspectStationAct.Reject[i] = false;
                inv.settxt(txtGrip1num, "");
                inv.settxt(txtGrip2num, "");
                inv.settxt(txtGrip3num, "");
                RobotLoadAct.OnGrip1_PartID = -1;
                RobotLoadAct.OnGrip2_PartID = -1;
                InspectStationAct.OnFooterGrip3_PartID = -1;
                Inspected_PartID = 0;
                if (InspectStationAct.Reject != null) for (int i = 0; i < InspectStationAct.Reject.Length; i++) InspectStationAct.Reject[i] = false;
                if (InspectStationAct.State != null) for (int i = 0; i < InspectStationAct.State.Length; i++) InspectStationAct.State[i] = 0;
                if (InspectStationAct.Reject != null && partData != null && RobotLoadAct.OnGrip1_PartID >= 0) for (int i = 0; i < InspectStationAct.Reject.Length; i++) partData[RobotLoadAct.OnGrip1_PartID].Reject[i] = false;
                if (InspectStationAct.Reject != null && partData != null && RobotLoadAct.OnGrip1_PartID >= 0) for (int i = 0; i < InspectStationAct.Reject.Length; i++) partData[RobotLoadAct.OnGrip2_PartID].Reject[i] = false;
                if (partData != null && RobotLoadAct.OnGrip2_PartID >= 0) partData[RobotLoadAct.OnGrip2_PartID].Position = (int)MyStatic.E_State.OnGrip1;
                InspectStationAct.WeldonState = 0;
                InspectStationAct.SuaState = 0;
                RobotLoadAct.InAction = false;
                InspectStationAct.InAction = false;
                InspectStationAct.VisionInAction = false;
                InspectStationAct.VisionFrontInAction = false;
                InspectStationAct.SuaInAction = false;
                FooterStationAct.AxisInAction = false;
                InspectStationAct.WeldonInAction = false;
                
                DeltaFront = 0;
                ErrorFront = 0;

                RefreshLables();
            }
            catch (Exception ex) { }
        }

        private async void btnOpenGripper_Click(object sender, EventArgs e)
        {

            int state = 0;
            int dout = -1;
            int ro = -1;
            int rostate = 0;
            try
            {
                ControlsEnable(false);
                if (((System.Windows.Forms.Button)sender).Name == "btnOpenGripper1") { ro = 2; rostate = 0; }
                if (((System.Windows.Forms.Button)sender).Name == "btnOpenGripper2") { ro = 3; rostate = 0; }
                if (((System.Windows.Forms.Button)sender).Name == "btnCloseGripper1") { ro = 2; rostate = 1; }
                if (((System.Windows.Forms.Button)sender).Name == "btnCloseGripper2") { ro = 3; rostate = 1; }
                var task2 = SetDOutput(dout, state, ro, rostate);//reaD AND WRITE
                await task2;
                ControlsEnable(true);


            }
            catch (Exception ex) { ControlsEnable(true); }

        }

        public int[] AI = new int[16];
        public int[] BI = new int[16];
        public int[] AO = new int[16];
        private async void btnReadIO_Click(object sender, EventArgs e)
        {
            try
            {
                MyStatic.ReadIOcont = true;
                btnReadBeckhoffIO.Enabled = false;
                var task1 = Task.Run(() => RunIO501(1));
                await task1;
                CommReply reply = new CommReply();
                reply = task1.Result;
                if (!reply.result) return; //MessageBox.Show("ERROR READ IO");
                                           //data[2] card1 input
                                           //data[3] box1 input
                                           //data[4] card2 output
                int ii = 0;
                for (int i = 0; i < 16; i++)
                {
                    ii = (Int32)Math.Pow(2, i);
                    if ((Convert.ToInt32(reply.data[2]) & ii) == ii) { AI[i] = 1; } else { AI[i] = 0; }
                    if ((Convert.ToInt32(reply.data[3]) & ii) == ii) { BI[i] = 1; } else { BI[i] = 0; }
                    if ((Convert.ToInt32(reply.data[4]) & ii) == ii) { AO[i] = 1; } else { AO[i] = 0; }
                }
                io_state.AI_IN0_Robot_DO101 = AI[0];
                io_state.AI_IN1_Robot_DO102 = AI[1];
                io_state.AI_IN2_Esafe = AI[2];
                io_state.AI_IN3_Doors1 = AI[3];
                io_state.AI_IN4_Doors2 = AI[4];
                io_state.AI_IN5_Doors3 = AI[5];
                io_state.AI_IN6_Doors4 = AI[6];
                io_state.AI_IN7_Doors5 = AI[7];
                io_state.AI_IN8_Doors6 = AI[8];
                io_state.AI_IN9_Pressure = AI[9];

                io_state.BI_IN0_SB200 = BI[0];
                io_state.BI_IN1_SB201 = BI[1];
                io_state.BI_IN2_SB202 = BI[2];
                io_state.BI_IN3_SB203 = BI[3];
                io_state.BI_IN4_SB204 = BI[4];

                io_state.AO_Out0_Air = AO[0];
                io_state.AO_Out1_V1 = AO[1];
                io_state.AO_Out2_V2 = AO[2];
                io_state.AO_Out3_V3 = AO[3];
                io_state.AO_Out4_KLV1 = AO[4];
                io_state.AO_Out5_KLV2 = AO[5];
                io_state.AO_Out6_KLV3 = AO[6];

                io_state.AO_Out8_Green = AO[8];
                io_state.AO_Out9_Yellow = AO[9];
                io_state.AO_Out10_Red = AO[10];
                io_state.AO_Out11_Buzzer = AO[11];
                io_state.AO_Out12_RobotDIN101 = AO[12];
                io_state.AO_Out13_RobotDIN102 = AO[13];


                btnReadBeckhoffIO.Enabled = true;
            }
            catch (Exception ex) { }
        }
        MyStatic.Lights lights = new MyStatic.Lights();
        private void ReadIO_asinc()
        {
            try
            {
                //MyStatic.ReadIOcont = true;
                //btnReadBeckhoffIO.Enabled = false;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff<= ReadIO cmd501" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                while (!MyStatic.bReset)
                {
                    CommReply reply = RunIO501(1);

                    if (!reply.result) return; //MessageBox.Show("ERROR READ IO");
                                               //data[2] card1 input
                                               //data[3] box1 input
                                               //data[4] card2 output
                    int ii = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        ii = (Int32)Math.Pow(2, i);
                        if ((Convert.ToInt32(reply.data[2]) & ii) == ii) { AI[i] = 1; } else { AI[i] = 0; }
                        if ((Convert.ToInt32(reply.data[3]) & ii) == ii) { BI[i] = 1; } else { BI[i] = 0; }
                        if ((Convert.ToInt32(reply.data[4]) & ii) == ii) { AO[i] = 1; } else { AO[i] = 0; }
                    }
                    io_state.AI_IN0_Robot_DO101 = AI[0];
                    io_state.AI_IN1_Robot_DO102 = AI[1];
                    io_state.AI_IN2_Esafe = AI[2];
                    if (io_state.AI_IN2_Esafe == 1)
                    { if (lbl101.BackColor != Color.LimeGreen) inv.set(lbl101, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl101.BackColor != Color.Yellow) inv.set(lbl101, "BackColor", Color.Yellow); }
                    io_state.AI_IN3_Doors1 = AI[3];
                    if (io_state.AI_IN3_Doors1 == 1)
                    { if (lbl103.BackColor != Color.LimeGreen) inv.set(lbl103, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl103.BackColor != Color.Yellow) inv.set(lbl103, "BackColor", Color.Yellow); }
                    io_state.AI_IN4_Doors2 = AI[4];
                    if (io_state.AI_IN4_Doors2 == 1)
                    { if (lbl104.BackColor != Color.LimeGreen) inv.set(lbl104, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl104.BackColor != Color.Yellow) inv.set(lbl104, "BackColor", Color.Yellow); }
                    io_state.AI_IN5_Doors3 = AI[5];
                    if (io_state.AI_IN5_Doors3 == 1)
                    { if (lbl105.BackColor != Color.LimeGreen) inv.set(lbl105, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl105.BackColor != Color.Yellow) inv.set(lbl105, "BackColor", Color.Yellow); }
                    io_state.AI_IN6_Doors4 = AI[6];
                    if (io_state.AI_IN6_Doors4 == 1)
                    { if (lbl106.BackColor != Color.LimeGreen) inv.set(lbl106, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl106.BackColor != Color.Yellow) inv.set(lbl106, "BackColor", Color.Yellow); }
                    io_state.AI_IN7_Doors5 = AI[7];
                    if (io_state.AI_IN7_Doors5 == 1)
                    { if (lbl107.BackColor != Color.LimeGreen) inv.set(lbl107, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl107.BackColor != Color.Yellow) inv.set(lbl107, "BackColor", Color.Yellow); }
                    io_state.AI_IN8_Doors6 = AI[8];
                    if (io_state.AI_IN8_Doors6 == 1)
                    { if (lbl108.BackColor != Color.LimeGreen) inv.set(lbl108, "BackColor", Color.LimeGreen); }
                    else
                    { if (lbl108.BackColor != Color.Yellow) inv.set(lbl108, "BackColor", Color.Yellow); }
                    io_state.AI_IN9_Pressure = AI[9];

                    io_state.BI_IN0_SB200 = BI[0];
                    io_state.BI_IN1_SB201 = BI[1];
                    io_state.BI_IN2_SB202 = BI[2];
                    io_state.BI_IN3_SB203 = BI[3];
                    io_state.BI_IN4_SB204 = BI[4];

                    io_state.AO_Out0_Air = AO[0];
                    io_state.AO_Out1_V1 = AO[1];
                    io_state.AO_Out2_V2 = AO[2];
                    io_state.AO_Out3_V3 = AO[3];
                    io_state.AO_Out4_KLV1 = AO[4];
                    io_state.AO_Out5_KLV2 = AO[5];
                    io_state.AO_Out6_KLV3 = AO[6];

                    io_state.AO_Out8_Green = AO[8];
                    io_state.AO_Out9_Yellow = AO[9];
                    io_state.AO_Out10_Red = AO[10];
                    io_state.AO_Out11_Buzzer = AO[11];
                    io_state.AO_Out12_RobotDIN101 = AO[12];
                    io_state.AO_Out13_RobotDIN102 = AO[13];
                    Thread.Sleep(100);
                    //
                    CommReply reply1 = new CommReply();
                    Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                    reply1.result = false;
                    Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                    for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                    ParmsPlc.SendParm[0] = 502;
                    ParmsPlc.SendParm[1] = lights.green;
                    ParmsPlc.SendParm[2] = lights.yellow;
                    ParmsPlc.SendParm[3] = lights.red;
                    ParmsPlc.SendParm[4] = lights.buzzer;

                    ParmsPlc.SendParm[10] = 1f;//tmout
                    reply1 = Beckhoff_IO.PlcSendCmd(StartAddressSendReadIO, ParmsPlc);

                    ParmsPlc.SendParm = null;


                    //
                    //var task2 = Task.Run(() => SetTraficLights(lights.green, lights.yellow, lights.red, lights.buzzer));
                    //await task2;
                }
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> Fini ReadIO cmd501" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));


                //btnReadBeckhoffIO.Enabled = true;
            }
            catch (Exception ex) { }
        }
        //Single MaxSpeed = 1000;
        private async void trackBarX_MouseLeave(object sender, EventArgs e)
        {
            inv.set(trackBarX, "Value", 0);
            //if (!chkFine.Checked) inv.settxt(txtSpeedSt, trackBarX.Value.ToString());
            try
            {
                bJogCont = false;
                if (chkFine.Checked) return;
                AxisMove = true;
                int device = 1;
                int direction = 0;
                if (trackBarX.Value > 0) direction = 0;
                else if (trackBarX.Value < 0) direction = 0;
                else direction = 0;


                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;

                float speed = Math.Abs(axis_Parameters[Axis - 1].Ax_Vmax * trackBarX.Value / 100);

                //var task1 = Task.Run(() => RunStations_Jog(device, axis, direction, speed));
                var task1 = Task.Run(() => Run_Jog(device, axis, direction, speed));
                //await Task.WhenAll(task1);
                //CommReply reply = new CommReply();
                //reply.result = false;
                //reply = task1.Result;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;


            }
        }

        private async void trackBarX_MouseUp(object sender, MouseEventArgs e)
        {
            inv.set(trackBarX, "Value", 0);
            // if (!chkFine.Checked) inv.settxt(txtSpeedSt, trackBarX.Value.ToString());
            try
            {
                bJogCont = false;
                if (chkFine.Checked) return;
                AxisMove = true;
                int device = 1;
                int direction = 0;
                if (trackBarX.Value > 0) direction = 0;
                else if (trackBarX.Value < 0) direction = 0;
                else direction = 0;


                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;

                float speed = Math.Abs(axis_Parameters[Axis - 1].Ax_Vmax * trackBarX.Value / 100);

                //var task1 = Task.Run(() => RunStations_Jog(device, axis, direction, speed));
                var task1 = Task.Run(() => Run_Jog(device, axis, direction, speed));
                await task1;
                CommReply reply = new CommReply();
                reply.result = false;
                reply = task1.Result;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;


            }
        }
        bool bJogCont = false;
        private async void trackBarX_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (!chkFine.Checked && trackBarX.Value != 0)
                {
                    inv.settxt(txtSpeedSt, Math.Abs(trackBarX.Value).ToString());
                    //inv.set(trackBarSpeedSt, "Value", int.Parse(txtSpeedSt.Text));
                    if (int.Parse(txtSpeedSt.Text) < trackBarSpeedSt.Minimum) trackBarSpeedSt.Value = trackBarSpeedSt.Minimum;
                    else if (int.Parse(txtSpeedSt.Text) > trackBarSpeedSt.Maximum) trackBarSpeedSt.Value = trackBarSpeedSt.Maximum;
                    else trackBarSpeedSt.Value = int.Parse(txtSpeedSt.Text);
                }
                int val = trackBarX.Value;
                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int ax = Axis;
                //
                //int speed = (int)(0.2f * AxStatus[ax - 1].Vmax * val / 100);
                //
                Single fine = Single.Parse(cmbFine.Text);
                bJogCont = false;
                Thread.Sleep(100);
                var task = Task.Run(() => JogContinue(ax, val, fine));
                await task;
                return;


            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;


            }
        }
        private bool JogContinue(int ax, int valcont, Single fine)
        {
            bJogCont = false;
            //if (!chkFine.Checked && val != 0)
            //{
            //    inv.settxt(txtSpeedSt, Math.Abs(val).ToString());
            //    //inv.set(trackBarSpeedSt, "Value", int.Parse(txtSpeedSt.Text));
            //    if (int.Parse(txtSpeedSt.Text) < trackBarSpeedSt.Minimum) trackBarSpeedSt.Value = trackBarSpeedSt.Minimum;
            //    else if (int.Parse(txtSpeedSt.Text) > trackBarSpeedSt.Maximum) trackBarSpeedSt.Value = trackBarSpeedSt.Maximum;
            //    else trackBarSpeedSt.Value = int.Parse(txtSpeedSt.Text);
            //}
            try
            {
                //if (chkFine.Checked) return;
                AxisMove = true;
                int device = 1;
                int direction = 0;
                if (valcont > 0) direction = 2;
                else if (valcont < 0) direction = 1;
                else direction = 0;


                Axis = int.Parse(ax.ToString());
                int axis = Axis;

                float speed = 0.2f * Math.Abs(axis_Parameters[Axis - 1].Ax_Vmax * valcont / 100);
                int mode = 0;
                if (chkFine.Checked) mode = 1;
                //var task1 = Task.Run(() => RunStations_Jog(device, axis, direction, speed));
                Single pos = Single.Parse(fine.ToString());
                CommReply reply = new CommReply();
                reply = Run_Jog(device, axis, direction, speed, mode, pos, 0);

                reply.result = false;

                //if (!reply.result)
                //{
                //    Thread.Sleep(100);
                //    var task3 = Task.Run(() => Run_Jog(device, axis, direction, speed, mode, pos, 0));
                //    await task3;
                //};
                Thread.Sleep(100);
                bJogCont = true;
                int cont = 1;
                while (bJogCont && Control.MouseButtons == MouseButtons.Left && !MyStatic.bReset)
                {
                    reply = Run_Jog(device, axis, direction, speed, mode, pos, cont);
                    reply.result = false;

                    Thread.Sleep(500);
                }
                return true;

            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return false;


            }
        }

        private async void btnMoveRel_Click(object sender, EventArgs e)
        {
            try
            {
                ControlsEnable(false);
                //if (trackBarSpeedSt.Value > 10) trackBarSpeedSt.Value = 10;
                Single dist = 0;
                //inv.set(btnPlus, "Enabled", false);
                //Axis = int.Parse(cmbAxes.Text);
                string[] s = cmbAxes.Text.Split(':');
                Axis = int.Parse(s[0]);
                int axis = Axis;
                Single Speed = AxStatus[axis - 1].Vmax;
                Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                dist = Single.Parse(txtRel.Text);


                var task1 = Task.Run(() => MoveRelSt(axis, dist, speed));
                await task1;


                CommReply reply = new CommReply();
                reply.result = false;
                reply = task1.Result;
                inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                if (!(reply.status == "" || reply.status == null))
                {
                    MessageBox.Show("ERROR MOVE RELATIVE! " + "\r" + reply.status);
                    ControlsEnable(true);
                    //inv.set(btnPlus, "Enabled", true);
                    return;
                }
                if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return; };

                ControlsEnable(true);
                inv.set(btnPlus, "Enabled", true);
            }
            catch (Exception ex) { MessageBox.Show("ERROR MOVE RELATIVE! "); }
        }
        AxStatus[] AxStatus = new AxStatus[5];
        private CommReply RunAxisStatus(int axis = 0)//current position
        {
            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;


                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Status;
                ParmsPlc.SendParm[1] = axis;//cam
                ParmsPlc.SendParm[10] = 2.5f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                ParmsPlc.SendParm = null;
                //wait fini async
                return reply;
            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        private async void btnStatus_Click(object sender, EventArgs e)
        {
            CommReply reply = new CommReply();
            try
            {
                for (int i = 0; i < 5; i++)
                {

                    var task1 = Task.Run(() => RunAxisStatus(i + 1));
                    await task1;
                    reply = task1.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR READ STATUS " + "\r");
                        return;
                    }
                    AxStatus[i].StandStill = (int)reply.data[3];
                    AxStatus[i].Moving = (int)reply.data[4];
                    AxStatus[i].Limit = (int)reply.data[5];
                    AxStatus[i].Disable = (int)reply.data[6];
                    AxStatus[i].Error = (int)reply.data[7];
                    AxStatus[i].ErrorId = (int)reply.data[8];
                    AxStatus[i].ReadErrorID = (int)reply.data[9];
                    Thread.Sleep(1);
                }
                var task2 = Task.Run(() => RunAxisStatus(6));
                await task2;
                reply = task2.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR READ STATUS " + "\r");
                    return;
                }
                AxStatus[0].Vmax = reply.data[3];
                AxStatus[1].Vmax = reply.data[4];
                AxStatus[2].Vmax = reply.data[5];
                AxStatus[3].Vmax = reply.data[6];
                AxStatus[4].Vmax = reply.data[7];

            }
            catch (Exception ex)
            {

                reply.result = false;
                reply.comment = ex.Message;


            }
        }

        private async void trackBarX_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void btnPortDisable_Click(object sender, EventArgs e)
        {
            //port "Ethernet 3" for example
            Disable("Ethernet 3");
        }
        public void Disable(string interfaceName)
        {

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.Arguments = "/C netsh interface set interface name= " + Convert.ToChar(34) + interfaceName + Convert.ToChar(34) + " admin=disabled";
            p.StartInfo.Verb = "runas";


            p.Start();

            p.WaitForExit();
            p.Close();


            return;


        }

        public void Enable(string interfaceName)
        {

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/C netsh interface set interface name= " + Convert.ToChar(34) + interfaceName + Convert.ToChar(34) + " admin=enabled";
            p.StartInfo.Verb = "runas";
            p.Start();
            p.WaitForExit();
            p.Close();
            return;



        }

        private void btnPortEnable_Click(object sender, EventArgs e)
        {
            //port "Ethernet 3" for example
            Enable("Ethernet 3");
        }


        private void MatrixTrans()
        {
            //P0X = 30.5746555328369
            //P0Y = -554.943481445313
            //P0Z = -56.4154319763184
            //P0R = 78.6154403686523
            //P1X = 320.494720458984
            //P1Y = -554.943481445313
            //P1Z = -56.4154052734375
            //P1R = 78.6154403686523
            //P2X = 30.5746841430664
            //P2Y = -383.103546142578
            //P2Z = -56.4155311584473
            //P2R = 78.6154479980469
            //P3X = 308.974761962891
            //P3Y = -383.103668212891
            //P3Z = -56.4155960083008
            //P3R = 78.6154403686523


            Single TrayDeltaX = Single.Parse(txtDx.Text);
            Single TrayDeltaY = Single.Parse(txtDy.Text);
            int TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
            int TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);
            float bHeight = (TrayInsertsOnY - 1) * TrayDeltaY;
            float bWidth = (TrayInsertsOnX - 1) * TrayDeltaX;

            Single Xx = (Single)RobotLoadPoints.P_1.x;
            Single Xy = (Single)RobotLoadPoints.P_2.y;
            Single Yx = (Single)RobotLoadPoints.P_2.x;
            Single Yy = (Single)RobotLoadPoints.P_2.y;
            Single Ox = (Single)RobotLoadPoints.P_0.x;
            Single Oy = (Single)RobotLoadPoints.P_0.y;

            Vector X = new Vector(Xx, Xy); //X axis vector
            Vector Y = new Vector(Yx, Yy); //Y axis vector
            Vector O = new Vector(Ox, Oy); //Origin of coordinate system.

            double scalex = Math.Sqrt(Math.Pow(Xx, 2) + Math.Pow(Xy, 2));
            double scaley = Math.Sqrt(Math.Pow(Yx, 2) + Math.Pow(Yy, 2));
            double rotation_ang = Math.Atan2(Xy, Yx);
        }
        private void PickCoord(Panel panel, ref int Partindex, ref position outpos)
        {

            //pictureBox1.Width = panelTrayOut.Width;
            //pictureBox1.Height = panelTrayOut.Height;
            //int partindex = TrayPartId;
            //Panel panel = panelTrayOut;

            //position outpos = new position();
            Template template = new Template(); // Original template image.
            Document document = new Document(); // Printed and scanned distorted image.

            //template.CreateTemplateImage();

            // The template image is printed and scanned. This method generates an example scan or this question.
            document.CreateDistortedImageFromTemplateImage();

            InvDrawTrayOutMatrix(panel, ref Partindex, ref outpos);
            //return outpos;
            // Stuck here.
            //document.Transform();
            // Draw transformed points on the image to verify that transformation is successful.
            //document.DrawPoints();
        }
        public System.Drawing.Point[] pnts = new System.Drawing.Point[1];
        public System.Drawing.Point[] pntsrot = new System.Drawing.Point[1];
        public Single angleOfRotation = 0;
        private void InvDrawTrayOutMatrix(Panel panel, ref int Partindex, ref position outpos, int from = 0, int to = 0)
        {

            Single scale = (Single)(panel.Height / 310.000);//panel.wi
            panel.Width = (int)(252 * scale);
            Single TrayDeltaX = Single.Parse(txtDx.Text);
            Single TrayDeltaY = Single.Parse(txtDy.Text);
            int TrayInsertsOnX = int.Parse(txtPlaceNumRow.Text);
            int TrayInsertsOnY = int.Parse(txtPlaceNumCol.Text);
            //Single yoff = 0;
            //Single xoff = 0;
            int insertsY = 0;
            int insertsZ = 0;
            Single deltaY = 0;
            Single deltaX = 0;
            int last = 0;
            Single Xmax = -500, Xmin = 500, Ymax = -500, Ymin = 500;

            int partindex = 0;
            string[] arr = new string[100];

            partindex = Partindex;
            if (partindex < 0) partindex = 0;


            Single MarkingArrayH = TrayDeltaX * (TrayInsertsOnX - 1);
            Single MarkingArrayW = TrayDeltaY * (TrayInsertsOnY - 1);
            outpos.Error = "";

            if (partindex >= pnts.Length)
            {
                MessageBox.Show("ERROR IN TRAY OUT PART INDEX", "ERROR", MessageBoxButtons.OK,
                           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                //MyStatic.bExitcycle = true;
                partindex = (pnts.Length) - 1;
                //return;
            }


            try
            {
                using (Graphics g = panel.CreateGraphics())
                {
                    //BeginGraphics:
                    Pen pen = new Pen(Color.Black, 2);
                    Brush brush = new SolidBrush(Color.Yellow);

                    position pos = new position();

                    int i = 0;

                    ////draw
                    g.Clear(panel.BackColor);
                    g.RotateTransform(angleOfRotation);
                    //Array.Resize<Cell>(ref cell, part.Length);
                    for (i = 0; i < pnts.Length; i++)
                    {

                        //mark
                        brush = new SolidBrush(Color.LightGray);

                        last = LastPlace;


                        if (partindex == i)
                        {

                            if (partindex >= pntsrot.Length - 1)
                            //if ((TrayInsertsOnX == 2 && TrayInsertsOnY == 10 && partindex >= 20 - 1) ||  //TrayInsertsOnX * TrayInsertsOnY)
                            //    (TrayInsertsOnX == 3 && TrayInsertsOnY == 18 && partindex >= 51 - 1) ||
                            //     (TrayInsertsOnX == 2 && TrayInsertsOnY == 18 && partindex >= 34 - 1))
                            { outpos.EndOfTray = 1; }

                            outpos.x = pntsrot[i].Y / 10.0;
                            outpos.y = pntsrot[i].X / 10.0;
                            outpos.z = 0;// part[i].z;
                            outpos.r = 0;


                        }


                        if (pnts[i].X / 10 < 0)
                        { outpos.Error = "Place position X OUT OF TRAY!"; };
                        if (pnts[i].Y / 10 < 0)
                        { outpos.Error = "Place position Y OUT OF TRAY!"; };
                        if (pnts[i].X / 10 > panel.Width)
                        { outpos.Error = "Place position X OUT OF TRAY!"; };
                        if (pnts[i].Y / 10 > panel.Height)
                        { outpos.Error = "Place position Y OUT OF TRAY!"; };

                        //##################### korea1 fini ##################
                        if (panel.Name == panelTrayOut.Name)
                        {
                            if (partindex > i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.LightGray);
                            }
                            else if (partindex == i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Yellow);
                            }
                            else if (partindex < i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Lime);
                            }
                        }
                        else if (panel.Name == panelTrayRej.Name)
                        {
                            if (partindex > i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Red);
                            }
                            else if (partindex == i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.Yellow);
                            }
                            else if (partindex < i)
                            {
                                pen.Color = Color.Black; pen.Width = 1;
                                brush = new SolidBrush(Color.LightGray);
                            }
                        }
                        ///
                        TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
                        TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
                        TrayInsertsOnX = int.Parse(newFrmMain.txtPlaceNumRow.Text);
                        TrayInsertsOnY = int.Parse(newFrmMain.txtPlaceNumCol.Text);
                        float bHeight = (TrayInsertsOnY - 1) * TrayDeltaY;
                        float bWidth = (TrayInsertsOnX - 1) * TrayDeltaX;
                        //Array.Resize<System.Drawing.Point>(ref Pointss, TrayInsertsOnX * TrayInsertsOnY);
                        Single scale1 = this.panelTrayOut.Width / (float)(TrayInsertsOnX * TrayDeltaX);
                        Single scale2 = this.panelTrayOut.Height / (float)(TrayInsertsOnY * TrayDeltaY);

                        g.FillRectangle(brush, pnts[i].X * scale1 / 10 + 5, pnts[i].Y * scale2 / 10, scale1 * TrayDeltaX - 2, scale2 * TrayDeltaY - 2);

                        g.DrawRectangle(pen, pnts[i].X * scale1 / 10 + 5, pnts[i].Y * scale2 / 10, scale1 * TrayDeltaX - 2, scale2 * TrayDeltaY - 2);
                        pen.Width = 1;

                        brush = new SolidBrush(Color.Silver);
                        g.DrawString(i.ToString(), this.Font, Brushes.Black, (pnts[i].X * scale1 / 10 + scale1 * TrayDeltaX / 2.0F), (pnts[i].Y * scale2 / 10 + scale2 * TrayDeltaY / 2.0F - 5));


                    }//end for

                    pen.Dispose();
                    brush.Dispose();
                    //BathRefresh(MyStatic.PartIndex);
                    // UpdateMainTexts(outpos);
                }

                //update texts


                lblTrayY.Text = (outpos.y).ToString("0.0");
                lblTrayX.Text = (outpos.x).ToString("0.0");

                if ((outpos.Error != null) && (outpos.Error != ""))
                {
                    dFile.WriteLogFile("DROW PALLET ERROR:" + outpos.Error);
                    MessageBox.Show("DROW PALLET ERROR:" + outpos.Error, "ERROR", MessageBoxButtons.OK,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }

                return;

            }
            catch (Exception er)
            {
                dFile.WriteLogFile("ERROR UPDATE PALLET:" + er.Message);
                MessageBox.Show("ERROR UPDATE PALLET:" + er.Message, "ERROR", MessageBoxButtons.OK,
                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }
        public class Page
        {
            public Bitmap Image1 { get; set; }

            Single TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
            Single TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
            int TrayInsertsOnX = int.Parse(newFrmMain.txtPlaceNumRow.Text);
            int TrayInsertsOnY = int.Parse(newFrmMain.txtPlaceNumCol.Text);
            public System.Drawing.Point[] Pointss = new System.Drawing.Point[100]; // Coordinates to transform in the TemplateScanned derived class!

        }
        public class Template : Page
        {
            public Template()
            {
                this.Image1 = new Bitmap(newFrmMain.pictureBox1.Width, newFrmMain.pictureBox1.Height);

                // Known points of interest. Consider them hardcoded.
                Single TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
                Single TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
                int TrayInsertsOnX = int.Parse(newFrmMain.txtPlaceNumRow.Text);
                int TrayInsertsOnY = int.Parse(newFrmMain.txtPlaceNumCol.Text);
                float bHeight = (TrayInsertsOnY - 1) * TrayDeltaY;
                float bWidth = (TrayInsertsOnX - 1) * TrayDeltaX;
                Pointss = new System.Drawing.Point[TrayInsertsOnX * TrayInsertsOnY];
                int index = 0;
                Single scale1 = this.Image1.Width / (float)(TrayInsertsOnX * TrayDeltaX);
                Single scale2 = this.Image1.Height / (float)(TrayInsertsOnY * TrayDeltaY);
                for (int y = 0; y < TrayInsertsOnY; y++)
                    for (int x = 0; x < TrayInsertsOnX; x++)
                        this.Pointss[index++] = new System.Drawing.Point((int)((TrayDeltaX) * x * 10), (int)((TrayDeltaY) * y * 10));

            }

        }
        public class Document : Page
        {
            public struct StructTransformation
            {
                public float AngleOfRotation;
                public SizeF ScaleRatio;
                public SizeF TranslationOffset;
            }

            private Template Template = new Template();
            private StructTransformation Transformation = new StructTransformation();

            public Document()
            {
                try
                {
                    //Single rotation_ang = 0.0f;
                    //Single ScaleWidth = 0.0f;
                    //Single ScaleHeight = 0.0f;
                    //rotation_ang = (Single)(rotation_ang * 360.0 / (2 * Math.PI));

                    Single TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
                    Single TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
                    int TrayInsertsOnX = int.Parse(newFrmMain.txtPlaceNumRow.Text);
                    int TrayInsertsOnY = int.Parse(newFrmMain.txtPlaceNumCol.Text);
                    float bHeight = (TrayInsertsOnY - 1) * TrayDeltaY;
                    float bWidth = (TrayInsertsOnX - 1) * TrayDeltaX;
                    this.Pointss = new System.Drawing.Point[TrayInsertsOnX * TrayInsertsOnY];
                    this.Template = new Template();

                    this.Transformation = new StructTransformation { AngleOfRotation = newFrmMain.rotation_ang, ScaleRatio = new SizeF(newFrmMain.ScaleWidth, newFrmMain.ScaleHeight), TranslationOffset = new SizeF(0, 0) };

                    for (int i = 0; i < this.Pointss.Length; i++)
                        this.Pointss[i] = this.Template.Pointss[i];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR CREATE TRAY! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
            }

            // Just distorts the original template image as if it had been read from a scanner.
            public void CreateDistortedImageFromTemplateImage()
            {
                // Distort coordinates.
                Single TrayDeltaX = Single.Parse(newFrmMain.txtDx.Text);
                Single TrayDeltaY = Single.Parse(newFrmMain.txtDy.Text);
                int TrayInsertsOnX = int.Parse(newFrmMain.txtPlaceNumRow.Text);
                int TrayInsertsOnY = int.Parse(newFrmMain.txtPlaceNumCol.Text);
                float bHeight = (TrayInsertsOnY - 1) * TrayDeltaY;
                float bWidth = (TrayInsertsOnX - 1) * TrayDeltaX;
                Single scale1 = (float)(newFrmMain.RobotLoadPoints.P1.x - newFrmMain.RobotLoadPoints.P0.x) / (float)(TrayInsertsOnX * TrayDeltaX);
                Single scale2 = (float)(newFrmMain.RobotLoadPoints.P1.y - newFrmMain.RobotLoadPoints.P0.y) / (float)(TrayInsertsOnY * TrayDeltaY);
                Matrix matrix = new Matrix();
                matrix.Rotate(this.Transformation.AngleOfRotation);
                matrix.Scale(this.Transformation.ScaleRatio.Width, this.Transformation.ScaleRatio.Height);
                matrix.Translate(this.Transformation.TranslationOffset.Width, this.Transformation.TranslationOffset.Height);

                System.Drawing.Point[] points = new System.Drawing.Point[TrayInsertsOnX * TrayInsertsOnY];
                newFrmMain.pnts = new System.Drawing.Point[Pointss.Length];
                for (int i = 0; i < newFrmMain.pnts.Length; i++) newFrmMain.pnts[i] = Pointss[i];
                matrix.TransformPoints(Pointss);
                newFrmMain.pntsrot = new System.Drawing.Point[Pointss.Length];
                for (int i = 0; i < newFrmMain.pnts.Length; i++) newFrmMain.pntsrot[i] = Pointss[i];
                newFrmMain.angleOfRotation = this.Transformation.AngleOfRotation;

            }


        }

        private async void btnTeachToolAuto_Click(object sender, EventArgs e)
        {
            System.Console.Beep();
            System.Media.SystemSounds.Beep.Play();
            try
            {
                inv.set(panel3, "Enabled", false);
                inv.settxt(txtToolX, "");
                inv.settxt(txtToolY, "");
                inv.settxt(txtToolZ, "");
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<= Teach Tool" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                inv.settxt(lblRobot, "Teach Tool");
                if (RobotLoadAct.InAction) return; ;
                RobotLoadAct.InAction = true;
                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();
                Parms.cmd = ((int)MyStatic.RobotCmd.TeachTool).ToString();
                Parms.DebugTime = 1000;
                Parms.comment = "Teach Tool";
                Array.Resize<Single>(ref Parms.SendParm, 10);

                Parms.timeout = 180;

                Parms.SendParm[0] = (int)MyStatic.RobotCmd.TeachTool;
                Single s = (Single)frmMain.newFrmMain.FanucSpeed;// general speed
                if (s > 15) s = 15;
                Parms.SendParm[1] = s;// general speed
                Parms.SendParm[2] = 165;// tool z 330 mm height of axis 2;-165 mm Z coord of gripper touching robot base;330-165=165 mm

                Parms.SendParm[9] = 180;// 0.5f;//timeout

                MyStatic.bReset = false;
                var task1 = Task.Run(() => FW1.RunCmdFanuc(Parms));

                await task1;

                RobotFunctions.CommReply rep1 = task1.Result;
                RobotLoadAct.InAction = false;
                inv.set(panel3, "Enabled", true);
                if (rep1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Robot<=Tool Calc Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    inv.settxt(lblRobot, "Teach Tool Fini");
                    inv.settxt(txtToolX, rep1.data[2].ToString("0.0"));
                    inv.settxt(txtToolY, rep1.data[3].ToString("0.0"));
                    inv.settxt(txtToolZ, rep1.data[4].ToString("0.0"));
                    return; ;
                }
                else
                {
                    //MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                    //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.settxt(lblRobot, "Teach Tool Error");
                    return; ;
                }

            }
            catch (Exception err)
            {
                inv.set(panel3, "Enabled", true);
                return;
            }
        }

        private async void btnPlcParameters_Click(object sender, EventArgs e)
        {
            try
            {
                var task = Task.Run(() => SendPlcParameters());
                await task;
                if (!task.Result.result)
                {
                    MessageBox.Show("ERROR SEND PLC PARAMETERS!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            catch (Exception ex) { }
        }


        MyStatic.AxisParameters[] axis_Parameters = new MyStatic.AxisParameters[5];
        private CommReply SendPlcParameters()//current position
        {
            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                int axis = 0;

                if (!LoadPlcData()) return reply;

                for (int i = 0; i < 5; i++)
                {
                    Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                    int j = 0;


                    axis = i + 1;
                    ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Parameters;
                    ParmsPlc.SendParm[1] = axis;
                    ParmsPlc.SendParm[2] = axis_Parameters[i].Ax_Home;
                    ParmsPlc.SendParm[3] = axis_Parameters[i].Ax_Max;
                    ParmsPlc.SendParm[4] = axis_Parameters[i].Ax_Min;
                    ParmsPlc.SendParm[5] = axis_Parameters[i].Ax_Vmax;
                    ParmsPlc.SendParm[6] = axis_Parameters[i].Ax_Min;
                    ParmsPlc.SendParm[7] = axis_Parameters[i].Ax_Max;
                    ParmsPlc.SendParm[10] = 2.5f;//tmout

                    reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);


                    ParmsPlc.SendParm = null;

                    if (!reply.result) return reply; ;
                    Thread.Sleep(2);
                }
                reply.result = true;
                return reply;

            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        private bool LoadPlcData()
        {
            string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\" + "PLC_Data" + ".ini";

            string[][] arrnew = new string[1][];
            arrnew[0] = new string[0];
            //create vars array
            string mess = "";
            if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ PLC FILE"); return false; }
            try
            {
                //create vars array
                //robot 1

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_Home", arrnew), out axis_Parameters[0].Ax_Home)) mess = mess + "axis_Parameters[0].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_Max", arrnew), out axis_Parameters[0].Ax_Max)) mess = mess + "axis_Parameters[0].Ax_Max" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_Min", arrnew), out axis_Parameters[0].Ax_Min)) mess = mess + "axis_Parameters[0].Ax_Min" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_Vmax", arrnew), out axis_Parameters[0].Ax_Vmax)) mess = mess + "axis_Parameters[0].Ax_Vmax" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax1", "Ax_Work", arrnew), out axis_Parameters[0].Ax_Work)) mess = mess + "axis_Parameters[0].Ax_Work" + "\r\n";
                //inv.set(upDwnCam2z, "UpDownValue", axis_Parameters[0].Ax_Work);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_Home", arrnew), out axis_Parameters[1].Ax_Home)) mess = mess + "axis_Parameters[1].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_Max", arrnew), out axis_Parameters[1].Ax_Max)) mess = mess + "axis_Parameters[1].Ax_Max" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_Min", arrnew), out axis_Parameters[1].Ax_Min)) mess = mess + "axis_Parameters[1].Ax_Min" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_Vmax", arrnew), out axis_Parameters[1].Ax_Vmax)) mess = mess + "axis_Parameters[1].Ax_Vmax" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax2", "Ax_Work", arrnew), out axis_Parameters[1].Ax_Work)) mess = mess + "axis_Parameters[1].Ax_Work" + "\r\n";
                //inv.set(upDwnLamp1Z, "UpDownValue", axis_Parameters[1].Ax_Work);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_Home", arrnew), out axis_Parameters[2].Ax_Home)) mess = mess + "axis_Parameters[2].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_Max", arrnew), out axis_Parameters[2].Ax_Max)) mess = mess + "axis_Parameters[2].Ax_Max" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_Min", arrnew), out axis_Parameters[2].Ax_Min)) mess = mess + "axis_Parameters[2].Ax_Min" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_Vmax", arrnew), out axis_Parameters[2].Ax_Vmax)) mess = mess + "axis_Parameters[2].Ax_Vmax" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax3", "Ax_Work", arrnew), out axis_Parameters[2].Ax_Work)) mess = mess + "axis_Parameters[2].Ax_Work" + "\r\n";
                //inv.set(upDwnCam1x, "UpDownValue", axis_Parameters[2].Ax_Work);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_Home", arrnew), out axis_Parameters[3].Ax_Home)) mess = mess + "axis_Parameters[3].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_Max", arrnew), out axis_Parameters[3].Ax_Max)) mess = mess + "axis_Parameters[3].Ax_Max" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_Min", arrnew), out axis_Parameters[3].Ax_Min)) mess = mess + "axis_Parameters[3].Ax_Min" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_Vmax", arrnew), out axis_Parameters[3].Ax_Vmax)) mess = mess + "axis_Parameters[3].Ax_Vmax" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax4", "Ax_Work", arrnew), out axis_Parameters[3].Ax_Work)) mess = mess + "axis_Parameters[3].Ax_Work" + "\r\n";
                //inv.set(upDwnFootWorkR, "UpDownValue", axis_Parameters[3].Ax_Home);

                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Home", arrnew), out axis_Parameters[4].Ax_Home)) mess = mess + "axis_Parameters[4].Ax_Home" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Max", arrnew), out axis_Parameters[4].Ax_Max)) mess = mess + "axis_Parameters[4].Ax_Max" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Min", arrnew), out axis_Parameters[4].Ax_Min)) mess = mess + "axis_Parameters[4].Ax_Min" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Vmax", arrnew), out axis_Parameters[4].Ax_Vmax)) mess = mess + "axis_Parameters[4].Ax_Vmax" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Work", arrnew), out axis_Parameters[4].Ax_Work)) mess = mess + "axis_Parameters[4].Ax_Work" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Weldone", arrnew), out axis_Parameters[4].Ax_Weldone)) mess = mess + "axis_Parameters[4].Ax_Weldone" + "\r\n";
                //if (!Single.TryParse(IniData.GetKeyValueArrINI("Ax5", "Ax_Diam", arrnew), out axis_Parameters[4].Ax_Diameter)) mess = mess + "axis_Parameters[4].Ax_Diam" + "\r\n";
                //inv.set(upDwnFootWorkX, "UpDownValue", axis_Parameters[4].Ax_Work);
                //inv.set(upDwnFootWeldX, "UpDownValue", axis_Parameters[4].Ax_Weldone);
                //inv.set(upDwnFootDX, "UpDownValue", axis_Parameters[4].Ax_Diameter);
                return true;



            }
            catch (Exception err)
            { MessageBox.Show("ERROR READ PLC FILE " + err); return false; }
        }
        private bool LoadDiamData(int diam, bool noitem)
        {
            string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\" + "Diam_Data" + ".ini";

            string[][] arrnew = new string[1][];
            arrnew[0] = new string[0];
            //create vars array
            string mess = "";
            if (!IniData.ReadIniFile(rob_inifile, ref arrnew)) { MessageBox.Show("ERROR READ DIAMETER FILE"); return false; }
            Single _diam = Convert.ToInt32(diam);
            Single data = 0f;
            try
            {
                //create vars array
                //robot 1

                if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "weldX", arrnew), out weldone.weldX)) mess = mess + "weldone.weldX" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "weldH", arrnew), out weldone.weldH)) mess = mess + "weldone.weldH" + "\r\n";
                if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "angle", arrnew), out weldone.angle)) mess = mess + "weldone.weldH" + "\r\n";
                if (mess.Trim() != "")
                {
                    MessageBox.Show("READ DIAMETER DATA ERROR!", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                if(noitem)//load corrections
                {
                    mess = "";
                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax1_WorkTop", arrnew), out data)) mess = mess + "Ax1_WorkTop" + "\r\n";
                    else inv.set(upDwnCam2z, "UpDownValue", data);

                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax2_WorkTop", arrnew), out data)) mess = mess + "Ax2_WorkTop" + "\r\n";
                    else inv.set(upDwnLamp1Z, "UpDownValue", data);

                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax3_WorkFront", arrnew), out data)) mess = mess + "Ax3_WorkFront" + "\r\n";
                    else inv.set(upDwnCam1x, "UpDownValue", data);

                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax4_WorkTop", arrnew), out data)) mess = mess + "Ax4_WorkTop" + "\r\n";
                    else inv.set(upDwnFootWorkR, "UpDownValue", data);
                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax5_WorkTop", arrnew), out data)) mess = mess + "Ax5_WorkTop" + "\r\n";
                    else inv.set(upDwnFootWorkTopX, "UpDownValue", data);
                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax5_Weldone", arrnew), out data)) mess = mess + "Ax5_Weldone" + "\r\n";
                    else inv.set(upDwnFootWeldX, "UpDownValue", data);
                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax5_Diam", arrnew), out data)) mess = mess + "Ax5_Diam" + "\r\n";
                    else inv.set(upDwnFootDX, "UpDownValue", data);
                    if (!Single.TryParse(IniData.GetKeyValueArrINI(_diam.ToString("0.000"), "Ax5_WorkFront", arrnew), out data)) mess = mess + "Ax5_WorkFront" + "\r\n";
                    else inv.set(upDwnFootWorkFrontX, "UpDownValue", data);

                    if (mess.Trim() != "")
                    {
                        MessageBox.Show("READ DIAMETER DATA ERROR!", "ERROR", MessageBoxButtons.OK,
                                           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
                return true;



            }
            catch (Exception err)
            { MessageBox.Show("ERROR READ DIAMETER FILE " + err); return false; }
        }
        private bool SaveDiamData(int diam, bool noitem=false)
        {
            string rob_inifile = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\" + "Diam_Data" + ".ini";
            string[][] arrsave = new string[1][];
            arrsave[0] = new string[1];
            string[] s;
            s = new string[9];
            string mess = "";
            //create vars array
            try
            {
                if (!noitem)
                {
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "angle", txtWeldonAngle.Text, ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }


                    string error = "";
                    if (!RobotData.WriteIniFile(rob_inifile.Trim(), arrsave, out error))
                    {
                        MessageBox.Show("ERROR SAVE DIAM DATA");
                        return false;
                    }
                    return true;
                }
                else
                {
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax1_WorkTop", upDwnCam2z.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax2_WorkTop", upDwnLamp1Z.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax3_WorkFront", upDwnCam1x.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax4_WorkTop", upDwnFootWorkR.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax5_WorkTop", upDwnFootWorkTopX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax5_Weldone", upDwnFootWeldX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax5_Diam", upDwnFootDX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    if (!RobotData.CreateKeyValueArr(diam.ToString("0.000"), "Ax5_WorkFront", upDwnFootWorkFrontX.UpDownValue.ToString(), ref arrsave, 0, 0, false, ref mess)) { MessageBox.Show("ERROR DATA " + mess); }
                    string error = "";
                    if (!RobotData.WriteIniFile(rob_inifile.Trim(), arrsave, out error))
                    {
                        MessageBox.Show("ERROR SAVE DIAM DATA");
                        return false;
                    }
                    return true;
                }

            }
            catch (Exception err)
            { MessageBox.Show("ERROR SAVE FILE " + err); return false; }
        }

        private async void chkV1_CheckedChanged(object sender, EventArgs e)
        {
            int valve = 0;
            int state = 0;
            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            reply.result = false;

            if (((System.Windows.Forms.CheckBox)sender).Name == "chkV1")
            {
                valve = 1;
                if (chkV1.Checked) state = 1; else state = 2;
            }
            else if (((System.Windows.Forms.CheckBox)sender).Name == "chkV2")
            {
                valve = 2;
                if (chkV2.Checked) state = 1; else state = 2;
            }
            else if (((System.Windows.Forms.CheckBox)sender).Name == "chkV3")
            {
                valve = 3;
                if (chkV3.Checked) state = 1; else state = 2;
            }
            else return;

            var task1 = Task.Run(() => SetValve(valve, state));
            await task1;
            reply = task1.Result;
        }
        private CommReply SetValve(int valve, int state)
        {

            CommReply reply = new CommReply();
            Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
            try
            {
                reply.result = false;

                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                int j = 0;

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Valve;
                ParmsPlc.SendParm[1] = valve;
                ParmsPlc.SendParm[2] = state;

                ParmsPlc.SendParm[10] = 5f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                return reply;
            }
            catch (Exception ex) { return reply; }
        }

        private async void btnCycle_Click(object sender, EventArgs e)
        {
            try
            {

                btnCycle.Enabled = false;
                ControlsEnable(false);
                MyStatic.bExitcycle = false;
                FooterStationAct.AxisInAction = false;

                var task = Task.Run(() => TestCycle());
                await task;
                  
                btnCycle.Enabled = true;
                ControlsEnable(true);
                //MyStatic.bExitcycle = false;

                //inv.set(btnPlus, "Enabled", true);
                //Single dist = 22.5f;
                //Single Speed = AxStatus[4 - 1].Vmax;
                //Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;
                //Stopwatch sw = new Stopwatch();
                //sw.Restart();
                //for (int i = 0; i < 16; i++)
                //{
                //    var task = Task.Run(() => MoveAbs(0, 4, (dist * i), speed));
                //    await task;
                //    Thread.Sleep((int)Single.Parse(txtWaitCycle.Text) * 1000);
                //    if (MyStatic.bExitcycle) break;

                //}
                //sw.Stop();
                //MessageBox.Show("Time:" + (sw.ElapsedMilliseconds / 1000.0f).ToString("0.00") + "\r\n"+ "Cycle:" + (sw.ElapsedMilliseconds / 16000.0f).ToString("0.00"));
                //    ControlsEnable(true);
                btnCycle.Enabled = true;
            }
            catch (Exception ex) { btnCycle.Enabled = true; }
        }
        private async Task<bool> TestCycle()
        {
            try
            {
                while (!MyStatic.bReset && !MyStatic.bExitcycle)
                {
                    Thread.Sleep(100);
                    Single b = 1.0f;
                    //MyStatic.bExitcycle = false;
                    if (FooterStationAct.AxisInAction) return false;
                    FooterStationAct.AxisInAction = true;
                    //btnFooterWork.Enabled = false;
                    //btnFooterWork1.Enabled = false;
                    //SetTraficLights(0, 0, 0, 0);//
                    int axis = 0;
                    if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                    {
                        MessageBox.Show("ERROR PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        btnFooterWork.Enabled = false;
                        FooterStationAct.AxisInAction = true;
                        return false;
                    }
                    if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                    if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                    Single Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                    Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                    Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                                                                             //lamp1
                    BitArray lamp = new BitArray(new bool[8]);
                    lamp[0] = true; //lamp1
                    byte[] Lamps = new byte[1];
                    lamp.CopyTo(Lamps, 0);


                    Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                    Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                    var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                    await task;
                    if (!task.Result.result)
                    {
                        //SetTraficLights(0, 0, 1, 0);//red ight
                        MessageBox.Show("FOOTER WORK ERROR!", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        //btnFooterWork.Enabled = true;
                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        //btnFooterWork.Enabled = true;
                        //btnFooterWork1.Enabled = true;
                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InWork;
                        return false;

                    }
                    //Thread.Sleep(200);
                    //SetTraficLights(0, 1, 0, 0);//yellow/green
                    //btnFooterWork.Enabled = true;
                    //btnFooterWork1.Enabled = true;
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InWork;
                    //------weldone----------
                    await Task.Delay(2000);
                    inv.set(btnToWeldone, "Enabled", false);
                    ControlsEnable(false);
                    axis = 0;
                    if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                    if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());

                    Pos5 = master.Ax5_Weldone + upDwnFootWeldX.UpDownValue + weldone.weldX;// + (Single.Parse(txtPartLength.Text) - master.Length);
                    Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Pos2 = master.Ax2_Work + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                    Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; ;// master.Ax3_Work;
                                                                       //lamp1
                    lamp = new BitArray(new bool[8]);
                    lamp[0] = true; //lamp1
                    Lamps = new byte[1];
                    lamp.CopyTo(Lamps, 0);

                    speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                    speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                    var task1 = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                    await task1;
                    CommReply reply1 = task1.Result;
                    if (!reply1.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> ERROR MOVE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        //InspectStationAct.VisionInAction = false;
                        //MyStatic.bReset = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "Error move Footer to Weldon Position!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;

                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        MessageBox.Show("ERROR MOVE FOOTER to Weldone POSITION! ", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        inv.set(btnToWeldone, "Enabled", true);
                        ControlsEnable(true);
                        return false;
                    }
                    FooterStationAct.AxisInAction = false;
                    await Task.Delay(2000);
                }
                
                return true;
                    //while (!MyStatic.bReset && !MyStatic.bExitcycle)
                    //{

                    //    //string[] s = cmbAxes.Text.Split(':');
                    //    //Axis = int.Parse(s[0]);
                    //    int axis = Axis;
                    //    Single Speed = AxStatus[axis - 1].Vmax;
                    //    Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                    //    Single dist = b * Single.Parse(txtDistCycle.Text);
                    //    b = -b;

                    //    var task1 = Task.Run(() => MoveAbsSt(axis, 160 + dist, speed));
                    //    await task1;


                    //    CommReply reply = new CommReply();
                    //    reply.result = false;
                    //    reply = task1.Result;
                    //    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                    //    if (!(reply.status == "" || reply.status == null))
                    //    {
                    //        MessageBox.Show("ERROR MOVE RELATIVE! " + "\r" + reply.status);
                    //        ControlsEnable(true);

                    //        return false;
                    //    }
                    //    if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return false; };
                    //    Thread.Sleep((int)(Single.Parse(txtWaitCycle.Text) * 1000));
                    //}
                    return true;
                //Single dist = 0;
                //for (int i = 0; i <16 ;i++)
                //{

                //    //string[] s = cmbAxes.Text.Split(':');
                //    //Axis = int.Parse(s[0]);
                //    if (MyStatic.bReset || MyStatic.bExitcycle) break;
                //    int axis = 4;
                //    Single Speed = AxStatus[axis - 1].Vmax;
                //    Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                //    //Single dist = b * Single.Parse(txtDistCycle.Text);
                //    //b = -b;
                //    dist = dist + (360.0f / 16.0f);
                //    var task1 = Task.Run(() => MoveAbsSt(axis, dist, speed));
                //    await task1;


                //    CommReply reply = new CommReply();
                //    reply.result = false;
                //    reply = task1.Result;
                //    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                //    if (!(reply.status == "" || reply.status == null))
                //    {
                //        MessageBox.Show("ERROR MOVE RELATIVE! " + "\r" + reply.status);
                //        ControlsEnable(true);

                //        return false;
                //    }
                //    if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return false; };
                //    Thread.Sleep((int)(Single.Parse(txtWaitCycle.Text) * 1000));
                //}

                //ControlsEnable(true);
                //return true;

                //btnCycle.Enabled = true;
            }
            catch (Exception ex) { return false; }
        }

        private async void btnFooterHome_Click(object sender, EventArgs e)
        {
            try
            {
                if (FooterStationAct.AxisInAction) return;
                FooterStationAct.AxisInAction = true;
                btnFooterHome.Enabled = false;
                btnFooterHome1.Enabled = false;
                //SetTraficLights(0, 0, 0, 0);//yellow/green
                int axis = 0;
                Single speed5 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed4 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[3].Ax_Vmax) / 100.0f;
                var task = Task.Run(() => MoveFooterHome(speed5, speed4));
                await task.ConfigureAwait(true);
                if (!task.Result.result)
                {
                    //SetTraficLights(0, 0, 1, 0);//red ight
                    MessageBox.Show("FOOTER HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                }
                Thread.Sleep(200);
                //SetTraficLights(0, 1, 0, 0);//yellow/green
                btnFooterHome.Enabled = true;
                btnFooterHome1.Enabled = true;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InHome;
            }
            catch (Exception ex)
            {
                //SetTraficLights(0, 0, 1, 0);//red ight
                MessageBox.Show("ERROR IN PART DATA!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                btnFooterHome.Enabled = true;
                btnFooterHome1.Enabled = true;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InError;
            }
        }

        //public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        //{
        //    var task = (Task)@this.Invoke(obj, parameters);
        //    await task.ConfigureAwait(false);
        //    var resultProperty = task.GetType().GetProperty("Result");
        //    return resultProperty.GetValue(task);
        //}

        delegate CommReply MoveFooter(Single speed5, Single speed4);
        CommReply reply1 = new CommReply();
        public CommReply MoveFooterHomeInv1(Single speed5, Single speed4)
        {
            CommReply reply = new CommReply();
            reply1.result = false;
            reply1.data = null;
            MoveFooter MoveFooterHomeInv2 = MoveFooterHome;

            try
            {
                if (txtMess.InvokeRequired)
                {


                    MoveFooter caller = new MoveFooter(MoveFooterHome);
                    IAsyncResult res = this.BeginInvoke(caller, new object[] { speed5, speed4 });
                    reply = (CommReply)this.EndInvoke(res);

                }
                else
                {

                    reply = MoveFooterHome(speed5, speed4);

                }
                return reply;
            }
            catch (Exception ex) { return reply; }

        }
        private CommReply MoveFooterHomeInv(Single speed5, Single speed4)
        {
            //public System.Windows.Threading.DispatcherOperation InvokeAsync;
            CommReply reply = new CommReply();

            if (txtMess.InvokeRequired)




                txtMess.Invoke(new Action(delegate () { MoveFooterHome(speed5, speed4); }));

            else
            {

                reply = MoveFooterHome(speed5, speed4);
            }
            return reply;
        }
        private CommReply MoveFooterHome(Single speed5, Single speed4)
        {
            try
            {

                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;

                //if (!LoadPlcData()) return reply;


                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                int j = 0;
                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveFooterHome;
                ParmsPlc.SendParm[1] = 0;
                ParmsPlc.SendParm[3] = speed5;
                ParmsPlc.SendParm[4] = speed4;

                ParmsPlc.SendParm[10] = 15.0f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true, false);

                ParmsPlc.SendParm = null;

                if (!reply.result)
                {
                    return reply; ;

                }

                reply.result = true;
                //inv.settxt(txtMess, txtMess.Text + "fini" + "\r\n");
                return reply;

            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }

        private async void btnFooterWork_Click(object sender, EventArgs e)
        {
            try
            {
                //robot to home
                var task1 = Task.Run(() => RobotHome());
                await task1;

                bool rep1 = task1.Result;

                if (!rep1)
              
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                //footer to work
                if (FooterStationAct.AxisInAction) return;
                FooterStationAct.AxisInAction = true;
                btnFooterWork.Enabled = false;
                btnFooterWork1.Enabled = false;
                //SetTraficLights(0, 0, 0, 0);//
                int axis = 0;
                if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                {
                    MessageBox.Show("ERROR PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    btnFooterWork.Enabled = false;
                    FooterStationAct.AxisInAction = true;
                    return;
                }
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                Single Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);


                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                if (!task.Result.result)
                {
                    //SetTraficLights(0, 0, 1, 0);//red ight
                    MessageBox.Show("FOOTER WORK ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    btnFooterWork.Enabled = true;
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;

                }
                Thread.Sleep(200);
                //SetTraficLights(0, 1, 0, 0);//yellow/green
                btnFooterWork.Enabled = true;
                btnFooterWork1.Enabled = true;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InWork;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR IN PART DATA!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                btnFooterWork.Enabled = true;
                btnFooterWork1.Enabled = true;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InError;
            }
        }
        private CommReply MoveFooterWork(Single Pos5, Single speed, Single Pos1, Single Pos2, Single Pos4, Single speed1, Single Pos3, Single Lamps)//current position
        {
            try
            {
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;

                //if (!LoadPlcData()) return reply;


                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                int j = 0;
                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.MoveFooterWorkNoAir;
                ParmsPlc.SendParm[1] = 0;
                ParmsPlc.SendParm[2] = Pos5;
                ParmsPlc.SendParm[3] = speed;
                ParmsPlc.SendParm[4] = Pos1;
                ParmsPlc.SendParm[5] = Pos2;
                ParmsPlc.SendParm[6] = Pos4;
                ParmsPlc.SendParm[7] = speed1;
                ParmsPlc.SendParm[8] = Pos3;
                ParmsPlc.SendParm[9] = Lamps;
                ParmsPlc.SendParm[10] = 9.5f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);

                ParmsPlc.SendParm = null;

                if (!reply.result) 
                    return reply; ;
                Thread.Sleep(2);

                reply.result = true;
                return reply;

            }
            catch (Exception ex)
            {
                CommReply reply = new CommReply();
                reply.result = false;
                reply.comment = ex.Message;
                return reply;

            }
        }
        public bool SetTraficLights(int green = 0, int yellow = 0, int red = 0, int buzzer = 0)
        {

            try
            {
                lights.green = green;
                lights.yellow = yellow;
                lights.red = red;
                lights.buzzer = buzzer;
                return true;
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = 502;
                ParmsPlc.SendParm[1] = green;
                ParmsPlc.SendParm[2] = yellow;
                ParmsPlc.SendParm[3] = red;
                ParmsPlc.SendParm[4] = buzzer;

                ParmsPlc.SendParm[10] = 1f;//tmout
                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, false, false);

                ParmsPlc.SendParm = null;

                return reply.result;

            }
            catch (Exception ex) { return false; }
        }

        private async void button5_Click_1(object sender, EventArgs e)
        {
            //16x22.5
            try
            {
                ControlsEnable(false);
                //if (trackBarSpeedSt.Value > 10) trackBarSpeedSt.Value = 10;
                Single dist = 0;
                inv.set(button5, "Enabled", false);
                //Axis = int.Parse(cmbAxes.Text);
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                Axis = 4;
                int axis = Axis;
                Single Speed = AxStatus[axis - 1].Vmax;//7200;
                Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;
                //move 
                for (int i = 0; i < 17; i++)
                {
                    Thread.Sleep(2);
                    dist = 22.5f * i;
                    var task1 = Task.Run(() => MoveAbs(0, axis, dist, speed));
                    await task1;


                    CommReply reply = new CommReply();
                    reply.result = false;
                    reply = task1.Result;
                    inv.settxt(txtCurrPosCams, reply.data[4].ToString("0.000"));


                    if (!(reply.status == "" || reply.status == null))
                    {
                        MessageBox.Show("ERROR MOVE FINE! " + "\r" + reply.status);
                        ControlsEnable(true);
                        inv.set(button5, "Enabled", true);
                        return;
                    }
                    if (reply.data[1] != 0) { MessageBox.Show("ERROR MOVE"); return; };
                    //

                }
                MessageBox.Show("Time= " + sw.ElapsedMilliseconds / 1000f);

                ControlsEnable(true);
                inv.set(button5, "Enabled", true);
            }
            catch (Exception ex) { MessageBox.Show("ERROR MOVE FINE! "); inv.set(button5, "Enabled", true); }
        }

        private async void btnCheckWeldon_Click(object sender, EventArgs e)
        {
            try
            {
                if (InspectStationAct.WeldonInAction) return;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon<= Weldon Start" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                InspectStationAct.WeldonInAction = true;
                InspectStationAct.WeldonState = (int)MyStatic.E_State.Occupied;
                FooterStationAct.AxisInAction = true;
                inv.settxt(lblInspect, "Run Weldon");
                inv.set(btnCheckWeldon, "Enabled", false);
                //move to weldon ax5 and ax4
                int axis = 0;
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());

                Single Pos5 = master.Ax5_Weldone + upDwnFootWeldX.UpDownValue + weldone.weldX;// + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);

                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                CommReply reply1 = task.Result;
                if (!reply1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> ERROR MOVE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                    //InspectStationAct.VisionInAction = false;
                    //MyStatic.bReset = true;
                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    ErrorMess = "Error move Footer to Weldon Position!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;

                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    inv.set(btnCheckWeldon, "Enabled", true);
                    return;
                }

                //run weldon io-link
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon<= run measure" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                var task1 = Task.Run(() => RunWeldonFileInv_1());//io link data
                await task1;
                if (!task1.Result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> ERROR ROTATE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                    //InspectStationAct.VisionInAction = false;
                    //MyStatic.bReset = true;
                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    ErrorMess = "Error Rotate Footer to Weldon Position!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    inv.set(btnCheckWeldon, "Enabled", true);
                    return;
                }
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InWork;
                InspectStationAct.WeldonState = (int)MyStatic.E_State.WeldonFini;
                InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                if (InspectStationAct.OnFooterGrip3_PartID >= 0) partData[InspectStationAct.OnFooterGrip3_PartID].Weldone = weldone.angle;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon<= get measure results" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                //run weldon graph
                var task2 = Task.Run(() => RunWeldonFileInv_2());//graph
                await task2;
                CommReply reply = task2.Result;
                if (reply.result && reply.data != null)//weldon ok
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weeldon=> Weldon Data Ready " + reply.data[0].ToString("0.00") + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (Math.Abs(reply.data[0] - weldone.angle) < 20)
                    {

                        inv.settxt(lblInspect, "Weldon Ready");
                        InspectStationAct.WeldonState = (int)MyStatic.E_State.WeldonDataFini;
                        InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                        inv.settxt(lblInspect, "weldon Ready");
                        InspectStationAct.WeldonInAction = false;
                    }
                    else
                    {
                        inv.settxt(lblInspect, "Weldon Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Weeldon=> Weldon Reject " + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.WeldonState = (int)MyStatic.E_State.RejectWeldon;
                        InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Weldone] = true;
                        InspectStationAct.WeldonInAction = false;

                    }
                }
                else
                {
                    inv.settxt(lblInspect, "Weldon Reject");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weeldon=> Weldon Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.WeldonState = (int)MyStatic.E_State.RejectWeldon;
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Weldone] = true;
                    InspectStationAct.WeldonInAction = false;

                }
                inv.set(btnCheckWeldon, "Enabled", true);



            }
            catch (Exception ex) { MessageBox.Show("ERROR CHECK WELDON! " + ex.Message); inv.set(btnCheckWeldon, "Enabled", true); }
        }
        private WebComm.CommReply StartCycleInspectWeldone()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                //set lights
                //CommReply rep = new CommReply();
                //Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                //reply.result = false;
                //Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                //for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                //ParmsPlc.SendParm[0] = MyStatic.CamsCmd.Lamps;
                //ParmsPlc.SendParm[2] = 2;
                //ParmsPlc.SendParm[3] = 2;
                //ParmsPlc.SendParm[4] = 1;


                //ParmsPlc.SendParm[10] = 15f;//tmout
                //rep= Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc);

                //ParmsPlc.SendParm = null;


                //move footer to work
                FooterStationAct.AxisInAction = true;

                int axis = 0;
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                Single Pos5 = master.Ax5_Weldone + upDwnFootWeldX.UpDownValue + weldone.weldX;// + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; // master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);

                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                CommReply reply1 = MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]);

                if (!reply1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                    //InspectStationAct.VisionInAction = false;
                    //MyStatic.bReset = true;
                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    ErrorMess = "ERROR MOVE FOOTER TO WELDON POSIION!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                    //MessageBox.Show("ERROR MOVE FOOTER TO weldone POSITION! Exit cycle", "ERROR", MessageBoxButtons.OK,
                    //           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    reply.result = false;
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    return reply;
                }
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InWork;
                //
                WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.CheckWeldone).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckWeldone;
                Parms.comment = "Start Cycle Weldon";
                Parms.timeout = 20;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CheckWeldone;
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = 20.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = WC1.RunCmd(Parms);
                Single nCode = 0;
                if (rep1.result)
                {

                }
                else
                {

                    MessageBox.Show("Vision COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return rep1;
                }

                //send parameters


                //reply.result = true;
                return rep1;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Vision COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }

        private async void btnOpenGrippers_Click(object sender, EventArgs e)
        {
            try
            {
                inv.set(btnOpenGrippers, "Enabled", false);
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                while (!MyStatic.bReset)
                {
                    FW1.ReadFanucIOAsync("dout", "0", "get", "0", 500);

                    Thread.Sleep(50);
                    RobotFunctions.CommReply reply = FW1.ReadFanucS2RegAsync(500);


                    if (reply.result && reply.data[1] == 1) reply.result = true;
                    else
                    {
                        inv.set(btnOpenGrippers, "Enabled", true);
                        sw.Stop();
                        return;
                    }
                    Thread.Sleep(200);
                    if (sw.ElapsedMilliseconds > 30000) break;
                }
                inv.set(btnOpenGrippers, "Enabled", true);
                sw.Stop();
            }
            catch (Exception ex) { inv.set(btnOpenGrippers, "Enabled", true); swcycle.Stop(); }
        }

        private async void btnCheckDiam_Click(object sender, EventArgs e)
        {
            try
            {
                //if (chkVisionSim.Checked) return;
                inv.set(btnRead, "Enabled", false);
                inv.set(btnCheckDiam, "Enabled", false);
                //WC1.SetControls1(txtClient, this, null, "Vision Start Inspect  Diameter", CameraAddr);

                var task1 = Task.Run(() => StartCycleInspectDiam());
                await task1;
                WebComm.CommReply reply = task1.Result;
                if (reply.result)
                {

                    inv.set(btnCheckDiam, "Enabled", true);
                }
                else
                {
                    MessageBox.Show("ERROR CHECK DIAMETER! ");
                }

                inv.set(btnCheckDiam, "Enabled", true);
                //if (bStop) break;
                Thread.Sleep(2);
                //}
            }
            catch (Exception ex) { MessageBox.Show("ERROR CHECK DIAMETER! " + ex.Message); inv.set(btnCheckDiam, "Enabled", true); }
        }
        int nDiameterCheckUpDwn = 2;
        int nFrontCountUpDwn = 0;
        int nColorUpDwn = 0;
        Single Cam1xFocusOffset = 0;
        //Double kRightEdge = 1; 
        Single VisionDiam = 0;
        private async Task<WebComm.CommReply> StartCycleInspectDiam()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            WebComm.CommReply rep1 = new WebComm.CommReply();
            VisionDiam = 0;
            rep1.result = false;
            rep1.data = new Single[10];
            try
            {
                nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
                
                //move footer to diam
                int axis = 0;
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                Single Pos5 = master.Ax5_Diam + upDwnFootDX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work;// + upDwnCam2z.UpDownValue - (Single.Parse(txtPartDiam.Text) - master.Diameter)/2.0f;
                Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[2] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);

                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 =  (int.Parse(txtSpeedSt.Text) / 100.0f);
                FooterStationAct.AxisInAction = true;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff<= move footer to diam position" + "pos=" + Pos5.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                CommReply reply1 = task.Result;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> move footer fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                if (!reply1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER TO Diam POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff<= move footer2 axis=" + axis.ToString() + "pos=" + Pos5.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    var task2 = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                    await task2;
                    CommReply reply2 = task2.Result;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> move footer fini2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (!reply2.result)
                    {

                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ERROR MOVE FOOTER TO DIAMETER POSITION!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        
                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        reply.result = false;
                        return reply;
                    }
                }
                Thread.Sleep(200);
                //read footer rot coord
                axis = 4;
                var task6 = Task.Run(() => RunCurrPosCams(axis));

                await task6;
                CommReply reply3 = new CommReply();
                reply3.result = false;
                reply3 = task6.Result;

                if (!(reply3.status == "" || reply3.status == null))
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR READ ROTATION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    //MyStatic.bReset = true;
                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    ErrorMess = "ERROR READ ROTATION!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    reply.result = false;
                    return reply;
                }

                Single curr_rot = reply3.data[5];

                //
                

                Single rot = 0;
                Single[] diam = new Single[nDiameterCheckUpDwn+3];
                Single[] RightEdge = new Single[nDiameterCheckUpDwn+3];
                int nDiameterCheckTest = nDiameterCheckUpDwn;
                for (int i = 0; i < nDiameterCheckUpDwn; i++)
                {

                    if (curr_rot > 265) //rotate back
                        rot = 270 - (90 * i);
                    else rot = (90 * i);
                    speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[3].Ax_Vmax) / 100.0f;
                    var task1 = Task.Run(() => MoveAbs(0, 4, rot, speed));
                    await task1;
                    CommReply rep2 = task1.Result;
                    if (!rep2.result)
                    {


                        //MessageBox.Show("AX4 Motion Error!", "ERROR", MessageBoxButtons.OK,
                        //                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER ROTATION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                        //MyStatic.bReset = true;
                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ERROR FOOTER ROTATION!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        reply.result = false;
                        return reply;
                    }

                    WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                    WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                    Parms.cmd = ((int)MyStatic.InspectCmd.CheckDiam).ToString();
                    Parms.DebugTime = 1000;
                    Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckDiam;
                    Parms.comment = "Start Cycle Diameter";
                    Parms.timeout = 20;
                    Array.Resize<Single>(ref Parms.SendParm, 3);
                    Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CheckDiam;
                    Parms.SendParm[1] = 1;// general speed
                    Parms.SendParm[2] = 20.0f;// 0.5f;//timeout
                    rep1 = WC1.RunCmd(Parms);
                    diam[i] = 0;
                    RightEdge[i] = 0;

                    if (rep1.result)
                    {

                        string[] temp = rep1.comment.Split(',');
                        if (temp.Length == 5 && temp[0].Trim() == "cmd91" && temp[1].Trim() == "1")
                        {
                            diam[i] = Single.Parse(temp[2]);
                            RightEdge[i] = Single.Parse(temp[3]);
                            if (diam[i] == null || Math.Abs(diam[i] - Single.Parse(txtPartDiam.Text)) > 2.0f)
                            {
                                nDiameterCheckUpDwn++;
                                if (nDiameterCheckUpDwn > nDiameterCheckTest + 2)
                                {
                                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR0 DIAMETER" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                                   
                                    FooterStationAct.AxisInAction = false;
                                    FooterStationAct.State = (int)MyStatic.E_State.InWork;
                                    
                                    VisionDiam = 0;
                                    rep1.data = new float[2];
                                    rep1.data[0] = 0;
                                    rep1.data[1] = 0;
                                    
                                    rep1.result = false;
                                    return rep1;
                                }
                            }
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> DIAMETER=" + temp[2] + " RightEdge = " + temp[3] + " n =" + nDiameterCheckUpDwn.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            if (i == 0) { inv.settxt(lblD1, diam[0].ToString()); inv.settxt(lblOf1, RightEdge[0].ToString()); }
                            else if (i == 1) { inv.settxt(lblD2, diam[1].ToString()); inv.settxt(lblOf2, RightEdge[1].ToString()); }
                        }
                    }
                    else
                    {

                        
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR2 DIAMETER" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                       

                    }
                    Thread.Sleep(20);
                }
                Thread.Sleep(200);
                Single D = 0;
                Single Fr = 0;
                int n = 0;
                for (int i = 0; i < nDiameterCheckUpDwn; i++)
                {
                    
                    if (diam[i] != 0 && RightEdge[i] != 0 && Math.Abs(diam[i] - Single.Parse(txtPartDiam.Text)) < 2.0f)
                    {
                        D = D + diam[i];
                        Fr = Fr + RightEdge[i];
                        n++;
                    }
                }
                if (n > 0)
                {
                    D = D / n;// (Single)nDiameterCheck;//check if part exist
                    Fr = Fr / n;// (Single)nDiameterCheck;
                }
                else
                {
                    Fr = 0;
                    D = 0;
                }
                rep1.data = new Single[10];
                if (InspectStationAct.OnFooterGrip3_PartID >= 0) partData[InspectStationAct.OnFooterGrip3_PartID].Diam = D;
                inv.settxt(lblD, D.ToString()); inv.settxt(lblOf, Fr.ToString());
                //Single D = (diam[0] + diam[1] + diam[2] + diam[3]) / (Single)nDiameterCheck;//check if part exist
                if (Math.Abs(D - Single.Parse(txtPartDiam.Text)) > Single.Parse(txtDtolerance.Text))
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR DIAMETER Av=" + D.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //MyStatic.bExitcycleNow = true;
                    //ErrorMess = "ERROR DIAMETER!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";
                    //FooterStationAct.AxisInAction = false;
                    //FooterStationAct.State = (int)MyStatic.E_State.InWork;
                    //rep.result = false;
                    //return rep1;

                    //
                    Cam1xFocusOffset = -(Single)((Fr - master.Ax5_RightEdge) * master.kRightEdge);
                    //Cam1x_fosus = Cam1x_master_focus + Cam1xFocusOffset;
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InWork;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> DIAMETER Av =" + D.ToString() + " RightEdge = " + Fr.ToString() + " n =" + nDiameterCheckUpDwn.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    VisionDiam = D;
                    rep1.data[0] = D;
                    rep1.data[1] = Cam1xFocusOffset;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> DIAMETER Av=" + rep1.data[0].ToString() + " Cam1xFocusOffset = " + rep1.data[1].ToString() + " n =" + nDiameterCheckUpDwn.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    rep1.result = false;
                    return rep1;
                }


                //reply.result = true;
                //calculate coefficient for vision right edge according to master
                //kRightEdge = 1.0f;
                //D=16 mm
                //L=92 mm
                //diam measure position M5x=49.1 right edge=40.59415 master position
                //diam measure position M5x=59.1 right edge=30.368
                //diam measure position M5x=44.1 right edge=45.730
                //kRightEdge = 15.0f / (45.730 - 30.368);

                //if  right edge=30.368
                Cam1xFocusOffset = -(Single)((Fr - master.Ax5_RightEdge) * master.kRightEdge);
                //Cam1x_fosus = Cam1x_master_focus + Cam1xFocusOffset;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InWork;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> DIAMETER Av =" + D.ToString() + " RightEdge = " + Fr.ToString() + " n =" + nDiameterCheckUpDwn.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                rep1.data[0] = D;
                rep1.data[1] = Cam1xFocusOffset;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> DIAMETER Av=" + rep1.data[0].ToString() + " Cam1xFocusOffset = " + rep1.data[1].ToString() + " n =" + nDiameterCheckUpDwn.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                return rep1;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Vision COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }

        private async void btnRobotStart_Click(object sender, EventArgs e)
        {
            try
            {
                btnRobotStart.Enabled = false;
                ControlsEnable(false);
                var task = Task.Run(() => RobotStart());
                await task;
                bool rep = task.Result;
                if (rep)
                {
                    ControlsEnable(true);
                    btnRobotStart.Enabled = true;

                    SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    inv.set(panel1, "BackColor", SystemColors.Control);
                }
                else
                {
                    ControlsEnable(true);
                    btnRobotStart.Enabled = false;
                    inv.set(btnRobotStart, "BackColor", Color.LightGray);
                    inv.set(panel1, "BackColor", Color.Gray);
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                }
            }
            catch (Exception ex)
            {
                ControlsEnable(true); btnRobotStart.Enabled = false; SetTraficLights(0, 0, 1, 0); inv.set(panel1, "BackColor", Color.Gray);
                MessageBox.Show("ERROR START ROBOT !" + ex.Message, "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private int WebSocketTest()
        {
            try
            {
                MyStatic.bExitcycle = false;
                int n = 0;
                string[] temp = new string[1000];
                while (!MyStatic.bReset && !MyStatic.bExitcycle)
                {
                    Thread.Sleep(10);
                    n++;
                    //inv.settxt(txtClient, txtClient.Text + "\r\n" + "start"+"  //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://169.254.152.130");
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Proxy = null;
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    httpWebRequest.ServicePoint.UseNagleAlgorithm = false;
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        //string json = "{\"user\":\"test\"," +
                        //              "\"password\":\"bla\"}";
                        //json = "{\"code\":10," + "\"cid\":4711,\"adr\": \"/deviceinfo/productcode/getdata\"}";
                        //json = "{\"code\":10," + "\"cid\":4711,\"adr\": \"/iolinkmaster/port[2]/iolinkdevice/pdin/getdata\"}";
                        string json = "{\"code\":10," + "\"cid\":4711,\"adr\": \"/iolinkmaster/port[3]/iolinkdevice/pdin/getdata\"}";

                        streamWriter.Write(json);

                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                    //{
                    //"code" : "request",
                    //"cid" : number,
                    //"adr" : service_adr,
                    //"reply" : "reply_data",
                    //" data" : "serialized_data",
                    //"auth" : { authinfo})
                    //}
                    // {
                    // "code" : "request",
                    //"cid" : number,
                    //"adr" : service_adr,
                    //"reply" : "reply_data",
                    //" data" : "serialized_data",
                    //"auth" : { authinfo})
                    //}
                    //                {
                    //                    "code":"code_id", "cid":id, adr":"data_point / service", "data":{req_data},
                    //"auth":{ "user":"usr_id","passwd":"password"}
                    //                }
                    //{ "code":10, "cid":4711,"adr":"/deviceinfo/productcode/getdata"}
                    //{ "code": "request", "cid": -1, "adr": "/iolinkmaster/port[2]/datastorage/getblobdata", "data": { "pos": 0, "length": h} }
                    //{
                    //                    "code":"request",
                    //"cid":4711,
                    //"adr":"/iolinkmaster/port[2]/iolinkdevice/pdin/getdata"
                    //}


                    //smart sensor
                    //byte0-quality 0-100%
                    //byte2.3- D0  byte1.7 D12
                    //byte 2.0 out1
                    //byte 2.1 out2
                    //byte 2.2 quality
                    //inv.settxt(txtClient, txtClient.Text + "\r\n" +"read"+ "  //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //inv.settxt(txtClient, txtClient.Text + "\r\n" + "read1" + "  //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        httpResponse.Close();
                        streamReader.Close();
                        streamReader.Dispose();


                        int i = result.ToString().IndexOf("value");
                        int ii = result.ToString().IndexOf("code");
                        string ss = result.ToString().Substring(i + 5, ii - i - 5);

                        ss = ss.Replace("{", string.Empty);
                        ss = ss.Replace("}", string.Empty);
                        ss = ss.Replace("\"", string.Empty);
                        ss = ss.Replace(",", string.Empty);
                        ss = ss.Replace(":", string.Empty);
                        // int a=int.Parse("0x"+ss);
                        int intValue = Convert.ToInt32("0x" + ss, 16);
                        byte[] intBytes = BitConverter.GetBytes(intValue);
                        BitArray b = new BitArray(new int[] { intValue });//1001 10011001 11010000

                        string binary = Convert.ToString(intValue, 2);//10011001101010111100
                        string binary1 = binary.Remove(binary.Length - 3, 3);  //Remove the exact bit, 3rd in this case
                        string binary2 = binary1.Substring(binary1.Length - 13);//1001101010111
                        string binary3 = binary1.Substring(0, binary1.Length - binary2.Length);
                        int newValue = Convert.ToInt32(binary2, 2);
                        int newQuality = Convert.ToInt32(binary3, 2);

                        //inv.settxt(txtClient, txtClient.Text + "\r\n" + n.ToString("000")+"  "+newValue.ToString("0000") + "  " + newQuality.ToString("00") + "  " + DateTime.Now.ToString("HH:mm:ss.fff"));
                        //temp[n-1] = n.ToString("000") + "  " + newValue.ToString("0000") + "  " + newQuality.ToString("00") + "  " + DateTime.Now.ToString("HH:mm:ss.fff");
                        inv.settxt(label99, newValue.ToString("0000"));
                        inv.settxt(label98, newQuality.ToString("00"));
                        //inv.settxt(txtClient, txtClient.Text + "\r\n" + "  //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                        //return intValue;

                    }
                }
                //for (int i = 0; i < n; i++) { inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[i]); Thread.Sleep(1); }
                return 0;

            }
            catch (Exception ex) { return 0; }
        }

        private int BeckhofSensorRead()
        {
            try
            {
                //MyStatic.bExitcycle = false;
                int n = 0;
                string[] temp = new string[1000];
                CommReply rep = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = 600;
                ParmsPlc.SendParm[10] = 1f;//tmout

                PlcReply GetParams = new PlcReply();
                PlcReply SendPar = new PlcReply();

                Array.Resize<Single>(ref GetParams.data, ParmsPlc.SendParm.Length);
                Array.Resize<Single>(ref SendPar.data, ParmsPlc.SendParm.Length);
                SendPar.data = ParmsPlc.SendParm;
                SendPar.StartAddress = StartAddressSendReadIOlink + 46; //offset for read
                SendPar.status = -1;
                while (!MyStatic.bReset && !MyStatic.bExitcycle)
                {
                    Thread.Sleep(10);
                    n++;
                    //read beckhoff


                    //bwWaitPlc_DoWork(SendPar, ref GetParams);
                    GetParams = Beckhoff_IOlink.WaitPlcReply(SendPar);

                    int newValue = 0;
                    int newQuality = 0;
                    Single newAngle = 0;

                    if (GetParams.status == -1)
                    {

                    }
                    else
                    {
                        newValue = Convert.ToInt32(GetParams.data[3]);
                        newQuality = Convert.ToInt32(GetParams.data[2]);
                        newAngle = GetParams.data[4];
                    }

                    inv.settxt(label99, newValue.ToString("0000"));
                    inv.settxt(label98, newQuality.ToString("00"));


                }

                //for (int i = 0; i < n; i++) { inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[i]); Thread.Sleep(1); }
                return 0;

            }
            catch (Exception ex) { return 0; }
        }

        //static System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();

        int nn = 0;
        string[] temp = new string[1000];
        private async void timer1_Tick(object sender, EventArgs e)
        {

            //timer1.Stop();
            ////////////////////
            try
            {
                //read beckhoff
                CommReply rep = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = 600;
                ParmsPlc.SendParm[10] = 1f;//tmout

                PlcReply GetParams = new PlcReply();
                PlcReply SendPar = new PlcReply();

                Array.Resize<Single>(ref GetParams.data, ParmsPlc.SendParm.Length);
                Array.Resize<Single>(ref SendPar.data, ParmsPlc.SendParm.Length);
                SendPar.data = ParmsPlc.SendParm;
                SendPar.StartAddress = StartAddressSendReadIOlink + 46; //offset for read
                SendPar.status = -1;

                //bwWaitPlc_DoWork(SendPar, ref GetParams);
                var task1 = Task.Run(() => Beckhoff_IOlink.WaitPlcReply(SendPar));
                await task1;
                int newValue = 0;
                int newQuality = 0;
                Single newAngle = 0;
                GetParams = task1.Result;
                if (GetParams.status == -1)
                {

                }
                else
                {
                    nn++;
                    newValue = Convert.ToInt32(GetParams.data[3]);
                    newQuality = Convert.ToInt32(GetParams.data[2]);
                    newAngle = GetParams.data[4];
                    temp[nn - 1] = nn.ToString("000") + "  " + newValue.ToString("0000") + "  " + newQuality.ToString("00") + "  " + DateTime.Now.ToString("HH:mm:ss.fff");
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[nn - 1]); Thread.Sleep(1);
                    string[] s1 = temp[nn - 1].Split(' ');
                    //if (CharWeldon.ChartAreas[0].AxisX.Maximum > Xscale) 
                    CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn - 1;
                    if (s1.Length > 6)
                    {
                        series.Points.Add(newValue);
                        series1.Points.Add(newQuality * 100);

                    }
                }


                return;
                ///----------------------------------------------------------------------------------------------------


                //read io-link ifm box
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://169.254.152.130");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Proxy = null;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.ServicePoint.UseNagleAlgorithm = false;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"code\":10," + "\"cid\":4711,\"adr\": \"/iolinkmaster/port[2]/iolinkdevice/pdin/getdata\"}";

                    streamWriter.Write(json);

                    streamWriter.Close();
                    streamWriter.Dispose();
                }


                //smart sensor
                //byte0-quality 0-100%
                //byte2.3- D0  byte1.7 D12
                //byte 2.0 out1
                //byte 2.1 out2
                //byte 2.2 quality

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();


                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    httpResponse.Close();
                    streamReader.Close();
                    streamReader.Dispose();


                    int i = result.ToString().IndexOf("value");
                    int ii = result.ToString().IndexOf("code");
                    string ss = result.ToString().Substring(i + 5, ii - i - 5);

                    ss = ss.Replace("{", string.Empty);
                    ss = ss.Replace("}", string.Empty);
                    ss = ss.Replace("\"", string.Empty);
                    ss = ss.Replace(",", string.Empty);
                    ss = ss.Replace(":", string.Empty);
                    // int a=int.Parse("0x"+ss);
                    int intValue = Convert.ToInt32("0x" + ss, 16);
                    byte[] intBytes = BitConverter.GetBytes(intValue);
                    BitArray b = new BitArray(new int[] { intValue });//1001 10011001 11010000

                    string binary = Convert.ToString(intValue, 2);//10011001101010111100
                    string binary1 = binary.Remove(binary.Length - 3, 3);  //Remove the exact bit, 3rd in this case
                    string binary2 = binary1.Substring(binary1.Length - 13);//1001101010111
                    string binary3 = binary1.Substring(0, binary1.Length - binary2.Length);
                    newValue = Convert.ToInt32(binary2, 2);
                    newQuality = Convert.ToInt32(binary3, 2);

                    temp[nn - 1] = nn.ToString("000") + "  " + newValue.ToString("0000") + "  " + newQuality.ToString("00") + "  " + DateTime.Now.ToString("HH:mm:ss.fff");
                    inv.settxt(label99, newValue.ToString("0000"));
                    inv.settxt(label98, newQuality.ToString("00"));


                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[nn - 1]); Thread.Sleep(1);
                string[] s = temp[nn - 1].Split(' ');
                if (CharWeldon.ChartAreas[0].AxisX.Maximum > Xscale) CharWeldon.ChartAreas[0].AxisX.Maximum = nn - 1;
                if (s.Length > 6)
                {
                    series.Points.Add(Single.Parse(s[2]));
                    series1.Points.Add(Single.Parse(s[4]) * 100);

                }


            }
            catch (Exception ex) { }
            ///////////////////////////////

        }
        public void ReadIOlinkInv()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => ReadIOlink()));

            }
            else
            {
                ReadIOlink();

            }
        }
        private bool ReadIOlink()
        {

            //timer1.Stop();
            ////////////////////
            try
            {
                //read beckhoff
                CommReply rep = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = 600;
                ParmsPlc.SendParm[10] = 1f;//tmout

                //PlcReply GetParams = new PlcReply();
                PlcReply SendPar = new PlcReply();

                //Array.Resize<Single>(ref GetParams.data, ParmsPlc.SendParm.Length);
                Array.Resize<Single>(ref SendPar.data, ParmsPlc.SendParm.Length);
                SendPar.data = ParmsPlc.SendParm;
                SendPar.StartAddress = StartAddressSendReadIOlink + 46; //offset for read
                SendPar.status = -1;
                int idata = 0;
                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Minimum = 0));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Maximum = 370));

                }
                else
                {
                    CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                    CharWeldon.ChartAreas[0].AxisX.Maximum = 370;

                }

                //CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                while (!MyStatic.bExitcycle && !MyStatic.bReset)
                {
                    Thread.Sleep(1);
                    ParmsPlc.SendParm[0] = 600;//curr pos
                    ParmsPlc.SendParm[1] = idata;//cam
                    ParmsPlc.SendParm[10] = 5f;//tmout

                    rep = Beckhoff_IOlink.PlcSendCmd(StartAddressSendReadIOlink, ParmsPlc, true);
                    //var task1 = Task.Run(() => Beckhoff_IOlink.WaitPlcReply(SendPar));

                    int newValue = 0;
                    int newQuality = 0;
                    Single newAngle = 0;
                    //GetParams = task1.Result;

                    idata = idata + 2;
                    if (!rep.result)//(GetParams.status == -1)
                    {

                    }
                    else
                    {
                        if (rep.data[4] >= 0 && rep.data[4] <= 360)
                        {
                            nn++;
                            newValue = Convert.ToInt32(rep.data[3]);
                            newQuality = Convert.ToInt32(rep.data[2]);
                            newAngle = rep.data[4];
                            temp[nn - 1] = nn.ToString("000") + "  " + newValue.ToString("0000") + "  " + newQuality.ToString("00") + "  " + newAngle.ToString("000.0") + "  " + DateTime.Now.ToString("HH:mm:ss.fff");
                            inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[nn - 1]); Thread.Sleep(1);
                            string[] s1 = temp[nn - 1].Split(' ');
                            //if (CharWeldon.ChartAreas[0].AxisX.Maximum > Xscale) 
                            //CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn - 1;
                            //CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                            if (s1.Length > 6)
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke(new Action(() => series.Points.AddXY(Math.Round(newAngle), newValue)));
                                    this.Invoke(new Action(() => series1.Points.AddXY(Math.Round(newAngle), newQuality * 100)));
                                    //series.Points.AddXY(Math.Round(newAngle), newValue);
                                }
                                else
                                {
                                    series.Points.AddXY(Math.Round(newAngle), newValue);
                                    series1.Points.AddXY(Math.Round(newAngle), newQuality * 100);
                                }


                            }
                            newAngle = rep.data[8];
                            if (newAngle >= 359)
                                return true;
                        }
                        if (rep.data[8] > 0 && rep.data[8] <= 360)
                        {
                            nn++;
                            newValue = Convert.ToInt32(rep.data[7]);
                            newQuality = Convert.ToInt32(rep.data[6]);
                            newAngle = rep.data[8];
                            temp[nn - 1] = nn.ToString("000") + "  " + newValue.ToString("0000") + "  " + newQuality.ToString("00") + "  " + newAngle.ToString("000.0") + "  " + DateTime.Now.ToString("HH:mm:ss.fff");
                            inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[nn - 1]); Thread.Sleep(1);
                            string[] s1 = temp[nn - 1].Split(' ');
                            //if (CharWeldon.ChartAreas[0].AxisX.Maximum > Xscale) 
                            //CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn - 1;
                            //CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                            if (s1.Length > 6)
                            {
                                if (InvokeRequired)
                                {
                                    this.Invoke(new Action(() => series.Points.AddXY(Math.Round(newAngle), newValue)));
                                    this.Invoke(new Action(() => series1.Points.AddXY(Math.Round(newAngle), newQuality * 100)));
                                    //series.Points.AddXY(Math.Round(newAngle), newValue);
                                }
                                else
                                {
                                    series.Points.AddXY(Math.Round(newAngle), newValue);
                                    series1.Points.AddXY(Math.Round(newAngle), newQuality * 100);
                                }


                            }
                        }
                        newAngle = rep.data[8];
                        if (newAngle >= 359)
                            return true;
                    }
                    if (newAngle >= 359)
                        return true;
                }


                return true;
            }

            catch (Exception ex) { return false; }
            ///////////////////////////////

        }
        Series series = new Series();
        Series series1 = new Series();
        Series series2 = new Series();
        private async void button6_Click(object sender, EventArgs e)
        {


            try
            {
                inv.set(button6, "Enabled", false);
                MyStatic.bExitcycle = false;
                CommReply rep = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                while (!MyStatic.bExitcycle && !MyStatic.bReset)
                {
                    Thread.Sleep(1);
                    ParmsPlc.SendParm[0] = 602;//curr pos
                    ParmsPlc.SendParm[10] = 5f;//tmout

                    var task1 = Task.Run(() => Beckhoff_IOlink.PlcSendCmd(StartAddressSendReadIOlink, ParmsPlc, true));
                    //var task1 = Task.Run(() => Beckhoff_IOlink.WaitPlcReply(SendPar));
                    await task1;
                    rep = task1.Result;
                    if (rep.result && rep.data.Length >= 5)
                    {
                        inv.settxt(label98, rep.data[2].ToString("00"));
                        inv.settxt(label99, (rep.data[3] / 100.0).ToString("00.00"));
                        inv.settxt(label103, rep.data[4].ToString("0.00"));
                    }

                    Thread.Sleep(100);
                }

                inv.set(button6, "Enabled", true);


            }
            catch (Exception ex) { inv.set(button6, "Enabled", true); }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {
                MyStatic.bExitcycle = true;
                inv.set(button7, "Enabled", false);
                var task1 = Task.Run(() => RunWeldonFileInv_1());//io link data
                await task1;
                var task2 = Task.Run(() => RunWeldonFileInv_2());//graph
                await task2;
                inv.set(button7, "Enabled", true);

            }
            catch (Exception ex) { button7.Enabled = true; }
        }

        private async Task<bool> RunWeldonFileInv_1()
        {
            try
            {
                //inv.set(button7, "Enabled", false);
                inv.settxt(txtWeldon, "Start weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                //----------------delete file--------------------------
                string filename = @"C:\Project\DataTest1.txt";
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                //rot to 0
                inv.set(lblChar, "AutoSize", true);
                inv.set(lblChar, "Visible", false);
                inv.set(CharWeldon, "Visible", false);



                int axis = 4;
                Single Speed = AxStatus[axis - 1].Vmax;//7200;
                Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;
                //Xscale = 370 * 5 / Single.Parse(txtSpeedSt.Text);
                //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + " rotate -5 weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Single dist = -5;
                CommReply b = MoveAbs(0, axis, dist, Speed / 2);

                if (!b.result)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Error move -5 weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    //inv.set(button6, "Enabled", true);
                    return false;
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini rotate -5 weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                Thread.Sleep(100);
                nn = 0;
                //MyStatic.bExitcycle = false;
                dist = 365;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "rotate 365 weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Single speed1 = Speed * 0.2f;//20%
                CommReply rep = new CommReply();
                var task3 = Task.Run(() => MoveAbs(0, axis, dist, speed1));

                //var task = Task.Run(() => BeckhofSensorRead());
                //await task;

                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = 601;
                ParmsPlc.SendParm[10] = 20f;//tmout
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Run io link weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                rep = Beckhoff_IOlink.PlcSendCmd(StartAddressSendReadIOlink, ParmsPlc, true);
                //var task1 = Task.Run(() => Beckhoff_IOlink.WaitPlcReply(SendPar));

                if (!rep.result) return false;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini rotate 365 weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                await task3;
                rep = task3.Result;
                if (!rep.result)
                    return false;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini IO link weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));


                if (!rep.result)
                    return false;


                return true;
            }
            catch (Exception ex)
            {
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + ex.ToString() + " //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                return false;
            }
        }
        private CommReply RunWeldonFileInv_3()
        {
            CommReply reply = new CommReply();
            try
            {
                //inv.set(button7, "Enabled", false);
                reply.result = false;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Start read weldon data //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                //----------------delete file--------------------------
                string filename = @"C:\Project\DataTest1.txt";

                inv.set(lblChar, "AutoSize", true);
                inv.set(lblChar, "Visible", false);
                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.Visible = false));
                    this.Invoke(new Action(() => CharWeldon.Series.Clear()));
                    this.Invoke(new Action(() => CharWeldon.Titles.Clear()));

                    this.Invoke(new Action(() => CharWeldon.Palette = ChartColorPalette.Excel));

                    this.Invoke(new Action(() => series = CharWeldon.Series.Add("Distance")));
                    this.Invoke(new Action(() => series1 = CharWeldon.Series.Add("Quality")));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].Color = Color.Orange));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].Color = Color.Blue));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].MarkerSize = 1));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].MarkerSize = 1));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].BorderWidth = 2));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].BorderWidth = 2));

                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.X = 0));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Y = 90));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Width = 12));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Height = 20));

                    //series.Points.AddXY(Math.Round(newAngle), newValue);
                }
                else
                {
                    CharWeldon.Visible = false;
                    CharWeldon.Series.Clear();
                    CharWeldon.Titles.Clear();
                    CharWeldon.Palette = ChartColorPalette.Excel;

                    series = CharWeldon.Series.Add("Distance");
                    series1 = CharWeldon.Series.Add("Quality");
                    CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine;
                    CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine;
                    CharWeldon.Series["Distance"].Color = Color.Orange;
                    CharWeldon.Series["Quality"].Color = Color.Blue;
                    CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None;
                    CharWeldon.Series["Distance"].MarkerSize = 1;
                    CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None;
                    CharWeldon.Series["Quality"].MarkerSize = 1;
                    CharWeldon.Series["Distance"].BorderWidth = 2;
                    CharWeldon.Series["Quality"].BorderWidth = 2;

                    CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);
                    CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                    CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false;
                    CharWeldon.Legends[0].Position.Auto = false;
                    CharWeldon.Legends[0].Position.X = 0;
                    CharWeldon.Legends[0].Position.Y = 90;
                    CharWeldon.Legends[0].Position.Width = 12;
                    CharWeldon.Legends[0].Position.Height = 20;


                }


                Thread.Sleep(100);


                int newValue = 0;
                int newQuality = 0;
                Single newAngle = 0;

                //--------------read file------------------
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                while (!File.Exists(filename) && !MyStatic.bReset)
                {
                    if (sw.ElapsedMilliseconds > 10000) break;
                    Thread.Sleep(100);
                }
                if (sw.ElapsedMilliseconds > 10000 || MyStatic.bReset)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File not exist weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    sw.Stop();
                    return reply;
                }

                StreamReader sr = new StreamReader(filename);
                int n = 0;
                nn = 0;
                sw.Restart();
                String strAll = "";
                while (!sr.EndOfStream)
                {
                    try
                    {
                        temp[nn] = sr.ReadLine();
                        //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[nn]); //Thread.Sleep(1);
                        strAll = strAll + "\r\n" + temp[nn];
                        string[] s1 = temp[nn].Split(' ');
                        newValue = (int)Single.Parse(s1[1]);
                        newQuality = (int)Single.Parse(s1[0]);
                        newAngle = Single.Parse(s1[2]);
                        if (s1.Length >= 4)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series.Points.AddXY(Math.Round(newAngle), newValue)));
                                this.Invoke(new Action(() => series1.Points.AddXY(Math.Round(newAngle), newQuality * 100)));
                                //series.Points.AddXY(Math.Round(newAngle), newValue);
                            }
                            else
                            {
                                series.Points.AddXY(Math.Round(newAngle), newValue);
                                series1.Points.AddXY(Math.Round(newAngle), newQuality * 100);
                            }


                        }
                        nn++;
                        Thread.Sleep(1);
                    }
                    catch (IOException)
                    {
                        if (sw.ElapsedMilliseconds > 10000)
                        {
                            inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File busy weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                            sw.Stop();
                            return reply;
                        }

                    };

                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + strAll + "\r\n"); Thread.Sleep(1);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                sr.Close();
                sw.Stop();
                //-------------------------------------------

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Single Aver = 0;
                //CharWeldon.ChartAreas[0].AxisX.Maximum = 370;//nn;
                int k = 0;
                for (int i = 0; i < nn; i++)
                {

                    string[] s = temp[i].Split(' ');
                    if (s.Length >= 4)
                    {
                        //series.Points.Add(Single.Parse(s[2]));
                        Aver = Aver + Single.Parse(s[0]);
                        k++;
                        //series1.Points.Add(Single.Parse(s[4]) * 100);
                    }

                }
                Aver = Aver / (k);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + Aver.ToString()); //Thread.Sleep(1);
                Single[] aver1 = new float[5];
                Single aver2 = Aver;
                int nnn = 0;
                int jj = 0;
                for (jj = 0; jj < aver1.Length; jj++)
                {
                    nnn = 0;
                    for (int i = 0; i < nn; i++)
                    {

                        string[] s = temp[i].Split(' ');
                        if (s.Length >= 4)
                        {

                            if (Single.Parse(s[0]) > 40 && Math.Abs(aver2 - Single.Parse(s[0])) < aver2 * 0.3f)
                            {
                                aver1[jj] = aver1[jj] + Single.Parse(s[0]);
                                nnn++;
                            }

                        }


                    }
                    aver2 = aver1[jj] / (nnn);
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "aver" + jj.ToString() + ": " + aver2.ToString("0.000")); Thread.Sleep(1);
                }



                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => series2 = CharWeldon.Series.Add("Weldone")));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].Color = Color.Tomato));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].BorderWidth = 3));
                }
                else
                {
                    series2 = CharWeldon.Series.Add("Weldone");
                    CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line;
                    CharWeldon.Series["Weldone"].Color = Color.Tomato;
                    CharWeldon.Series["Weldone"].BorderWidth = 3;
                }
                int j = 0;
                Single jmin = 0;
                Single jmax = 0;
                Single QMax = 0;
                Single DMax = 0;
                Single Daver = 0;
                Single nMax = 0;
                string[] ss = null;
                Single ftemp = 0;
                for (int i = 0; i < nn; i++)
                {

                    ss = temp[i].Split(' ');

                    if (ss.Length >= 4 && Single.Parse(ss[0]) > aver2 * 0.8f)
                    //if (ss.Length >= 4 && Math.Abs(aver2 - Single.Parse(ss[0])) < aver2 * 0.2f)
                    {

                        //if (ftemp < Single.Parse(ss[0]))
                        //    jmax = Single.Parse(ss[2]);// i;
                        if (QMax < Single.Parse(ss[0]))
                        { QMax = Single.Parse(ss[0]); nMax = i; }
                        ftemp = Single.Parse(ss[0]);
                    }

                    else
                    {
                        if (jmin == 0 && i > 0 && ftemp > Single.Parse(ss[0]))
                            jmin = Single.Parse(ss[2]);// angle min
                        if (Single.Parse(ss[0]) > aver2 * 0.1f && i > 0 && ftemp - Single.Parse(ss[0]) > 10)
                            jmin = Single.Parse(ss[2]);// angle min

                        j++;
                        if (ftemp < Single.Parse(ss[0]) && Single.Parse(ss[0]) > aver2 * 0.5f)
                            jmax = Single.Parse(ss[2]);// i;
                        Daver = Daver + Single.Parse(ss[1]);
                        ftemp = Single.Parse(ss[0]);
                        if (jmax > 150)
                        { }
                        if (Single.Parse(ss[2]) > 310)
                        { }

                    }
                }
                //Debug.Print(jmin.ToString(), jmax.ToString());
                Daver = Daver / j;
                Single dD = 0;// Math.Abs(Daver - QMax);
                              //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmax=" + (jmax).ToString("0.000"));
                              //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmin=" + (jmin).ToString("0.000"));
                              //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Hweldone=" + (dD).ToString("0.000") + ":" + nMax.ToString());
                              //if(jmin > jmax) { ftemp = jmax;jmax = jmin;jmin = ftemp; }
                if (jmax > jmin)
                {
                    for (int ii = 0; ii < nn; ii++)
                    {
                        ss = temp[ii].Split(' ');
                        if (Single.Parse(ss[2]) < jmin)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1)));
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1);
                        }

                        else if (Single.Parse(ss[2]) > jmax)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1)));
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1);
                        }
                        else
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), aver2 * 100)));
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), aver2 * 100);
                        }
                        Thread.Sleep(1);
                    }
                }
                else
                    for (int ii = 0; ii < nn; ii++)
                    {
                        ss = temp[ii].Split(' ');
                        if (Single.Parse(ss[2]) < jmax)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1)));
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1);
                        }

                        else if (Single.Parse(ss[2]) > jmin)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1)));
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1);
                        }
                        else
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), aver2 * 100)));
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), aver2 * 100);
                        }
                        Thread.Sleep(1);
                    }

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmax=" + (jmax).ToString("0.000") + " Jmin=" + (jmin).ToString("0.000") + " Angle=" + ((jmax - jmin)).ToString("0.000"));
                //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmin=" + (jmin).ToString("0.000"));
                //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Angle:" + ((jmax - jmin)).ToString("0.000")); Thread.Sleep(1);
                if (jmax - jmin > 0) inv.settxt(label103, ((jmax - jmin)).ToString("0.0"));
                else inv.settxt(label103, ((jmin - jmax)).ToString("0.0"));
                Array.Resize<Single>(ref reply.data, 11); ;
                reply.data[0] = jmax - jmin;
                inv.set(button7, "Enabled", true);

                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.Visible = true));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Minimum = 0));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Maximum = 370));// nn;
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000)));

                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].CursorX.AutoScroll = true));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].IsValueShownAsLabel = true));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20));// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset()));
                }
                else
                {

                    CharWeldon.Visible = true;
                    CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                    CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);

                    CharWeldon.ChartAreas[0].CursorX.AutoScroll = true;
                    CharWeldon.Series["Distance"].IsValueShownAsLabel = true;
                    CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20;// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                reply.result = true;
                return reply;
            }
            catch (Exception ex)
            {
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + ex.ToString() + " //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                return reply;
            }
        }
        private CommReply RunWeldonFileInv_2()
        {

            //read array file from beckhoff
            CommReply reply = new CommReply();
            try
            {
                //inv.set(button7, "Enabled", false);
                reply.result = false;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Start read weldon data //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                //----------------delete file--------------------------
                string filename = @"C:\Project\DataTest1.txt";
                //if (File.Exists(filename))
                //{
                //    File.Delete(filename);
                //}
                //rot to 0
                inv.set(lblChar, "AutoSize", true);
                inv.set(lblChar, "Visible", false);
                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.Visible = false));
                    this.Invoke(new Action(() => CharWeldon.Series.Clear()));
                    this.Invoke(new Action(() => CharWeldon.Titles.Clear()));

                    //this.Invoke(new Action(() => CharWeldon.Palette = ChartColorPalette.Excel));

                    this.Invoke(new Action(() => series = CharWeldon.Series.Add("Distance")));
                    this.Invoke(new Action(() => series1 = CharWeldon.Series.Add("Quality")));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].Color = Color.Orange));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].Color = Color.Blue));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].MarkerSize = 1));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].MarkerSize = 1));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].BorderWidth = 2));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].BorderWidth = 2));

                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.X = 0));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Y = 90));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Width = 12));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Height = 20));
                    //series.Points.AddXY(Math.Round(newAngle), newValue);
                }
                else
                {
                    CharWeldon.Visible = false;
                    CharWeldon.Series.Clear();
                    CharWeldon.Titles.Clear();
                    //CharWeldon.Palette = ChartColorPalette.Excel;

                    series = CharWeldon.Series.Add("Distance");
                    series1 = CharWeldon.Series.Add("Quality");
                    //CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine;
                    CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine;
                    CharWeldon.Series["Distance"].Color = Color.Orange;
                    CharWeldon.Series["Quality"].Color = Color.Blue;
                    CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None;
                    CharWeldon.Series["Distance"].MarkerSize = 1;
                    CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None;
                    CharWeldon.Series["Quality"].MarkerSize = 1;
                    CharWeldon.Series["Distance"].BorderWidth = 2;
                    CharWeldon.Series["Quality"].BorderWidth = 2;

                    CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);
                    CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                    CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false;
                    CharWeldon.Legends[0].Position.Auto = false;
                    CharWeldon.Legends[0].Position.X = 0;
                    CharWeldon.Legends[0].Position.Y = 90;
                    CharWeldon.Legends[0].Position.Width = 12;
                    CharWeldon.Legends[0].Position.Height = 20;
                }


                //Thread.Sleep(100);


                Single newValue = 0;
                Single newQuality = 0;
                Single newAngle = 0;

                //--------------read file------------------
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                while (!File.Exists(filename) && !MyStatic.bReset)
                {
                    if (sw.ElapsedMilliseconds > 10000) break;
                    Thread.Sleep(100);
                }
                if (sw.ElapsedMilliseconds > 10000 || MyStatic.bReset)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File not exist weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    sw.Stop();
                    return reply;
                }
                //temp = new string[];
                string tt = "";
                byte[] bytes;
                bytes = File.ReadAllBytes(filename);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini read weldon data //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                tt = Encoding.UTF8.GetString(bytes);// + " " + nn.ToString();
                String strAll = "";
                temp = tt.Split('\0');// \r
                //string[]t= tt.Split('\r');// \r
                //string[] at = new string[1];
                string[] temp2 = new string[1];
                string[] tempOrigin = new string[1];
                //int an = 0;
                //char[] separators = new char[] { ';', ',', '\r', '\t', '\n','\0' };
                int lines = 0;
                for (int b = 0; b < temp.Length; b++)
                {

                    if (temp[b].Trim() != "" && temp[b].IndexOf('\r') > 5 && temp[b].IndexOf((nn).ToString(" 0.0")) > 5)
                    {

                        if (nn >= 1) Array.Resize<String>(ref temp2, nn + 1);
                        temp2[nn] = temp[b].Replace("\r\n", "") + " " + nn.ToString();
                        strAll = strAll + "\r\n" + temp2[nn];
                        nn++;

                    }
                    if (temp[b].Trim() != "" && temp[b].IndexOf('\r') > 5 && temp[b].IndexOf("lines") >= 0)
                    { lines = int.Parse(temp[b].Substring(5, temp[b].Length - 9)); }


                }
                nn = lines;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "lines " + lines.ToString()); //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Array.Resize<string>(ref temp2, nn);
                //string[] tmp= new string[temp.Length];
                //int nn = 0;
                //string[] temp2 =new string[1];
                //string[] temp3 = new string[1];
                //int aa = 0;
                //for (int i = 0; i < temp.Length; i++)
                //{

                //    temp3 = temp[i].Split(' ');
                //    //Byte[] un=null;
                //    //int off1 = 0;
                //    //int off2 = 0;

                //    ////String decodedString = System.Text.UnicodeEncoding.UTF8.GetString(un);
                //    //string decoded = System.Net.WebUtility.HtmlDecode(temp3[1]);
                //    //string str = System.Uri.UnescapeDataString(temp3[1]);
                //    //string rtstr = "";

                //    //    char c = (char)Int16.Parse(temp3[1], System.Globalization.NumberStyles.HexNumber);

                //    if (temp3[temp3.Length - 1] == aa.ToString("0.0"))
                //    {
                //            string s = temp3[temp3.Length - 4];
                //            string[] s1 = s.Split('\0');// '\'
                //            temp3[temp3.Length - 4] = s1[s1.Length - 1];
                //            if (nn >= 1) Array.Resize<String>(ref temp2, nn + 1);
                //            temp2[nn] = temp3[temp3.Length - 4] + " " + temp3[temp3.Length - 3] + " " + temp3[temp3.Length - 2] + " " + temp3[temp3.Length - 1] + " " + (nn).ToString();
                //            strAll = strAll + "\r\n" + temp2[nn];
                //            nn++;
                //            aa++;

                //    }


                //}


                //---------------------fini read file from plc----------------------
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + strAll + "\r\n"); Thread.Sleep(1);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Single Aver = 0;
                //CharWeldon.ChartAreas[0].AxisX.Maximum = 370;//nn;
                int nn1 = 0;
                Single qualityMin = 40;
                for (int i = 0; i < nn; i++)//nn-file lines
                {

                    string[] s = temp2[i].Split(' ');
                    if (s.Length >= 4 && Single.Parse(s[0]) > qualityMin)//if quality > 40
                    {
                        Aver = Aver + Single.Parse(s[0]);
                        nn1++;

                    }

                }
                Aver = Aver / (nn1);//calculate quality average
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + Aver.ToString()); //Thread.Sleep(1);
                Single[] aver1 = new float[5];
                Single aver2 = Aver;//aver2 - average of quality > qualityMin (real data from distance sensor)
                int nnn = 0;
                int jj = 0;
                for (jj = 0; jj < aver1.Length; jj++)
                {
                    nnn = 0;
                    for (int i = 0; i < nn; i++)//nn-file lines
                    {

                        string[] s = temp2[i].Split(' ');
                        if (s.Length >= 4)
                        {

                            if (Single.Parse(s[0]) > qualityMin && Math.Abs(aver2 - Single.Parse(s[0])) < aver2 * 0.2f)
                            {
                                aver1[jj] = aver1[jj] + Single.Parse(s[0]); //delta between average of quality and data < aver2*0.2
                                nnn++;
                            }

                        }


                    }
                    aver2 = aver1[jj] / (nnn);//new quality average in window of 0.2 of average before 
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "aver" + jj.ToString() + ": " + aver2.ToString("0.000")); Thread.Sleep(1);
                }



                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => series2 = CharWeldon.Series.Add("Weldone")));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].Color = Color.Tomato));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].BorderWidth = 3));
                }
                else
                {
                    series2 = CharWeldon.Series.Add("Weldone");
                    CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line;
                    CharWeldon.Series["Weldone"].Color = Color.Tomato;
                    CharWeldon.Series["Weldone"].BorderWidth = 3;
                }
                int j = 0;
                Single jmin = 0;
                Single jmax = 0;
                Single QMax = 0;
                Single QMin = 100;
                Single DMax = 0;
                Single Daver = 0;
                Single nMax = 0;
                string[] ss = null;
                Single ftemp = 0;
                Single ftemp1 = 0;
                Single okmin = 0;
                Single okmax = 0;
                int ln = 0;
                string[] ss1 = new string[1];
                string[] ss2 = new string[1];
                int i1 = 0;

                tempOrigin = new string[nn];
                //find quality data less average
                for (int i = 0; i < nn; i++)
                {

                    tempOrigin[i] = temp2[i];
                    ss = temp2[i].Split(' ');


                    if (ss.Length >= 4 && (Single.Parse(ss[0]) > aver2 * 1.2 || Math.Abs(aver2 - Single.Parse(ss[0])) < aver2 * 0.2f) && Single.Parse(ss[0]) > qualityMin)
                    {
                        //if quality > 1.2 average or in window of 0.2 average
                        ss1[i1] = temp2[i];
                        i1++;
                        Array.Resize<String>(ref ss1, i1 + 1);
                    }

                    else
                    {

                        j++;

                        Daver = Daver + Single.Parse(ss[1]);//distance
                        ftemp = Single.Parse(ss[0]);

                        if (ss2.Length < ss1.Length && ss1.Length > 2)
                        {
                            ss2 = new string[ss1.Length - 2];
                            for (int ii = 0; ii < ss1.Length - 2; ii++) ss2[ii] = ss1[ii];//move ss1 to ss2

                        }
                        ss1 = new string[1];//clear ss1
                        i1 = 0;
                        ln = ss1.Length;

                    }
                }
                if (ss2.Length > ss1.Length)
                {
                    ss1 = new string[ss2.Length];
                    for (int ii = 0; ii < ss1.Length; ii++) ss1[ii] = ss2[ii];//

                }
                Single angOff = 0;
                int id = 0;
                if (ss1.Length > 50) //move graph left on angOff
                {
                    ss = ss1[ss1.Length - 20].Split(' ');
                    if (ss.Length >= 5)
                    {
                        angOff = Single.Parse(ss[2]);
                        id = (int)Single.Parse(ss[4]);
                        inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "AngOff=" + (angOff).ToString("0.000") + " :" + id.ToString());
                    }
                }
                string[] temp1 = new string[temp2.Length];
                Single angOff2 = 0;
                for (int i = id; i < temp2.Length; i++)//move left on angOff
                {
                    temp1[i - id] = temp2[i];
                    ss = temp2[i].Split(' ');
                    ss[2] = (Single.Parse(ss[2]) - angOff).ToString();
                    ss[3] = (i - id).ToString();
                    temp1[i - id] = ss[0] + " " + ss[1] + " " + ss[2] + " " + ss[3];
                    angOff2 = Single.Parse(ss[2]);
                }
                //0-quality 1-distance 2-angle 3-number
                for (int i = 0; i < id; i++) //move negative graph after last temp1 coordinate
                {
                    temp1[temp2.Length - id + i] = temp2[i];
                    ss = temp2[i].Split(' ');
                    ss[2] = (Single.Parse(ss[2]) + angOff2).ToString();
                    ss[3] = (temp2.Length - id + i).ToString();
                    temp1[temp2.Length - id + i] = ss[0] + " " + ss[1] + " " + ss[2] + " " + ss[3];

                }

                //
                Daver = Daver / j;
                Single dD = 0;// Math.Abs(Daver - QMax);

                Single WeldonAngle = 0;
                string[] torgTemp = new string[tempOrigin.Length];
                Single angletemp = 0;
                int ij = 0;

                //calculate jmin - first graph down  and jmax - last graph up 
                Single[] max = new Single[10];
                Single[] min = new Single[10]; ;
                int imax = 0;
                int imin = 0;
                for (int ii = 0; ii < temp1.Length; ii++)
                {
                    Single angOff1 = angOff;


                    if (temp1[ii] != null && temp1[ii] != "")
                    {
                        ss = temp1[ii].Split(' ');
                        newValue = Single.Parse(ss[1]);
                        newQuality = Single.Parse(ss[0]);
                        newAngle = Single.Parse(ss[2]);// - angOff1;

                        if (newQuality > aver2 / 2.0f && newQuality < 100) max[imax] = newAngle;
                        if (newQuality <= aver2 / 2.0f && newQuality > 5) min[imin] = newAngle;

                        if (max[imax] > min[imin] && min[imin] > 0)
                        { imax++; imin++; }
                        if (max[imax] < min[imin] && max[imax] > 0)
                        { imax++; imin++; }
                    }
                }
                for (int i = 0; i < max.Length; i++) if (max[i] > 0 && min[i] > 0) { jmin = max[i]; break; }
                for (int i = 0; i < max.Length; i++) if (max[i] > 0 && min[i] > 0) { jmax = max[i]; }

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmax=:" + (jmax).ToString("0.000"));//end angle of minimum quality
                for (int ii = 0; ii < temp1.Length; ii++)
                {
                    Single angOff1 = angOff;


                    if (temp1[ii] != null && temp1[ii] != "")
                    {
                        ss = temp1[ii].Split(' ');
                        newValue = Single.Parse(ss[1]);
                        newQuality = Single.Parse(ss[0]);
                        newAngle = Single.Parse(ss[2]);// - angOff1;


                        if (newAngle > 0)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series.Points.AddXY(newAngle, newValue)));
                                this.Invoke(new Action(() => series1.Points.AddXY(newAngle, newQuality * 100.0f)));

                            }
                            else
                            {
                                series.Points.AddXY(newAngle, newValue);
                                series1.Points.AddXY(newAngle, newQuality * 100.0f);
                            }
                        }
                    }


                    ss = temp1[ii].Split(' ');
                    angOff1 = 0;
                    if (Single.Parse(ss[2]) - angOff1 < jmin && Single.Parse(ss[2]) > 0)
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => series2.Points.AddXY((Single.Parse(ss[2]) - angOff1), 1)));//weldone angle
                        }
                        else series2.Points.AddXY((Single.Parse(ss[2]) - angOff1), 1);
                    }

                    else if (Single.Parse(ss[2]) - angOff1 > jmax && Single.Parse(ss[2]) > 0)
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => series2.Points.AddXY((Single.Parse(ss[2]) - angOff1), 1)));//weldone angle
                        }
                        else series2.Points.AddXY((Single.Parse(ss[2]) - angOff1), 1);
                    }
                    else
                    {
                        if (Single.Parse(ss[2]) > 0)
                        {
                            if (InvokeRequired)
                            {
                                this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), aver2 * 100)));//weldone angle
                            }
                            else series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), aver2 * 100);
                        }
                    }
                    Thread.Sleep(1);
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "jmin=" + jmin.ToString() + " jmax=" + jmax.ToString() + " Angle=" + ((jmax - jmin)).ToString("0.000")); Thread.Sleep(1);
                inv.settxt(label103, ((jmax - jmin)).ToString("0.0"));
                Array.Resize<Single>(ref reply.data, 11); ;
                reply.data[0] = jmax - jmin;
                inv.set(button7, "Enabled", true);

                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.Visible = true));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Minimum = 0));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Maximum = 370));// nn;
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000)));

                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].CursorX.AutoScroll = true));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].IsValueShownAsLabel = true));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20));// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset()));
                }
                else
                {

                    CharWeldon.Visible = true;
                    CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                    CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);

                    CharWeldon.ChartAreas[0].CursorX.AutoScroll = true;
                    CharWeldon.Series["Distance"].IsValueShownAsLabel = true;
                    CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20;// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                reply.result = true;
                return reply;
            }
            catch (Exception ex)
            {
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + ex.ToString() + " //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                return reply;
            }
        }
        private CommReply RunWeldonFileInv_4()
        {
            //read string file from beckhoff
            CommReply reply = new CommReply();
            try
            {

                reply.result = false;
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Start read weldon data //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                //----------------delete file--------------------------
                string filename = @"C:\Project\DataTest1.txt";

                inv.set(lblChar, "AutoSize", true);
                inv.set(lblChar, "Visible", false);
                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.Visible = false));
                    this.Invoke(new Action(() => CharWeldon.Series.Clear()));
                    this.Invoke(new Action(() => CharWeldon.Titles.Clear()));

                    //this.Invoke(new Action(() => CharWeldon.Palette = ChartColorPalette.Excel));

                    this.Invoke(new Action(() => series = CharWeldon.Series.Add("Distance")));
                    this.Invoke(new Action(() => series1 = CharWeldon.Series.Add("Quality")));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].Color = Color.Orange));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].Color = Color.Blue));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].MarkerSize = 1));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].MarkerSize = 1));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].BorderWidth = 2));
                    this.Invoke(new Action(() => CharWeldon.Series["Quality"].BorderWidth = 2));

                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.X = 0));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Y = 90));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Width = 12));
                    this.Invoke(new Action(() => CharWeldon.Legends[0].Position.Height = 20));
                    //series.Points.AddXY(Math.Round(newAngle), newValue);
                }
                else
                {
                    CharWeldon.Visible = false;
                    CharWeldon.Series.Clear();
                    CharWeldon.Titles.Clear();
                    //CharWeldon.Palette = ChartColorPalette.Excel;

                    series = CharWeldon.Series.Add("Distance");
                    series1 = CharWeldon.Series.Add("Quality");
                    //CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine;
                    CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine;
                    CharWeldon.Series["Distance"].Color = Color.Orange;
                    CharWeldon.Series["Quality"].Color = Color.Blue;
                    CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None;
                    CharWeldon.Series["Distance"].MarkerSize = 1;
                    CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None;
                    CharWeldon.Series["Quality"].MarkerSize = 1;
                    CharWeldon.Series["Distance"].BorderWidth = 2;
                    CharWeldon.Series["Quality"].BorderWidth = 2;

                    CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);
                    CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                    CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false;
                    CharWeldon.Legends[0].Position.Auto = false;
                    CharWeldon.Legends[0].Position.X = 0;
                    CharWeldon.Legends[0].Position.Y = 90;
                    CharWeldon.Legends[0].Position.Width = 12;
                    CharWeldon.Legends[0].Position.Height = 20;
                }


                //Thread.Sleep(100);


                int newValue = 0;
                int newQuality = 0;
                Single newAngle = 0;

                //--------------read file------------------
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                while (!File.Exists(filename) && !MyStatic.bReset)
                {
                    if (sw.ElapsedMilliseconds > 10000) break;
                    Thread.Sleep(100);
                }
                if (sw.ElapsedMilliseconds > 10000 || MyStatic.bReset)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File not exist weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    sw.Stop();
                    return reply;
                }

                //--------------------read text file-----------------------
                StreamReader sr = new StreamReader(filename);
                int n = 0;
                nn = 0;
                sw.Restart();
                String strAll = "";
                string[] temp2 = new string[1];
                //temp = new string[1];

                while (!sr.EndOfStream)
                {
                    try
                    {
                        if (nn > temp2.Length - 1) { Array.Resize<String>(ref temp2, nn + 1); }
                        temp2[nn] = sr.ReadLine() + " " + nn.ToString();

                        strAll = strAll + "\r\n" + temp2[nn];
                        nn++;

                        Thread.Sleep(1);
                    }
                    catch (IOException)
                    {
                        if (sw.ElapsedMilliseconds > 10000)
                        {
                            inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File busy weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                            sw.Stop();
                            return reply;
                        }

                    };

                }


                //sr.Close();
                //sw.Stop();
                //-------------------------------------------
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + strAll + "\r\n"); Thread.Sleep(1);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Single Aver = 0;
                //CharWeldon.ChartAreas[0].AxisX.Maximum = 370;//nn;
                int nn1 = 0;
                for (int i = 0; i < nn; i++)
                {

                    string[] s = temp2[i].Split(' ');
                    if (s.Length >= 4 && Single.Parse(s[0]) > 40)
                    {
                        Aver = Aver + Single.Parse(s[0]);
                        nn1++;

                    }

                }
                Aver = Aver / (nn1);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + Aver.ToString()); //Thread.Sleep(1);
                Single[] aver1 = new float[5];
                Single aver2 = Aver;
                int nnn = 0;
                int jj = 0;
                for (jj = 0; jj < aver1.Length; jj++)
                {
                    nnn = 0;
                    for (int i = 0; i < nn; i++)
                    {

                        string[] s = temp2[i].Split(' ');
                        if (s.Length >= 4)
                        {

                            if (Single.Parse(s[0]) > 40 && Math.Abs(aver2 - Single.Parse(s[0])) < aver2 * 0.2f)
                            {
                                aver1[jj] = aver1[jj] + Single.Parse(s[0]);
                                nnn++;
                            }

                        }


                    }
                    aver2 = aver1[jj] / (nnn);
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "aver" + jj.ToString() + ": " + aver2.ToString("0.000")); Thread.Sleep(1);
                }



                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => series2 = CharWeldon.Series.Add("Weldone")));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].Color = Color.Tomato));
                    this.Invoke(new Action(() => CharWeldon.Series["Weldone"].BorderWidth = 3));
                }
                else
                {
                    series2 = CharWeldon.Series.Add("Weldone");
                    CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line;
                    CharWeldon.Series["Weldone"].Color = Color.Tomato;
                    CharWeldon.Series["Weldone"].BorderWidth = 3;
                }
                int j = 0;
                Single jmin = 0;
                Single jmax = 0;
                Single QMax = 0;
                Single QMin = 100;
                Single DMax = 0;
                Single Daver = 0;
                Single nMax = 0;
                string[] ss = null;
                Single ftemp = 0;
                Single ftemp1 = 0;
                Single okmin = 0;
                Single okmax = 0;
                int ln = 0;
                string[] ss1 = new string[1];
                string[] ss2 = new string[1];
                int i1 = 0;

                for (int i = 0; i < nn; i++)
                {

                    ss = temp2[i].Split(' ');
                    //if (QMax < Single.Parse(ss[0])) { QMax = Single.Parse(ss[0]); okmax = Single.Parse(ss[2]); }
                    //if (QMin > Single.Parse(ss[0])) { QMin = Single.Parse(ss[0]); okmin = Single.Parse(ss[2]); }

                    if (ss.Length >= 4 && (Single.Parse(ss[0]) > aver2 * 1.2 || Math.Abs(aver2 - Single.Parse(ss[0])) < aver2 * 0.2f) && Single.Parse(ss[0]) > 40)
                    //if (ss.Length >= 4 && i > 0  && Single.Parse(ss[0]) > aver2 * 0.8f && ftemp < Single.Parse(ss[0]))
                    {
                        ss1[i1] = temp2[i];
                        i1++;
                        Array.Resize<String>(ref ss1, i1 + 1);
                    }

                    else
                    {
                        //if (jmin == 0 && i > 0 && ftemp > Single.Parse(ss[0])) jmin = Single.Parse(ss[2]);// i;

                        j++;

                        Daver = Daver + Single.Parse(ss[1]);
                        ftemp = Single.Parse(ss[0]);

                        if (ss2.Length < ss1.Length)
                        {
                            ss2 = new string[ss1.Length];
                            for (int ii = 0; ii < ss1.Length; ii++) ss2[ii] = ss1[ii];

                        }
                        ss1 = new string[1];
                        i1 = 0;
                        ln = ss1.Length;

                    }
                }
                if (ss2.Length > ss1.Length)
                {
                    ss1 = new string[ss2.Length];
                    for (int ii = 0; ii < ss1.Length; ii++) ss1[ii] = ss2[ii];

                }
                Single angOff = 0;
                int id = 0;
                if (ss1.Length > 50)
                {
                    ss = ss1[ss1.Length - 20].Split(' ');
                    if (ss.Length >= 5)
                    {
                        angOff = Single.Parse(ss[2]);
                        id = (int)Single.Parse(ss[4]);
                        inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "AngOff=" + (angOff).ToString("0.000") + " :" + id.ToString());
                    }
                }
                string[] temp1 = new string[temp2.Length];
                Single angOff2 = 0;
                for (int i = id; i < temp2.Length; i++)
                {
                    temp1[i - id] = temp2[i];
                    ss = temp2[i].Split(' ');
                    ss[2] = (Single.Parse(ss[2]) - angOff).ToString();
                    ss[3] = (i - id).ToString();
                    temp1[i - id] = ss[0] + " " + ss[1] + " " + ss[2] + " " + ss[3];
                    angOff2 = Single.Parse(ss[2]);
                }

                for (int i = 0; i < id; i++)
                {
                    temp1[temp2.Length - id + i] = temp2[i];
                    ss = temp2[i].Split(' ');
                    ss[2] = (Single.Parse(ss[2]) + angOff2).ToString();
                    ss[3] = (temp2.Length - id + i).ToString();
                    temp1[temp2.Length - id + i] = ss[0] + " " + ss[1] + " " + ss[2] + " " + ss[3];

                }
                //shift graph on angOff and second calculation
                for (int i = 0; i < temp1.Length; i++)
                {
                    Single angOff1 = angOff;

                    if (temp1[i] != null && temp1[i] != "")
                    {
                        ss = temp1[i].Split(' ');
                        if (ss.Length >= 4 && (Single.Parse(ss[0]) > aver2 * 1.2 || Math.Abs(aver2 - Single.Parse(ss[0])) < aver2 * 0.2f) && Single.Parse(ss[0]) > 40)
                        //if (ss.Length >= 4 && i > 0  && Single.Parse(ss[0]) > aver2 * 0.8f && ftemp < Single.Parse(ss[0]))
                        {

                        }

                        else
                        {
                            if (jmin == 0 && i > 0)// && ftemp > Single.Parse(ss[0]))
                                jmin = Single.Parse(ss[2]);// - angOff1;// i;

                            j++;
                            jmax = Single.Parse(ss[2]);// - angOff1;// i;
                            Daver = Daver + Single.Parse(ss[1]);

                        }
                    }
                }
                //
                Daver = Daver / j;
                Single dD = 0;// Math.Abs(Daver - QMax);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmax=:" + (jmax).ToString("0.000"));
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Hweldone=:" + (dD).ToString("0.000") + ":" + nMax.ToString());
                for (int ii = 0; ii < temp1.Length; ii++)
                {
                    Single angOff1 = angOff;

                    if (temp1[ii] != null && temp1[ii] != "")
                    {
                        ss = temp1[ii].Split(' ');
                        newValue = (int)Single.Parse(ss[1]);
                        newQuality = (int)Single.Parse(ss[0]);
                        newAngle = Single.Parse(ss[2]);// - angOff1;
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => series.Points.AddXY(Math.Round(newAngle), newValue)));
                            this.Invoke(new Action(() => series1.Points.AddXY(Math.Round(newAngle), newQuality * 100)));

                        }
                        else
                        {
                            series.Points.AddXY(Math.Round(newAngle), newValue);
                            series1.Points.AddXY(Math.Round(newAngle), newQuality * 100);
                        }
                    }

                    //
                    ss = temp1[ii].Split(' ');
                    angOff1 = 0;
                    if (Single.Parse(ss[2]) - angOff1 < jmin)
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), 1)));
                        }
                        else series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), 1);
                    }

                    else if (Single.Parse(ss[2]) - angOff1 > jmax)
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), 1)));
                        }
                        else series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), 1);
                    }
                    else
                    {
                        if (InvokeRequired)
                        {
                            this.Invoke(new Action(() => series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), aver2 * 100)));
                        }
                        else series2.Points.AddXY(Math.Round(Single.Parse(ss[2]) - angOff1), aver2 * 100);
                    }
                    Thread.Sleep(1);
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "jmin=" + jmin.ToString() + " jmax=" + jmax.ToString() + " Angle=" + ((jmax - jmin)).ToString("0.000")); Thread.Sleep(1);
                inv.settxt(label103, ((jmax - jmin)).ToString("0.0"));
                Array.Resize<Single>(ref reply.data, 11); ;
                reply.data[0] = jmax - jmin;
                inv.set(button7, "Enabled", true);

                if (InvokeRequired)
                {
                    this.Invoke(new Action(() => CharWeldon.Visible = true));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Minimum = 0));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.Maximum = 370));// nn;
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370)));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000)));

                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].CursorX.AutoScroll = true));
                    this.Invoke(new Action(() => CharWeldon.Series["Distance"].IsValueShownAsLabel = true));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20));// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset()));
                    this.Invoke(new Action(() => CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset()));
                }
                else
                {

                    CharWeldon.Visible = true;
                    CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                    CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);

                    CharWeldon.ChartAreas[0].CursorX.AutoScroll = true;
                    CharWeldon.Series["Distance"].IsValueShownAsLabel = true;
                    CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20;// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;
                    CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                reply.result = true;
                return reply;
            }
            catch (Exception ex)
            {
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + ex.ToString() + " //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                return reply;
            }
        }


        private async Task<bool> RunWeldonFile()
        {
            try
            {
                //inv.set(button7, "Enabled", false);
                inv.settxt(txtWeldon, "Start weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                //----------------delete file--------------------------
                string filename = @"C:\Project\DataTest1.txt";
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                //rot to 0
                inv.set(lblChar, "AutoSize", true);
                inv.set(lblChar, "Visible", false);

                CharWeldon.Series.Clear();
                CharWeldon.Titles.Clear();
                CharWeldon.Palette = ChartColorPalette.Excel;

                series = CharWeldon.Series.Add("Distance");
                series1 = CharWeldon.Series.Add("Quality");
                CharWeldon.Series["Distance"].ChartType = SeriesChartType.FastLine;
                CharWeldon.Series["Quality"].ChartType = SeriesChartType.FastLine;
                CharWeldon.Series["Distance"].Color = Color.Orange;
                CharWeldon.Series["Quality"].Color = Color.Blue;
                CharWeldon.Series["Distance"].MarkerStyle = MarkerStyle.None;
                CharWeldon.Series["Distance"].MarkerSize = 1;
                CharWeldon.Series["Quality"].MarkerStyle = MarkerStyle.None;
                CharWeldon.Series["Quality"].MarkerSize = 1;
                CharWeldon.Series["Distance"].BorderWidth = 2;
                CharWeldon.Series["Quality"].BorderWidth = 2;
                CharWeldon.ChartAreas[0].AxisX.Interval = 20;
                CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// Xscale;
                CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                CharWeldon.ChartAreas[0].CursorX.AutoScroll = true;
                CharWeldon.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
                CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
                CharWeldon.ChartAreas[0].CursorY.AutoScroll = true;
                CharWeldon.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
                CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);


                int axis = 4;
                Single Speed = AxStatus[axis - 1].Vmax;//7200;
                Single speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;
                //Xscale = 370 * 5 / Single.Parse(txtSpeedSt.Text);
                Single dist = -5;
                var task1 = Task.Run(() => MoveAbs(0, axis, dist, Speed / 2));
                await task1;
                if (!task1.Result.result)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Error move -5 weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    //inv.set(button6, "Enabled", true);
                    return false;
                }

                Thread.Sleep(100);
                nn = 0;
                //MyStatic.bExitcycle = false;
                dist = 365;
                var task2 = Task.Run(() => MoveAbs(0, axis, dist, speed));

                //var task = Task.Run(() => BeckhofSensorRead());
                //await task;
                CommReply rep = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = 601;
                ParmsPlc.SendParm[10] = 20f;//tmout

                var task3 = Task.Run(() => Beckhoff_IOlink.PlcSendCmd(StartAddressSendReadIOlink, ParmsPlc, true));
                //var task1 = Task.Run(() => Beckhoff_IOlink.WaitPlcReply(SendPar));
                await task2;
                //await task3;

                //inv.set(button7,"Enabled", true);
                //return;

                int newValue = 0;
                int newQuality = 0;
                Single newAngle = 0;

                axis = 4;
                Speed = AxStatus[axis - 1].Vmax;//7200;
                speed = Speed * Single.Parse(txtSpeedSt.Text) / 100;

                dist = -5;
                var task4 = Task.Run(() => MoveAbs(0, axis, dist, Speed / 2));
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini Rotate weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                await task3;//wait io link
                rep = task3.Result;
                //--------------read file------------------
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                while (!File.Exists(filename) && !MyStatic.bReset)
                {
                    if (sw.ElapsedMilliseconds > 10000) break;
                    Thread.Sleep(100);
                }
                if (sw.ElapsedMilliseconds > 10000 || MyStatic.bReset)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File not exist weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    sw.Stop();
                    return false;
                }

                StreamReader sr = new StreamReader(filename);
                int n = 0;
                nn = 0;
                sw.Restart();
                String strAll = "";
                while (!sr.EndOfStream)
                {
                    try
                    {
                        temp[nn] = sr.ReadLine();
                        //inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[nn]); //Thread.Sleep(1);
                        strAll = strAll + "\r\n" + temp[nn];
                        string[] s1 = temp[nn].Split(' ');
                        newValue = (int)Single.Parse(s1[1]);
                        newQuality = (int)Single.Parse(s1[0]);
                        newAngle = Single.Parse(s1[2]);
                        //if (s1.Length >= 4)
                        //{

                        //        series.Points.AddXY(Math.Round(newAngle), newValue);
                        //        series1.Points.AddXY(Math.Round(newAngle), newQuality * 100);



                        //}
                        nn++;
                        Thread.Sleep(1);
                    }
                    catch (IOException)
                    {
                        if (sw.ElapsedMilliseconds > 10000)
                        {
                            inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "File busy weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                            sw.Stop();
                            return false;
                        }

                    };

                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + strAll + "\r\n"); Thread.Sleep(1);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini Read File weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                sr.Close();
                sw.Stop();
                //-------------------------------------------

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                Single Aver = 0;
                //CharWeldon.ChartAreas[0].AxisX.Maximum = 370;//nn;
                for (int i = 0; i < nn; i++)
                {

                    string[] s = temp[i].Split(' ');
                    if (s.Length >= 4)
                    {
                        //series.Points.Add(Single.Parse(s[2]));
                        Aver = Aver + Single.Parse(s[0]);
                        //series1.Points.Add(Single.Parse(s[4]) * 100);
                    }

                }
                Aver = Aver / (nn);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + Aver.ToString()); //Thread.Sleep(1);
                Single[] aver1 = new float[5];
                Single aver2 = Aver;
                int nnn = 0;
                int jj = 0;
                for (jj = 0; jj < aver1.Length; jj++)
                {
                    nnn = 0;
                    for (int i = 0; i < nn; i++)
                    {

                        string[] s = temp[i].Split(' ');
                        if (s.Length >= 4)
                        {

                            if (Single.Parse(s[0]) > 40 && Math.Abs(aver2 - Single.Parse(s[0])) < aver2 * 0.2f)
                            {
                                aver1[jj] = aver1[jj] + Single.Parse(s[0]);
                                nnn++;
                            }

                        }


                    }
                    aver2 = aver1[jj] / (nnn);
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "aver" + jj.ToString() + ": " + aver2.ToString("0.000")); Thread.Sleep(1);
                }


                series2 = CharWeldon.Series.Add("Weldone");
                CharWeldon.Series["Weldone"].ChartType = SeriesChartType.Line;
                CharWeldon.Series["Weldone"].Color = Color.Tomato;
                CharWeldon.Series["Weldone"].BorderWidth = 3;

                int j = 0;
                Single jmin = 0;
                Single jmax = 0;
                Single QMax = 0;
                Single DMax = 0;
                Single Daver = 0;
                Single nMax = 0;
                string[] ss = null;
                for (int i = 0; i < nn; i++)
                {

                    ss = temp[i].Split(' ');

                    if (ss.Length >= 4 && Math.Abs(aver2 - Single.Parse(ss[0])) < aver2 * 0.2f)
                    {

                        if (QMax < Single.Parse(ss[0])) { QMax = Single.Parse(ss[0]); nMax = i; }

                    }

                    else
                    {
                        if (jmin == 0) jmin = Single.Parse(ss[2]);// i;

                        j++;
                        jmax = Single.Parse(ss[2]);// i;
                        Daver = Daver + Single.Parse(ss[1]);


                    }
                }
                Daver = Daver / j;
                Single dD = 0;// Math.Abs(Daver - QMax);
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Jmax=:" + (jmax).ToString("0.000"));
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Hweldone=:" + (dD).ToString("0.000") + ":" + nMax.ToString());
                for (int ii = 0; ii < nn; ii++)
                {
                    ss = temp[ii].Split(' ');
                    if (Single.Parse(ss[2]) < jmin)
                    {
                        series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1);
                    }

                    else if (Single.Parse(ss[2]) > jmax)
                    {
                        series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), 1);
                    }
                    else
                    {
                        series2.Points.AddXY(Math.Round(Single.Parse(ss[2])), aver2 * 100);
                    }
                    Thread.Sleep(1);
                }
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Angle:" + ((jmax - jmin)).ToString("0.000")); Thread.Sleep(1);
                inv.settxt(label103, ((jmax - jmin)).ToString("0.00"));
                inv.set(button7, "Enabled", true);
                await task4;
                if (!task4.Result.result)
                {
                    inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Error move back weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    return false;
                }


                CharWeldon.ChartAreas[0].AxisX.Minimum = 0;
                CharWeldon.ChartAreas[0].AxisX.Maximum = 370;// nn;
                CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                CharWeldon.Series["Distance"].IsValueShownAsLabel = true;
                CharWeldon.ChartAreas[0].AxisX.LabelStyle.Interval = 20;// (int)CharWeldon.ChartAreas[0].AxisX.Maximum/10.0f;

                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + "Fini weldon //" + DateTime.Now.ToString("HH:mm:ss.fff"));
                return true;
            }
            catch (Exception ex)
            {
                inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + ex.ToString() + " //" + DateTime.Now.ToString("HH:mm:ss.fff"));

                return false;
            }
        }

        int chartX = 0;
        int chartY = 0;
        private void CharWeldon_MouseDown(object sender, MouseEventArgs e)
        {
            chartX = e.Location.X;
            chartY = e.Location.Y;
            CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            CharWeldon.ChartAreas[0].CursorX.AutoScroll = true;
            CharWeldon.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            CharWeldon.ChartAreas[0].CursorY.AutoScroll = true;
            CharWeldon.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = true;
            lblChar.Text = "";
            CharMoseDown = true;
        }
        int Xclale = 200;
        private void CharWeldon_MouseUp(object sender, MouseEventArgs e)
        {
            if (chartX > e.Location.X)
            {
                if (nn > 200) CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, nn);
                else CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);


                CharWeldon.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                CharWeldon.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                CharWeldon.ChartAreas[0].AxisX.ScaleView.Zoom(0, 370);
                CharWeldon.ChartAreas[0].AxisY.ScaleView.Zoom(0, 11000);
                CharWeldon.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                CharWeldon.ChartAreas[0].AxisY.ScrollBar.Enabled = false;


                lblChar.Text = "";

            }
            if (chartX == e.Location.X) CharMoseDown = false;
            if (CharMoseDown)
            {
                CharMoseDown = false;
                return;
            }
            else CharMoseDown = true;

            int x = e.Location.X;
            int y = e.Location.Y;
            //System.Drawing.Point xy = new System.Drawing.Point();
            //xy.X = x;
            //xy.Y = y;
            //lblChar.Text = x.ToString();
            lblChar.Location = new System.Drawing.Point(x + 5, y - 40);
            lblChar.Visible = true;
            lblChar.Show();
            this.CharWeldon.Controls.Add(this.lblChar);
            double yy = CharWeldon.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
            double xx = CharWeldon.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            lblChar.Text = "X=" + xx.ToString("0") + "\r\n" + "Y=" + yy.ToString("0");
            lblChar.BackColor = System.Drawing.Color.Transparent;

        }
        Label lblChar = new Label();

        bool CharMoseDown = false;

        private async void btnRejectCalib_Click(object sender, EventArgs e)
        {
            if (MyStatic.Robot == MyStatic.RobotLoad)
            {

                try
                {
                    txtPr0x.Text = "";
                    txtPr0y.Text = "";
                    txtPr1x.Text = "";
                    txtPr1y.Text = "";
                    txtPr2x.Text = "";
                    txtPr2y.Text = "";
                    txtPr3x.Text = "";
                    txtPr3y.Text = "";
                    RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                    var task1 = Setup(1, 1);
                    fini = await task1;
                    if (fini.result)
                    {
                        MessageBox.Show("SAVE POSITIONS? ", "SETUP", MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        DialogResult res1 = MessageBox.Show("SAVE POSITIONS? ", "SETUP", MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        if (res1 == DialogResult.Yes) //save positions
                        {
                            RobotLoadPoints.P_0.x = Single.Parse(txtPr0x.Text);
                            RobotLoadPoints.P_0.y = Single.Parse(txtPr0y.Text);
                            RobotLoadPoints.P_1.x = Single.Parse(txtPr1x.Text);
                            RobotLoadPoints.P_1.y = Single.Parse(txtPr1y.Text);
                            RobotLoadPoints.P_2.x = Single.Parse(txtPr2x.Text);
                            RobotLoadPoints.P_2.y = Single.Parse(txtPr2y.Text);
                            SavePosIni();
                            int err = 0;
                            var task3 = WritePosition(RobotLoad, RobotLoadPoints.P_0.id, RobotLoadPoints.P_0);
                            await task3;
                            RobotFunctions.CommReply reply = task3.Result;
                            if (!reply.result) { err++; }
                            task3 = WritePosition(RobotLoad, RobotLoadPoints.P_1.id, RobotLoadPoints.P_1);
                            await task3;
                            reply = task3.Result;
                            if (!reply.result) { err++; }
                            task3 = WritePosition(RobotLoad, RobotLoadPoints.P_2.id, RobotLoadPoints.P_2);
                            await task3;
                            reply = task3.Result;
                            if (!reply.result) { err++; }
                            if (err != 0)
                            {
                                MessageBox.Show("ERROR SEND POSITIONS TO ROBOT! ", "ERROR", MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                return;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("ERROR TEACH CALIBRATION POINTS! ", "ERROR", MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }

                    var task2 = Task.Run(() => RobotHome());
                    await task2;

                    ControlsEnable(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CALIBRATION ERROR! " + ex.Message, "ERROR", MessageBoxButtons.OK,
                                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        #region //---------rejected grid--------------
        private void InitGridReject()
        {

            try
            {

                dataGridViewReject.Columns.Clear();
                dataGridViewReject.Rows.Clear();
                dataGridViewReject.AutoResizeColumns();
                dataGridViewReject.AllowUserToAddRows = false;

                dataGridViewReject.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                DataGridViewCell cell = new DataGridViewTextBoxCell();

                DataGridViewColumn col0 = new DataGridViewColumn();
                col0.Name = "Num";
                col0.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col0);
                dataGridViewReject.Columns["Num"].HeaderText = "Num";

                dataGridViewReject.Columns["Num"].ValueType = typeof(int);
                dataGridViewReject.Columns["Num"].Width = 60;
                dataGridViewReject.Columns["Num"].Frozen = true;

                DataGridViewColumn col1 = new DataGridViewColumn();

                col1.Name = "PartId";
                col1.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col1);
                dataGridViewReject.Columns["PartID"].HeaderText = "PartID";

                dataGridViewReject.Columns["PartID"].ValueType = typeof(int);
                dataGridViewReject.Columns["PartID"].Width = 60;
                //dataGridViewReject.Columns["PartID"].SortMode = DataGridViewColumnSortMode.Automatic;

                DataGridViewColumn col2 = new DataGridViewColumn();

                col2.Name = "Diameter";
                col2.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col2);
                dataGridViewReject.Columns["Diameter"].HeaderText = "Diameter";

                dataGridViewReject.Columns["Diameter"].ValueType = typeof(string);
                dataGridViewReject.Columns["Diameter"].Width = 70;

                DataGridViewColumn col3 = new DataGridViewColumn();

                col3.Name = "InspTop";
                col3.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col3);
                dataGridViewReject.Columns["InspTop"].HeaderText = "Inspect Top";

                dataGridViewReject.Columns["InspTop"].ValueType = typeof(string);
                dataGridViewReject.Columns["InspTop"].Width = 70;
                dataGridViewReject.Columns["InspTop"].SortMode = DataGridViewColumnSortMode.Automatic;

                DataGridViewColumn col4 = new DataGridViewColumn();

                col4.Name = "InspFront";
                col4.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col4);
                dataGridViewReject.Columns["InspFront"].HeaderText = "Inspect Front";

                dataGridViewReject.Columns["InspFront"].ValueType = typeof(string);
                dataGridViewReject.Columns["InspFront"].Width = 70;
                dataGridViewReject.Columns["InspFront"].SortMode = DataGridViewColumnSortMode.Automatic;


                DataGridViewColumn col5 = new DataGridViewColumn();
                col5.Name = "Count";
                col5.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col5);
                dataGridViewReject.Columns["Count"].HeaderText = "Count";

                dataGridViewReject.Columns["Count"].ValueType = typeof(string);
                dataGridViewReject.Columns["Count"].Width = 70;

                DataGridViewColumn col6 = new DataGridViewColumn();
                col6.Name = "Weldone";
                col6.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col6);
                dataGridViewReject.Columns["Weldone"].HeaderText = "Weldone";

                dataGridViewReject.Columns["Weldone"].ValueType = typeof(string);
                dataGridViewReject.Columns["Weldone"].Width = 70;

                DataGridViewColumn col7 = new DataGridViewColumn();
                col7.Name = "Color";
                col7.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col7);
                dataGridViewReject.Columns["Color"].HeaderText = "Color";

                dataGridViewReject.Columns["Color"].ValueType = typeof(string);
                dataGridViewReject.Columns["Color"].Width = 70;

                DataGridViewColumn col8 = new DataGridViewColumn();
                col8.Name = "Comment";
                col8.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col8);
                dataGridViewReject.Columns["Comment"].HeaderText = "Comment";

                dataGridViewReject.Columns["Comment"].ValueType = typeof(string);
                dataGridViewReject.Columns["Comment"].Width = 200;

                DataGridViewColumn col9 = new DataGridViewColumn();
                col9.Name = "Time";
                col9.CellTemplate = cell;
                dataGridViewReject.Columns.Add(col9);
                dataGridViewReject.Columns["Time"].HeaderText = "Time";

                dataGridViewReject.Columns["Time"].ValueType = typeof(string);
                dataGridViewReject.Columns["Time"].Width = 100;

                //this.dataGridViewStations.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewStations_CellClick);
                //this.dataGridViewStations.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewStations__DataError);




                dataGridViewReject.Enabled = true;
                dataGridViewReject.Refresh();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void InitGridRejectSum()
        {

            try
            {

                dataGridViewRejectSum.Columns.Clear();
                dataGridViewRejectSum.Rows.Clear();
                dataGridViewRejectSum.AutoResizeColumns();
                dataGridViewRejectSum.AllowUserToAddRows = false;

                dataGridViewRejectSum.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                DataGridViewCell cell = new DataGridViewTextBoxCell();

                DataGridViewColumn col0 = new DataGridViewColumn();
                col0.Name = "MeasureID";
                col0.CellTemplate = cell;
                dataGridViewRejectSum.Columns.Add(col0);
                dataGridViewRejectSum.Columns["MeasureID"].HeaderText = "Measure ID";

                dataGridViewRejectSum.Columns["MeasureID"].ValueType = typeof(string);
                dataGridViewRejectSum.Columns["MeasureID"].Width = 100;
                dataGridViewRejectSum.Columns["MeasureID"].Frozen = true;

                DataGridViewColumn col1 = new DataGridViewColumn();

                col1.Name = "MeasureSum";
                col1.CellTemplate = cell;
                dataGridViewRejectSum.Columns.Add(col1);
                dataGridViewRejectSum.Columns["MeasureSum"].HeaderText = "Measure Sum";

                dataGridViewRejectSum.Columns["MeasureSum"].ValueType = typeof(string);
                dataGridViewRejectSum.Columns["MeasureSum"].Width = 100;

                DataGridViewColumn col2 = new DataGridViewColumn();

                col2.Name = "InspectID";
                col2.CellTemplate = cell;
                dataGridViewRejectSum.Columns.Add(col2);
                dataGridViewRejectSum.Columns["InspectID"].HeaderText = "Inspect ID";

                dataGridViewRejectSum.Columns["InspectID"].ValueType = typeof(string);
                dataGridViewRejectSum.Columns["InspectID"].Width = 100;

                DataGridViewColumn col3 = new DataGridViewColumn();

                col3.Name = "InspectSum";
                col3.CellTemplate = cell;
                dataGridViewRejectSum.Columns.Add(col3);
                dataGridViewRejectSum.Columns["InspectSum"].HeaderText = "Inspect Sum";

                dataGridViewRejectSum.Columns["InspectSum"].ValueType = typeof(string);
                dataGridViewRejectSum.Columns["InspectSum"].Width = 100;


                dataGridViewRejectSum.Enabled = true;
                dataGridViewRejectSum.Refresh();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        #endregion
        int RejectPartId = 0;
        //int RejectAll = 0;
        //int RejectMeasure = 0;
        //int RejectInspect = 0;
        int AllPartsCount = 0;
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            RejectPartId = 0;
            inv.settxt(txtPartIDRej, RejectPartId.ToString());
            position outpos = new position();
            outpos.Error = "";
            GetReject(RejectPartId, 0, "", "", "", "", "", "", "", false);
            InitGridReject();
            //inv.settxt(txtRejected, "0");
            //inv.settxt(txtByMeasure, "0");
            //inv.settxt(txtByInspect, "0");
            //inv.settxt(txtAllParts, "0");
            //RejectAll = 0;

            //RejectMeasure = 0;
            //RejectInspect = 0;
            AllPartsCount = 0;

        }
        private RobotFunctions.CommReply GetReject(int rejectid, int partid, string diam, string InspTop, string InspFront, string count, string Weldone, string color, string comment, bool addrow = false)
        {
            RobotFunctions.CommReply commreply = new RobotFunctions.CommReply();
            try
            {

                String st = "";
                //if (rejmeasurestring == "" && rejinspectstring == "") st = "General";

                //else st = "ERROR";
                //if (rejmeasurestring != "") rejmeasurestring = "Yes";
                // if (rejinspectstring != "") rejinspectstring = "Yes";
                string time = DateTime.Now.ToString("d.M.yyyy") + " " + DateTime.Now.ToString("HH:mm:ss.f");

                if (addrow)
                {
                    this.Invoke(new Action(() => dataGridViewReject.Rows.Add(rejectid, partid, diam, InspTop, InspFront, count, Weldone, color, comment, time)));
                }
                //save to file
                if (addrow)
                {
                    string[] s = new string[1];
                    //s[0] = "RejectID:" + rejectid.ToString() + " /" + "PartId:" + partid.ToString() + " /" + "AllParts:" + txtAllParts.Text + " /" + "Rejected:" + txtRejected.Text + " /" + "Measure:" + rejmeasurestring.ToString() + " /" + "Inspect:" + rejinspectstring.ToString() + " /" + comment + " /" + time;
                    string s1 = txtOrder.Text.Trim();
                    if (s1 == "") s1 = "Test";
                    //WriteToFile(Path.GetDirectoryName(Application.ExecutablePath) + "\\AutomationIni\\RejectLog\\ " + s1 + ".ini", s);
                }



                return commreply;
            }
            catch (Exception ex)
            {
                commreply.result = false;
                commreply.comment = ex.Message;
                return commreply;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPartDiam.Text == "") { MessageBox.Show("ENTER TARGET ANGLE!"); return; }
                SaveDiamData((int)Single.Parse(txtPartDiam.Text));
            }
            catch (Exception ex) { MessageBox.Show("ERROR SAVE " + ex.Message); }
        }

        private void btnSaveSt_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPosStations.Text == "" || cmbPosStations.SelectedIndex < 0)
                {
                    MessageBox.Show("ENTER MEASURE STATION!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                btnSaveSt.Enabled = false;
                //read positions
                CommReply reply = new CommReply();
                Beckhoff.SendPlcParms ParmsPlc = new Beckhoff.SendPlcParms();
                reply.result = false;
                Array.Resize<Single>(ref ParmsPlc.SendParm, 11);
                for (int i = 0; i < ParmsPlc.SendParm.Length; i++) ParmsPlc.SendParm[i] = 0;

                ParmsPlc.SendParm[0] = MyStatic.CamsCmd.CurrentPos;//curr pos
                ParmsPlc.SendParm[1] = 0;//cam
                ParmsPlc.SendParm[10] = 0.5f;//tmout

                reply = Beckhoff_Gen_1.PlcSendCmd(StartAddressSendGen_1, ParmsPlc, true);
                if (!reply.result || reply.data.Length < 11 || (reply.data[2] == 0 && reply.data[3] == 0 && reply.data[4] == 0 && reply.data[5] == 0 && reply.data[6] == 0))
                {
                    MessageBox.Show("ERROR READ DATA FROM BECKHOFF!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    btnSaveSt.Enabled = true;
                    return;
                }
                inv.set(upDownControlPosSt, "UpDownValue", 0f);

                Single[] axis = new Single[5];
                for (int i = 0; i < axis.Length; i++) { axis[i] = reply.data[i + 2]; }


                DialogResult res = MessageBox.Show("Save positions for " + cmbPosStations.Text, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (res != DialogResult.Yes)
                {
                    return;
                }

                switch (cmbPosStations.SelectedIndex)
                {
                    case 0://Top Inspection

                        upDwnCam2z.UpDownValue = axis[0] - master.Ax1_Work;//ax1 cam2 top upDwnCam2z
                        upDwnLamp1Z.UpDownValue = axis[1] - master.Ax2_Work; //ax2 H1 upDwnLamp1Z
                        upDwnFootWorkR.UpDownValue = axis[3] - master.Ax4_Work;//ax4 footer rot upDwnFootWorkR
                        upDwnFootWorkTopX.UpDownValue = axis[4] - (master.Ax5_Work + (Single.Parse(txtPartLength.Text) - master.Length));//ax5 footer X upDwnFootWorkTopX
                        SaveItem();
                        SaveDiamData((int)Single.Parse(txtPartDiam.Text), true);
                        break;
                    case 1://Front Inspection

                        upDwnCam1x.UpDownValue = axis[2] - master.Ax3_Front;//ax3 cam1 front upDwnCam1x
                        upDwnFootWorkFrontX.UpDownValue = axis[4] - (master.Ax5_Front - Cam1xFocusOffset + (Single.Parse(txtPartLength.Text) - master.Length));//ax5 footer X upDwnFootWorkFrontX
                        SaveItem();
                        SaveDiamData((int)Single.Parse(txtPartDiam.Text), true);
                        break;
                    case 2://Measure Weldone
                        upDwnFootWeldX.UpDownValue = axis[4] - master.Ax5_Weldone;//ax5 footer X upDwnFootWeldX first time write to  Diam_data.ini [diam]weldX
                        SaveItem();
                        SaveDiamData((int)Single.Parse(txtPartDiam.Text), true);
                        break;
                    case 3://Measure Diameter
                        upDwnFootDX.UpDownValue = axis[4] - master.Ax5_Diam;//ax5 footer X upDwnFootDX
                        SaveItem();
                        SaveDiamData((int)Single.Parse(txtPartDiam.Text), true);
                        break;
                    default:
                        break;
                }
                //save offsets to item
                //SaveItem();
                btnSaveSt.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR READ DATA! " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                btnSaveSt.Enabled = true;
                return;
            }

        }

        private async void btnFrontViev_Click(object sender, EventArgs e)
        {
            try
            {
                //if (chkVisionSim.Checked) return;
                inv.set(panel10, "Enabled", false);

                var task1 = Task.Run(() => StartCycleInspectFront(0, 1));
                await task1;
                WebComm.CommReply reply = task1.Result;
                if (reply.result)
                {

                    inv.set(panel10, "Enabled", true);
                }
                else
                {
                    MessageBox.Show("ERROR CHECK FrontView! ");
                }

                inv.set(panel10, "Enabled", true);

                Thread.Sleep(2);
                //}
            }
            catch (Exception ex) { MessageBox.Show("ERROR CHECK Front View! " + ex.Message); inv.set(panel10, "Enabled", true); }
        }
        Single DeltaFront = 0;
        private async Task<WebComm.CommReply> StartCycleInspectFront(Single deltaFront, int snap)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            WebComm.CommReply rep1 = new WebComm.CommReply();
            rep1.result = true;
            rep1.data = new Single[10];
            
            try
            {
                
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<=  MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                //move cam1 to front
                Thread.Sleep(500);
                //for (int i = 0; i < 1; i++)
                //{
                    if (Cam1xFocusOffset == null || Cam1xFocusOffset == Double.NaN)
                    {
                        Cam1xFocusOffset = 0;
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> ERROR focus offset" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    }
                    Single Pos5 = master.Ax5_Front + upDwnFootWorkFrontX.UpDownValue - Cam1xFocusOffset + (Single.Parse(txtPartLength.Text) - master.Length);
                    Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos2 = master.Ax2_Work;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                    Single Pos4 = master.Ax4_Front + deltaFront;// + upDwnFootWorkR.UpDownValue;
                    Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue;
                    //lamp1
                    BitArray lamp = new BitArray(new bool[8]);
                    lamp[1] = true; //lamp2
                    byte[] Lamps = new byte[1];
                    lamp.CopyTo(Lamps, 0);


                    Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                    Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                    var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                    await task;
                    if (!task.Result.result)
                    {
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> ERROR MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                        MyStatic.bExitcycle = true;
                        MyStatic.bExitcycleNow = true;
                        ErrorMess = "ERROR MOVE TO FRONT POSITION!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                        FooterStationAct.AxisInAction = false;
                        FooterStationAct.State = (int)MyStatic.E_State.InError;
                        reply.result = false;
                        return reply;

                    }

                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=>  FINI MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    Thread.Sleep(20);
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Front Count inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    FooterStationAct.AxisInAction = false;
                    var task6 = Task.Run(() => FrontCamCount(snap));
                    await task6;
                    reply = task6.Result;
                    if (reply.result && reply.comment != null && reply.comment != "")
                    {

                        reply.result = true;
                        //string s = reply.comment.Remove(0, 3);
                        //string[] ss = s.Split(',');
                        //if (ss.Length >= 4 && ss[0] == ((int)MyStatic.InspectCmd.FrontCamCount).ToString() && ss[1] == "1" && int.Parse(ss[2]) == upDwnCount.UpDownValue)
                        //{
                        //    inv.settxt(lblInspect, "Vision Front Ready");
                        //    reply.result = true;
                        //    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front Count fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //    return reply;

                        //}
                        //else
                        //{
                        //    //if(master.Ax4_Front + deltaFront > 250) deltaFront = deltaFront - 80.0f;//rotate 90 deg
                        //    //else deltaFront = deltaFront + 80.0f;
                        //    reply.result = false;
                        //    return reply;
                        //}
                    }
                    else
                    {
                        reply.result = false;
                        return reply;
                    }
                //}


               
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front Count fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Vision front ERROR:" + err.Message);
                return reply;
            }
        }
        private async Task<WebComm.CommReply> StartSnapFront(Single deltaFront, int snap)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            WebComm.CommReply rep1 = new WebComm.CommReply();
            rep1.result = true;
            rep1.data = new Single[10];

            try
            {

                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<=  MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                //move cam1 to front
                Thread.Sleep(500);
                //for (int i = 0; i < 1; i++)
                //{
                if (Cam1xFocusOffset == null || Cam1xFocusOffset == Double.NaN)
                {
                    Cam1xFocusOffset = 0;
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> ERROR focus offset" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                }
                Single Pos5 = master.Ax5_Front + upDwnFootWorkFrontX.UpDownValue - Cam1xFocusOffset + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Front + deltaFront;// + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[1] = true; //lamp2
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);


                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                if (!task.Result.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> ERROR MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    ErrorMess = "ERROR MOVE TO FRONT POSITION!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    reply.result = false;
                    return reply;

                }

                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=>  FINI MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                Thread.Sleep(20);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front<= Front Snap for inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                FooterStationAct.AxisInAction = false;
                var task6 = Task.Run(() => FrontCamSnap(snap));
                await task6;
                reply = task6.Result;
                //if (reply.result)
                //{

                //    reply.result = true;
                    
                //}
                //else
                //{
                //    reply.result = false;
                //    return reply;
                //}
                //}



                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision Front=> Front Snap fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Vision front snap ERROR:" + err.Message);
                return reply;
            }
        }


        #region ---------Invoke--------------
        public async Task<object> test_Inv(Func<object> mymethod)
        {
            //Func<object> funcnew = mymethod;

            //Func<Task<object>> func = async () => await  GetFuncResult(mymethod);


            Func<Task<object>> func = async () => await GetFuncResult(mymethod);
            if (InvokeRequired)
            {
                var task = (Task<object>)Invoke(func);
                await task.ConfigureAwait(false);
                if (task.IsCompleted) return task.Result;
                else
                {
                    MessageBox.Show("Task not complited!");
                    return false;
                }
            }
            else
            {
                return await func();

            }
        }

        public static async Task<object> GetFuncResult(Func<object> mymethod)
        {
            //Func<object> func = mymethod;
            //object result = mymethod();//func()

            if (mymethod() is Task task)
            {
                await task.ConfigureAwait(false);
                return ((dynamic)task).Result;
            }

            return mymethod();// result;
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            Stopwatch sp = new Stopwatch();
            sp.Restart();
            //button9.Enabled = false;

            Func<object> func = () => test2(1500, "ok");
            var task = Task.Run(() => test_Inv(func));
            await task.ConfigureAwait(true);
            bool b = (bool)task.Result;

            sp.Stop();
            //button9.Enabled = true;
            Thread.Sleep(1000);
            txtClient.Text = txtClient.Text + sp.ElapsedMilliseconds.ToString();
        }


        private async Task<bool> test2(int n, string s)
        {
            try
            {

                for (int i = 0; i < n; i++)
                {
                    await Task.Delay(1);
                    txtClient.Text = txtClient.Text + s + " " + i.ToString() + "\r\n";

                }

                return true;

            }
            catch (Exception ex) { return false; }
        }
        #endregion  ---------invoke--------------

        private async void btnKCL_Click(object sender, EventArgs e)
        {
            try
            {
                string str = cmbKCL.Text;
                var task = Task.Run(() => CheckKCL(str));
                await task;
            }
            catch (Exception ex) { }
        }
        private async Task<RobotFunctions.CommReply> CheckKCL(string strkcl)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {


                FanucWeb.SendRobotParms Parms = new FanucWeb.SendRobotParms();

                var task1 = Task.Run(() => FW1.RunKCLFanuc(strkcl));
                await task1;

                reply = task1.Result;

                if (reply.result)
                {

                }
                else
                {

                    MessageBox.Show("ROBOT1 COMMUNICATION ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return reply;
                }

                //send parameters


                reply.result = true;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("ROBOT COMMUNICATION ERROR:" + err.Message);
                return reply;
            }
        }


        private WebComm.CommReply FrontCamSnap(int snap)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.FrontCamSnap).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.FrontCamSnap;
                Parms.comment = "FrontCamSnap";
                Parms.timeout = 5;
                Array.Resize<Single>(ref Parms.SendParm, 4);
                //16
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = snap;// 1-"snap-inspect.jpg"; 2-"snap-inspect_Prev.jpg"
                Parms.SendParm[3] = 5.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.FrontCamSnap;
                rep1 = WC2.RunCmd(Parms);

                //if (!rep1.result) 
                //    MessageBox.Show("FRONT CAMERA SNAP ERROR!", "ERROR", MessageBoxButtons.OK,
                //               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                if (rep1.result) reply.result = true;
                else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("CAMERA SNAP ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply FrontCamCount(int snap)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.FrontCamCount).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.FrontCamCount;
                Parms.comment = "FrontCamCount";
                Parms.timeout = 20;
                Array.Resize<Single>(ref Parms.SendParm, 4);
                //16
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = snap;// 1- snap-inspect,2- snap-inspect_Prev
                Parms.SendParm[3] = 20.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.FrontCamCount;
                rep1 = WC2.RunCmd(Parms);

                //if (!rep1.result) MessageBox.Show("FRONT CAMERA COUNT ERROR1!", "ERROR", MessageBoxButtons.OK,
                //               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                if (rep1.result) reply.result = true;
                else reply.result = false;
                reply.comment = rep1.comment;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("FRONT CAMERA COUNT ERROR2:" + err.Message);
                return reply;
            }
        }
        
        private async Task<WebComm.CommReply> FrontCamOnOff(int On)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.FrontCamOnOff).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.FrontCamOnOff;
                Parms.comment = "FrontCamOnOff";
                Parms.timeout = 10;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                //16
                Parms.SendParm[1] = On;// on/off camera
                Parms.SendParm[2] = 10.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.FrontCamOnOff;
                //rep1 = WC2.RunCmd(Parms);
                var task1 = Task.Run(() => WC2.RunCmd(Parms));
                await task1;
                rep1 = task1.Result;
                if (!rep1.result)
                {
                    //MessageBox.Show("FRONT CAMERA ON/OFF ERROR!", "ERROR", MessageBoxButtons.OK,
                    //           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    Thread.Sleep(500);
                    var task2 = Task.Run(() => WC2.RunCmd(Parms));
                    await task2;
                    rep1 = task2.Result;
                }


                if (rep1.result) reply.result = true;
                else reply.result = false;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("Front CAMERA on/off ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply InspectFront(int bmpnum)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                WC3.SetControls1(txtClient, this, null, "CognexComm", CognexAddr);
                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.CognexFront).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.CognexFront;
                Parms.comment = "Inspect Front";
                Parms.timeout = 20;
                Array.Resize<Single>(ref Parms.SendParm, 4);
                //16
                Parms.SendParm[1] = 1;// general speed
                Parms.SendParm[2] = bmpnum;
                Parms.SendParm[3] = 20.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CognexFront;
                rep1 = WC3.RunCmd(Parms);

                //if (!rep1.result) MessageBox.Show("FRONT INSPECT ERROR!", "ERROR", MessageBoxButtons.OK,
                //               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                if (rep1.result) reply.result = true;
                else reply.result = false;
                reply.comment = rep1.comment;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("FRONT INSPECT ERROR:" + err.Message);
                return reply;
            }
        }
        private WebComm.CommReply CheckColor()
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {


                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.CheckColor).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckColor;
                Parms.comment = "Check Color";
                Parms.timeout = 10;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                //16
                Parms.SendParm[1] = upDwnColor.UpDownValue;// general speed
                Parms.SendParm[2] = 10.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.CheckColor;
                rep1 = WC1.RunCmd(Parms);

                //if (!rep1.result) MessageBox.Show("COLOR INSPECT ERROR!", "ERROR", MessageBoxButtons.OK,
                //               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                if (rep1.result) reply.result = true;
                else reply.result = false;
                reply.comment = rep1.comment;
                return reply;

            }
            catch (Exception err)
            {
                reply.result = false;

                MessageBox.Show("FRONT INSPECT ERROR:" + err.Message);
                return reply;
            }
        }
        int nfrontCount = 0;
        private async void btnCountFront_Click(object sender, EventArgs e)
        {
            WebComm.CommReply reply = new WebComm.CommReply();
            nfrontCount = 0;
            try
            {
                //while (true)
                //{
                inv.set(btnCountFront, "Enabled", false);

                WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                var task2 = Task.Run(() => FrontCamCount(1));
                await task2;
                reply = task2.Result;
                inv.set(btnCountFront, "Enabled", true);
                //
                if (reply.result)
                {
                    inv.settxt(lblInspect, "Vision Front Fini");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Snap Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (reply.comment != null && reply.comment.StartsWith("cmd"))
                    {
                        string s = reply.comment.Remove(0, 3);
                        string[] ss = s.Split(',');
                        if (ss.Length >= 3 && ss[0] != ((int)MyStatic.InspectCmd.FrontCamCount).ToString())
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=>Vision Count Error1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                            InspectStationAct.VisionInAction = false;
                            return;

                        }
                        if (ss.Length >= 3 && ss[1] != "1")
                        {
                            inv.settxt(lblInspect, "Vision Front Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Vision Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;

                            InspectStationAct.VisionInAction = false;
                            return;

                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Front Ready");
                            //InspectStationAct.State = (int)MyStatic.E_State.FrontSnapReady;
                            nfrontCount = int.Parse(ss[2]);
                            InspectStationAct.VisionInAction = false;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Vision Count=" + nfrontCount.ToString() + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            return;
                        }

                    }
                    else
                    {
                        inv.settxt(lblInspect, "Vision Front Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Error5" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;

                        InspectStationAct.VisionInAction = false;
                        return;
                    }

                }
                else
                {
                    inv.settxt(lblInspect, "Vision Front Reject");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Front Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;

                    InspectStationAct.VisionInAction = false;
                    return;
                }
                //
                inv.set(btnCountFront, "Enabled", true);

                Thread.Sleep(2);
                //}
            }
            catch (Exception ex) { MessageBox.Show("ERROR COMMUNICATION" + ex.Message); inv.set(btnCountFront, "Enabled", true); }
        }

        private async void btnOneCycle_Click(object sender, EventArgs e)
        {
            System.Console.Beep();
            inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
            FanucSpeed = trackSpeed.Value;
            System.Media.SystemSounds.Beep.Play();
            chkStep.Checked = false;
            ErrorMess = "";
            btnOneCycle.Enabled = false;
            nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
            nFrontCountUpDwn = (int)upDwnCount.UpDownValue;
            nColorUpDwn = (int)upDwnColor.UpDownValue;
            DeltaFront = 0;
            FrontRotate = Single.Parse(txtFrontRotate.Text);
            try
            {
                DialogResult res = MessageBox.Show("One Cycle?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (res != DialogResult.Yes)
                {
                    btnOneCycle.Enabled = true;
                    return;
                }
                ClearStations();
                btnCycleStop.BackColor = Color.LightGray;
                   
                ControlsEnable(false);
                MyStatic.bExitcycle = false;
                MyStatic.bExitcycleNow = false;
                MyStatic.bEmpty = false;

                DeltaFront = 0;
                ErrorFront = 0;


                MyStatic.bOneCycle = true;
                var task = Task.Run(() => Task.Run(() => RunMain()));
                await task;
                if (!task.Result)
                {
                    MessageBox.Show("RUN CYCLE Stopped");
                    btnOneCycle.Enabled = true;
                    return;
                }
                if (MyStatic.bExitcycleNow)
                {
                    MessageBox.Show("EXIT CYCLE WITH ERROR " + "\r\n" + ErrorMess);
                    btnOneCycle.Enabled = true;
                    return;
                }
                Thread.Sleep(1000);
                btnOneCycle.Enabled = true;
                return;
                //}
            }
            catch (Exception err)
            {
                MessageBox.Show("RUN ONE CYCLE ERROR:" + err.Message);
                btnOneCycle.Enabled = true;
                return;
            }
        }

        private async void btnCognexTop_Click(object sender, EventArgs e)
        {
            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                if (InspectStationAct.VisionInAction) return;
                btnCognexTop.Enabled = false;

                inv.settxt(lblInspect, "Vision Inspect");
                InspectStationAct.VisionInAction = true;

                //move footer to work top
                int axis = 0;
                Single Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; // master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);


                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                if (FooterStationAct.AxisInAction) { InspectStationAct.VisionInAction = false; return; }
                FooterStationAct.AxisInAction = true;
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff<= Move work" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                var task2 = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task2;
                if (!task2.Result.result)
                {

                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE FOOTER TO WORK POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                    InspectStationAct.VisionInAction = false;


                    MessageBox.Show("ERROR MOVE FOOTER TO WORK POSITION! Exit cycle", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    return;
                }
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> fini Move work" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));

                if (task2.Result.data.Length >= 9 && task2.Result.data[8] != 1)
                {
                    MyStatic.bExitcycleNow = true;
                    MyStatic.bExitcycle = true;
                    //Stopwatch sw1 = new Stopwatch();
                    //sw1.Restart();
                    //while(RobotLoadAct.OnGrip2_State != (int)(int)MyStatic.E_State.Empty || sw1.ElapsedMilliseconds < 10000 ) { Thread.Sleep(500); }
                    //Thread.Sleep(1000);
                    
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Beckhoff=> NO PART IN FOOTER" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    MessageBox.Show("ERROR NO PART IN FOOTER! Exit cycle", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    InspectStationAct.VisionInAction = false;
                    ClearStations();
                    MyStatic.bExitcycleNow = true;
                    btnCognexTop.Enabled = true;
                    return;
                }

                //
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<= Run Inspect" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                bool withcognex = false;
                var task = Task.Run(() => StartCycleInspectVision(withcognex));
                var task1 = Task.Run(() => StartCycleInspectCognex());
                await task;
                reply = task.Result;
                await task1;
                reply = task1.Result;
                btnCognexTop.Enabled = true;


            }
            catch (Exception ex)
            { btnCognexTop.Enabled = true; }
        }

        private async void btnDataToVision_Click(object sender, EventArgs e)
        {
            try
            {

                inv.set(btnDataToVision, "Enabled", false);
                //WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                var task1 = Task.Run(() => DataToVision());
                await task1;
                WebComm.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR SEND DATA TO VISION!");
                }
                var task2 = Task.Run(() => DataToFront());
                await task2;
                reply = task2.Result;
                if (!reply.result)
                {
                    MessageBox.Show("ERROR SEND DATA TO FRONT!");
                }
                inv.set(btnDataToVision, "Enabled", true);

            }
            catch (Exception ex) { MessageBox.Show("ERROR SEND DATA TO VISION " + ex.Message); inv.set(btnDataToVision, "Enabled", true); }

        }

        private async void btnToDiameter_Click(object sender, EventArgs e)
        {
            WebComm.CommReply reply = new WebComm.CommReply();
            WebComm.CommReply rep1 = new WebComm.CommReply();
            rep1.result = false;
            rep1.data = new Single[10];
            Single Diam = 0;
            try
            {
                inv.set(btnToDiameter, "Enabled", false);
                ControlsEnable(false);
                var task1 = Task.Run(() => StartCycleInspectDiam());
                await task1;
                reply = task1.Result;

                if (reply.result)
                {
                    inv.settxt(lblInspect, "Vision Diameter Fini");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Diameter Inspect Fini" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    if (reply.comment != null && reply.comment.StartsWith("cmd"))
                    {
                        string s = reply.comment.Remove(0, 3);
                        string[] ss = s.Split(',');
                        if (ss.Length >= 3 && ss[0] != ((int)MyStatic.InspectCmd.CheckDiam).ToString())
                        {
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=>Diameter Vision Error1" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                            InspectStationAct.VisionInAction = false;
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;
                            MessageBox.Show("Vision Diameter Error", "ERROR", MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            inv.set(btnToDiameter, "Enabled", true);
                            return;

                        }
                        if (ss.Length >= 3 && ss[1] != "1")
                        {
                            inv.settxt(lblInspect, "Vision Diameter Reject");
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Diameter Vision Error2" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                            InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;
                            InspectStationAct.VisionInAction = false;
                            MessageBox.Show("Vision Diameter Error " + "Cam1 xFocusOffset = " + Cam1xFocusOffset.ToString() + " Diameter = " + ss[2], "ERROR", MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            inv.set(btnToDiameter, "Enabled", true);
                            return;

                        }
                        else
                        {
                            inv.settxt(lblInspect, "Vision Diameter Ready");
                            InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;

                            InspectStationAct.VisionInAction = false;
                            MessageBox.Show("Vision Diameter Ready " + "Cam1 xFocusOffset = " + Cam1xFocusOffset.ToString() + " Diameter = " + ss[2], "ERROR", MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            inv.set(btnToDiameter, "Enabled", true);
                            ControlsEnable(true);
                            return;
                        }

                    }
                    else
                    {
                        inv.settxt(lblInspect, "Vision Diameter Reject");
                        Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Vision Error5" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                        //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                        InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                        InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;

                        InspectStationAct.VisionInAction = false;
                        MessageBox.Show("Vision Diameter Reject", "ERROR", MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        inv.set(btnToDiameter, "Enabled", true);
                        ControlsEnable(true);
                        return;
                    }

                }
                else
                {
                    inv.settxt(lblInspect, "Vision Diameter Reject");
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> Inspect Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    //InspectStationAct.State = (int)MyStatic.E_State.RejectMeasure;
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionDiam] = (int)MyStatic.E_State.DiamFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.VisionDiam] = true;
                    InspectStationAct.VisionInAction = false;
                    MessageBox.Show("Vision Diameter Reject", "ERROR", MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.set(btnToDiameter, "Enabled", true);
                    ControlsEnable(true);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR MOVE FOOTER TO Diameter POSITION! ", "ERROR", MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                inv.set(btnToDiameter, "Enabled", true);
            }
            inv.set(btnToDiameter, "Enabled", true);
        }

        private async void btnToFront_Click(object sender, EventArgs e)
        {
            WebComm.CommReply reply = new WebComm.CommReply();
            WebComm.CommReply rep1 = new WebComm.CommReply();
            rep1.result = true;
            rep1.data = new Single[10];
            try
            {
                inv.set(btnToFront, "Enabled", false);
                ControlsEnable(false);
                Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision<=  MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                //move cam1 to front

                Single Pos5 = master.Ax5_Front + upDwnFootWorkFrontX.UpDownValue - Cam1xFocusOffset + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Front;// + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[1] = true; //lamp2
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);


                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                if (!task.Result.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision=> ERROR MOVE  TO front POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.State[(int)MyStatic.InspectSt.VisionFront] = (int)MyStatic.E_State.FrontSnapFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    MessageBox.Show("ERROR MOVE FOOTER to Frontr POSITION! ", "ERROR", MessageBoxButtons.OK,
                                 MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    reply.result = false;
                    inv.set(btnToFront, "Enabled", true);
                    return ;

                }
                ControlsEnable(true);
            }
            catch (Exception err)
            {
                reply.result = false;
                inv.set(btnToFront, "Enabled", false);

                MessageBox.Show("ERROR MOVE FOOTER to Frontr POSITION! ", "ERROR", MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private async void btnToWeldone_Click(object sender, EventArgs e)
        {
            //move to weldon ax5 and ax4
            try
            {
                inv.set(btnToWeldone, "Enabled", false);
                ControlsEnable(false);
                int axis = 0;
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());

                Single Pos5 = master.Ax5_Weldone + upDwnFootWeldX.UpDownValue + weldone.weldX;// + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; ;// master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);

                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                CommReply reply1 = task.Result;
                if (!reply1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Weldon=> ERROR MOVE FOOTER TO weldon POSITION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                    InspectStationAct.State[(int)MyStatic.InspectSt.Weldone] = (int)MyStatic.E_State.WeldonFini;
                    InspectStationAct.Reject[(int)MyStatic.Reject.Beckhoff] = true;

                    //InspectStationAct.VisionInAction = false;
                    //MyStatic.bReset = true;
                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    ErrorMess = "Error move Footer to Weldon Position!" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")";

                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;

                    MyStatic.bExitcycle = true;
                    MyStatic.bExitcycleNow = true;
                    MessageBox.Show("ERROR MOVE FOOTER to Weldone POSITION! ", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.set(btnToWeldone, "Enabled", true);
                    ControlsEnable(true);
                    return;
                }
                ControlsEnable(true);
            }
            catch (Exception ex) {
                MessageBox.Show("ERROR MOVE FOOTER to Weldone POSITION! ", "ERROR", MessageBoxButtons.OK,
                                     MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                inv.set(btnToWeldone, "Enabled", true);
            }
        }

        private async void btnCycleNoRobot_Click(object sender, EventArgs e)
        {
            try
            {
               Stopwatch sw = Stopwatch.StartNew();
                System.Console.Beep();
                inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
                FanucSpeed = trackSpeed.Value;
                System.Media.SystemSounds.Beep.Play();
                chkStep.Checked = false;
                ErrorMess = "";
                btnOneCycle.Enabled = false;
                nDiameterCheckUpDwn = (int)upDwnNdiam.UpDownValue;
                nFrontCountUpDwn = (int)upDwnCount.UpDownValue;
                nColorUpDwn = (int)upDwnColor.UpDownValue;
                DialogResult res = MessageBox.Show("One Cycle?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (res != DialogResult.Yes)
                {
                    btnOneCycle.Enabled = true;
                    return;
                }

                btnCycleStop.BackColor = Color.LightGray;
                ClearStations();
                ControlsEnable(false);
                MyStatic.bExitcycle = false;
                MyStatic.bExitcycleNow = false;
                MyStatic.bEmpty = false;
                MyStatic.bOneCycle = true;
                cntLongPart = 0;
                btnClearStations_Click(sender, e);
                //
                Single t1 = 0;
                RobotLoadAct.InAction = false;
                InspectStationAct.InAction = false;
                InspectStationAct.VisionInAction = false;
                InspectStationAct.VisionFrontInAction = false;
                InspectStationAct.SuaInAction = false;
                FooterStationAct.AxisInAction = false;
                //MyStatic.bExitcycleNow = false;
                InspectStationAct.WeldonInAction = false;
                inv.settxt(txtCycleTime, "");
                sw.Restart();
                var task = Task.Run(() => CycleNoRobot());
                await task;
                inv.settxt(txtCycleTime, (sw.ElapsedMilliseconds / 1000.0f).ToString("0.000"));
                ControlsEnable(true);
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR !" +ex.Message, "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private async Task<bool> CycleNoRobot()
        {
            Stopwatch sw = new Stopwatch();
            try
            {

                sw.Restart();
                inv.settxt(txtCycleTime, (sw.ElapsedMilliseconds / 1000.0).ToString("0.00"));
                bInspectLongPart = false;
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                MyStatic.InitFini = false;

                Task.Run(() => frmMain.newFrmMain.ListAdd3("=========Start Cycle============" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                dFile.WriteLogFile("===========Start Cycle==============");
                if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                {
                    MessageBox.Show("ERROR IN PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    return false;
                }
                //check robot
                var task1 = Task.Run(() => CheckComm());
                await task1;
                RobotFunctions.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR IN ROBOT COMMUNICATION!", "ERROR", MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return false;
                }
                //save data
                //save vision data 
                var task33 = Task.Run(() => DataVisionSave());
                await task33;
                WebComm.CommReply reply33 = task33.Result;
                if (!reply33.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Error Save Vision Data!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return false;
                }
                var task34 = Task.Run(() => DataFrontSave());
                await task34;
                reply33 = task34.Result;
                if (!reply33.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Error save front data!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return false;
                }
                //home
                var task2 = Task.Run(() => RobotHome());
                await task2;

                bool rep1 = task2.Result;

                if (!rep1)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("Robot HOME Error!", "ERROR", MessageBoxButtons.OK,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return false;
                }
                //check communication with vision
                //if (!chkVisionSim.Checked)
                //{
                    WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                    var task3 = Task.Run(() => CheckComm1(1));
                    await task3;
                    WebComm.CommReply rep2 = task3.Result;
                    inv.set(btnRead, "Enabled", true);
                    if (!rep2.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("Error Vision Communication!", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        ControlsEnable(true);
                        return false;
                    }
                    WC2.SetControls1(txtClient, this, null, "VisionFrontComm", FrontCamAddr);
                    var task31 = Task.Run(() => CheckComm1(2));
                    await task31;
                    WebComm.CommReply rep21 = task31.Result;
                    inv.set(btnRead, "Enabled", true);
                    if (!rep21.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("Error Front cam Communication!", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        ControlsEnable(true);
                        return false;
                    }
                    //cognex
                    WC3.SetControls1(txtClient, this, null, "CognexComm", CognexAddr);
                    var task32 = Task.Run(() => CheckComm1(3));
                    await task32;
                    rep21 = task32.Result;
                    inv.set(btnRead, "Enabled", true);
                    if (!rep21.result)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        MessageBox.Show("Error Cognex Communication!", "ERROR", MessageBoxButtons.OK,
                              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        ControlsEnable(true);
                        return false;
                    }
                //}
                //set lights
                CommReply rep = new CommReply();


                //check beckhoff
                var task12 = Task.Run(() => RunAxisStatus(6));
                await task12;
                rep = task12.Result;
                if (!rep.result)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR READ BECKHOFF STATUS " + "\r");
                    return false;
                }
                if (rep.data[8] == 0)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR AIR PRESSURE " + "\r");
                    return false;
                }
                if (rep.data[9] == 0)
                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ERROR MOTORS POWER " + "\r");
                    return false;
                }
                //robot
                RobotLoadAct.OnGrip1_State = (int)MyStatic.E_State.Empty;

                InspectStationAct.State[(int)MyStatic.InspectSt.Footer] = (int)MyStatic.E_State.Occupied;
                InspectStationAct.SuaState = (int)MyStatic.E_State.Occupied;
                inv.settxt(txtGrip3num, txtGrip1num.Text);
                InspectStationAct.OnFooterGrip3_PartID = RobotLoadAct.OnGrip1_PartID;
                //Inspected_PartID = RobotLoadAct.OnGrip1_PartID;
                inv.settxt(txtGrip1num, "");
                RobotLoadAct.OnGrip1_PartID = -1;
                InspectStationAct.OnFooterGrip3_PartID = 1;
                partData[InspectStationAct.OnFooterGrip3_PartID].Position = (int)MyStatic.E_State.OnInsp;

                //send data to vision
                //WC1.SetControls1(txtClient, this, null, "VisionComm", CameraAddr);
                var task21 = Task.Run(() => DataToVision());
                await task21;
                WebComm.CommReply reply1 = task21.Result;
                if (!reply1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision <=" + "ERROR DEND DATA TO VISION" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                }
                var task41 = Task.Run(() => DataToFront());
                await task41;
                reply1 = task41.Result;
                if (!reply1.result)
                {
                    Task.Run(() => frmMain.newFrmMain.ListAdd3("Vision <=" + "ERROR DEND DATA TO FRONT" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                }
                ///////////////////
                RobotLoadAct.InAction = false;
                InspectStationAct.InAction = false;
                //robot fini
                if (FooterStationAct.AxisInAction) return false;
                FooterStationAct.AxisInAction = true;
                inv.set(btnFooterWork,"Enabled",false);
                inv.set(btnFooterWork1, "Enabled", false);
                //SetTraficLights(0, 0, 0, 0);//
                int axis = 0;
                if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                {
                    MessageBox.Show("ERROR PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.set(btnFooterWork, "Enabled", false);
                    inv.set(btnFooterWork1, "Enabled", false);
                    FooterStationAct.AxisInAction = true;
                    return false;
                }
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                Single Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);


                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task;
                if (!task.Result.result)
                {
                    //SetTraficLights(0, 0, 1, 0);//red ight
                    MessageBox.Show("FOOTER WORK ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    inv.set(btnFooterWork, "Enabled", false);
                    inv.set(btnFooterWork1, "Enabled", false);
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;

                }
                Thread.Sleep(200);
                //SetTraficLights(0, 1, 0, 0);//yellow/green
                ControlsEnable(false);
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InWork;


                //main loop
                while (true)
                {
                    SetTraficLights(1, 0, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    Thread.Sleep(20);

                    if (MyStatic.bReset)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========RESET Cycle============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        MessageBox.Show("Error Reset!", "ERROR", MessageBoxButtons.OK,
                             MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }
                    if (MyStatic.bExitcycleNow && !RobotLoadAct.InAction && !InspectStationAct.VisionInAction && !InspectStationAct.SuaInAction)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========Exit Cycle WITH ERROR============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        MessageBox.Show("EXIT CYCLE WITH ERROR!", "ERROR", MessageBoxButtons.OK,
                             MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }
                    if (FooterStationAct.State == (int)MyStatic.E_State.InError)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========Footer Error!============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        MessageBox.Show("FOOTER ERROR!", "ERROR", MessageBoxButtons.OK,
                             MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return false;
                    }

                    // if ((!RobotLoadAct.InAction && !MyStatic.bReset) || TaskRobot == null || TaskRobot.Status != TaskStatus.Running) { TaskRobot = Task.Run(() => RobotMainTask()); }
                    if ((!InspectStationAct.VisionInAction && !MyStatic.bReset) || TaskVision == null || TaskVision.Status != TaskStatus.Running) { TaskVision = Task.Run(() => VisionMainTask()); }
                    if ((!InspectStationAct.SuaInAction && !MyStatic.bReset) || TaskSua == null || TaskSua.Status != TaskStatus.Running) { TaskSua = Task.Run(() => CognexMainTask()); }
                    if ((!InspectStationAct.WeldonInAction && !MyStatic.bReset) || TaskWeldon == null || TaskWeldon.Status != TaskStatus.Running) { TaskWeldon = Task.Run(() => WeldonMainTask()); }
                    if (!MyStatic.bReset && (TaskRefresh == null || (TaskRefresh.Status != TaskStatus.Running))) TaskRefresh = Task.Run(() => RefreshLables());
                    if ((!InspectStationAct.VisionFrontInAction && !MyStatic.bReset) || TaskVisionFront == null || TaskVisionFront.Status != TaskStatus.Running) { TaskVisionFront = Task.Run(() => VisionFrontMainTask()); }

                    if (MyStatic.bReset)
                    {
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                        Task.Run(() => newFrmMain.ListAdd3("==========RESET Cycle============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                        return false;
                    }

                    Thread.Sleep(100);
                    if (MyStatic.bExitcycle) break;
                    if (MyStatic.bExitcycle &&
                    FooterStationAct.State == (int)MyStatic.E_State.InHome && !RobotLoadAct.InAction) break;
                    //if (MyStatic.bExitcycleNow && !RobotLoadAct.InAction) break;
                    if (MyStatic.bExitcycleNow) break;
                    if (!InspectStationAct.VisionInAction && FooterStationAct.State == (int)MyStatic.E_State.InHome && !InspectStationAct.SuaInAction && !InspectStationAct.WeldonInAction
                        && (InspectStationAct.SuaState == (int)MyStatic.E_State.SuaFrontFini || !chkInspectFront.Checked)) break;





                }
                Task.Run(() => frmMain.newFrmMain.ListAdd3("EXIT CYCLE" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                MyStatic.bRobotLoadRunning = false;

                MyStatic.bExitcycle = true;
                //home

                MyStatic.bStartcycle = false;
                Task.Run(() => newFrmMain.ListAdd3("==========Stop Cycle============" + "//" + DateTime.Now.ToString("HH:mm:ss.fff"), frmMain.newFrmMain.txtAutoLog, false));
                sw.Stop();
                inv.settxt(txtCycleTime, (sw.ElapsedMilliseconds / 1000.0).ToString("0.00"));

                RefreshLables();

                ControlsEnable(true);
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                return false;

            }
            catch (Exception err)
            {
                MessageBox.Show("RUN ONE CYCLE ERROR:" + err.Message);
                //btnOneCycle.Enabled = true;
                return false;

            }
        }

        private async void btnIscarEnable_Click(object sender, EventArgs e)
        {

            //WebComm.CommReply reply = new WebComm.CommReply();
            try
            {
                ControlsEnable(false);
                var task = Task.Run(() => VisionIscarPort(1));
                await task;
                if(!task.Result) MessageBox.Show("Vision Iscar Port Enable ERROR!", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                ControlsEnable(true);
                //WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                //Parms.cmd = ((int)MyStatic.InspectCmd.CheckComm).ToString();
                //Parms.DebugTime = 1000;
                //Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckComm;
                //Parms.comment = "Port";
                //Parms.timeout = 5;
                //Array.Resize<Single>(ref Parms.SendParm, 3);
                ////16
                //Parms.SendParm[1] = 1;//enable
                //Parms.SendParm[2] = 5.0f;// 0.5f;//timeout
                //WebComm.CommReply rep1 = new WebComm.CommReply();

                //    Parms.SendParm[0] = (Single)MyStatic.InspectCmd.EnableComm;
                //    reply = WC1.RunCmd(Parms);

                //    if (!reply.result)
                //    {
                //        reply.result = false;
                //        MessageBox.Show("Vision Iscar Port ERROR!", "ERROR", MessageBoxButtons.OK,
                //                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //    }
                //    else reply.result = true;


            }
            catch (Exception err)
            {
                //reply.result = false;

                MessageBox.Show("vision Iscar Port ERROR:" + err.Message);
                ControlsEnable(true);

            }
        }

        private async void btnIscarDisable_Click(object sender, EventArgs e)
        {
            //WebComm.CommReply reply = new WebComm.CommReply();
            try
            {

                ControlsEnable(false);
                var task = Task.Run(() => VisionIscarPort(0));
                await task;
                if (!task.Result) MessageBox.Show("Vision Iscar Port Disable ERROR!", "ERROR", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                ControlsEnable(true);
                //WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                //Parms.cmd = ((int)MyStatic.InspectCmd.CheckComm).ToString();
                //Parms.DebugTime = 1000;
                //Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckComm;
                //Parms.comment = "Port";
                //Parms.timeout = 5;
                //Array.Resize<Single>(ref Parms.SendParm, 3);
                ////16
                //Parms.SendParm[1] = 0;//disable
                //Parms.SendParm[2] = 5.0f;// 0.5f;//timeout
                //WebComm.CommReply rep1 = new WebComm.CommReply();

                //Parms.SendParm[0] = (Single)MyStatic.InspectCmd.EnableComm;
                //reply = WC1.RunCmd(Parms);

                //if (!reply.result)
                //{
                //    reply.result = false;
                //    MessageBox.Show("Vision Iscar Port ERROR!", "ERROR", MessageBoxButtons.OK,
                //                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //}
                //else reply.result = true;


            }
            catch (Exception err)
            {
                //reply.result = false;
                ControlsEnable(true);
                MessageBox.Show("vision Iscar Port ERROR:" + err.Message);

            }
        }
        private async Task<bool> VisionIscarPort(int state)
        {

            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {


                WebComm.SendRobotParms Parms = new WebComm.SendRobotParms();
                Parms.cmd = ((int)MyStatic.InspectCmd.CheckComm).ToString();
                Parms.DebugTime = 1000;
                Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckComm;
                Parms.comment = "Port";
                Parms.timeout = 5;
                Array.Resize<Single>(ref Parms.SendParm, 3);
                //16
                Parms.SendParm[1] = state;// 1-enable,0-disable
                Parms.SendParm[2] = 5.0f;// 0.5f;//timeout
                WebComm.CommReply rep1 = new WebComm.CommReply();

                Parms.SendParm[0] = (Single)MyStatic.InspectCmd.EnableComm;
                var task = Task.Run(() => WC1.RunCmd(Parms));
                await task;
                reply = task.Result;

                if (!reply.result)
                {
                    reply.result = false;
                    return false;
                    //MessageBox.Show("Vision Iscar Port ERROR!", "ERROR", MessageBoxButtons.OK,
                    //               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else { reply.result = true; return true; }


            }
            catch (Exception err)
            {
                reply.result = false;
                return false;

                //MessageBox.Show("vision Iscar Port ERROR:" + err.Message);

            }
        }

        private async void btnInitCycle_Click(object sender, EventArgs e)
        {
            try
            {
               var task = Task.Run(()=> InitCycle());
               await task;

            }
            catch (Exception ex){ }
        }

        private async void btnCheckColor_Click(object sender, EventArgs e)
        {
            WebComm.CommReply reply = new WebComm.CommReply();
            try
            {
                var task = Task.Run(() => CheckColor());
                await task;
                reply = task.Result;
               
                if (!reply.result) MessageBox.Show("COLOR INSPECT ERROR!", "ERROR", MessageBoxButtons.OK,
                               MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            }
            catch (Exception ex) { }
        }

        private async void btnCyclicTest_Click(object sender, EventArgs e)
        {
            try
            {
                //Stopwatch sw = new Stopwatch();
                //sw.Restart();
                bExitcycleNoRobot = false;
                System.Console.Beep();
                inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
                FanucSpeed = trackSpeed.Value;
                System.Media.SystemSounds.Beep.Play();
                chkStep.Checked = false;
                ErrorMess = "";
                btnOneCycle.Enabled = false;
                DialogResult res = MessageBox.Show("Cyclic Test?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                if (res != DialogResult.Yes)
                {
                    btnOneCycle.Enabled = true;
                    return;
                }
                MyStatic.bExitcycleNow = false;
                MyStatic.bReset = false;
                MyStatic.bExitcycle = false;
                

                btnCycleStop.BackColor = Color.LightGray;
                while (!bExitcycleNoRobot && !MyStatic.bReset)
                {
                    
                    //inv.settxt(txtCycleTime, (sw.ElapsedMilliseconds / 1000.0).ToString("0.00"));
                    this.Invoke(new Action(() => ClearStations()));
                    ControlsEnable(false);
                    MyStatic.bExitcycle = false;
                    MyStatic.bExitcycleNow = false;
                    MyStatic.bEmpty = false;
                    MyStatic.bOneCycle = true;
                    cntLongPart = 0;
                   
                    
                    //
                    Single t1 = 0;
                    RobotLoadAct.InAction = false;
                    InspectStationAct.InAction = false;
                    InspectStationAct.VisionInAction = false;
                    InspectStationAct.SuaInAction = false;
                    FooterStationAct.AxisInAction = false;
                    //MyStatic.bExitcycleNow = false;
                    InspectStationAct.WeldonInAction = false;
               
                        FooterStationAct.AxisInAction = false;
                        //sw.Restart();
                        //inv.settxt(txtCycleTime, (0).ToString("0.00"));
                        inv.set(txtCycleTime, "Refresh", true);
                        var task = Task.Run(() => CycleNoRobot());
                        await task;
                        bool b = task.Result;
                        //inv.settxt(txtCycleTime, (sw.ElapsedMilliseconds / 1000.0).ToString("0.00"));
                        //inv.set(txtCycleTime, "Refresh", true);
                    //sw.Stop();
                    if (bExitcycleNoRobot || MyStatic.bReset) 
                        break;
                        Thread.Sleep(3000);
                    
                    cntLongPart = 0;
                }
                ControlsEnable(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR !" + ex.Message, "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private async void btnSavePosInPR_Click(object sender, EventArgs e)
        {
            try
            {
                                              
                int robot = 0;
                int id = 0;//base position
                robot = MyStatic.RobotLoad;
                id = 40;//test
                
                var task1 = WriteCurrentPosition(robot, id);
                await task1;
                RobotFunctions.CommReply reply = task1.Result;
                if (!reply.result)
                {
                    MyStatic.bExitcycle = true;
                    MyStatic.bReset = true;
                    RobotLoadAct.InAction = false;

                    return;
                }
            }
            catch (Exception ex) { }
        }

        private async void btnTeachPos_Click(object sender, EventArgs e)
        {
            //-----read tray corrections--------
            //execute in step:send to robot PR[temp] coord
            //and send menu button R[65] R[66]
            //go to TP and run Macro
            //jog robot to position
            //read current pos and compare with send coord
            //teach with master part D=16mm L=92mm order=7271583 item=5667916

            try
            {

                MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;
                inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
                FanucSpeed = trackSpeed.Value;
                int robot = 0;
                int id = 0;//base position
                int teach = 0;
                action = 0;

                if (cmbPosition.Text == "PR[02] Pick Tray") action = 1;
                else if (cmbPosition.Text == "PR[04] Place Inspection") action = 2;
                else if (cmbPosition.Text == "PR[05] Pick Inspection") action = 3;
                else if (cmbPosition.Text == "PR[03] Place Tray") action = 4;
                else if (cmbPosition.Text == "PR[06] Place Reject") action = 5;
                //else if (cmbPosition.Text == "PR[25] Air Clean") action = 6;
                else
                {
                    MessageBox.Show("SELECT POSITION TO TEACH!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    ControlsEnable(true);
                    return;
                }

                int PR = 0;
                string st = cmbPosition.Text.Trim().Substring(3,2);
                PR = int.Parse(st);
                              

                basepos = new position();
                baseOrg = new position();
                inv.set(chkStep, "Checked", true);
                SetTraficLights(0, 1, 0, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                switch (action)
                {
                    case 1://pick tray

                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickTrayOrg.id;
                        RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                        RobotLoadPoints.PickTray = RobotLoadPoints.PickTrayOrg;
                        baseOrg = RobotLoadPoints.PickTrayOrg;//with pallet without corrections
                        
                        var task = Task.Run(() => PickFromTray(baseOrg));
                        await task;
                        fini = task.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = 1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Pick Tray" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            RobotLoadAct.InAction = false;
                            //teaching pos

                           
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PICK FROM TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 2://Place Inspection
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PlaceInspect.id;
                        fini = new RobotFunctions.CommReply();
                        baseOrg = RobotLoadPoints.PlaceInspect;//with pallet without corrections
                        
                        var task10 = Task.Run(() => PlacePartInsp(baseOrg));
                        await task10;
                        fini = task10.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Place Inspection");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLace Inspection ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 3://Pick Inspection
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickInspect.id;
                        fini = new RobotFunctions.CommReply();
                        baseOrg = RobotLoadPoints.PickInspect;//with pallet without corrections
                        
                        var task11 = Task.Run(() => PickPartInsp(baseOrg));
                        await task11;
                        fini = task11.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip2_PartID = 1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Pick Inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Pick Inspection");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                            
                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PICK INSPECTION ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;

                    case 4://place tray
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PlaceTrayOrg.id;

                        fini = new RobotFunctions.CommReply();

                        /////////////////////
                        RobotLoadPoints.PlaceTray = RobotLoadPoints.PlaceTrayOrg;
                        baseOrg = RobotLoadPoints.PlaceTrayOrg;//with pallet without corrections
                       
                        //////////////
                        var task12 = Task.Run(() => PlaceTray(baseOrg));
                        await task12;
                        fini = task12.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip2_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Tray" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Place Tray");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLACE TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    case 5://place reject
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickTrayOrg.id;
                        fini = new RobotFunctions.CommReply();

                        RobotLoadPoints.PlaceReject = RobotLoadPoints.PlaceRejectOrg;
                        baseOrg = RobotLoadPoints.PlaceRejectOrg;
                        var task13 = Task.Run(async () => PlaceReject(basepos));
                        await task13;
                        fini = task13.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip2_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Reject" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Pick Tray");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                                if (fini.data.Length > 3 && fini.data[2] == 3)
                                {
                                    MessageBox.Show("WITH Teach Pendant JOG ROBOT TO THE POSITION AND PRESS OK! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 1;
                                }
                                else if (fini.data.Length > 3 && fini.data[2] == 4)
                                {
                                    MessageBox.Show("ROBOT PROGRAM ABORTED! ", "SETUP", MessageBoxButtons.OK,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                    teach = 0;
                                }
                            
                            //reset start robot
                            //read current pos
                            //set corrections
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLACE REJECT ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        break;
                    
                    }
                //read current pos
                //return;
                if (chkStep.Checked && teach == 1)
                {

                    if (MyStatic.Robot == MyStatic.RobotLoad)
                    {
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.Currpos.id;
                    }

                    //var task1 = ReadPosition(robot, id);
                    //await task1;
                    var task1 = Task.Run(() => ReadCurPos(RobotLoadPoints.Currpos.id));
                    await task1;
                    RobotFunctions.CommReply reply = task1.Result;
                    if (!reply.result)
                    {
                        MyStatic.bExitcycle = true;
                        MyStatic.bReset = true;
                        RobotLoadAct.InAction = false;
                        SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0

                        MessageBox.Show("READ POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                           MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    position pos = new position();
                    pos.x = reply.data[2];
                    pos.y = reply.data[3];
                    pos.z = reply.data[4];
                    pos.r = reply.data[7];
                    Double dx = -baseOrg.x + pos.x;
                    Double dy = -baseOrg.y + pos.y;
                    Double dz = -baseOrg.z + pos.z;
                    Double dr = -baseOrg.r + pos.r;
                    if (Math.Abs(dx) > 30 || Math.Abs(dy) > 30 || Math.Abs(dz) > 50)
                    {

                        MessageBox.Show("CORRECTIONS TOO LARGE. EXIT!" + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r'
                            , "ERROR", MessageBoxButtons.OK,
                                                                   MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                        return;
                    }
                    DialogResult res = new DialogResult();
                    switch (action)
                    {
                        
                        case 1:

                             res = MessageBox.Show("Save Position Pick ? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r'+
                            "Correction R = " + dr.ToString("0.00") + '\r' ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res != DialogResult.Yes)
                            {
                                //save current position to PR
                                RobotLoadAct.InAction = false;
                                ControlsEnable(true);
                                return;
                            }
                            break;
                        case 2:

                            res = MessageBox.Show("Save Position Place Inspection? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res != DialogResult.Yes)
                            {
                                //save current position to PR
                                RobotLoadAct.InAction = false;
                                ControlsEnable(true);
                                return;
                            }
                            break;
                        case 3:

                            res = MessageBox.Show("Save Position Pick Inspection? " + '\r' +
                           "Correction X = " + dx.ToString("0.00") + '\r' +
                           "Correction Y = " + dy.ToString("0.00") + '\r' +
                           "Correction Z = " + dz.ToString("0.00") + '\r' +
                           "Correction R = " + dr.ToString("0.00") + '\r'
                               ,
                                  "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res != DialogResult.Yes)
                            {
                                //save current position to PR
                                RobotLoadAct.InAction = false;
                                ControlsEnable(true);
                                return;
                            }
                            break;

                        case 4:

                             res = MessageBox.Show("Save Position Place Tray? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res != DialogResult.Yes)
                            {
                                //save current position to PR
                                RobotLoadAct.InAction = false;
                                ControlsEnable(true);
                                return;
                            }
                            break;
                        case 5:

                            res = MessageBox.Show("Save Position Place Reject? " + '\r' +
                            "Correction X = " + dx.ToString("0.00") + '\r' +
                            "Correction Y = " + dy.ToString("0.00") + '\r' +
                            "Correction Z = " + dz.ToString("0.00") + '\r' +
                            "Correction R = " + dr.ToString("0.00") + '\r'
                                ,
                                   "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            if (res != DialogResult.Yes)
                            {
                                //save current position to PR
                                RobotLoadAct.InAction = false;
                                ControlsEnable(true);
                                return;
                            }
                            break;
                    }
                   
                    var task3 = Task.Run(() => ResetProgram());
                    await task3;
                    reply = task3.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR RESET TASK!");
                        ControlsEnable(true);
                        btnRobotStart.Enabled = true;
                        return;
                    }

                    var task4 = Task.Run(() => RunProgram("TP_MAIN"));
                    await task4;
                    reply = task4.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR RUN ROBOT!");
                        ControlsEnable(true);
                        btnRobotStart.Enabled = true;
                        return;
                    }
                    //save current position to PR
                    Thread.Sleep(1000);
                    robot = MyStatic.RobotLoad;
                    var task20 = WriteCurrentPosition(robot, PR);
                    await task20;
                    reply = task20.Result;
                    if (!reply.result)
                    {
                        Thread.Sleep(500);
                        var task22 = WriteCurrentPosition(robot, PR);
                        await task22;
                        reply = task22.Result;
                        if (!reply.result)
                        {
                            RobotLoadAct.InAction = false;
                            MessageBox.Show("ERROR SAVE POSITION TO PR" + PR.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            return;
                        }

                       
                    }
                    var task21 = Task.Run(() => ReadPos());
                    await task21;
                    reply = task21.Result;
                    if (!reply.result)
                    {
                        MessageBox.Show("ERROR READ ROBOT POSITIONS!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    ControlsEnable(true);
                }

                ControlsEnable(true);



            }
            catch (Exception err)
            {
                SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                MessageBox.Show("ROBOT READ POSITION ERROR:" + err.Message);
            }
        }

        private async void btnEmpty_Click(object sender, EventArgs e)
        {
            
                System.Console.Beep();
                System.Media.SystemSounds.Beep.Play();
                chkStep.Checked = false;
                inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
                FanucSpeed = trackSpeed.Value;
                ErrorMess = "";
                try
                {
                    DialogResult res = MessageBox.Show("Start Empty Cycle?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (res != DialogResult.Yes)
                    {
                        btnEmpty.Enabled = true;
                        return;
                    }


                    btnCycleStop.BackColor = Color.LightGray;

                    ControlsEnable(false);
                    MyStatic.bExitcycle = false;
                    MyStatic.bExitcycleNow = false;
                    MyStatic.bEmpty = true;
                    MyStatic.bOneCycle = false;
                    cntLongPart = 0;
                    var task = Task.Run(() => Task.Run(() => RunMain()));
                    await task;
                    if (!task.Result)
                    {
                        MessageBox.Show("RUN CYCLE Stopped");
                        return;
                    }
                    if (MyStatic.bExitcycleNow)
                    {
                        MessageBox.Show("EXIT CYCLE WITH ERROR " + "\r\n" + ErrorMess);
                        return;
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception ex) { }
            
        }

        private void chkStep_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStep.Checked)
            {
                try
                {
                    if (WV2 != null )
                    {
                       WV2.Source = new Uri(urlrobot1 + "panel10.stm");
                        //tabPage17.Show();
                        //if (this.tabPage17 != null)
                        //{
                        //    int loc = tabControl1.SelectedIndex;
                        //    this.tabControl1.TabPages.Insert(loc, this.tabPage17);
                        //}
                        tabControl1.SelectedTab = tabPage17;

                    }
                   
                }
                catch (Exception ex)
                {
                    WV2.Source = new Uri("about:blank");
                    //tabPage17.Hide();
                    //this.tabControl1.TabPages.Remove(this.tabPage17);
                    tabControl1.SelectedTab = tabPage7;
                }

            }
            else
            {
                try
                {

                    WV2.Source = new Uri("about:blank");
                    tabControl1.SelectedTab = tabPage7;
                }
                catch (Exception ex)
                {
                    
                }
            }

        }

        private async void btnPutInspect_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res1 = MessageBox.Show("PUT PART WITH ROBOT INTO FOOTER ?", "Warning", MessageBoxButtons.YesNo,
                               MessageBoxIcon.Warning);
                if (res1 != DialogResult.Yes)
                {
                    return;
                }
                    MyStatic.bStartcycle = false;
                ControlsEnable(false);
                MyStatic.bReset = false;
                //footer home
                if (FooterStationAct.AxisInAction) return;
                FooterStationAct.AxisInAction = true;
                btnFooterHome.Enabled = false;
                btnFooterHome1.Enabled = false;
               
                int axis = 0;
                Single speed5 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed4 = (int.Parse(txtSpeedSt.Text) * axis_Parameters[3].Ax_Vmax) / 100.0f;
                var task12 = Task.Run(() => MoveFooterHome(speed5, speed4));
                await task12.ConfigureAwait(true);
                if (!task12.Result.result)
                {
                   
                    MessageBox.Show("FOOTER HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    return;
                }
                Thread.Sleep(200);
                //SetTraficLights(0, 1, 0, 0);//yellow/green
                btnFooterHome.Enabled = true;
                btnFooterHome1.Enabled = true;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InHome;
                //end footer home
                inv.settxt(txtSpeed, Math.Abs(trackSpeed.Value).ToString());
                FanucSpeed = trackSpeed.Value;
                int robot = 0;
                int id = 0;//base position
                chkStep.Checked = false;
                


                basepos = new position();
                baseOrg = new position();
                //pick tray

                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PickTrayOrg.id;
                        RobotFunctions.CommReply fini = new RobotFunctions.CommReply();
                        RobotLoadPoints.PickTray = RobotLoadPoints.PickTrayOrg;
                        int partid = TrayPartId;
                        var task1 = Task.Run(() => GetPickTray(partid, false));
                        await task1;
                        RobotFunctions.CommReply commreply = task1.Result;
                        if (commreply.result && commreply.data.Length >= 4)
                        {
                            baseOrg = RobotLoadPoints.PickTrayOrg;//with pallet without corrections
                            baseOrg.x = commreply.data[0];
                            baseOrg.y = commreply.data[1];
                            baseOrg.z = commreply.data[2];
                            baseOrg.r = commreply.data[3];
                            basepos.x = baseOrg.x + (Single)UpDwnX3.UpDownValue;
                            basepos.y = baseOrg.y + (Single)UpDwnY3.UpDownValue;
                            basepos.z = baseOrg.z + (Single)UpDwnZ3.UpDownValue;
                            basepos.r = baseOrg.r + (Single)UpDwnR3.UpDownValue;
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT GET POSITION ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        var task = Task.Run(() => PickFromTray(basepos));
                        await task;
                        fini = task.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = 1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Pick Tray" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Pick Tray");
                            RobotLoadAct.InAction = false;
                            
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PICK FROM TRAY ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                        
                   //Place Inspection
                        robot = MyStatic.RobotLoad;
                        id = RobotLoadPoints.PlaceInspect.id;
                        fini = new RobotFunctions.CommReply();
                        baseOrg = RobotLoadPoints.PlaceInspect;//with pallet without corrections
                        basepos.x = RobotLoadPoints.PlaceInspect.x + (Single)UpDwnX4.UpDownValue;
                        basepos.y = RobotLoadPoints.PlaceInspect.y + (Single)UpDwnY4.UpDownValue;
                        basepos.z = RobotLoadPoints.PlaceInspect.z + (Single)UpDwnZ4.UpDownValue;
                        basepos.r = RobotLoadPoints.PlaceInspect.r + (Single)UpDwnR4.UpDownValue;
                        var task10 = Task.Run(() => PlacePartInsp(basepos));
                        await task10;
                        fini = task10.Result;

                        if (fini.result)
                        {
                            //ControlsEnable(true);
                            RobotLoadAct.OnGrip1_PartID = -1;
                            Task.Run(() => frmMain.newFrmMain.ListAdd3("=>Fini Place Inspection" + "// (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", frmMain.newFrmMain.txtAutoLog, false));
                            dFile.WriteLogFile("Place Inspection");
                            RobotLoadAct.InAction = false;
                            //teaching pos

                           
                            
                        }
                        else
                        {
                            MyStatic.bExitcycle = true;
                            SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                            MessageBox.Show("ROBOT PLace Inspection ERROR!", "ERROR", MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            MyStatic.bReset = true;
                            RobotLoadAct.InAction = false;
                            return;
                        }
                //footer to work
                var task13 = Task.Run(() => RobotHome());
                await task13;

                bool rep1 = task13.Result;

                if (!rep1)

                {
                    SetTraficLights(0, 0, 1, 0);//(int green = 0, int yellow = 0, int red = 0, int buzzer = 0
                    MessageBox.Show("ROBOT1 HOME ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                //footer to work
                if (FooterStationAct.AxisInAction) return;
                FooterStationAct.AxisInAction = true;
                btnFooterWork.Enabled = false;
                btnFooterWork1.Enabled = false;
                //SetTraficLights(0, 0, 0, 0);//
                
                if (Single.Parse(txtPartLength.Text) <= 0 || master.Length <= 0)
                {
                    MessageBox.Show("ERROR PART LENGTH!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    btnFooterWork.Enabled = false;
                    FooterStationAct.AxisInAction = true;
                    return;
                }
                if (txtPartDiam.Text.Trim() == "" || txtPartDiam.Text.Trim() == "0") inv.settxt(txtPartDiam, master.Diameter.ToString());
                if (txtPartLength.Text.Trim() == "" || txtPartLength.Text.Trim() == "0") inv.settxt(txtPartLength, master.Length.ToString());
                Single Pos5 = master.Ax5_Work + upDwnFootWorkTopX.UpDownValue + (Single.Parse(txtPartLength.Text) - master.Length);
                Single Pos1 = master.Ax1_Work + upDwnCam2z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos2 = master.Ax2_Work;// + upDwnLamp1Z.UpDownValue;// - (Single.Parse(txtPartDiam.Text) - master.Diameter) / 2.0f;
                Single Pos4 = master.Ax4_Work + upDwnFootWorkR.UpDownValue;
                Single Pos3 = master.Ax3_Front + upDwnCam1x.UpDownValue; //master.Ax3_Work;
                //lamp1
                BitArray lamp = new BitArray(new bool[8]);
                lamp[0] = true; //lamp1
                byte[] Lamps = new byte[1];
                lamp.CopyTo(Lamps, 0);


                Single speed = (int.Parse(txtSpeedSt.Text) * axis_Parameters[4].Ax_Vmax) / 100.0f;
                Single speed1 = (int.Parse(txtSpeedSt.Text) / 100.0f);
                var task11 = Task.Run(() => MoveFooterWork(Pos5, speed, Pos1, Pos2, Pos4, speed1, Pos3, Lamps[0]));
                await task11;
                if (!task11.Result.result)
                {
                   
                    MessageBox.Show("FOOTER WORK ERROR!", "ERROR", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    btnFooterWork.Enabled = true;
                    FooterStationAct.AxisInAction = false;
                    FooterStationAct.State = (int)MyStatic.E_State.InError;
                    return;

                }
                Thread.Sleep(200);
                
                btnFooterWork.Enabled = true;
                btnFooterWork1.Enabled = true;
                FooterStationAct.AxisInAction = false;
                FooterStationAct.State = (int)MyStatic.E_State.InWork;
                ControlsEnable(true);




            }
            catch(Exception ex) { }
        }
    }
}