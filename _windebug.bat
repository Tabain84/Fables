@SET CURPATH=%~dp0

@SET EXENAME=ServUO

@TITLE: %EXENAME% - https://www.servuo.com

::##########

@ECHO:
@ECHO: Compile %EXENAME% for Windows
@ECHO:

@PAUSE

dotnet build -c Debug

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO OFF

"%CURPATH%%EXENAME%.exe" -debug

