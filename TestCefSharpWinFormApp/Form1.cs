using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Net;

namespace TestCefSharpWinFormApp
{
    public partial class Form1 : Form
    {
        private ChromiumWebBrowser chromeBrowser;

        public Form1()
        {
            InitializeComponent();
            InitializeChromium();

            // Register an object in javascript named "cefCustomObject" with function of the CefCustomObject class :3
            //chromeBrowser.RegisterJsObject("cefCustomObject", new CefCustomObject(chromeBrowser, this));
            //Replaced with
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.WcfEnabled = true;
            chromeBrowser.JavascriptObjectRepository.Register("cefCustomObject", new CefCustomObject(chromeBrowser, this), isAsync: false, options: BindingOptions.DefaultBinder);
        }

        //private string testHtml;

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void InitializeChromium()
        {
            var tempText =
@"<p>
<strong>InvokeThread</strong> 解释：
1。UI执行A
2。UI开线程InvokeThread，B和C同时执行，B执行在线程UI上，C执行在线程invokeThread上。
3。invokeThread封送消息给UI，然后自己等待，UI处理完消息后，处理invokeThread封送的消息，即代码段E
4。UI执行完E后，转到线程invokeThread上，invokeThread线程执行代码段D
</p>
";
            //testHtml = "testcontent";
            var urlEncodedContent = WebUtility.UrlEncode(tempText);
            
            String page = string.Format(@"{0}\html-resources\html\editor.html?content={1}",
                Application.StartupPath,
                urlEncodedContent
                );
            //System.Console.WriteLine(page);

            CefSettings settings = new CefSettings();
            //settings.CefCommandLineArgs.Add("disable-gpu", "1");
            Cef.EnableHighDPISupport();
            Cef.Initialize(settings);
            this.chromeBrowser = new ChromiumWebBrowser(page);//"http://www.google.com");
            this.Controls.Add(chromeBrowser);
            this.chromeBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chromeBrowser.Location = new System.Drawing.Point(0, 0);
            this.chromeBrowser.TabIndex = 0;

            // Allow the use of local resources in the browser
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            chromeBrowser.BrowserSettings = browserSettings;


            //chromeBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler();


        }

        //public class RenderProcessMessageHandler : IRenderProcessMessageHandler
        //{
        //    // Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
        //    // If the page has no javascript, no context will be created.
        //    void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        //    {
        //        //const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
        //        string script = GetSetHtmlEditorContentScript();
        //        browserControl.EvaluateScriptAsync(script);
        //    }


        //    public void OnContextReleased(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
        //    {
        //    }

        //    public void OnFocusedNodeChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IDomNode node)
        //    {
        //    }

        //    public void OnUncaughtException(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, JavascriptException exception)
        //    {
        //    }
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string script = string.Format("document.getElementById('startMonth').value;");
            string script = "$('#editor').froalaEditor('html.get')";
            this.chromeBrowser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    //var startDate = response.Result;
                    //startDate is the value of a HTML element.
                    var html = response.Result;
                    MessageBox.Show(html.ToString());
                    // 关闭窗口
                    this.BeginInvoke(new beginInvokeDelegate(this.Close));
                }
            });
        }

        private delegate void beginInvokeDelegate();

        private void button2_Click(object sender, EventArgs e)
        {
            string script = GetSetHtmlEditorContentScript();
            this.chromeBrowser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;
                if (response.Success && response.Result != null)
                {
                }
            });
        }

        private static string GetSetHtmlEditorContentScript()
        {
            string content = "Initialize the Froala <strong>WYSIWYG</strong> HTML 'Editor' on a textarea.";
            string scriptFormat = "$('#editor').froalaEditor('html.set', '{0}')";
            string script = String.Format(scriptFormat, content.Replace("'", "\\'"));
            return script;
        }

    }
}
