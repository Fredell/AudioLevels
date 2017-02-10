using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using AudioLevels.Classes;
using Hardcodet.Wpf.TaskbarNotification;

namespace AudioLevels
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            AudioDevices = new AudioDeviceLocator().LocateConnectedAudioDevices();
            Hide();
        }

        private TaskbarIcon TaskbarIcon { get; set; }
        private IReadOnlyCollection<AudioDevice> AudioDevices { get; }

        [DllImport("user32.dll", EntryPoint = "SetWindowCompositionAttribute")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy {AccentState = AccentState.AccentEnableBlurbehind};

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WcaAccentPolicy,
                Data = accentPtr,
                SizeOfData = accentStructSize
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();

            foreach (var device in AudioDevices)
            {
                var testCtrl = new LevelAdjustmentCluster(device);
                StackPanel.Children.Add(testCtrl);
            }
        }

        private void MainApp_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void MainApp_Initialized(object sender, EventArgs e)
        {
            TaskbarIcon = new TaskbarIcon {Icon = Properties.Resources.TrayIcon};
            TaskbarIcon.TrayLeftMouseUp += Tbi_TrayLeftMouseUp;
            TaskbarIcon.Visibility = Visibility.Visible;

            var exitItem = new MenuItem {Header = "Exit application"};
            exitItem.Click += ExitItem_Click;
            TaskbarIcon.ContextMenu = new ContextMenu();
            TaskbarIcon.ContextMenu.Items.Add(exitItem);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Tbi_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
                Hide();
            else
            {
                Show();
                Activate();
            }

            UpdateLayout();

            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        protected new void Hide()
        {
            base.Hide();
        }

        private void MainApp_Closed(object sender, EventArgs e)
        {
            TaskbarIcon.Dispose();
        }
    }
}