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
let evaluation poolNum rowNum (rows : Row array) =
   // capa[pool][row] contains the cpacity for all the pool*row
   let capa = Array.init poolNum (fun _ -> Array.create rowNum 0)
   for r = 0 to rowNum - 1 do 
      let row = rows.[r]
      for slot in row do 
         for server in slot.servers do 
            capa.[server.pool].[r] <- capa.[server.pool].[r] + server.capa
   capa
   |> Array.map (fun capaP -> (Array.sum capaP) - (Array.max capaP) )
   |> Array.min

//-------------------------------------------------------------------------------------------------
// MAIN

[<EntryPoint>]
let main argv =
   // import
   let inPath = "../dc.in"
   let rows,servers,poolNum = import inPath
   // solution
   let newRows = solutionGreedy rows servers poolNum
   // evaluation
   let score = evaluation poolNum rows.Length newRows
   printfn "score : %d" score
   //export 
   export "../output.txt" poolNum rows.Length servers.Length newRows
   0 // return an integer exit code
