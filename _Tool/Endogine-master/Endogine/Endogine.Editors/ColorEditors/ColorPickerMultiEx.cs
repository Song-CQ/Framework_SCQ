using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Endogine.ColorEx;

namespace Endogine.Editors.ColorEditors
{
    public partial class ColorPickerMultiEx : UserControl
    {
        public event EventHandler ColorChanged;

        public ColorPickerMultiEx()
        {
            InitializeComponent();
            this.FindColorSpaces();
        }

        private void FindColorSpaces()
        {
            Type test = typeof(Endogine.ColorEx.ColorBase);
            Type[] types = test.Assembly.GetTypes();
            string find = "ColorEx.Color";
            //TODO: simple way to get one namespace only?
            List<string> names = new List<string>();
            foreach (Type type in types)
            {
                int index = type.FullName.IndexOf(find);
                if (index > 0 && type.BaseType == typeof(Endogine.ColorEx.ColorBase)) //type.FullName.IndexOf("ColorBase") < 0 
                {
                    string s = type.FullName.Remove(0, index + find.Length).ToUpper();
                    names.Add(s);
                }
            }

            if (names.Contains("HSL"))
            {
                names.Remove("HSL");
                names.Insert(0, "HSL");
            }
            foreach (string name in names)
            {
                this.comboColorSpace.Items.Add(name);
            }
            //this.comboColorSpace.Items.Add("HSB");
            //this.comboColorSpace.Items.Add("HSL");
            //this.comboColorSpace.Items.Add("HWB");
            //this.comboColorSpace.Items.Add("Lab");
            //this.comboColorSpace.Items.Add("RGB");

            this.comboColorSpace.SelectedIndex = 0;
            //this.SetColorType(typeof(Endogine.ColorEx.ColorHsb));
        }

        private void comboColorSpace_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type test = typeof(Endogine.ColorEx.ColorBase);
            string sType = test.AssemblyQualifiedName;
            string space = (string)((ComboBox)sender).SelectedItem;
            sType = sType.Replace("ColorBase", "Color" + space);
            Type type = Type.GetType(sType, true, true); //("Endogine.ColorEx.Color" + space

            ColorBase clr = this.ColorObject;
            this.SetColorType(type);
            this.ColorObject = clr;
        }

        //public Color Color
        //{
        //    get
        //    {
        //        return this.colorPickerMulti1.ColorObject.ColorRGBA;
        //    }
        //    set
        //    {
        //        Endogine.ColorEx.ColorBase clr = this.colorPickerMulti1.ColorObject;
        //        clr.ColorRGBA = value;
        //        this.colorPickerMulti1.ColorObject = clr;
        //    }
        //}
        public ColorBase ColorObject
        {
            get
            {
                return this.colorPickerMulti1.ColorObject;
            }
            set
            {
                ColorBase clr = this.colorPickerMulti1.ColorObject;
                clr.RgbFloat = value.RgbFloat;
                this.colorPickerMulti1.ColorObject = clr;
            }
        }

        public void SetColorType(Type type)
        {
            if (this.colorPickerMulti1.ColorObject != null && this.colorPickerMulti1.ColorObject.GetType() == type)
                return;

            System.Reflection.ConstructorInfo ci = type.GetConstructor(new Type[]{});
            Endogine.ColorEx.ColorBase clr = (Endogine.ColorEx.ColorBase)ci.Invoke(new object[] { });
            if (clr.GetAxisInfo().Length > 3)
            {
                //TODO: can't handle 4-dimensional spaces ATM - add another slider!
                return;
            }
            if (this.colorPickerMulti1.ColorObject == null)
                clr.Vector = new Endogine.Vector4(1, 0, 0, 0);
            else
                clr.RgbFloat = this.colorPickerMulti1.ColorObject.RgbFloat;

            this.colorPickerMulti1.ColorObject = clr;


            this.comboSliderAxis.Items.Clear();

            Endogine.ColorEx.ColorBase.AxisInfo[] infos = clr.GetAxisInfo();
            foreach (Endogine.ColorEx.ColorBase.AxisInfo info in infos)
            {
                this.comboSliderAxis.Items.Add(info.Name);
            }

            this.comboSliderAxis.SelectedIndex = 0;
        }

        private void comboSliderAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            string axis = (string)this.comboSliderAxis.SelectedItem;
            this.colorPickerMulti1.SliderRepresentsAxis = this.comboSliderAxis.SelectedIndex;
        }

        private void colorPickerMulti1_ColorChanged(object sender, EventArgs e)
        {
            if (this.ColorChanged != null)
                this.ColorChanged(this, null);
        }
    }
}