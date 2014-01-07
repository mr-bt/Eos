mkdir bin\Package\

..\NuGet.exe pack Eos.Atomic.csproj -Prop Configuration=Release -Symbols -OutputDirectory bin\Package

pause