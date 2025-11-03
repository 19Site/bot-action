using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/**
 * namespace
 */
namespace BotAction {

    /**
     * keyboard class
     */
    internal class Keyboard {

        // function keys
        public const byte VK_SHIFT = 0x10;
        public const byte VK_LSHIFT = 0xA0;
        public const byte VK_RSHIFT = 0xA1;
        public const byte VK_CONTROL = 0x11;
        public const byte VK_RETURN = 0x0D;
        public const byte VK_OEM_3 = 0xC0; // for key "`"

        public const byte VK_LEFT = 0x25;
        public const byte VK_UP = 0x26;
        public const byte VK_RIGHT = 0x27;
        public const byte VK_DOWN = 0x28;
        public const byte VK_SPACE = 0x20;

        // actions
        public const uint KEYEVENTF_KEYDOWN = 0x0000;
        public const uint KEYEVENTF_KEYUP = 0x0002;


        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        /**
         * key in
         */
        public static void KeyIn(byte keyCode, bool isCtrl = false, bool isShift = false, int sleep = 20) {

            // is ctrl
            if (isCtrl) {

                // ctrl key down
                keybd_event(Keyboard.VK_CONTROL, 0, Keyboard.KEYEVENTF_KEYDOWN, IntPtr.Zero);

                // wait
                Task.Delay(20);
            }

            // is shift
            if (isShift) {

                // shift key down
                keybd_event(Keyboard.VK_LSHIFT, 0, Keyboard.KEYEVENTF_KEYDOWN, IntPtr.Zero);

                // wait
                Task.Delay(20);
            }

            // key down
            keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);

            // wait
            Task.Delay(20);

            // key up
            keybd_event(keyCode, 0, KEYEVENTF_KEYUP, IntPtr.Zero);

            // is shift
            if (isShift) {

                // wait
                Task.Delay(20);

                // shift key up
                keybd_event(Keyboard.VK_LSHIFT, 0, Keyboard.KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            // is ctrl
            if (isCtrl) {

                // wait
                Task.Delay(20);

                // ctrl up
                keybd_event(Keyboard.VK_CONTROL, 0, Keyboard.KEYEVENTF_KEYUP, IntPtr.Zero);
            }

            // wait
            Task.Delay(sleep);
        }
    }
}
