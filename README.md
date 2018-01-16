# DomoticzToRouterSmsBot

__**Under construction**__

Bot who react to SMS on TP-Link router and control Domoticz

Now only Toggle switch to ON or OFF is supported

#### SMS Sample 
```
switch LightOrSwitchName to ON/OFF
```

#### Enviroment variables
* **AllowedNumbers** - Phone numbers allowed to control domoticz
* **DomoticzUri** - URL to domoticz server
* **TpLinkUri** - TP-LINK router URL
* **TpLinkUserName** - TP-LINK router username
* **TpLinkPassword** - TP-LINK router user password

[How to setup env varibale on raspberry pi](https://raspberrypi.stackexchange.com/questions/62548/setting-environment-variable-for-service)

#### DotNet instalation on raspberry
[How to](https://blogs.msdn.microsoft.com/david/2017/07/20/setting_up_raspian_and_dotnet_core_2_0_on_a_raspberry_pi/)
* Run __sudo apt-get install curl libunwind8 gettext__. This will use the apt-get package manager to install three prerequiste packages.
* Run __curl -sSL -o dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz__ to download the latest .NET Core Runtime for ARM32. This is refereed to as armhf on the Daily Builds page.
* Run __sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet__ to create a destination folder and extract the downloaded package into it.
* Run __sudo ln -s /opt/dotnet/dotnet /usr/local/bin`__ to set up a symbolic link...a shortcut to you Windows folks 😉 to the dotnet executable.
* Test the installation by typing __dotnet --help__.

##### Instaling bot
```
cd ~
git clone https://github.com/McMlok/DomoticzToRouterSmsBot.git
cd ~/DomoticzToRouterSmsBot
dotner run
```
