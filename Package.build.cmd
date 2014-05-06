C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Package.build.xml

@IF %ERRORLEVEL% NEQ 0 GOTO err
@EXIT /B 0
:err
@PAUSE
@EXIT /B 1