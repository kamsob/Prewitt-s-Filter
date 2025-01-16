using System.Runtime.InteropServices;

namespace WinFormsApp3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetDllDirectory(string pathname);
        
        [STAThread]
        static void Main()
        {
            string dllDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\x64\Release");
            SetDllDirectory(dllDirectory);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}