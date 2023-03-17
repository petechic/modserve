# ModServe9
Modbus TCP to SQL Server Historian

.Net application

This is a basic application that allows archiving (historically) data from a Modbus TCP device
to SQL Server. The user can enter a connection string, sample rate, IP
address/Port (of Modbus device), and click "START" to start the
server. The following features have been added, so far:

a. Threaded

b. 96 total registers
being stored from the Modbus TCP device. I use Mod_RSim simulator to simulate
data being read/written.

c. 19 extra registers
are being converted into IEEE 754 values from the original Modbus map (double and
quadruple precision).

d. The SQL Server is by default the localDB instance on the user's machine.

e. Sample rate can be throttled down to 1 sample (of the full map) per second, with all the IEEE 754
registers being converted.

f. When you STOP the server, the app closes (to destroy connections to the Modbus device cleanly, so
far).


To be added:

a. Basic monitoring grid
(maybe, although I am finishing a full separate app for reading/writing
registers, and GUI)

b. Building the app into a Windows Service.

c. ASP.Net Core migration
for cross-platform. 


I hope you like it.
Please, let me know what you think.

 

       Pete

