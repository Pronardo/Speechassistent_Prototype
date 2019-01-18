﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AAA_Speech_Proto.Text2Speech
{
    class WPFMicrosoftSynthesizer : SpeechSynth
    {
        //WPF Functionality must be outsourced to SynthProcessor resp. WPFSynthProcessor
        public int DebounceTimer { get; set; } = 300; //milliseconds
        private Dictionary<string, string> SpeechMappings = new Dictionary<string, string>();

        public WPFMicrosoftSynthesizer(UIElement element)
        {
            Init(element);
        }
        public void Init(UIElement element)
        {
            SeedMappings();
            ObserveMouse(element);
        }
        private void SeedMappings()
        {
            SpeechMappings.Add("Button", "Content");
            SpeechMappings.Add("Label", "Content");
        }
        public void SynthesizeInput(string input)
        {
            Console.WriteLine($"synthesize input {input}");
            using (var synthesizer = new SpeechSynthesizer())
            {
                synthesizer.Volume = 100;
                synthesizer.Rate = -2;
                synthesizer.Speak(input);
            }
        }

        public void ObserveMouse(UIElement element)
        {
            var mouseMove = Observable.FromEventPattern<MouseEventArgs>(element, "MouseMove")
                .Sample(TimeSpan.FromMilliseconds(DebounceTimer))
               .Subscribe(
                    args => Process(args.Sender as FrameworkElement),
                    error => LogError(error)
            );
        }

        private void LogError(Exception error)
        {
            Console.WriteLine("An error occured while processing", error.Message);
        }

        private void Process(FrameworkElement element)
        {
            string elename = element.Name;
            if(SpeechMappings.ContainsKey(elename))
            {
                Console.WriteLine($"Evaluate speech output for {elename}");
                var prop=SpeechMappings[elename];
                Type type = element.GetType();
                string propertyvalue = type.GetProperty(prop).GetValue(type).ToString();
                SynthesizeInput(propertyvalue);
            }
            else
            {
                Console.WriteLine($"Element {elename} is not mapped with speech settings");
            }
        }
    }
}