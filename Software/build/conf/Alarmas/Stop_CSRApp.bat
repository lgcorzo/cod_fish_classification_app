
@echo off
title switchin off the application....
ECHO "This workstation will log off automatically when installation is complete."
START /WAIT /B "" "taskkill.exe" /f /im CSRApp.exe 
START /WAIT /B "" "pskill.exe" \\CAMCONTROL02 -t -u Roboconcept -p R7D-t=KM  CSRApp_client.exe
START /WAIT /B "" "pskill.exe" \\CAMCONTROL01 -t -u Roboconcept -p R7D-t=KM  CSRApp_client.exe
ECHO "Finished"
EXIT