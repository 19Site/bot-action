using System.Runtime.InteropServices;
using System.Threading.Tasks;

/**
 * namespace
 */
namespace BotAction {

    /**
     * mouse class
     */
    internal class Mouse {

        // mouse action
        public const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        public const uint MOUSEEVENTF_LEFTUP = 0x04;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [StructLayout(LayoutKind.Sequential)]
        public struct Point {

            // position X
            public int X { get; set; }

            // position Y
            public int Y { get; set; }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        /**
         * get cursor position
         */
        public static Point GetCursorPosition() {

            // create point
            Mouse.Point point = new Mouse.Point();

            // get cursor position
            Mouse.GetCursorPos(out point);

            // return result
            return point;
        }

        /**
         * move to (position)
         */
        public static void MoveTo(Point point, int sleep = 20) {

            // set cursor position
            SetCursorPos(point.X, point.Y);

            // wait
            Task.Delay(sleep);
        }

        /**
         * do left click
         */
        public static void LeftClick(uint dx = 0, uint dy = 0, uint cButtons = 0, uint dwExtraInfo = 0, int sleep = 20) {

            // mouse down
            mouse_event(MOUSEEVENTF_LEFTDOWN, dx, dy, cButtons, dwExtraInfo);

            // wait
            Task.Delay(20);

            // mouse up
            mouse_event(MOUSEEVENTF_LEFTUP, dx, dy, cButtons, dwExtraInfo);

            // wait
            Task.Delay(sleep);
        }

        /**
         * do right click
         */
        public static void RightClick(uint dx = 0, uint dy = 0, uint cButtons = 0, uint dwExtraInfo = 0, int sleep = 20) {

            // mouse down
            mouse_event(MOUSEEVENTF_RIGHTDOWN, dx, dy, cButtons, dwExtraInfo);

            // wait
            Task.Delay(20);

            // mouse up
            mouse_event(MOUSEEVENTF_RIGHTUP, dx, dy, cButtons, dwExtraInfo);

            // wait
            Task.Delay(sleep);
        }
    }
}
