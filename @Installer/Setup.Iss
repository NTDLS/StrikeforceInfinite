#define Version "0.0.42"

[Setup]
;-- Main Setup Information
 AppName                         = Axis Engine
 AppVerName                      = Axis Engine {#Version}
 AppCopyright                    = Copyright © 1995-2026 NetworkDLS
 DefaultDirName                  = {commonpf}\NetworkDLS\Axis Engine
 DefaultGroupName                = NetworkDLS\Axis Engine
 UninstallDisplayIcon            = {app}\Ae.Client.exe
 SetupIconFile                   = ..\@Art\Branding\AeIcon.ico
 PrivilegesRequired              = admin
 Uninstallable                   = Yes
 Compression                     = ZIP/9
 ChangesAssociations             = Yes
 OutputBaseFilename              = Ae.Installer-{#Version}
 ArchitecturesInstallIn64BitMode = x64os
 ArchitecturesAllowed            = x64os

;-- Windows 2000 & XP (Support Dialog)
 AppPublisher    = NetworkDLS
 AppPublisherURL = http://www.NetworkDLS.com/
 AppUpdatesURL   = http://www.NetworkDLS.com/
 AppVersion      = {#Version}
 
[Components]
 Name: "Client";       Description: "Axis Engine";    Types: Full Compact Custom; Flags: Fixed;
 Name: "Explorer";     Description: "Asset Explorer"; Types: Full Compact Custom; Flags: Fixed;
 Name: "Assets";       Description: "Assets";         Types: Full Compact Custom; Flags: Fixed;
 Name: "FontAsset";    Description: "Fonts";          Types: Full Compact Custom; Flags: Fixed;

 [Files]
 Source: "publish\Client\*.*";                 DestDir: "{app}"; Components: Client;   Flags: IgnoreVersion; 
 Source: "publish\AssetExplorer\*.*";          DestDir: "{app}"; Components: Explorer; Flags: IgnoreVersion; 
 Source: "Ae.Assets.db";                       DestDir: "{app}"; Components: Assets;   Flags: IgnoreVersion recursesubdirs;

 Source: "Fonts\Digital-7 (Mono).ttf";         DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Digital-7 Mono";  Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Digital-7.ttf";                DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "digital-7";       Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Orbitronio.ttf";               DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Orbitronio";      Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Regular.ttf";           DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto";          Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\Roboto-Thin.ttf";              DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Roboto Th";       Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\visitor1.ttf";                 DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Visitor TT1 BRK"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCode.ttf";             DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code Bold"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCodeBold.ttf";         DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code Bold"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCodeExtraLight.ttf";   DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code ExtraLight"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCodeLight.ttf";        DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code Light"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCodeRegulat.ttf";      DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code Regular"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCodeSemiBold.ttf";     DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code SemiBold"; Flags: onlyifdoesntexist uninsneveruninstall
 Source: "Fonts\CascadiaCodeSemiLight.ttf";    DestDir: "{commonfonts}"; Components: FontAsset; FontInstall: "Cascadia Code SemiLight"; Flags: onlyifdoesntexist uninsneveruninstall

[Icons]
 Name: "{group}\Axis Engine";   Filename: "{app}\Ae.Client.exe";        Components: Client;
 Name: "{group}\Axis Explorer"; Filename: "{app}\Ae.AssetExplorer.exe"; Components: Explorer;

[Run]
 Filename: "{app}\Ae.Client.exe"; Description: "Run Axis Engine now?"; Flags: postinstall nowait skipifsilent shellexec; Components: Client;
