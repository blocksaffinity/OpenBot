# OpenBot
OpenBot is an open-source Discord bot framework written in C# and built around a modular architecture.
It's designed to provide the foundations for running and expanding a Discord bot without having everything tied together in one place.

## Features
- **Modules** – Split the bot into separate components, each with its own responsibilities and lifecycle.
- **Extensions** – Load extensions separately from the core bot, allowing functionality to be added without modifying OpenBot itself.
- **Lifecycle management** – Modules and extensions have registration, startup and shutdown stages.
- **Configuration** – YAML-based configuration with configuration handling built into the framework.
- **Logging** – A centralised logging system used throughout OpenBot.
- **Discord integration** – Discord connectivity and client management handled through Discord.Net.
- **Developer API** – Interfaces, contexts and base classes for developing modules and extensions.

## Project status
OpenBot is currently in development. Features and APIs may change as the project evolves, and it isn't yet considered stable for production use.

## Built with
- C#
- .NET
- [Discord.Net](https://github.com/discord-net/Discord.Net)
- [YamlDotNet](https://github.com/aaubry/YamlDotNet)

## Documentation
Documentation is currently being worked on.

## Contributing
Suggestions, bug reports and contributions are welcome. If you have an idea for OpenBot or find an issue, feel free to open an issue on GitHub.

## License
See [LICENSE](LICENSE) for licensing information.
