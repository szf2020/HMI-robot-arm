using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Reflection.Emit;
using System.Windows.Forms.DataVisualization.Charting;

namespace EndmillHMI
{
    class WebComm
    {
        public ListBox lstSend;
        public TextBox txtsend;
        string Robotname = "";
        public string CameraAddress = "";

        public Form frm;
        public struct SendRobotParms //====2013
        {
            public string comment;
            public Single[] SendParm;
            public bool NotSendMess;
            public string cmd;
            public int FunctionCode;
            public Single timeout;
            public int DebugTime;
            //public int FunctionCode;
        }
        public struct CommReply
        {
            public bool result;
            public float[] data;
            public string status;
            public string comment;//====2013
            public int FunctionCode;
            public string Error;
        }
        public DataIniFile dFile = new DataIniFile();
       
        public void SetControls(ListBox LstSend, Form Frm, WebBrowser WB)
        {
            lstSend = LstSend;
            //txtstate = Txtstate;
            frm = Frm;
            //WBnew = WB;
           

        }
        public void SetControls1(TextBox txtSend, Form Frm, WebBrowser WB,string RobotName="",string address="")
        {
            txtsend = null;
            txtsend = txtSend;
            
            frm = Frm;
            //WBnew = WB;
            Robotname = RobotName;
            CameraAddress = address;

        }
        delegate void SetListText(string text, ListBox lst, Form frm);
        public void SetTextLst(string text, ListBox lst, Form frm)
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
                    lst.Items.Add(text);
                    if (lst.Items.Count > 100) { lst.Items.Clear(); }
                }
            }
            catch { }

        }
        delegate void SettxtText(string text, TextBox txt, Form frm);
        public void SetTexttxt(string text, TextBox txt, Form frm)
        {
            
            if ((txt == null) || (frm == null)) { return; }

            try
            {
                if (txt.InvokeRequired)
                {
                    SettxtText d = new SettxtText(SetTexttxt);
                    frm.Invoke(d, new object[] { text, txt, frm });
                }
                else
                {
                    text = Robotname + " " + text;
                    txt.Text =  txt.Text + text+"\r\n"; 
                }
            }
            catch { }

        }
        /// <summary>
        
        //delegate void SetWB(WebBrowser wb,Form frm);
        bool ready;
       
              
        public static void SetControlThreadSafe(Control control, Action<object[]> action, object[] args)
        {
            if (control.InvokeRequired)
                try { control.Invoke(new Action<Control, Action<object[]>, object[]>(SetControlThreadSafe), control, action, args); } catch { }
            else action(args);
        }
        //delegate WebBrowser StringInvoker();
        
        /// 
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="regnum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
       
       
        public async Task<bool> GetContentsOfUrl(WebBrowser wb)
        {
            while (wb.ReadyState != WebBrowserReadyState.Complete || wb.IsBusy)
            {
                
                Thread.Sleep(20);
                if (MyStatic.bReset) return false;
            }
            return true;
        }

        CancellationTokenSource cancelToken = new CancellationTokenSource();
        System.Net.WebRequest req = null;
        public CommReply WriteFanucRegAsync(string reg, string regnum, string value,int timeout,string set="setreal")
        {
                CommReply reply = new CommReply();
                Stopwatch stopw = new Stopwatch();
            cancelToken = new CancellationTokenSource();
            CancellationToken cancel = cancelToken.Token;

            try
            {
                
                
                stopw.Restart();
                
                //string Url = "http://192.168.0.101:5000/ "; //"http://localhost:5000/ ";http://192.168.0.101:5000 http://127.0.0.1:5000/
                string Url = CameraAddress;
                string Data =  value;
                SetTexttxt("<--" + Data + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", txtsend, frm);
                req = System.Net.WebRequest.Create(Url + "?" + Data);
                req.Proxy = null;
                req.Timeout = timeout;



                System.Net.WebResponse resp =  req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd();
                SetTexttxt("-->" + Out +
                                                          " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", txtsend, frm);

                sr.Close();
                reply.comment = Out;
                reply.result = true;
                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error write HTML:" + ex.Message;
                
                SetTexttxt("<--" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", txtsend, frm);
                return reply;
            }
            
            return reply;
        }
        public void StopComm()
        {
            try
            {
                req.Abort();
               
            }
            catch (Exception ex) { }
        }
        
        public  void Timeout(HttpWebRequest req)
        {
            Stopwatch stopw = new Stopwatch();
            stopw.Restart();
            while (!MyStatic.bReset)
            {
                Application.DoEvents();
                if (stopw.ElapsedMilliseconds > 3000)
                {
                    req.Abort();
                    SetTexttxt("-->" + "timeout error" +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", txtsend, frm);
                    return;

                }
            }
        }
        public  WebComm.CommReply RunCmd(SendRobotParms Parms, bool debug = false)
        //send cmd to robot
        {

            
            int cmd = int.Parse(Parms.cmd);
            
            WebComm.CommReply reply = new WebComm.CommReply();
            //switch (cmd)
            //{
               
                  
               
            //    case (int)MyStatic.InspectCmd.CheckComm://-------vision check communication
            //        Parms.FunctionCode = (int)MyStatic.InspectCmd.CheckComm;
            //        Parms.comment = "Check comm";
            //        Parms.timeout = 2;
            //        Array.Resize<Single>(ref Parms.SendParm, 3);
            //        Parms.SendParm[0] = cmd;//16
            //        Parms.SendParm[1] =1;// general speed
            //        Parms.SendParm[2] = 2;// 0.5f;//timeout
            //        break;
                                
            //    //all commands
            //    default:
            //        break;
                    
            //}

            
            string ErrMessage = "";
            
            MyStatic.TaskExecute = true;
            
                try
                {
                   
                    reply.result = false;
                    string reg = "strreg";
                    string num = "1";
                    string val = "";
                    for (int i = 0; i < Parms.SendParm.Length; i++)
                    {
                        val = val + Parms.SendParm[i].ToString() + ",";
                    }
                    val = "cmd" + val + "end";
                   
                    reply = WriteFanucRegAsync(reg, num, val, (int)Parms.timeout*1000);
                //var task = WriteHttpAsync(reg, num, val, 1000);
                   
                    //if(reply.comment.StartsWith("cmd"))
                    //{
                    //    string s = reply.comment.Remove(0, 3);
                    //    string[] ss = s.Split(',');
                    //    if (ss.Length >=3 && ss[0] != cmd.ToString())
                    //    {
                    //        reply.result = false;
                    //        return reply;

                    //    }
                    //    if (ss.Length >= 3 && ss[1] != "1")
                    //    {
                    //        reply.result = false;
                    //        return reply;

                    //    }

                    //}
                    if (!reply.result) { return reply; }
                string[] str = reply.comment.Split(',');
                if (str.Length >= 3 && str[1] == "1") { reply.result = true; } else reply.result = false;
                    return reply;
                }
                catch (Exception ex)
                {
                    reply.result = false;
                    return reply;

                }
            
            return reply;
            

        }
        public async Task<CommReply> WriteHttpAsync(string reg, string regnum, string value, int timeout, string set = "setreal")
        {
            CommReply reply = new CommReply();
            Stopwatch stopw = new Stopwatch();
            cancelToken = new CancellationTokenSource();
            CancellationToken cancel = cancelToken.Token;
            
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(CameraAddress);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Proxy = null;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.ServicePoint.UseNagleAlgorithm = false;
                httpWebRequest.KeepAlive = false;
                string Data = value;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "?" + Data;

                    streamWriter.Write(json);
                    SetTexttxt("<--" + json.ToString() +
                                         " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", txtsend, frm);

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
                    SetTexttxt("-->" + result.ToString() +
                                          " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", txtsend, frm);

                    reply.comment = result;
                    reply.result = true;
                    return reply;

                }
               
                return reply;

                //for (int i = 0; i < nn; i++) { inv.settxt(txtWeldon, txtWeldon.Text + "\r\n" + temp[i]); Thread.Sleep(1); }
                //timer1.Start();
            }
            catch (Exception ex) { return reply; }
            ///////////////////////////////

        }





    }
}
