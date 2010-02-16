; setup.nsi
;
!include WordFunc.nsh
!insertmacro VersionCompare
 
!include LogicLib.nsh

!include MUI.nsh
;--------------------------------

!define WNDCLASS "wndclass_desked_gsk"

!ifndef VERSION
!error "VERSION is not undefined"
!endif

; The name of the installer
Name "Atlassian Connector For Visual Studio"

LicenseData LICENSE

; The file to write
OutFile "plvs\bin\Release\plvs-setup.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\Atlassian Connector For Visual Studio"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Atlassian\Plvs" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

Function .onInit
	Call CheckVS
	Call CheckDotNET35
	Call CheckDotNET20
	FindWindow $0 "${WNDCLASS}"
	StrCmp $0 0 +3
	    MessageBox MB_ICONSTOP|MB_OK "Visual Studio is running. Close it and try again."
		Abort
FunctionEnd

Function un.onInit
	FindWindow $0 "${WNDCLASS}"
	StrCmp $0 0 +3
	    MessageBox MB_ICONSTOP|MB_OK "Visual Studio is running. Close it and try again."
		Abort
FunctionEnd

Function CheckDotNET20
	ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" "Install"
	IfErrors 0 +3
		MessageBox MB_ICONSTOP|MB_OK ".NET Framework 2.0 is not installed"
		Abort
FunctionEnd

Function CheckDotNET35
	ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Install"
	IfErrors 0 +3
		MessageBox MB_ICONSTOP|MB_OK ".NET Framework 3.5 is not installed"
		Abort
FunctionEnd

Function CheckVS
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors 0 +3
		MessageBox MB_ICONSTOP|MB_OK "Visual Studio 2008 is not installed"
		Abort
FunctionEnd

Function LaunchVS
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
  Exec "$0\devenv.exe"
FunctionEnd

; Pages

!define MUI_ICON plvs\Resources\icons\ide_plugin_32.ico
!define MUI_UNICON plvs\Resources\icons\ide_plugin_32.ico
!define MUI_WELCOMEFINISHPAGE_BITMAP "plvs\Resources\icons\atlassian-installer.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "plvs\Resources\icons\atlassian-installer.bmp"
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE LICENSE
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
 
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_CHECKED
!define MUI_FINISHPAGE_RUN_TEXT "Start Visual Studio"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchVS"
!define MUI_FINISHPAGE_NOAUTOCLOSE
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

;--------------------------------

; The stuff to install
Section "Atlassian Connector For Visual Studio (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File "plvs\bin\Release\plvs.dll"
  File "plvs\bin\Release\Ankh.ExtensionPoints.dll"
  File "plvs\bin\Release\Aga.Controls.dll"
  File "plvs\bin\Release\edit.png"

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Atlassian\Plvs "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "DisplayName" "Atlassian Connector For Visual Studio"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "DisplayVersion" "${VERSION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "Publisher" "Atlassian"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "" "#110"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "ProductDetails" "#112"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "PID" "1.0"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "LogoID" "#600"
  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "UseInterface" 1
  
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "" "Atlassian.plvs.PlvsPackage, plvs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "InprocServer32" "$SYSDIR\mscoree.dll"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "Class" "Atlassian.plvs.PlvsPackage"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "CodeBase" "$INSTDIR\plvs.dll"
  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "ID" 1
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "MinEdition" "Standard"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "ProductVersion" "1.0"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "ProductName" "Atlassian Connector for Visual Studio"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "CompanyName" "Atlassian"

  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\AutoLoadPackages\{ADFC4E64-0397-11D1-9F4E-00A0C911004F}" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" 0

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Menus" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" ", 1000, 1"

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "Name" "Atlassian.plvs.AtlassianToolWindow"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "Orientation" "Bottom"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "Style" "Tabbed"
  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "DontForceCreate" 1

  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}\Visibility" "{f1536ef8-92ec-443c-9ed7-fdadf150da82}" 0

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "Name" "Atlassian.plvs.IssueDetailsToolWindow"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "Orientation" "Bottom"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "Style" "Tabbed"
  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "DontForceCreate" 1

  WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}\Visibility" "{f1536ef8-92ec-443c-9ed7-fdadf150da82}" 0

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Services\{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Services\{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}" "Name" "JIRA Link Service"

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "" "JIRA Links (Margin)"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "DisplayName" "JIRA Links (Margin)"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "Service" "{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}"

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "" "JIRA Links (Text)"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "DisplayName" "JIRA Links (Text)"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "Service" "{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}"

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "" "Atlassian JIRA Connector"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "Service" "{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"

  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "" "#113"
  WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "Package" "{36FA5F7F-2B5D-4CEC-8C06-10C483683A16}"

  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
  ExecWait '"$0\devenv.exe" /setup'
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Atlassian Connector For Visual Studio"
  CreateShortCut "$SMPROGRAMS\Atlassian Connector For Visual Studio\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs"
  DeleteRegKey HKLM SOFTWARE\Plvs
  
  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage"
  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"

  DeleteRegValue HKLM "Software\Microsoft\VisualStudio\9.0\AutoLoadPackages\{ADFC4E64-0397-11D1-9F4E-00A0C911004F}" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" 

  DeleteRegValue HKLM "Software\Microsoft\VisualStudio\9.0\Menus" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"

  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}"

  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}"

  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Services\{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}"

  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}"
  
  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}"
  
  DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
  
  ; Remove files and uninstaller
  Delete $INSTDIR\plvs.dll
  Delete $INSTDIR\Ankh.ExtensionPoints.dll
  Delete $INSTDIR\Aga.Controls.dll
  Delete $INSTDIR\edit.png

  Delete $INSTDIR\uninstall.exe

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Atlassian Connector For Visual Studio\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\Atlassian Connector For Visual Studio"
  RMDir "$INSTDIR"

  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
  ExecWait '"$0\devenv.exe" /setup'

SectionEnd