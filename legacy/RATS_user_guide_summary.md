# RATS (Registration And Timing System) User Guide Summary
**Software Version:** v4.0.1 | **Database Version:** v3.3

---

## 1. System Overview
The system consists of two integrated components:
* **RATS_DB_Front_end (MS Access):** Handles the management of events, robots, contestants, and competition sequencing.
* **RATSxxx.exe (Visual Studio):** Interfaces with the physical timing gates to capture and record run times.

---

## 2. Setup: Registration & Event Management (RATS_DB_Front_end)

### Event Initialization
1.  Open **RATS_DB_Front_end** and navigate to the **Context Window**.
2.  **To create a new event:** Click **NEW EVENT** and enter the name/date.
3.  **To select an event:** Use the record navigation arrows at the bottom of the form to find your event.
4.  **Refresh:** Click **Refresh** to update the list if changes aren't visible.

### Registering Participants
1.  Click **Record Entries** on the Context Form.
2.  **Select Robot:** Choose from the dropdown. If it is a new robot, click **New Robot** to enter its details first.
3.  **Enter Contestant:** Link a person to the robot record.
4.  **Assign Contest:** Select the competition (e.g., Drag Race, Line Follower) and click **Enter Robot**.

### Managing Run Order
1.  Click **Sequence Entries** on the Context Form.
2.  Choose the specific contest from the list.
3.  Assign ascending numbers in the **Sequence** column to determine the starting order.

---

## 3. Operation: Timing & Scoring (RATS_xxx.ex)

### Connection
1.  Connect timing hardware via USB.
2.  Launch **RATS_xxx.ex**.
3.  Select the correct **Serial Port** and click **Connect**.
4.  Verify status: The "Timing Gates State" should show "Waiting start present."

### Competition Workflow
1.  **Mode Toggle:** Click **Practice <> Contest** to switch from practice to official scoring.
2.  **Selection:** Choose the **Competition Class** (Heats/Finals), the **Contest**, and then the specific **Robot** on the line.
3.  **Timing:** * The clock starts automatically when the start gate is triggered.
    * **Touches:** If a robot is touched (penalty), click the **Add Touch** button.
4.  **Finalizing Results:**
    * **Store Best Score:** When all runs for an entry are done, click this to save the best time to the database.
    * **DNF:** If the robot fails to finish, click the **DNF** button to record the failure.

---

## 4. Results & Reporting

### Live Displays
* Click the **Results** button in RATS_xxx.ex to open the live leaderboard window.
* Drag this window to a secondary monitor or projector for the audience.

### Exporting Data
From the **RATS_DB_Front_end Context Form**, you can generate:
* **Scores Reports:** Official printed standings.
* **Excel Exports:** For deep data analysis.
* **Markdown Exports:** Formatted specifically for posting results to websites (e.g., WordPress).

### Recovery
* If the database connection fails, check the **Message Log** text file created on the PC desktop. Every gate trigger and time is logged there as a backup.