using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace EndmillHMI
{
    public class MyStatic
    {
        public static Boolean bSafeSpeed = false;
        public const int Stop = 3;
        //public static Single[] Parm=new Single[30];
        public static Boolean bReset = false;
        public static Boolean bPortReading = false;
        public static Boolean bExitcycle = false;
        public static Boolean bExitcycleNoRobot = false;

        public static Boolean bExitcycleNow = false;
        //public static Boolean bExitMaincycle = false;
        public static Boolean bStartcycle = false;
        public static Boolean bManual = false;
        public static Boolean bLast = false;
        public static Boolean bEmpty = false;
        public static Boolean bExitWeight = false;
        public static int Robot = 0;
        public const int RobotLoad = 1;
        //public const int RobotUnload = 2;

        public static Boolean bNotUpdatePlace = false;
        public static IntPtr MyHandle;
        public static String MyName;
        public static int MainWidth;
        public static int MainHeight;
        public static Boolean bOnePosition = false;
        public static DateTime CycleTime;
        public static Boolean bOneCycle = false;

        public static Boolean BigScreen = true;
        public static Boolean startApp = false;
        //public static int SlaveID=1;
        public static int TimerInterval = 100;
        public static Boolean ReadingIO = false;
        public static Boolean ReadingPlc1IO = false;
        public static Boolean ReadingPlc2IO = false;
        public static int SetOut = -1;
        public static int SetHandOut = -1;
        public static int SetPlc1Out = -1;
        public static int SetPlc2Out = -1;
        public static Boolean WaitReady = false;
        public static Boolean RobotErrorMess = false;

        public static Int16 SafeSpeed = 5;
        //siquence
        public const int sRobotCycle = 1;
        public const int sSendParms = 2;
        public const int sSendPos = 3;
        //public const int sMovePurch = 4;
        //public const int sMoveMaint = 5;
        public const int sJog = 6;
        public const int sOneCycle = 7;
        public Boolean bApplicationClosed = false;
        public static long StswTime = 0;
        public static int StswDelay = 20;
        public static Stopwatch Stsw = new Stopwatch();//$
        public static Stopwatch stw = new Stopwatch();//$

        //Toshiba

        public static Boolean chkDebug = false;
        //public static Boolean bRobot1finished = false;
        public static Boolean sRobot1_Init = false;
        public static Boolean bSocket = false;
        public static string GetString = "";
        public static Int16 sReplay_count = 0;

        public static int SetCard = -1;
        //public static Boolean WaitReady = false;

        public static Boolean RobotReady = false;
        public static Boolean ApplicationRunning = false;

        public static int RobotTPmode = 0;
        public struct RobotMode
        {
            public int mode;
            public int status;
            public bool mess;
            public bool RunAuto;
            public bool ReadIO;

        }

        public const int teaching = 2;
        public const int auto = 1;
        public const int stop = 1;
        public const int run = 2;
        public static RobotMode Robot1mode = new RobotMode();
        public static short Speed = 0;
        //public static int[] S = new int[10];//step function 
        //public static int[] S1 = new int[10];//step function 
        public static bool bToshibaComm = false;
        public static bool bToshibaRunning = false;



        public static int FanucTPact = 0;
        public static int TimeScale = 1;
        public static int trackSpeed = 5;

        public struct BathState
        {
            public bool Grip1_Loaded;
            public bool Grip2_Loaded;
            public bool RMS_Loaded;
            public bool HC_Loaded;
            public bool Scale_Loaded;
            public bool MsNotMeasure;
            public bool HcNotMeasure;

            public int BathID;
            public int Barcode;
            public Single Weight;
            public Single RMS;
            public Single HC;
            public bool BathReady;
            public Single X;
            public Single Y;
            public int order;
            public string MsSetting;
            public string HcSetting;
            public Single CoOrigin;
            public Single Dencity;
            public int WorkID;
            public int itemNumber;
            public Single HcNorm;
            public Single HcMin;
            public Single HcMax;
            public Single MsNorm;
            public Single MsMin;
            public Single MsMax;

        }
        //public static BathState Bath = new BathState();
        //public struct NewAction
        //{

        //    public static int PickPart;
        //    public static int InGrip;//0 or grip1 or grip2
        //    public static int InGrip1;///
        //    public static int InGrip2;///
        //    public static Single Weight;


        //    public static int MsAxis;//0 or MsAxisIn or MsAxisOut
        //    //public static bool MsAxisLoad;
        //    public static int MsAxisLoadID;
        //    public static bool MsNotMeasure;
        //    public static bool PlaceMs;
        //    public static int MsMeagure;//0  or MsInMeagure or MsFiniMeagure
        //    public static Single MsResult;
        //    public static bool PickeMs;

        //    public static int HcsAxis;//0  or HcAxisIn or HcAxisOut
        //    //public static bool HcAxisLoad;
        //    public static int HcAxisLoadID;
        //    public static bool HcNotMeasure;
        //    public static bool PlaceHc;
        //    public static int HcMeagure;//0  or HcInMeagure or HcFiniMeagure
        //    public static Single HcResult;
        //    public static bool PickHc;
        //    public static bool MeasureFini;
        //    public static bool PlacePallet;

        //}
        public struct Tab
        {
            public const int PickPart = 1;
            public static int InGrip1 = 2;///
            public static int InGrip2 = 3;//
            public const int Weight = 4;
            public const int MsAxis = 5;//0 or MsAxisIn or MsAxisOut
            public const int MsAxisLoad = 6;
            public const int MsNotMeasure = 7;
            public const int PlaceMs = 8;
            public const int MsMeagure = 9;//0  or MsInMeagure or MsFiniMeagure
            public const int MsResult = 10;
            public const int PickeMs = 11;
            public const int HcAxis = 12;//0  or HcAxisIn or HcAxisOut
            public const int HcAxisLoad = 13;
            public const int HcNotMeasure = 14;
            public const int PlaceHc = 15;
            public const int HcMeagure = 16;//0  or HcInMeagure or HcFiniMeagure
            public const int HcResult = 17;
            public const int PickHc = 18;
            public const int MeasureFini = 19;
            public const int PlacePallet = 20;
        }
        public struct Tab3
        {
            public const int Bath = 0;
            public const int Order = 1;
            public const int Hc = 2;
            public const int RMS = 3;
            public const int Ratio = 4;
            public const int Weight = 5;
        }

        public struct PartState
        {
            public static int Id;
            public static bool OnPallet;
            public static int InGrip;//0 or grip1 or grip2
            public static int Barcode;
            public static bool BarcodeFini;
            public static Single Weight;
            public static bool WeightFini;

            public static Single MsResult;
            public static int MsAxis;//0 or MsAxisIn or MsAxisOut
            public static bool MsNotMeasure;
            public static int MsMeagure;//0  or MsInMeagure or MsFiniMeagure

            public static Single HcResult;
            public static int HcAxis;//0  or HcAxisIn or HcAxisOut
            public static bool HcNotMeasure;
            public static int HcMeagure;//0  or HcInMeagure or HcFiniMeagure
            public static bool MeasureFini;

        }

        public struct BathAction
        {
            public static int RobotAction;
            public static int Grip1;
            public static int Grip2;
            public static int MsLoaded;
            public static Single MsResult;
            public static int HcLoaded;
            public static Single HcResult;
            public static int ID1;
            public static int ID2;
            public static int ID3;
            public static int NextID;
        }




        //public static PartState[] Partstate = new PartState[120];
        public const int Grip1 = 11;
        public const int Grip2 = 12;
        public const int MsAxisIn = 21;
        public const int MsAxisOut = 22;
        public const int MsInMeagure = 31;
        public const int MsFiniMeagure = 32;
        public const int HcAxisIn = 41;
        public const int HcAxisOut = 42;

        public const int HcInMeagure = 51;
        public const int HcFiniMeagure = 52;






        public static bool TwoBath = false;
        public static Color color = Color.White;
        public static int PartIndex = 0;
        public static bool bEndPallet1 = false;
        public static bool bEndPallet2 = false;
        public static bool TaskExecute = false;
        public static int BathIndex = 0;

        public enum RobotCmd
        {

            Parm = 10,
            WritePos = 11,
            jog = 12,
            teach = 13,
            readIO = 14,
            setIO = 15,
            MoveHome = 16,
            TPjog = 17,
            ReadPos = 18,
            SetTool = 19,
            MoveAway = 20,
            ContinueJog = 21,
            TeachTool = 23,
            PocketCalib = 24,
            TrayCalib = 25,
            GripperAct = 26,
            MoveMaint = 27,
            PickTray = 40,
            AirClean = 28,

            PlaceTray = 50,
            PickInspection = 60,
            PlaceInspection = 65,
            PlaceReject = 66,
            AboveInsp = 67,
            CycleAuto = 60,
            Empting = 70,
            MainProg = 94,
            Info = 95,
            StopCycle = 96,
            RunProgram = 97,
            AbortProgram = 98,
            CheckComm = 99,
            WriteReg = 22,
            Setup = 71

        }
        public enum InspectCmd
        {

            //Parm = 10,
            //WritePos = 11,
            StartWaitSua = 83,
            //StopSua = 84,
            //CheckSua = 89,
            Startvision = 94,
            StartCycle = 95,
            StopCycle = 96,
            //RunProgram = 97,
            //AbortProgram = 98,
            CheckComm = 99,
            //WriteReg = 22,
            //Setup = 71,
            CheckWeldone = 92,
            CheckDiam = 91,
            CheckFrontCam = 199,
            FrontCamSnap = 196,
            FrontCamCount = 195,
            InspectFront = 93,
            CheckColor = 97,
            CheckCognex = 59,
            StartCognex = 53,
            DataToVision = 90,
            DataVisionSave = 98,
            EnableComm = 80,
            InitCycle = 79,
            CognexFront = 52,
            FrontCamOnOff = 197,
            DataToFront = 190,
            DataFrontSave = 191,
            
        }

        //public static bool RobotInAction = false;


        public static bool FirstPallet = true;


        //fanuc
        public static bool bRobotLoadRunning = false;
        public static bool bRobotUnLoadRunning = false;
        public static bool RobotLoadInAction = false;
        public static bool RobotUnLoadInAction = false;
        public static bool CncInAction = false;
        public enum RobotState
        {
            PickFromTray = 1,
            PlaceInCnc = 2,
            PickFromCnc = 3,
            PlaceOnTray = 4,
            PlaceOnFlip = 5,
            PickFromFlip = 6
        }
        public struct RobotAction
        {
            public int Action;
            public int OnGrip1_PartID;
            public int OnGrip2_PartID;
            public bool InAction;
            public bool OnGrip1_Ready;
            public bool OnGrip2_Ready;
            public int OnGrip1_State;
            public int OnGrip2_State;


        }
        public enum Reject
        {
            VisionTop = 0,
            VisionFront = 1,
            VisionDiam = 2,
            SuaTop = 3,
            SuaFront = 4,
            Weldone  =5,
            VisionCount = 6,
            VisionColor = 7,
            Beckhoff = 8,
            Robot = 9


        }
        public enum InspectSt
        {
            VisionTop = 0,
            VisionFront = 1,
            VisionDiam = 2,
            SuaTop = 3,
            SuaFront = 4,
            Weldone = 5,
            VisionCount = 6,
            VisionColor = 7,
            Beckhoff = 8,
            Robot = 9,
            Footer = 10,


        }
        public struct VisionAction
        {
            public int Action;
            public bool InAction;
            public int OnFooterGrip3_PartID;
            public bool VisionInAction;
            public bool VisionFrontInAction;
            public int[] State;
            public int SuaState;
            public bool SuaInAction;
            public bool WeldonInAction;
            public int WeldonState;
            public bool[] Reject;

        }
        public struct AxisAction
        {
            public int Action;
            public bool InAction;
            public bool AxisInAction;
            public int State;
           
        }
        
        public enum CncStationState
        {
            Empty = -1,
            Occupied = 1,
            OneSideReady = 2,
            Ready = 3
        }
        //public const int empty = 1;
        //public const int Occupied = 2;
        //public const int OneSideReady = 3;
        //public const int Ready = 4;
        public struct CncStation
        {
            public int State;
            public int PartID;
            public bool Enable;

        }
        public struct Clamp
        {
            public bool On;
            public bool OnFini;


        }

        //public static bool LeftClampOn = false;
        //public static bool RightClampOn = false;
        public enum Partstate
        {
            OnPickTray = 1,
            OnRobotLoad = 2,
            InPocket = 3,
            InFlip = 4
        }
        //public const int OnPickTray = 1;
        //public const int OnRobotLoad = 2;
        //public const int InPocket = 3;
        //public const int InFlip = 4;
        public struct PartData
        {
            public int[] State;//ontray;onrobotload;inCNC;inflip;onrobotunload;ontrayout
            public Single pickX;
            public Single pickY;
            public Single pickZ;
            public Single pickR;
            public Single placeX;
            public Single placeY;
            public Single placeZ;
            public Single placeR;
            //public bool Ready;
            //public bool VisionReady;
            //public bool RejectMeasure;
            //public bool RejectSua;
            public int Num;
            public int Position;
            public bool[] Reject;
            public Single Diam;
            public Single Weldone;
            public Single Count;


        }
        public enum Camstate
        {
            Snap = 1,
            Finish = 2,
            Error = 3

        }
        public struct CameraData
        {
            public int State;
            public Single coordX;
            public Single coordY;
            public Single coordZ;
            public Single coordR;
            public bool Ready;


        }
        public static int SlaveID = 1;
        public static Int16 PlcTrayReady = 0;
        public static Int16 PlcSetReady = 1;
        public static Int16 PlcEndCycle = 3;
        public static Int16 PlcEndLoad = 0;//////////////////////////////////////
        public static Int16 PlcEndUnLoad = 1;
        public static Int16 PlcEndReset = 2;
        public static Int16 PlcEndReady = 4;
        public static Int16 PlcEndFanucStart = 5;

        public static Int16 MplcAdd = 1;
        public static Int16 MplcCycle = 12;
        public static Int16 MplcLoad = 10;
        public static Int16 MplcUnLoad = 11;
        public static Int16 MplcReset = 2;
        public static Int16 MplcSetReady = 7;
        public static Int16 MplcCheckReady = 13;
        public static Int16 MplcFanucReset = 14;
        public static Int16 MplcFanucStart = 15;
        public static Int16 MplcLampOn = 16;
        public static Int16 MplcLampOff = 17;
        public static Int16 MplcLampChange = 18;

        public static Int16 MplcWriteIO = 0;
        public static Int16 DplcError = 6;
        public static int Doffset = 0;//32768;
        public static int Moffset = 0;
        public static short Dstatus = 5;

        public const int StEmpty = 0;
        public const int StOccupied = 1;
        public const int StReady = 2;
        public const int StFlipped = 3;
        public const int StFlippedReady = 4;
        public const int StDisable = 5;

        public struct Station
        {
            public int State;// '0-empty,1-occupied,2-ready,3-flipped
            public int Num;
            public bool Act;
        }
        public const int M_ResetConv = 112;
        public const int M_FeedConv = 113;
        public const int M_ChangeConv = 114;
        public const int M_ClearConv = 115;
        public const int M_ResetConvFini = 2;
        public const int M_FeedConvFini = 3;
        public const int M_ChangeConvFini = 4;
        public const int M_ClearConvFini = 5;
        public const int M_ResetConvErr = 2;
        public const int M_FeedConvErr = 3;
        public const int M_ChangeConvErr = 4;
        public const int M_ClearConvErr = 5;


        public const int M_ConvErr = 9;


        public const int M_ResetPLC = 112;
        public const int M_ResetFini = 2;
        public const int M_LeftClose = 115;
        public const int M_LeftOpen = 118;
        public const int M_LeftTris = 114;
        public const int M_LeftClErr = 25;
        public const int M_LeftOpErr = 24;
        public const int M_LeftOpFini = 4;
        public const int M_LeftClFini = 5;

        public const int M_RightClose = 117;
        public const int M_RightOpen = 116;
        public const int M_RightClErr = 7;
        public const int M_RightOpFini = 6;
        public const int M_RightClFini = 7;


        public const int M_Flip0 = 400;
        public const int M_Flip180 = 401;
        public const int M_FlipErr = 9;
        public const int M_FlipFini = 0;

        public const int M_GripClose = 402;
        public const int M_GripOpen = 403;
        public const int M_GripErr = 10;
        public const int M_GripFini = 1;

        public const int M_IndexCNC = 113;
        public const int M_IndexCncFini = 3;
        public const int M_IndexCncErr = 3;


        public const int M_Trayx2 = 1;
        public const int M_ConvCheck = 116;
        public const int M_ConvCheckFini = 6;
        public const int M_ConvCheckErr = 10;
        public struct RobotData
        {
            public Single SpeedOvr;
            public Single NormalSpeed;
            public Single PickSpeed;
            public Single PlaceSpeed;
            public Single RobotAbove;
            public Single RobotAbovePick;
            public Single RobotAbovePlace;
            public Single DelayPick;
            public Single DelayPlace;
            public Single Step;
            public Single CheckGrip;
            public Single InvLock;
            public Single ToolOffX;
            public Single ToolOffY;
            public Single ToolOffZ;
            public Single ToolOffR;
            public string Gripper;


        }
        public static int PlcAddress1;
        public static int PlcAddress2;
        public static bool BeckhoffchkDebug = false;
        public struct CamsCmd
        {
            public const int Error = 103;
            public const int Parameters = 410;
            public const int Power = 411;
            public const int MoveRel = 412;
            public const int MoveVel = 413;
            public const int Stop = 414;
            public const int MoveAbs = 415;
            public const int Reset = 416;
            public const int CurrentPos = 417;
            public const int Status = 418;
            public const int ReadIO = 419;
            public const int Outputs = 420;
            public const int Valve = 421;
            public const int Lamps = 423;//0-nothing,1-on,2-off
            public const int MoveJog = 424;
            public const int MoveHome = 425;
            public const int MoveFooterHome = 429;
            public const int MoveFooterWork = 430;
            public const int MoveFooterWorkNoAir = 431;
            public const int MoveWork = 426;
            public const int Lights = 427;
            public const int MoveAll = 428;
            public const int IO = 501;
            



        }
        
        public struct StationAxis
        {
            public const int ROT_ST1 = 1;//rotation M1
            public const int X_ST1 = 2;//foscus M2
            public const int FZ_ST2 = 3;//focus M3
            public const int LZ_ST2 = 5;//light M5
            public const int FX_ST3 = 4;//side M4




        }
        public static bool bPower = false;
        public static Boolean ReadIOcont = false;
        public static Boolean InitFini = false;  
        public enum E_State
        {
            Empty = 0,
            PartReady = 2,
            
            Occupied = 1,//part on station not ready
            DiamFini = 6,
            SuaFini = 7,
            OnGrip1 = 8,
            OnGrip2 = 9,
            OnInsp = 10,
            OnTray = 11,
            OnTrayFini = 12,
            OnTrayReject = 13,
            WeldonFini = 14,
            InHome = 15,
            InWork = 16,
            InError = 17,
            
            WeldonDataFini = 19,
            FrontSnapFini = 20,
            TopSnapFini = 25,
            RejectWeldon = 18,
            RejectFrontCount = 21,
            Reject = 3,
            RejectMeasure = 4,
            RejectSuaTop = 22,
            RejectSuaFront = 23,
            RejectDiam = 24,
            SuaFrontFini = 27,
            ColorFini = 26,
            

        }
        public struct AxStatus
        {
            public int StandStill;
            public int Moving;
            public int Limit;
            public int Disable;
            
            public int Error;
            public int ErrorId;
            public int ReadErrorID;
            public Single Vmax;

        }
        public struct AxisParameters
        {
            public Single Ax_Home;
            public Single Ax_Min;
            public Single Ax_Max;
            public Single Ax_Vmax;
            public Single Ax_WorkTop;
            public Single Ax_WorkFront;
            public Single Ax_Weldone;
            public Single Ax_Diameter;

        }
        public struct Master
        {
            public Single Diameter;
            public Single diameter;
            public Single Weight;
            public Single Length;
            public Single LU;
            public Single cam2z;
            public Single Ax1_Work;
            public Single Ax2_Work;
            public Single Ax3_Work;//top
            public Single Ax4_Work;
            public Single Ax4_Front;
            public Single Ax5_Work;
            public Single Ax5_Weldone;
            public Single Ax5_Diam;
            public Single kRightEdge;
            public Single Ax5_Front;
            public Single Ax3_Front;//cam1 front
            public Single Ax5_RightEdge;


        }
        public struct Weldone
        {
            public Single weldX;
            public Single weldH;
            public Single angle;



        }
        public struct Lights
        {
            public int green;
            public int yellow;
            public int red;
            public int buzzer;


        }


    }
}
