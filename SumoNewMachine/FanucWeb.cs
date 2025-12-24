using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TwinCAT.TypeSystem;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EndmillHMI
{
    class FanucWeb
    {
        public WebBrowser WBnew;

        public string sUrlwrite="";//= new Uri("D:/test6.html");// html file on D:write reg
        public string sUrlread="";// = new Uri("http://192.168.0.20/fr/example.stm");// html file on FR:read reg
        public string sUrlrobot="";// = new Uri("http://192.168.0.20/");// html file on FR:read reg
        public string sUrlswrite="";// = new Uri("D:/test7.html");// html file on D:write reg
        public string sUrlregwrite="";

        int RegStartWrite = 150;//11 data reg
        int RegStartRead = 170;//11 data reg
        public System.Windows.Forms.TextBox lstSend;

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
        public DataIniFile dFile = new DataIniFile();
        public void FanucWebInit(string surlwrite,string surlread,string surlrobot,
            string surlswrite, string surlregwrite)
        {
            //WB1 = new WebBrowser();

            sUrlwrite = surlwrite;// "D:/test6.html";// html file on D:write reg
            sUrlread = surlread;// "http://192.168.0.20/fr/example.stm";// html file on FR:read reg
            sUrlrobot = surlrobot;// "http://192.168.0.20/";// html file on FR:read reg
            sUrlswrite = surlswrite;// "D:/test7.html";// html file on D:write reg
            sUrlregwrite = surlregwrite;// "http://192.168.0.20/karel/mpnlsrv ";
        }
        public void SetControls(System.Windows.Forms.TextBox LstSend, Form Frm, WebBrowser WB)
        {
            lstSend = LstSend;
            //txtstate = Txtstate;
            frm = Frm;
            WBnew = WB;

        }
        delegate void SetListText(string text, System.Windows.Forms.TextBox txt, Form frm);
        public void SetTextLst(string text, System.Windows.Forms.TextBox txt, Form frm)
        {
            //if (!Enable) { return; }
            if ((txt == null) || (frm == null)) { return; }

            try
            {
                if (txt.InvokeRequired)
                {
                    SetListText d = new SetListText(SetTextLst);
                    frm.Invoke(d, new object[] { text, txt, frm });
                }
                else
                {
                    txt.Text = txt.Text + text + "\r\n";
                    if (txt.TextLength >10000) 
                    { txt.Text=""; }
                }
            }
            catch { }

        }
        /// <summary>
        //public void SetWBrowser(WebBrowser WB,Form Frm)
        //{
        //    WBnew = WB;
        //    frm = Frm;
        //}
        delegate void SetWB(WebBrowser wb,Form frm);
        bool ready;
        public  void CheckWBready(WebBrowser wb, Form frm)
        {
            //b != WebBrowserReadyState.Complete)  || WB1.IsBusy

            if ((wb == null) || (frm == null)) { return; }

            try
            {
                if (wb.InvokeRequired)
                {
                    SetWB d = new SetWB(CheckWBready);
                    frm.Invoke(d, new object[] { wb,frm });
                }
                else
                {
                    WebBrowserReadyState b = wb.ReadyState;
                    if (b != WebBrowserReadyState.Complete || wb.IsBusy) ready = false; else ready = true;

                }
                
            }
            catch (Exception ex) {
                string err = ex.Message;
            }

        }
        delegate void SetWB1(WebBrowser wb, string txt, string val,Form frm);
        
        public void SetWBtext(WebBrowser wb,string txt,string val, Form frm)
        {
            //if (!Enable) { return; }
            if ((wb == null) || (frm == null)) { return; }

            try
            {
                if (wb.InvokeRequired)
                {
                    SetWB1 d = new SetWB1(SetWBtext);
                    frm.Invoke(d, new object[] { wb,txt,val,frm });
                }
                else
                {
                    wb.Document.GetElementById(txt).InnerText =val;
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

        }
        delegate void SetWB2(WebBrowser wb, string txt, string val, Form frm);

        public void SetWBclick(WebBrowser wb, string txt, string val, Form frm)
        {
            //WBnew.Document.GetElementById("sub1").InvokeMember("Click");//click on submit hml button
            if ((wb == null) || (frm == null)) { return; }

            try
            {
                if (wb.InvokeRequired)
                {
                    SetWB2 d = new SetWB2(SetWBclick);
                    frm.Invoke(d, new object[] { wb, txt, val, frm });
                }
                else
                {
                    wb.Document.GetElementById(txt).InvokeMember(val);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

        }
        public static void SetControlThreadSafe(Control control, Action<object[]> action, object[] args)
        {
            if (control.InvokeRequired)
                try { control.Invoke(new Action<Control, Action<object[]>, object[]>(SetControlThreadSafe), control, action, args); } catch { }
            else action(args);
        }
        delegate WebBrowser StringInvoker();
        
        /// 
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="regnum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public  RobotFunctions.CommReply WriteFanucReg(string reg,string regnum,string value)
        {

            var urlwrite = new Uri(sUrlwrite);// new Uri("D:/test6.html");// html file on D:write reg
            var urlread = new Uri(sUrlread); // new Uri("http://192.168.0.20/fr/example1.stm");// html file on FR:read reg
            var urlrobot = new Uri(sUrlrobot); //new Uri("http://192.168.0.20/");// html file on FR:read reg
            var urlswrite = new Uri(sUrlswrite); //new Uri("D:/test7.html");// html file on D:write reg
            //WebBrowser WB1 = new WebBrowser();
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                //WebBrowser WB1 = new WebBrowser();
                //WB1.DocumentText = "";
                //lstRobot.Items.Add("Start write to robot" + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")");
                
                //SetTextLst("Start write to robot" +
                //       " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                //Thread.Sleep(1500);//for test
                reply.result = false;

                WBnew.Dock = DockStyle.Fill;
                
                WBnew.AllowNavigation = true;
                //if (WB2.Url != urlwrite) { var task1 = Task.Run(() => WB2.Navigate(urlwrite)); 
                //await task1;
                //}
                if (WBnew.Url != urlswrite)
                    WBnew.Navigate(urlswrite);//show write form
                   // WebBrowserReadyState b=new WebBrowserReadyState();
                CheckWBready(WBnew, frm);
                //WB1.Refresh(); don't use!
                //bool b = await GetContentsOfUrl(WB1);
                while (!ready) // || WB1.IsBusy)
                {
                    Application.DoEvents();
                    Thread.Sleep(20);
                    if (MyStatic.bReset) return reply;
                    CheckWBready(WBnew, frm);
                }
                //while (WB1.ReadyState != WebBrowserReadyState.Complete || WB1.IsBusy)
                //{
                //    //Application.DoEvents();
                //    Thread.Sleep(20);
                //    if (MyStatic.bReset) return reply;
                //}


                //WB1.Focus();
                switch (reg.Trim())
                {
                    case "numreg":
                        SetWBtext(WBnew, "txt3", "numreg", frm);// WB1.Document.GetElementById("txt3").InnerText = "numreg";
                        SetWBtext(WBnew, "txt2", value.Trim(), frm);//WB1.Document.GetElementById("txt2").InnerText = value.Trim() ;
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "strreg":
                        SetWBtext(WBnew, "txt3", "strreg", frm);// WB1.Document.GetElementById("txt3").InnerText = "strreg";
                        if (value.Trim()!="") SetWBtext(WBnew, "txt2", value.Trim() + ",end", frm);// WB1.Document.GetElementById("txt2").InnerText = value.Trim() +",end" ;
                        else SetWBtext(WBnew, "txt2", "", frm); //WB1.Document.GetElementById("txt2").InnerText = "";
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "dout":
                        SetWBtext(WBnew, "txt3", "dout", frm); //WB1.Document.GetElementById("txt3").InnerText = "dout";
                        SetWBtext(WBnew, "txt2", value.Trim(), frm); //WB1.Document.GetElementById("txt2").InnerText = value.Trim() ;
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "sysvar":
                        SetWBtext(WBnew, "txt3", "sysvar", frm); //WB1.Document.GetElementById("txt3").InnerText = "sysvar";
                        SetWBtext(WBnew, "txt2", value.Trim(), frm); //WB1.Document.GetElementById("txt2").InnerText = value.Trim() ;
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "read strreg":
                        SetWBtext(WBnew, "txt3", "rdstrreg", frm); //WB1.Document.GetElementById("txt3").InnerText = "rdstrreg";
                        SetWBtext(WBnew, "txt2", "", frm); //WB1.Document.GetElementById("txt2").InnerText = "";
                        SetWBtext(WBnew, "txt4", "", frm); //WB1.Document.GetElementById("txt4").InnerText = "";
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    default:
                       
                        return reply;

                }



                SetWBclick(WBnew, "sub1", "Click", frm); //WBnew.Document.GetElementById("sub1").InvokeMember("Click");//click on submit hml button
                Thread.Sleep(20);
                // b = await GetContentsOfUrl(WB1);
                //while (WB1.ReadyState != WebBrowserReadyState.Complete || WB1.IsBusy)
                //{
                //    Application.DoEvents();
                //    Thread.Sleep(20);
                //    if (MyStatic.bReset) return reply;
                //}
                CheckWBready(WBnew, frm);
                //WB1.Refresh(); don't use!
                //bool b = await GetContentsOfUrl(WB1);
                while (!ready) // || WB1.IsBusy)
                {
                    Application.DoEvents();
                    Thread.Sleep(20);
                    if (MyStatic.bReset) return reply;
                    CheckWBready(WBnew, frm);
                }
                //lstRobot.Items.Add("End write to robot" + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")");
                //SetTextLst("End write to robot" +
                //        " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error swrite:" + ex.Message);
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                return reply;
            }
            return reply;
            
        }
        public RobotFunctions.CommReply WriteFanucRegStatic( string reg, string regnum, string value)
        {

            var urlwrite = new Uri(sUrlwrite);//new Uri("D:/test6.html");// html file on D:write reg
            var urlread = new Uri(sUrlread);// html file on FR:read reg
            var urlrobot = new Uri(sUrlrobot);// html file on FR:read reg
            var urlswrite = new Uri(sUrlswrite);// html file on D:write reg
            //WebBrowser WB1 = new WebBrowser();
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {
                //WebBrowser WB1 = new WebBrowser();
                //WB1.DocumentText = "";
                //lstRobot.Items.Add("Start write to robot" + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")");

                //SetTextLst("Start write to robot" +
                //       " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                //Thread.Sleep(1500);//for test
                reply.result = false;

                WBnew.Dock = DockStyle.Fill;

                WBnew.AllowNavigation = true;
                //if (WB2.Url != urlwrite) { var task1 = Task.Run(() => WB2.Navigate(urlwrite)); 
                //await task1;
                //}
                if (WBnew.Url != urlswrite)
                    WBnew.Navigate(urlswrite);//show write form
                                              // WebBrowserReadyState b=new WebBrowserReadyState();
                CheckWBready(WBnew, frm);
                //WB1.Refresh(); don't use!
                //bool b = await GetContentsOfUrl(WB1);
                while (!ready) // || WB1.IsBusy)
                {
                    Application.DoEvents();
                    Thread.Sleep(20);
                    if (MyStatic.bReset) return reply;
                    CheckWBready(WBnew, frm);
                }
                //while (WB1.ReadyState != WebBrowserReadyState.Complete || WB1.IsBusy)
                //{
                //    //Application.DoEvents();
                //    Thread.Sleep(20);
                //    if (MyStatic.bReset) return reply;
                //}


                //WB1.Focus();
                switch (reg.Trim())
                {
                    case "numreg":
                        SetWBtext(WBnew, "txt3", "numreg", frm);// WB1.Document.GetElementById("txt3").InnerText = "numreg";
                        SetWBtext(WBnew, "txt2", value.Trim(), frm);//WB1.Document.GetElementById("txt2").InnerText = value.Trim() ;
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "strreg":
                        SetWBtext(WBnew, "txt3", "strreg", frm);// WB1.Document.GetElementById("txt3").InnerText = "strreg";
                        if (value.Trim() != "") SetWBtext(WBnew, "txt2", value.Trim() + ",end", frm);// WB1.Document.GetElementById("txt2").InnerText = value.Trim() +",end" ;
                        else SetWBtext(WBnew, "txt2", "", frm); //WB1.Document.GetElementById("txt2").InnerText = "";
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "dout":
                        SetWBtext(WBnew, "txt3", "dout", frm); //WB1.Document.GetElementById("txt3").InnerText = "dout";
                        SetWBtext(WBnew, "txt2", value.Trim(), frm); //WB1.Document.GetElementById("txt2").InnerText = value.Trim() ;
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "sysvar":
                        SetWBtext(WBnew, "txt3", "sysvar", frm); //WB1.Document.GetElementById("txt3").InnerText = "sysvar";
                        SetWBtext(WBnew, "txt2", value.Trim(), frm); //WB1.Document.GetElementById("txt2").InnerText = value.Trim() ;
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    case "read strreg":
                        SetWBtext(WBnew, "txt3", "rdstrreg", frm); //WB1.Document.GetElementById("txt3").InnerText = "rdstrreg";
                        SetWBtext(WBnew, "txt2", "", frm); //WB1.Document.GetElementById("txt2").InnerText = "";
                        SetWBtext(WBnew, "txt4", "", frm); //WB1.Document.GetElementById("txt4").InnerText = "";
                        SetWBtext(WBnew, "txt1", regnum.Trim(), frm); //WB1.Document.GetElementById("txt1").InnerText = regnum.Trim();
                        break;
                    default:

                        return reply;

                }



                SetWBclick(WBnew, "sub1", "Click", frm); //WBnew.Document.GetElementById("sub1").InvokeMember("Click");//click on submit hml button
                Thread.Sleep(20);
                // b = await GetContentsOfUrl(WB1);
                //while (WB1.ReadyState != WebBrowserReadyState.Complete || WB1.IsBusy)
                //{
                //    Application.DoEvents();
                //    Thread.Sleep(20);
                //    if (MyStatic.bReset) return reply;
                //}
                CheckWBready(WBnew, frm);
                //WB1.Refresh(); don't use!
                //bool b = await GetContentsOfUrl(WB1);
                while (!ready) // || WB1.IsBusy)
                {
                    Application.DoEvents();
                    Thread.Sleep(20);
                    if (MyStatic.bReset) return reply;
                    CheckWBready(WBnew, frm);
                }
                //lstRobot.Items.Add("End write to robot" + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")");
                Task.Run(() => SetTextLst("End write to robot" +
                        " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
            }
            catch (Exception ex)
            {
                MessageBox.Show("error swrite:" + ex.Message);
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                return reply;
            }
            return reply;

        }
        public bool GetContentsOfUrl(WebBrowser wb)
        {
            while (wb.ReadyState != WebBrowserReadyState.Complete || wb.IsBusy)
            {
                //Application.DoEvents();
                Thread.Sleep(20);
                if (MyStatic.bReset) return false;
            }
            return true;
        }

        public RobotFunctions.CommReply ReadFanucS2Reg()
        {
            //SetTextLst("Start read robot" +
            //            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            try
            {

                var urlwrite = new Uri(sUrlwrite);// html file on D:write reg
                var urlread = new Uri(sUrlread);// html file on FR:read reg
                var urlrobot = new Uri(sUrlrobot);// html file on FR:read reg
                var urlswrite = new Uri(sUrlswrite);// html file on D:write reg

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlread);
               
                req.Timeout = 200;
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                System.IO.Stream st1 = res.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(st1);
                string body = sr.ReadToEnd();
                 
                int a = body.IndexOf("<BODY>");
                int b = body.IndexOf("</BODY>");
                string str = body.Substring(a + 6, b - a - 6);
                string[] data = str.Split('\n');
                Array.Resize<Single>(ref reply.data, 11);
                string[] data1 = data[1].Split(',');
                if (data1[0].IndexOf("cmd") == 0 && data1[data1.Length - 1] == "end")
                {
                    for (int i = 0; i < reply.data.Length; i++) reply.data[i] = 0;
                    reply.data[0] = Single.Parse(data1[0].Substring(3, 2));
                    for (int i = 1; i < data1.Length - 2; i++)
                    {
                        reply.data[i] = Single.Parse(data1[i]);
                    }
                    //enter receive data into result.data
                    reply.result = true;
                }
                else
                {
                    reply.result = true;
                    Task.Run(() => SetTextLst("-->" + "end read str" +
                                            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                    return reply;
                }

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error read:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }
            reply.result = true;
            //return ;
            /////////
            string st = "";
            for (int i = 0; i < reply.data.Length; i++) st = st + reply.data[i].ToString() + " ";
            Task.Run(() => SetTextLst("-->" + st + 
                                            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
            return reply;
        }
        public  RobotFunctions.CommReply ReadFanucS2RegAsync1(int timeout)
        {
            //SetTextLst("Start read robot" +
            //            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            stopw.Restart();
           
            try
            {

                var urlwrite = new Uri(sUrlwrite);// html file on D:write reg
                var urlread = new Uri("http://192.168.0.20/MD/STRREG.VA");// new Uri(sUrlread);// html file on FR:read reg
                var urlrobot = new Uri(sUrlrobot);// html file on FR:read reg
                var urlswrite = new Uri(sUrlswrite);// html file on D:write reg



                reply.result = false;
                
                while (true)
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlread);
                    //Thread.Sleep(10000);//test
                    req.Timeout = 200;


                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    System.IO.Stream st1 = res.GetResponseStream();
                    string body = "";
                    System.IO.StreamReader sr = new System.IO.StreamReader(st1);
                    
                        body= sr.ReadToEnd();
                    return reply;


                    Thread.Sleep(20);
                    Application.DoEvents();
                    if (MyStatic.bReset) return reply;
                    //body = sr.ReadToEnd();
                    if (timeout != null && timeout >= 0 && stopw.ElapsedMilliseconds > timeout)
                    {
                        reply.result = false;
                        SetTextLst("-->" + "timeout error" +
                                               " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        return reply;
                    }
                    int a = body.IndexOf("<BODY>");
                    int b = body.IndexOf("</BODY>");
                    string str = body.Substring(a + 6, b - a - 6);
                    string[] data = str.Split('\n');
                    //SetTextLst("-->" + data[1] +
                    //                         " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                    Array.Resize<Single>(ref reply.data, 20);
                    string[] data1 = data[1].Split(',');
                    if (data1[0].IndexOf("cmd") == 0 && data1[data1.Length - 1] == "end")
                    {
                        for (int i = 0; i < reply.data.Length; i++) reply.data[i] = 0;
                        reply.data[0] = Single.Parse(data1[0].Substring(3, 2));
                        for (int i = 1; i < data1.Length - 1; i++)
                        {
                            reply.data[i] = Single.Parse(data1[i]);
                        }
                        //enter receive data into result.data
                        string str1 = "";
                        for (int i = 0; i < reply.data.Length; i++) str1 = str1 +" "+ reply.data[i].ToString();
                        reply.result = true;
                        SetTextLst("Read Reg-->" + str1 +
                                              " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        //SetTextLst("-->" + "end read str" +
                        //                       " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        return reply;
                    }
                    else
                    {
                        //reply.result = true;
                        //SetTextLst("-->" + "end read1 str" +
                        //                        " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        //return reply;
                    }
                    Thread.Sleep(1);
                }
            }

            
            catch (Exception ex)
            {
                    reply.result = false;
                    reply.comment = "error read:" + ex.Message;
                    //WB1.Stop();
                    //WB1.Url = new Uri("about:blank");
                    SetTextLst("-->" + reply.comment +
                                               " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                    //return reply;
            }
            
            reply.result = true;
            //return ;
            /////////
            string st = "";
            for (int i = 0; i < reply.data.Length; i++) st = st + reply.data[i].ToString() + " ";
            Task.Run(() => SetTextLst("-->" + st +
                                            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
            return reply;
        }
        public RobotFunctions.CommReply ReadFanucS2RegAsync(int timeout)
        {
            //SetTextLst("Start read robot" +
            //            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            stopw.Restart();
            Single[] dataGet = new Single[20];
            try
            {

                var urlwrite = new Uri(sUrlwrite);// html file on D:write reg
                var urlread =  new Uri(sUrlread);// html file on FR:read reg;//new Uri("http://192.168.0.20/fr/example1.stm"); ;// new Uri("http://192.168.100.20/MD/STRREG.VA");// html file on FR:read reg
                var urlrobot = new Uri(sUrlrobot);// html file on FR:read reg
                var urlswrite = new Uri(sUrlswrite);// html file on D:write reg

                reply.result = false;

                while (true)
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Proxy = null;
                        
                        string s = client.DownloadString(urlread);
                        string[] data = s.Split('\n');
                        
                        string sss = "";
                        ////for (int i = 0; i < data.Length; i++)
                        for (int i = 108; i < 122; i++)
                        {
                            for (int ii = 100; ii < 114; ii++)
                            {
                                if (data[i].Contains("[" + ii.ToString() + "]"))
                                {
                                    //SetTextLst(data[i], lstSend, frm);

                                    int j = data[i].IndexOf('=');
                                    int jj = data[i].IndexOf("'");
                                    string ss = data[i].Substring(j + 1, jj - j - 1);
                                    dataGet[ii - 100] = Single.Parse(ss);
                                    sss = sss + '[' + ii.ToString() + ']' + ss + ';';
                                }
                                //Thread.Sleep(1);
                            }
                            //Thread.Sleep(1);
                        }


                        if (dataGet[0] > 0)//have cmd back
                        {
                            Task.Run(() => SetTextLst("-->"+sss +  " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                            reply.result = true;
                            reply.data = new Single[16];
                            for(int i = 0; i < 16; i++) { reply.data[i] = dataGet[i]; }
                            return reply;
                        }

                    }

                    Thread.Sleep(10);
                    
                    if (MyStatic.bReset) return reply;
                    
                    if (timeout != null && timeout >= 0 && stopw.ElapsedMilliseconds > timeout)
                    {
                        reply.result = false;
                        SetTextLst("-->" + "timeout error" +
                                               " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        return reply;
                    }
                    

                    Thread.Sleep(1);
                    if (MyStatic.bReset) return reply;
                }
                

                reply.result = true;
                
                string st = "";
                for (int i = 0; i < reply.data.Length; i++) st = st + reply.data[i].ToString() + " ";
                SetTextLst("-->" + st +  " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }


            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error read:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;



            }
        }
        public  RobotFunctions.CommReply ReadFanucS2RegAsync3(int timeout)
        {
            //SetTextLst("Start read robot" +
            //            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            stopw.Restart();

            try
            {

                var urlwrite = new Uri(sUrlwrite);// html file on D:write reg
                var urlread = new Uri("http://192.168.0.20/example3.stm");// new Uri(sUrlread);// html file on FR:read reg
                var urlrobot = new Uri(sUrlrobot);// html file on FR:read reg
                var urlswrite = new Uri(sUrlswrite);// html file on D:write reg



                reply.result = false;
                Task.Run(() => SetTextLst("<--" +"start read" + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                while (true)
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlread);
                    //Thread.Sleep(10000);//test
                    req.Timeout = 200;


                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    System.IO.Stream st1 = res.GetResponseStream();
                    string body = "";
                    System.IO.StreamReader sr = new System.IO.StreamReader(st1);

                    body = sr.ReadToEnd();
                    Task.Run(() => SetTextLst("-->" +body + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                    return reply;


                    Thread.Sleep(20);
                    Application.DoEvents();
                    if (MyStatic.bReset) return reply;
                    //body = sr.ReadToEnd();
                    if (timeout != null && timeout >= 0 && stopw.ElapsedMilliseconds > timeout)
                    {
                        reply.result = false;
                        SetTextLst("-->" + "timeout error" +
                                               " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        return reply;
                    }
                    int a = body.IndexOf("<BODY>");
                    int b = body.IndexOf("</BODY>");
                    string str = body.Substring(a + 6, b - a - 6);
                    string[] data = str.Split('\n');
                    //SetTextLst("-->" + data[1] +
                    //                         " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                    Array.Resize<Single>(ref reply.data, 20);
                    string[] data1 = data[1].Split(',');
                    if (data1[0].IndexOf("cmd") == 0 && data1[data1.Length - 1] == "end")
                    {
                        for (int i = 0; i < reply.data.Length; i++) reply.data[i] = 0;
                        reply.data[0] = Single.Parse(data1[0].Substring(3, 2));
                        for (int i = 1; i < data1.Length - 1; i++)
                        {
                            reply.data[i] = Single.Parse(data1[i]);
                        }
                        //enter receive data into result.data
                        string str1 = "";
                        for (int i = 0; i < reply.data.Length; i++) str1 = str1 + " " + reply.data[i].ToString();
                        reply.result = true;
                        SetTextLst("Read Reg-->" + str1 +
                                              " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        //SetTextLst("-->" + "end read str" +
                        //                       " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        return reply;
                    }
                    else
                    {
                        //reply.result = true;
                        //SetTextLst("-->" + "end read1 str" +
                        //                        " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                        //return reply;
                    }
                    Thread.Sleep(1);
                }
            }


            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error read:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                //return reply;
            }

            reply.result = true;
            //return ;
            /////////
            string st = "";
            for (int i = 0; i < reply.data.Length; i++) st = st + reply.data[i].ToString() + " ";
            Task.Run(() => SetTextLst("-->" + st +
                                            " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
            return reply;
        }
        CancellationTokenSource cancelToken = new CancellationTokenSource();
        System.Net.WebRequest reqwr = null;
        public  RobotFunctions.CommReply WriteFanucRegAsync(string reg, string regnum, string value,int timeout)
        {
                RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
                Stopwatch stopw = new Stopwatch();
            cancelToken = new CancellationTokenSource();
            CancellationToken cancel = cancelToken.Token;

            try
            {

                Task.Run(() => SetTextLst("Write Reg<--" +reg+" "+ regnum+" "+ value+" "+
                                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                stopw.Restart();
                string Url = sUrlregwrite;// "http://192.168.0.20/karel/mpnlsrv ";
               
                string Data = "object=" + reg + "&" +
                    "operate=setreal" + "&" +
                    "index=" + regnum + "&" +
                    "value=" + value;
                reqwr = System.Net.WebRequest.Create(Url + "?" + Data);
                reqwr.Proxy = null;
                
                //reqwr.Credentials = CredentialCache.DefaultCredentials;
                //reqwr.Method = "HEAD";



                System.Net.WebResponse resp = (WebResponse) reqwr.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out =  sr.ReadToEnd();

                sr.Close();
                sr.Dispose();
                //SetTextLst("-->" + "End write:"  +
                //                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                reply.result = true;
                
                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error write HTML:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }
            SetTextLst("-->" + "end write: " +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            return reply;
        }
        public void StopComm()
        {
            try
            {
                reqwr.Abort();

            }
            catch (Exception ex) { }
        }
        public  RobotFunctions.CommReply WriteFanucRegIntAsync(string reg, string regnum, string value, int timeout)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            try
            {

                Task.Run(() => SetTextLst("Write Reg<--" + reg + " " + regnum + " " + value + " " +
                                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                stopw.Restart();
                string Url = sUrlregwrite;// "http://192.168.0.20/karel/mpnlsrv ";
                //string Data = "object=numreg" + "&" + "operate=setreal" + "&"+
                //    "index=1" + "&" + "value=2";
                //string reg = cmbReg.Text.Trim();
                //string regnum = txtRegNum.Text.Trim();
                //string value = txtRegVal.Text.Trim();
                string Data = "object=" + reg + "&" +
                    "operate=setint" + "&" +
                    "index=" + regnum + "&" +
                    "value=" + value;
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
                System.Net.WebResponse resp = (WebResponse)req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd();

                sr.Close();
                //SetTextLst("-->" + "End write:"  +
                //                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                reply.result = true;
                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error write HTML:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }
            SetTextLst("-->" + "end write: " +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            return reply;
        }
        public RobotFunctions.CommReply ReadFanucPosRegAsync(string reg, string regnum,  int timeout)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            try
            {

                Task.Run(() => SetTextLst("Write Reg<--" + reg + " " + regnum + " " + " " +
                                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                stopw.Restart();
                string Url = sUrlregwrite;// "http://192.168.0.20/karel/mpnlsrv ";
                //string Data = "object=numreg" + "&" + "operate=setreal" + "&"+
                //    "index=1" + "&" + "value=2";
                //string reg = cmbReg.Text.Trim();
                //string regnum = txtRegNum.Text.Trim();
                //string value = txtRegVal.Text.Trim();
                string Data = "object=" + reg + "&" +
                    "operate=rdpos" + "&" +
                    "index=" + regnum;
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
                System.Net.WebResponse resp = (WebResponse)req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd();

                sr.Close();
                //SetTextLst("-->" + "End write:"  +
                //                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                reply.result = true;
                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error write HTML:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }
            SetTextLst("-->" + "end write: " +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            return reply;
        }
        public RobotFunctions.CommReply ReadFanucIOAsync(string reg, string regnum,string operate,string value="", int timeout=500)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            try
            {

                Task.Run(() => SetTextLst("Write Reg<--" + "object=" + reg + "&" + "operate=" + operate + "&" +  "index=" + regnum + "&" + "value=" + value +
                " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                stopw.Restart();
                string Url = sUrlregwrite;// "http://192.168.0.20/karel/mpnlsrv ";
                //string Data = "object=numreg" + "&" + "operate=setreal" + "&"+
                //    "index=1" + "&" + "value=2";
                //string reg = cmbReg.Text.Trim();
                //string regnum = txtRegNum.Text.Trim();
                //string value = txtRegVal.Text.Trim();
                string Data = "object=" + reg + "&" +
                    "operate="+operate + "&" +
                    "index=" + regnum + "&" + "value=" + value;
                //
                //Uri siteUri = new Uri(Url);
                //ServicePoint sp = ServicePointManager.FindServicePoint(siteUri);
                //sp.ConnectionLimit = 10;
                //ServicePointManager.DefaultConnectionLimit = 10;
                //
                //ServicePointManager.UseNagleAlgorithm = true;
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;
                    //
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
                req.Timeout = 100;
                //var task = Task.Run(() => req.GetResponseAsync());
                //Thread.Sleep(100);
                //await task;
                //System.Net.WebResponse resp = task.Result;
                //System.Net.WebResponse resp = null;
                
                //resp = (WebResponse)await req.GetResponseAsync();
                //while (resp == null)
                //{
                //    //resp = (WebResponse)req.GetResponseAsync().Result;
                //    req.Timeout = 1000;
                //    req.GetResponseAsync();
                //    if (MyStatic.bReset)
                //    {
                //        reply.result = false;
                //        return reply;
                //    }
                //    Thread.Sleep(100);
                //}

                System.Net.WebResponse resp = (WebResponse) req.GetResponse();
                //System.Net.WebResponse resp = (WebResponse) req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out =  sr.ReadToEnd();

                sr.Close();
                //SetTextLst("-->" + "End write:"  +
                //                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                reply.result = true;
                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment =  ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }
            SetTextLst("-->" + "end write: " +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            return reply;
        }
        public RobotFunctions.CommReply RunCmdFanuc(SendRobotParms Parms, bool debug = false)
        //send cmd to robot
        {

            
            int cmd = int.Parse(Parms.cmd);
            //frmMain.newFrmMain.ControlsEnable(false);
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
           
            //send message
            var ctss = new CancellationTokenSource();
            int timeout = (int)(Parms.timeout * 1000);
            ctss.CancelAfter((int)Parms.timeout * 1000);
            string ErrMessage = "";
           

            MyStatic.TaskExecute = true;
            if (debug)
            {
                Thread.Sleep(1000);
                reply.result = true;
                return reply;

            }
            else
            {
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
                    // string val = "cmd10,5,6,7,end";//robot command
                    reply= WriteFanucRegAsync(reg, num, val, 1000);
                    
                    if (!reply.result) { return reply; }

                    //wait result
                    reply= ReadFanucS2RegAsync((int)Parms.timeout * 1000);
                    
                    //clear R[100]
                     WriteFanucRegAsync("numreg", "100", "0", 1000);
                    

                    if (reply.data==null ||  reply.data[0] != cmd)
                    { reply.result = false; }
                    return reply;
                }
                catch (Exception ex)
                {
                    reply.result = false;
                    return reply;

                }
            }
            return reply;
            //return (CommReply);



        }
        public  RobotFunctions.CommReply ReadHTTP(string order, int timeout)
        {

            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();

            stopw.Restart();
            int err = 0;
            try
            {

                //string surlread = "http://galws-ltd.ssl.imc-grp.com.iscar.com:10080/IS/machine/MillingPack," + order;
                string surlread = "https://galws-ltd.ssl.imc-grp.com:10443/is/machine/MillingPack," + order;
                var urlread = new Uri(surlread);// html file on FR:read reg

                reply.result = false;

                while (true)
                {
                    try
                    {

                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlread);

                        req.Timeout = 2000;
                        req.ReadWriteTimeout = 2000;
                        req.KeepAlive = false;
                        System.IO.Stream st1 = null;
                        try
                        {
                            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                            st1 = res.GetResponseStream();
                        }
                        catch { }

                        if (st1 != null)
                        {

                            string body = "";
                            System.IO.StreamReader sr = new System.IO.StreamReader(st1);
                            {
                                body = sr.ReadToEnd();
                            }

                            Thread.Sleep(20);
                            Application.DoEvents();
                            if (MyStatic.bReset) return reply;

                            if (timeout != null && timeout >= 0 && stopw.ElapsedMilliseconds > timeout)
                            {
                                reply.result = false;

                                SetTextLst("-->" + "timeout error" +
                                                       " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                                return reply;
                            }

                            reply.result = true;
                            if (body != null)
                            {
                                SetTextLst("S400-->" + body +
                                                      " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                                reply.comment = body;
                                return reply;
                            }
                        }
                        else if (timeout != null && timeout >= 0 && stopw.ElapsedMilliseconds > timeout)
                        {
                            reply.result = false;

                            SetTextLst("-->" + "timeout error" +
                                                   " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                            return reply;
                        }
                    }
                    catch (Exception ex)
                    {
                        err++;
                        stopw.Restart();
                        if (err > 2)
                        {
                            reply.result = false;
                            reply.comment = "error read:" + ex.Message;

                            SetTextLst("-->" + reply.comment + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                            return reply;
                        }

                    }
                }
               
            }


            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error read:" + ex.Message;

                SetTextLst("-->" + reply.comment + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }



        }
        public RobotFunctions.CommReply RunKCLFanuc(String strKCL, bool debug = false)
        //send cmd to robot
        {


            
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();

            //send message
            var ctss = new CancellationTokenSource();
            int timeout = 5;
            ctss.CancelAfter(5000);
            string ErrMessage = "";


            MyStatic.TaskExecute = true;
            if (debug)
            {
                Thread.Sleep(1000);
                reply.result = true;
                return reply;

            }
            else
            {
                try
                {
                    reply.result = false;
                    
                   
                    reply = WriteFanucKCL(strKCL, 1000);

                    if (!reply.result) { return reply; }

                    //wait result
                    //reply = ReadFanucS2RegAsync((int)Parms.timeout * 1000);

                    //clear R[100]
                   // WriteFanucRegAsync("numreg", "100", "0", 1000);


                    if (reply.data == null )
                    { reply.result = false; }
                    return reply;
                }
                catch (Exception ex)
                {
                    reply.result = false;
                    return reply;

                }
            }
            return reply;
            //return (CommReply);



        }
        public RobotFunctions.CommReply WriteFanucKCL(string strKCL,  int timeout)
        {
            RobotFunctions.CommReply reply = new RobotFunctions.CommReply();
            Stopwatch stopw = new Stopwatch();
            cancelToken = new CancellationTokenSource();
            CancellationToken cancel = cancelToken.Token;

            try
            {

                Task.Run(() => SetTextLst("Write KCL<--" + strKCL + " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm));
                stopw.Restart();
                string Url =  "http://192.168.0.20/KCL/" + strKCL;
                 Url = "http://192.168.0.20/karel/ComSet?sValue=200&sIndx=1&sRealFlag=-1&sFc=2";


                reqwr = System.Net.WebRequest.Create(Url);
                reqwr.Proxy = null;

                //reqwr.Credentials = CredentialCache.DefaultCredentials;
                //reqwr.Method = "HEAD";



                System.Net.WebResponse resp = (WebResponse)reqwr.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();

                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd();
                string strReply = "";
                reply.comment = "";
                Debug.Print(Out);
                if(Out.IndexOf("SHOW VAR [MYKCL] STROUT")>-1)
                {
                    string str1 = "[MYKCL] STROUT  Storage: DRAM  Access: RW  : STRING[128] =";
                    string str2 = "</XMP>";
                    int a = Out.IndexOf(str1);
                    int b = Out.IndexOf(str2);
                    
                    if( b > a) strReply = Out.Substring(a + str1.Length,b- a - str1.Length);
                    //strReply.Replace("\n","").Replace("\r","");
                    reply.comment = strReply.Trim();
                }

                sr.Close();
                sr.Dispose();
                //SetTextLst("-->" + "End write:"  +
                //
                // if(reply.co" // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                if (reply.comment !="" ) reply.result = true;
                SetTextLst("-->" + reply.comment +  " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);

                return reply;

            }
            catch (Exception ex)
            {
                reply.result = false;
                reply.comment = "error write KCL:" + ex.Message;
                //WB1.Stop();
                //WB1.Url = new Uri("about:blank");
                SetTextLst("-->" + reply.comment +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
                return reply;
            }
            SetTextLst("-->" + "end write: " +
                                           " // (" + DateTime.Now.ToString("HH:mm:ss.fff") + ")", lstSend, frm);
            return reply;
        }
    }
}
