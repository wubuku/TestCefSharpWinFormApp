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

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void InitializeChromium()
        {
            String page = string.Format(@"{0}\html-resources\html\editor.html", Application.StartupPath);
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
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }


    }
}
