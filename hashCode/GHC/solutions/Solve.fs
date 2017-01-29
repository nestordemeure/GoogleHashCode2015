module GHC.Solve

open ExtCore.Collections

open GHC.Extensions
open GHC.Extensions.Common
open GHC.Domain

//-------------------------------------------------------------------------------------------------
// GREEDY SERVEUR INSERTION

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
// DEALS WITH POOLS ONLY

let solutionPools poolNum rowNum servers =
   let pools = Array.init poolNum (fun _ -> Array.create rowNum 0)
   let servers = Array.sortByDescending (fun se -> se.capa) servers // best first
   [|
      for server in servers do 
      let pool = Array.minBy capaOfPool pools
      let poolIndex = Array.findIndex ((=) pool) pools
      pool.[server.row] <- server.capa
      yield {server with pool=poolIndex }
   |]

//-------------------------------------------------------------------------------------------------
// SOLUTION

/// a rough greedy solution
let solutionGreedy (rows : Row array) (servers:Server array) poolNum =
   let newRows = Array.copy rows
   // put servers in the rows
   servers
   |> Array.sortByDescending (fun se -> se.size) // bigger first
   |> Array.iter (insertServer newRows)
   // put pools in the servers
   newRows
   |> serveursOfRows
   |> solutionPools poolNum rows.Length



