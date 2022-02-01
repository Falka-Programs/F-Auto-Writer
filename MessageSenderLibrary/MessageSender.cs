using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using WindowsInput.Native;
using WindowsInput;
using System.Threading;

namespace AutoWriter
{

    public class MessageSender
    {
        List<String> messages = new List<String>();
        int timeout = 0;
        Process[] processes;
        Thread workThread;

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        InputSimulator sim = new InputSimulator();

        private void ThreadMethod()
        {
            try
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    SendMessage(messages[i], processes);
                    Thread.Sleep(timeout * 1000);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        public void SendMessages(List<String> mes, int tim, Process[] proc)
        {
            processes = proc;
            messages = mes;
            timeout = tim;
            workThread = new Thread(new ThreadStart(ThreadMethod));
            workThread.IsBackground = true;
            workThread.Start();
        }
        public void StopSending()
        {
            workThread.Abort();
        }
        public void SendMessage(string message, Process[] proc)
        {

            message = message.ToUpper();

            if (proc.Length == 0)
                throw new ArgumentException("Wrong processes");
            else
            {
                // Console.WriteLine(processes.Length);
                Process process = proc[0];
                //foreach(var process in processes)
                //{

                for (int i = 0; i < message.Length; i++)
                {
                    try
                    {

                        if (message[i] == '\n' || message[i] == '\r') { continue; }
                        if (message[i] == ' ')
                        {
                            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                            continue;
                        }
                        SetForegroundWindow(process.MainWindowHandle);
                        sim.Keyboard.KeyPress((VirtualKeyCode)FindValue(message[i]));
                    }
                    catch (Exception err)
                    {
#if DEBUG
                        Console.WriteLine($"{err.Message}");
#endif
                    }

                }
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                //}
            }
        }

        private class SymbolInVm
        {
            public char symbol { get; set; }
            public int value { get; set; }
            public SymbolInVm(char sym, int val)
            {
                symbol = sym;
                value = val;
            }
        }



        private List<SymbolInVm> symbols;
        private void FillSymbols()
        {
            symbols = new List<SymbolInVm>();
            char[] alph = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            int temp = 65;
            for (int i = 0; i < alph.Length; i++)
            {
                symbols.Add(new SymbolInVm(alph[i], temp++));
            }
            symbols.Add(new SymbolInVm('?', 63));
        }

        private int FindValue(char sym)
        {
            for (int i = 0; i < symbols.Count; i++)
            {

                if (symbols[i].symbol == sym)
                {
                    // Console.WriteLine($"VALUE SENDED {sym}");
                    return symbols[i].value;
                }
            }
            throw new ArgumentOutOfRangeException("Not finded");
        }



        public MessageSender()
        {
            FillSymbols();
        }
    }
}
