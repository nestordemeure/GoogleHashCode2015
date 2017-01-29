module GHC.Export

open ExtCore.Collections
open System.IO

open GHC.Extensions
open GHC.Domain

//-------------------------------------------------------------------------------------------------
// EXPORTATION

let export path poolNum rowNum serverNum (servers : Server array) =
   let allServers = Array.create serverNum "x"
   for server in servers do 
      allServers.[server.id] <- sprintf "%d %d %d" server.row server.slot server.pool
   File.WriteAllLines(path, allServers)
