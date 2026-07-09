@echo off
chcp 65001 > nul
title FIOLIN ONE
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0FIOLIN ONE.ps1"
pause

