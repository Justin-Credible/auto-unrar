# auto-unrar

This is a quick and dirty command line tool used to automatically extract a rar file given the path to a rar file OR the path to a directory containing a rar file.

This is useful for torrent clients that can execute a command after a torrent has finished downloading.

Files are extracted to the same directory that the rar is located.

## Usage

I've tested this with qBittorrent v3.3.3, but it should work with other clients as well.

1. Open the `.sln` file and compile the project
2. Copy the resulting binary `bin/debug/auto-unrar.exe` to the desired location
3. Copy the configuration file `bin/debug/auto-unrar.exe.config` to the desired location (same as the binary)
4. Edit the configuration file to point at the location of `unrar.exe` and desired log file path
5. Open qBittorrent, navigate to File > Options > Downloads
6. Check "Run external program on torrent completion"
7. Enter the quoted path to `auto-unrar.exe` with the `%F` flag:

````
"D:/auto-unrar.exe" "%F"
````