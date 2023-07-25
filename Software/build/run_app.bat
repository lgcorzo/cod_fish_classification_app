ECHO OFF
start /B  schtasks /End /TN CSRApp_server
start /B  schtasks /Run /TN CSRApp_server
