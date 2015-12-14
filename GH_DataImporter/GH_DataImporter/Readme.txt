This is a file containing some code of mine that I typed up recently for a Database project. Basically, the executable searches in its current folder for CSV files and parses them - uploading the transactions via MySQL to a database. An example CSV file can be found in the debug folder for the project. 

This is code that will be used by a real client right here in Columbia, Missouri. It was done to make it easier for our client to sync their database with our web application's database. The CSV files are exported directly from their service and can be parsed and uploaded in a couple of clicks. This application was necessary because no API currently exists to extract information "behind the scenes" from the service they currently are using to manage their information. In other words, it is a nice, yet temporary, fix.

The application does the following things:

1) Finds files in the current folder
2) Parses the files into memory
3) Queries our database to find missing information and fills it in
	- This includes information needed to prevent duplicate transactions from being uploaded
4) Connects to our database and uploads the information

*Please note that for our web application, the data is being scrambled a bit with a random number generator to prevent the actual yearly income for the company from being displayed to our class during our upcoming demonstration.

*In addition, please note that this application will not execute directly because I stripped out the actual credentials for the web server we are connecting to. I figured you were far more interested in the code, but I can assure you that it works great.