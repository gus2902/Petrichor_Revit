using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.ComponentModel;
using System.Data;
using Quadrant = System.Int32;


namespace Petrichor_Revit
{
    class SojuUtil
    {

        // 매개변수 설정
        public static void SetParam(Document doc, Parameter param, string value)
        {
            if (param.IsReadOnly == true) return; // 읽기 전용이면 그냥 패스~

            using (var t = new Transaction(doc))
            {

                t.Start("Start");

                switch (param.StorageType)
                {
                    case Autodesk.Revit.DB.StorageType.Double:

                        //음.. 일단 유틸이니까 피트로 온다고 생각하자 ``

                        double v = 0;
                        if (double.TryParse(value, out v))
                        {
                            param.Set(v);
                        }

                        break;

                    case Autodesk.Revit.DB.StorageType.ElementId:
                        // 재료 이면서 id를 사용하면 해당 재료를 검색한다.

                        // 다른 경우도 있는지 확인 필요

                        if (ParameterType.Material == param.Definition.ParameterType)
                        {
                            Element element = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToElements().Where(x => x.Name.ToUpper() == value.ToUpper()).First<Element>();

                            if (element != null)
                            {
                                param.Set(element.Id);
                            }
                        }

                        break;


                    case Autodesk.Revit.DB.StorageType.Integer:
                        // 음... 넣는건 그냥 숫자뿐이
                        // 이전에 숫자가 결정되야함

                        int i = 0;
                        if (int.TryParse(value, out i))
                        {
                            param.Set(i);
                        }

                        break;
                    case Autodesk.Revit.DB.StorageType.String:

                        param.Set(value);

                        break;

                    case Autodesk.Revit.DB.StorageType.None:

                        // None 는 뭐하는 놈이냐
                        // 본적 없음 그냥 패스

                        break;

                    default:
                        break;
                }

                t.Commit();

            }


        }

        // 패밀리 로드
        public static Family LoadFamily_FullPath(Document doc, string family_fullpath)
        {
            Family family = null;

            try
            {
                if (File.Exists(family_fullpath))
                {
                    using (var t = new Transaction(doc))
                    {

                        t.Start("Start");

                        doc.LoadFamily(family_fullpath, out family);

                        t.Commit();

                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }

            return family;
        }

        // 신규 패밀리 타입 생성
        public static FamilySymbol CreateFamilySymbol(Document doc, Family family, string type_name)
        {
            FamilySymbol fs = null;

            ElementId d_typeid = family.GetFamilySymbolIds().First<ElementId>(); // 무조껀 하나 이상의 타입은 있다~! 어차피 아무거나 하나 복사해야함.

            FamilySymbol symbol = doc.GetElement(d_typeid) as FamilySymbol;

            // 혹시나 모르니 한번더 타입이 있는지 체크하자
            bool chk = GetFamilySymbol(doc, family.Name, type_name, out fs);

            if (chk) return fs; // 타입이 이미 있는데 호출함

            try
            {
                using (var t = new Transaction(doc))
                {

                    t.Start("Start");

                    fs = symbol.Duplicate(type_name) as FamilySymbol;

                    t.Commit();

                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }

            return fs;

        }


        // 패밀리가 있는지 확인
        public static bool GetFamily(Document doc, string f_name, out Family family)
        {
            family = null;

            // 기본 반복문 사용
            List<Family> list = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().ToList<Family>();

            foreach (Family f in list)
            {
                if (f_name.ToUpper() == f.Name.ToUpper())
                {
                    family = f;

                    return true;
                }
            }

            // 1 줄 처리 / 람다식
            //family = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(x => x.Name.ToUpper() == f_name.ToUpper()).First<Family>();

            if (family != null) return true;

            return false;
        }


        // 해당 이름의 패밀리 타입이 있는지 확인
        // 있으면 패밀리 타입 반환
        public static bool GetFamilySymbol(Document doc, string f_name, string t_name, out FamilySymbol symbol)
        {
            symbol = null;

            // 기본 반복문 사용
            List<FamilySymbol> list = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList<FamilySymbol>();

            foreach (FamilySymbol fs in list)
            {
                string tmp_f_name = fs.FamilyName;

                if (f_name.ToUpper() == tmp_f_name.ToUpper())
                {
                    if (t_name.ToUpper() == fs.Name.ToUpper())
                    {
                        symbol = fs;

                        return true;
                    }
                }
            }

            return false;
        }


    }
}
