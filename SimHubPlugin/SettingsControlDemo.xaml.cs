﻿using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media.TextFormatting;
using System.Text.Json;
using FMOD;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Web;
using MahApps.Metro.Controls;
using System.Runtime.CompilerServices;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Input;
using System.Windows.Shapes;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SimHub.Plugins.OutputPlugins.GraphicalDash.PSE;
using SimHub.Plugins.Styles;
using System.Windows.Media;


namespace User.PluginSdkDemo
{


    /// <summary>
    /// Logique d'interaction pour SettingsControlDemo.xaml
    /// </summary>
    public partial class SettingsControlDemo : System.Windows.Controls.UserControl
    {


        public uint pedalConfigPayload_version = 109;

        public uint indexOfSelectedPedal_u = 1;

        public DataPluginDemo Plugin { get; }

        public DAP_config_st[] dap_config_st = new DAP_config_st[3];
        private string stringValue;




        


        // read config from JSON on startup
        //ReadStructFromJson();


        // read JSON config from JSON file
        //private void ReadStructFromJson()
        //{



        //    try
        //    {
        //        // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
        //        // https://www.educative.io/answers/how-to-read-a-json-file-in-c-sharp

        //        string currentDirectory = Directory.GetCurrentDirectory();
        //        string dirName = currentDirectory + "\\PluginsData\\Common";
        //        //string jsonFileName = ComboBox_JsonFileSelected.Text;
        //        string jsonFileName = ((ComboBoxItem)ComboBox_JsonFileSelected.SelectedItem).Content.ToString();
        //        string fileName = dirName + "\\" + jsonFileName + ".json";

        //        string text = System.IO.File.ReadAllText(fileName);

        //        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(DAP_config_st));
        //        var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
        //        dap_config_st[indexOfSelectedPedal_u] = (DAP_config_st)deserializer.ReadObject(ms);
        //        //TextBox_debugOutput.Text = "Config loaded!";
        //        //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.Text;
        //        //TextBox_debugOutput.Text += "    ";
        //        //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.SelectedIndex;

        //        updateTheGuiFromConfig();

        //    }
        //    catch (Exception caughtEx)
        //    {

        //        string errorMessage = caughtEx.Message;
        //        TextBox_debugOutput.Text = errorMessage;
        //    }


        //}

        private void DrawGridLines()
        {
            // Specify the number of rows and columns for the grid
            int rowCount = 5;
            int columnCount = 5;

            // Calculate the width and height of each cell
            double cellWidth = canvas.Width / columnCount;
            double cellHeight = canvas.Height / rowCount;

            // Draw horizontal gridlines
            for (int i = 1; i < rowCount; i++)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = i * cellHeight,
                    X2 = canvas.Width,
                    Y2 = i * cellHeight,
                    //Stroke = Brush.Black,
                    Stroke = System.Windows.Media.Brushes.LightSteelBlue,
                    StrokeThickness = 1,
                    Opacity = 0.1

                };
                canvas.Children.Add(line);
            }

