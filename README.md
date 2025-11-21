# CodespaceTest
testing github codespaces
Michael was here

<details>
  <summary>Click to expand</summary>

## Command to auto publish and commit code ##

`dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
mv ./Gaiia_Automation_Test/bin/Release/net8.0/win-x64/publish/Gaiia_Automation_Test.exe ./Program
git add .
git commit -m "Automated commit"
git push`

</details>

