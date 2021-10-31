using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace Rokit_WakeUp
{
    class Program
    {
        private static readonly double MAX_VOLUME = 80;
        private static readonly int PLAY_TIME = 6;
        private static readonly string FILE_NAME = "C:\\Program Files (x86)\\RokitWakeUp\\MyFile.wav";
        private static readonly double HZ = 10;
        private static double _currentVolume = 0;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            WakeUp();
        }

        static void WakeUp()
        {
            SoundPlayer player = new SoundPlayer();
            CoreAudioController audioController = new CoreAudioController();
            CoreAudioDevice cad = audioController.DefaultPlaybackDevice;
            if (!cad.FullName.ToLower().Contains("scarlett") && !cad.FullName.ToLower().Contains("speakers"))
            {
                //switch audio device onto scarlett
                List<CoreAudioDevice> cads = audioController
                    .GetDevices()
                    .Where(x => x.FullName.ToLower().Contains("scarlett") && x.FullName.ToLower().Contains("speakers"))
                    .ToList();
                if (cads.Count > 1)
                {
                    Console.WriteLine("Too many results for Scarlett and Speakers, unable to continue");
                    Thread.Sleep(5000);
                    System.Environment.Exit(0);
                }
                
                audioController.SetDefaultDevice(cads[0]);

            }

            cad = audioController.DefaultPlaybackDevice;
            if (cad.FullName.Contains("Scarlett"))
            {
                _currentVolume = cad.Volume;
                Console.WriteLine($"Current volume: {_currentVolume}");
                Console.WriteLine($"Creating sound file with these parameters");
                Console.WriteLine($"Play time:{PLAY_TIME}");
                Console.WriteLine($"File name:{FILE_NAME}");
                Console.WriteLine($"Hz:{HZ}");
                CreateWAV.CreateWavFile(FILE_NAME, PLAY_TIME, HZ);
                Console.WriteLine($"Setting volume to {MAX_VOLUME}");
                cad.Volume = MAX_VOLUME;
                Console.WriteLine($"Playing {FILE_NAME}");
                player.SoundLocation = FILE_NAME;
                player.PlaySync();
                Console.WriteLine("Done playing");

                Console.WriteLine($"Setting volume back to {_currentVolume}");
                cad.Volume = _currentVolume;
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("Default device is not Scarlett");
                Thread.Sleep(5000);
                System.Environment.Exit(0);
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CoreAudioDevice cad = new CoreAudioController().DefaultPlaybackDevice;
            cad.Volume = _currentVolume;
        }
    }
}