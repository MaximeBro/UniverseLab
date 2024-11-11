# UniverseLab

## uDrive

### Project setup
Default appsettings.json
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "Files": {
    "ParentFolder": "../stored-data/files",
    "MaxStorage": 2000000,
    "MaxSubFolders": 5
  },
  "ConnectionStrings": {
    "MainDb": "Data Source=../stored-data/db/main.db"
  },
  "Google": {
    "Instance": "https://accounts.google.com/o/oauth2/v2/auth",
    "ClientId": "",
    "ClientSecret": "",
    "CallbackPath": "/signin-google"
  },
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5005"
      },
      "Http": {
        "Url": "http://localhost:5004"
      }
    }
  }
}
```

Default solution folders
```yaml
/ProjectSolution
    /uDrive
    /stored-data
        /db       # main.db here
        /files    # client folders here
    ...
```