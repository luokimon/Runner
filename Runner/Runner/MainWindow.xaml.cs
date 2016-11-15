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
using Runner.DeviceChange;
using System.Windows.Interop;

namespace Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _mainDirPath;
        private string _fileSetting;

        public static readonly DependencyProperty DeviceInfoProperty =
            DependencyProperty.Register("SlaveAddr", typeof(string), typeof(MainWindow), new PropertyMetadata(new PropertyChangedCallback(OnDeviceInfoPropertyChanged)));

        public string SlaveAddr
        {
            get { return (string)GetValue(DeviceInfoProperty); }
            set { SetValue(DeviceInfoProperty, value); }
        }

        private static void OnDeviceInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            return;
        }

        UsbDetector usbDetector;

        public MainWindow()
        {
            _mainDirPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            _fileSetting = _mainDirPath+ "\\ToucherSetting.xml";

            if (false == File.Exists(_fileSetting))
            {
                CreateXMLDocument();
            }

            InitializeComponent();

            usbDetector = new UsbDetector();
            usbDetector.StateChanged += new UsbStateChangedEventHandler(usbDetector_StateChanged);
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

        public void CreateXMLDocument()

        {

            XmlDocument xmlDoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>

            XmlDeclaration xmlDeclar;

            xmlDeclar = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", null);

            xmlDoc.AppendChild(xmlDeclar);
            //加入Employees根元素

            XmlElement xmlElement = xmlDoc.CreateElement("", "Employees", "");

            xmlDoc.AppendChild(xmlElement);
            //添加节点

            XmlNode root = xmlDoc.SelectSingleNode("Employees");

            XmlElement xe1 = xmlDoc.CreateElement("Node");

            xe1.SetAttribute("Name", "李明");

            xe1.SetAttribute("ISB", "2-3631-4");
            //添加子节点

            XmlElement xeSub1 = xmlDoc.CreateElement("title");

            xeSub1.InnerText = "学习VS";

            xe1.AppendChild(xeSub1);


            XmlElement xeSub2 = xmlDoc.CreateElement("price");

            xe1.AppendChild(xeSub2);

            XmlElement xeSub3 = xmlDoc.CreateElement("weight");

            xeSub3.InnerText = "20";

            xeSub2.AppendChild(xeSub3);


            root.AppendChild(xe1);

            xmlDoc.Save(_fileSetting);//保存的路径

        }

        private void runStart_Click(object sender, RoutedEventArgs e)
        {
            UserProperty pro = new UserProperty();
            propertyGridRunnder.SelectedObjectName = "Runner";
            propertyGridRunnder.SelectedObject = pro;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper interop = new WindowInteropHelper(this);
            HwndSource hwndSource = HwndSource.FromHwnd(interop.Handle);
            HwndSourceHook hook = new HwndSourceHook(usbDetector.HwndHandler);
            hwndSource.AddHook(hook); ;
            usbDetector.RegisterDeviceNotification(interop.Handle);
        }
    }
}
