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
            // Отримаємо контекст синхронізації для поточного потоку 
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
                    foreach (Process p in lp) // список усіх процесів, запущених у системі
                    {
                        // uiContext.Send відправляє синхронне повідомлення в контекст синхронізації
                        // SendOrPostCallback - делегат указує метод, який викликається під час відправлення повідомлення в контекст синхронізації. 
                        uiContext.Send(d => listBox1.Items.Add(p.ProcessName) /* Викликаний делегат SendOrPostCallback */,
                            null /* Об'єкт, переданий делегату */);// отримаємо ім'я чергового процесу
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
                    foreach (Process p in lp) // список усіх процесів, запущених у системі
                    {
                        if (p.MainWindowHandle != IntPtr.Zero) // тільки віконний процес
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
                // створюємо новий процес
                Process proc = new Process();
                // Запускаємо Блокнот
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
                // Відкриваємо зображення у додатку за замовчуванням
                ProcessStartInfo procInfo = new ProcessStartInfo();
                procInfo.FileName = "cat.jpg";
                procInfo.UseShellExecute = true;
                Process.Start(procInfo);

                // Запускаємо браузер Chrome із заданою адресою
                procInfo = new ProcessStartInfo(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
                procInfo.UseShellExecute = false;
                procInfo.Arguments = "https://www.microsoft.com/uk-ua";
                Process.Start(procInfo);

                // Запускаємо браузер MSEdge із заданою адресою
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
                // Отримуємо колекцію процесів Notepad
                Process[] procs = Process.GetProcessesByName("Notepad");
                MessageBox.Show("Усього : " + procs.Length.ToString());
                int i = 0;
                while (i != procs.Length)
                {
                    procs[i].Kill();// зупиняємо процес
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