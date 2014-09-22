if not exist "builds\nuget" mkdir "builds\nuget"
pushd %project_root%
nuget pack "src\Mek.Glob\Mek.Glob\Mek.Glob.csproj" -OutputDirectory "builds\nuget"  -Prop Configuration=Release -Symbols
popd