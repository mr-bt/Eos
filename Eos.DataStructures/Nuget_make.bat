mkdir bin\Package\

..\NuGet.exe pack Eos.DataStructures.csproj -Prop Configuration=Release -Symbols -OutputDirectory bin\Package

pause