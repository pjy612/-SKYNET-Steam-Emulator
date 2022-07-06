﻿using NAudio.Wave;
using SKYNET.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class AudioManager
    {
        private static WaveInEvent waveSource;
        private static List<byte> Buffer;
        private static List<byte> AvailableBuffer;

        public static void Initialize()
        {
            Buffer = new List<byte>();
            AvailableBuffer = new List<byte>();

            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(4800, 1);
            waveSource.DataAvailable += OnDataAvailable;
        }

        private static void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            MutexHelper.Wait("Buffer", delegate
            {
                Buffer.AddRange(e.Buffer);
            });
        }

        internal static void StartVoiceRecording()
        {
            waveSource.StartRecording();
        }

        internal static void StopVoiceRecording()
        {
            waveSource.StopRecording();
        }

        internal static void GetAvailableVoice(out uint compressed, out uint unCompressed)
        {
            compressed = 0;
            unCompressed = 0;

            MutexHelper.Wait("Buffer", delegate
            {
                AvailableBuffer.AddRange(Buffer);
            });

            MutexHelper.Wait("Buffer", delegate
            {
                Buffer.Clear();
            });

            compressed = (uint)AvailableBuffer.Count();
            unCompressed = (uint)AvailableBuffer.Count();

        }

        internal static void GetVoice(out byte[] buffer)
        {
            byte[] bytes = default; 

            MutexHelper.Wait("Buffer", delegate
            {
                bytes = AvailableBuffer.ToArray();
            });

            buffer = bytes != null ? bytes :new byte[0] ;

            MutexHelper.Wait("Buffer", delegate
            {
                AvailableBuffer.Clear();
            });
        }
    }
}
