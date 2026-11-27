// Oops this wasn't meant to be AI but I wanted to the prompt pinned to the bottom so here we are.

using System.Text;
using OpenBot.API;
using OpenBot.API.Extensions;
using OpenBot.API.Modules;

namespace OpenBot.Modules;

internal sealed class ConsoleCommandHandlerModule : Module
{
    private readonly object _consoleLock = new();
    private readonly StringBuilder _currentInput = new();
    private readonly TextWriter _originalOut;
    private string _currentPrompt = "$ ";
    private bool _isReadingInput;

    internal ConsoleCommandHandlerModule(ModuleContext ctx) : base(ctx)
    {
        _originalOut = Console.Out;
        Console.SetOut(new PinnedPromptTextWriter(this, _originalOut));
    }

    /// <summary>
    /// Non-blocking replacement for your Python-like Input() helper.
    /// </summary>
    private async Task<string> InputAsync(string prompt = "", string start = "\n", string end = "", CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            lock (_consoleLock)
            {
                _currentPrompt = start + prompt + end;
                _currentInput.Clear();
                _isReadingInput = true;
                _originalOut.Write(_currentPrompt);
            }

            while (!ct.IsCancellationRequested)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(15); // Prevent CPU spin and allow clean cancellation
                    continue;
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                lock (_consoleLock)
                {
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        _isReadingInput = false;
                        _originalOut.WriteLine();
                        string result = _currentInput.ToString();
                        _currentInput.Clear();
                        return result;
                    }

                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (_currentInput.Length > 0)
                        {
                            _currentInput.Remove(_currentInput.Length - 1, 1);
                            _originalOut.Write("\b \b");
                        }
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        _currentInput.Append(keyInfo.KeyChar);
                        _originalOut.Write(keyInfo.KeyChar);
                    }
                }
            }

            return string.Empty;
        }, ct);
    }

    internal async Task CommandHandlerLoop(CancellationToken ct = default)
    {
        // Replaced scary while(true) with cancellation-safe loop
        while (!ct.IsCancellationRequested)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string consoleInput = await InputAsync($"$ ", start: "", ct: ct);

            if (string.IsNullOrWhiteSpace(consoleInput))
                continue;

            // Handle your OpenBot commands here
            //Console.WriteLine($"[Command] Running: {consoleInput}");
            if (consoleInput == "help")
            {
                Console.Write(
"\nOpenBot Console Commands:\n" + 
"   - help            -> Displays this help menu\n" +
"   - list modules    -> Lists all registered modules\n" +
"   - list extensions -> Lists all registered extensions\n" +
"   - stop            -> Shut down OpenBot\n\n"
                );
            }
            else if (consoleInput == "list extensions")
            {
                Context.Modules.GetOrNull<IExtensionsManagerModule>()?.GetExtensions().ToList().ForEach(ext => Console.WriteLine($"{ext.Name,-15} [Version: {ext.Metadata.version}] [API Version: {ext.Metadata.apiVersion}] [Status: {ext.Status}]"));
                Console.WriteLine("");
            }
            else if (consoleInput == "list modules")
            {
                Context.Modules.GetModules().ToList().ForEach(mod => Console.WriteLine($"{mod.Name,-15} [Status: {mod.Status}]"));
                Console.WriteLine("");
            }
            else if (consoleInput == "stop")
            {
                break;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: command {consoleInput} is not a valid command.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    /// <summary>
    /// No idea
    /// </summary>
    private sealed class PinnedPromptTextWriter : TextWriter
    {
        private readonly ConsoleCommandHandlerModule _module;
        private readonly TextWriter _inner;

        public PinnedPromptTextWriter(ConsoleCommandHandlerModule module, TextWriter inner)
        {
            _module = module;
            _inner = inner;
        }

        public override Encoding Encoding => _inner.Encoding;

        public override void WriteLine(string? value)
        {
            lock (_module._consoleLock)
            {
                if (_module._isReadingInput)
                {
                    ClearPromptLine();
                    _inner.WriteLine(value);
                    RedrawPromptLine();
                }
                else
                {
                    _inner.WriteLine(value);
                }
            }
        }

        public override void Write(string? value)
        {
            lock (_module._consoleLock)
            {
                if (_module._isReadingInput && value != null && value.Contains('\n'))
                {
                    ClearPromptLine();
                    _inner.Write(value);
                    RedrawPromptLine();
                }
                else
                {
                    _inner.Write(value);
                }
            }
        }

        private void ClearPromptLine()
        {
            int promptLength = _module._currentPrompt.TrimStart('\n', '\r').Length + _module._currentInput.Length;
            int linesOccupied = Math.Max(1, (int)Math.Ceiling((double)promptLength / Console.WindowWidth));

            for (int i = 0; i < linesOccupied; i++)
            {
                int targetRow = Math.Max(0, Console.CursorTop - i);
                Console.SetCursorPosition(0, targetRow);
                _inner.Write(new string(' ', Console.WindowWidth - 1));
            }
            Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - (linesOccupied - 1)));
        }

        private void RedrawPromptLine()
        {
            _inner.Write(_module._currentPrompt.TrimStart('\n', '\r') + _module._currentInput);
        }
    }
}
