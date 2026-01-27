@echo off
set path=%PATH%;C:\Program Files\7-Zip;
set path=C:\Program Files (x86)\Inno Setup 6\;%path%

rd publish /q /s
rd output /q /s

md output
md publish

call PackAssets.bat

dotnet publish ..\Si.Client -c Release -o publish --runtime win-x64 --self-contained false
del publish\*.pdb /q

iscc Setup.Iss
rd publish /q /s

pause
