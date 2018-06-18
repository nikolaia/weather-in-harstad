open System.IO
#r "paket:
nuget Fake.Core.Target 
nuget Fake.Azure.Kudu //"
// Use this for IDE support. Not required by FAKE 5.
#load ".fake/deploy.fsx/intellisense.fsx"
open Fake.Core
open Fake.Azure
open Fake.Core.TargetOperators
    
Target.create "KuduStageFiles" <| fun tp ->
    tp.Context.Arguments
    |> Seq.map (fun arg -> arg.Split('=') |> fun split -> split.[0], split.[1])
    |> Map.ofSeq<string,string>
    |> Map.tryFind "stageFolder"
    |> function
        | Some app -> Kudu.stageFolder app (fun _ -> true) // TODO: use build/app locally and /app on kudu :thinking_face:
        | None -> failwith "Please provide a folder to stage as the only argument of the script (fake run ./deploy.fsx stageFolder=/app)"

Target.create "Deploy" <| fun _ ->  Kudu.kuduSync()

"KuduStageFiles"
    ==> "Deploy"

Target.runOrDefaultWithArguments "Deploy"