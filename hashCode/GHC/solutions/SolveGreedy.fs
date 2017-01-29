module GHC.SolveGreedy

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

//-------------------------------------------------------------------------------------------------
// SERVEUR INSERTION

/// how tightly would the server fit in this interval (returns System.Int32.MaxValue if it cannot fit)
let fitInInterval server interval = 
   if server.size > interval.length then System.Int32.MaxValue else interval.length - server.size

/// how tightly would the server fit in this row (returns System.Int32.MaxValue if it cannot fit)
let fitInRow server (row : Row) = 
   row 
   |> List.map (fitInInterval server)
   |> List.min

//-----

/// take some intervals and a server, put the serveur where it fit the most tightly
let insertServerInRow server row =
   let bestInterval = List.minBy (fitInInterval server) row
   let rec insert row = 
      match row with 
      | [] -> []
      | inter::q when inter <> bestInterval -> 
         inter::(insert q)
      | inter::q ->
         {inter with length = inter.length - server.size ; servers = server::inter.servers}::q
   insert row

/// take some rows and a server, put the serveur where it fit the most tightly
let insertServer rows serveur =
   let bestRow = Array.minBy (fitInRow serveur) rows 
   if fitInRow serveur bestRow <> System.Int32.MaxValue then 
      let newBestRow = insertServerInRow serveur bestRow
      let i = Array.findIndex (fun r -> r = bestRow) rows
      rows.[i] <- newBestRow

//-------------------------------------------------------------------------------------------------
// SOLUTION

/// a rough greedy solution
let solutionGreedy (rows : Row array) (serveurs:Server array) poolNum =
   let rng = System.Random()
   let newRows = Array.copy rows
   serveurs
   |> Array.sortByDescending (fun se -> se.size) // bigger first
   |> Array.map (fun se -> {se with pool = rng.Next(poolNum) }) // random pool, no time to be clever
   |> Array.iter (insertServer newRows)
   newRows



