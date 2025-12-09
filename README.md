# Program Overview

This program streamlines workflows that involve copying data from an Excel spreadsheet and using it across multiple browser tabs. Instead of manually opening pages, navigating to URLs, and copying the required information, the program automates the entire setup process.

---

## What It Does

- **Reads entries from the Daily Instal SP excel sheet**  
  Each row in the spreadsheet represents a record. The program parses each row and extracts the fields needed for your workflow.

- **Automatically opens browser tabs**  
  Based on the data in each entry, the program launches the required browser tabs with the appropriate URLs.

- **Copies relevant info to the clipboard**  
  For each record, the program prepares key data and automatically copies it to your clipboard.  
  This means your main manual step is simply pressing **Ctrl + V** wherever the information needs to go.

- **Greatly reduces repetitive work**  
  No more switching between Excel, links, and copy/paste. Everything is prepped for you.

---

## Benefits

- Faster, more consistent workflow  
- Minimal manual copy/paste  
- Fewer human errors  

---

## Precall Workflow
<details>
  <summary>Click to view workflow details</summary>

### What the Program Does for Each Entry

- Automatically opens the customer’s Gaiia account.
- Updates the clipboard with the customer’s phone number (if available in Excel) so you can paste it directly into NICE.
- Creates the Gaiia note automatically based on the selected call resolution  
  *(no note is created if the appointment was rescheduled or canceled)*.
- Generates an email (when needed) with install details included.
- Produces a results line ready to be pasted back into Excel.
  
---


### Workflow Steps

1. Open the program.

2. Enter your name as it appears in the Excel spreadsheet.  
   This name will be used to fill out the spreadsheet after the program finishes running.

3. Select **Precall Workflow**.

4. Paste the Excel data.  
   - You must include at least columns **A–J** for the data to be considered valid.
   - Install Time must be in the format mm/dd/yyyy, hh:mm:ss

https://github.com/user-attachments/assets/7f45c960-e453-49ff-b295-fa04fe781192

5. Confirm the Vetro ID.

6. Press **Ctrl + V** to paste the phone number into NICE.

7. Select the appropriate call resolution.  
   Send an email if required.

8. Create the note in the customer’s Gaiia account.

9. Repeat steps for each account until all customers have been contacted.

10. At the end of the workflow, paste the generated results into the **Daily Excel SP** spreadsheet.
</details>

---

## Wellness Check Workflow
<details>
  <summary>Click to view workflow details</summary>

### What the Program Does for Each Entry

- Opens the customer’s Gaiia account automatically.
- Loads the Ticket Title, Description, Assigned User, and Category into your clipboard at the correct times so you can paste them directly into the ticket.
- Highlights items that need verification (active account, payment on file, WiFi Man photos, tech notes, PONMON levels, service errors, etc.).
- Automatically navigates PONMON and eero Insight.
- Copies the customer’s phone number into your clipboard for pasting into NICE.
- Generates both **internal** and **external** ticket notes based on your chosen call resolution.
- Prepares the final results line for each customer so it can be pasted into the **Daily Excel SP** spreadsheet at the end of the workflow.

---

### Workflow Steps

1. Open the program.

2. Enter your name as it appears in the Excel spreadsheet.  
   - This name will be used to update the spreadsheet after the program completes.

3. Select **Wellness Check Workflow**.

4. Paste the Excel data.  
   - You must include at least columns **A–J** for the data to be considered valid.  
   - **Install Time** must follow the format: `mm/dd/yyyy, hh:mm:ss`.

5. Paste the **Ticket Title**.

6. Set the **Ticket Type** to **Customer Assistance**.

7. Set the **Ticket Priority** to **Normal**.

8. Paste the **Ticket Description**.

9. Paste the **Assigned User**.

10. Paste the **Category**.

11. Verify the account is **Active**.

12. Verify **Payment** is on file.

13. Select the **Fiber Install Work Order**.

14. Verify **WiFi Man** pictures were uploaded.

15. Verify **Tech Notes** were added.

16. Verify **PONMON Light Levels**.

17. Check for any **Service Errors**.

18. Select the **Customer’s Network**.

19. Enter the **Channel Utilization**.

20. Enter the **Average Utilization**.

21. Enter the **Noise Levels**.

22. Paste the phone number into **NICE**.

23. Enter any **Customer Feedback Notes**.

24. Select the appropriate **Call Resolution**.  
    - Send an email if necessary.

25. Paste the **Internal Note** into the ticket.

26. Paste the **External Note** into the ticket.

27. Repeat these steps for each account until all customers have been contacted.

28. At the end of the workflow, paste the generated results into the **Daily Excel SP** spreadsheet.
</details>

---

## How to Download and Run
1. Click the green code button.
2. Select Download zip.
3. Extract file
4. Open extracted file
5. Open Program folder
6. Run Gaiia_Automation_Test.exe

https://github.com/user-attachments/assets/0101054a-d21f-41f5-af2e-b1726175b1b9

---


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
