echo off

set /P APPVEYOR_BUILD_NUMBER=Please enter a build number (e.g. 134):
set /P PACKAGE_VERISON=Please enter your package version (e.g. 1.0.5):
set /P UMBRACO_PACKAGE_PRERELEASE_SUFFIX=Please enter your package release suffix or leave empty (e.g. beta):

set /P APPVEYOR_REPO_TAG=If you want to simulate a GitHub tag for a release (e.g. true):

if "%APPVEYOR_BUILD_NUMBER%" == "" (
  set APPVEYOR_BUILD_NUMBER=100
)
if "%PACKAGE_VERISON%" == "" (
  set PACKAGE_VERISON=0.1.0
)

set APPVEYOR_BUILD_VERSION=%PACKAGE_VERISON%.%APPVEYOR_BUILD_NUMBER%

build.bat

exit