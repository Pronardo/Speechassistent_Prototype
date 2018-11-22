﻿using AAA_Speech_Proto.Speech2Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
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

namespace AAA_Speech_Proto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecon speechRecon = new VeryFirstRecon();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRecognize_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Button clicked");
            speechRecon.StartRecognize(SpeechResponse); //SpeechRecognized is the response method
        }

        private void SpeechResponse(object sender, MySpeechEventArgs e)
        {
            Console.Write("Retrieved Response - interpreted data");
            lblText.Content = e.Text; //Write response data in label
            WPFSpeechProcessor processor = new WPFSpeechProcessor();
            processor.Init(this);
            processor.Process(e.Text); //-> DoProcess is internal & concrete
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = 100;
                synthesizer.Rate = -2;
            synthesizer.Speak("Hello User");

        }
    }
}