            // Draw vertical gridlines
            for (int i = 1; i < columnCount; i++)
            {
                Line line = new Line
                {
                    X1 = i * cellWidth,
                    Y1 = 0,
                    X2 = i * cellWidth,
                    Y2 = canvas.Height,
                    //Stroke = Brushes.Black,
                    Stroke = System.Windows.Media.Brushes.LightSteelBlue,
                    StrokeThickness = 1,
                    Opacity = 0.1
                };
                canvas.Children.Add(line);
            }
        }

        private void InitReadStructFromJson()
        {



            try
            {
                // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0
                // https://www.educative.io/answers/how-to-read-a-json-file-in-c-sharp
                string jsonFileName="NA";

                string currentDirectory = Directory.GetCurrentDirectory();
                string dirName = currentDirectory + "\\PluginsData\\Common";
                //string jsonFileName = ComboBox_JsonFileSelected.Text;
                if (indexOfSelectedPedal_u == 0)
                {
                    jsonFileName = ("DiyPedalConfig_Clutch_Default");
                }
                else if (indexOfSelectedPedal_u == 1)
                {
                    jsonFileName = ("DiyPedalConfig_Brake_Default");
                }
                else if (indexOfSelectedPedal_u == 2)
                {
                     jsonFileName = ("DiyPedalConfig_Accelerator_Default");                    
                }

                string fileName = dirName + "\\" + jsonFileName + ".json";
                string text = System.IO.File.ReadAllText(fileName);



                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(DAP_config_st));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
                dap_config_st[indexOfSelectedPedal_u] = (DAP_config_st)deserializer.ReadObject(ms);
                TextBox_debugOutput.Text = "Config loaded!"+ jsonFileName;
                //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.Text;
                //TextBox_debugOutput.Text += "    ";
                //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.SelectedIndex;

                updateTheGuiFromConfig();

            }
            catch (Exception caughtEx)
            {

                string errorMessage = caughtEx.Message;
                TextBox_debugOutput.Text = errorMessage;
            }


        }

        private void UpdateSerialPortList_click()
        {

            var SerialPortSelectionArray = new List<SerialPortChoice>();
            string[] comPorts = SerialPort.GetPortNames();
            if (comPorts.Length > 0)
            {

                foreach (string portName in comPorts)
                {
                    SerialPortSelectionArray.Add(new SerialPortChoice(portName, portName));
                }
            }
            else
            {
                SerialPortSelectionArray.Add(new SerialPortChoice("NA", "NA"));
            }

            SerialPortSelection.DataContext = SerialPortSelectionArray;
        }

        private bool isDragging = false;
        private Point offset;
       


        public SettingsControlDemo()
        {


            for (uint pedalIdx = 0; pedalIdx < 3; pedalIdx++)
            {
                dap_config_st[pedalIdx].payloadHeader_.payloadType = 100;
                dap_config_st[pedalIdx].payloadHeader_.version = (byte)pedalConfigPayload_version;

                dap_config_st[pedalIdx].payloadPedalConfig_.pedalStartPosition = 35;
                dap_config_st[pedalIdx].payloadPedalConfig_.pedalEndPosition = 80;
                dap_config_st[pedalIdx].payloadPedalConfig_.maxForce = 90;
                dap_config_st[pedalIdx].payloadPedalConfig_.relativeForce_p000 = 0;
                dap_config_st[pedalIdx].payloadPedalConfig_.relativeForce_p020 = 20;
                dap_config_st[pedalIdx].payloadPedalConfig_.relativeForce_p040 = 40;
                dap_config_st[pedalIdx].payloadPedalConfig_.relativeForce_p060 = 60;
                dap_config_st[pedalIdx].payloadPedalConfig_.relativeForce_p080 = 80;
                dap_config_st[pedalIdx].payloadPedalConfig_.relativeForce_p100 = 100;
                dap_config_st[pedalIdx].payloadPedalConfig_.dampingPress = 0;
                dap_config_st[pedalIdx].payloadPedalConfig_.dampingPull = 0;
                dap_config_st[pedalIdx].payloadPedalConfig_.absFrequency = 5;
                dap_config_st[pedalIdx].payloadPedalConfig_.absAmplitude = 20;
                dap_config_st[pedalIdx].payloadPedalConfig_.lengthPedal_AC = 150;
                dap_config_st[pedalIdx].payloadPedalConfig_.horPos_AB = 215;
                dap_config_st[pedalIdx].payloadPedalConfig_.verPos_AB = 80;
                dap_config_st[pedalIdx].payloadPedalConfig_.lengthPedal_CB = 200;
                dap_config_st[pedalIdx].payloadPedalConfig_.Simulate_ABS_trigger= 0;
                dap_config_st[pedalIdx].payloadPedalConfig_.Simulate_ABS_value= 80;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_0 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_1 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_2 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_3 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_4 = 0;

                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_0 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_1 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_2 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_3 = 0;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_4 = 0;

                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_p_gain = 0.3f;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_i_gain = 50.0f;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_d_gain = 0.0f;

                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.control_strategy_b = 0;

                InitializeComponent();
                // debug mode invisiable
                text_debug_abs_trigger.Opacity = 0;
                text_debug_max_force.Opacity = 0;
                text_debug_max_pos.Opacity = 0;
                text_debug_min_force.Opacity = 0;
                text_debug_min_pos.Opacity = 0;
                PedalMaxForce_Slider.Opacity = 0;
                PedalMaxPos_Slider.Opacity = 0;
                PedalMinForce_Slider.Opacity = 0;
                PedalMinPos_Slider.Opacity = 0;
                Simulate_ABS_slider.Opacity = 0;

                //setting drawing color with Simhub theme workaround
                text_min_force.Foreground= btn_update.Background;
                text_max_force.Foreground = btn_update.Background;
                text_max_pos.Foreground = btn_update.Background;
                text_min_pos.Foreground = btn_update.Background;
                rect0.Fill = btn_update.Background;
                rect1.Fill = btn_update.Background;
                rect2.Fill = btn_update.Background;
                rect3.Fill = btn_update.Background;
                rect4.Fill = btn_update.Background;
                rect5.Fill = btn_update.Background;
                rect6.Fill = btn_update.Background;
                rect7.Fill = btn_update.Background;
                rect8.Fill = btn_update.Background;
                rect9.Fill = btn_update.Background;
                Line_V_force.Stroke = btn_update.Background;
                Line_H_pos.Stroke = btn_update.Background;
                Polyline_BrakeForceCurve.Stroke = btn_update.Background;


                // Call this method to generate gridlines on the Canvas
                DrawGridLines();

            }

        }



        public byte[] getBytesPayload(payloadPedalConfig aux)
        {
            int length = Marshal.SizeOf(aux);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            byte[] myBuffer = new byte[length];

            Marshal.StructureToPtr(aux, ptr, true);
            Marshal.Copy(ptr, myBuffer, 0, length);
            Marshal.FreeHGlobal(ptr);

            return myBuffer;
        }


        public byte[] getBytes(DAP_config_st aux)
        {
            int length = Marshal.SizeOf(aux);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            byte[] myBuffer = new byte[length];

            Marshal.StructureToPtr(aux, ptr, true);
            Marshal.Copy(ptr, myBuffer, 0, length);
            Marshal.FreeHGlobal(ptr);

            return myBuffer;
        }


        //public byte[] getBytes_Action(DAP_action_st aux)
        //{
        //    int length = Marshal.SizeOf(aux);
        //    IntPtr ptr = Marshal.AllocHGlobal(length);
        //    byte[] myBuffer = new byte[length];

        //    Marshal.StructureToPtr(aux, ptr, true);
        //    Marshal.Copy(ptr, myBuffer, 0, length);
        //    Marshal.FreeHGlobal(ptr);

        //    return myBuffer;
        //}


        public DAP_config_st getConfigFromBytes(byte[] myBuffer)
        {
            DAP_config_st aux;

            // see https://stackoverflow.com/questions/31045358/how-do-i-copy-bytes-into-a-struct-variable-in-c
            int size = Marshal.SizeOf(typeof(DAP_config_st));
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(myBuffer, 0, ptr, size);

            aux = (DAP_config_st)Marshal.PtrToStructure(ptr, typeof(DAP_config_st));
            Marshal.FreeHGlobal(ptr);

            return aux;
        }


        //unsafe private UInt16 checksumCalc(byte* data, int length)
        //{

        //    UInt16 curr_crc = 0x0000;
        //    byte sum1 = (byte)curr_crc;
        //    byte sum2 = (byte)(curr_crc >> 8);
        //    int index;
        //    for (index = 0; index < length; index = index + 1)
        //    {
        //        int v = (sum1 + (*data));
        //        sum1 = (byte)v;
        //        sum1 = (byte)(v % 255);

        //        int w = (sum1 + sum2) % 255;
        //        sum2 = (byte)w;

        //        data++;// = data++;
        //    }

        //    int x = (sum2 << 8) | sum1;
        //    return (UInt16)x;
        //}


        public SettingsControlDemo(DataPluginDemo plugin) : this()
        {
            this.Plugin = plugin;

            UpdateSerialPortList_click();

            // check if Json config files are present, otherwise create new ones
            //for (int jsonIndex = 0; jsonIndex < ComboBox_JsonFileSelected.Items.Count; jsonIndex++)
            //{

            //    ComboBox_JsonFileSelected.SelectedIndex = jsonIndex;

            //    // which config file is seleced
            //    string currentDirectory = Directory.GetCurrentDirectory();
            //    string dirName = currentDirectory + "\\PluginsData\\Common";
            //    //string jsonFileName = ComboBox_JsonFileSelected(ComboBox_JsonFileSelected.Items[jsonIndex]).Text;
            //    string jsonFileName = ((ComboBoxItem)ComboBox_JsonFileSelected.SelectedItem).Content.ToString();
            //    string fileName = dirName + "\\" + jsonFileName + ".json";


            //    // Check if file already exists, otherwise create    
            //    if (!File.Exists(fileName))
            //    {
            //        // create default config
            //        // https://stackoverflow.com/questions/3275863/does-net-4-have-a-built-in-json-serializer-deserializer
            //        // https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data?redirectedfrom=MSDN
            //        var stream1 = new MemoryStream();
            //        var ser = new DataContractJsonSerializer(typeof(DAP_config_st));
            //        ser.WriteObject(stream1, Plugin.dap_config_initial_st);

            //        stream1.Position = 0;
            //        StreamReader sr = new StreamReader(stream1);
            //        string jsonString = sr.ReadToEnd();

            //        System.IO.File.WriteAllText(fileName, jsonString);
            //    }
            //}

            string currentDirectory = Directory.GetCurrentDirectory();
            string dirName = currentDirectory + "\\PluginsData\\Common";
            //string jsonFileName = ComboBox_JsonFileSelected(ComboBox_JsonFileSelected.Items[jsonIndex]).Text;
            string jsonFileNameA = "DiyPedalConfig_Accelerator_Default";
            string jsonFileNameB = "DiyPedalConfig_Brake_Default";
            string jsonFileNameC = "DiyPedalConfig_Clutch_Default";
            string fileNameA = dirName + "\\" + jsonFileNameA + ".json";
            string fileNameB = dirName + "\\" + jsonFileNameB + ".json";
            string fileNameC = dirName + "\\" + jsonFileNameC + ".json";

            if (!File.Exists(fileNameA))
            {
                // create default config
                // https://stackoverflow.com/questions/3275863/does-net-4-have-a-built-in-json-serializer-deserializer
                // https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data?redirectedfrom=MSDN
                var stream1 = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof(DAP_config_st));
                ser.WriteObject(stream1, Plugin.dap_config_initial_st);

                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                string jsonString = sr.ReadToEnd();

                System.IO.File.WriteAllText(fileNameA, jsonString);
            }

            if (!File.Exists(fileNameB))
            {
                // create default config
                // https://stackoverflow.com/questions/3275863/does-net-4-have-a-built-in-json-serializer-deserializer
                // https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data?redirectedfrom=MSDN
                var stream1 = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof(DAP_config_st));
                ser.WriteObject(stream1, Plugin.dap_config_initial_st);

                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                string jsonString = sr.ReadToEnd();

                System.IO.File.WriteAllText(fileNameB, jsonString);
            }
            if (!File.Exists(fileNameC))
            {
                // create default config
                // https://stackoverflow.com/questions/3275863/does-net-4-have-a-built-in-json-serializer-deserializer
                // https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data?redirectedfrom=MSDN
                var stream1 = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof(DAP_config_st));
                ser.WriteObject(stream1, Plugin.dap_config_initial_st);

                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                string jsonString = sr.ReadToEnd();

                System.IO.File.WriteAllText(fileNameC, jsonString);
            }

            for (uint pedalIndex = 0; pedalIndex < 3; pedalIndex++)
            {
                indexOfSelectedPedal_u = pedalIndex;
                //ComboBox_JsonFileSelected.SelectedIndex = Plugin.Settings.selectedJsonFileNames[indexOfSelectedPedal_u];
                //ComboBox_JsonFileSelected.SelectedIndex = Plugin.Settings.selectedJsonIndexLast[indexOfSelectedPedal_u];
                InitReadStructFromJson();
                updateTheGuiFromConfig();

            }

        }




        private void updateTheGuiFromConfig()
        {
            // update the sliders
            PedalMinForce_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce;
            PedalMaxForce_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce;

            PedalMinPos_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
            PedalMaxPos_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;

            PedalAbsAmplitude_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.absAmplitude;
            PedalAbsFrequency_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.absFrequency;

            PedalDampening_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.dampingPress;

            PID_tuning_P_gain_slider.Value = (double)dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_p_gain;
            PID_tuning_I_gain_slider.Value = (double)dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_i_gain;
            PID_tuning_D_gain_slider.Value = (double)dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_d_gain;

            PID_tuning_control_strategy_slider.Value = (byte)dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.control_strategy_b;
            Simulate_ABS_slider.Value = (byte)dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value;

            maxGameOutput_slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxGameOutput;

            kfModelNoiseScaling_slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.kf_modelNoise;

            debugFlagSlider_0.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.debug_flags_0;

            Update_BrakeForceCurve();
            if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_trigger == 1)
            {
                Simulate_ABS_check.IsChecked = true;
            }
            else
            {
                Simulate_ABS_check.IsChecked = false;
            }
            //set control point position
            text_point_pos.Opacity = 0;
            Canvas.SetTop(rect0, -1 * (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p000 - canvas.Height + rect0.Height / 2));
            Canvas.SetLeft(rect0, 0*canvas.Width/5-rect0.Width/2);
            Canvas.SetTop(rect1, -1 * (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p020 - canvas.Height + rect1.Height / 2));
            Canvas.SetLeft(rect1, 1 * canvas.Width / 5 - rect1.Width / 2);
            Canvas.SetTop(rect2, -1 * (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p040 - canvas.Height + rect2.Height / 2));
            Canvas.SetLeft(rect2, 2 * canvas.Width / 5 - rect2.Width / 2);
            Canvas.SetTop(rect3, -1 * (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p060 - canvas.Height + rect3.Height / 2));
            Canvas.SetLeft(rect3, 3 * canvas.Width / 5 - rect3.Width / 2);
            Canvas.SetTop(rect4, -1 * (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p080 - canvas.Height + rect4.Height / 2));
            Canvas.SetLeft(rect4, 4 * canvas.Width / 5 - rect4.Width / 2);
            Canvas.SetTop(rect5, -1 * (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p100 - canvas.Height + rect5.Height / 2));
            Canvas.SetLeft(rect5, 5 * canvas.Width / 5 - rect5.Width / 2);
            //set for ABS slider
            Canvas.SetLeft(rect_SABS_Control, dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value * canvas.Width / 100);
            Canvas.SetTop(rect_SABS_Control , 0);
            Canvas.SetLeft(rect_SABS, dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value * canvas.Width / 100);
            Canvas.SetTop(rect_SABS, 0);
            rect_SABS.Width = canvas.Width - dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value * canvas.Width / 100;
            Canvas.SetLeft(text_SABS, dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value * canvas.Width / 100 + rect_SABS_Control.Width);
            text_SABS.Text = "ABS trigger value: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value + "%";
            if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_trigger == 1)
            {
                rect_SABS.Opacity = 1;
                rect_SABS_Control.Opacity = 1;
                text_SABS.Opacity = 1;
            }
            else {
                rect_SABS.Opacity = 0;
                rect_SABS_Control.Opacity = 0;
                text_SABS.Opacity = 0;
            }
            //set for slider;
            double dx = (canvas_horz_slider.Width-10) / 100;
            Canvas.SetTop(rect6, 15);
            //TextBox_debugOutput.Text= Convert.ToString(dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition);
            Canvas.SetLeft(rect6, rect6.Width / 2+dx* dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition);
            Canvas.SetLeft(text_min_pos, rect6.Width / 2 + dx * dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition - text_min_pos.Width / 2);
            Canvas.SetTop(text_min_pos, canvas_horz_slider.Height - 10);
            text_min_pos.Text = "Min Pos: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition + "%";
            text_max_pos.Text = "Max Pos: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition + "%";
            Canvas.SetTop(rect7, 15);
            Canvas.SetLeft(rect7, rect7.Width / 2 + dx * dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition);
            Canvas.SetLeft(text_max_pos, rect6.Width / 2 + dx * dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition - text_max_pos.Width / 2);
            Canvas.SetTop(text_max_pos, canvas_horz_slider.Height - 10);
            //set for vertical slider
            double dy = (canvas_vert_slider.Height/250);
            Canvas.SetTop(rect8,canvas_vert_slider.Height-dy* dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce);
            Canvas.SetLeft(rect8, canvas_vert_slider.Width / 2 - rect8.Width / 2);
            Canvas.SetLeft(text_min_force, 12 + rect8.Width+3);
            Canvas.SetTop(text_min_force, Canvas.GetTop(rect8) + 3);
            Canvas.SetTop(rect9, canvas_vert_slider.Height - dy * dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce);
            Canvas.SetLeft(rect9, canvas_vert_slider.Width / 2 - rect9.Width / 2);
            Canvas.SetLeft(text_max_force, 12 + rect9.Width+3);
            Canvas.SetTop(text_max_force, Canvas.GetTop(rect9)-6);
            
            text_min_force.Text = "Preload:  " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce + "Kg";
            text_max_force.Text = "Max Force: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce + "Kg";

            //// Select serial port accordingly
            string tmp = (string)Plugin._serialPort[indexOfSelectedPedal_u].PortName;
            try
            {
                SerialPortSelection.SelectedValue = tmp;
                TextBox_debugOutput.Text = "Serial port selected: " + SerialPortSelection.SelectedValue;

            }
            catch (Exception caughtEx)
            {
            }


            if (Plugin._serialPort[indexOfSelectedPedal_u].IsOpen == true)
            {
                ConnectToPedal.IsChecked = true;
            }
            else
            {
                ConnectToPedal.IsChecked = false;
            }

            //try
            //{
            //    //ComboBox_JsonFileSelected.SelectedItem = Plugin.Settings.selectedJsonFileNames[indexOfSelectedPedal_u];
            //    //ComboBox_JsonFileSelected.SelectedValue = (string)Plugin.Settings.selectedJsonFileNames[indexOfSelectedPedal_u];

            //    ComboBox_JsonFileSelected.SelectedIndex = Plugin.Settings.selectedJsonIndexLast[indexOfSelectedPedal_u];

            //    //ReadStructFromJson();


            //    //SerialPortSelection.SelectedValue
            //    //TextBox_debugOutput.Text = "Error 2: ";
            //    //TextBox_debugOutput.Text += Plugin.Settings.selectedJsonFileNames[indexOfSelectedPedal_u];
            //    //TextBox_debugOutput.Text += "     ";
            //    //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.SelectedValue;
            //}
            //catch (Exception caughtEx)
            //{
            //    string errorMessage = caughtEx.Message;
            //    TextBox_debugOutput.Text = "Error 1: ";
            //    TextBox_debugOutput.Text += errorMessage;
            //}

            //= ComboBox_JsonFileSelected.SelectedItem.ToString();

            //ConnectToPedal.IsChecked = true;

            //TextBox_debugOutput.Text = "Pedal selected: " + indexOfSelectedPedal_u;
            //TextBox_debugOutput.Text += ",    connected: " + ConnectToPedal.IsChecked;
            //TextBox_debugOutput.Text += ",    serial port name: " + tmp;

        }




        private void Update_BrakeForceCurve()
        {

            double[] x = new double[6];
            double[] y = new double[6];
            x[0] = 0;
            x[1] = 20;
            x[2] = 40;
            x[3] = 60;
            x[4] = 80;
            x[5] = 100;

            y[0] = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p000;
            y[1] = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p020;
            y[2] = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p040;
            y[3] = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p060;
            y[4] = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p080;
            y[5] = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p100;

            // Use cubic interpolation to smooth the original data
            (double[] xs2, double[] ys2, double[] a, double[] b) = Cubic.Interpolate1D(x, y, 100);


            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_0 = (float)a[0];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_1 = (float)a[1];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_2 = (float)a[2];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_3 = (float)a[3];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_a_4 = (float)a[4];

            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_0 = (float)b[0];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_1 = (float)b[1];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_2 = (float)b[2];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_3 = (float)b[3];
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.cubic_spline_param_b_4 = (float)b[4];


            TextBox_debugOutput.Text = "";
            for (uint i = 0; i < a.Length; i++)
            {
                TextBox_debugOutput.Text += "\na[" + i + "]: " + a[i] + "      b[" + i + "]: " + b[i];
            }


            System.Windows.Media.PointCollection myPointCollection2 = new System.Windows.Media.PointCollection();


            for (int pointIdx = 0; pointIdx < 100; pointIdx++)
            {
                System.Windows.Point Pointlcl = new System.Windows.Point(4 * xs2[pointIdx], ys2[pointIdx]);
                myPointCollection2.Add(Pointlcl);
            }

            this.Polyline_BrakeForceCurve.Points = myPointCollection2;

        }



        public class SerialPortChoice
        {
            public SerialPortChoice(string display, string value)
            {
                Display = display;
                Value = value;
            }

            public string Value { get; set; }
            public string Display { get; set; }
        }



        // Select which pedal to config
        // see https://stackoverflow.com/questions/772841/is-there-selected-tab-changed-event-in-the-standard-wpf-tab-control
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            indexOfSelectedPedal_u = (uint)MyTab.SelectedIndex;

            // update the sliders & serial port selection accordingly
            updateTheGuiFromConfig();
        }





        /********************************************************************************************************************/
        /*							Slider callbacks																		*/
        /********************************************************************************************************************/
        private void Slider_PedalMinForce(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce = Convert.ToByte(e.NewValue);

            if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce > dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce)
            {
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce;
                PedalMinForce_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce;
            }
        }

        private void Slider_PedalMaxForce(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce = Convert.ToByte(e.NewValue);

            if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce < dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce)
            {
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce;
                PedalMaxForce_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce;
            }
        }


        private void Slider_PedalMinPos(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition = Convert.ToByte(e.NewValue);

            if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition > dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition)
            {
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;
                PedalMinPos_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
            }
        }

        private void Slider_PedalMaxPos(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition = Convert.ToByte(e.NewValue);

            if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition < dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition)
            {
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
                PedalMaxPos_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;
            }
        }

        private void Slider_AbsAmplitude(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.absAmplitude = Convert.ToByte(e.NewValue);
        }

        private void Slider_AbsFrequency(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.absFrequency = Convert.ToByte(e.NewValue);
        }

        public void TestAbs_click(object sender, RoutedEventArgs e)
        {
            //if (indexOfSelectedPedal_u == 1)
                if (TestAbs.IsChecked==false)
                { 
                    TestAbs.IsChecked= true;
                    Plugin.sendAbsSignal = (bool)TestAbs.IsChecked;
                    TextBox_debugOutput.Text = "ABS-Test begin";
                }
                else
                {
                    TestAbs.IsChecked = false;
                    //Plugin.sendAbsSignal = !Plugin.sendAbsSignal;
                    Plugin.sendAbsSignal = (bool)TestAbs.IsChecked;
                    TextBox_debugOutput.Text = "ABS-Test stopped";
                }
            
        }

        public void Slider_Dampening(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.dampingPress = Convert.ToByte(e.NewValue);
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.dampingPull = Convert.ToByte(e.NewValue);
        }








        /********************************************************************************************************************/
        /*							PID tuning                      														*/
        /********************************************************************************************************************/
        public void PID_tuning_P_gain_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_p_gain = (float)e.NewValue;
        }

        public void PID_tuning_I_gain_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_i_gain = (float)e.NewValue;
        }

        public void PID_tuning_D_gain_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.PID_d_gain = (float)e.NewValue;
        }

        public void PID_tuning_control_strategy_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.control_strategy_b = (byte)e.NewValue;
        }


        


        public void maxGameOutput_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxGameOutput = (byte)e.NewValue;
        }



        public void kfModelNoiseScaling_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.kf_modelNoise = (byte)e.NewValue;
        }

        public void debugFlagSlider_0_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ( (e.NewValue >= 0) && (e.NewValue <= 255) )
            {
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.debug_flags_0 = (byte)e.NewValue;
            }

            //extBox_debugOutput.Text = e.NewValue.ToString();
        }
        

        /********************************************************************************************************************/
        /*							Write/read config to/from Json file														*/
        /********************************************************************************************************************/

        //private void ComboBox_SelectionChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // https://stackoverflow.com/questions/3721430/what-is-the-simplest-way-to-get-the-selected-text-of-a-combo-box-containing-only

        //        string stringValue = ((ComboBoxItem)ComboBox_JsonFileSelected.SelectedItem).Content.ToString();


        //        // string stringValue = ComboBox_JsonFileSelected.SelectedValue.ToString();

        //        //TextBox_debugOutput.Text = stringValue;
        //        Plugin.Settings.selectedJsonFileNames[indexOfSelectedPedal_u] = stringValue;

        //        Plugin.Settings.selectedJsonIndexLast[indexOfSelectedPedal_u] = ComboBox_JsonFileSelected.SelectedIndex;



        //        //ReadStructFromJson();
        //    }
        //    catch (Exception caughtEx)
        //    {

        //        string errorMessage = caughtEx.Message;
        //        TextBox_debugOutput.Text = errorMessage;
        //    }
        //}




        //public void SaveStructToJson_click(object sender, RoutedEventArgs e)
        //{
        //    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-8-0

        //    try
        //    {
        //        // which config file is seleced
        //        string currentDirectory = Directory.GetCurrentDirectory();
        //        string dirName = currentDirectory + "\\PluginsData\\Common";
        //        string jsonFileName = ComboBox_JsonFileSelected.Text;
        //        string fileName = dirName + "\\" + jsonFileName + ".json";

        //        this.dap_config_st[indexOfSelectedPedal_u].payloadHeader_.version = (byte)pedalConfigPayload_version;

        //        // https://stackoverflow.com/questions/3275863/does-net-4-have-a-built-in-json-serializer-deserializer
        //        // https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data?redirectedfrom=MSDN
        //        var stream1 = new MemoryStream();
        //        var ser = new DataContractJsonSerializer(typeof(DAP_config_st));
        //        ser.WriteObject(stream1, dap_config_st[indexOfSelectedPedal_u]);

        //        stream1.Position = 0;
        //        StreamReader sr = new StreamReader(stream1);
        //        string jsonString = sr.ReadToEnd();

        //        // Check if file already exists. If yes, delete it.     
        //        if (File.Exists(fileName))
        //        {
        //            File.Delete(fileName);
        //        }


        //        System.IO.File.WriteAllText(fileName, jsonString);
        //        TextBox_debugOutput.Text = "Config exported!";

        //    }
        //    catch (Exception caughtEx)
        //    {

        //        string errorMessage = caughtEx.Message;
        //        TextBox_debugOutput.Text = errorMessage;
        //    }

        //}



        //public void ReadStructFromJson_click(object sender, RoutedEventArgs e)
        //{
        //    ReadStructFromJson();
        //}


        /********************************************************************************************************************/
        /*							Refind min endstop																		*/
        /********************************************************************************************************************/
        unsafe public void ResetPedalPosition_click(object sender, RoutedEventArgs e)
        {

            if (Plugin._serialPort[indexOfSelectedPedal_u].IsOpen)
            {
                
                try
                {
                    // compute checksum
                    DAP_action_st tmp;
                    tmp.payloadPedalAction_.resetPedalPos_u8 = 1;


                    DAP_action_st* v = &tmp;
                    byte* p = (byte*)v;
                    tmp.payloadFooter_.checkSum = Plugin.checksumCalc(p, sizeof(payloadHeader) + sizeof(payloadPedalAction));


                    int length = sizeof(DAP_action_st);
                    byte[] newBuffer = new byte[length];
                    newBuffer = Plugin.getBytes_Action(tmp);


                    // clear inbuffer 
                    Plugin._serialPort[indexOfSelectedPedal_u].DiscardInBuffer();

                    // send query command
                    Plugin._serialPort[indexOfSelectedPedal_u].Write(newBuffer, 0, newBuffer.Length);
                }
                catch (Exception caughtEx)
                {
                    string errorMessage = caughtEx.Message;
                    TextBox_debugOutput.Text = errorMessage;
                }




                try
                {
                    DateTime startTime = DateTime.Now;
                    while ((Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0) && (DateTime.Now - startTime).Seconds < 2)
                    {
                        string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                    }
                }
                catch (TimeoutException) { }
            }
        }



        /********************************************************************************************************************/
        /*							System identification																	*/
        /********************************************************************************************************************/
        public void StartSystemIdentification_click(object sender, RoutedEventArgs e)
        {

            TextBox_debugOutput.Text = "Start system identification";


            try
            {

                string currentDirectory = Directory.GetCurrentDirectory();
                string dirName = currentDirectory + "\\PluginsData\\Common";
                string logFileName = "DiyActivePedalSystemIdentification";
                string fileName = dirName + "\\" + logFileName + ".txt";

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }


                // This text is added only once to the file.
                if (!File.Exists(fileName))
                {
                    using (StreamWriter sw = File.CreateText(fileName))
                    {

                        // trigger system identification
                        Plugin._serialPort[indexOfSelectedPedal_u].Write("3");

                        System.Threading.Thread.Sleep(100);


                        // read system return log
                        while (Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0)
                        {
                            string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                            sw.Write(message);

                            System.Threading.Thread.Sleep(20);

                        }
                    }

                }

                TextBox_debugOutput.Text = "Finished system identification";


                ////// trigger system identification
                ////Plugin._serialPort[indexOfSelectedPedal_u].Write("3");

                ////System.Threading.Thread.Sleep(100);


                ////// read system return log
                ////while (Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0)
                ////{
                ////    string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                ////    using (StreamWriter sw = File.AppendText(fileName))
                ////    {
                ////        sw.WriteLine(message);
                ////    }
                ////    System.Threading.Thread.Sleep(100);
                ////}


            }
            catch (Exception caughtEx)
            {
                string errorMessage = caughtEx.Message;
                TextBox_debugOutput.Text = errorMessage;
            }

        }




        /********************************************************************************************************************/
        /*							Serial monitor update																	*/
        /********************************************************************************************************************/
        public void SerialMonitorRead_click(object sender, RoutedEventArgs e)
        {

            // read system return log
            try
            {
                DateTime startTime = DateTime.Now;
                while ((Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0) && (DateTime.Now - startTime).Seconds < 2)
                {
                    string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                    TextBox_serialMonitor.Text += message;
                }
            }
            catch (TimeoutException) { }
        }


        /********************************************************************************************************************/
        /*							Send config to pedal																	*/
        /********************************************************************************************************************/
        unsafe public void SendConfigToPedal_click(object sender, RoutedEventArgs e)
        {
            if (Plugin._serialPort[indexOfSelectedPedal_u].IsOpen)
            {

                // compute checksum
                //getBytes(this.dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_)
                DAP_config_st tmp = this.dap_config_st[indexOfSelectedPedal_u];

                //payloadPedalConfig tmp = this.dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_;
                DAP_config_st* v = &tmp;
                byte* p = (byte*)v;
                this.dap_config_st[indexOfSelectedPedal_u].payloadFooter_.checkSum = Plugin.checksumCalc(p, sizeof(payloadHeader) + sizeof(payloadPedalConfig));


                //TextBox_debugOutput.Text = "CRC simhub calc: " + this.dap_config_st[indexOfSelectedPedal_u].payloadFooter_.checkSum + "    ";

                TextBox_debugOutput.Text = String.Empty;

                try
                {
                    this.dap_config_st[indexOfSelectedPedal_u].payloadHeader_.version = (byte)pedalConfigPayload_version;
                    this.dap_config_st[indexOfSelectedPedal_u].payloadHeader_.storeToEeprom = 1;
                    int length = sizeof(DAP_config_st);
                    //int val = this.dap_config_st[indexOfSelectedPedal_u].payloadHeader_.checkSum;
                    //string msg = "CRC value: " + val.ToString();
                    byte[] newBuffer = new byte[length];
                    newBuffer = getBytes(this.dap_config_st[indexOfSelectedPedal_u]);

                    //TextBox_debugOutput.Text = "ConfigLength" + length;

                    // clear inbuffer 
                    Plugin._serialPort[indexOfSelectedPedal_u].DiscardInBuffer();

                    // send data
                    Plugin._serialPort[indexOfSelectedPedal_u].Write(newBuffer, 0, newBuffer.Length);
                    //Plugin._serialPort[indexOfSelectedPedal_u].Write("\n");
                }
                catch (Exception caughtEx)
                {
                    string errorMessage = caughtEx.Message;
                    TextBox_debugOutput.Text = errorMessage;
                }


                
                System.Threading.Thread.Sleep(100);
                try
                {
                    
                    while (Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0)
                    {
                        string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                        TextBox_debugOutput.Text += message;

                    }
                }
                catch (TimeoutException) { }



            }
        }




        /********************************************************************************************************************/
        /*							Read config from pedal																	*/
        /********************************************************************************************************************/
        unsafe public void ReadConfigFromPedal_click(object sender, RoutedEventArgs e)
        {
            if (Plugin._serialPort[indexOfSelectedPedal_u].IsOpen)
            {


                // compute checksum
                DAP_action_st tmp;
                tmp.payloadPedalAction_.returnPedalConfig_u8 = 1;


                DAP_action_st* v = &tmp;
                byte* p = (byte*)v;
                tmp.payloadFooter_.checkSum = Plugin.checksumCalc(p, sizeof(payloadHeader) + sizeof(payloadPedalAction));


                int length = sizeof(DAP_action_st);
                byte[] newBuffer = new byte[length];
                newBuffer = Plugin.getBytes_Action(tmp);


                // clear inbuffer 
                Plugin._serialPort[indexOfSelectedPedal_u].DiscardInBuffer();

                // send query command
                Plugin._serialPort[indexOfSelectedPedal_u].Write(newBuffer, 0, newBuffer.Length);

                
                // wait for response
                System.Threading.Thread.Sleep(100);

                TextBox_debugOutput.Text = "Reading pedal config";

                try
                {

                    length = sizeof(DAP_config_st);
                    byte[] newBuffer_config = new byte[length];

                    int receivedLength = Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead;

                    if (receivedLength == length)
                    {
                        Plugin._serialPort[indexOfSelectedPedal_u].Read(newBuffer_config, 0, length);


                        DAP_config_st pedalConfig_read_st = getConfigFromBytes(newBuffer_config);
                        
                        // check CRC
                        DAP_config_st* v_config = &pedalConfig_read_st;
                        byte* p_config = (byte*)v_config;


                        if (Plugin.checksumCalc(p_config, sizeof(payloadHeader) + sizeof(payloadPedalConfig)) == pedalConfig_read_st.payloadFooter_.checkSum)
                        {
                            this.dap_config_st[indexOfSelectedPedal_u] = pedalConfig_read_st;
                            updateTheGuiFromConfig();
                            TextBox_debugOutput.Text += "Read config from pedal successful!";
                        }
                        else
                        {
                            TextBox_debugOutput.Text += "CRC mismatch!";
                            TextBox_debugOutput.Text += "Data size mismatch!\n";
                            TextBox_debugOutput.Text += "Expected size: " + length + "\n";
                            TextBox_debugOutput.Text += "Received size: " + receivedLength;
                        }
                    }
                    else 
                    {
                        TextBox_debugOutput.Text += "Data size mismatch";

                        DateTime startTime = DateTime.Now;
                        //TimeSpan diffTime = DateTime.Now - startTime;
                        //int millisceonds = (int)diffTime.TotalSeconds;
                        

                        while ( (Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0) && (DateTime.Now - startTime).Seconds < 2 )
                        {
                            string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                            TextBox_debugOutput.Text += message;

                        }

                    }

                    
                    

                }
                catch (Exception ex)
                {
                    TextBox_debugOutput.Text = ex.Message;
                    ConnectToPedal.IsChecked = false;
                }

                //catch (TimeoutException) { }



            }
        }





        /********************************************************************************************************************/
        /*							Connect to pedal																		*/
        /********************************************************************************************************************/
        unsafe public void ConnectToPedal_click(object sender, RoutedEventArgs e)
        {


            if (ConnectToPedal.IsChecked == false)
            {
                if (Plugin._serialPort[indexOfSelectedPedal_u].IsOpen == false)
                {
                    try
                    {
                        Plugin._serialPort[indexOfSelectedPedal_u].Open();
                        TextBox_debugOutput.Text = "Serialport open";
                        ConnectToPedal.IsChecked = true;

                        try
                        {
                            while (Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0)
                            {
                                string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();                                
                            }
                        }
                        catch (TimeoutException) { }

                    }
                    catch (Exception ex)
                    {
                        TextBox_debugOutput.Text = ex.Message;
                        ConnectToPedal.IsChecked = false;
                    }

                }
                else
                {
                    Plugin._serialPort[indexOfSelectedPedal_u].Close();
                    ConnectToPedal.IsChecked = false;
                    TextBox_debugOutput.Text = "Serialport already open, close it";
                }
            }
            else
            {
                ConnectToPedal.IsChecked = false;
                Plugin._serialPort[indexOfSelectedPedal_u].Close();
                TextBox_debugOutput.Text = "Serialport close";
            }

            ////reading config from pedal

            if (checkbox_pedal_read.IsChecked == true)
            {
                if (Plugin._serialPort[indexOfSelectedPedal_u].IsOpen)
                {


                    // compute checksum
                    DAP_action_st tmp;
                    tmp.payloadPedalAction_.returnPedalConfig_u8 = 1;


                    DAP_action_st* v = &tmp;
                    byte* p = (byte*)v;
                    tmp.payloadFooter_.checkSum = Plugin.checksumCalc(p, sizeof(payloadHeader) + sizeof(payloadPedalAction));


                    int length = sizeof(DAP_action_st);
                    byte[] newBuffer = new byte[length];
                    newBuffer = Plugin.getBytes_Action(tmp);


                    // clear inbuffer 
                    Plugin._serialPort[indexOfSelectedPedal_u].DiscardInBuffer();

                    // send query command
                    Plugin._serialPort[indexOfSelectedPedal_u].Write(newBuffer, 0, newBuffer.Length);


                    // wait for response
                    System.Threading.Thread.Sleep(100);

                    TextBox_debugOutput.Text += "\n" + "Reading pedal config";

                    try
                    {

                        length = sizeof(DAP_config_st);
                        byte[] newBuffer_config = new byte[length];

                        int receivedLength = Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead;

                        if (receivedLength == length)
                        {
                            Plugin._serialPort[indexOfSelectedPedal_u].Read(newBuffer_config, 0, length);


                            DAP_config_st pedalConfig_read_st = getConfigFromBytes(newBuffer_config);

                            // check CRC
                            DAP_config_st* v_config = &pedalConfig_read_st;
                            byte* p_config = (byte*)v_config;


                            if (Plugin.checksumCalc(p_config, sizeof(payloadHeader) + sizeof(payloadPedalConfig)) == pedalConfig_read_st.payloadFooter_.checkSum)
                            {
                                this.dap_config_st[indexOfSelectedPedal_u] = pedalConfig_read_st;
                                updateTheGuiFromConfig();
                                TextBox_debugOutput.Text += "\n" + "Read config from pedal successful!";
                            }
                            else
                            {
                                TextBox_debugOutput.Text += "CRC mismatch!";
                                TextBox_debugOutput.Text += "Data size mismatch!\n";
                                TextBox_debugOutput.Text += "Expected size: " + length + "\n";
                                TextBox_debugOutput.Text += "Received size: " + receivedLength;
                            }
                        }
                        else
                        {
                            TextBox_debugOutput.Text += "Data size mismatch";

                            DateTime startTime = DateTime.Now;
                            //TimeSpan diffTime = DateTime.Now - startTime;
                            //int millisceonds = (int)diffTime.TotalSeconds;


                            while ((Plugin._serialPort[indexOfSelectedPedal_u].BytesToRead > 0) && (DateTime.Now - startTime).Seconds < 2)
                            {
                                string message = Plugin._serialPort[indexOfSelectedPedal_u].ReadLine();
                                TextBox_debugOutput.Text += message;

                            }

                        }




                    }
                    catch (Exception ex)
                    {
                        TextBox_debugOutput.Text = ex.Message;
                        ConnectToPedal.IsChecked = false;
                    }

                    //catch (TimeoutException) { }



                }
            }
            

        }

        /********************************************************************************************************************/
        /*							Serial port selection																	*/
        /********************************************************************************************************************/
        public void UpdateSerialPortList_click(object sender, RoutedEventArgs e)
        {
            UpdateSerialPortList_click();
        }

        public void SerialPortSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tmp = (string)SerialPortSelection.SelectedValue;
            //Plugin._serialPort[indexOfSelectedPedal_u].PortName = tmp;


            //try 
            //{
            //    TextBox_debugOutput.Text = "Debug: " + Plugin.Settings.selectedComPortNames[indexOfSelectedPedal_u];
            //}
            //catch (Exception caughtEx)
            //{
            //    string errorMessage = caughtEx.Message;
            //    TextBox_debugOutput.Text = errorMessage;
            //}

            try
            {
                Plugin.Settings.selectedComPortNames[indexOfSelectedPedal_u] = tmp;
                Plugin._serialPort[indexOfSelectedPedal_u].PortName = tmp;

                TextBox_debugOutput.Text = "COM port selected: " + Plugin.Settings.selectedComPortNames[indexOfSelectedPedal_u];
            }
            catch (Exception caughtEx)
            {
                string errorMessage = caughtEx.Message;
                TextBox_debugOutput.Text = errorMessage;
            }



        }
        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Title = "Datei auswählen";
                openFileDialog.Filter = "Configdateien (*.json)|*.json";
                string currentDirectory = Directory.GetCurrentDirectory();
                openFileDialog.InitialDirectory = currentDirectory + "\\PluginsData\\Common";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    string filePath = openFileDialog.FileName;
                    string text1 = System.IO.File.ReadAllText(filePath);
                    string content = (string)openFileDialog.FileName;
                    TextBox_debugOutput.Text = content;
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(DAP_config_st));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(text1));
                    dap_config_st[indexOfSelectedPedal_u] = (DAP_config_st)deserializer.ReadObject(ms);
                    //TextBox_debugOutput.Text = "Config loaded!";
                    //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.Text;
                    //TextBox_debugOutput.Text += "    ";
                    //TextBox_debugOutput.Text += ComboBox_JsonFileSelected.SelectedIndex;
                    updateTheGuiFromConfig();
                    TextBox_debugOutput.Text = "Config new imported!";
                    TextBox2.Text = "Open "+openFileDialog.FileName;
                }
            }

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveFileDialog.Title = "Datei speichern";
                saveFileDialog.Filter = "Textdateien (*.json)|*.json";
                string currentDirectory = Directory.GetCurrentDirectory();
                saveFileDialog.InitialDirectory = currentDirectory + "\\PluginsData\\Common";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                     string fileName = saveFileDialog.FileName;
                

                this.dap_config_st[indexOfSelectedPedal_u].payloadHeader_.version = (byte)pedalConfigPayload_version;

                // https://stackoverflow.com/questions/3275863/does-net-4-have-a-built-in-json-serializer-deserializer
                // https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data?redirectedfrom=MSDN
                var stream1 = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof(DAP_config_st));
                ser.WriteObject(stream1, dap_config_st[indexOfSelectedPedal_u]);

                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                string jsonString = sr.ReadToEnd();

                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }


                System.IO.File.WriteAllText(fileName, jsonString);
                TextBox_debugOutput.Text = "Config new exported!";
                TextBox2.Text = "Save " + saveFileDialog.FileName;
                }
            }
        }

        private void DisconnectToPedal_click(object sender, RoutedEventArgs e)
        {
            if (ConnectToPedal.IsChecked == true)
            {
                Plugin._serialPort[indexOfSelectedPedal_u].Close();
                ConnectToPedal.IsChecked = false;
                TextBox_debugOutput.Text = "Serialport close";
            }           
            else
            {
                ConnectToPedal.IsChecked = false;
                Plugin._serialPort[indexOfSelectedPedal_u].Close();
                TextBox_debugOutput.Text = "Not Checked Serialport close";
            }

        }

        private void Simulate_ABS_check_Checked(object sender, RoutedEventArgs e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_trigger = 1;
            TextBox_debugOutput.Text = "simulateABS: on";
            rect_SABS.Opacity = 1;
            rect_SABS_Control.Opacity = 1;
            text_SABS.Opacity = 1;

        }
        private void Simulate_ABS_check_Unchecked(object sender, RoutedEventArgs e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_trigger = 0;
            TextBox_debugOutput.Text = "simulateABS: off";
            rect_SABS.Opacity = 0;
            rect_SABS_Control.Opacity = 0;
            text_SABS.Opacity = 0;

        }

        private void ABS_SLIDER_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value = Convert.ToByte(e.NewValue);
            TextBox_debugOutput.Text = "ABS trigger value:"+ dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value;
        }


        //dragable control rect.

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var rectangle = sender as Rectangle;
            offset = e.GetPosition(rectangle);
            rectangle.CaptureMouse();
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var rectangle = sender as Rectangle;
                //double x = e.GetPosition(canvas).X - offset.X;
                double y = e.GetPosition(canvas).Y - offset.Y;

                // Ensure the rectangle stays within the canvas
                //x = Math.Max(0, Math.Min(x, canvas.ActualWidth - rectangle.ActualWidth));
                y = Math.Max(-1*rectangle.Height/2, Math.Min(y, canvas.Height - rectangle.Height/2));

                //Canvas.SetLeft(rectangle, x);
                Canvas.SetTop(rectangle, y);
                double y_actual = canvas.Height - y -rectangle.Height/2;
                if (rectangle.Name == "rect0")
                {
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p000 = Convert.ToByte(y_actual);
                    text_point_pos.Text = "Travel:0%";
                    text_point_pos.Text += "\nForce: "+(int)y_actual+"%";
                }
                if (rectangle.Name == "rect1")
                {

                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p020 = Convert.ToByte(y_actual);
                    text_point_pos.Text = "Travel:20%";
                    text_point_pos.Text += "\nForce: " + (int)y_actual + "%";
                }
                if (rectangle.Name == "rect2")
                {
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p040 = Convert.ToByte(y_actual);
                    text_point_pos.Text = "Travel:40%";
                    text_point_pos.Text += "\nForce: " + (int)y_actual + "%";
                }
                if (rectangle.Name == "rect3")
                {
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p060 = Convert.ToByte(y_actual);
                    text_point_pos.Text = "Travel:60%";
                    text_point_pos.Text += "\nForce: " + (int)y_actual + "%";
                }
                if (rectangle.Name == "rect4")
                {
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p080 = Convert.ToByte(y_actual);
                    text_point_pos.Text = "Travel:80%";
                    text_point_pos.Text += "\nForce: " + (int)y_actual + "%";
                }
                if (rectangle.Name == "rect5")
                {
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.relativeForce_p100 = Convert.ToByte(y_actual);
                    text_point_pos.Text = "Travel:100%";
                    text_point_pos.Text += "\nForce: " + (int)y_actual + "%";
                }
                text_point_pos.Opacity = 1;

                Update_BrakeForceCurve();



                // Update the position in the dictionary
                //rectanglePositions[rectangle.Name] = new Point(x, y);
            }
        }

        private void Rectangle_MouseMove_H(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var rectangle = sender as Rectangle;
                double x = e.GetPosition(canvas_horz_slider).X - offset.X;
                //double y = e.GetPosition(canvas).Y - offset.Y;

                // Ensure the rectangle stays within the canvas

                double min_posiiton = Canvas.GetLeft(rect6)+rectangle.ActualWidth/2;
                double max_position = Canvas.GetLeft(rect7)-rectangle.ActualWidth/2;
                double dx = 100/(canvas_horz_slider.Width - 10);
                if (rectangle.Name == "rect6")
                {
                    x = Math.Max(-1*rectangle.ActualWidth/2, Math.Min(x, max_position));
                    double actual_x = (x-5) * dx;
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition = Convert.ToByte(actual_x);

                    if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition > dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition)
                    {
                        dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;
                        //PedalMinPos_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
                    }
                    TextBox_debugOutput.Text = "Pedal min position:" + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
                    Canvas.SetLeft(text_min_pos, x-text_min_pos.Width/2);
                    Canvas.SetTop(text_min_pos, canvas_horz_slider.Height-10);
                }
                if (rectangle.Name == "rect7")
                {
                    x = Math.Max(min_posiiton, Math.Min(x, canvas_horz_slider.ActualWidth - rectangle.ActualWidth));
                    double actual_x = (x - 5) * dx;
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition = Convert.ToByte(actual_x);

                    if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition < dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition)
                    {
                        dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
                        //PedalMaxPos_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;
                    }
                    TextBox_debugOutput.Text = "Pedal max position:" + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;
                    Canvas.SetLeft(text_max_pos, x - text_max_pos.Width / 2);
                    Canvas.SetTop(text_max_pos, canvas_horz_slider.Height - 10);
                }
                text_min_pos.Text = "Min Pos: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition+"%";
                text_max_pos.Text = "Max Pos: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition+"%";

                //y = Math.Max(-1 * rectangle.ActualHeight / 2, Math.Min(y, canvas.ActualHeight - rectangle.ActualHeight / 2));

                Canvas.SetLeft(rectangle, x);

            }
        }
        private void Rectangle_MouseMove_V(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var rectangle = sender as Rectangle;
                double y = e.GetPosition(canvas_vert_slider).Y - offset.Y;
                //double y = e.GetPosition(canvas).Y - offset.Y;

                // Ensure the rectangle stays within the canvas

                double min_position =  Canvas.GetTop(rect8) - rectangle.Height / 2;
                double max_position = Canvas.GetTop(rect9) + rectangle.Height / 2;
                double dy = 250 / (canvas_vert_slider.Height);
                if (rectangle.Name == "rect8")
                {
                    y = Math.Max(max_position, Math.Min(y, canvas_vert_slider.Height + rectangle.Height / 2));
                    
                    double actual_y = (canvas_vert_slider.Height- y-rectangle.Height/2)  * dy;
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce = Convert.ToByte(actual_y);

                    if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce > dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce)
                    {
                        dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce;
                        //PedalMinForce_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce;
                    }
                    
                    //TextBox_debugOutput.Text = "Pedal min position:" + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalStartPosition;
                    Canvas.SetLeft(text_min_force, 12+rect8.Width+3);
                    Canvas.SetTop(text_min_force, Canvas.GetTop(rect8) +3);
                }
                if (rectangle.Name == "rect9")
                {
                    y = Math.Max(-1 * rectangle.Height / 2, Math.Min(y, min_position ));
                    
                    double actual_y = (canvas_vert_slider.Height - y - rectangle.Height / 2) * dy;
                    dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce = Convert.ToByte(actual_y);
                    if (dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce < dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce)
                    {
                        dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce;
                        //PedalMaxForce_Slider.Value = dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce;
                    }
                    
                    //TextBox_debugOutput.Text = "Pedal max position:" + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.pedalEndPosition;
                    Canvas.SetLeft(text_max_force, 12 + rect9.Width+3);
                    Canvas.SetTop(text_max_force, Canvas.GetTop(rect9) - 6);
                    
                    
                }
                text_min_force.Text = "Preload:  " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.preloadForce + "Kg";
                text_max_force.Text = "Max Force: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.maxForce + "Kg";
                
                //y = Math.Max(-1 * rectangle.ActualHeight / 2, Math.Min(y, canvas.ActualHeight - rectangle.ActualHeight / 2));

                Canvas.SetTop(rectangle, y);

            }
        }

        private void Rectangle_MouseMove_ABS(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var rectangle = sender as Rectangle;
                double x = e.GetPosition(canvas).X - offset.X;
                //double y = e.GetPosition(canvas).Y - offset.Y;

                // Ensure the rectangle stays within the canvas
                double dx = canvas.Width / 100;
                double min_posiiton = 50 * dx;
                double max_position = 95 * dx;
                //double dx = 100 / (canvas_horz_slider.Width - 10);
                x = Math.Max(min_posiiton, Math.Min(x, max_position));
                Canvas.SetLeft(rect_SABS, x);
                rect_SABS.Width = canvas.Width - x;
                double actual_x = x / dx;
                dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value = Convert.ToByte(actual_x);
                TextBox_debugOutput.Text = "ABS trigger value: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value+"%";
                text_SABS.Text = "ABS trigger value: " + dap_config_st[indexOfSelectedPedal_u].payloadPedalConfig_.Simulate_ABS_value + "%";
                Canvas.SetLeft(text_SABS, x + rect_SABS_Control.Width);
                Canvas.SetLeft(rectangle, x);

            }
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                var rectangle = sender as Rectangle;
                isDragging = false;
                rectangle.ReleaseMouseCapture();
                text_point_pos.Opacity=0;
            }
        }

        private void Debug_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            text_debug_abs_trigger.Opacity = 1;
            text_debug_max_force.Opacity = 1;
            text_debug_max_pos.Opacity = 1;
            text_debug_min_force.Opacity = 1;
            text_debug_min_pos.Opacity = 1;
            PedalMaxForce_Slider.Opacity = 1;
            PedalMaxPos_Slider.Opacity = 1;
            PedalMinForce_Slider.Opacity = 1;
            PedalMinPos_Slider.Opacity = 1;
            Simulate_ABS_slider.Opacity =1;
        }
        private void Debug_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            text_debug_abs_trigger.Opacity = 0;
            text_debug_max_force.Opacity = 0;
            text_debug_max_pos.Opacity = 0;
            text_debug_min_force.Opacity = 0;
            text_debug_min_pos.Opacity = 0;
            PedalMaxForce_Slider.Opacity = 0;
            PedalMaxPos_Slider.Opacity = 0;
            PedalMinForce_Slider.Opacity = 0;
            PedalMinPos_Slider.Opacity = 0;
            Simulate_ABS_slider.Opacity = 0;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        /*
private void GetRectanglePositions()
{
   foreach (var kvp in rectanglePositions)
   {
       Console.WriteLine($"{kvp.Key}: X={kvp.Value.X}, Y={kvp.Value.Y}");
   }
}
*/

    }
    
}
