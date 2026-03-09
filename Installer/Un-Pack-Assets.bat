@echo off

rd AssetPacker /q /s
md AssetPacker

dotnet publish ..\Ae.AssetPacker -c Release -o AssetPacker --runtime win-x64 --self-contained false

.\AssetPacker\Ae.AssetPacker.exe -unpack -d ..\Assets -db .\Ae.Assets.db
pause
