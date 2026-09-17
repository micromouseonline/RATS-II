The ZIP folder contains the following files:

1. accessruntime_4288-1001_x86_en-us.exe	Access 2016 runtime for users without Access 2016 licence
2. AccessDatabaseEngine.exe			Connector for Access 2016 required for some versions of Windows to connect Application to DB
3. RATS.exe					Application executable version 4.0.1a
4. RATS_Competitions_be.accdb			The data tables of the access database (be = "back end")
5. RATS_DB_Front_end.accdb			The access application to maintain the database & record entries
6. RATS_User_Guide_for_v3p9p1_db3p3.pdf		User guide covering the RATS timing supervisor and the RATS database
7. preferredMessageSequences.pdf		A description of the preferred usage of RATS <-> Timing Gates messages including example state machines
8. exampleTimer.zip				Example timer code for Arduino Nano supporting 3,2 or 1 gate function

If you are building timing gates, document 7 sets out the preferred usage of messages and is strongly recommended that you are familiar with the contents of this document.

A. Quick Start
RATS can be installed simply by copying the executable (File 3) to your PC and opening it from there.

RATS can run in standalone mode and make no connection to a database. The supervisor will be restricted to practice mode to display individual runs without storing results this can be more than enough for practice and demonstration sessions and for testing gates.

RATS will put 2 logfiles onto your desktop one containing date and time stamped audit of all the messages between RATS and the connected gates, the second is a verbatim log of characters received from the gates.

To have the full function of displaying contestant entries and storing run times RATS requires you to define a network drive "R:" which can point anywhere on your PC. 

B. Full Installation
If you don't have Access on your PC you need to install Access 2016 runtime which is free to distribute (File 1)
Some versions of Windows require the database connector to be installed (File 2). A recent install on Windows 11 did not require this step but you may need this even if you have Access
The RATS executable (File 3) can be stored anywhere you like on your PC and you can open it from there
The back end database (File 4) must be located on R:
The front end access application (File 5) can be located anywhere on your PC it will look for the backend on R:

Windows 11 has introduced several layers of protection for files and shared access. To be able to map the network drive R: to a folder on your PC:
.	Network discovery must be enabled
.	The folder that you are mapping must be set as shareable (with yourself)
The access front end (File 5) must be unblocked (Win11 will have detected it as coming from another computer so needs to be unblocked in the file properties)

In access:
.	Macros must be enabled
.	The database location should be added as a trusted location

C. Documentation
Document 6 is slightly out of date but still give a pretty good idea of how to drive the applications - the supervisor has multiple windows available: 

C.1	main control window - opens when the application is launched and is the window that controls everything
C.2	Channel Display window - toggled on and off by the buttons for different aspect ratios on the control window.
	This is the main timing window that is projected to competitors and the audience
C.3	1 Channel Results window - for display at the end of a challenge 
C.4	Calibration window - for timing gates that support the calibration messages shows the gate sensor readings



