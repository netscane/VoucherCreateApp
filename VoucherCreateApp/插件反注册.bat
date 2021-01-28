@echo off
if exist %windir%\SysWOW64 (
 C:\Windows\SysWOW64\Regsvr32  /u %~dp0\K3RptVCBoot.dll
)else (
 Regsvr32.exe  /u K3RptVCBoot.dll
)