MSBuild.exe ../TAUtil.sln /p:Configuration=Release /t:Clean,Build && python MakeReleaseZip.py --release
@pause
