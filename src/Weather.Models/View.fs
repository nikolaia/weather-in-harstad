namespace Weather.Models

module View =
    
    open Domain
    
    type TemperatureViewModel = {
        Provider : string
        Temp : string
    } with
        static member Create (domain : Temperature) =
            {
                Provider = match domain.Provider with
                           | Yr -> "Yr"
                           | Storm -> "Storm"
                           | Sql -> "Sql"
                Temp = domain.TempString
            }