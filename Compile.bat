@echo off
color a
title "Golden Sentry Discord IPS Compiler CLI V1.8"
mode con: cols=135 lines=57
choice /M "Golden Sentry Discord IPS ?"
If Errorlevel 2 Goto No
If Errorlevel 1 Goto Yes

:No
echo "Compilation Canceled"
exit

:Yes
echo "Are you sure you wish to Compile ? CTRL+C to quit or"
pause
echo "Setting up Directories"
mkdir "../Golden Sentry Discord IPS/Windows"
mkdir "../Golden Sentry Discord IPS/MacOS"
mkdir "../Golden Sentry Discord IPS/Linux"
echo "Initializing Build"
dotnet build -v diag
dotnet publish --force --self-contained true -c Release -r win-x86 -v diag -o "../../Golden Sentry Discord IPS/Windows/Golden Sentry Discord IPS Windows x86/"
echo "Do you wish to compile for 64 bit ? CTRL+C to quit or"
pause
dotnet publish --force --self-contained true -c Release -r win-x64 -v diag -o "../../Golden Sentry Discord IPS/Windows/Golden Sentry Discord IPS Windows x64/"
dotnet publish --force --self-contained true -c Release -r linux-x64 -v diag -o "../../Golden Sentry Discord IPS/Linux/Golden Sentry Discord IPS Linux x64/"
dotnet publish --force --self-contained true -c Release -r osx-x64 -v diag -o "../../Golden Sentry Discord IPS/MacOS/Golden Sentry Discord IPS MacOS x64/"
dotnet publish --force --self-contained true -c Release -r osx.10.10-x64 -v diag -o "../../Golden Sentry Discord IPS/MacOS/Golden Sentry Discord IPS MacOS Yosemite x64/"
dotnet publish --force --self-contained true -c Release -r osx.10.11-x64 -v diag -o "../../Golden Sentry Discord IPS/MacOS/Golden Sentry Discord IPS MacOS El Capitan x64/"
echo "Compiled Successfully, Golden Sentry Discord IPS Ready For deployment on MacOS, Windows and Linux"
pause
exit