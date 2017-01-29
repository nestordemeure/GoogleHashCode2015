module GHC.Domain

open ExtCore.Collections
open System.Collections.Generic

open GHC.Extensions
open GHC.Extensions.Common

//-------------------------------------------------------------------------------------------------
// CAPACITY

/// used to represent a capacity
type Capa = { garan : int ; local : int array array ; full : int}

//-----

/// create an empty capa
let emptyCapa poolNum rowNum =
   let loc = Array.init poolNum (fun _ -> Array.create rowNum 0)
   {garan = 0 ; local = loc ; full = 0}

/// takes a pool and returns its garanted capacity
/// a pool is represented as an array of row, a row is just the sum of the capacity of its servers
let capaOfPool (pool : int array) = (Array.sum pool) - (Array.max pool)

/// update a capacity by adding a server to a given pool*row
let updateCapa row pool capaServer capa =
   let loc = Array.init capa.local.Length (fun p -> Array.copy capa.local.[p])
   loc.[pool].[row] <- loc.[pool].[row] + capaServer
   let mutable garan = System.Int32.MaxValue
   for p = 0 to capa.local.Length - 1 do 
      garan <- min garan (capaOfPool loc.[p])
   {garan = garan ; local = loc ; full = capa.full + capaServer}

//-------------------------------------------------------------------------------------------------
// INPUT

type Server = 
   { 
      id : int
      size : int ; capa : int
      pool : int ; row : int ; slot : int
   }

type Interval = { index : int ; length : int ; servers : Server list }

type Row = Interval list

//-----

/// take some rows and extract the servers (while updating their informations)
let serveursOfRows (rows : Row array) =
   [|
      for r = 0 to rows.Length - 1 do 
      let row = rows.[r]
      for interval in row do 
         let mutable slotI = interval.index
         for server in interval.servers do 
            yield { server with row = r ; slot = slotI}
            slotI <- slotI + server.size
   |]
