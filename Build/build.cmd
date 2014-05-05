@ECHO OFF
call npm install
@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe"  "%~dp0\node_modules\grunt-cli\bin\grunt" %*
) ELSE (
  node  "%~dp0\node_modules\grunt-cli\bin\grunt" %*
)