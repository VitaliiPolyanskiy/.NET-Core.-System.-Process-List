using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Security.Principal;

namespace ProcessList
{
    public partial class Form1 : Form
    {
        public SynchronizationContext uiContext;

        public Form1()
        {
            InitializeComponent();
            // Получим контекст синхронизации для текущего потока 
            uiContext = SynchronizationContext.Current;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    uiContext.Send(d => listBox1.Items.Clear(), null);
                    Process[] lp = Process.GetProcesses();
                    foreach (Process p in lp) // список всех процессов, запущенных в системе
                    {
                        // uiContext.Send отправляет синхронное сообщение в контекст синхронизации
                        // SendOrPostCallback - делегат указывает метод, вызываемый при отправке сообщения в контекст синхронизации. 
                        uiContext.Send(d => listBox1.Items.Add(p.ProcessName) /* Вызываемый делегат SendOrPostCallback */,
                            null /* Объект, переданный делегату */);// получим имя очередного процесса
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    uiContext.Send(d => listBox1.Items.Clear(), null);
                    Process[] lp = Process.GetProcesses();
                    foreach (Process p in lp) // список всех процессов, запущенных в системе
                    {
                        if (p.MainWindowHandle != IntPtr.Zero) // только оконный процесс
                            uiContext.Send(d => listBox1.Items.Add(p.ProcessName), null);
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void butGetNETBIOS_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(Environment.MachineName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(Environment.UserDomainName + @"\" + Environment.UserName);
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                MessageBox.Show(user.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // создаем новый процесс
                Process proc = new Process();
                // Запускаем Блокнот
                proc.StartInfo.FileName = "Notepad.exe";
                proc.StartInfo.Arguments = Application.ExecutablePath;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {           
                // Открываем изображение в приложении по умолчанию
                ProcessStartInfo procInfo = new ProcessStartInfo();
                procInfo.FileName = "cat.jpg";
                procInfo.UseShellExecute = true;
                Process.Start(procInfo);

                // Запускаем браузер Chrome с заданным адресом
                procInfo = new ProcessStartInfo(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
                procInfo.UseShellExecute = false;
                procInfo.Arguments = "https://www.microsoft.com/uk-ua";
                Process.Start(procInfo);

                // Запускаем браузер MSEdge с заданным адресом
                procInfo = new ProcessStartInfo("msedge.exe");
                procInfo.UseShellExecute = true;
                procInfo.Arguments = "https://www.microsoft.com/uk-ua";
                Process.Start(procInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем коллекцию процессов Notepad
                Process[] procs = Process.GetProcessesByName("Notepad");
                MessageBox.Show("Всего : " + procs.Length.ToString());
                int i = 0;
                while (i != procs.Length)
                {
                    procs[i].Kill();// останавливаем процесс
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    Process proc = Process.GetProcessesByName("devenv")[0];
                    ProcessThreadCollection processThreads = proc.Threads;
                    uiContext.Send(d => listBox1.Items.Clear(), null);
                    foreach (ProcessThread thread in processThreads)
                    {
                        string str = String.Format("ThreadId: {0}  StartTime: {1}",
                            thread.Id, thread.StartTime);
                        uiContext.Send(d => listBox1.Items.Add(str), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
