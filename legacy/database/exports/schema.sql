-- ----------------------------------------------------------
-- MDB Tools - A library for reading MS Access database files
-- Copyright (C) 2000-2011 Brian Bruns and others.
-- Files in libmdb are licensed under LGPL and the utilities under
-- the GPL, see COPYING.LIB and COPYING files respectively.
-- Check out http://mdbtools.sourceforge.net
-- ----------------------------------------------------------

-- That file uses encoding UTF-8

CREATE TABLE `Best_Score_Time`
 (
	`ID`			INTEGER, 
	`Entry_ID`			INTEGER, 
	`Mouse_Name`			varchar, 
	`Contestant_Name`			varchar, 
	`Contestant_Class`			varchar, 
	`Score_Time_mS`			INTEGER, 
	`Run_Time_mS`			INTEGER, 
	`Competition_ID`			INTEGER
);

CREATE TABLE `Challenge`
 (
	`Challenge_ID`			INTEGER, 
	`Challenge_Type`			varchar
);

CREATE TABLE `Competition`
 (
	`Competition_ID`			INTEGER, 
	`Event_ID`			INTEGER, 
	`Competition_Name`			varchar, 
	`Challenge`			varchar, 
	`Competition_Class`			varchar, 
	`Competition_Date`			DateTime, 
	`Scoring_Model_Short_Name`			varchar, 
	`Entry_Time_Limit_Secs`			INTEGER, 
	`Competition_Structure_Name`			varchar, 
	`Number_of_Runs_Allowed`			INTEGER, 
	`Grace_Period_Secs`			INTEGER
);

CREATE TABLE `Competition_Structure`
 (
	`ID`			INTEGER, 
	`Competition_Structure_Name`			varchar
);

CREATE TABLE `Context`
 (
	`ID`			INTEGER, 
	`Effective_Date`			DateTime, 
	`Current_Event`			varchar, 
	`Current_Event_ID`			INTEGER, 
	`Current_Competition_Name`			varchar, 
	`Database_Version`			varchar
);

CREATE TABLE `Entry`
 (
	`Entry_ID`			INTEGER, 
	`Mouse_Name`			varchar, 
	`Competition_ID`			INTEGER, 
	`Entry_Used`			INTEGER, 
	`Sequence_Number`			INTEGER, 
	`Registration_Source`			varchar, 
	`Outcome`			varchar
);

CREATE TABLE `Entry_Run`
 (
	`Entry_Run_ID`			INTEGER, 
	`Entry_ID`			INTEGER, 
	`Run_Time_mSecs`			INTEGER, 
	`Course_Time_mSecs`			INTEGER, 
	`Touches`			INTEGER, 
	`Score_Time_mSecs`			INTEGER, 
	`Date_of_Run`			DateTime, 
	`Time_of_Run`			DateTime, 
	`Manual_Timing`			varchar
);

CREATE TABLE `ETL_Contestant`
 (
	`Contestant_ID`			INTEGER, 
	`Contestant_Name`			varchar, 
	`Class`			varchar, 
	`Contestant_Email_Address`			varchar
);

CREATE TABLE `ETL_Entry`
 (
	`Entry_ID`			INTEGER, 
	`Mouse_Name`			varchar, 
	`Competition_ID`			INTEGER, 
	`Entry_Used`			INTEGER, 
	`Sequence_Number`			INTEGER
);

CREATE TABLE `ETL_Mouse`
 (
	`Mouse_ID`			INTEGER, 
	`Mouse_Name`			varchar, 
	`Contestant_ID`			INTEGER, 
	`Further_Information`			varchar
);

CREATE TABLE `Mouse`
 (
	`Mouse_ID`			INTEGER, 
	`Mouse_Name`			varchar, 
	`Contestant_ID`			INTEGER, 
	`Further_Information`			varchar
);

CREATE TABLE `Robotics_Event`
 (
	`Event_ID`			INTEGER, 
	`Event_Name`			varchar, 
	`Event_Date`			DateTime, 
	`Database_Version`			varchar
);

CREATE TABLE `Scoring_Model`
 (
	`ID`			INTEGER, 
	`Scoring_Model_Short_Name`			varchar, 
	`Scoring_Model_Name`			varchar, 
	`Start_Trigger`			varchar, 
	`End_Trigger`			varchar, 
	`Touches_Enabled`			INTEGER, 
	`Touches_Cumulative`			INTEGER, 
	`Touch_Time_mS`			INTEGER, 
	`Entry_Time_Divider`			INTEGER, 
	`Touch_Time_Divider`			INTEGER, 
	`Touches_Per_Run`			INTEGER
);

CREATE TABLE `Contestant`
 (
	`Contestant_ID`			INTEGER, 
	`Contestant_Name`			varchar, 
	`Class`			varchar, 
	`Contestant_Email_Address`			varchar
);

CREATE TABLE `ETL_seeding_upload`
 (
	`ID`			INTEGER, 
	`Entry_ID`			INTEGER, 
	`Mouse_Name`			varchar, 
	`Competition_ID`			INTEGER, 
	`Sequence_Number`			INTEGER
);


