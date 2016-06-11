using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ApplicationForControl
{
    public partial class ControlPanel : Form
    {
        /// <summary>
        /// Путь к корневой папке
        /// </summary>
        private static string PathToMyDoc = System.AppDomain.CurrentDomain.BaseDirectory + @"\";

        public ControlPanel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == "" || textBox1.Text == "")
                {
                    Fail();
                }
                else
                {
                    CreateXMLBasic();
                    AddTaskToXml();
                    OK();
                }

            }
            catch { FatalError(); }
        }
        /// <summary>
        /// Создает надпись в статусе "ОК"
        /// </summary>
        private void OK()
        {
            label5.Text = "OK";
            label5.ForeColor = Color.DarkGreen;
            label5.Visible = true;

        }
        /// <summary>
        /// Создает надпись в статусе "Fail"
        /// </summary>
        private void Fail()
        {
            label5.Text = "Fail";
            label5.ForeColor = Color.Red;
            label5.Visible = true;
        }
        /// <summary>
        /// Создает надпись в статусе "FatalError"
        /// </summary>
        private void FatalError()
        {
            label5.Text = "FatalError";
            label5.ForeColor = Color.Blue;
            label5.Visible = true;
        }
        /// <summary>
        /// Открывает проводник для того чтобы узнать путь к exe-файлу
        /// </summary>
        /// <returns>Путь к exeшнику</returns>
        private string FilePath()
        {
            using (OpenFileDialog Ofd = new OpenFileDialog())
            {
                if(Ofd.ShowDialog() == DialogResult.OK)
                {
                    return Ofd.FileName;
                }

            }
            return null;
        }
        /// <summary>
        /// Скрывает статус когда происходит заполнения окна данными
        /// </summary>
        private void VisiblePressOnLableName(object sender, EventArgs e)
        {
            label5.Visible = false;
        }
        /// <summary>
        /// По нажатию на кнопку очищения, происходит удаление XML файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanXMLfile(object sender, EventArgs e)
        {
            label5.Visible = false;
            try
            {
                if (File.Exists(PathToMyDoc + "Settings.xml"))
                {
                    File.Delete(PathToMyDoc + "Settings.xml");
                    OK();
                }
                else { Fail(); }
            }
            catch { FatalError(); }
           
            
        }
        /// <summary>
        /// Создает новую задачу в xml файл
        /// </summary>
        private void AddTaskToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(PathToMyDoc + "Settings.xml");

            XmlElement name = xmlDoc.CreateElement("Name");
            name.InnerText = textBox1.Text;
            xmlDoc.DocumentElement.AppendChild(name);

            XmlElement path = xmlDoc.CreateElement("path");
            path.InnerText = textBox2.Text;
            xmlDoc.DocumentElement.AppendChild(path);

            XmlElement startTime = xmlDoc.CreateElement("StartTime");
            startTime.InnerText = string.Format("{0:dd.MM.yyyy} {1}:{2}", dateTimePicker3.Value, numericUpDown6.Value, numericUpDown5.Value);
            xmlDoc.DocumentElement.AppendChild(startTime);

            XmlElement stopTime = xmlDoc.CreateElement("StopTime");
            stopTime.InnerText = string.Format("{0:dd.MM.yyyy} {1}:{2}", dateTimePicker2.Value, numericUpDown4.Value, numericUpDown2.Value);
            xmlDoc.DocumentElement.AppendChild(stopTime);

            xmlDoc.Save(PathToMyDoc + "Settings.xml");
        }
        /// <summary>
        /// Задает путь в текст бокс
        /// </summary>
        private void InsertPathInTextBox(object sender, EventArgs e)
        {
            textBox2.Text = FilePath();
        }
        /// <summary>
        /// Создает XML файл и создает первоначальную DOM структуру.
        /// </summary>
        private void CreateXMLBasic()
        {
            if (!File.Exists(PathToMyDoc + "Settings.xml"))
            {

                XmlTextWriter textWritter = new XmlTextWriter(PathToMyDoc + "Settings.xml", Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("Task");
                textWritter.WriteEndElement();
                textWritter.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label5.Visible = false;
        }
        
        private void ControlPanel_MouseClick(object sender, MouseEventArgs e)
        {
            label5.Visible = false;
        }
        /// <summary>
        /// Открытие корневой директории по нажатию на кнопку
        /// </summary>
        private void OpenMainDir(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
