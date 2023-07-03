# VeilBlockLogger
Saves Veil blockchain data to a database.
Processes the data then uploads it to veil-stats.com

## Setup
- Create a Microsoft SQL database
- Create the schema, tables and store procs using the scripts in the SqlScripts folder
- Update the app.config with the database connection string
- Also in the app.config set the ftp url, username and password to upload the json data files to veil-stats server. 

## Local Veil Node Setup
- Config a local veil node to accept RPC requests.
- Username: veiluser
- Password: veilrpcpassword
- URL 127.0.0.1:58812


