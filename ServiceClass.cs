using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace KursovoiServise
{
    static class ServiceClass
    {//Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) 
        private static string PathToProgram = "E:/";
        private static List<string> DateStart = new List<string>();
        private static List<string> DateStop = new List<string>();
        private static List<string> path = new List<string>();
        private static List<string> Name = new List<string>();

        public static void CreateFile()
        {
            if (!File.Exists(PathToProgram + "Settings.xml"))
            {

                XmlTextWriter textWritter = new XmlTextWriter(PathToProgram + "Settings.xml", Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("Task");
                textWritter.WriteEndElement();
                textWritter.Close();
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(PathToProgram + "Settings.xml");

            if (xmlDoc.DocumentElement.FirstChild == null)
            {
                for (int i = 1; i < 3; i++)
                {
                    XmlElement name = xmlDoc.CreateElement("Name");
                    name.InnerText = i + "-" + "ExampleName";
                    xmlDoc.DocumentElement.AppendChild(name);

                    XmlElement path = xmlDoc.CreateElement("path");
                    path.InnerText = i + "-" + @"C:\ExamplePath\To.exe";
                    xmlDoc.DocumentElement.AppendChild(path);

                    XmlElement startTime = xmlDoc.CreateElement("StartTime");
                    startTime.InnerText = DateTime.Now.ToString();
                    xmlDoc.DocumentElement.AppendChild(startTime);

                    XmlElement stopTime = xmlDoc.CreateElement("StopTime");
                    stopTime.InnerText = DateTime.Now.ToString();
                    xmlDoc.DocumentElement.AppendChild(stopTime);
                }
                xmlDoc.Save(PathToProgram + "Settings.xml");
            }
            else { ParseXML(xmlDoc);  RunProgramm();}
           
        }


        public static void PrintInLog(string text)
        {
            File.AppendAllText(PathToProgram + "log.txt", Convert.ToString(DateTime.Now) + " - " + text + Environment.NewLine, Encoding.UTF8);
            
        }

        private static void ParseXML(XmlDocument doc)
        {
            var nod = doc.SelectSingleNode("Task");
            try
            {
                if (nod != null)
                {
                    foreach (XmlNode node in nod.SelectNodes("Name"))
                    {
                        Name.Add(node.InnerText);
                    }
                    foreach (XmlNode node in nod.SelectNodes("path"))
                    {
                        path.Add(node.InnerText);
                    }
                    foreach (XmlNode node in nod.SelectNodes("StartTime"))
                    {
                        DateStart.Add(node.InnerText);
                    }
                    foreach (XmlNode node in nod.SelectNodes("StopTime"))
                    {
                        DateStop.Add(node.InnerText);
                    }
                }
                else { PrintInLog("Ошибка в файле Settings. Не найдет тег <Task>"); }
            }
            catch
            {ServiceClass.PrintInLog("Преобразование данных в коллеции произошло с ошибкой");}
        }

        private static void RunProgramm()
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process(); //проверить и переписать участок кода включения программы и ее закрытия
                //В службах не работает то,что работает в консоли.

                foreach (string c in DateStart)
                {
                    if (Convert.ToDateTime(c) <= DateTime.Now)
                    {
                        System.Diagnostics.Process prc = new System.Diagnostics.Process(); // Объявляем объект
                        prc.StartInfo.FileName = @"E:\ProgramFiles\Arduino\arduino.exe";
                        prc.Start(); // Запускаем процесс
                      
                        PrintInLog("Йа тут");
                        foreach (string n in DateStop)
                        {
                            if (Convert.ToDateTime(n) <= DateTime.Now)
                            {
                                PrintInLog("Йа тут2");
                                // prc.Kill(); // Убиваем процесс
                            }
                        }
                    }
                }
            }
            catch { ServiceClass.PrintInLog("RunProgramm - вылетел с ошибкой."); }
        }
    }


}
