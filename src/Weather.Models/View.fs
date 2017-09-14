namespace Weather.Models

module View =
    
    open Domain
    
    type TempViewModel = {
        Provider : TempProvider
        Temp : string
    }