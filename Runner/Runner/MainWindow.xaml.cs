using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Runner.UserProperties;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Windows.Interop;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using LibUsbDotNet.DeviceNotify;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
namespace Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _mainDirPath;
        private string _fileSetting;

        private const short twscVID = 0x284b;
        private const short twscPID = 0x3000;

        List<ToggleButton> devButtons = new List<ToggleButton>();
        public static readonly DependencyProperty DeviceInfoProperty =
            DependencyProperty.Register("SlaveAddr", typeof(string), typeof(MainWindow), new PropertyMetadata(new PropertyChangedCallback(OnDeviceInfoPropertyChanged)));

        public string SlaveAddr
        {
            get { return (string)GetValue(DeviceInfoProperty); }
            set { SetValue(DeviceInfoProperty, value); }
        }

        private static void OnDeviceInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            MainWindow _mainWindow = d as MainWindow;
            SettingsRunner.Default.SlaveAddr = _mainWindow.SlaveAddr;
            return;
        }

        //UsbDetector usbDetector;

        public MainWindow()
        {
            _mainDirPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            _fileSetting = _mainDirPath+ "\\ToucherSetting.xml";

            InitializeComponent();

#if false
            usbDetector = new UsbDetector();
            usbDetector.StateChanged += new UsbStateChangedEventHandler(usbDetector_StateChanged);
#endif
        }

        void usbDetector_StateChanged(bool arrival)
        {
            if (arrival)
                Debug.WriteLine("Add");
            else
                Debug.WriteLine("removed");
            //if (arrival)
            //    MessageBox.Show("add");
            //else
            //    MessageBox.Show("removed");
        }

        private void runStart_Click(object sender, RoutedEventArgs e)
        {
            UserProperty pro = new UserProperty();
            propertyGridRunnder.SelectedObjectName = "Runner";
            propertyGridRunnder.SelectedObject = pro;
        }

        public static UsbDevice MyUsbDevice;
        public static IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;
            SlaveAddr = SettingsRunner.Default.SlaveAddr;

        }

        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            // A Device system-level event has occured

            switch (e.EventType)
            {
                case EventType.DeviceArrival:
                case EventType.DeviceRemoveComplete:
                    {
                        Debug.WriteLine(e.EventType.ToString());                        
                        Debug.WriteLine("FullName:" + e.Device.Name);
                        if((e.Device.IdVendor == 0x284b)&&(e.Device.IdProduct == 0x3000))
                            DeviceChange();
                        //Debug.WriteLine(e.ToString());
                    }
                    break;
                default:
                    {
                        Debug.WriteLine(e.EventType.ToString());
                        Debug.WriteLine("FullName:" + e.Device.Name);
                    }
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UsbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent;            
            SettingsRunner.Default.Save();
        }

        private void DeviceChange()
        {
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;

            if(devButtons.Count > 0)
                devButtons.Clear();
            foreach (UsbRegistry usbRegistry in allDevices)
            {
                if (usbRegistry.Open(out MyUsbDevice))
                {
                    if((MyUsbDevice.Info.Descriptor.ProductID == twscPID)&&
                            (MyUsbDevice.Info.Descriptor.VendorID == twscVID))
                    {
                        byte[] buffer = new byte[256];
                        buffer[0] = 0x01;
                        buffer[1] = 0x1C;
                        buffer[2] = 0x12;
                        buffer[3] = 0x00;
                        UsbSetupPacket setupPacket = new UsbSetupPacket(0x40,0xB0, 0, 3, 4);
                        int len = 4;
                        int transferLen;
                        if(MyUsbDevice.ControlTransfer(ref setupPacket, buffer, len, out transferLen))
                        {
                            Debug.WriteLine("PowerOn OK!");
                        }
                    }
                }
            }

            for(int i=0; i< devButtons.Count; i++)
            {
                devStackPanel.Children.Add(devButtons[i]);
            }

            if(devButtons.Count > 0)
            {
                devsExpander.IsExpanded = true;
            }
            else
            {
                devStackPanel.Children.Clear();
                devsExpander.IsExpanded = false;
            }

            //foreach (UsbRegistry usbRegistry in allDevices)
            //{

                //    // Opens the USB device for communucation. 
                //    // MyUsbDevice-The newly created UsbDevice.
                //    if (usbRegistry.Open(out MyUsbDevice))
                //    {
                //        //Console.WriteLine(MyUsbDevice.Info.ToString());
                //        // Gets the actual device descriptor the the current UsbDevice.
                //        Debug.WriteLine(MyUsbDevice.Info.ToString());
                //        // Gets all available configurations for this UsbDevice
                //        for (int iConfig = 0; iConfig < MyUsbDevice.Configs.Count; iConfig++)
                //        {
                //            UsbConfigInfo configInfo = MyUsbDevice.Configs[iConfig];
                //            //Console.WriteLine(configInfo.ToString());
                //            Debug.WriteLine(configInfo.ToString());

                //            ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                //            for (int iInterface = 0; iInterface < interfaceList.Count; iInterface++)
                //            {
                //                UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
                //                //Console.WriteLine(interfaceInfo.ToString());
                //                Debug.WriteLine(interfaceInfo.ToString());

                //                ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                //                for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++)
                //                {
                //                    //Console.WriteLine(endpointList[iEndpoint].ToString());
                //                    Debug.WriteLine(endpointList[iEndpoint].ToString());
                //                }
                //            }
                //        }
                //    }
                //}
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> Children { get; set; }
    }
}
