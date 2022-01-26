using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using WindowsInput.Native;
using WindowsInput;

namespace AutoWriter
{
    
    public class MessageSender
    {
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        InputSimulator sim = new InputSimulator();

        public void SendMessage(string message, Process[] processes)
        {

            message = message.ToUpper();

            if (processes.Length == 0) 
                throw new ArgumentException("Wrong processes");
            else
            {
                // Console.WriteLine(processes.Length);
                Process process = processes[0];
                //foreach(var process in processes)
                //{

                for (int i = 0; i < message.Length; i++)
                {
                    try
                    {
                            SetForegroundWindow(process.MainWindowHandle);
                            sim.Keyboard.KeyPress((VirtualKeyCode)FindValue(message[i]));
                        // Console.WriteLine("MEssage posted!");
                    }
                    catch (Exception e)
                    {
                        System.Windows.MessageBox.Show($"Symbol {message[i]} not finded. Only english letter is allowed!\r\nSymbol skipped", "Error");
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
        }

        private int FindValue(char sym)
        {
            for (int i = 0; i < symbols.Count; i++)
            {

                if (symbols[i].symbol == sym)
                {
                    Console.WriteLine($"VALUE SENDED {sym}");
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
