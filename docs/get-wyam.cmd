ECHO OFF
..\src\.nuget\NuGet.exe install Wyam -Pre -OutputDirectory .wyam
CD .wyam\Wyam*\tools
XCOPY /E /H /Y /C *.* ..\..
CD ..
RMDIR tools /S /Q
DEL *.nupkg /Q
CD ..\..
ECHO ON

REM .wyam\Wyam.exe --preview

@IF %ERRORLEVEL% NEQ 0 GOTO err
@EXIT /B 0
:err
@PAUSE
@EXIT /B 1