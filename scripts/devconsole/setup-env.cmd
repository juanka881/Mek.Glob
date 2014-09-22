@echo off

rem base dirs
set project_root=%~dp0..\..
set project_tools_root=%project_root%\tools
set project_scripts_root=%project_root%\scripts

rem tool dirs
set nuget_bin=%project_tools_root%\nuget

rem set the path with the tool dirs
set path=%PATH%;%project_scripts_root%;%nuget_bin%

rem set project root as start dir
cd %project_root%

@echo on
