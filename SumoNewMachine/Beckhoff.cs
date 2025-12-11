using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwinCAT.Ads;

namespace EndmillHMI
{
   
    public struct CommReply
        {
            public bool result;
            public float[] data;
            public string status;
            public string comment;//====2013
            public int FunctionCode;
        }
    class Beckhoff
    {
        public TextBox text1;//=new TextBox();
        public TextBox text2;//=new TextBox();
        public Form frm;//=new Form();
        public static TcAdsClient tcAds = new TcAdsClient();
        //TcAdsClient tcAds= BeckhoffPLC.tcAds;// = new TcAdsClient();
        //public BackgroundWorker bwWaitPlc = new BackgroundWorker();//wait cmd ready 
        //public string PlcNetID = "5.25.119.160.1.1";
        //public int PlcPort = 851;
        public string bwName = "";
       
        public struct SendPlcParms //====2013
        {
            public string comment;
            public Single[] SendParm;
            
        }
        public struct PlcReply
        {
            public float[] data;
            public string comment;//====2013
            public int status;
            public int StartAddress;
        }
        
         #region //---------Delegate Procedures--------------
       
        delegate void SetTextBox(string text, TextBox txt, Form frm);
        bool Prnt = true;
        public void SetText(int id, TextBox text, Form Frm,bool prnt=true)
        {
            switch (id)
            {
                case (1): text1 = text; break;
                case (2): text2 = text; break;
                default:

                    break;
            }
            frm = Frm;
            Prnt = prnt;
        }
        public void WriteToFile(string filebackup, string txt)
        {
            try
            {
                if (File.Exists(filebackup))
                {
                    //File.Delete(filebackup);
                    FileStream aFile = new FileStream(filebackup, FileMode.Append, FileAccess.Write);
                    StreamWriter sw1 = new StreamWriter(aFile);
                    sw1.WriteLine(txt);
                    sw1.Close();
                }
                else
                { 
                    FileStream aFile = new FileStream(filebackup, FileMode.Create, FileAccess.Write);
                    StreamWriter sw1 = new StreamWriter(aFile);
                    sw1.WriteLine(txt);
                    sw1.Close();
                }
                                
            }
            catch (Exception err)
            {
              
                
            }
        }
        private void SetTxtText(string text, TextBox txt, Form frm)
        {
            //string text1 = txt.Text + text;
            if ((txt == null) || (frm == null)) { return; }
            if (!Prnt) return;
            if (!frmMain.newFrmMain.chkLog.Checked) return;
            //if (!MainHMI.FrmMain.chkDebugPrint.Checked) return;
            try
            {
                if (txt.InvokeRequired)
                {
                    string text1 =  text;
                    SetTextBox d = new SetTextBox(SetTxtText);
                    frm.Invoke(d, new object[] { text, txt, frm });
                }
                else
                {
                    if (txt.Text.Trim().Length > 2000)
                    {

                        //WriteToFile(Path.GetDirectoryName(Application.ExecutablePath) + "\\AutomationIni\\Log\\Beckhofflog " + DateTime.Now.ToString("yyyy-MM-dd HH") + ".ini", txt.Text);
                        WriteToFile(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\AutomationIni\\Log\\Beckhofflog " + DateTime.Now.ToString("yyyy-MM-dd HH") + ".ini", txt.Text);
                        txt.Text = "";
                        //text1 = text;
                    }
                    else
                        //string text1 =  text;
                    txt.Text = text;
                    //if(txt.Text.Length > 0)
                    //    txt.SelectionStart = txt.Text.Length - 1;
                    //else
                    //    txt.SelectionStart = 0;
                    //txt.ScrollToCaret();
                }

            }
            catch (Exception ex)
            { }

        }
        delegate void SetListText(string text, ListBox lst,Form frm);
        private void SetTextLst(string text, ListBox lst, Form frm)
        {
            
            if ((lst == null) || (frm == null)) { return; }
            try
            {
                if (lst.InvokeRequired)
                {
                    SetListText d = new SetListText(SetTextLst);
                    frm.Invoke(d, new object[] { text, lst, frm });
                }
                else
                {
                    lst.Visible = false;
                    lst.Items.Add(text); ;
                    lst.Visible = true;
                    if (lst.Items.Count > 100)
                    {
                        for (int i = 0; i < 20; i++) lst.Items.RemoveAt(0);
                    }

                    lst.SetSelected(lst.Items.Count - 1, true);
                }
                
            }
            catch { }
            

        }
        delegate void SetProgressBar(int val, ProgressBar progressbar, Form frm);
        private void SetProgress(int val, ProgressBar progressbar, Form frm)
        {
            if ((progressbar == null) || (frm == null)) { return; }
            try
            {
                if (progressbar.InvokeRequired)
                {
                    SetProgressBar d = new SetProgressBar(SetProgress);
                    frm.Invoke(d, new object[] { val, progressbar, frm });
                }
                else
                {
                    progressbar.Value = val;
                }
                
            }
            catch {  }


        }
        #endregion
        #region ---------event to Form--------------
        public event EventHandler<MyEventArgs> GetFiniPlc;//= delegate { };
        public delegate void EventHandler(object sender, MyEventArgs e);

        public class MyEventArgs : System.EventArgs
        {
            private string _string;
            public string StringToSend
            {
                set { _string = value; }
                get { return _string; }

            }
            private Single[] _result;
            public Single[] DataGet
            {
                set { _result = value; }
                get { return _result; }

            }


        }


        public MyEventArgs MyE = new MyEventArgs();
        public void GetFini(Single[] result)
        {
            long t = MyStatic.Stsw.ElapsedMilliseconds;
            if (t - MyStatic.StswTime < MyStatic.StswDelay)//$
            {
                Thread.Sleep(MyStatic.StswDelay);// (MyStatic.StswDelay);
            }
            MyStatic.StswTime = MyStatic.Stsw.ElapsedMilliseconds;

            MyE.DataGet = result;
            this.GetFiniPlc(this, MyE);
            
        }
        #endregion
        private void DeviceAdd(int address,ref string device)
        {
            return;
            switch (address)
            {
                case 10: case 56:  device = " Cam1 ";
                    break;
                case 110:case 156: device = " Cam2 ";
                    break;
                case 210:case 256: device = " Gen  ";
                    break;
                case 310:case 356: device = " Gen1  ";
                    break;
                case 410: case 456:
                    device = " ReadIO  ";
                    break;
                case 510: case 556: device = " IOlink  ";
                    break;



                default: device = ""; break;
            }
        }
        AdsStream ds;
        BinaryWriter bwr;
        public Boolean PlcWrite(int StartAddress, SendPlcParms SendParams, ref string Error)
        {
            
            try
            {
                
                 ds = new AdsStream(4 * SendParams.SendParm.Length);//single 11 parameters,11*4 byte
                 bwr = new BinaryWriter(ds);
                tcAds.Write(0x4020, StartAddress + 46, ds);//clear read array
                string dv="";
                DeviceAdd(StartAddress,ref dv);

                string st = "";
                for (int i = 0; i < SendParams.SendParm.Length; i++)//create binary writer array
                {
                   bwr.Write(SendParams.SendParm[i]);
                   st = st + "[" + i.ToString() + "]" + SendParams.SendParm[i].ToString() + " ";
                }
                ds.Position = 0;
                tcAds.Write(0x4020, StartAddress, ds);//write to beckhoff from address 4020 all bytes
                if (SendParams.SendParm[0] != 719 && SendParams.SendParm[0] != 720 && SendParams.SendParm[0] != 501 && SendParams.SendParm[0] != 502 && SendParams.SendParm[0] != 600)
                {
                    st =  "<--" + bwName + dv + st + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                    Task.Run(() => SetTxtText(text1.Text + st, text1, frm));
                }
                bwr.Close();
                bwr.Dispose();
                
                Error = "";
                return true;
            }
            catch (Exception e)
            {
                
                Error = "ERROR SEND DATA TO PLC! " + e.Message;
                return false;
            }
        }
        //BinaryReader br;
        AdsStream ds1;
        public Boolean PlcRead(int StartAddress, ref PlcReply GetParams, ref string Error)
        {
            Array.Resize<Single>(ref GetParams.data, 11);//11 parameters
            ds1 = new AdsStream(4 * GetParams.data.Length);//single 11 parameters,11*4 byte
            BinaryReader br = new BinaryReader(ds1);
            try
            {
                
                
                ds1.Position = 0;
                try
                {
                    tcAds.Read(0x4020, StartAddress, ds1);
                }
                catch(Exception ex) { SetTxtText(text1.Text + "ERROR PLC READ1:" + ex.Message + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n", text1, frm); }
                string st = "";
                string dv = "";
                DeviceAdd(StartAddress, ref dv);
                for (int i = 0; i < GetParams.data.Length; i++)//create binary writer array
                {
                    try
                    {
                        if (br!=null )//&& br.BaseStream.CanRead)
                        {
                            GetParams.data[i] = br.ReadSingle();
                            st = st + "[" + i.ToString() + "]" + GetParams.data[i].ToString() + " ";
                        }
                        else
                        {
                            SetTxtText(text1.Text + "ERROR PLC READ2:" + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n", text1, frm);
                        }
                    }
                    catch(Exception ex) { SetTxtText(text1.Text + "ERROR PLC READ3:" + ex.Message + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n", text1, frm); }

                }
                if (GetParams.data[0] != 719 && GetParams.data[0] != 720 && GetParams.data[0] != 501 && GetParams.data[0] != 502 && GetParams.data[0] != 600)
                {
                    st =  "-->" + bwName + dv + st + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                    if (GetParams.data[0] != 0)
                        Task.Run(() => SetTxtText(text1.Text + st, text1, frm));
                }
                br.Close();
                br.Dispose();
                
                return true;
            }
            catch (Exception e)
            {
                SetTxtText(text1.Text + "ERROR PLC READ4:" +e.Message + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n", text1, frm);
                Error = e.Message;
                br.Close();
                br.Dispose();
                return false;
            }


        }
        private Boolean InArray(int[] arr, int cmd)
        {
            int first = Array.IndexOf<int>(arr, cmd);
            if (first >= 0) return true; else return false;
        }
        private bool CheckCmd(int cmd, string Name)
        {
            /*
            Beckhoff_Gen.bwName = "Beckhoff_Gen";
                Beckhoff_Rotate1.bwName = "Beckhoff_Rot1";
                Beckhoff_Rotate2.bwName = "Beckhoff_Rot2";
                
                Beckhoff_Conv1.bwName = "Beckhoff_Conv1";
                Beckhoff_Conv2.bwName = "Beckhoff_Conv2";
                Beckhoff_Table.bwName = "Beckhoff_Table";
                Beckhoff_Focus.bwName = "Beckhoff_Focus";
            */
            int[] Cam2cmd = {  100,101,102,103,104,105,106,107,108,109,110,111, 112, 113,114,115,116,117,118,119,120,121,122};
            int[] Cam1cmd = { 10, 11, 12, 13, 14 };
            int[] GenCmd1 = { 409, 410, 411, 412, 413, 414, 415,416,417,418,419,420,421,422,423,424,425,426,427,428,429,430,431,432,433 };
            int[] IOCmd = { 501,502,503,504,505 };
            int[] IOlinkCmd = { 600, 601, 602, 603, 604, 605 };


            int[] cmdArr={};

            if (Name=="Beckhoff_Gen1") cmdArr = GenCmd1;
            if (Name == "Beckhoff_Cam1") cmdArr = Cam1cmd;
            if (Name == "Beckhoff_ReadIO") cmdArr = IOCmd;
            if (Name == "Beckhoff_IOlink") cmdArr = IOlinkCmd;



            if (InArray(cmdArr, cmd))//wait reply
                return true;
            else
            {
                string st = text1.Text + "-->" + "ERROR CHECK CMD "+bwName+":"+ cmd.ToString() + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                Task.Run(() => SetTxtText(text1.Text + st, text1, frm));
                return false;
            }
        }
       
        public PlcReply WaitPlcReply(PlcReply SendParams)
        {
            PlcReply GetParams = new PlcReply();
            Stopwatch stopper = new Stopwatch();
            stopper.Start();

            PlcReply plcreply = new PlcReply();
            try
            {
                plcreply = SendParams;

                string Error = "";
                plcreply.status = -1;
                SendParams.status = -1;

                Single[] arg = SendParams.data;//  RobotParms.SendParm;
                int StartAddress = SendParams.StartAddress;

                int cmdwait = (int)arg[0];
                Single tmout = arg[arg.Length - 1];
                GetParams.status = -1;
                GetParams.comment = "";

                while(!MyStatic.bReset)
                {
                    //Application.DoEvents();
                    Thread.Sleep(10);
                    if (MyStatic.bReset)
                        return GetParams;
                    if (cmdwait != SendParams.data[0])
                    {
                        int a = 0;
                        string str = bwName;
                    }

                    Thread.Sleep(10);
                    string st = "";
                    if (MyStatic.BeckhoffchkDebug)//flow chart
                    {
                        stopper.Stop(); StopperCmd.Stop();
                        return GetParams; ;
                    }
                    else
                    {


                        if (!PlcRead(StartAddress, ref GetParams, ref Error))
                        { GetParams.status = -1; GetParams.comment = "Error"; stopper.Stop(); StopperCmd.Stop();
                            return GetParams;  }//error read 
                        if ((cmdwait == MyStatic.CamsCmd.MoveAbs || cmdwait == MyStatic.CamsCmd.MoveRel) && (GetParams.data[0] == MyStatic.CamsCmd.Stop || GetParams.data[0] == MyStatic.CamsCmd.Reset))
                        {
                            st =  "-->" + "STOPPED" + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                            Task.Run(() => SetTxtText(text1.Text + st, text1, frm));

                            GetParams.status = -1;
                            GetParams.comment = "Error cmd";
                            GetParams.data[0] = 0;
                            stopper.Stop(); StopperCmd.Stop();
                            return GetParams;
                        }
                        if (GetParams.data[0] == cmdwait)//ok
                        {
                            if (GetParams.data[1] != 0)
                            {

                                GetParams.status = -1;
                                stopper.Stop(); StopperCmd.Stop();
                                string Err = "";
                                //MainHMI.FrmMain.BechoffReadError((int)(GetParams.data[1]), ref Err);
                                GetParams.comment = "Error execution:" + GetParams.data[1] + " " + Err;
                                SetTxtText(text1.Text + "Error execution " + GetParams.data[1].ToString() + " " + Err + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n", text1, frm);
                                return GetParams;
                            }
                            if (!CheckCmd((int)GetParams.data[0], bwName))

                            {
                                st =  "-->" + "ERROR CMD " + GetParams.data[0].ToString() + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                                SetTxtText(text1.Text + st, text1, frm);

                                GetParams.status = -1;
                                GetParams.comment = "Error cmd";
                                GetParams.data[0] = 0;
                                stopper.Stop(); StopperCmd.Stop();
                                return GetParams;
                            }
                            GetParams.status = 0;
                            GetParams.comment = "";
                            stopper.Stop(); StopperCmd.Stop();
                            return GetParams;
                        }

                        if ((tmout > 0) & (tmout * 1000 < stopper.ElapsedMilliseconds))
                        {
                            GetParams.comment = "TimeOut";
                            SetTxtText(text1.Text + " TimeOut" + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n", text1, frm);
                            GetParams.status = -1; //timeout
                            Array.Resize<Single>(ref GetParams.data, 1);
                            GetParams.data[0] = 0;
                            stopper.Stop(); StopperCmd.Stop();
                            return GetParams;
                            
                        }
                    }

                }
                
                stopper.Stop(); StopperCmd.Stop();

                return  GetParams; ;
            }
            catch(Exception ex)
            {
                GetParams.status = -1; //timeout
                Array.Resize<Single>(ref GetParams.data, 1);
                GetParams.data[0] = 0;
                stopper.Stop(); StopperCmd.Stop();
                return GetParams;

            }



        }

        Stopwatch StopperCmd = new Stopwatch();
        
        public bool PlcSendCmd(int StartAddressSend, SendPlcParms SendParams, ref string err, bool WaitFini = true)
        { return false; }
        public CommReply PlcSendCmd(int StartAddressSend, SendPlcParms SendParams, bool WaitFini=true, bool notsend=false)
        {
            Stopwatch stopw = new Stopwatch();
            stopw.Start();
            StopperCmd.Reset(); StopperCmd.Start();
            CommReply reply=new CommReply();
            try
            {
                Array.Resize<Single>(ref reply.data, SendParams.SendParm.Length);

                string Error = "";
                if (!MyStatic.BeckhoffchkDebug)//flow chart
                {

                    if (!notsend)
                    {
                        if (!PlcWrite(StartAddressSend, SendParams, ref Error))
                        {
                            reply.result = false;
                            reply.status = Error;
                            return reply;
                        }
                    }
                    else
                    {
                        ds = new AdsStream(4 * SendParams.SendParm.Length);//single 11 parameters,11*4 byte
                        tcAds.Write(0x4020, StartAddressSend + 46, ds);//clear read array
                    }
                }
                else
                {

                    string st = "";

                    for (int i = 0; i < SendParams.SendParm.Length; i++)//create binary writer array
                    {

                        st = st + "[" + i.ToString() + "]" + SendParams.SendParm[i].ToString() + " ";
                    }

                    //st = text1.Text + "<--" + bwName + st + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                    //SetTxtText(st, text1, frm);
                    Thread.Sleep((int)((SendParams.SendParm[SendParams.SendParm.Length - 1]) * 100));
                    Array.Resize<Single>(ref reply.data, SendParams.SendParm.Length);
                    reply.data[0] = SendParams.SendParm[0];
                    reply.data[1] = 0;
                    reply.result = true;
                    reply.status = "";
                    st = SendParams.SendParm[0].ToString() + ",0 ";
                    st = text1.Text + "-->" + bwName + st + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                    //SetTxtText(st, text1, frm);
                    return reply;

                }
                reply.result = false;
                if (!WaitFini)
                    return reply;

                PlcReply GetParams = new PlcReply();
                PlcReply SendPar = new PlcReply();
                Array.Resize<Single>(ref GetParams.data, SendParams.SendParm.Length);
                Array.Resize<Single>(ref SendPar.data, SendParams.SendParm.Length);
                SendPar.data = SendParams.SendParm;
                SendPar.StartAddress = StartAddressSend + 46; //offset for read
                SendPar.status = -1;

                //bwWaitPlc_DoWork(SendPar, ref GetParams);
                GetParams = WaitPlcReply(SendPar);
                
                if (GetParams.status == -1)
                    reply.result = false;
                else reply.result = true;
                reply.data = GetParams.data;
                reply.comment = GetParams.comment;
                Array.Resize<Single>(ref reply.data, SendParams.SendParm.Length);
                return reply;
            }
            catch (Exception ex)
            {
                string st = "-->" + bwName + "Error Send cmd" + " // " + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                SetTxtText(text1.Text + st, text1, frm);
                return reply; }
           
        }
        
    }
}
