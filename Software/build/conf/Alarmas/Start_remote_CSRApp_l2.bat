ECHO OFF 
set PATH=D:\Setup\PSTools\;%PATH%
pskill \\CAMCONTROL02 -t -u Roboconcept -p R7D-t=KM  CSRApp_client.exe
psexec \\CAMCONTROL02 -u Roboconcept -p R7D-t=KM  -i 1 -w D:\ROBOCONCEPT\CSR_SPECIES_CLIENT D:\ROBOCONCEPT\CSR_SPECIES_CLIENT\run_app.bat



