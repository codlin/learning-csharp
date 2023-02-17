/// Passing arguments to a console app
WriteLine($"There are {args.Length} arguments.");

/// <h1>
/// Setting options with arguments
/// </h1>
/// We will now use these arguments to allow the user to pick a color for the background, foreground,
/// and cursor size of the output window. The cursor size can be an integer value from 1, meaning a line
/// at the bottom of the cursor cell, up to 100, meaning a percentage of the height of the cursor cell.
/// 我们现在使用这些参数来允许用户为背景、前景、和输出窗口的光标大小。游标大小可以是 1 之间的整数值，表示一行
/// 在光标单元格的底部，最多为 100，表示光标单元格高度的百分比。
if (args.Length < 3)
{
    WriteLine("You must specify two colors and cursor size, e.g.");
    WriteLine("dotnet run red yellow 50");
    return; // stop running
}
ForegroundColor = (ConsoleColor)Enum.Parse(enumType: typeof(ConsoleColor), value: args[0], ignoreCase: true);
BackgroundColor = (ConsoleColor)Enum.Parse(enumType: typeof(ConsoleColor), value: args[1], ignoreCase: true);
CursorSize = int.Parse(args[2]);
// dotnet run red yellow 50
