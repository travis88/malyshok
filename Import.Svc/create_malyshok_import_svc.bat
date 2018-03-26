

sc create malyshok_import_svc binPath= "D:\www\malyshok\Import.Svc\bin\Release\Import.Svc.exe" start=auto DisplayName=malyshok_import_svc
sc start malyshok_import_svc

pause