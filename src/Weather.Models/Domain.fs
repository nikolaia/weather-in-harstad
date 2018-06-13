namespace Weather.Models

module Domain =
    
    type TempProvider =
    | Yr
    | Storm
    | Sql
    with
        member this.ProviderString =
            match this with
            | Yr -> "Yr"
            | Storm -> "Storm"
            | Sql -> "Sql"
   
    type Temp = {
        Provider : TempProvider
        Temp : decimal option
    } with 
        member this.TempString = 
            match this.Temp with
            | Some t -> sprintf "%G°" t
            | None -> "-"