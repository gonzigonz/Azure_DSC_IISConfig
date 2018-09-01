configuration IISConfig {
   Node TestWebServerVM {
      WindowsFeature IIS {
         Ensure               = 'Present'
         Name                 = 'Web-Server'
         IncludeAllSubFeature = $true
      }
      File DirectoryCreated {
            Ensure = 'Present'
            Type = 'Directory'
            DestinationPath = 'C:\Users\All Users\Desktop\gonzwashere.txt'
      }
   }
}