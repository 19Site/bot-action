using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows;

/**
 * namespace
 */
namespace BotAction {

    /**
     * program
     */
    internal class Program {

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /**
         * arg result
         */
        struct ArgResult {

            // name
            public string Name;

            // value
            public string? Value;

            // is exists
            public bool IsExists;
        }

        /**
         * get value
         */
        private static ArgResult GetArgsValue(string[] data, string name) {

            // result
            ArgResult result = new ArgResult {

                // name
                Name = name,

                // value
                Value = null,

                // is exists
                IsExists = false,
            };

            // no data
            if (data.Length == 0) {

                // return result
                return result;
            }

            // name position
            int namePosition = Array.IndexOf(data, name);

            // not found
            if (namePosition == -1) {

                // return result
                return result;
            }

            // found
            else {

                // is exists
                result.IsExists = true;

                // next position
                int nextPosition = namePosition + 1;

                // overflow
                if (nextPosition >= data.Length) {

                    // return result
                    return result;
                }

                // have value
                result.Value = data[nextPosition];

                // return result
                return result;
            }
        }

        /**
         * is numeric
         */
        private static bool IsNumeric(string value = "") {

            // check by regex
            return Regex.IsMatch(value, @"^-?[0-9]+$");
        }

        /**
         * do task
         */
        private static void DoTask(string[] args) {

            // new json object
            JsonObject result = new JsonObject();

            // default is ok
            result["ok"] = true;

            // workflow
            try {

                // no command line args
                if (args.Length == 0) {

                    // throw error
                    throw new Exception("use --help for usage");
                }

                // base command
                string baseCommand = args[0];

                // command: --help
                if (baseCommand == "--help") {

                    Console.WriteLine("Usage:");

                    Console.WriteLine("  bot-action.exe [command] [...options]");

                    Console.WriteLine();

                    Console.WriteLine("command:");

                    Console.WriteLine("  --get-cursor-position      Get mouse cursor current position");

                    Console.WriteLine("  --clipboard                Clipboard action");

                    Console.WriteLine("    --read                   Read clipboard text");

                    Console.WriteLine("    --text <Text>            (String) Text to by copy to clipboard");

                    Console.WriteLine("    --file <File Path>       (String) File to by copy to clipboard");

                    Console.WriteLine("  --mouse <Mouse Key>        (Enum[left,right,l,r]) Mouse click");

                    Console.WriteLine("    --x <Position X>         (Integer) Position x (in pixel) mouse to move");

                    Console.WriteLine("    --y <Position Y>         (Integer) Position y (in pixel) mouse to move");

                    Console.WriteLine("    --times <Times>          (Integer) How many <Times> want to click");

                    Console.WriteLine("  --key <Key>                (Char) Key press <Key>");

                    Console.WriteLine("    --shift                  With shift key pressed");

                    Console.WriteLine("    --ctrl                   With ctrl key pressed");

                    Console.WriteLine("    --times <Times>          (Integer) How many <Times> want to press");

                    Console.WriteLine("  --text <Text>              (String) Input <Text> string");
                }

                // command: --get-cursor-position
                else if (baseCommand == "--get-cursor-position") {

                    // get cursor position
                    Mouse.Point pos = Mouse.GetCursorPosition();

                    // x
                    result["x"] = pos.X;

                    // y
                    result["y"] = pos.Y;
                }

                // command: --key
                else if (baseCommand == "--key") {

                    // args key
                    ArgResult argKey = GetArgsValue(args, "--key");

                    // args shift
                    ArgResult argShift = GetArgsValue(args, "--shift");

                    // args ctrl
                    ArgResult argCtrl = GetArgsValue(args, "--ctrl");

                    // args times
                    ArgResult argTimes = GetArgsValue(args, "--times");

                    // is exists (key)
                    if (argKey.IsExists) {

                        // times
                        int times = 1;

                        // is exists (times)
                        if (argTimes.IsExists && argTimes.Value != null && IsNumeric(argTimes.Value)) {

                            // times
                            times = int.Parse(argTimes.Value);
                        }

                        // have value
                        if (argKey.Value != null) {

                            // is vk code found
                            bool isVkCodeFound = false;

                            // vk code
                            byte vkCode = 0;

                            // single key
                            if (argKey.Value.Length == 1) {

                                // get vk
                                short vkRaw = Keyboard.VkKeyScan(argKey.Value[0]);

                                // vk code not found
                                if (vkRaw == -1) {

                                    // throw error
                                    throw new Exception($"Cannot map key '{argKey.Value}' to virtual key code.");
                                }

                                // vk raw value to byte
                                vkCode = (byte)(vkRaw & 0xFF);

                                // is found
                                isVkCodeFound = true;
                            }

                            // {enter} key
                            else if (argKey.Value == "enter") {

                                // return key
                                vkCode = Keyboard.VK_RETURN;

                                // is found
                                isVkCodeFound = true;
                            }

                            // have vk code
                            if (isVkCodeFound) {

                                // do times
                                for (int i = 0; i < times; ++i) {

                                    // key press
                                    Keyboard.KeyIn(vkCode, isShift: argShift.IsExists, isCtrl: argCtrl.IsExists);
                                }
                            }
                        }
                    }
                }

                // command: --text
                else if (baseCommand == "--text") {

                    // args text
                    ArgResult arg = GetArgsValue(args, "--text");

                    // missing text args
                    if (!arg.IsExists || arg.Value == null) {

                        // throw error
                        throw new Exception("invalid text input");
                    }

                    // input "text" directly to console
                    WindowsInput.Simulate.Events().Click(arg.Value).Invoke();
                }

                // command: --clipboard
                else if (baseCommand == "--clipboard") {

                    // args read
                    ArgResult argRead = GetArgsValue(args, "--read");

                    // args text
                    ArgResult argText = GetArgsValue(args, "--text");

                    // args file
                    ArgResult argFile = GetArgsValue(args, "--file");

                    // args base64
                    ArgResult argBase64 = GetArgsValue(args, "--base64");

                    // have action
                    bool haveAction = false;

                    // read exists
                    if (argRead.IsExists) {

                        // read text from clipboard
                        string text = Clipboard.GetText();

                        // set to result
                        result["text"] = text;

                        // have action
                        haveAction = true;
                    }

                    // text exists
                    else if (argText.IsExists && argText.Value != null) {

                        // text (to be copy clipboard)
                        string text = argText.Value;

                        // base64 exists
                        if (argBase64.IsExists) {

                            // decode base64
                            byte[] bytes = Convert.FromBase64String(text);

                            // assign to text
                            text = Encoding.UTF8.GetString(bytes);
                        }

                        // copy text to clipboard
                        Clipboard.SetText(text);

                        // have action
                        haveAction = true;
                    }

                    // file exists
                    else if (argFile.IsExists && argFile.Value != null) {

                        // file (to be copy clipboard)
                        string file = argFile.Value;

                        // base64 exists
                        if (argBase64.IsExists) {

                            // decode base64
                            byte[] bytes = Convert.FromBase64String(file);

                            // assign to text
                            file = Encoding.UTF8.GetString(bytes);
                        }

                        // create string collection (file list)
                        StringCollection sc = new StringCollection();

                        // add image file path
                        sc.Add(file);

                        // add to clipboard
                        Clipboard.SetFileDropList(sc);

                        // have action
                        haveAction = true;
                    }

                    // no actions
                    if (!haveAction) {

                        // throw
                        throw new Exception("invalid clipboard action");
                    }
                }

                // command: --mouse
                else if (baseCommand == "--mouse") {

                    { // mouse move

                        // args x
                        ArgResult argX = GetArgsValue(args, "--x");

                        // args y
                        ArgResult argY = GetArgsValue(args, "--y");

                        // have X
                        bool haveX = false;

                        // have Y
                        bool haveY = false;

                        // have arg x
                        if (argX.IsExists && argX.Value != null && IsNumeric(argX.Value)) {

                            // have x
                            haveX = true;
                        }

                        // have arg y
                        if (argY.IsExists && argY.Value != null && IsNumeric(argY.Value)) {

                            // have y
                            haveY = true;
                        }

                        // any options is missing
                        if (haveX != haveY) {

                            // throw error
                            throw new Exception("missing position " + (haveY ? "x" : "y"));
                        }

                        // have X and Y (mouse move position)
                        if (haveX && haveY) {

                            // move to position
                            Mouse.MoveTo(new Mouse.Point {

                                // x
                                X = int.Parse(argX.Value!),

                                // y
                                Y = int.Parse(argY.Value!),
                            });
                        }
                    }

                    { // mouse click

                        // args mouse
                        ArgResult argMouse = GetArgsValue(args, "--mouse");

                        // is exists
                        if (argMouse.IsExists) {

                            // is [left, right, l, r]
                            if (argMouse.Value != null && Regex.IsMatch(argMouse.Value, @"^(left|right|l|r)$")) {

                                // times
                                int times = 1;

                                // args times
                                ArgResult argTimes = GetArgsValue(args, "--times");

                                // have numeric value (click times)
                                if (argTimes.Value != null && IsNumeric(argTimes.Value)) {

                                    // times
                                    times = int.Parse(argTimes.Value);
                                }

                                // do times
                                for (int i = 0; i < times; ++i) {

                                    // is "left
                                    if (Regex.IsMatch(argMouse.Value, @"^(left|l)$")) {

                                        // left click
                                        Mouse.LeftClick();
                                    }

                                    // is "right"
                                    else if (Regex.IsMatch(argMouse.Value, @"^(right|r)$")) {

                                        // right click
                                        Mouse.RightClick();
                                    }
                                }
                            }
                        }
                    }
                }

                // base ommand not found
                else {

                    // throw error
                    throw new Exception($"unknown command: '{baseCommand}'");
                }

                // print help
                Console.WriteLine(result.ToJsonString());
            }

            // error
            catch (Exception ex) {

                // failure
                result["ok"] = false;

                // error
                result["error"] = ex.Message;

                // print help
                Console.WriteLine(result.ToJsonString());
            }
        }

        /**
         * main
         */
        [STAThread]
        static void Main(string[] args) {

            // do task
            DoTask(args);
        }
    }
}
