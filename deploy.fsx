open System.IO
#r "paket:
nuget Fake.Core.Target 
nuget Fake.Azure.Kudu //"
// Use this for IDE support. Not required by FAKE 5.
#load ".fake/deploy.fsx/intellisense.fsx"
open Fake.Core
open Fake.Azure
open Fake.Core.TargetOperators
open Fake.IO

// All of this KuduSync stuff is to get around some bugs until it's fixed in the fake module: https://github.com/fsharp/FAKE/pull/1995 
let kuduPath =
    Process.tryFindFileOnPath "kudusync"
    |> function
        | Some kudusync when File.Exists kudusync -> kudusync |> Path.GetDirectoryName |> DirectoryInfo.ofPath
        | _ -> failwith "Couldn't find az command on PATH" 

/// Synchronises all staged files from the temporary deployment to the actual deployment, removing
/// any obsolete files, updating changed files and adding new files.
let kuduSync() =
    let result =
        Process.execWithResult(fun psi ->
            { psi with
                FileName = Path.Combine(kuduPath.FullName, "kudusync.cmd")
                Arguments = sprintf """-v 50 -f "%s" -t "%s" -n "%s" -p "%s" -i ".git;.hg;.deployment;deploy.cmd" """ 
                                Kudu.deploymentTemp 
                                Kudu.deploymentTarget 
                                Kudu.nextManifestPath
                                Kudu.previousManifestPath })
            (System.TimeSpan.FromMinutes 5.)
    result.Results |> Seq.iter (fun cm -> printfn "%O: %s" cm.Timestamp cm.Message)
    if not result.OK then failwith "Error occurred during Kudu Sync deployment."
    
Target.create "KuduStageFiles" <| fun tp ->
    tp.Context.Arguments
    |> Seq.map (fun arg -> arg.Split('=') |> fun split -> split.[0], split.[1])
    |> Map.ofSeq<string,string>
    |> Map.tryFind "stageFolder"
    |> function
        | Some app -> Kudu.stageFolder app (fun _ -> true) // TODO: use build/app locally and /app on kudu :thinking_face:
        | None -> failwith "Please provide a folder to stage as the only argument of the script (fake run ./deploy.fsx stageFolder=/app)"

Target.create "Deploy" <| fun _ ->  kuduSync()

"KuduStageFiles"
    ==> "Deploy"

Target.runOrDefaultWithArguments "Deploy"