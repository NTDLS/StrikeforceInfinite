@echo off

dotnet pack ..\Ae.Engine -c Release -o NugetPublish
dotnet pack ..\Ae.MpClientToServerComms -c Release -o NugetPublish
dotnet pack ..\Ae.MpCommsMessages -c Release -o NugetPublish

pause
