using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoWriter
{
    
    public class MessageSender
    {
        private class SymbolInVm
        {
            public char symbol { get; set; }
            public int value { get; set; }
            public SymbolInVm(char sym,int val){
                symbol = sym;
                value = val;
            }
        }
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        private List<SymbolInVm> symbols;

		const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_RETURN = 0x0D;

        private void FillSymbols()
        {
            symbols = new List<SymbolInVm>();
            char[] alph = { 'A','B','C','D', 'E', 'F', 'G', 'H' , 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P','Q','R','S','T','U','V','W','X','Y','Z' };
            int temp = 65;
            for(int i=0; i<alph.Length; i++)
            {
                symbols.Add(new SymbolInVm(alph[i], temp++));
            }
        }

        private int FindValue(char sym)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if(symbols[i].symbol == sym)
                {
                    return symbols[i].value;
                }
            }
            throw new ArgumentOutOfRangeException("Not finded");
        }

        public void SendMessage(string message, Process[] processes)
        {
            if (processes.Length == 0)
            {
                throw new ArgumentException("Wrong processes");
            }
            else
            {
                foreach(var process in processes)
                {
                    int keyCode = 0;
                    for(int i = 0; i < message.Length; i++)
                    {
                        try
                        {
                            keyCode = FindValue(message[i]);
                            PostMessage(process.MainWindowHandle, WM_KEYDOWN, keyCode, 0);
                        }
                        catch(Exception e)
                        {
                            System.Windows.MessageBox.Show($"Symbol {message[i]} not finded. Only english letter is allowed!\r\nSymbol skipped","Error");
                        }
                        
                    }
                    PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_RETURN, 0);
                }
            }
        }

        public MessageSender()
        {
            FillSymbols();
        }
    }
}
