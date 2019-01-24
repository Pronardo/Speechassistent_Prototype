﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AAA_Speech_Proto.Text2Speech
{
    class WPFMicrosoftSynthesizer : SpeechSynth
    {
        //Tasks:
        //Debounce to MouseMoveEvent
        //Compare e.Source to Mappings
        //Get Mapped Property of recieved element
        //Give String to TTS Engine
        //WPF Functionality must be outsourced to SynthProcessor resp. WPFSynthProcessor
        public int DebounceTimer { get; set; } = 300; //milliseconds
        private Dictionary<string, string> SpeechMappings = new Dictionary<string, string>();
        public UIElement ObservedElement { get; set; }
        public WPFMicrosoftSynthesizer(UIElement element)
        {
            Init(element);
        }
        public void Init(UIElement element)
        {
            SeedMappings();
            ObserveMouse(element);
            ObservedElement = element;
        }
        private void SeedMappings()
        {
            SpeechMappings.Add("Button", "Content");
            SpeechMappings.Add("Label", "Content");
            SpeechMappings.Add("TextBox", "Text");
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
            var mouseMove = Observable
                .FromEventPattern<MouseEventArgs>(element, "MouseMove")
                .Select(x => x.EventArgs.Source)
                .Sample(TimeSpan.FromMilliseconds(DebounceTimer))
               .Subscribe(
                    completed => Process(completed),
                    error => LogError(error)
            );
        }

        private void LogError(Exception error)
        {
            Console.WriteLine("An error occured while processing", error.Message);
        }

        private void Process(object element)
        {
            Type type = element.GetType();
            string eletype = type.Name;
            //eletype = "Button";
            if (SpeechMappings.ContainsKey(eletype))
            {
                Console.WriteLine($"Evaluate speech output for {eletype}");
                var prop = SpeechMappings[eletype];

                Object target = new Object();
                Application.Current.Dispatcher.InvokeAsync(
                    new Action(() => target = element)); //not working!


                PropertyInfo property = type.GetProperty(prop);
                string propertyvalue = property.GetValue(target).ToString();
                SynthesizeInput(propertyvalue);
            }
            else
            {
                Console.WriteLine($"Element {eletype} is not mapped with speech settings");
            }
        }
    }
}
