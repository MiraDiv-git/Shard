# Shard Engine
Lightweight 2D Engine

### Navigation
- [Quick Start](#quick-start)
- [Build](#build)

## Quick Start
1. Clone the repo
    ```sh
    git clone https://github.com/MiraDiv-git/Shard.git
    ```
2. [Build or restore solution](#build)
3. Done. Now work in `Game` project.

## Build
Shard Engine uses Cake build system.
1. Restore tools
    ```sh
    dotnet tool restore
    ```
2. Build the solution
    ```sh
    dotnet cake --runtime linux-x64
    dotnet cake --runtime win-x64
    ```
> Running `dotnet cake` without `--runtime` argument will publish for both platforms.

---

If you want to restore solution without publishing, use:
```sh
dotnet restore
```