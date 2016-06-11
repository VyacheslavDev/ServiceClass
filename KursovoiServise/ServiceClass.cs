using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Linq;

namespace ServiceForRunApp
{
    static class ServiceClass
    {
        private static string PathToMyDoc = System.AppDomain.CurrentDomain.BaseDirectory + @"\ControlApplication\";

        private static List<string> DateStart = new List<string>();
        private static List<string> DateStop = new List<string>();
        private static List<string> path = new List<string>();
        private static List<string> Name = new List<string>();
        private static int[] Procid = new int[500];

        /// <summary>
        /// Метод для начала работы с файлами
        /// </summary>
        public static void CreateFileAndRunWorks()
        { 
            try
            {
               ParseXMLToCollection(); RunProgramm(); 
            }
            catch (Exception e) { PrintInLog("Ошибка структуры файла XML - попробуйте удалить файл Settings.xml", e); }
        }

        public static void PrintInLog(string text)
        {
            File.AppendAllText(PathToMyDoc + "log.txt", Convert.ToString(DateTime.Now) + " - " + text + Environment.NewLine, Encoding.UTF8);

        }
        public static void PrintInLog(string text, Exception e)
        {
            File.AppendAllText(PathToMyDoc + "log.txt", Convert.ToString(DateTime.Now) + " - " + text + Environment.NewLine + e + Environment.NewLine, Encoding.UTF8);

        }

        /// <summary>
        /// парсинг данных из хмл в коллекции
        /// </summary>
        private static void ParseXMLToCollection()
        {
            try
            {
                if (File.Exists(PathToMyDoc + "Settings.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(PathToMyDoc + "Settings.xml");

                    DateStart.Clear(); DateStop.Clear(); path.Clear(); Name.Clear(); //ClearDataInCollection
                    var nod = doc.SelectSingleNode("Task");

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
            }
            catch (Exception e)
            { ServiceClass.PrintInLog("Преобразование данных в коллеции произошло с ошибкой", e); }
        }
        /// <summary>
        /// Запустить программу
        /// </summary>
        private static void RunProgramm()
        {
            int Processid = 0;
            try
            {
                for (int i = 0; i < DateStart.Count; i++)
                {
                    if (CheckStartProg(i))
                    {
                        PrintInLog("Запуск программы " + Name.ElementAt<string>(i));
                        RunApp(ref Processid, path.ElementAt<string>(i));
                        Procid[i]= Processid;
                    }
                    else if (CheckStopProg(i))
                    {
                        PrintInLog("Закрыть программу " + Name.ElementAt<string>(i));
                        KillProcess(Procid[i]);
                    }
                }
            }
            catch (Exception e) { PrintInLog("RunProgramm - вылетел с ошибкой ", e); }
        }
        
        private static bool CheckStartProg(int index)
        {
            if (string.Format("{0:dd-MM-yyyy HH:mm}", Convert.ToDateTime(DateStart.ElementAt<string>(index))) ==
                string.Format("{0:dd-MM-yyyy HH:mm}", DateTime.Now))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Проверить нужно ли остановить программу
        /// </summary>
        /// <param name="index">Индекс задачи</param>
        private static bool CheckStopProg(int index)
        {
            if (string.Format("{0:dd-MM-yyyy HH:mm}", Convert.ToDateTime(DateStop.ElementAt<string>(index))) ==
               string.Format("{0:dd-MM-yyyy HH:mm}", DateTime.Now))
            {
                return true;
            }
            return false;
        }

      /// <summary>
      /// Убивает процесс по ИД
      /// </summary>
      /// <param name="Processid">Надо указать ИД процесса</param>
        public static void KillProcess(int Processid)
        {
            try
            {
                Process.GetProcessById(Processid).Kill();
                PrintInLog("Процесс " + Process.GetProcessById(Processid).ProcessName + " был закрыт");
            }
            catch
            {
                PrintInLog("Процесс с ID " + Processid + " не был закрыт программно в результате закрытия пользователем, либо ID процесса был изменен самим приложением");
            }
        }

        /// <summary>
        /// Метод запускает приложение, через ядро ОС, в обход 0 сеанса.
        /// </summary>
        /// <param name="Processid">Получение ID запущенного процесса</param>
        /// <param name="path">Путь до файла запуска</param>
        /// <param name="pathcut">Путь до каталога файла запуска<param>
        private static void RunApp(ref int Processid, string path)
        {
            string pathcut = path.Substring(0, 3);
            StringBuilder output = new StringBuilder();

            if (!Win32Api.CreateProcessAsUser(path, pathcut, "winlogon", out output, ref Processid))
            { PrintInLog("Ошибка запуска приложения " + Process.GetProcessById(Processid).ProcessName + " причина ошибки: " + output.ToString()); }
            else
            { PrintInLog("Приложение " + Process.GetProcessById(Processid).ProcessName + " запущенно. (ID процесса= " + Processid + ")"); }
        }
    }

}
