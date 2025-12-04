# CodespaceTest
Testing github codespaces

<details>
  <summary>Command to auto publish and commit code</summary>

  `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
  mv ./Gaiia_Automation_Test/bin/Release/net8.0/win-x64/publish/Gaiia_Automation_Test.exe ./Program
  git add .
  git commit -m "Automated commit"
  git push`

</details>

<details>
  <summary>List of keywords for generated messages</summary>
  <ul>
    <li>#ACCNUM - AccountNumber (123456)</li>
    <li>#FNAME - FirstName      (John)</li>
    <li>#LNAME - LastName       (Doe)</li>
    <li>#PHONE - PhoneNumber    (123-456-7890)</li>
    <li>#SUB - Subsciption      (500Mbps. for $75/Month)</li>
    <li>#LOC - Address          (123 test, place, FL 12345, USA)</li>
    <li>#TIME - InstallTime     (Tuesday Nov. 25th, 11:00am - 2:00pm)</li>
  <ul>

</details>