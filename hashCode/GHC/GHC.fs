module GHC.Main

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain
open GHC.Import
open GHC.Solve
open GHC.Export
open System.Collections.Generic

//-------------------------------------------------------------------------------------------------
// EVALUATION

/// returns the garanted capacity for the given rows
let evaluation poolNum rowNum (servers : Server array) =
   // capa[pool][row] contains the capacity for all the pool*row
   let capa = Array.init poolNum (fun _ -> Array.create rowNum 0)
   for server in servers do 
      capa.[server.pool].[server.row] <- capa.[server.pool].[server.row] + server.capa
   capa
   |> Array.map capaOfPool
   |> Array.min

//-------------------------------------------------------------------------------------------------
// MAIN

[<EntryPoint>]
let main argv =
   // import
   let inPath = "../dc.in"
   let rows,servers,poolNum = import inPath
   // solution
   let newServers = solutionTwoPhases rows servers poolNum
   // evaluation
   let score = evaluation poolNum rows.Length newServers
   printfn "score : %d" score
   //export 
   export "../output.txt" poolNum rows.Length servers.Length newServers
   0 // return an integer exit code
