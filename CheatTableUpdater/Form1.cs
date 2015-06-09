using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace CheatTableUpdater
{
    public partial class Form1 : Form
    {

        private PropertyChanger offset = new PropertyChanger();

        private OpenFileDialog dialog;


        public Form1()
        {
            InitializeComponent();
            numericUpDown1.DataBindings.Add("Value", offset, "Offset");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (dialog == null)
            {
                dialog = new OpenFileDialog();
                dialog.DefaultExt = "CT";
                dialog.Filter = "Cheat Engine Table (*.CT)|*.CT|All files (*.*)|*.*";
                dialog.Title = "Load cheat engine table...";
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                XDocument file = new XDocument();

                try
                {
                    file = XDocument.Load(dialog.FileName, LoadOptions.PreserveWhitespace);
                }
                catch (Exception)
                {
                    return;
                }


                if (file.Elements().Count() == 0) return;



                int num = 0;
                string[] temp = {};

                var elements = file.Descendants("Address");

                foreach (XElement ele in elements)
                {

                    temp = ele.Value.Split('+');
                    if (temp.Length == 1) continue;

                    num = int.Parse(temp[1], System.Globalization.NumberStyles.HexNumber);
                    num += offset.Offset;

                    ele.Value = "Agarest2.exe+" + num.ToString("X7");


                }


                temp = dialog.FileName.Split('\\');



                using (var writer = 
                    new XmlTextWriter(Application.StartupPath + "\\" + temp.Last(), new UTF8Encoding(false)))
                {
                    file.Save(writer);
                }

            }

        }


    }


    public class PropertyChanger : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private static int offset = 0;


        public int Offset
        {
            get { return offset; }
            set { offset = value; RaisePropertyChangedEvent("offset"); }
        }


        private void RaisePropertyChangedEvent(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (PropertyChanged != null)
                handler(this, new PropertyChangedEventArgs(propertyName));

        }


    }


}
