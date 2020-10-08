cd %~dp0
@echo off
cls
call make-package YZ.Helpers
call make-package YZ.Helpers.EFCore
call make-package YZ.Helpers.FFMpeg
call make-package YZ.Helpers.WebRTC
call make-package YZ.Helpers.Win32

pause