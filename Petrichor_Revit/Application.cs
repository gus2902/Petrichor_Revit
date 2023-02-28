using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
//Application 만들때 필요
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
//아이콘 이미지 작업을 위해 필요
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace Petrichor_Revit
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        internal static Application thisApp = null;
        private fRCC m_MyForm;


        #region IExternalApplication Members
        /// <summary>
        /// Implements the OnStartup event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            //Revit이 시작할때 
            CreateRibbonSamplePanel(application);

            m_MyForm = null;   // no dialog needed yet; the command will bring it
            thisApp = this;  // static access to this application instance

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        /// <summary>
        /// Implements the OnShutdown event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Autodesk.Revit.UI.Result OnShutdown(UIControlledApplication application)
        {
            //Revit이 끝날때

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        public void ShowForm(UIApplication uiapp)
        {
            if (m_MyForm == null || m_MyForm.IsDisposed)
            {
                RequestHandler handler = new RequestHandler();

                ExternalEvent exEvent = ExternalEvent.Create(handler);

                m_MyForm = new fRCC(exEvent, handler);
                m_MyForm.Show();
            }
        }
        public void WakeFormUp()
        {
            if (m_MyForm != null)
            {
                m_MyForm.WakeUp();
            }
        }

        #endregion

        private void CreateRibbonSamplePanel(UIControlledApplication application)
        {
            string tabname = "HONGlize";
            application.CreateRibbonTab(tabname);

            string panelname = "Test Panel";
            RibbonPanel panel = application.CreateRibbonPanel(tabname, panelname);

            SplitButtonData buttondata = new SplitButtonData("TEST", "TEST");
            SplitButton button = panel.AddItem(buttondata) as SplitButton;

            PushButton pushbutton = button.AddPushButton(new PushButtonData("TEST", "TEST", @"D:\temp\SynologyDrive\01_Programimg\01_CSharp\02_Revit\HONGlize\HONGlize\bin\Debug\HONGlize.dll", "HONGlize.RevitCacheCleaner"));
            pushbutton.LargeImage = convertFromBitmap(HONGlize.Properties.Resources.Icon32);
            pushbutton.Image = convertFromBitmap(HONGlize.Properties.Resources.Icon32);
            pushbutton.ToolTip = "기능설명";

        }

        BitmapSource convertFromBitmap(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}