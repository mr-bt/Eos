mkdir bin\Package\

..\NuGet.exe pack Eos.Utils.csproj -Prop Configuration=Release -Symbols -OutputDirectory bin\Package

pause