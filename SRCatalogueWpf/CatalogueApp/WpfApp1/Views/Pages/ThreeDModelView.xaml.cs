using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
//using System.Windows.Forms;

namespace WpfApp1.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ThreeDModelView.xaml
    /// </summary>
    public partial class ThreeDModelView : UserControl //,IDisposable
    {
        //private const string MODEL_PATH = @"data\GTU.FBX";

        //[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        //public struct HWND__
        //{

        //    /// int
        //    public int unused;
        //}
        public ThreeDModelView()
        {
            InitializeComponent();            

          //  host = new UnityHwndHost(@"data\TreeEnergy\Catalogue_Models\AR1.exe");
          //  this.AppContainer.Child =host;
          //  this.Unloaded += new RoutedEventHandler((s, e) => { this.Dispose(); });

           
            //this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            //this.Loaded += new RoutedEventHandler(OnVisibleChanged);
            //this.SizeChanged += new SizeChangedEventHandler(OnResize);
            //this.Unloaded += new RoutedEventHandler((s, e) => { this.Dispose(); });

        }

       
        //  private UnityHwndHost host;



        // ~ThreeDModelView()
        // {

        // this.Dispose();
        // }



        ///// <summary>
        ///// Track if the application has been created
        ///// </summary>
        //private bool _iscreated = false;

        ///// <summary>
        ///// Track if the control is disposed
        ///// </summary>
        //private bool _isdisposed = false;

        ///// <summary>
        ///// Handle to the application Window
        ///// </summary>
        //IntPtr _appWin;

        //private Process _childp;

        ///// <summary>
        ///// The name of the exe to launch
        ///// </summary>
        //private string exeName = @"data\Catalogue_Models\AR1.exe";

        //public string ExeName
        //{
        //    get
        //    {
        //        return exeName;
        //    }
        //    set
        //    {
        //        exeName = value;
        //    }
        //}


        //[DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
        //     CharSet = CharSet.Unicode, ExactSpelling = true,
        //     CallingConvention = CallingConvention.StdCall)]
        //private static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //[DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        //private static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        //[DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
        //public static extern int SetWindowLongA([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nIndex, int dwNewLong);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        //private const int SWP_NOOWNERZORDER = 0x200;
        //private const int SWP_NOREDRAW = 0x8;
        //private const int SWP_NOZORDER = 0x4;
        //private const int SWP_SHOWWINDOW = 0x0040;
        //private const int WS_EX_MDICHILD = 0x40;
        //private const int SWP_FRAMECHANGED = 0x20;
        //private const int SWP_NOACTIVATE = 0x10;
        //private const int SWP_ASYNCWINDOWPOS = 0x4000;
        //private const int SWP_NOMOVE = 0x2;
        //private const int SWP_NOSIZE = 0x1;
        //private const int GWL_STYLE = (-16);
        //private const int WS_VISIBLE = 0x10000000;
        //private const int WS_CHILD = 0x40000000;


        ///// <summary>
        ///// Force redraw of control when size changes
        ///// </summary>
        ///// <param name="e">Not used</param>
        //protected void OnSizeChanged(object s, SizeChangedEventArgs e)
        //{
        //    this.InvalidateVisual();
        //}


        ///// <summary>
        ///// Create control when visibility changes
        ///// </summary>
        ///// <param name="e">Not used</param>
        //protected void OnVisibleChanged(object s, RoutedEventArgs e)
        //{
        //    // If control needs to be initialized/created
        //    if (_iscreated == false)
        //    {

        //        // Mark that control is created
        //        _iscreated = true;

        //        // Initialize handle value to invalid
        //        _appWin = IntPtr.Zero;

        //        try
        //        {
        //            var procInfo = new System.Diagnostics.ProcessStartInfo(this.exeName);
        //            // procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(this.exeName);
        //            // Start the process
        //            _childp = System.Diagnostics.Process.Start(procInfo);


        //            // Wait for process to be created and enter idle condition
        //            _childp.WaitForInputIdle();


        //            int repeat = 50;
        //            while (_childp.MainWindowHandle == IntPtr.Zero && repeat-- > 0)
        //            {
        //                Thread.Sleep(100); 
        //               // Thread.Yield();
        //            }
        //            if (_childp.MainWindowHandle == IntPtr.Zero)
        //            {
        //                throw new Exception("Runout for starting the process");
        //            }
        //            Debug.WriteLine("Found Unity window: " + _childp.MainWindowHandle);


        //            // Get the main handle
        //            _appWin = _childp.MainWindowHandle;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.Print(ex.Message + "Error");
        //        }

        //        // Put it into this form
        //        var helper = new WindowInteropHelper(Window.GetWindow(this.AppContainer));
        //        SetParent(_appWin, helper.Handle);

        //        //Remove border and whatnot
        //        SetWindowLongA(_appWin, GWL_STYLE, WS_VISIBLE);

        //        //Move the window to overlay it on this window
        //        MoveWindow(_appWin, 200, 200, (int)this.AppContainer.ActualWidth, (int)this.AppContainer.ActualHeight, true);




        //        Console.WriteLine();

        //            // Display current process statistics.

        //            Console.WriteLine($"{_childp} -");
        //            Console.WriteLine("-------------------------------------");

        //            Console.WriteLine($"  Physical memory usage     : {_childp.WorkingSet64}");
        //            Console.WriteLine($"  Base priority             : {_childp.BasePriority}");
        //            Console.WriteLine($"  Priority class            : {_childp.PriorityClass}");
        //            Console.WriteLine($"  User processor time       : {_childp.UserProcessorTime}");
        //            Console.WriteLine($"  Privileged processor time : {_childp.PrivilegedProcessorTime}");
        //            Console.WriteLine($"  Total processor time      : {_childp.TotalProcessorTime}");
        //            Console.WriteLine($"  Paged system memory size  : {_childp.PagedSystemMemorySize64}");
        //            Console.WriteLine($"  Paged memory size         : {_childp.PagedMemorySize64}");



        //            if (_childp.Responding)
        //            {
        //                Console.WriteLine("Status = Running");
        //            }
        //            else
        //            {
        //                Console.WriteLine("Status = Not Responding");
        //            }





        //    }
        //    else
        //    {
        //        this.Dispose();
        //    }
        //}

        ///// <summary>
        ///// Update display of the executable
        ///// </summary>
        ///// <param name="e">Not used</param>
        //protected void OnResize(object s, SizeChangedEventArgs e)
        //{
        //    if (this._appWin != IntPtr.Zero)
        //    {
        //        MoveWindow(_appWin, 0, 0, (int)this.ActualWidth, (int)this.ActualHeight, true);
        //    }
        //}

        //  protected virtual void Dispose(bool disposing)
        // {

        //      host.Dispose();
        // this.AppContainer.Child = null;


        //if (!_isdisposed)
        //{
        //    if (disposing)
        //    {
        //        bool he = _childp.HasExited;
        //        if (_iscreated && _appWin != IntPtr.Zero)
        //        {

        //            bool ex = _childp.WaitForExit(500);

        //            int i = 30;
        //            while (!_childp.HasExited)
        //            {
        //                if (--i > 0)
        //                {
        //                    Debug.WriteLine("Process not dead yet, killing... " + i.ToString());

        //                    bool mes = _childp.CloseMainWindow();

        //                }
        //                Thread.Sleep(100);

        //            }




        //            if (_childp.HasExited)
        //            {
        //                Console.WriteLine();
        //                Console.WriteLine($"  Process exit code          : {_childp.ExitCode}");
        //            }

        //            // _childp.Close();



        //            // Clear internal handle
        //            _appWin = IntPtr.Zero;

        //            Console.WriteLine("process disposed");
        //        }


        //    }
        //    _isdisposed = true;


        //}
        //  }

        //   public void Dispose()
        //   {
        //       this.Dispose(true);
        //       GC.SuppressFinalize(this);
        //   }
    }
}
