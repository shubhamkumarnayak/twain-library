/*
 * Created by SharpDevelop.
 * User: RahulN
 * Date: 25/02/2014
 * Time: 1:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TwainLib;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;


namespace TestComponents
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
    public partial class MainForm : Form, IMessageFilter
	{
        private bool msgfilter;
        private ITwain tw;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
            //Bitmap bmp = new Bitmap("D:\\2.TIFF");
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
        private void EndingScan()
        {
            if (msgfilter)
            {
                Application.RemoveMessageFilter(this);
                msgfilter = false;
                this.Enabled = true;
                this.Activate();
            }
        }
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            TwainCommand cmd = tw.PassMessage(ref m);
            if ((cmd == TwainCommand.Not))
            {
                return false;
            }
            if ((cmd == TwainCommand.Null))
            {
                return false;
            }
            switch (cmd)
            {
                case TwainCommand.CloseRequest:
                    {
                        EndingScan();
                        tw.CloseSrc();
                        break;
                    }
                case TwainCommand.CloseOk:
                    {
                        EndingScan();
                        tw.CloseSrc();
                        break;
                    }
                case TwainCommand.DeviceEvent:
                    {
                        System.Diagnostics.Debug.Print("here");
                        break;
                    }
                case TwainCommand.TransferReady:
                    {
                        ImageNotification delMan = new ImageNotification(GetImage);
                        int pics = 0;
                        pics = tw.TransferPictures(delMan, this);
                        break;
                    }
            }

            return true;
        }
        int i = 0;
        int pagecount = 1;
        public void GetImage(System.IntPtr prmHBmp)
        {
            if (prmHBmp != IntPtr.Zero)
            {
                //Here you need to use any image library to convert Hbmp to bitmap or jpeg or tiff....
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            nLicense.License.nLicense _lcs = new nLicense.License.nLicense();
            tw = _lcs.GetTwain();
            tw.Init(this.Handle);
            if (!tw.Select()) { return; }
            if (!msgfilter)
            {
                msgfilter = true;
                Application.AddMessageFilter(this);
            }
            TwainNegotiationParams p = new TwainNegotiationParams();
            p._DPI = 300;
            p._imgColorTypes = ImageColorTypes.Color;
            p._scantype = Scantype.Simplex;
            p._showUI = false;
            p._xferCount = 1;
            SuccessfulAcquiredParams s = tw.Acquire(p);
        }
	}
}
