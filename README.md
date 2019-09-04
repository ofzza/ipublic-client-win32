# ipublic-client-win32
[Standalone service](https://github.com/ofzza/dotnet-standalone-service) registering client's public IP changes with a [IPublic server](https://github.com/ofzza/ipublic-server) ...

## How to use

#### Configure client
```sh
> ipublic-client-win32.exe --config
```
*Prompts current configuration and allows for updates to all confguration parameters*

#### Run from CLI
```cmd
> ipublic-client-win32.exe
```

#### Install as service
```sh
> ipublic-client-win32.exe --install
```
*Will prompt for elevated privilages if needed.*

#### Uninstall service
```sh
> ipublic-client-win32.exe --uninstall
```
*Will prompt for elevated privilages if needed.*

#### Start service
```sh
> ipublic-client-win32.exe --start
```
*Will prompt for elevated privilages if needed.*

#### Stop service
```sh
> ipublic-client-win32.exe --stop
```
*Will prompt for elevated privilages if needed.*
