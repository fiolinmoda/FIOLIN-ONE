@echo off
chcp 65001 > nul
title FIOLIN ONE Kapat
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0FIOLIN ONE Kapat.ps1" %*
pause

