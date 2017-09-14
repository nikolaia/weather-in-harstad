namespace Weather.Models

module Domain =
    
    type TempProvider =
    | Yr
    | Storm
    | Bestefar
    
    type Temp = {
        Provider : TempProvider
        Temp : decimal option
    } with 
        member this.TempString = 
            match this.Temp with
            | Some t -> sprintf "%G" t
            | None -> "-"