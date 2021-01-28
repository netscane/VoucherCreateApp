@echo off
if exist %windir%\SysWOW64 (
 C:\Windows\SysWOW64\Regsvr32  %~dp0\K3RptVCBoot.dll
)else (
 Regsvr32.exe  K3RptVCBoot.dll
)


