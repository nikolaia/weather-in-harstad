#r "paket:
nuget Fake.Core.Target 
nuget Fake.Azure.Kudu //"
// Use this for IDE support. Not required by FAKE 5.
#load ".fake/deploy.fsx/intellisense.fsx"
open Fake.Core
open Fake.Azure
open Fake.Core.TargetOperators
open System.IO
open System
/// The path to the KuduSync application.
let kuduPath = (Environment.environVarOrDefault "GO_WEB_CONFIG_TEMPLATE" ".") |> Path.GetDirectoryName

Target.create "KuduStageFiles" <| fun _ ->
    Kudu.stageFolder "app" (fun _ -> true)

Target.create "Deploy" <| fun _ ->
    let result =
        Process.execWithResult(fun psi ->
            { psi with
                FileName = Path.Combine(kuduPath, "kudusync.cmd")
                Arguments = sprintf """-v 50 -f "%s" -t "%s" -n "%s" -p "%s" -i ".git;.hg;.deployment;deploy.cmd""" 
                    Kudu.deploymentTemp
                    Kudu.deploymentTarget
                    Kudu.nextManifestPath
                    Kudu.previousManifestPath })
            (TimeSpan.FromMinutes 5.)
    result.Results |> Seq.iter (fun cm -> printfn "%O: %s" cm.Timestamp cm.Message)
    if not result.OK then failwith "Error occurred during Kudu Sync deployment."

"KuduStageFiles"
    ==> "Deploy"
    
Target.runOrDefaultWithArguments "Deploy"