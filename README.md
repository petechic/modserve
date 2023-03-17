# modserve

Modserve is a Modbus TCP to SQL Server historian, with IEEE 754 conversions.

.Net 4.8 application

This is a C# application that allows archiving (historically) data from a Modbus TCP device to SQL Server. The user can enter a connection string, sample rate, IP address/Port (of Modbus device), and click "START" to start the server. The following features have been added, so far:

a. Threaded

b. 96 total registers being stored from the Modbus TCP device. I use Mod_RSim (Version 2) simulator to simulate data being read/written.

c. 19 extra registers are being converted into IEEE 754 values from the original Modbus map (double and quadruple precision).

d. The SQL Server is by default the localDB instance on the user's machine. For example, the default connection string in tne interface points to the default installation of SQL Server Express 2022. All that is needed is to add a new database called "HISTORY" (plus run the dbo.History.SQL query on the database).

e. Sample rate can be throttled down to 1 sample (of the full map) per second, with all the IEEE 754 registers being converted.

f. When you STOP the server, the app closes (to destroy connections to the Modbus device cleanly, so far).

g. Updated to work with Visual Studio 2022, SQL Server Express 2022, Mod Sim 2 (ModRSim2.exe), and .Net 4.8.

I hope you like it. Please, let me know what you think.

Pete
