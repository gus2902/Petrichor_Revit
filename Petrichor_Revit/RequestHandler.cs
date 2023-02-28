using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Petrichor_Revit
{

    public class RequestHandler : IExternalEventHandler
    {
        #region 꼭 필요 항목
        private Request m_request = new Request();

        public Request Request
        {
            get { return m_request; }
        }


        public String GetName()
        {
            return "External Event Sample";
        }
        #endregion


        public void Execute(UIApplication uiapp)
        {
            // Revit과 연동 하기 위한 거
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Autodesk.Revit.DB.View view = doc.ActiveView;

            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;  // no request at this time -> we can leave immediately
                        }
                    case RequestId.Test:
                        {

                            break;
                        }

                    case RequestId.TypeCreate:
                        {
                            DataGridView dgv = fRCC.dv;

                            for (int i = 0; i < dgv.Rows.Count - 1; i++)
                            {
                                string family_name = dgv.Rows[i].Cells[0].Value.ToString();
                                string type_name = dgv.Rows[i].Cells[i].Value.ToString();

                                Family family = null;
                                SojuUtil.GetFamily(doc, family_name, out family);

                                if (family != null)
                                {
                                    FamilySymbol symbol = SojuUtil.CreateFamilySymbol(doc, family, type_name);

                                    if (symbol != null)
                                    {
                                        for (int ii = 2; ii < dgv.Columns.Count; ii++)
                                        {
                                            string param_name = dgv.Columns[ii].HeaderText;
                                            string value = dgv.Rows[i].Cells[ii].Value.ToString();

                                            Parameter param = symbol.LookupParameter(param_name);

                                            if (param != null)
                                            {
                                                double v = 0;

                                                if (double.TryParse(value, out v) && param.StorageType == StorageType.Double)
                                                {
                                                    //SojuUtil.SetParam(doc, param, v.ToString());
                                                    //이미 숫자인거 환인했는데 그냥 넣자

                                                    using (var t = new Transaction(doc))
                                                    {
                                                        t.Start("Start");

                                                        //v = UnitUtils.ConvertToInternalUnits(v, DisplayUnitType.OUT_MILLIMETERS);
                                                        //ConvertToInternalUnits 과 DisplayUnitType은 2021까지만 지원
                                                        param.Set(v);

                                                        t.Commit();
                                                    }
                                                }
                                                else
                                                {
                                                    SojuUtil.SetParam(doc, param, value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            MessageBox.Show("OK");

                            break;
                        }
                    case RequestId.ShowName:
                        {
                            Reference r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                            Element element = doc.GetElement(r.ElementId);

                            TaskDialog.Show("OK", element.Name);

                            break;
                        }

                    default:
                        {
                            // some kind of a warning here should
                            // notify us about an unexpected request 
                            break;
                        }
                }
            }
            finally
            {
                Application.thisApp.WakeFormUp();
            }

            return;
        }

    }

}
