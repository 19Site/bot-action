# Bot Action

C# .NET console command line for simulate mouse and keyboard actions.

# Usage

```sh
c:\>./bot-action.exe --help

Usage:
    bot-action.exe [command] [...options]

command:
    --get-cursor-position        Get mouse cursor current position
    --clipboard                  Clipboard action
        --read                   Read clipboard text
        --text <Text>            (String) Text to by copy to clipboard
        --file <File Path>       (String) File to by copy to clipboard
    --mouse <Mouse Key>          (Enum[left,right,l,r]) Mouse click
        --x <Position X>         (Integer) Position x (in pixel) mouse to move
        --y <Position Y>         (Integer) Position y (in pixel) mouse to move
        --times <Times>          (Integer) How many <Times> want to click
    --key <Key>                  (Char) Key press <Key>
        --shift                  With shift key pressed
        --ctrl                   With ctrl key pressed
        --times <Times>          (Integer) How many <Times> want to press
    --text <Text>                (String) Input <Text> string
```