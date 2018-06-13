namespace Weather.Models

module View =
    
    open Domain
    
    type TempViewModel = {
        Provider : string
        Temp : string
    } with
        static member Create (domain : Temp) =
            {
                Provider = match domain.Provider with
                           | Yr -> "Yr"
                           | Storm -> "Storm"
                           | Sql -> "Sql"
                Temp = domain.TempString
            }