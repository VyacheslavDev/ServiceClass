using System.ServiceProcess;

namespace ServiceForRunApp
{
    public partial class ServiceForRunApp : ServiceBase
    {
        public ServiceForRunApp()
        {
            InitializeComponent();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            ServiceClass.CreateFileAndRunWorks();
        }

        protected override void OnStart(string[] args)
        {
            ServiceClass.PrintInLog("Старт работы службы");
            ServiceClass.CreateFileAndRunWorks();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // For correct works service, interval will be not more than 1 time per minute
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            ServiceClass.PrintInLog("Служба остановлена");
        }

        

    }
}
