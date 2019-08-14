# GetMeOntoTonberry
Notifies me when I can make a character on FFXIV's Tonberry Server

# Instructions
* Clone the project
* Compile with Visual Studio 2017 (Community is ok)
* Create an EmailConfiguration.json file with the following structure
  Note that only gmail accounts are supported for now
  ```json
  {
	"sender": 
	{
		"email": "sender@gmail.com",
		"password": "********"
	},
	"receiver":
	{
		"email": "receiver@gmail.com"
	}
}
  ```
* Place EmailConfiguration.json in the same folder as GetMeOntoTonberry.exe
* Run the compiled GetMeOntoTonberry.exe

# Optional configuration
Change the serverName variable at the top of the Program class to a different server to check its availability
